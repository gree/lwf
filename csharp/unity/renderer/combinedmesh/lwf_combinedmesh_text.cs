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

namespace LWF {
namespace CombinedMeshRenderer {

public class TextMeshRenderer : UnityRenderer.UnityTextRenderer, IMeshRenderer
{
	Matrix m_matrix;
	Matrix4x4 m_matrixForRender;
	UnityEngine.Color m_colorMult;
	UnityEngine.Color m_colorAdd;
	Color32 m_color;
	int m_z;
	int m_bufferIndex;
	bool m_updated;
	CombinedMeshBuffer m_buffer;

	public TextMeshRenderer(LWF lwf, UnityRenderer.TextContext context)
		: base(lwf, context)
	{
		m_matrix = new Matrix(0, 0, 0, 0, 0, 0);
		m_matrixForRender = new Matrix4x4();
		m_colorMult = new UnityEngine.Color();
		m_colorAdd = new UnityEngine.Color();
		m_color = new Color32();
		m_z = -1;
		m_updated = false;
		m_buffer = null;
		m_bufferIndex = -1;
	}

	public override void Render(Matrix matrix, ColorTransform colorTransform,
		int renderingIndex, int renderingCount, bool visible)
	{
		if (!visible || m_empty)
			return;

		Factory factory = (Factory)m_context.factory;
		factory.ConvertColorTransform(
			ref m_colorMult, ref m_colorAdd, colorTransform);
		if (m_colorMult.a <= 0)
			return;

		m_color = m_colors32[0] * m_colorMult + m_colorAdd;

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

		factory.Render(this, m_vertices.Length / 4,
			m_context.settings.font.material, m_colorAdd);
	}

	void IMeshRenderer.UpdateMesh(CombinedMeshBuffer buffer)
	{
		int bufferIndex = buffer.index;
		int vertexCount = m_vertices.Length;
		buffer.index += vertexCount / 4;

		for (int i = bufferIndex; i < buffer.index; ++i) {
			int cIndex = i * 4;
			var bc = buffer.colors32[cIndex];
			if (bc.r != m_color.r ||
					bc.g != m_color.g ||
					bc.b != m_color.b ||
					bc.a != m_color.a) {
				buffer.modified = true;
				for (int j = 0; j < 4; ++j)
					buffer.colors32[cIndex + j] = m_color;
			}
		}

		if (m_updated || m_buffer != buffer ||
				m_bufferIndex != bufferIndex || buffer.initialized) {
			m_buffer = buffer;
			m_bufferIndex = bufferIndex;
			buffer.modified = true;
			int index = bufferIndex * 4;
			for (int i = 0; i < vertexCount; ++i) {
				buffer.uv[index + i] = m_uv[i];
				buffer.vertices[index + i] =
					m_matrixForRender.MultiplyPoint3x4(m_vertices[i]);
			}
		}
	}
}

}	// namespace CombinedMeshRenderer
}	// namespace LWF
