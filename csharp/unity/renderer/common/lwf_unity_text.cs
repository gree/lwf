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

public class TextContextTable
{
	public TextContext[] contexts;
	public Dictionary<string, List<TextContext>> map;

	public TextContextTable(int n)
	{
		contexts = new TextContext[n];
		map = new Dictionary<string, List<TextContext>>();
	}
}

public partial class Factory : IRendererFactory
{
	protected TextContextTable m_defaultTable;
	protected Dictionary<string, TextContextTable> m_movieTables;
	protected Data m_data;

	protected void CreateTextContexts(Data data)
	{
		m_data = data;
		m_defaultTable = new TextContextTable(m_data.texts.Length);

		for (int i = 0; i < m_data.texts.Length; ++i) {
			Format.Text text = m_data.texts[i];
			if (text.nameStringId != -1) {
				TextContext context =
					new TextContext(this, gameObject, m_data, text);
				m_defaultTable.contexts[i] = context;
				string textName = m_data.strings[text.nameStringId];
				List<TextContext> contexts;
				if (!m_defaultTable.map.TryGetValue(textName, out contexts))
					m_defaultTable.map[textName] = new List<TextContext>();
				m_defaultTable.map[textName].Add(context);
			}
		}
	}

	public void UseTextWithMovie(string fullPath)
	{
		if (m_movieTables == null)
			m_movieTables = new Dictionary<string, TextContextTable>();

		TextContextTable table;
		if (!m_movieTables.TryGetValue(fullPath, out table)) {
			table = new TextContextTable(m_data.texts.Length);
			m_movieTables[fullPath] = table;
		}

		for (int i = 0; i < m_data.texts.Length; ++i) {
			Format.Text text = m_data.texts[i];
			if (text.nameStringId != -1) {
				TextContext context =
					new TextContext(this, gameObject, m_data, text);
				table.contexts[i] = context;
				string textName = m_data.strings[text.nameStringId];
				List<TextContext> contexts;
				if (!table.map.TryGetValue(textName, out contexts))
					table.map[textName] = new List<TextContext>();
				table.map[textName].Add(context);
			}
		}
	}

	public virtual void Destruct()
	{
		for (int i = 0; i < m_data.texts.Length; ++i) {
			if (m_defaultTable.contexts[i] != null)
				m_defaultTable.contexts[i].Destruct();

			if (m_movieTables != null) {
				foreach (TextContextTable table in m_movieTables.Values) {
					if (table.contexts[i] != null)
						table.contexts[i].Destruct();
				}
			}
		}
	}

	public bool SetText(string fullPath,
		string name, string text, UnityEngine.Color[] colors = null)
	{
		TextContextTable table;
		if (m_movieTables == null ||
				!m_movieTables.TryGetValue(fullPath, out table))
			return false;

		return SetTextInternal(table, name, text, colors);
	}

	public bool SetText(
		string name, string text, UnityEngine.Color[] colors = null)
	{
		return SetTextInternal(m_defaultTable, name, text, colors);
	}

	private bool SetTextInternal(TextContextTable table,
		string name, string text, UnityEngine.Color[] colors = null)
	{
		List<TextContext> contexts;
		if (table == null ||
				!table.map.TryGetValue(name, out contexts) || contexts == null)
			return false;

		bool result = true;
		foreach (TextContext context in contexts) {
			if (context.bitmapFontRenderer != null) {
				if (colors == null) {
					if (!context.bitmapFontRenderer.SetText(
							text, context.color))
						result = false;
				} else {
					if (!context.bitmapFontRenderer.SetText(text, colors))
						result = false;
				}
			} else if (context.systemFontRenderer != null) {
				if (!context.systemFontRenderer.SetText(text, context.color))
					result = false;
			} else {
				result = false;
			}
		}
		return result;
	}

	public string GetText(string fullPath, string name)
	{
		TextContextTable table;
		if (m_movieTables == null ||
				!m_movieTables.TryGetValue(fullPath, out table))
			return null;

		return GetTextInternal(table, name);
	}

	public string GetText(string name)
	{
		return GetTextInternal(m_defaultTable, name);
	}

	private string GetTextInternal(TextContextTable table, string name)
	{
		List<TextContext> contexts;
		if (table == null ||
				!table.map.TryGetValue(name, out contexts) || contexts == null)
			return null;

		if (contexts[0].bitmapFontRenderer != null)
			return contexts[0].bitmapFontRenderer.GetText();
		if (contexts[0].systemFontRenderer != null)
			return contexts[0].systemFontRenderer.GetText();
		return null;
	}

	public TextContext GetTextContext(int objectId, Text text)
	{
		if (m_movieTables != null) {
			string fullPath = text.parent.GetFullName();
			if (fullPath != null) {
				for (;;) {
					TextContextTable table;
					if (m_movieTables.TryGetValue(fullPath, out table))
						return table.contexts[objectId];
					int i = fullPath.LastIndexOf('.');
					if (i == -1)
						break;
					fullPath = fullPath.Remove(i);
				}
			}
		}

		return m_defaultTable == null ? null :
			m_defaultTable.contexts[objectId];
	}
}

public class TextContext
{
	public Factory factory;
	public GameObject parent;
	public BitmapFont.Renderer bitmapFontRenderer;
	public ISystemFontRenderer systemFontRenderer;
	public ISystemFontRenderer.Parameter systemFontRendererParameter;
	public UnityEngine.Color color;

	public TextContext(Factory f, GameObject p, Data data, Format.Text text)
	{
		factory = f;
		parent = p;

		Format.TextProperty textProperty =
			data.textProperties[text.textPropertyId];
		Format.Font fontProperty = data.fonts[textProperty.fontId];
		color = factory.ConvertColor(data.colors[text.colorId]);

		string str = data.strings[text.stringId];
		string fontName = data.strings[fontProperty.stringId];
		string fontPath = factory.fontPrefix + fontName;
		float fontHeight = (float)textProperty.fontHeight;
		float width = (float)text.width;
		float height = (float)text.height;
		float lineSpacing = 1.0f + (float)textProperty.leading / fontHeight;
		float letterSpacing = fontProperty.letterspacing;
		float tabSpacing = 4.0f;
		float leftMargin = textProperty.leftMargin / fontHeight;
		float rightMargin = textProperty.rightMargin / fontHeight;

		if (fontName.StartsWith("_")) {

			ISystemFontRenderer.Style style;
			if (fontName == "_bold")
				style = ISystemFontRenderer.Style.BOLD;
			else if (fontName == "_italic")
				style = ISystemFontRenderer.Style.ITALIC;
			else if (fontName == "_bold_italic")
				style = ISystemFontRenderer.Style.BOLD_ITALIC;
			else
				style = ISystemFontRenderer.Style.NORMAL;

			ISystemFontRenderer.Align align;
			int a = textProperty.align & (int)Align.ALIGN_MASK;
			switch (a) {
			default:
			case (int)Align.LEFT:
				align = ISystemFontRenderer.Align.LEFT;   break;
			case (int)Align.RIGHT:
				align = ISystemFontRenderer.Align.RIGHT;  break;
			case (int)Align.CENTER:
				align = ISystemFontRenderer.Align.CENTER; break;
			}

			ISystemFontRenderer.VerticalAlign valign;
			int va = textProperty.align & (int)Align.VERTICAL_MASK;
			switch (va) {
			default:
				valign = ISystemFontRenderer.VerticalAlign.TOP;
				break;
			case (int)Align.VERTICAL_BOTTOM:
				valign = ISystemFontRenderer.VerticalAlign.BOTTOM;
				break;
			case (int)Align.VERTICAL_MIDDLE:
				valign = ISystemFontRenderer.VerticalAlign.MIDDLE;
				break;
			}

			systemFontRendererParameter = new ISystemFontRenderer.Parameter(
				fontHeight, width, height, style, align, valign, lineSpacing,
				letterSpacing, leftMargin, rightMargin);
			systemFontRenderer = ISystemFontRenderer.Construct();
			systemFontRenderer.SetText(str, color);

		} else {

			BitmapFont.Renderer.Align align;
			int a = textProperty.align & (int)Align.ALIGN_MASK;
			switch (a) {
			default:
			case (int)Align.LEFT:
				align = BitmapFont.Renderer.Align.LEFT;   break;
			case (int)Align.RIGHT:
				align = BitmapFont.Renderer.Align.RIGHT;  break;
			case (int)Align.CENTER:
				align = BitmapFont.Renderer.Align.CENTER; break;
			}

			BitmapFont.Renderer.VerticalAlign valign;
			int va = textProperty.align & (int)Align.VERTICAL_MASK;
			switch (va) {
			default:
				valign = BitmapFont.Renderer.VerticalAlign.TOP;
				break;
			case (int)Align.VERTICAL_BOTTOM:
				valign = BitmapFont.Renderer.VerticalAlign.BOTTOM;
				break;
			case (int)Align.VERTICAL_MIDDLE:
				valign = BitmapFont.Renderer.VerticalAlign.MIDDLE;
				break;
			}

			bitmapFontRenderer = new BitmapFont.Renderer(fontPath,
				fontHeight,
				width,
				height,
				align,
				valign,
				0.25f,
				lineSpacing,
				letterSpacing,
				tabSpacing,
				leftMargin,
				rightMargin);
			bitmapFontRenderer.SetText(str, color);

		}
	}

	public void Destruct()
	{
		if (systemFontRenderer != null)
			systemFontRenderer.Destruct();
		if (bitmapFontRenderer != null)
			bitmapFontRenderer.Destruct();
	}
}

public class TextRenderer : Renderer
{
	private TextContext m_context;
	private Matrix4x4 m_matrix;
	private Matrix4x4 m_renderMatrix;
	private UnityEngine.Color m_colorMult;
#if LWF_USE_ADDITIONALCOLOR
	private UnityEngine.Color m_colorAdd;
#endif
#if UNITY_EDITOR
	private bool m_visible;
#endif
	private bool m_shouldBeOnTop;
	private float m_zOffset;

	public TextRenderer(LWF lwf, TextContext context) : base(lwf)
	{
		m_context = context;
		m_matrix = new Matrix4x4();
		m_renderMatrix = new Matrix4x4();
		m_colorMult = new UnityEngine.Color();
#if LWF_USE_ADDITIONALCOLOR
		m_colorAdd = new UnityEngine.Color();
#endif
		if (m_context != null && m_context.systemFontRenderer != null) {
			ISystemFontRenderer.Parameter p =
				context.systemFontRendererParameter;
			float scale = lwf.scaleByStage;
			m_context.systemFontRenderer.Init(
				p.mSize * scale,
				p.mWidth * scale,
				p.mHeight * scale,
				p.mStyle,
				p.mAlign,
				p.mVerticalAlign,
				p.mLineSpacing * scale,
				p.mLetterSpacing * scale,
				p.mLeftMargin * scale,
				p.mRightMargin * scale);
		}

		CombinedMeshRenderer.Factory factory =
			lwf.rendererFactory as CombinedMeshRenderer.Factory;
		if (factory != null) {
			m_shouldBeOnTop = true;
			m_zOffset = Mathf.Abs(factory.zRate);
		}
	}

	public override void Render(Matrix matrix, ColorTransform colorTransform,
		int renderingIndex, int renderingCount, bool visible)
	{
#if UNITY_EDITOR
		m_visible = visible;
#endif
		if (m_context == null || !visible)
			return;

		float scale = 1;
		if (m_context.systemFontRenderer != null)
			scale /= m_lwf.scaleByStage;

		Factory factory = m_context.factory;
		factory.ConvertMatrix(ref m_matrix, matrix, scale,
			m_shouldBeOnTop ? m_zOffset : renderingCount - renderingIndex);
		Factory.MultiplyMatrix(ref m_renderMatrix,
			m_context.parent.transform.localToWorldMatrix, m_matrix);

#if LWF_USE_ADDITIONALCOLOR
		factory.ConvertColorTransform(
			ref m_colorMult, ref m_colorAdd, colorTransform);
		if (m_context.bitmapFontRenderer != null) {
			m_context.bitmapFontRenderer.Render(
				m_renderMatrix, m_colorMult, m_colorAdd,
				m_context.parent.layer, factory.camera);
		}
#else
		factory.ConvertColorTransform(ref m_colorMult, colorTransform);
		if (m_context.bitmapFontRenderer != null) {
			m_context.bitmapFontRenderer.Render(m_renderMatrix, m_colorMult,
				m_context.parent.layer, factory.camera);
		}
#endif
		if (m_context.systemFontRenderer != null) {
			m_context.systemFontRenderer.Render(m_renderMatrix, m_colorMult,
				m_context.parent.layer, factory.camera);
		}
	}

#if UNITY_EDITOR
	public override void RenderNow()
	{
		if (m_context == null || !m_visible)
			return;
		if (m_context.bitmapFontRenderer != null)
			m_context.bitmapFontRenderer.RenderNow(m_renderMatrix, m_colorMult);
		if (m_context.systemFontRenderer != null)
			m_context.systemFontRenderer.RenderNow(m_renderMatrix, m_colorMult);
	}
#endif
}

}	// namespace UnityRenderer
}	// namespace LWF
