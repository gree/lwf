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

#ifndef LWF_COCOS2DX_RESOURCECACHE_H
#define LWF_COCOS2DX_RESOURCECACHE_H

#include "platform/CCPlatformMacros.h"
#include "base/CCValue.h"
#include "lwf_type.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
# include "base/CCEventListenerCustom.h"
#endif


namespace LWF {
struct Data;
}

NS_CC_BEGIN


struct LWFTextRendererContext
{
	enum {
		BMFONT,
		SYSTEMFONT,
		TTF,
	};

	int type;
	std::string font;

	LWFTextRendererContext() {}
	LWFTextRendererContext(int t, std::string f) : type(t), font(f) {}
};

class LWFResourceCache
{
private:
	typedef LWF::map<LWF::string,
		LWF::pair<int, LWF::shared_ptr<LWF::Data> > > DataCache;
	typedef LWF::map<LWF::Data *, DataCache::iterator> DataCacheMap;
	typedef LWF::pair<int, LWF::vector<LWF::PreloadCallback> > DataCallbackList;
	typedef LWF::map<LWF::string, DataCallbackList> DataCallbackMap;
	typedef LWF::map<LWF::string, LWF::pair<int, ValueMap> > ParticleCache;
	typedef LWF::map<std::string, LWFTextRendererContext> TextRendererCache_t;

private:
	static LWFResourceCache *m_instance;

private:
	DataCache m_dataCache;
	DataCacheMap m_dataCacheMap;
	DataCallbackMap m_dataCallbackMap;
	ParticleCache m_particleCache;
	TextRendererCache_t m_textRendererCache;
	LWF::string m_fontPathPrefix;
	LWF::string m_particlePathPrefix;
	cocos2d::GLProgram *m_addColorGLProgram;
	cocos2d::GLProgram *m_addColorPAGLProgram;
#if (CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
	cocos2d::EventListenerCustom *m_listener;
#endif

public:
	static LWFResourceCache *sharedLWFResourceCache();

public:
	LWFResourceCache();
	~LWFResourceCache();

	LWF::shared_ptr<LWF::Data> loadLWFData(const LWF::string &path);
	void unloadLWFData(const LWF::shared_ptr<LWF::Data> &data);

	ValueMap loadParticle(const LWF::string &path, bool retain = true);
	void unloadParticle(const LWF::string &path);

	cocos2d::Texture2D *addImage(const char *file);

	void unloadAll();
	void dump();

	cocos2d::GLProgram *getAddColorGLProgram();
	cocos2d::GLProgram *getAddColorPAGLProgram();

	const LWF::string &getFontPathPrefix() {return m_fontPathPrefix;}
	void setFontPathPrefix(const LWF::string path) {m_fontPathPrefix = path;}
	LWFTextRendererContext getTextRendererContext(const LWF::string &font);

	const LWF::string &getDefaultParticlePathPrefix()
		{return m_particlePathPrefix;}
	void setDefaultParticlePathPrefix(const LWF::string path)
		{m_particlePathPrefix = path;}

private:
	void initAddColorGLProgram();

	void unloadLWFDataInternal(const LWF::shared_ptr<LWF::Data> &data);
	LWF::shared_ptr<LWF::Data> loadLWFDataInternal(const LWF::string &path);
};

NS_CC_END

#endif
