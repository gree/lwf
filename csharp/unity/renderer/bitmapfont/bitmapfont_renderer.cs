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

using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace BitmapFont {

public partial class Header
{
	public short fontSize;
	public short fontAscent;
	public short metricCount;
	public short sheetWidth;
	public short sheetHeight;
}

public partial class Metric
{
	public float advance;
	public short u;
	public short v;
	public sbyte bearingX;
	public sbyte bearingY;
	public byte width;
	public byte height;
	public byte first;
	public byte second;
	public byte prevNum;
	public byte nextNum;
}

public partial class Data
{
	public Header header;
	public short[] indecies;
	public Metric[] metrics;
	public string textureName;
}

public class Renderer
{
	public enum Align
	{
		LEFT,
		RIGHT,
		CENTER
	}

	public enum VerticalAlign
	{
		TOP,
		BOTTOM,
		MIDDLE
	}

	protected Data mData;
	protected Mesh mMesh;
	protected Material mMaterial;
	protected MaterialPropertyBlock mProperty;
	protected string mName;
	protected string mText;
	protected float mSize;
	protected float mWidth;
	protected float mHeight;
	protected float mLineSpacing;
	protected float mLetterSpacing;
	protected float mTabSpacing;
	protected float mLeftMargin;
	protected float mRightMargin;
	protected float mAsciiSpaceAdvance;
	protected float mNonAsciiSpaceAdvance;
	protected Align mAlign;
	protected VerticalAlign mVerticalAlign;
	protected bool mEmpty;

	public Mesh mesh {get {return mMesh;}}
	public Material material {get {return mMaterial;}}

	public Renderer() {}

	public Renderer(string fontName,
		float size = 0,
		float width = 0,
		float height = 0,
		Align align = Align.LEFT,
		VerticalAlign verticalAlign = VerticalAlign.TOP,
		float spaceAdvance = 0.25f,
		float lineSpacing = 1.0f,
		float letterSpacing = 0.0f,
		float tabSpacing = 4.0f,
		float leftMargin = 0.0f,
		float rightMargin = 0.0f)
	{
		Init(fontName, size, width, height, align, verticalAlign, spaceAdvance,
			lineSpacing, letterSpacing, tabSpacing, leftMargin, rightMargin);
	}

	public void Init(string fontName,
		float size = 0,
		float width = 0,
		float height = 0,
		Align align = Align.LEFT,
		VerticalAlign verticalAlign = VerticalAlign.TOP,
		float spaceAdvance = 0.25f,
		float lineSpacing = 1.0f,
		float letterSpacing = 0.0f,
		float tabSpacing = 4.0f,
		float leftMargin = 0.0f,
		float rightMargin = 0.0f)
	{
		ResourceCache cache = ResourceCache.SharedInstance();
		mName = fontName;
		mData = cache.LoadData(mName);
		string dir = System.IO.Path.GetDirectoryName(mName);
		if (dir.Length > 0)
			dir += "/";
		mMaterial = cache.LoadTexture(dir + mData.textureName);
		mMesh = new Mesh();
		mMesh.name = "BitmapFont";
		mProperty = new MaterialPropertyBlock();

		Metric asciiEm = SearchMetric('M');
		Metric nonasciiEm = SearchMetric('\u004d');

		mSize = size;
		mAlign = align;
		mVerticalAlign = verticalAlign;
		mWidth = width;
		mHeight = height;
		mLetterSpacing = letterSpacing * mSize;
		mAsciiSpaceAdvance = mLetterSpacing + (asciiEm == null ?
			1 : asciiEm.advance) * spaceAdvance * mSize;
		mNonAsciiSpaceAdvance = mLetterSpacing + (nonasciiEm == null ?
			1 : nonasciiEm.advance) * spaceAdvance * mSize;
		mTabSpacing = mAsciiSpaceAdvance * tabSpacing;
		mLineSpacing = lineSpacing * mSize;
		mLeftMargin = leftMargin * mSize;
		mRightMargin = rightMargin * mSize;
		mEmpty = true;
	}

	public void Destruct()
	{
		if (mName == null)
			return;

		ResourceCache cache = ResourceCache.SharedInstance();
		cache.UnloadTexture(mData.textureName);
		cache.UnloadData(mName);
		Mesh.Destroy(mMesh);
		mName = null;
	}

	public class compFirst : IComparer<Metric>
	{
		public int Compare(Metric a, Metric b)
		{
			return a.first.CompareTo(b.first);
		}
	}

	public class compSecond : IComparer<Metric>
	{
		public int Compare(Metric a, Metric b)
		{
			return a.second.CompareTo(b.second);
		}
	}

	protected virtual Metric SearchMetric(char c)
	{
		Metric[] metrics = mData.metrics;
		byte first = (byte)(((ushort)c) >> 8);
		byte second = (byte)(((ushort)c) & 0xff);
		short index = mData.indecies[first];

		int offset = index + second;
		if (offset < 0) {
			// not found
			return null;
		}
		if (offset >= mData.header.metricCount)
			offset = mData.header.metricCount - 1;

		Metric m = new Metric();
		if (first != metrics[offset].first) {
			if (index < 0)
				index = 0;
			m.first = first;
			offset = Array.BinarySearch(metrics,
				index, offset - index + 1, m, new compFirst());
			if (offset < 0 || first != metrics[offset].first) {
				// not found
				return null;
			}
		}

		if (second != metrics[offset].second) {
			int left = offset - metrics[offset].prevNum;
			int right = offset + metrics[offset].nextNum;
			m.second = second;
			offset = Array.BinarySearch(metrics,
				left, right - left + 1, m, new compSecond());
		}

		if (offset < 0 || metrics[offset].second != second) {
			// not found
			return null;
		}

		return metrics[offset];
	}

	public virtual bool SetText(string text, Color color)
	{
		Color[] colors = new Color[text.Length];
		for (int i = 0; i < text.Length; ++i)
			colors[i] = color;
		return SetText(text, colors);
	}

	private static bool IsAscii(char c)
	{
		return (c >= '!') && (c <= '~');
	}

	private class LineContext
	{
		public int vertexBegin;
		public int vertexEnd;
		public float left;
		public float right;

		public LineContext(int vBegin, int vEnd, float l, float r)
		{
			vertexBegin = vBegin;
			vertexEnd = vEnd;
			left = l;
			right = r;
		}
	}

	private void FeedLine(List<LineContext> lines,
		ref int vertexBegin, int vertexEnd, ref float left, ref float right)
	{
		lines.Add(new LineContext(vertexBegin, vertexEnd, left, right));
		vertexBegin = vertexEnd;
		left = mWidth;
		right = 0;
	}

	public virtual bool SetText(string text, Color[] colors)
	{
		bool result = true;
		mText = text;
		if (text == null || text.Length == 0) {
			mEmpty = true;
			mMesh.Clear();
			return result;
		}

		mEmpty = false;
		int chars = text.Length;
		Vector3[] vertices = new Vector3[chars * 4];
		Vector2[] uv = new Vector2[chars * 4];
		int[] triangles = new int[chars * 6];
		Color32[] vertexColors = new Color32[chars * 4];
		float scale = mSize / (float)mData.header.fontSize;
		float x = mLeftMargin;
		float y = -(float)mData.header.fontAscent * scale;
		float sheetWidth = (float)mData.header.sheetWidth;
		float sheetHeight = (float)mData.header.sheetHeight;
		int lastAscii = -1;
		int lastIndex = -1;
		int vertexBegin = 0;
		float left = mWidth;
		float right = 0;
		float top = mHeight;
		float bottom = 0;
		List<LineContext> lines = new List<LineContext>();

		for (int i = 0; i < text.Length; ++i) {
			char c = text[i];

			if (c == '\n') {
				// LINEFEED
				x = mLeftMargin;
				y -= mLineSpacing;
				lastAscii = -1;
				FeedLine(lines, ref vertexBegin, i, ref left, ref right);
				continue;
			} else if (c == ' ') {
				// SPACE
				x += mAsciiSpaceAdvance;
				lastAscii = -1;
				continue;
			} else if (c == '\t') {
				// TAB
				x += mTabSpacing;
				lastAscii = -1;
				continue;
			} else if (c == '\u3000') {
				// JIS X 0208 SPACE
				x += mNonAsciiSpaceAdvance;
				lastAscii = -1;
				continue;
			}
			if (IsAscii(c)) {
				// ASCII
				if (lastAscii == -1) {
					// Save index for Auto linefeed
					lastAscii = i;
				}
			} else {
				// non-ASCII
				lastAscii = -1;
			}

			Metric metric = SearchMetric(c);
			if (metric == null) {
				// not found
				result = false;
				continue;
			}

			float advance = metric.advance * mSize + mLetterSpacing;

			float px = x + advance;
			if (mWidth != 0 && px > mWidth - mRightMargin) {
				// Auto linefeed.
				int index = lastAscii;
				lastAscii = -1;
				x = mLeftMargin;
				y -= mLineSpacing;
				if (index != -1 && IsAscii(c)) {
					// ASCII
					int nextIndex = index - 1;
					if (lastIndex != nextIndex) {
						i = nextIndex;
						lastIndex = i;
						right = vertices[(i - 1) * 4].x;
						FeedLine(lines,
							ref vertexBegin, i, ref left, ref right);
						continue;
					}
				} else {
					FeedLine(lines, ref vertexBegin, i, ref left, ref right);
				}
			}

			float x0 = x + (float)metric.bearingX * scale;
			float x1 = x0 + (float)metric.width * scale;
			float y0 = y + (float)metric.bearingY * scale;
			float y1 = y0 - (float)metric.height * scale;

			if (left > x0)
				left = x0;
			if (right < x1)
				right = x1;
			if (top > y0)
				top = y0;
			if (bottom < y1)
				bottom = y1;

			x += advance;

			float w = 2.0f * sheetWidth;
			float u0 = (float)(2 * metric.u + 1) / w;
			float u1 = u0 + (float)(metric.width * 2 - 2) / w;
			float h = 2.0f * sheetHeight;
			float v0 = (float)(2 * (sheetHeight - metric.v) + 1) / h;
			float v1 = (v0 - (float)(metric.height * 2 + 2) / h);

			int vertexOffset = i * 4;
			vertices[vertexOffset + 0] = new Vector3(x1, y0, 0);
			vertices[vertexOffset + 1] = new Vector3(x1, y1, 0);
			vertices[vertexOffset + 2] = new Vector3(x0, y0, 0);
			vertices[vertexOffset + 3] = new Vector3(x0, y1, 0);

			uv[vertexOffset + 0] = new Vector2(u1, v0);
			uv[vertexOffset + 1] = new Vector2(u1, v1);
			uv[vertexOffset + 2] = new Vector2(u0, v0);
			uv[vertexOffset + 3] = new Vector2(u0, v1);

			int triangleOffset = i * 6;
			triangles[triangleOffset + 0] = 0 + vertexOffset;
			triangles[triangleOffset + 1] = 1 + vertexOffset;
			triangles[triangleOffset + 2] = 2 + vertexOffset;
			triangles[triangleOffset + 3] = 2 + vertexOffset;
			triangles[triangleOffset + 4] = 1 + vertexOffset;
			triangles[triangleOffset + 5] = 3 + vertexOffset;

			for (int n = 0; n < 4; ++n)
				vertexColors[vertexOffset + n] = colors[i];
		}
		FeedLine(lines, ref vertexBegin, text.Length, ref left, ref right);

		if (mWidth != 0 && mAlign != Align.LEFT) {
			foreach (LineContext line in lines) {
				float tw = line.right - line.left;
				float offset;
				if (mAlign == Align.CENTER) {
					offset = (mWidth - mRightMargin - tw) / 2.0f;
				} else {
					// Align.RIGHT
					offset = mWidth - mRightMargin - tw;
				}

				for (int i = line.vertexBegin; i < line.vertexEnd; ++i)
					for (int n = 0; n < 4; ++n)
						vertices[i * 4 + n].x += offset;
			}
		}

		if (mHeight != 0 && mVerticalAlign != VerticalAlign.TOP) {
			float th = bottom - top;
			float offset;
			if (mVerticalAlign == VerticalAlign.MIDDLE) {
				offset = (mHeight - th) / 2.0f;
			} else {
				// VerticalAlign.BOTTOM
				offset = mHeight - th;
			}

			for (int i = 0; i < vertices.Length; ++i)
				vertices[i].y -= offset;
		}

		mMesh.Clear();
		mMesh.vertices = vertices;
		mMesh.uv = uv;
		mMesh.triangles = triangles;
		mMesh.colors32 = vertexColors;
		mMesh.RecalculateBounds();
		//mMesh.Optimize();
		return result;
	}

	public virtual string GetText()
	{
		return mText;
	}

	public virtual void Render(Matrix4x4 matrix, Camera camera = null)
	{
		if (mEmpty)
			return;
		Graphics.DrawMesh(mMesh, matrix, mMaterial, 0, camera);
	}

	public virtual void Render(Matrix4x4 matrix,
		Color multColor, int layer = 0, Camera camera = null)
	{
		if (mEmpty)
			return;
		mProperty.Clear();
		mProperty.AddColor("_Color", multColor);
		Graphics.DrawMesh(
			mMesh, matrix, mMaterial, layer, camera, 0, mProperty);
	}

#if LWF_USE_ADDITIONALCOLOR
	public virtual void Render(Matrix4x4 matrix,
		Color multColor, Color addColor, int layer = 0, Camera camera = null)
	{
		if (mEmpty)
			return;
		mProperty.Clear();
		mProperty.AddColor("_Color", multColor);
		mProperty.AddColor("_AdditionalColor", addColor);
		Graphics.DrawMesh(
			mMesh, matrix, mMaterial, layer, camera, 0, mProperty);
	}
#endif

#if UNITY_EDITOR
	public virtual void RenderNow(Matrix4x4 matrix, Color multColor)
	{
		if (mEmpty)
			return;
		Material material = new Material(mMaterial);
		material.color = multColor;
		material.SetPass(0);
		Graphics.DrawMeshNow(mMesh, matrix);
		Material.Destroy(material);
	}
#endif
}

}	// namespace BitmapFont
