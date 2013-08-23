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

public class TextContext
{
	public Factory factory;
	public GameObject parent;
	public BitmapFont.Renderer bitmapFontRenderer;
	public ISystemFontRenderer systemFontRenderer;
	public ISystemFontRenderer.Parameter systemFontRendererParameter;
	public UnityEngine.Color color;

	public TextContext(Factory f, GameObject p, Data data, int objectId)
	{
		factory = f;
		parent = p;

		Format.Text text = data.texts[objectId];
		Format.TextProperty textProperty =
			data.textProperties[text.textPropertyId];
		Format.Font fontProperty = data.fonts[textProperty.fontId];
		color = factory.ConvertColor(data.colors[text.colorId]);

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

		}
	}

	public void Destruct()
	{
		if (systemFontRenderer != null)
			systemFontRenderer.Destruct();
		if (bitmapFontRenderer != null)
			bitmapFontRenderer.Destruct();
	}

	public void SetText(string text)
	{
		if (bitmapFontRenderer != null)
			bitmapFontRenderer.SetText(text, color);
		if (systemFontRenderer != null)
			systemFontRenderer.SetText(text, color);
	}

	public void Render(Matrix4x4 m, UnityEngine.Color colorMult,
#if LWF_USE_ADDITIONALCOLOR
		UnityEngine.Color colorAdd,
#endif
		int layer, Camera camera)
	{
#if LWF_USE_ADDITIONALCOLOR
		if (bitmapFontRenderer != null)
			bitmapFontRenderer.Render(m, colorMult, colorAdd, layer, camera);
#else
		if (bitmapFontRenderer != null)
			bitmapFontRenderer.Render(m, colorMult, layer, camera);
#endif
		if (systemFontRenderer != null)
			systemFontRenderer.Render(m, colorMult, layer, camera);
	}

#if UNITY_EDITOR
	public void RenderNow(Matrix4x4 m, UnityEngine.Color c)
	{
		if (bitmapFontRenderer != null)
			bitmapFontRenderer.RenderNow(m, c);
		if (systemFontRenderer != null)
			systemFontRenderer.RenderNow(m, c);
	}
#endif
}

public class UnityTextRenderer : TextRenderer
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

	public UnityTextRenderer(LWF lwf, int objectId) : base(lwf)
	{
		Factory factory = lwf.rendererFactory as Factory;
		m_context = new TextContext(
			factory, factory.gameObject, lwf.data, objectId);
		m_matrix = new Matrix4x4();
		m_renderMatrix = new Matrix4x4();
		m_colorMult = new UnityEngine.Color();
#if LWF_USE_ADDITIONALCOLOR
		m_colorAdd = new UnityEngine.Color();
#endif
		if (m_context != null && m_context.systemFontRenderer != null) {
			ISystemFontRenderer.Parameter p =
				m_context.systemFontRendererParameter;
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

		CombinedMeshRenderer.Factory c =
			lwf.rendererFactory as CombinedMeshRenderer.Factory;
		if (c != null) {
			m_shouldBeOnTop = true;
			m_zOffset = Mathf.Abs(c.zRate);
		}
	}

	public override void Destruct()
	{
		if (m_context != null)
			m_context.Destruct();
		base.Destruct();
	}

	public override void SetText(string text)
	{
		if (m_context == null)
			return;
		m_context.SetText(text);
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
		m_context.Render(m_renderMatrix, m_colorMult, m_colorAdd,
			m_context.parent.layer, factory.camera);
#else
		factory.ConvertColorTransform(ref m_colorMult, colorTransform);
		m_context.Render(m_renderMatrix, m_colorMult,
			m_context.parent.layer, factory.camera);
#endif
	}

#if UNITY_EDITOR
	public override void RenderNow()
	{
		if (m_context == null || !m_visible)
			return;
		m_context.RenderNow(m_renderMatrix, m_colorMult);
	}
#endif
}

}	// namespace UnityRenderer
}	// namespace LWF
