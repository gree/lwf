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

using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;

namespace LWF {
namespace UnityRenderer {

public partial class Factory : IRendererFactory
{
	public GameObject gameObject;
	public Camera camera;
	public string texturePrefix;
	public string fontPrefix;
	public float zOffset;
	public float zRate;
	public int renderQueueOffset;
	public TextureLoader textureLoader;
	public TextureUnloader textureUnloader;
	private Matrix4x4 matrix;

	protected Factory(GameObject gObj,
		float zOff, float zR, int rQOff, Camera cam,
		string texturePrfx = "", string fontPrfx = "",
		TextureLoader textureLdr = null,
		TextureUnloader textureUnldr = null)
	{
		gameObject = gObj;
		zOffset = zOff;
		zRate = zR;
		renderQueueOffset = rQOff;
		camera = cam;
		texturePrefix = texturePrfx;
		fontPrefix = fontPrfx;
		textureLoader = textureLdr;
		textureUnloader = textureUnldr;
		matrix = Matrix4x4.identity;
	}

	public virtual void Init(LWF lwf)
	{
		lwf.scaleByStage = Screen.height / lwf.height;
	}

	public virtual void BeginRender(LWF lwf)
	{
	}

	public virtual void EndRender(LWF lwf)
	{
	}

	public virtual void Destruct()
	{
	}

	public virtual Renderer ConstructBitmap(LWF lwf,
		int objectId, Bitmap bitmap)
	{
		return null;
	}

	public virtual Renderer ConstructBitmapEx(LWF lwf,
		int objectId, BitmapEx bitmapEx)
	{
		return null;
	}

	public virtual TextRenderer ConstructText(LWF lwf, int objectId, Text text)
	{
		return null;
	}

	public virtual Renderer ConstructParticle(LWF lwf,
		int objectId, Particle particle)
	{
		return null;
	}

	public static void ConvertMatrix(ref Matrix4x4 m, Matrix lm,
		float scale, float z, float zO, float zR, float height)
	{
		m.m00 = lm.scaleX * scale;
		m.m01 = -lm.skew0 * scale;
		m.m02 = 0;
		m.m03 = (lm.skew0 * scale * height) + lm.translateX;

		m.m10 = -lm.skew1 * scale;
		m.m11 = lm.scaleY * scale;
		m.m12 = 0;
		m.m13 = (-lm.scaleY * scale * height) + (-lm.translateY);

		m.m20 = 0;
		m.m21 = 0;
		m.m22 = 1;
		m.m23 = zO + z * zR;

		m.m30 = 0;
		m.m31 = 0;
		m.m32 = 0;
		m.m33 = 1;
	}

	public void ConvertMatrix(ref Matrix4x4 m,
		Matrix lm, float scale = 1, float z = 0, float height = 0)
	{
		ConvertMatrix(ref m, lm, scale, z, zOffset, zRate, height);
	}

	public static void MultiplyMatrix(ref Matrix4x4 m,
		Matrix4x4 l, Matrix4x4 r /* The matrix should be from LWF.Matrix */)
	{
		m.m00 = l.m00 * r.m00 + l.m01 * r.m10;
		m.m01 = l.m00 * r.m01 + l.m01 * r.m11;
		m.m02 = l.m02;
		m.m03 = l.m00 * r.m03 + l.m01 * r.m13 + l.m02 * r.m23 + l.m03;

		m.m10 = l.m10 * r.m00 + l.m11 * r.m10;
		m.m11 = l.m10 * r.m01 + l.m11 * r.m11;
		m.m12 = l.m12;
		m.m13 = l.m10 * r.m03 + l.m11 * r.m13 + l.m12 * r.m23 + l.m13;

		m.m20 = l.m20 * r.m00 + l.m21 * r.m10;
		m.m21 = l.m20 * r.m01 + l.m21 * r.m11;
		m.m22 = l.m22;
		m.m23 = l.m20 * r.m03 + l.m21 * r.m13 + l.m22 * r.m23 + l.m23;

		m.m30 = l.m30 * r.m00 + l.m31 * r.m10;
		m.m31 = l.m30 * r.m01 + l.m31 * r.m11;
		m.m32 = l.m32;
		m.m33 = l.m30 * r.m03 + l.m31 * r.m13 + l.m32 * r.m23 + l.m33;
	}

#if LWF_USE_ADDITIONALCOLOR
	public void ConvertColorTransform(
		ref UnityEngine.Color mc, ref UnityEngine.Color ac, ColorTransform c)
	{
		mc.r = c.multi.red;
		mc.g = c.multi.green;
		mc.b = c.multi.blue;
		mc.a = c.multi.alpha + c.add.alpha;

		ac.r = c.add.red;
		ac.g = c.add.green;
		ac.b = c.add.blue;
		ac.a = 0;
	}
#else
	public void ConvertColorTransform(
		ref UnityEngine.Color mc, ColorTransform c)
	{
		mc.r = c.multi.red;
		mc.g = c.multi.green;
		mc.b = c.multi.blue;
		mc.a = c.multi.alpha;
	}
#endif

	public UnityEngine.Color ConvertColor(Color c)
	{
		return new UnityEngine.Color(c.red, c.green, c.blue, c.alpha);
	}

	public UnityEngine.Color ConvertColor(Color c, ColorTransform t)
	{
		Color nc = new Color();
		Utility.CalcColor(nc, c, t);
		return new UnityEngine.Color(nc.red, nc.green, nc.blue, nc.alpha);
	}

	public Vector3 WorldToLWFPoint(LWF lwf, Vector3 p)
	{
		Matrix4x4 gm = gameObject.transform.worldToLocalMatrix;

		Matrix lm = lwf.rootMovie.matrix;

		matrix.m00 = lm.scaleX;
		matrix.m01 = lm.skew0;
		matrix.m03 = lm.translateX;

		matrix.m10 = lm.skew1;
		matrix.m11 = lm.scaleY;
		matrix.m13 = lm.translateY;

		return matrix.inverse.MultiplyPoint(gm.MultiplyPoint(p));
	}
}

}	// namespace UnityRenderer
}	// namespace LWF
