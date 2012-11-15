using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System;
using UnityEngine;

namespace SystemFont {

/*
public enum Align
{
	LEFT,
	CENTER,
	RIGHT,
}

public enum VerticalAlign
{
	TOP,
	MIDDLE,
	BOTTOM,
}

public enum Style
{
	NORMAL,
	BOLD,
	ITALIC,
	BOLD_ITALIC,
}
*/

public class Renderer : LWF.UnityRenderer.ISystemFontRenderer
{
	protected Mesh mMesh;
	protected Material mMaterial;
	protected MaterialPropertyBlock mProperty;
	protected Texture2D mTexture2D;
	protected UnityEngine.Color mColor;
	protected string mText;
	protected float mSize;
	protected float mWidth;
	protected float mHeight;
	protected Style mStyle;
	protected float mLineSpacing;
	protected float mLetterSpacing;
	protected float mLeftMargin;
	protected float mRightMargin;
	protected Align mAlign;
	protected VerticalAlign mVerticalAlign;
	protected bool mEmpty;
	protected bool mInitialized;

	public Mesh mesh {get {return mMesh;}}
	public Material material {get {return mMaterial;}}
	public Texture2D texture2D {get {return mTexture2D;}}
	public UnityEngine.Color color {get {return mColor;}}

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
# if UNITY_EDITOR || UNITY_STANDALONE_OSX
	[DllImport("SystemFontRenderer")]
# elif UNITY_IPHONE
	[DllImport("__Internal")]
# endif
	private static extern void _SystemFontRenderer_RenderTexture(string text,
		float size, float width, float height, int style,
		int align, int verticalAlign, float lineSpacing, float letterSpacing,
		float leftMargin, float rightMargin, int textureId);

#endif	// UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE

	public Renderer()
	{
	}

	public Renderer(float size,
		float width,
		float height,
		Style style,
		Align align = Align.LEFT,
		VerticalAlign verticalAlign = VerticalAlign.TOP,
		float lineSpacing = 1.0f,
		float letterSpacing = 0.0f,
		float leftMargin = 0.0f,
		float rightMargin = 0.0f)
	{
		Init(size, width, height, style, align, verticalAlign,
			lineSpacing, letterSpacing, leftMargin, rightMargin);
	}

	public override void Init(float size,
		float width,
		float height,
		Style style,
		Align align = Align.LEFT,
		VerticalAlign verticalAlign = VerticalAlign.TOP,
		float lineSpacing = 1.0f,
		float letterSpacing = 0.0f,
		float leftMargin = 0.0f,
		float rightMargin = 0.0f)
	{
		mSize = size;
		mWidth = width;
		mHeight = height;
		mStyle = style;
		mAlign = align;
		mVerticalAlign = verticalAlign;
		mLetterSpacing = letterSpacing;
		mLineSpacing = lineSpacing;
		mLeftMargin = leftMargin;
		mRightMargin = rightMargin;
		mEmpty = true;

		mTexture2D = new Texture2D(8, 8, TextureFormat.Alpha8, false);
		mTexture2D.Apply(true, true);
		mTexture2D.filterMode = FilterMode.Bilinear;
		mTexture2D.anisoLevel = 1;
		mTexture2D.wrapMode = TextureWrapMode.Clamp;

		mMaterial = new Material(Shader.Find("SystemFont"));
		mMaterial.mainTexture = mTexture2D;
		mMaterial.color = new UnityEngine.Color(1, 1, 1, 1);

		Vector3[] vertices = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		int[] triangles = new int[6];

		int w = 1;
		int h = 1;
		while (w < width)
			w <<= 1;
		while (h < height)
			h <<= 1;

		vertices[0] = new Vector3(w, -h, 0);
		vertices[1] = new Vector3(w, 0, 0);
		vertices[2] = new Vector3(0, -h, 0);
		vertices[3] = new Vector3(0, 0, 0);

		float w2 = 2.0f * w;
		float u0 = 1.0f / w2;
		float u1 = u0 + (float)(w * 2 - 2) / w2;
		float h2 = 2.0f * h;
		float v0 = 1.0f / h2;
		float v1 = v0 + (float)(h * 2 - 2) / h2;

		uv[0] = new Vector2(u1, v0);
		uv[1] = new Vector2(u1, v1);
		uv[2] = new Vector2(u0, v0);
		uv[3] = new Vector2(u0, v1);

		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 2;
		triangles[4] = 1;
		triangles[5] = 3;

		mMesh = new Mesh();
		mMesh.vertices = vertices;
		mMesh.uv = uv;
		mMesh.triangles = triangles;
		mMesh.RecalculateBounds();

		mProperty = new MaterialPropertyBlock();

		mInitialized = true;

		if (mText != null)
			SetText(mText, mColor);
	}

	public override void Destruct()
	{
		if (mMesh != null)
			Mesh.Destroy(mMesh);
		if (mTexture2D != null)
			Texture2D.Destroy(mTexture2D);
	}

	public override bool SetText(string text, Color color)
	{
		mText = text == null ? "" : text;
		mEmpty = text.Length == 0;
		mColor = color;

		if (!mInitialized)
			return true;

#if UNITY_ANDROID && !UNITY_EDITOR

		AndroidJavaObject o = new AndroidJavaObject(
			"net.gree.unitysystemfontrenderer.SystemFontRenderer");
		o.CallStatic("RenderTexture",
			mText, mSize, mWidth, mHeight, (int)mStyle,
			(int)mAlign, (int)mVerticalAlign, mLetterSpacing, mLineSpacing,
			mLeftMargin, mRightMargin, mTexture2D.GetNativeTextureID());

#else	// UNITY_ANDROID && !UNITY_EDITOR

		_SystemFontRenderer_RenderTexture(
			mText, mSize, mWidth, mHeight, (int)mStyle,
			(int)mAlign, (int)mVerticalAlign, mLetterSpacing, mLineSpacing,
			mLeftMargin, mRightMargin, mTexture2D.GetNativeTextureID());
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		GL.IssuePluginEvent(0);
#endif

#endif	// UNITY_ANDROID && !UNITY_EDITOR

		return true;
	}

	public override string GetText()
	{
		return mText;
	}

	public override void Render(
		Matrix4x4 matrix, int layer = 0, Camera camera = null)
	{
		if (mEmpty)
			return;
		mProperty.Clear();
		mProperty.AddColor("_Color", mColor);
		Graphics.DrawMesh(mMesh,
			matrix, mMaterial, layer, camera, 0, mProperty);
	}

	public override void Render(Matrix4x4 matrix,
		Color multColor, int layer = 0, Camera camera = null)
	{
		if (mEmpty)
			return;
		mProperty.Clear();
		mProperty.AddColor("_Color", mColor * multColor);
		Graphics.DrawMesh(mMesh,
			matrix, mMaterial, layer, camera, 0, mProperty);
	}
}

}	// namespace SystemFont
