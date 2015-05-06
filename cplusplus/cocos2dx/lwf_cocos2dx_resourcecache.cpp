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

#include "base/CCDirector.h"
#include "base/CCValue.h"
#include "platform/CCFileUtils.h"
#include "renderer/ccShaders.h"
#include "renderer/CCGLProgram.h"
#include "renderer/CCTextureCache.h"
#include "lwf_cocos2dx_resourcecache.h"
#include "lwf_data.h"
#include "lwf_core.h"
#include <cstdlib>

#include "lwf_compat.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
# include "base/CCEventDispatcher.h"
# include "base/CCEventType.h"
#endif

#ifndef LWF_USE_IMAGECOLORIZE
# define LWF_USE_IMAGECOLORIZE 1
#endif
#if LWF_USE_IMAGECOLORIZE
# include <regex>
#endif

#define STRINGIFY(A)  #A

static const char *s_additiveColor_frag = STRINGIFY(
\n#ifdef GL_ES\n
precision lowp float;
\n#endif\n

uniform vec3 additiveColor;

varying vec4 v_fragmentColor;
varying vec2 v_texCoord;

void main()
{
	gl_FragColor = v_fragmentColor * texture2D(CC_Texture0, v_texCoord) +
		vec4(additiveColor, 0);
}
);

static const char *s_additiveColorWithPremultipliedAlpha_frag = STRINGIFY(
\n#ifdef GL_ES\n
precision lowp float;
\n#endif\n

uniform vec3 additiveColor;

varying vec4 v_fragmentColor;
varying vec2 v_texCoord;

void main()
{
	vec4 color = v_fragmentColor * texture2D(CC_Texture0, v_texCoord);
	gl_FragColor = color + vec4(
		additiveColor.x * color.w,
		additiveColor.y * color.w,
		additiveColor.z * color.w,
		0);
}
);

using namespace LWF;
using namespace std;
using LWFData = ::LWF::Data;

NS_CC_BEGIN


LWFResourceCache *LWFResourceCache::m_instance;

LWFResourceCache *LWFResourceCache::sharedLWFResourceCache()
{
	if (!m_instance)
		m_instance = new LWFResourceCache();

	return m_instance;
}

LWFResourceCache::LWFResourceCache()
	: m_addColorGLProgram(0), m_addColorPAGLProgram(0)
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
		, m_listener(0)
#endif
{
	m_fontPathPrefix = "fonts/";
	m_particlePathPrefix = "particles/";
}

LWFResourceCache::~LWFResourceCache()
{
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	if (m_listener) {
		Director::getInstance(
			)->getEventDispatcher()->removeEventListener(m_listener);
	}
#endif

	CC_SAFE_RELEASE_NULL(m_addColorGLProgram);
	CC_SAFE_RELEASE_NULL(m_addColorPAGLProgram);
}

shared_ptr<LWFData> LWFResourceCache::loadLWFDataInternal(const string &path)
{
	Data fileData = FileUtils::getInstance()->getDataFromFile(path.c_str());
	if (fileData.isNull())
		return shared_ptr<LWFData>();

	shared_ptr<LWFData> data =
		make_shared<LWFData>(fileData.getBytes(), fileData.getSize());
	if (!data->Check())
		return shared_ptr<LWFData>();

	return data;
}

shared_ptr<LWFData> LWFResourceCache::loadLWFData(const string &path)
{
	DataCache::iterator it = m_dataCache.find(path);
	if (it != m_dataCache.end()) {
		++it->second.first;
		return it->second.second;
	}

	shared_ptr<LWFData> data = loadLWFDataInternal(path);
	m_dataCache[path] = make_pair(1, data);
	m_dataCacheMap[data.get()] = m_dataCache.find(path);
	return data;
}

#if LWF_USE_IMAGECOLORIZE
static void checkImagePath(const char *file, string &imagePath,
	bool &toRGB, bool &toRGBA, bool &toADD, unsigned char &red,
	unsigned char &green, unsigned char &blue, unsigned char &alpha)
{
	static regex eRGB(
		"(.*)_rgb_([0-9a-f]{6})(.*)$", regex_constants::icase);
	static regex eRGB10(
		"(.*)_rgb_([0-9*),([0-9]*),([0-9]*)(.*)$",
		regex_constants::icase);
	static regex eRGBA(
		"(.*)_rgba_([0-9a-f]{8})(.*)$", regex_constants::icase);
	static regex eRGBA10(
		"(.*)_rgba_([0-9]*),([0-9]*),([0-9]*),([0-9]*)(.*)$",
		regex_constants::icase);
	static regex eADD(
		"(.*)_add_([0-9a-f]{6})(.*)$", regex_constants::icase);
	static regex eADD10(
		"(.*)_add_([0-9]*),([0-9]*),([0-9]*)(.*)$",
		regex_constants::icase);

	imagePath = file;
	toRGB = false;
	toRGBA = false;
	toADD = false;
	red = 0;
	green = 0;
	blue = 0;
	alpha = 0;
	smatch match;
	if (strcasestr(file, "_rgb_") != 0) {
		if (regex_match(imagePath, match, eRGB)) {
			toRGB = true;
			string rgbHex = match[2].str();
			red = strtoul(rgbHex.substr(0, 2).c_str(), 0, 16);
			green = strtoul(rgbHex.substr(2, 2).c_str(), 0, 16);
			blue = strtoul(rgbHex.substr(4, 2).c_str(), 0, 16);
			imagePath = match[1].str() + match[3].str();
		} else if (regex_match(imagePath, match, eRGB10)) {
			toRGB = true;
			red = strtoul(match[2].str().c_str(), 0, 10);
			green = strtoul(match[3].str().c_str(), 0, 10);
			blue = strtoul(match[4].str().c_str(), 0, 10);
			imagePath = match[1].str() + match[5].str();
		}
	} else if (strcasestr(file, "_rgba_") != 0) {
		if (regex_match(imagePath, match, eRGBA)) {
			toRGBA = true;
			string rgbaHex = match[2].str();
			red = strtoul(rgbaHex.substr(0, 2).c_str(), 0, 16);
			green = strtoul(rgbaHex.substr(2, 2).c_str(), 0, 16);
			blue = strtoul(rgbaHex.substr(4, 2).c_str(), 0, 16);
			alpha = strtoul(rgbaHex.substr(6, 2).c_str(), 0, 16);
			imagePath = match[1].str() + match[3].str();
		} else if (regex_match(imagePath, match, eRGBA10)) {
			toRGBA = true;
			red = strtoul(match[2].str().c_str(), 0, 10);
			green = strtoul(match[3].str().c_str(), 0, 10);
			blue = strtoul(match[4].str().c_str(), 0, 10);
			alpha = strtoul(match[5].str().c_str(), 0, 10);
			imagePath = match[1].str() + match[6].str();
		}
	} else if (strcasestr(file, "_add_") != 0) {
		if (regex_match(imagePath, match, eADD)) {
			toADD = true;
			string rgbHex = match[2].str();
			red = strtoul(rgbHex.substr(0, 2).c_str(), 0, 16);
			green = strtoul(rgbHex.substr(2, 2).c_str(), 0, 16);
			blue = strtoul(rgbHex.substr(4, 2).c_str(), 0, 16);
			imagePath = match[1].str() + match[3].str();
		} else if (regex_match(imagePath, match, eADD10)) {
			toADD = true;
			red = strtoul(match[2].str().c_str(), 0, 10);
			green = strtoul(match[3].str().c_str(), 0, 10);
			blue = strtoul(match[4].str().c_str(), 0, 10);
			imagePath = match[1].str() + match[5].str();
		}
	}
}

static bool checkAtlas(string &imagePath,
	int &rotated, int &u, int &v, int &w, int &h, int &sw, int &sh)
{
	static regex eATLAS("(.*\\.[^_]+)_atlas_(.*\\.*)_info_"
		"([0-9])_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)",
		regex_constants::icase);
	static regex ePATH("(.*/)[^\\/]+", regex_constants::icase);

	smatch match;
	if (regex_match(imagePath, match, eATLAS)) {
		string dir = match[1].str();
		string atlasFile = match[2].str();
		rotated = (int)strtoul(match[3].str().c_str(), 0, 10);
		u = (int)strtoul(match[4].str().c_str(), 0, 10);
		v = (int)strtoul(match[5].str().c_str(), 0, 10);
		w = (int)strtoul(match[6].str().c_str(), 0, 10);
		h = (int)strtoul(match[7].str().c_str(), 0, 10);
		sw = rotated ? h : w;
		sh = rotated ? w : h;

		if (regex_match(dir, match, ePATH))
			dir = match[1].str();

		imagePath = dir + atlasFile;

		return true;
	}

	return false;
}

static Image *generateImage(string imagePath, Image *baseImage,
	int rotated, int u, int v, int w, int h, int sw, int sh)
{
	unsigned char *srcData = baseImage->getData();
	int ow = baseImage->getWidth();

	int dataLen = w * h * 4;
	unsigned char *dstData = (unsigned char *)calloc(1, dataLen);

	switch (baseImage->getRenderFormat()) {
	case Texture2D::PixelFormat::RGBA8888:
		{
			if (rotated) {
				for (int sy = v, dx = 0; sy < v + sh; ++sy, ++dx) {
					for (int sx = u, dy = h - 1;
							sx < u + sw; ++sx, --dy) {
						unsigned char *sp =
							&srcData[sy * ow * 4 + sx * 4];
						unsigned char *dp =
							&dstData[dy * w * 4 + dx * 4];
						memcpy(dp, sp, 4);
					}
				}
			} else {
				for (int sy = v, dy = 0; sy < v + h; ++sy, ++dy) {
					unsigned char *sp =
						&srcData[sy * ow * 4 + u * 4];
					unsigned char *dp = &dstData[dy * w * 4];
					memcpy(dp, sp, w * 4);
				}
			}
		}
		break;

	case Texture2D::PixelFormat::RGB888:
		{
			if (rotated) {
				for (int sy = v, dx = 0; sy < v + sh; ++sy, ++dx) {
					for (int sx = u, dy = h - 1;
							sx < u + sw; ++sx, --dy) {
						unsigned char *sp =
							&srcData[sy * ow * 3 + sx * 3];
						unsigned char *dp =
							&dstData[dy * w * 4 + dx * 4];
						memcpy(dp, sp, 3);
						*(dp + 3) = 0xff;
					}
				}
			} else {
				for (int sy = v, dy = 0; sy < v + h; ++sy, ++dy) {
					for (int sx = u, dx = 0;
							sx < u + w; ++sx, ++dx) {
						unsigned char *sp =
							&srcData[sy * ow * 3 + sx * 3];
						unsigned char *dp =
							&dstData[dy * w * 4 + dx * 4];
						memcpy(dp, sp, 3);
						*(dp + 3) = 0xff;
					}
				}
			}
		}
		break;

	default:
		log("cocos2d: WARNING: %s: Image pixel format is not "
			"supported for converting color", imagePath.c_str());
		break;
	}

	Image *image = new Image();
	image->initWithRawData(dstData, dataLen, w, h, 8);
	free(dstData);

	return image;
}

static void colorImage(string imagePath, Image *image, bool toRGB,
	bool toRGBA, bool toADD, unsigned char red, unsigned char green,
	unsigned char blue, unsigned char alpha)
{
	switch (image->getRenderFormat()) {
	case Texture2D::PixelFormat::RGBA8888:
		{
			unsigned char *p = image->getData();
			int height = image->getHeight();
			int width = image->getWidth();

			for (int i = 0; i < height * width * 4; i += 4) {
				unsigned char *pr = &p[i + 0];
				unsigned char *pg = &p[i + 1];
				unsigned char *pb = &p[i + 2];
				unsigned char *pa = &p[i + 3];

				if (toRGB) {
					*pr = red;
					*pg = green;
					*pb = blue;
				} else if (toRGBA) {
					*pr = red;
					*pg = green;
					*pb = blue;
					*pa = alpha;
				} else if (toADD) {
					*pr = max(min(*pr + red, 255), 0);
					*pg = max(min(*pg + green, 255), 0);
					*pb = max(min(*pb + blue, 255), 0);
				}
				if (image->hasPremultipliedAlpha()) {
					if (*pa == 0) {
						*pr = 0;
						*pg = 0;
						*pb = 0;
					} else {
						*pr = *pr * 255 / *pa;
						*pg = *pg * 255 / *pa;
						*pb = *pb * 255 / *pa;
					}
				}
			}
		}
		break;

	case Texture2D::PixelFormat::RGB888:
		{
			unsigned char *p = image->getData();
			int height = image->getHeight();
			int width = image->getWidth();

			for (int i = 0; i < height * width * 3; i += 3) {
				unsigned char *pr = &p[i + 0];
				unsigned char *pg = &p[i + 1];
				unsigned char *pb = &p[i + 2];

				if (toRGB || toRGBA) {
					*pr = red;
					*pg = green;
					*pb = blue;
				} else if (toADD) {
					*pr = max(min(*pr + red, 255), 0);
					*pg = max(min(*pg + green, 255), 0);
					*pb = max(min(*pb + blue, 255), 0);
				}
			}
		}
		break;

	default:
		log("cocos2d: WARNING: %s: Image pixel format is not support "
			"for converting color", imagePath.c_str());
		break;
	}
}
#endif

Texture2D *LWFResourceCache::addImage(const char *file)
{
	TextureCache *cache = Director::getInstance()->getTextureCache();
	Texture2D *texture = 0;

#if LWF_USE_IMAGECOLORIZE
	string imagePath;
	bool toRGB;
	bool toRGBA;
	bool toADD;
	unsigned char red;
	unsigned char green;
	unsigned char blue;
	unsigned char alpha;

	checkImagePath(file,
		imagePath, toRGB, toRGBA, toADD, red, green, blue, alpha);

	if (toRGB || toRGBA || toADD) {
		texture = cache->getTextureForKey(file);
		if (!texture) {
			Image *image = new Image();

			int rotated;
			int u;
			int v;
			int w;
			int h;
			int sw;
			int sh;

			if (checkAtlas(imagePath, rotated, u, v, w, h, sw, sh)) {
				image->initWithImageFile(imagePath);
				Image *fragmentImage = generateImage(
					imagePath, image, rotated, u, v, w, h , sw, sh);
				image->release();
				image = fragmentImage;
			} else {
				image->initWithImageFile(imagePath);
			}

			colorImage(imagePath,
				image, toRGB, toRGBA, toADD, red, green, blue, alpha);

			texture = cache->addImage(image, file);
			image->release();
		}
	} else {
		texture = cache->addImage(imagePath);
	}
#else
	texture = cache->addImage(file);
#endif

	return texture;
}


void LWFResourceCache::initAddColorGLProgram()
{
	m_addColorGLProgram = GLProgram::createWithByteArrays(
		ccPositionTextureColor_noMVP_vert,
		s_additiveColor_frag);
	m_addColorGLProgram->retain();

	m_addColorPAGLProgram = GLProgram::createWithByteArrays(
		ccPositionTextureColor_noMVP_vert,
		s_additiveColorWithPremultipliedAlpha_frag);
	m_addColorPAGLProgram->retain();

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	m_listener = EventListenerCustom::create(
			EVENT_RENDERER_RECREATED, [this](EventCustom*){
		m_addColorGLProgram->reset();
		m_addColorGLProgram->initWithByteArrays(
			ccPositionTextureColor_noMVP_vert,
			s_additiveColor_frag);
		m_addColorGLProgram->link();
		m_addColorGLProgram->updateUniforms();

		m_addColorPAGLProgram->reset();
		m_addColorPAGLProgram->initWithByteArrays(
			ccPositionTextureColor_noMVP_vert,
			s_additiveColorWithPremultipliedAlpha_frag);
		m_addColorPAGLProgram->link();
		m_addColorPAGLProgram->updateUniforms();
	});
	Director::getInstance()->getEventDispatcher(
		)->addEventListenerWithFixedPriority(m_listener, -1);
#endif
}

GLProgram *LWFResourceCache::getAddColorGLProgram()
{
	if (m_addColorGLProgram == 0)
		initAddColorGLProgram();
	return m_addColorGLProgram;
}

GLProgram *LWFResourceCache::getAddColorPAGLProgram()
{
	if (m_addColorPAGLProgram == 0)
		initAddColorGLProgram();
	return m_addColorPAGLProgram;
}

void LWFResourceCache::unloadLWFDataInternal(const shared_ptr<LWFData> &data)
{
	map<string, bool>::iterator
		it(data->resourceCache.begin()), itend(data->resourceCache.end());
	TextureCache *cache = Director::getInstance()->getTextureCache();
	for (; it != itend; ++it) {
		Texture2D *texture = cache->getTextureForKey(it->first.c_str());
		if (texture && texture->getReferenceCount() == 1)
			cache->removeTexture(texture);
	}
}

void LWFResourceCache::unloadLWFData(const shared_ptr<LWFData> &data)
{
	DataCacheMap::iterator it = m_dataCacheMap.find(data.get());
	if (it == m_dataCacheMap.end())
		return;

	if (--it->second->second.first <= 0) {
		unloadLWFDataInternal(data);
		m_dataCache.erase(it->second);
		m_dataCacheMap.erase(it);
	}
}

ValueMap LWFResourceCache::loadParticle(const string &path, bool retain)
{
	ParticleCache::iterator it = m_particleCache.find(path);
	if (it != m_particleCache.end()) {
		if (retain)
			++it->second.first;
		return it->second.second;
	}

	string fullPath =
		FileUtils::getInstance()->fullPathForFilename(path);
	ValueMap dict =
		FileUtils::getInstance()->getValueMapFromFile(fullPath.c_str());

	if (!dict.empty()) {
		m_particleCache[path] = make_pair(retain ? 1 : 0, dict);
		dict["path"] = Value(path);
	}

	return dict;
}

void LWFResourceCache::unloadParticle(const string &path)
{
	ParticleCache::iterator it = m_particleCache.find(path);
	if (it == m_particleCache.end())
		return;

	if (--it->second.first <= 0)
		m_particleCache.erase(it);
}

LWFTextRendererContext LWFResourceCache::getTextRendererContext(
	const string &font)
{
	TextRendererCache_t::iterator it = m_textRendererCache.find(font);
	if (it != m_textRendererCache.end())
		return it->second;

	FileUtils *fileUtils = FileUtils::getInstance();
	string fontPath = getFontPathPrefix() + font;

	const char *p = font.c_str() + font.size() - 4;
	if (strncasecmp(p, ".fnt", 4) == 0) {
		LWFTextRendererContext c(LWFTextRendererContext::BMFONT, fontPath);
		m_textRendererCache[font] = c;
		return c;
	}

	if (strncasecmp(p, ".ttf", 4) == 0) {
		LWFTextRendererContext c(LWFTextRendererContext::TTF, fontPath);
		m_textRendererCache[font] = c;
		return c;
	}

	fontPath = getFontPathPrefix() + font + ".fnt";
	if (fileUtils->isFileExist(fontPath)) {
		LWFTextRendererContext c(LWFTextRendererContext::BMFONT, fontPath);
		m_textRendererCache[font] = c;
		return c;
	}

	fontPath = getFontPathPrefix() + font + ".ttf";
	if (fileUtils->isFileExist(fontPath)) {
		LWFTextRendererContext c(LWFTextRendererContext::TTF, fontPath);
		m_textRendererCache[font] = c;
		return c;
	}

	if (font[0] == '_')
		fontPath = font.substr(1);
	else
		fontPath = font;

	LWFTextRendererContext c(LWFTextRendererContext::SYSTEMFONT, fontPath);
	m_textRendererCache[font] = c;
	return c;
}


void LWFResourceCache::unloadAll()
{
	DataCacheMap::iterator
		it(m_dataCacheMap.begin()), itend(m_dataCacheMap.end());
	while (it != itend) {
		unloadLWFDataInternal(it->second->second.second);
		m_dataCache.erase(it->second);
		m_dataCacheMap.erase(it++);
	}
	m_dataCache.clear();
	m_dataCacheMap.clear();

	m_particleCache.clear();

	CC_SAFE_RELEASE_NULL(m_addColorGLProgram);
	CC_SAFE_RELEASE_NULL(m_addColorPAGLProgram);
}

void LWFResourceCache::dump()
{
	log("LWFResourceCache=====");
	log("LWF--------------------");
	DataCacheMap::iterator
		it(m_dataCacheMap.begin()), itend(m_dataCacheMap.end());
	for (; it != itend; ++it)
		log("%d %s", it->second->second.first, it->second->first.c_str());

	log("Particle---------------");
	ParticleCache::iterator
		pit(m_particleCache.begin()), pitend(m_particleCache.end());
	for (; pit != pitend; ++pit)
		log("%d %s", pit->second.first, pit->first.c_str());
	log("=======================");
}

NS_CC_END

