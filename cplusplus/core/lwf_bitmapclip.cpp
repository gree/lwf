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

#include "lwf_bitmapclip.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_movie.h"

namespace LWF {

BitmapClip::BitmapClip(LWF *lwf, Movie *p, int objId)
	: Bitmap(lwf, p, objId)
{
	const Format::Bitmap &data = lwf->data->bitmaps[objId];
	const Format::TextureFragment &fragment =
		lwf->data->textureFragments[data.textureFragmentId];
	const Format::Texture &texdata = lwf->data->textures[fragment.textureId];
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
}

void BitmapClip::Exec(int mId, int cId)
{
}

void BitmapClip::Update(const Matrix *m, const ColorTransform *c)
{
	bool dirty = false;
	if (rotation != _rotation) {
		_rotation = rotation;
		float radian = _rotation * M_PI / 180.0f;
		_cos = cosf(radian);
		_sin = sinf(radian);
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

	matrix.scaleX =
		m->scaleX * _matrix.scaleX +
		m->skew0  * _matrix.skew1;
	matrix.skew0 =
		m->scaleX * _matrix.skew0 +
		m->skew0  * _matrix.scaleY;
	matrix.translateX =
		m->scaleX * x +
		m->skew0  * y +
		m->translateX +
			m->scaleX * regX + m->skew0 * regY +
			matrix.scaleX * -regX + matrix.skew0 * -regY;
	matrix.skew1 =
		m->skew1  * _matrix.scaleX +
		m->scaleY * _matrix.skew1;
	matrix.scaleY =
		m->skew1  * _matrix.skew0 +
		m->scaleY * _matrix.scaleY;
	matrix.translateY =
		m->skew1  * x +
		m->scaleY * y +
		m->translateY +
			m->skew1 * regX + m->scaleY * regY +
			matrix.skew1 * -regX + matrix.scaleY * -regY;

	colorTransform.Set(c);
	colorTransform.multi.alpha *= alpha;

	lwf->RenderObject();
}

void BitmapClip::DetachFromParent()
{
	if (parent) {
		parent->DetachBitmap(depth);
		parent = 0;
	}
}

}	// namespace LWF
