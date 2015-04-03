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

using ResourceCache = LWF.UnityRenderer.ResourceCache;

namespace LWF {
namespace UIVertexRenderer {

public partial class Factory : IRendererFactory
{
	private BitmapContext[] m_bitmapContexts;
	private BitmapContext[] m_bitmapExContexts;

	private void CreateBitmapContexts()
	{
		m_bitmapContexts = new BitmapContext[data.bitmaps.Length];
		for (int i = 0; i < data.bitmaps.Length; ++i) {
			Format.Bitmap bitmap = data.bitmaps[i];
			// Ignore null texture
			if (bitmap.textureFragmentId == -1)
				continue;
			int bitmapExId = -i - 1;
			Format.BitmapEx bitmapEx = new Format.BitmapEx();
			bitmapEx.matrixId = bitmap.matrixId;
			bitmapEx.textureFragmentId = bitmap.textureFragmentId;
			bitmapEx.u = 0;
			bitmapEx.v = 0;
			bitmapEx.w = 1;
			bitmapEx.h = 1;
			m_bitmapContexts[i] =
				new BitmapContext(this, data, bitmapEx, i, bitmapExId);
		}

		m_bitmapExContexts = new BitmapContext[data.bitmapExs.Length];
		for (int i = 0; i < data.bitmapExs.Length; ++i) {
			Format.BitmapEx bitmapEx = data.bitmapExs[i];
			// Ignore null texture
			if (bitmapEx.textureFragmentId == -1)
				continue;
			m_bitmapExContexts[i] =
				new BitmapContext(this, data, bitmapEx, i, i);
		}
	}

	private void DestructBitmapContexts()
	{
		for (int i = 0; i < m_bitmapContexts.Length; ++i)
			if (m_bitmapContexts[i] != null)
				m_bitmapContexts[i].Destruct();
		for (int i = 0; i < m_bitmapExContexts.Length; ++i)
			if (m_bitmapExContexts[i] != null)
				m_bitmapExContexts[i].Destruct();
	}
}

public class BitmapContext
{
	private Factory m_factory;
	private Material m_material;
	private Data m_data;
	private float m_height;
	private Vector3[] m_vertices;
	private Vector2[] m_uv;
	private Format.Constant m_format;
	private string m_textureName;
	private int m_bitmapExId;
	private bool m_premultipliedAlpha;

	public Factory factory {get {return m_factory;}}
	public Material material {get {return m_material;}}
	public Data data {get {return m_data;}}
	public string textureName {get {return m_textureName;}}
	public float height {get {return m_height;}}
	public Vector3[] vertices {get {return m_vertices;}}
	public Vector2[] uv {get {return m_uv;}}
	public Format.Constant format {get {return m_format;}}
	public int bitmapExId {get {return m_bitmapExId;}}
	public bool premultipliedAlpha {get {return m_premultipliedAlpha;}}

	public BitmapContext(Factory f,
		Data d, Format.BitmapEx bitmapEx, int objId, int bId)
	{
		m_factory = f;
		m_data = d;
		m_bitmapExId = bId;

		Format.TextureFragment fragment =
			data.textureFragments[bitmapEx.textureFragmentId];
		Format.Texture texture = data.textures[fragment.textureId];

		m_textureName = factory.texturePrefix + texture.filename;
		if (LWF.GetTextureLoadHandler() != null)
			m_textureName = LWF.GetTextureLoadHandler()(
				m_textureName, factory.texturePrefix, texture.filename);

		m_premultipliedAlpha = (texture.format ==
			(int)Format.Constant.TEXTUREFORMAT_PREMULTIPLIEDALPHA);

		m_material = ResourceCache.SharedInstance().LoadTexture(
			data.name, m_textureName, texture.format,
			factory.useAdditionalColor, factory.textureLoader,
			factory.textureUnloader, factory.shaderName);
		if (factory.renderQueueOffset != 0)
			m_material.renderQueue += factory.renderQueueOffset;

		m_format = (Format.Constant)texture.format;

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

		m_height = h / texture.scale;

		float x0 = x / texture.scale;
		float y0 = y / texture.scale;
		float x1 = (x + w) / texture.scale;
		float y1 = (y + h) / texture.scale;

		m_vertices = new Vector3[]{
			new Vector3(x1, y1, 0),
			new Vector3(x1, y0, 0),
			new Vector3(x0, y0, 0),
			new Vector3(x0, y1, 0),
		};

		if (fragment.rotated == 0) {
			float u0 = u / tw;
			float v0 = (v - h) / th;
			float u1 = (u + w) / tw;
			float v1 = v / th;
			m_uv = new Vector2[]{
				new Vector2(u1, v1),
				new Vector2(u1, v0),
				new Vector2(u0, v0),
				new Vector2(u0, v1),
			};
		} else {
			float u0 = u / tw;
			float v0 = (v - w) / th;
			float u1 = (u + h) / tw;
			float v1 = v / th;
			m_uv = new Vector2[]{
				new Vector2(u1, v0),
				new Vector2(u0, v0),
				new Vector2(u0, v1),
				new Vector2(u1, v1),
			};
		}
	}

	public void Destruct()
	{
		ResourceCache.SharedInstance().UnloadTexture(
			m_data.name, m_textureName);
	}
}

public class BitmapRenderer : Renderer, IMeshRenderer
{
	BitmapContext m_context;
	Material m_material;
	Matrix m_matrix;
	Matrix4x4 m_matrixForRender;
	UnityEngine.Color m_colorMult;
	UnityEngine.Color m_colorAdd;
	int m_blendMode;
	int m_z;
	int m_bufferIndex;
	bool m_updated;
	UIVertexBuffer m_buffer;

	public BitmapRenderer(LWF lwf, BitmapContext context) : base(lwf)
	{
		m_context = context;
		m_matrix = new Matrix(0, 0, 0, 0, 0, 0);
		m_matrixForRender = new Matrix4x4();
		m_colorMult = new UnityEngine.Color();
		m_colorAdd = new UnityEngine.Color();
		m_blendMode = (int)Format.Constant.BLEND_MODE_NORMAL;
		m_z = -1;
		m_updated = false;
		m_buffer = null;
		m_bufferIndex = -1;
	}

	public override void Destruct()
	{
		if (m_material != null) {
			Material.Destroy(m_material);
			m_material = null;
		}
	}

	public override void Render(Matrix matrix, ColorTransform colorTransform,
		int renderingIndex, int renderingCount, bool visible)
	{
		// Ignore null texture
		if (m_context == null)
			return;

		if (!visible)
			return;

		Factory factory = m_context.factory;
		factory.ConvertColorTransform(
			ref m_colorMult, ref m_colorAdd, colorTransform);
		if (m_colorMult.a <= 0)
			return;

		if (m_context.premultipliedAlpha) {
			m_colorMult.r *= m_colorMult.a;
			m_colorMult.g *= m_colorMult.a;
			m_colorMult.b *= m_colorMult.a;
		}

		m_updated = m_matrix.SetWithComparing(matrix);

		int z = renderingCount - renderingIndex;
		if (m_z != z) {
			m_updated = true;
			m_z = z;
		}

		if (m_updated) {
			factory.ConvertMatrix(
				ref m_matrixForRender, matrix, 1, z, m_context.height);
		}

		if (m_blendMode != factory.blendMode) {
			m_blendMode = factory.blendMode;
			if (m_material != null) {
				Material.Destroy(m_material);
				m_material = null;
			}

			m_material = ResourceCache.CreateBlendMaterial(
				m_context.material, m_context.premultipliedAlpha, m_blendMode);
		}

		Material material =
			m_material == null ? m_context.material : m_material;

		factory.Render(this, 1, material, m_colorAdd);
	}

	void IMeshRenderer.UpdateMesh(UIVertexBuffer buffer)
	{
		int bufferIndex = buffer.index++;

		Color32 color32 = m_colorMult;

		int index = bufferIndex * 4;
		Color32 bc = buffer.vertices[index].color;
		if (buffer.initialized ||
				bc.r != color32.r ||
				bc.g != color32.g ||
				bc.b != color32.b ||
				bc.a != color32.a) {
			buffer.modified = true;
			for (int i = 0; i < 4; ++i)
				buffer.vertices[index + i].color = color32;
		}

		if (m_updated || m_buffer != buffer ||
				m_bufferIndex != bufferIndex || buffer.initialized) {
			m_buffer = buffer;
			m_bufferIndex = bufferIndex;
			buffer.modified = true;
			for (int i = 0; i < 4; ++i) {
				buffer.vertices[index + i].uv0 = m_context.uv[i];
				buffer.vertices[index + i].position =
					m_matrixForRender.MultiplyPoint3x4(m_context.vertices[i]);
			}
		}
	}
}

}	// namespace UIVertexRenderer
}	// namespace LWF
