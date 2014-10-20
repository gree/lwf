/*
 * Copyright (C) 2014 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#import <OpenGLES/ES2/gl.h>
#import "LWFResourceCache.h"
#import "LWFBitmapRenderer.h"
#import "lwf_core.h"
#import "lwf_data.h"

using namespace std;

namespace LWF {

class Autolock
{
private:
	dispatch_semaphore_t m_semaphore;
public:
	Autolock(dispatch_semaphore_t s) : m_semaphore(s) {
		dispatch_semaphore_wait(m_semaphore, DISPATCH_TIME_FOREVER);
	}
	~Autolock() {
		dispatch_semaphore_signal(m_semaphore);
	}
};

LWFResourceCache *LWFResourceCache::m_instance;

LWFResourceCache *LWFResourceCache::shared()
{
	static dispatch_once_t once;

	dispatch_once(&once, ^{
		m_instance = new LWFResourceCache();
	});

	return m_instance;
}

LWFResourceCache::LWFResourceCache()
{
	m_dataSemaphore = dispatch_semaphore_create(1);
	m_textureSemaphore = dispatch_semaphore_create(1);
}

LWFResourceCache::~LWFResourceCache()
{
#if !OS_OBJECT_USE_OBJC
	dispatch_release(m_dataSemaphore);
	dispatch_release(m_textureSemaphore);
#endif
}

shared_ptr<Data> LWFResourceCache::loadLWFData(const string &pathstr, EAGLContext *context)
{
	{
		Autolock lock(m_dataSemaphore);
		DataCache::iterator it = m_dataCache.find(pathstr);
		if (it != m_dataCache.end()) {
			++it->second.refCount;
			return it->second.data;
		}
	}

	NSString *dataPath;
	if (pathstr[0] == '/') {
		dataPath = [NSString stringWithUTF8String:pathstr.c_str()];
	} else {
		NSString *path = [NSString stringWithUTF8String:pathstr.c_str()];
		NSString *file = [path stringByDeletingPathExtension];
		NSString *ext = [path pathExtension];
		dataPath = [[NSBundle mainBundle] pathForResource:file ofType:ext];
	}
	NSData *nsdata = [NSData dataWithContentsOfFile:dataPath];
	if (!nsdata)
		return shared_ptr<Data>();

	shared_ptr<Data> data =
		make_shared<Data>([nsdata bytes], [nsdata length]);
	if (!data->Check())
		return shared_ptr<Data>();

	string fullPath = [dataPath UTF8String];
	vector<shared_ptr<LWFBitmapRendererContext> > bitmapContexts;
	bitmapContexts.resize(data->bitmaps.size());
	for (size_t i = 0; i < data->bitmaps.size(); ++i) {
		const Format::Bitmap &b = data->bitmaps[i];
		if (b.textureFragmentId == -1)
			continue;

		Format::BitmapEx bx;
		bx.matrixId = b.matrixId;
		bx.textureFragmentId = b.textureFragmentId;
		bx.u = 0;
		bx.v = 0;
		bx.w = 1;
		bx.h = 1;

		bitmapContexts[i] = make_shared<LWFBitmapRendererContext>(
			data.get(), bx, fullPath, context);
	}

	vector<shared_ptr<LWFBitmapRendererContext> > bitmapExContexts;
	bitmapExContexts.resize(data->bitmapExs.size());
	for (size_t i = 0; i < data->bitmapExs.size(); ++i) {
		const Format::BitmapEx &bx = data->bitmapExs[i];
		if (bx.textureFragmentId == -1)
			continue;

		bitmapExContexts[i] = make_shared<LWFBitmapRendererContext>(
			data.get(), bx, fullPath, context);
	}

	{
		Autolock lock(m_dataSemaphore);
		m_dataCache[pathstr] =
			DataContext(data, bitmapContexts, bitmapExContexts);
		m_dataCacheMap[data.get()] = m_dataCache.find(pathstr);
	}

	return data;
}

void LWFResourceCache::unloadLWFData(const shared_ptr<Data> &data)
{
	Autolock lock(m_dataSemaphore);
	DataCacheMap::iterator it = m_dataCacheMap.find(data.get());
	if (it == m_dataCacheMap.end())
		return;

	if (--it->second->second.refCount <= 0) {
		m_dataCache.erase(it->second);
		m_dataCacheMap.erase(it);
	}
}

const LWFResourceCache::DataContext *LWFResourceCache::getDataContext(
	const shared_ptr<Data> &data) const
{
	Autolock lock(m_dataSemaphore);
	DataCacheMap::const_iterator it = m_dataCacheMap.find(data.get());
	if (it == m_dataCacheMap.end())
		return 0;

	return &it->second->second;
}

const LWFTexture *LWFResourceCache::loadTexture(
	const string &dataPath, const string &texturePath, EAGLContext *context)
{
	size_t pos = dataPath.find_last_of('/');
	string basePath;
	if (pos == string::npos)
		basePath = "";
	else
		basePath = dataPath.substr(0, pos + 1);
	string path = basePath + texturePath;

	if (LWF::GetTextureLoadHandler())
		path = LWF::GetTextureLoadHandler()(path, basePath, texturePath);

	{
		Autolock lock(m_textureSemaphore);
		TextureCache::iterator it = m_textureCache.find(path);
		if (it != m_textureCache.end()) {
			++it->second.refCount;
			return &it->second.lwfTexture;
		}
	}

	NSString *fullPath = [NSString stringWithUTF8String:path.c_str()];
	UIImage *image = [UIImage imageWithContentsOfFile:fullPath];
	if (!image)
		return NULL;

	CGImageRef CGImage = image.CGImage;
	int w = (int)CGImageGetWidth(CGImage);
	int h = (int)CGImageGetHeight(CGImage);

	size_t dataLength = w * h * 4;
	void *pixels = malloc(dataLength);
	CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
	CGContextRef cgx = CGBitmapContextCreate(pixels, w, h, 8, 4 * w,
		colorSpace, kCGImageAlphaPremultipliedLast|kCGBitmapByteOrder32Big);
	CGColorSpaceRelease(colorSpace);
	CGRect rect = CGRectMake(0, 0, w, h);
	CGContextClearRect(cgx, rect);
	CGContextTranslateCTM(cgx, 0, 0);
	CGContextDrawImage(cgx, rect, CGImage);
	CGContextRelease(cgx);

	EAGLContext *currentContext = [EAGLContext currentContext];
	EAGLContext *ctx = [[EAGLContext alloc]
		initWithAPI:kEAGLRenderingAPIOpenGLES2 sharegroup:[context sharegroup]];
	[EAGLContext setCurrentContext:ctx];

	GLuint textureId;
	glGenTextures(1, &textureId);

	glBindTexture(GL_TEXTURE_2D, textureId);
	glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA,
		(GLsizei)w, (GLsizei)h, 0, GL_RGBA, GL_UNSIGNED_BYTE, pixels);

	[EAGLContext setCurrentContext:currentContext];
	ctx = nil;

	free(pixels);

	LWFTexture lwfTexture;
	lwfTexture.textureId = textureId;
	lwfTexture.width = w;
	lwfTexture.height = h;

	{
		Autolock lock(m_textureSemaphore);
		m_textureCache[path] = TextureContext(lwfTexture, context);
		it = m_textureCache.find(path);
		m_textureCacheMap[&it->second.lwfTexture] = it;
		return &it->second.lwfTexture;
	}
}

void LWFResourceCache::unloadTexture(const LWFTexture *lwfTexture)
{
	Autolock lock(m_textureSemaphore);
	TextureCacheMap::iterator it = m_textureCacheMap.find(lwfTexture);
	if (it == m_textureCacheMap.end())
		return;

	if (--it->second->second.refCount <= 0) {
		EAGLContext *currentContext = [EAGLContext currentContext];
		[EAGLContext setCurrentContext:it->second->second.eaglContext];
		GLuint textureId = it->second->second.lwfTexture.textureId;
		glDeleteTextures(1, &textureId);
		[EAGLContext setCurrentContext:currentContext];

		m_textureCache.erase(it->second);
		m_textureCacheMap.erase(it);
	}
}

void LWFResourceCache::unloadAll()
{
	{
		Autolock lock(m_dataSemaphore);
		m_dataCache.clear();
		m_dataCacheMap.clear();
	}

	{
		Autolock lock(m_textureSemaphore);
		EAGLContext *currentContext = [EAGLContext currentContext];
		TextureCache::iterator
			it(m_textureCache.begin()), itend(m_textureCache.end());
		for (; it != itend; ++it) {
			EAGLContext *ctx = [[EAGLContext alloc]
				initWithAPI:kEAGLRenderingAPIOpenGLES2
					sharegroup:[it->second.eaglContext sharegroup]];
			[EAGLContext setCurrentContext:ctx];
			GLuint textureId = it->second.lwfTexture.textureId;
			glDeleteTextures(1, &textureId);
			[EAGLContext setCurrentContext:nil];
			ctx = nil;
		}
		[EAGLContext setCurrentContext:currentContext];
		m_textureCache.clear();
		m_textureCacheMap.clear();
	}
}

}	// namespace LWF
