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
using System.Text.RegularExpressions;

using BlendMode = UnityEngine.Rendering.BlendMode;
using BlendOp = UnityEngine.Rendering.BlendOp;

using LWFDataLoader = System.Func<string, byte[]>;
using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;

using LWFDataItem = LWF.UnityRenderer.CacheItem<LWF.Data>;
using TextureItem =
	LWF.UnityRenderer.CacheItem<LWF.UnityRenderer.TextureContext>;
using MeshItem = LWF.UnityRenderer.CacheItem<LWF.UnityRenderer.MeshContext>;
using RenderedMeshItem = LWF.UnityRenderer.CacheItem<UnityEngine.Mesh[]>;

using LWFDataCache = System.Collections.Generic.Dictionary<
	string, LWF.UnityRenderer.CacheItem<LWF.Data>>;
using TextureCache = System.Collections.Generic.Dictionary<
	string, LWF.UnityRenderer.CacheItem<LWF.UnityRenderer.TextureContext>>;
using MeshCache = System.Collections.Generic.Dictionary<
	string, LWF.UnityRenderer.CacheItem<LWF.UnityRenderer.MeshContext>>;
using ShaderCache = System.Collections.Generic.Dictionary<
	string, UnityEngine.Shader>;
using RenderedMeshCache = System.Collections.Generic.Dictionary<
	string, LWF.UnityRenderer.CacheItem<UnityEngine.Mesh[]>>;

namespace LWF {
namespace UnityRenderer {

public class TextureContext
{
	public UnityEngine.Material material;
	public TextureUnloader unloader;

	public TextureContext(UnityEngine.Material m, TextureUnloader u)
	{
		material = m;
		unloader = u;
	}
}

public class MeshContext
{
	public Mesh mesh;
	public float height;

	public MeshContext(Mesh m, float h)
	{
		mesh = m;
		height = h;
	}
}

public class CacheItem<Type>
{
	private Type m_entity;
	private int m_refCount;

	public int Ref() {return ++m_refCount;}
	public int Unref() {return --m_refCount;}
	public int RefCount() {return m_refCount;}
	public Type Entity() {return m_entity;}

	public CacheItem(Type entity)
	{
		m_entity = entity;
		m_refCount = 0;
	}
}

public class ResourceCache
{
	private static ResourceCache s_instance;
	private LWFDataLoader m_lwfDataLoader;
	private TextureLoader m_textureLoader;
	private TextureUnloader m_textureUnloader;
	private LWFDataCache m_lwfDataCache;
	private TextureCache m_textureCache;
	private MeshCache m_meshCache;
	private ShaderCache m_shaderCache;
	private RenderedMeshCache m_renderedMeshCache;

	public TextureCache textureCache {get {return m_textureCache;}}

	public static ResourceCache SharedInstance()
	{
		if (s_instance == null)
			s_instance = new ResourceCache();
		return s_instance;
	}

	private ResourceCache()
	{
		m_lwfDataCache = new LWFDataCache();
		m_textureCache = new TextureCache();
		m_meshCache = new MeshCache();
		m_shaderCache = new ShaderCache();
		m_renderedMeshCache = new RenderedMeshCache();
		SetLoader();
	}

	public void SetLoader(
		LWFDataLoader lwfDataLoader = null,
		TextureLoader textureLoader = null,
		TextureUnloader textureUnloader = null)
	{
		m_lwfDataLoader = lwfDataLoader;
		m_textureLoader = textureLoader;
		m_textureUnloader = textureUnloader;

		if (m_lwfDataLoader == null) {
			m_lwfDataLoader = (filename) => {
				TextAsset asset =
					(TextAsset)Resources.Load(filename, typeof(TextAsset));
				if (asset == null)
					Debug.LogError(string.Format(
						"Resources.Load can't load [{0}]", filename));
				return asset.bytes;
			};
		}

		if (m_textureLoader == null) {
			m_textureLoader = (filename) => {
				Texture2D texture = (Texture2D)Resources.Load(filename, typeof(Texture2D));
				if (texture == null)
					Debug.LogError(string.Format(
						"Resources.Load can't load [{0}]", filename));
				texture.wrapMode = TextureWrapMode.Clamp;
				return texture;
			};
		}

		if (m_textureUnloader == null) {
			m_textureUnloader = (texture) => {
				if (!Application.isEditor)
					Resources.UnloadAsset(texture);
			};
		}
	}

	public bool IsLWFDataLoaded(string filename)
	{
		LWFDataItem item;
		return m_lwfDataCache.TryGetValue(filename, out item);
	}

	public Data LoadLWFData(string filename, LWFDataLoader lwfDataLoader = null)
	{
		LWFDataItem item;
		if (!m_lwfDataCache.TryGetValue(filename, out item)) {
			if (lwfDataLoader == null)
				lwfDataLoader = m_lwfDataLoader;
			byte[] bytes = lwfDataLoader(filename);
			if (bytes == null)
				return null;
			Data data = new Data(bytes);
			item = new LWFDataItem(data);
			m_lwfDataCache[filename] = item;
		}
		item.Ref();
		return item.Entity();
	}

	public void UnloadLWFData(string filename)
	{
		LWFDataItem item;
		if (m_lwfDataCache.TryGetValue(filename, out item)) {
			if (item.Unref() <= 0)
				m_lwfDataCache.Remove(filename);
		}
	}

	public Material LoadTexture(string lwfName, string filename, int format,
		bool useAdditionalColor, TextureLoader textureLoader = null,
		TextureUnloader textureUnloader = null, string shaderName = "LWF")
	{
		TextureItem item;
		string cacheName = lwfName + "/" + filename;
		if (!m_textureCache.TryGetValue(cacheName, out item)) {
			Shader shader = GetShader(shaderName);
			Material material = new Material(shader);
			if (useAdditionalColor)
				material.EnableKeyword("ENABLE_ADD_COLOR");
			material.SetInt("BlendEquation", (int)BlendOp.Add);

			int blendModeSrc;
			switch ((Format.Constant)format) {
			default:
			case Format.Constant.TEXTUREFORMAT_NORMAL:
				blendModeSrc = (int)BlendMode.SrcAlpha;
				break;

			case Format.Constant.TEXTUREFORMAT_PREMULTIPLIEDALPHA:
				blendModeSrc = (int)BlendMode.One;
				break;
			}
			material.SetInt("BlendModeSrc", blendModeSrc);
			material.SetInt("BlendModeDst", (int)BlendMode.OneMinusSrcAlpha);

			if (textureLoader == null)
				textureLoader = m_textureLoader;

			material.mainTexture = textureLoader(filename);
			if (material.mainTexture != null) {
				material.mainTexture.name =
					string.Format("LWF/{0}/{1}", lwfName, filename);
				material.name = material.mainTexture.name;
			}
			material.color = new UnityEngine.Color(1, 1, 1, 1);

			TextureContext context = new TextureContext(material,
				textureUnloader == null ? m_textureUnloader : textureUnloader);
			item = new TextureItem(context);
			m_textureCache[cacheName] = item;
		}
		item.Ref();
		return item.Entity().material;
	}

	public static Material CreateBlendMaterial(
		Material baseMaterial, bool premultipliedAlpha, int blendMode)
	{
		switch (blendMode) {
		case (int)Format.Constant.BLEND_MODE_ADD:
		case (int)Format.Constant.BLEND_MODE_MULTIPLY:
		case (int)Format.Constant.BLEND_MODE_SCREEN:
		case (int)Format.Constant.BLEND_MODE_SUBTRACT:
			Material material = new Material(baseMaterial);
			int blendModeSrc = 0;
			int blendModeDst = 0;
			switch (blendMode) {
			case (int)Format.Constant.BLEND_MODE_ADD:
				blendModeSrc = premultipliedAlpha ?
					(int)BlendMode.One : (int)BlendMode.SrcAlpha;
				blendModeDst = (int)BlendMode.One;
				break;

			case (int)Format.Constant.BLEND_MODE_MULTIPLY:
				blendModeSrc = (int)BlendMode.DstColor;
				blendModeDst = (int)BlendMode.OneMinusSrcAlpha;
				break;

			case (int)Format.Constant.BLEND_MODE_SCREEN:
				blendModeSrc = (int)BlendMode.OneMinusDstColor;
				blendModeDst = (int)BlendMode.One;
				break;

			case (int)Format.Constant.BLEND_MODE_SUBTRACT:
				blendModeSrc = premultipliedAlpha ?
					(int)BlendMode.One : (int)BlendMode.SrcAlpha;
				blendModeDst = (int)BlendMode.One;
				material.SetInt("BlendEquation", (int)BlendOp.ReverseSubtract);
				break;
			}
			material.SetInt("BlendModeSrc", blendModeSrc);
			material.SetInt("BlendModeDst", blendModeDst);
			return material;

		default:
			return null;
		}
	}

	public void UnloadTexture(string lwfName, string filename)
	{
		TextureItem item;
		string cacheName = lwfName + "/" + filename;
		if (m_textureCache.TryGetValue(cacheName, out item)) {
			if (item.Unref() <= 0) {
				TextureContext context = item.Entity();
				if (context.material.mainTexture != null)
					context.unloader((Texture2D)context.material.mainTexture);
				if (!Application.isEditor)
					Material.Destroy(context.material);
				m_textureCache.Remove(cacheName);
			}
		}
	}

	public MeshContext LoadMesh(string lwfName,
		Data data, Format.BitmapEx bitmapEx, int bitmapExId)
	{
		MeshItem item;
		string cacheName = string.Format("{0}/{1}", lwfName, bitmapExId);
		if (!m_meshCache.TryGetValue(cacheName, out item)) {

			Format.TextureFragment fragment =
				data.textureFragments[bitmapEx.textureFragmentId];
			Format.Texture texture = data.textures[fragment.textureId];

			float tw = (float)texture.width;
			float th = (float)texture.height;

			float x = (float)fragment.x;
			float y = - (float)fragment.y;
			float u = (float)fragment.u;
			float v = th - (float)fragment.v;
			float w = (float)fragment.w;
			float h = (float)fragment.h;

			float bu = bitmapEx.u * w;
			float bv = bitmapEx.v * h;
			float bw = bitmapEx.w;
			float bh = bitmapEx.h;

			x += bu;
			y += bv;
			u += bu;
			v += bv;
			w *= bw;
			h *= bh;

			float height = h / texture.scale;

			float x0 = x / texture.scale;
			float y0 = y / texture.scale;
			float x1 = (x + w) / texture.scale;
			float y1 = (y + h) / texture.scale;

			Mesh mesh = new Mesh();
			mesh.name = "LWF/" + cacheName;
			mesh.vertices = new Vector3[]{
				new Vector3(x1, y1, 0),
				new Vector3(x1, y0, 0),
				new Vector3(x0, y1, 0),
				new Vector3(x0, y0, 0),
			};

			if (fragment.rotated == 0) {
				float u0 = u / tw;
				float v0 = (v - h) / th;
				float u1 = (u + w) / tw;
				float v1 = v / th;
				mesh.uv = new Vector2[]{
					new Vector2(u1, v1),
					new Vector2(u1, v0),
					new Vector2(u0, v1),
					new Vector2(u0, v0),
				};
			} else {
				float u0 = u / tw;
				float v0 = (v - w) / th;
				float u1 = (u + h) / tw;
				float v1 = v / th;
				mesh.uv = new Vector2[]{
					new Vector2(u1, v0),
					new Vector2(u0, v0),
					new Vector2(u1, v1),
					new Vector2(u0, v1),
				};
			}

			mesh.triangles = new int[]{
				0, 1, 2,
				2, 1, 3,
			};
			mesh.RecalculateBounds();
			//mesh.Optimize();

			MeshContext context = new MeshContext(mesh, height);
			item = new MeshItem(context);
			m_meshCache[cacheName] = item;
		}
		item.Ref();
		return item.Entity();
	}

	public void UnloadMesh(string lwfName, int bitmapExId)
	{
		MeshItem item;
		string cacheName = string.Format("{0}/{1}", lwfName, bitmapExId);
		if (m_meshCache.TryGetValue(cacheName, out item)) {
			if (item.Unref() <= 0) {
				if (!Application.isEditor)
					Mesh.Destroy(item.Entity().mesh);
				m_meshCache.Remove(cacheName);
			}
		}
	}

	public Shader GetShader(string filename)
	{
		Shader shader;
		if (!m_shaderCache.TryGetValue(filename, out shader)) {
			shader = Shader.Find(filename);
			if (shader == null)
				Debug.LogError(
					string.Format("Shader.Find can't find [{0}]", filename));
			m_shaderCache[filename] = shader;
		}
		return shader;
	}

	public Mesh[] AddRenderedMesh(string lwfName, int frames)
	{
		RenderedMeshItem item;
		if (!m_renderedMeshCache.TryGetValue(lwfName, out item)) {
			item = new RenderedMeshItem(new Mesh[frames]);
			m_renderedMeshCache[lwfName] = item;
		}
		item.Ref();
		return item.Entity();
	}

	public void DeleteRenderedMesh(string lwfName)
	{
		RenderedMeshItem item;
		if (m_renderedMeshCache.TryGetValue(lwfName, out item)) {
			if (item.Unref() <= 0) {
				if (!Application.isEditor) {
					foreach (Mesh mesh in item.Entity()) {
						if (mesh != null)
							Mesh.Destroy(mesh);
					}
				}
				m_renderedMeshCache.Remove(lwfName);
			}
		}
	}

	public void UnloadAll()
	{
		foreach (RenderedMeshItem item in m_renderedMeshCache.Values) {
			if (!Application.isEditor) {
				foreach (Mesh mesh in item.Entity()) {
					if (mesh != null)
						Mesh.Destroy(mesh);
				}
			}
		}
		m_renderedMeshCache.Clear();
		m_lwfDataCache.Clear();
		if (!Application.isEditor) {
			foreach (MeshItem item in m_meshCache.Values)
				Mesh.Destroy(item.Entity().mesh);
		}
		m_meshCache.Clear();
		foreach (TextureItem item in m_textureCache.Values) {
			TextureContext context = item.Entity();
			if (context.material.mainTexture != null)
				context.unloader((Texture2D)context.material.mainTexture);
			if (!Application.isEditor)
				Material.Destroy(context.material);
		}
		m_textureCache.Clear();
		m_shaderCache.Clear();
	}

	public string Dump()
	{
		string dump = "LWFData:\n";
		foreach (string key in m_lwfDataCache.Keys)
			dump += "  " + key + "\n";
		dump += "Texture:\n";
		foreach (string key in m_textureCache.Keys)
			dump += "  " + key + "\n";
		dump += "Shader:\n";
		foreach (string key in m_shaderCache.Keys)
			dump += "  " + key + "\n";
		return dump;
	}
}

}	// namespace UnityRenderer
}	// namespace LWF
