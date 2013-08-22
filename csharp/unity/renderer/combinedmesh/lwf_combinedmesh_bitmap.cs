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

namespace LWF {
namespace CombinedMeshRenderer {

public partial class Factory : IRendererFactory
{
	private BitmapContext[] m_bitmapContexts;
	private BitmapContext[] m_bitmapExContexts;

	private void CreateBitmapContexts(Data data)
	{
		m_bitmapContexts = new BitmapContext[data.bitmaps.Length];
		for (int i = 0; i < data.bitmaps.Length; ++i) {
			Format.Bitmap bitmap = data.bitmaps[i];
			// Ignore null texture
			if (bitmap.textureFragmentId == -1)
				continue;
			Format.BitmapEx bitmapEx = new Format.BitmapEx();
			bitmapEx.matrixId = bitmap.matrixId;
			bitmapEx.textureFragmentId = bitmap.textureFragmentId;
			bitmapEx.u = 0;
			bitmapEx.v = 0;
			bitmapEx.w = 1;
			bitmapEx.h = 1;
			m_bitmapContexts[i] = new BitmapContext(this, i, data, bitmapEx);
		}

		m_bitmapExContexts = new BitmapContext[data.bitmapExs.Length];
		for (int i = 0; i < data.bitmapExs.Length; ++i) {
			Format.BitmapEx bitmapEx = data.bitmapExs[i];
			// Ignore null texture
			if (bitmapEx.textureFragmentId == -1)
				continue;
			m_bitmapExContexts[i] = new BitmapContext(this, i, data, bitmapEx);
		}
	}
}

public class BitmapContext
{
	public Factory factory;
	public int objectId;
	public Vector3[] vertices;
	public Vector2[] uv;
	public int[] triangles;
	public float height;
	public Format.Constant format;

	public BitmapContext(Factory f,
		int objId, Data data, Format.BitmapEx bitmapEx)
	{
		factory = f;
		Format.TextureFragment fragment =
			data.textureFragments[bitmapEx.textureFragmentId];
		Format.Texture texture = data.textures[fragment.textureId];

		objectId = objId;
		format = (Format.Constant)texture.format;

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

		height = h / texture.scale;

		float x0 = x / texture.scale;
		float y0 = y / texture.scale;
		float x1 = (x + w) / texture.scale;
		float y1 = (y + h) / texture.scale;

		vertices = new Vector3[]{
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
			uv = new Vector2[]{
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
			uv = new Vector2[]{
				new Vector2(u1, v0),
				new Vector2(u0, v0),
				new Vector2(u1, v1),
				new Vector2(u0, v1),
			};
		}

		triangles = new int[]{
			0, 1, 2,
			2, 1, 3,
		};
	}
}

public class BitmapRenderer : Renderer
{
	BitmapContext m_context;
	Matrix4x4 m_matrix;
	UnityEngine.Color m_colorMult;
#if LWF_USE_ADDITIONALCOLOR
	UnityEngine.Color m_colorAdd;
#endif
	bool m_available;

	static Color32 s_clearColor = new Color32(0, 0, 0, 0);

	public BitmapRenderer(LWF lwf, BitmapContext context) : base(lwf)
	{
		m_context = context;
		m_matrix = new Matrix4x4();
		m_colorMult = new UnityEngine.Color();
#if LWF_USE_ADDITIONALCOLOR
		m_colorAdd = new UnityEngine.Color();
#endif
		m_available = false;

		if (m_context != null)
			m_context.factory.AddBitmap();
	}

	public override void Destruct()
	{
		if (m_context != null)
			m_context.factory.DeleteBitmap();
		base.Destruct();
	}

	public override void Render(Matrix matrix, ColorTransform colorTransform,
		int renderingIndex, int renderingCount, bool visible)
	{
		// Ignore null texture
		if (m_context == null)
			return;

		Factory factory = m_context.factory;
		CombinedMeshBuffer buffer = factory.buffer;
		int bufferIndex = buffer.index++;

		if (!factory.updated)
			return;

		if (!visible)
			goto invisible;

#if LWF_USE_ADDITIONALCOLOR
		factory.ConvertColorTransform(
			ref m_colorMult, ref m_colorAdd, colorTransform);
#else
		factory.ConvertColorTransform(ref m_colorMult, colorTransform);
#endif
		if (m_colorMult.a <= 0)
			goto invisible;
		if (factory.premultipliedAlpha) {
			m_colorMult.r *= m_colorMult.a;
			m_colorMult.g *= m_colorMult.a;
			m_colorMult.b *= m_colorMult.a;
		}
		Color32 color32 = m_colorMult;

		factory.ConvertMatrix(ref m_matrix, matrix, 1,
			renderingCount - renderingIndex, m_context.height);

		int index = bufferIndex * 4;
		Color32 bc = buffer.colors32[index];
		if (bc.r != color32.r ||
				bc.g != color32.g ||
				bc.b != color32.b ||
				bc.a != color32.a) {
			for (int i = 0; i < 4; ++i)
				buffer.colors32[index + i] = color32;
		}

		if (!buffer.clean && m_available &&
				buffer.objects[bufferIndex] == m_context.objectId) {
			index = bufferIndex * 4;
			for (int i = 0; i < 4; ++i) {
				buffer.vertices[index + i] =
					m_matrix.MultiplyPoint3x4(m_context.vertices[i]);
			}
			return;
		}

		buffer.objects[bufferIndex] = m_context.objectId;

		index = bufferIndex * 4;
		for (int i = 0; i < 4; ++i) {
			buffer.vertices[index + i] =
				m_matrix.MultiplyPoint3x4(m_context.vertices[i]);
			buffer.uv[index + i] = m_context.uv[i];
		}

		int offset = bufferIndex * 4;
		index = bufferIndex * 6;
		for (int i = 0; i < 6; ++i)
			buffer.triangles[index + i] = m_context.triangles[i] + offset;

		buffer.changed = true;
		m_available = true;
		return;

invisible:
		factory.ConvertMatrix(ref m_matrix, matrix, 1,
							  renderingCount - renderingIndex, m_context.height);
		Vector3 v = m_matrix.MultiplyPoint3x4(m_context.vertices[0]);
		index = bufferIndex * 4;
		for (int i = 0; i < 4; ++i) {
			buffer.vertices[index + i] = v;
			buffer.colors32[index + i] = s_clearColor;
		}
		m_available = false;
	}
}

}	// namespace CombinedMeshRenderer
}	// namespace LWF
