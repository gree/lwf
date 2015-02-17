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

#ifndef LWF_BITMAPCLIP_H
#define	LWF_BITMAPCLIP_H

#include "lwf_object.h"
#include "lwf_bitmap.h"

namespace LWF {

class LWF;
class Movie;

class BitmapClip : public Bitmap
{
public:
	int depth;
	bool visible;
	string name;
	float width;
	float height;
	float regX;
	float regY;
	float x;
	float y;
	float scaleX;
	float scaleY;
	float rotation;
	float alpha;
	float offsetX;
	float offsetY;
	float originalWidth;
	float originalHeight;

private:
	float _scaleX;
	float _scaleY;
	float _rotation;
	float _cos;
	float _sin;
	Matrix _matrix;

public:
	BitmapClip(LWF *lwf, Movie *p, int objId);

	void Exec(int mId = 0, int cId = 0);
	void Update(const Matrix *m, const ColorTransform *c);

	void DetachFromParent();

	bool IsBitmapClip() const {return true;}
};

}	// namespace LWF

#endif
