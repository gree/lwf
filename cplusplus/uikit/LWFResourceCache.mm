/*
 * Copyright (C) 2013 GREE, Inc.
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

#import <UIKit/UIKit.h>
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
	m_semaphore = dispatch_semaphore_create(1);
}

LWFResourceCache::~LWFResourceCache()
{
#if !OS_OBJECT_USE_OBJC
	dispatch_release(m_semaphore);
#endif
}

shared_ptr<Data> LWFResourceCache::loadLWFData(const string &pathstr)
{
	{
		Autolock lock(m_semaphore);
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

		bitmapContexts[i] =
			make_shared<LWFBitmapRendererContext>(data.get(), bx, fullPath);
	}

	vector<shared_ptr<LWFBitmapRendererContext> > bitmapExContexts;
	bitmapExContexts.resize(data->bitmapExs.size());
	for (size_t i = 0; i < data->bitmapExs.size(); ++i) {
		const Format::BitmapEx &bx = data->bitmapExs[i];
		if (bx.textureFragmentId == -1)
			continue;

		bitmapExContexts[i] =
			make_shared<LWFBitmapRendererContext>(data.get(), bx, fullPath);
	}

	{
		Autolock lock(m_semaphore);
		m_dataCache[pathstr] =
			DataContext(data, bitmapContexts, bitmapExContexts);
		m_dataCacheMap[data.get()] = m_dataCache.find(pathstr);
	}

	return data;
}

void LWFResourceCache::unloadLWFData(const shared_ptr<Data> &data)
{
	Autolock lock(m_semaphore);
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
	Autolock lock(m_semaphore);
	DataCacheMap::const_iterator it = m_dataCacheMap.find(data.get());
	if (it == m_dataCacheMap.end())
		return 0;

	return &it->second->second;
}

UIImage *LWFResourceCache::loadTexture(
	const string &dataPath, const string &texturePath)
{
	size_t pos = dataPath.find_last_of('/');
	string path;
	if (pos == string::npos)
		path = texturePath;
	else
		path = dataPath.substr(0, pos + 1) + texturePath;

	{
		Autolock lock(m_semaphore);
		TextureCache::iterator it = m_textureCache.find(path);
		if (it != m_textureCache.end()) {
			++it->second.refCount;
			return it->second.uiImage;
		}
	}

	NSString *fullPath = [NSString stringWithUTF8String:path.c_str()];
	UIImage *image = [UIImage imageWithContentsOfFile:fullPath];
	if (!image)
		return NULL;

	{
		Autolock lock(m_semaphore);
		m_textureCache[path] = TextureContext(image);
		TextureCache::iterator it = m_textureCache.find(path);
		m_textureCacheMap[image] = it;
	}

	return image;
}

void LWFResourceCache::unloadTexture(UIImage *uiImage)
{
	Autolock lock(m_semaphore);
	TextureCacheMap::iterator it = m_textureCacheMap.find(uiImage);
	if (it == m_textureCacheMap.end())
		return;

	if (--it->second->second.refCount <= 0) {
		m_textureCache.erase(it->second);
		m_textureCacheMap.erase(it);
	}
}

void LWFResourceCache::unloadAll()
{
	Autolock lock(m_semaphore);
	m_dataCache.clear();
	m_dataCacheMap.clear();
	m_textureCache.clear();
	m_textureCacheMap.clear();
}

}	// namespace LWF
