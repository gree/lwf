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

#import <dispatch/dispatch.h>
#import "lwf_type.h"

namespace LWF {
struct Data;
class LWFBitmapRendererContext;

class LWFResourceCache
{
public:
	struct DataContext {
		int refCount;
		shared_ptr<Data> data;
		vector<shared_ptr<LWFBitmapRendererContext> > bitmapContexts;
		vector<shared_ptr<LWFBitmapRendererContext> > bitmapExContexts;

		DataContext() {}
		DataContext(shared_ptr<Data> d,
				vector<shared_ptr<LWFBitmapRendererContext> > &b,
				vector<shared_ptr<LWFBitmapRendererContext> > &bx)
			: refCount(1), data(d), bitmapContexts(b), bitmapExContexts(bx) {}
	};

	struct TextureContext
	{
		int refCount;
		UIImage *uiImage;
	
		TextureContext() {}
		TextureContext(UIImage *u)
			: refCount(1), uiImage(u) {}
	};

private:
	typedef map<string, DataContext> DataCache;
	typedef map<Data *, DataCache::iterator> DataCacheMap;
	typedef map<string, TextureContext> TextureCache;
	typedef map<UIImage *, TextureCache::iterator> TextureCacheMap;

private:
	static LWFResourceCache *m_instance;

private:
	dispatch_semaphore_t m_dataSemaphore;
	dispatch_semaphore_t m_textureSemaphore;
	DataCache m_dataCache;
	DataCacheMap m_dataCacheMap;
	TextureCache m_textureCache;
	TextureCacheMap m_textureCacheMap;

public:
	static LWFResourceCache *shared();

public:
	LWFResourceCache();
	~LWFResourceCache();

	shared_ptr<Data> loadLWFData(const string &pathstr);
	void unloadLWFData(const shared_ptr<Data> &data);
	const DataContext *getDataContext(const shared_ptr<Data> &data) const;

	UIImage *loadTexture(const string &dataPath, const string &texturePath);
	void unloadTexture(UIImage *uiImage);

	void unloadAll();
};

}	// namespace LWF
