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

namespace LWF {

public class BitmapClip : Bitmap
{
	public int depth;
	public bool visible;
	public string name;
	public float width;
	public float height;
	public float regX;
	public float regY;
	public float x;
	public float y;
	public float scaleX;
	public float scaleY;
	public float rotation;
	public float alpha;
	public float offsetX;
	public float offsetY;
	public float originalWidth;
	public float originalHeight;

	private float _scaleX;
	private float _scaleY;
	private float _rotation;
	private float _cos;
	private float _sin;
	private Matrix _matrix;

	public BitmapClip(LWF lwf, Movie parent, int objId)
		: base(lwf, parent, objId)
	{
		var data = lwf.data.bitmaps[objId];
		var fragment = lwf.data.textureFragments[data.textureFragmentId];
		var texdata = lwf.data.textures[fragment.textureId];
		width = fragment.w / texdata.scale;
		height = fragment.h / texdata.scale;
		offsetX = fragment.x;
		offsetY = fragment.y;
		originalWidth = fragment.ow;
		originalHeight = fragment.oh;

		depth = -1;
		visible = true;

		regX = 0;
		regY = 0;
		x = 0;
		y = 0;
		scaleX = 0;
		scaleY = 0;
		rotation = 0;
		alpha = 1;

		_scaleX = scaleX;
		_scaleY = scaleY;
		_rotation = rotation;
		_cos = 1;
		_sin = 0;

		_matrix = new Matrix();
	}

	public override void Exec(int matrixId = 0, int colorTransformId = 0)
	{
	}

	public override void Update(Matrix m, ColorTransform c)
	{
		bool dirty = false;
		if (rotation != _rotation) {
			_rotation = rotation;
			float radian = _rotation * (float)System.Math.PI / 180.0f;
			_cos = (float)System.Math.Cos(radian);
			_sin = (float)System.Math.Sin(radian);
			dirty = true;
		}
		if (dirty || _scaleX != scaleX || _scaleY != scaleY) {
			_scaleX = scaleX;
			_scaleY = scaleY;
			_matrix.scaleX = _scaleX *  _cos;
			_matrix.skew1  = _scaleX *  _sin;
			_matrix.skew0  = _scaleY * -_sin;
			_matrix.scaleY = _scaleY *  _cos;
		}

		_matrix.translateX = x - regX;
		_matrix.translateY = y - regY;

		m_matrix.scaleX =
			m.scaleX * _matrix.scaleX +
			m.skew0  * _matrix.skew1;
		m_matrix.skew0 =
			m.scaleX * _matrix.skew0 +
			m.skew0  * _matrix.scaleY;
		m_matrix.translateX =
			m.scaleX * x +
			m.skew0  * y +
			m.translateX +
				m.scaleX * regX + m.skew0 * regY +
				m_matrix.scaleX * -regX + m_matrix.skew0 * -regY;
		m_matrix.skew1 =
			m.skew1  * _matrix.scaleX +
			m.scaleY * _matrix.skew1;
		m_matrix.scaleY =
			m.skew1  * _matrix.skew0 +
			m.scaleY * _matrix.scaleY;
		m_matrix.translateY =
			m.skew1  * x +
			m.scaleY * y +
			m.translateY +
				m.skew1 * regX + m.scaleY * regY +
				m_matrix.skew1 * -regX + m_matrix.scaleY * -regY;

		m_colorTransform.Set(c);
		m_colorTransform.multi.alpha *= alpha;

		m_lwf.RenderObject();
	}

	public void DetachFromParent()
	{
		if (m_parent != null) {
			m_parent.DetachBitmap(depth);
			m_parent = null;
		}
	}

	public override bool IsBitmapClip() {return true;}
}

}	// namespace LWF
