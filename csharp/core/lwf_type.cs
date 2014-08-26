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

public class Point
{
	public float x;
	public float y;

	public Point(float px = 0, float py = 0)
	{
		x = px;
		y = py;
	}
}

public partial class Translate
{
	public float translateX;
	public float translateY;

	public Translate()
	{
		translateX = 0;
		translateY = 0;
	}
}

public partial class Matrix
{
	public float scaleX;
	public float scaleY;
	public float skew0;
	public float skew1;
	public float translateX;
	public float translateY;

	public Matrix()
	{
		Clear();
	}

	public Matrix(Matrix m)
	{
		Set(m);
	}

	public Matrix(
		float scX, float scY, float sk0, float sk1, float tX, float tY)
	{
		scaleX = scX;
		scaleY = scY;
		skew0 = sk0;
		skew1 = sk1;
		translateX = tX;
		translateY = tY;
	}

	public void Clear()
	{
		scaleX = 1;
		scaleY = 1;
		skew0 = 0;
		skew1 = 0;
		translateX = 0;
		translateY = 0;
	}

	public Matrix Set(Matrix m)
	{
		scaleX = m.scaleX;
		scaleY = m.scaleY;
		skew0 = m.skew0;
		skew1 = m.skew1;
		translateX = m.translateX;
		translateY = m.translateY;
		return this;
	}

	public bool SetWithComparing(Matrix m)
	{
		if (m == null)
			return false;

		float sX = m.scaleX;
		float sY = m.scaleY;
		float s0 = m.skew0;
		float s1 = m.skew1;
		float tX = m.translateX;
		float tY = m.translateY;
		bool changed = false;
		if (scaleX != sX) {
			scaleX = sX;
			changed = true;
		}
		if (scaleY != sY) {
			scaleY = sY;
			changed = true;
		}
		if (skew0 != s0) {
			skew0 = s0;
			changed = true;
		}
		if (skew1 != s1) {
			skew1 = s1;
			changed = true;
		}
		if (translateX != tX) {
			translateX = tX;
			changed = true;
		}
		if (translateY != tY) {
			translateY = tY;
			changed = true;
		}
		return changed;
	}
}

public partial class Color
{
	public float red, green, blue, alpha;

	public Color() {}

	public Color(float r, float g, float b, float a)
	{
		Set(r, g, b, a);
	}

	public void Set(float r, float g, float b, float a)
	{
		red = r;
		green = g;
		blue = b;
		alpha = a;
	}

	public void Set(Color c)
	{
		red = c.red;
		green = c.green;
		blue = c.blue;
		alpha = c.alpha;
	}

	public bool Equals(Color c)
	{
		return red == c.red &&
			green == c.green &&
			blue == c.blue &&
			alpha == c.alpha;
	}
}

public partial class AlphaTransform
{
	public float alpha;

	public AlphaTransform() {alpha = 1;}
	public AlphaTransform(float a) {alpha = a;}
}

public partial class ColorTransform
{
	public Color multi;
	public Color add;

	public ColorTransform(
		float multiRed = 1,
		float multiGreen = 1,
		float multiBlue = 1,
		float multiAlpha = 1,
		float addRed = 0,
		float addGreen = 0,
		float addBlue = 0,
		float addAlpha = 0
		)
	{
		multi = new Color(multiRed, multiGreen, multiBlue, multiAlpha);
		add = new Color(addRed, addGreen, addBlue, addAlpha);
	}

	public ColorTransform(ColorTransform c)
	{
		multi = new Color();
		add = new Color();
		Set(c);
	}

	public void Clear()
	{
		multi.Set(1, 1, 1, 1);
		add.Set(0, 0, 0, 0);
	}

	public ColorTransform Set(ColorTransform c)
	{
		multi.Set(c.multi);
		add.Set(c.add);
		return this;
	}

	public bool SetWithComparing(ColorTransform c)
	{
		if (c == null)
			return false;

		Color cm = c.multi;
		float red = cm.red;
		float green = cm.green;
		float blue = cm.blue;
		float alpha = cm.alpha;
		bool changed = false;
		Color m = multi;
		if (m.red != red) {
			m.red = red;
			changed = true;
		}
		if (m.green != green) {
			m.green = green;
			changed = true;
		}
		if (m.blue != blue) {
			m.blue = blue;
			changed = true;
		}
		if (m.alpha != alpha) {
			m.alpha = alpha;
			changed = true;
		}

		Color ca = c.add;
		red = ca.red;
		green = ca.green;
		blue = ca.blue;
		alpha = ca.alpha;
		Color a = add;
		if (a.red != red) {
			a.red = red;
			changed = true;
		}
		if (a.green != green) {
			a.green = green;
			changed = true;
		}
		if (a.blue != blue) {
			a.blue = blue;
			changed = true;
		}
		if (a.alpha != alpha) {
			a.alpha = alpha;
			changed = true;
		}
		return changed;
	}
}

public class Bounds
{
	public float xMin;
	public float xMax;
	public float yMin;
	public float yMax;

	public Bounds(
		float pxMin = 0, float pxMax = 0, float pyMin = 0, float pyMax = 0)
	{
		xMin = pxMin;
		xMax = pxMax;
		yMin = pyMin;
		yMax = pyMax;
	}
}

}	// namespace LWF
