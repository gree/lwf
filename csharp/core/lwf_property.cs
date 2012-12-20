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

namespace LWF {

public class Property
{
	public LWF m_lwf;
	public Matrix m_matrix;
	public ColorTransform m_colorTransform;
	public float m_scaleX = 1;
	public float m_scaleY = 1;
	public float m_rotation;
	public int m_renderingOffset;
	public bool m_hasMatrix;
	public bool m_hasColorTransform;

	public Matrix matrix {get {return m_matrix;}}
	public ColorTransform colorTransform {get {return m_colorTransform;}}
	public int renderingOffset {get {return m_renderingOffset;}}
	public bool hasMatrix {get {return m_hasMatrix;}}
	public bool hasColorTransform {get {return m_hasColorTransform;}}
	public bool hasRenderingOffset
		{get {return m_renderingOffset != System.Int32.MinValue;}}

	public Property(LWF lwf)
	{
		m_lwf = lwf;
		m_matrix = new Matrix();
		m_colorTransform = new ColorTransform();
		ClearRenderingOffset();
	}

	public void Clear()
	{
		m_scaleX = 1;
		m_scaleY = 1;
		m_rotation = 0;
		m_matrix.Clear();
		m_colorTransform.Clear();
		if (m_hasMatrix || m_hasColorTransform) {
			m_lwf.SetPropertyDirty();
			m_hasMatrix = false;
			m_hasColorTransform = false;
		}
		ClearRenderingOffset();
	}

	public void Move(float x, float y)
	{
		m_matrix.translateX += x;
		m_matrix.translateY += y;
		m_hasMatrix = true;
		m_lwf.SetPropertyDirty();
	}

	public void MoveTo(float x, float y)
	{
		m_matrix.translateX = x;
		m_matrix.translateY = y;
		m_hasMatrix = true;
		m_lwf.SetPropertyDirty();
	}

	public void Rotate(float degree)
	{
		RotateTo(m_rotation + degree);
	}

	public void RotateTo(float degree)
	{
		m_rotation = degree;
		SetScaleAndRotation();
	}

	public void Scale(float x, float y)
	{
		m_scaleX *= x;
		m_scaleY *= y;
		SetScaleAndRotation();
	}

	public void ScaleTo(float x, float y)
	{
		m_scaleX = x;
		m_scaleY = y;
		SetScaleAndRotation();
	}

	private void SetScaleAndRotation()
	{
		float radian = m_rotation * (float)System.Math.PI / 180.0f;
		float c = (float)System.Math.Cos(radian);
		float s = (float)System.Math.Sin(radian);
		m_matrix.scaleX = m_scaleX * c;
		m_matrix.skew0 = m_scaleY * -s;
		m_matrix.skew1 = m_scaleX * s;
		m_matrix.scaleY = m_scaleY * c;
		m_hasMatrix = true;
		m_lwf.SetPropertyDirty();
	}

	public void SetMatrix(Matrix m, float sX = 1, float sY = 1, float r = 0)
	{
		m_matrix.Set(m);
		m_scaleX = sX;
		m_scaleY = sY;
		m_rotation = r;
		m_hasMatrix = true;
		m_lwf.SetPropertyDirty();
	}

	public void SetAlpha(float alpha)
	{
		m_colorTransform.multi.alpha = alpha;
		m_hasColorTransform = true;
		m_lwf.SetPropertyDirty();
	}

	public void SetRed(float red)
	{
		m_colorTransform.multi.red = red;
		m_hasColorTransform = true;
		m_lwf.SetPropertyDirty();
	}

	public void SetGreen(float green)
	{
		m_colorTransform.multi.green = green;
		m_hasColorTransform = true;
		m_lwf.SetPropertyDirty();
	}

	public void SetBlue(float blue)
	{
		m_colorTransform.multi.blue = blue;
		m_hasColorTransform = true;
		m_lwf.SetPropertyDirty();
	}

	public void SetColorTransform(ColorTransform c)
	{
		m_colorTransform.Set(c);
		m_hasColorTransform = true;
		m_lwf.SetPropertyDirty();
	}

	public void SetRenderingOffset(int rOffset)
	{
		m_renderingOffset = rOffset;
	}

	public void ClearRenderingOffset()
	{
		m_renderingOffset = System.Int32.MinValue;
	}
}

}	// namespace LWF
