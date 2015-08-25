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
using System.Collections.Generic;

namespace LWF {
namespace UnityRenderer {

using Align = Format.TextProperty.Align;

public partial class Factory : IRendererFactory
{
	protected TextContext[] m_textContexts;

	protected void CreateTextContexts()
	{
		m_textContexts = new TextContext[data.texts.Length];
		for (int i = 0; i < data.texts.Length; ++i)
			m_textContexts[i] = new TextContext(this, data, data.texts[i]);
	}

	protected void DestructTextContexts()
	{
		for (int i = 0; i < m_textContexts.Length; ++i)
			if (m_textContexts[i] != null)
				m_textContexts[i].Destruct();
	}
}

public class TextContext
{
	private Factory m_factory;
	private TextGenerationSettings m_textGenerationSettings;
	private float m_height;

	public Factory factory {get {return m_factory;}}
	public TextGenerationSettings settings
		{get {return m_textGenerationSettings;}}
	public float height {get {return m_height;}}

	public TextContext(Factory f, Data data, Format.Text text)
	{
		m_factory = f;
		Format.TextProperty textProperty =
			data.textProperties[text.textPropertyId];
		Format.Font fontProperty = data.fonts[textProperty.fontId];

		string fontName = data.strings[fontProperty.stringId];
		float fontHeight = (float)textProperty.fontHeight;
		float width = (float)text.width;
		m_height = (float)text.height;
#if !UNITY_4_5
		float lineSpacing = 1.0f + (float)textProperty.leading / fontHeight;
#endif
		//float letterSpacing = fontProperty.letterspacing;
		float leftMargin = textProperty.leftMargin / fontHeight;
		float rightMargin = textProperty.rightMargin / fontHeight;

		var font = Resources.Load<Font>(fontName);
		if (font == null)
			font = Resources.GetBuiltinResource<Font>("Arial.ttf");

		int va = textProperty.align & (int)Align.VERTICAL_MASK;
		int a = textProperty.align & (int)Align.ALIGN_MASK;
		TextAnchor textAnchor = TextAnchor.UpperLeft;
		switch (va) {
		default:
			switch (a) {
			default:
			case (int)Align.LEFT:
				textAnchor = TextAnchor.UpperLeft;
				break;
			case (int)Align.RIGHT:
				textAnchor = TextAnchor.UpperRight;
				break;
			case (int)Align.CENTER:
				textAnchor = TextAnchor.UpperCenter;
				break;
			}
			break;

		case (int)Align.VERTICAL_BOTTOM:
			switch (a) {
			default:
			case (int)Align.LEFT:
				textAnchor = TextAnchor.LowerLeft;
				break;
			case (int)Align.RIGHT:
				textAnchor = TextAnchor.LowerRight;
				break;
			case (int)Align.CENTER:
				textAnchor = TextAnchor.LowerCenter;
				break;
			}
			break;

		case (int)Align.VERTICAL_MIDDLE:
			switch (a) {
			default:
			case (int)Align.LEFT:
				textAnchor = TextAnchor.MiddleLeft;
				break;
			case (int)Align.RIGHT:
				textAnchor = TextAnchor.MiddleRight;
				break;
			case (int)Align.CENTER:
				textAnchor = TextAnchor.MiddleCenter;
				break;
			}
			break;
		}

		var s = new TextGenerationSettings();
#if UNITY_4_5
		s.anchor = textAnchor;
		s.extents = new Vector2(width - leftMargin - rightMargin, m_height);
		s.style = FontStyle.Normal;
		s.size = (int)fontHeight;
		s.wrapMode = TextWrapMode.Wrap;
#else
		s.textAnchor = textAnchor;
		s.generationExtents =
			new Vector2(width - leftMargin - rightMargin, m_height);
		s.fontStyle = FontStyle.Normal;
		s.fontSize = (int)fontHeight;
		s.lineSpacing = lineSpacing;
		s.horizontalOverflow = HorizontalWrapMode.Wrap;
		s.verticalOverflow = VerticalWrapMode.Overflow;
		s.scaleFactor = 1.0f;
#endif
		s.color = factory.ConvertColor(data.colors[text.colorId]);
		s.pivot = new Vector2(-leftMargin, 0);
		s.richText = true;
		s.font = font;
		m_textGenerationSettings = s;
	}

	public void Destruct()
	{
	}
}

public class UnityTextRenderer : TextRenderer
{
	protected TextContext m_context;
	protected TextGenerator m_textGenerator;
	protected Vector3[] m_vertices;
	protected Vector2[] m_uv;
	protected Color32[] m_colors32;
	protected bool m_empty;

	public UnityTextRenderer(LWF lwf, TextContext context) : base(lwf)
	{
		m_context = context;
		m_textGenerator = new TextGenerator();
		m_empty = true;
	}

	public override void SetText(string text)
	{
		if (string.IsNullOrEmpty(text)) {
			m_empty = true;
			return;
		}

		m_empty = false;
		m_textGenerator.Populate(text, m_context.settings);

		var n = m_textGenerator.verts.Count;
		m_vertices = new Vector3[n];
		m_uv = new Vector2[n];
		m_colors32 = new Color32[n];
		var table = new int[]{1, 2, 0, 3};
		for (int i = 0; i < n; ++i) {
			int j = (int)(i / 4) * 4;
			int k = table[i % 4];
			m_vertices[i] = m_textGenerator.verts[j + k].position;
#if UNITY_4_5
			m_uv[i] = m_textGenerator.verts[j + k].uv;
#else
			m_uv[i] = m_textGenerator.verts[j + k].uv0;
#endif
			m_colors32[i] = m_textGenerator.verts[j + k].color;
		}
	}
}

}	// namespace UnityRenderer
}	// namespace LWF
