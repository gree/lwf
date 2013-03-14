/*
 * Copyright (C) 2012 GREE, Inc.
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

using UnityEngine;
using System;
using System.Collections.Generic;

using DataLoader = System.Func<string, byte[]>;
using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;

using DataItem = BitmapFont.CacheItem<BitmapFont.Data>;
using TextureItem = BitmapFont.CacheItem<UnityEngine.Material>;

using DataCache = System.Collections.Generic.Dictionary<
	string, BitmapFont.CacheItem<BitmapFont.Data>>;
using TextureCache = System.Collections.Generic.Dictionary<
	string, BitmapFont.CacheItem<UnityEngine.Material>>;
using ShaderCache = System.Collections.Generic.Dictionary<
	string, UnityEngine.Shader>;

namespace BitmapFont {

public class CacheItem<Type>
{
	private Type m_entity;
	private int m_refCount;

	public CacheItem(Type entity) {
		m_entity = entity;
		m_refCount = 0;
	}
	public int Ref() {return ++m_refCount;}
	public int Unref() {return --m_refCount;}
	public Type Entity() {return m_entity;}
}

public class ResourceCache
{
	private static ResourceCache s_instance;
	private DataLoader m_dataLoader;
	private TextureLoader m_textureLoader;
	private TextureUnloader m_textureUnloader;
	private DataCache m_dataCache;
	private TextureCache m_textureCache;
	private ShaderCache m_shaderCache;

	public static ResourceCache SharedInstance()
	{
		if (s_instance == null)
			s_instance = new ResourceCache();
		return s_instance;
	}

	private ResourceCache()
	{
		m_dataCache = new DataCache();
		m_textureCache = new TextureCache();
		m_shaderCache = new ShaderCache();
		SetLoader();
	}

	public void SetLoader(DataLoader dataLoader = null,
		TextureLoader textureLoader = null,
		TextureUnloader textureUnloader = null)
	{
		m_dataLoader = dataLoader;
		m_textureLoader = textureLoader;
		m_textureUnloader = textureUnloader;

		if (m_dataLoader == null) {
			m_dataLoader = (name) => {
				TextAsset asset = (TextAsset)Resources.Load(name);
				return asset.bytes;
			};
		}

		if (m_textureLoader == null) {
			m_textureLoader = (name) => {
				return (Texture2D)Resources.Load(name);
			};
		}

		if (m_textureUnloader == null) {
			m_textureUnloader = (texture) => {
				Resources.UnloadAsset(texture);
			};
		}
	}

	public Data LoadData(string name)
	{
		DataItem item;
		if (!m_dataCache.TryGetValue(name, out item)) {
			Data data = new Data(m_dataLoader(name));
			item = new DataItem(data);
			m_dataCache[name] = item;
		}
		item.Ref();
		return item.Entity();
	}

	public void UnloadData(string name)
	{
		DataItem item;
		if (m_dataCache.TryGetValue(name, out item)) {
			if (item.Unref() <= 0)
				m_dataCache.Remove(name);
		}
	}

	public Material LoadTexture(string name)
	{
		TextureItem item;
		if (!m_textureCache.TryGetValue(name, out item)) {
			Shader shader = GetShader("BitmapFont");
			Material material = new Material(shader);
			material.mainTexture = m_textureLoader(name);
			if (material.mainTexture != null) {
				material.mainTexture.name = "BitmapFont/" + name;
				material.name = material.mainTexture.name;
			}
			material.color = new UnityEngine.Color(1, 1, 1, 1);
			item = new TextureItem(material);
			m_textureCache[name] = item;
		}
		item.Ref();
		return item.Entity();
	}

	public void UnloadTexture(string name)
	{
		TextureItem item;
		if (m_textureCache.TryGetValue(name, out item)) {
			if (item.Unref() <= 0) {
				Material material = item.Entity();
				if (material.mainTexture != null)
					m_textureUnloader((Texture2D)material.mainTexture);
				Material.Destroy(material);
				m_textureCache.Remove(name);
			}
		}
	}

	public Shader GetShader(string name)
	{
		Shader shader;
		if (!m_shaderCache.TryGetValue(name, out shader)) {
			shader = Shader.Find(name);
			m_shaderCache[name] = shader;
		}
		return shader;
	}

	public void UnloadAll()
	{
		m_dataCache.Clear();
		foreach (TextureItem item in m_textureCache.Values) {
			Material material = item.Entity();
			if (material.mainTexture != null)
				m_textureUnloader((Texture2D)material.mainTexture);
			Material.Destroy(material);
		}
		m_textureCache.Clear();
		m_shaderCache.Clear();
	}
}

}	// namespace BitmapFont
