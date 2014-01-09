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

#include "CCPlatformMacros.h"
#include "CCValue.h"
#include "lwf_type.h"


namespace LWF {
class Data;
}

NS_CC_BEGIN


class LWFResourceCache
{
private:
	typedef LWF::map<LWF::string,
		LWF::pair<int, LWF::shared_ptr<LWF::Data> > > DataCache;
	typedef LWF::map<LWF::Data *, DataCache::iterator> DataCacheMap;
	typedef LWF::pair<int, LWF::vector<LWF::PreloadCallback> > DataCallbackList;
	typedef LWF::map<LWF::string, DataCallbackList> DataCallbackMap;
	typedef LWF::map<LWF::string, LWF::pair<int, ValueMap> > ParticleCache;

private:
	static LWFResourceCache *m_instance;

private:
	DataCache m_dataCache;
	DataCacheMap m_dataCacheMap;
	DataCallbackMap m_dataCallbackMap;
	ParticleCache m_particleCache;
	LWF::string m_fontPathPrefix;
	LWF::string m_particlePathPrefix;

public:
	static LWFResourceCache *sharedLWFResourceCache();

public:
	LWFResourceCache();
	~LWFResourceCache();

	LWF::shared_ptr<LWF::Data> loadLWFData(const LWF::string &path);
	void unloadLWFData(const LWF::shared_ptr<LWF::Data> &data);

    ValueMap loadParticle(const LWF::string &path, bool retain = true);
	void unloadParticle(const LWF::string &path);

	void unloadAll();
	void dump();

	const LWF::string &getFontPathPrefix() {return m_fontPathPrefix;}
	void setFontPathPrefix(const LWF::string path) {m_fontPathPrefix = path;}
	const LWF::string &getDefaultParticlePathPrefix()
		{return m_particlePathPrefix;}
	void setDefaultParticlePathPrefix(const LWF::string path)
		{m_particlePathPrefix = path;}

private:
	void unloadLWFDataInternal(const LWF::shared_ptr<LWF::Data> &data);
	LWF::shared_ptr<LWF::Data> loadLWFDataInternal(const LWF::string &path);
};

NS_CC_END

#endif
