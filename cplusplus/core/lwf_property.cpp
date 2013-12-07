/*
 * Copyright (C) 2013 GREE, Inc.
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

#include "lwf_property.h"
#include "lwf_core.h"

namespace LWF {

Property::Property(LWF *l)
{
	lwf = l;

	hasMatrix = false;
	hasColorTransform = false;
	scaleX = 1;
	scaleY = 1;
	rotation = 0;

	ClearRenderingOffset();
}

void Property::Clear()
{
	scaleX = 1;
	scaleY = 1;
	rotation = 0;
	matrix.Clear();
	colorTransform.Clear();
	if (hasMatrix || hasColorTransform) {
		lwf->SetPropertyDirty();
		hasMatrix = false;
		hasColorTransform = false;
	}
	ClearRenderingOffset();
}

void Property::Move(float x, float y)
{
	matrix.translateX += x;
	matrix.translateY += y;
	hasMatrix = true;
	lwf->SetPropertyDirty();
}

void Property::MoveTo(float x, float y)
{
	matrix.translateX = x;
	matrix.translateY = y;
	hasMatrix = true;
	lwf->SetPropertyDirty();
}

void Property::Rotate(float degree)
{
	RotateTo(rotation + degree);
}

void Property::RotateTo(float degree)
{
	rotation = degree;
	SetScaleAndRotation();
}

void Property::Scale(float x, float y)
{
	scaleX *= x;
	scaleY *= y;
	SetScaleAndRotation();
}

void Property::ScaleTo(float x, float y)
{
	scaleX = x;
	scaleY = y;
	SetScaleAndRotation();
}

void Property::SetScaleAndRotation()
{
	float radian = rotation * M_PI / 180.0f;
	float c = cosf(radian);
	float s = sinf(radian);
	matrix.scaleX = scaleX * c;
	matrix.skew0 = scaleY * -s;
	matrix.skew1 = scaleX * s;
	matrix.scaleY = scaleY * c;
	hasMatrix = true;
	lwf->SetPropertyDirty();
}

void Property::SetMatrix(const Matrix *m, float sX, float sY, float r)
{
	matrix.Set(m);
	scaleX = sX;
	scaleY = sY;
	rotation = r;
	hasMatrix = true;
	lwf->SetPropertyDirty();
}

void Property::SetAlpha(float alpha)
{
	colorTransform.multi.alpha = alpha;
	hasColorTransform = true;
	lwf->SetPropertyDirty();
}

void Property::SetRed(float red)
{
	colorTransform.multi.red = red;
	hasColorTransform = true;
	lwf->SetPropertyDirty();
}

void Property::SetGreen(float green)
{
	colorTransform.multi.green = green;
	hasColorTransform = true;
	lwf->SetPropertyDirty();
}

void Property::SetBlue(float blue)
{
	colorTransform.multi.blue = blue;
	hasColorTransform = true;
	lwf->SetPropertyDirty();
}

void Property::SetColorTransform(const ColorTransform *c)
{
	colorTransform.Set(c);
	hasColorTransform = true;
	lwf->SetPropertyDirty();
}

void Property::SetRenderingOffset(int rOffset)
{
	renderingOffset = rOffset;
}

void Property::ClearRenderingOffset()
{
	renderingOffset = INT_MIN;
	hasRenderingOffset = false;
}

}	// namespace LWF
