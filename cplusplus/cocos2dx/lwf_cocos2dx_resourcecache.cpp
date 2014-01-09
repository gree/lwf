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

#include "CCDirector.h"
#include "CCFileUtils.h"
#include "CCTextureCache.h"
#include "CCValue.h"
#include "lwf_cocos2dx_resourcecache.h"
#include "lwf_data.h"

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
{
	m_fontPathPrefix = "fonts/";
	m_particlePathPrefix = "particles/";
}

LWFResourceCache::~LWFResourceCache()
{
}

shared_ptr<LWFData> LWFResourceCache::loadLWFDataInternal(const string &path)
{
    long size;
	unsigned char *buffer =
		FileUtils::getInstance()->getFileData(path.c_str(), "r", &size);
	if (!buffer)
		return shared_ptr<LWFData>();

	shared_ptr<LWFData> data = make_shared<LWFData>(buffer, size);
	delete [] buffer;
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


void LWFResourceCache::unloadLWFDataInternal(const shared_ptr<LWFData> &data)
{
	map<string, bool>::iterator
		it(data->resourceCache.begin()), itend(data->resourceCache.end());
	TextureCache *cache = Director::getInstance()->getTextureCache();
	for (; it != itend; ++it) {
		Texture2D *texture = cache->getTextureForKey(it->first.c_str());
		if (texture && texture->retainCount() == 1)
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

