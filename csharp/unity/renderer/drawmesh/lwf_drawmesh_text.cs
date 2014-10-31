/*
 * Copyright (C) 2014 GREE, Inc.
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
using System.Collections.Generic;
using LWF.UnityRenderer;

namespace LWF {
namespace DrawMeshRenderer {

public class TextMeshRenderer : UnityTextRenderer
{
	private int[] m_triangles;
	private Mesh m_mesh;
	private Matrix4x4 m_matrix;
	private Matrix4x4 m_renderMatrix;
	private UnityEngine.Color m_colorMult;
	private UnityEngine.Color m_colorAdd;
	private Color32 m_color;
#if UNITY_EDITOR
	private bool m_visible;
#endif

	public TextMeshRenderer(LWF lwf, TextContext context) : base(lwf, context)
	{
		m_mesh = new Mesh();
		m_matrix = new Matrix4x4();
		m_renderMatrix = new Matrix4x4();
		m_colorMult = new UnityEngine.Color();
		m_colorAdd = new UnityEngine.Color();
		m_color = new Color32();
	}

	public override void Destruct()
	{
		Mesh.Destroy(m_mesh);
		base.Destruct();
	}

	public override void SetText(string text)
	{
		base.SetText(text);

		if (m_empty)
			return;

		m_color = m_colors32[0];

		var tn = m_vertices.Length / 4 * 6;
		m_triangles = new int[tn];
		for (int i = 0, j = 0; i < tn; i += 6, j += 4) {
			m_triangles[i + 0] = j + 0;
			m_triangles[i + 1] = j + 1;
			m_triangles[i + 2] = j + 2;
			m_triangles[i + 3] = j + 2;
			m_triangles[i + 4] = j + 1;
			m_triangles[i + 5] = j + 3;
		}
	}

	public override void Render(Matrix matrix, ColorTransform colorTransform,
		int renderingIndex, int renderingCount, bool visible)
	{
#if UNITY_EDITOR
		m_visible = visible;
#endif
		if (!visible || m_empty)
			return;

		Factory factory = (Factory)m_context.factory;
		factory.ConvertMatrix(ref m_matrix, matrix, 1,
			renderingCount - renderingIndex, m_context.height);
		Factory.MultiplyMatrix(ref m_renderMatrix,
			factory.gameObject.transform.localToWorldMatrix, m_matrix);

		factory.ConvertColorTransform(
			ref m_colorMult, ref m_colorAdd, colorTransform);

		var color = m_color * m_colorMult + m_colorAdd;
		for (int i = 0; i < m_colors32.Length; ++i)
			m_colors32[i] = color;

		m_mesh.Clear(true);
		m_mesh.vertices = m_vertices;
		m_mesh.uv = m_uv;
		m_mesh.triangles = m_triangles;
		m_mesh.colors32 = m_colors32;

		Graphics.DrawMesh(m_mesh,
			m_renderMatrix, m_context.settings.font.material, 0);
	}

#if UNITY_EDITOR
	public override void RenderNow()
	{
		if (!m_visible || m_empty)
			return;

		m_context.settings.font.material.SetPass(0);
		Graphics.DrawMeshNow(m_mesh, m_renderMatrix);
	}
#endif
}

}	// namespace DrawMeshRenderer
}	// namespace LWF
