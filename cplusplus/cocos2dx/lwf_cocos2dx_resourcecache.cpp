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
#include <regex>
#include <cstdlib>

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

Texture2D *LWFResourceCache::addImage(const char *file)
{
	static std::regex eRGB(
		"(.*)_rgb_([0-9a-f]{6})(.*)$", std::regex_constants::icase);
	static std::regex eRGB10(
		"(.*)_rgb_([0-9*),([0-9]*),([0-9]*)(.*)$",
		std::regex_constants::icase);
	static std::regex eRGBA(
		"(.*)_rgba_([0-9a-f]{8})(.*)$", std::regex_constants::icase);
	static std::regex eRGBA10(
		"(.*)_rgba_([0-9]*),([0-9]*),([0-9]*),([0-9]*)(.*)$",
		std::regex_constants::icase);
	static std::regex eADD(
		"(.*)_add_([0-9a-f]{6})(.*)$", std::regex_constants::icase);
	static std::regex eADD10(
		"(.*)_add_([0-9]*),([0-9]*),([0-9]*)(.*)$",
		std::regex_constants::icase);
	static std::regex eATLAS("(.*\\.[^_]+)_atlas_(.*\\.*)_info_"
		"([0-9])_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)",
		std::regex_constants::icase);
	static std::regex ePATH("(.*/)[^\\/]+", std::regex_constants::icase);

	std::string texPath = file;
	std::smatch match;
	bool toRGB = false;
	bool toRGBA = false;
	bool toADD = false;
	unsigned char red = 0;
	unsigned char green = 0;
	unsigned char blue = 0;
	unsigned char alpha = 0;
	if (strcasestr(file, "_rgb_") != 0) {
		if (std::regex_match(texPath, match, eRGB)) {
			texPath = match[1].str() + match[3].str();
			toRGB = true;
			std::string rgbHex = match[2].str();
			red = std::strtoul(rgbHex.substr(0, 2).c_str(), 0, 16);
			green = std::strtoul(rgbHex.substr(2, 2).c_str(), 0, 16);
			blue = std::strtoul(rgbHex.substr(4, 2).c_str(), 0, 16);
		} else if (std::regex_match(texPath, match, eRGB10)) {
			texPath = match[1].str() + match[5].str();
			toRGB = true;
			red = std::strtoul(match[2].str().c_str(), 0, 10);
			green = std::strtoul(match[3].str().c_str(), 0, 10);
			blue = std::strtoul(match[4].str().c_str(), 0, 10);
		}
	} else if (strcasestr(file, "_rgba_") != 0) {
		if (std::regex_match(texPath, match, eRGBA)) {
			texPath = match[1].str() + match[3].str();
			toRGBA = true;
			std::string rgbaHex = match[2].str();
			red = std::strtoul(rgbaHex.substr(0, 2).c_str(), 0, 16);
			green = std::strtoul(rgbaHex.substr(2, 2).c_str(), 0, 16);
			blue = std::strtoul(rgbaHex.substr(4, 2).c_str(), 0, 16);
			alpha = std::strtoul(rgbaHex.substr(6, 2).c_str(), 0, 16);
		} else if (std::regex_match(texPath, match, eRGBA10)) {
			texPath = match[1].str() + match[6].str();
			toRGBA = true;
			red = std::strtoul(match[2].str().c_str(), 0, 10);
			green = std::strtoul(match[3].str().c_str(), 0, 10);
			blue = std::strtoul(match[4].str().c_str(), 0, 10);
			alpha = std::strtoul(match[5].str().c_str(), 0, 10);
		}
	} else if (strcasestr(file, "_add_") != 0) {
		if (std::regex_match(texPath, match, eADD)) {
			texPath = match[1].str() + match[3].str();
			toADD = true;
			std::string rgbHex = match[2].str();
			red = std::strtoul(rgbHex.substr(0, 2).c_str(), 0, 16);
			green = std::strtoul(rgbHex.substr(2, 2).c_str(), 0, 16);
			blue = std::strtoul(rgbHex.substr(4, 2).c_str(), 0, 16);
		} else if (std::regex_match(texPath, match, eADD10)) {
			texPath = match[1].str() + match[5].str();
			toADD = true;
			red = std::strtoul(match[2].str().c_str(), 0, 10);
			green = std::strtoul(match[3].str().c_str(), 0, 10);
			blue = std::strtoul(match[4].str().c_str(), 0, 10);
		}
	}

	TextureCache *cache = Director::getInstance()->getTextureCache();
	Texture2D *texture = 0;

	if (toRGB || toRGBA || toADD) {
		texture = cache->getTextureForKey(file);
		if (!texture) {
			Image *image = new Image();

			if (std::regex_match(texPath, match, eATLAS)) {
				std::string dir = match[1].str();
				std::string atlasFile = match[2].str();
				int rotated = (int)std::strtoul(match[3].str().c_str(), 0, 10);
				int u = (int)std::strtoul(match[4].str().c_str(), 0, 10);
				int v = (int)std::strtoul(match[5].str().c_str(), 0, 10);
				int w = (int)std::strtoul(match[6].str().c_str(), 0, 10);
				int h = (int)std::strtoul(match[7].str().c_str(), 0, 10);
				int sw = rotated ? h : w;
				int sh = rotated ? w : h;

				if (std::regex_match(dir, match, ePATH))
					dir = match[1].str();
				image->initWithImageFile(dir + atlasFile);
				unsigned char *srcData = image->getData();
				int ow = image->getWidth();

				int dataLen = w * h * 4;
				unsigned char *dstData = (unsigned char *)calloc(1, dataLen);

				switch (image->getRenderFormat()) {
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
						"support for converting color", file);
					break;
				}

				image->release();
				image = new Image();
				image->initWithRawData(dstData, dataLen, w, h, 8);
				free(dstData);
			} else {
				image->initWithImageFile(texPath);
			}

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
							*pr = std::max(std::min(*pr + red, 255), 0);
							*pg = std::max(std::min(*pg + green, 255), 0);
							*pb = std::max(std::min(*pb + blue, 255), 0);
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
							*pr = std::max(std::min(*pr + red, 255), 0);
							*pg = std::max(std::min(*pg + green, 255), 0);
							*pb = std::max(std::min(*pb + blue, 255), 0);
						}
					}
				}
				break;

			default:
				log("cocos2d: WARNING: %s: Image pixel format is not support "
					"for converting color", file);
				break;
			}

			texture = cache->addImage(image, file);
			image->release();
		}
	} else {
		texture = cache->addImage(texPath);
	}

	return texture;
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
		FileUtils::getInstance()->fullPathForFilename(path.c_str());
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

	static std::regex eFnt(".*\\.fnt$", std::regex_constants::icase);
	if (std::regex_match(font, eFnt)) {
		LWFTextRendererContext c(LWFTextRendererContext::BMFONT, fontPath);
		m_textRendererCache[font] = c;
		return c;
	}

	static std::regex eTtf(".*\\.ttf$", std::regex_constants::icase);
	if (std::regex_match(font, eTtf)) {
		LWFTextRendererContext c(LWFTextRendererContext::TTF, fontPath);
		m_textRendererCache[font] = c;
		return c;
	}

	fontPath = getFontPathPrefix() + font + ".fnt";
	if (fileUtils->isFileExist(font)) {
		LWFTextRendererContext c(LWFTextRendererContext::BMFONT, fontPath);
		m_textRendererCache[font] = c;
		return c;
	}

	fontPath = getFontPathPrefix() + font + ".ttf";
	if (fileUtils->isFileExist(font)) {
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

