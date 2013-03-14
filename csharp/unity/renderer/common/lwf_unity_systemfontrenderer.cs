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

using Constructor = System.Func<LWF.UnityRenderer.ISystemFontRenderer>;

namespace LWF {
namespace UnityRenderer {

public class ISystemFontRenderer
{
	public enum Style
	{
		NORMAL,
		BOLD,
		ITALIC,
		BOLD_ITALIC,
	}

	public enum Align
	{
		LEFT,
		CENTER,
		RIGHT
	}

	public enum VerticalAlign
	{
		TOP,
		MIDDLE,
		BOTTOM
	}

	public class Parameter
	{
		public float mSize;
		public float mWidth;
		public float mHeight;
		public Style mStyle;
		public Align mAlign;
		public VerticalAlign mVerticalAlign;
		public float mLineSpacing;
		public float mLetterSpacing;
		public float mLeftMargin;
		public float mRightMargin;

		public Parameter(float size, float width, float height, Style style,
			Align align, VerticalAlign verticalAlign, float lineSpacing,
			float letterSpacing, float leftMargin, float rightMargin)
		{
			mSize = size;
			mWidth = width;
			mHeight = height;
			mStyle = style;
			mAlign = align;
			mVerticalAlign = verticalAlign;
			mLineSpacing = lineSpacing;
			mLetterSpacing = letterSpacing;
			mLeftMargin = leftMargin;
			mRightMargin = rightMargin;
		}
	}

	private static Constructor s_constructor;

	public static void SetConstructor(Constructor c)
	{
		s_constructor = c;
	}

	public static ISystemFontRenderer Construct()
	{
		return s_constructor();
	}

	public ISystemFontRenderer()
	{
	}

	public virtual void Init(float size, float width, float height,
		Style style, Align align, VerticalAlign verticalAlign,
		float lineSpacing, float letterSpacing, float leftMargin,
		float rightMargin)
	{
	}

	public virtual void Destruct()
	{
	}

	public virtual bool SetText(string text, UnityEngine.Color color)
	{
		return false;
	}

	public virtual string GetText()
	{
		return null;
	}

	public virtual void Render(
		Matrix4x4 matrix, int layer = 0, Camera camera = null)
	{
	}

	public virtual void Render(Matrix4x4 matrix,
		UnityEngine.Color multColor, int layer = 0, Camera camera = null)
	{
	}

#if UNITY_EDITOR
	public virtual void RenderNow(Matrix4x4 matrix, UnityEngine.Color multColor)
	{
	}
#endif
}

}	// namespace UnityRenderer
}	// namespace LWF
