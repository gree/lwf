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

#ifndef LWF_PROPERTY_H
#define LWF_PROPERTY_H

#include "lwf_type.h"

namespace LWF {

class LWF;

class Property
{
public:
	LWF *lwf;
	Matrix matrix;
	ColorTransform colorTransform;
	int renderingOffset;
	bool hasMatrix;
	bool hasColorTransform;
	bool hasRenderingOffset;
	float scaleX;
	float scaleY;
	float rotation;

public:
	Property(LWF *l);
	void Clear();
	void Move(float x, float y);
	void MoveTo(float x, float y);
	void Rotate(float degree);
	void RotateTo(float degree);
	void Scale(float x, float y);
	void ScaleTo(float x, float y);
	void SetMatrix(const Matrix *m, float sX = 1, float sY = 1, float r = 0);
	void SetAlpha(float alpha);
	void SetRed(float red);
	void SetGreen(float green);
	void SetBlue(float blue);
	void SetColorTransform(const ColorTransform *c);
	void SetRenderingOffset(int rOffset);
	void ClearRenderingOffset();

private:
	void SetScaleAndRotation();
};

}	// namespace LWF

#endif
