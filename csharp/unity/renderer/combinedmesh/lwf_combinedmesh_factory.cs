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

using ResourceCache = LWF.UnityRenderer.ResourceCache;
using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;

namespace LWF {
namespace CombinedMeshRenderer {

public class CombinedMeshBuffer
{
	public int[] objects;
	public Vector3[] vertices;
	public Vector2[] uv;
	public int[] triangles;
	public Color32[] colors32;
	public int index;
	public bool clean;
	public bool changed;

	public void Alloc(int n)
	{
		objects = new int[n];
		vertices = new Vector3[n * 4];
		uv = new Vector2[n * 4];
		triangles = new int[n * 6];
		colors32 = new Color32[n * 4];
		index = 0;
		clean = true;
		changed = true;
	}
}

public partial class Factory : UnityRenderer.Factory
{
	public UnityEngine.MeshRenderer meshRenderer;
	public MeshFilter meshFilter;
	public CombinedMeshBuffer buffer;
	public Mesh mesh;
	public bool updated;
	public bool premultipliedAlpha;
	private Data data;
	private string textureName;
	private int updateCount;
	private int bitmapCount;

	public Factory(Data d, GameObject gObj,
			float zOff = 0, float zR = 1, int rQOff = 0, Camera cam = null,
			string texturePrfx = "", string fontPrfx = "",
			TextureLoader textureLdr = null,
			TextureUnloader textureUnldr = null)
		: base(gObj, zOff, zR, rQOff,
			cam, texturePrfx, fontPrfx, textureLdr, textureUnldr)
	{
		data = d;
		mesh = new Mesh();
		mesh.name = "LWF/" + data.name;
#if !UNITY_3_5
		mesh.MarkDynamic();
#endif

		meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = mesh;

		meshRenderer = gameObject.AddComponent<UnityEngine.MeshRenderer>();
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;

		textureName = texturePrefix + data.textures[0].filename;
		meshRenderer.sharedMaterial =
			ResourceCache.SharedInstance().LoadTexture(
				data.name, textureName, data.textures[0].format,
					textureLoader, textureUnloader);
		if (renderQueueOffset != 0)
			meshRenderer.sharedMaterial.renderQueue += renderQueueOffset;

		premultipliedAlpha = (data.textures[0].format ==
			(int)Format.Constant.TEXTUREFORMAT_PREMULTIPLIEDALPHA);

		buffer = new CombinedMeshBuffer();

		CreateBitmapContexts(data);
	}

	public override void Destruct()
	{
		meshRenderer.sharedMaterial = null;
		ResourceCache.SharedInstance().UnloadTexture(data.name, textureName);
		meshFilter.sharedMesh = null;
		UnityEngine.MeshRenderer.Destroy(meshRenderer);
		MeshFilter.Destroy(meshFilter);
		Mesh.Destroy(mesh);
		base.Destruct();
	}

	public void AddBitmap()
	{
		++bitmapCount;
	}

	public void DeleteBitmap()
	{
		--bitmapCount;
	}

	public override void BeginRender(LWF lwf)
	{
		base.BeginRender(lwf);

		if (buffer.objects == null || buffer.objects.Length != bitmapCount) {
			updated = true;
			updateCount = lwf.updateCount;
			buffer.Alloc(bitmapCount);
		} else {
			buffer.clean = false;
			buffer.changed = false;
			if (updateCount != lwf.updateCount) {
				updated = true;
				updateCount = lwf.updateCount;
			} else {
				updated = false;
			}
		}

		buffer.index = 0;
	}

	public override void EndRender(LWF lwf)
	{
		base.EndRender(lwf);

		if (!updated)
			return;

		if (buffer.index == 0) {
			if (mesh.vertices != null && mesh.vertices.Length > 0)
				mesh.Clear(true);
			return;
		}

		if (buffer.changed) {
			mesh.Clear(true);
			mesh.vertices = buffer.vertices;
			mesh.uv = buffer.uv;
			mesh.triangles = buffer.triangles;
		} else {
			mesh.vertices = buffer.vertices;
		}
		mesh.colors32 = buffer.colors32;
		mesh.RecalculateBounds();
		//mesh.Optimize();
	}

	public override Renderer ConstructBitmap(
		LWF lwf, int objectId, Bitmap bitmap)
	{
		return new BitmapRenderer(lwf, m_bitmapContexts[objectId]);
	}

	public override Renderer ConstructBitmapEx(
		LWF lwf, int objectId, BitmapEx bitmapEx)
	{
		return new BitmapRenderer(lwf, m_bitmapExContexts[objectId]);
	}

	public override TextRenderer ConstructText(LWF lwf, int objectId, Text text)
	{
		return new UnityRenderer.UnityTextRenderer(lwf, objectId);
	}
}

}	// namespace CombinedMeshRenderer
}	// namespace LWF
