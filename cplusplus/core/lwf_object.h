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

#ifndef LWF_OBJECT_H
#define LWF_OBJECT_H

#include "lwf_format.h"

namespace LWF {

class LWF;
class Movie;
class Object;
class Renderer;

typedef Format::Object OType;

class Object
{
public:
	LWF *lwf;
	Movie *parent;
	int type;
	int execCount;
	int objectId;
	int matrixId;
	int colorTransformId;
	Matrix matrix;
	int dataMatrixId;
	ColorTransform colorTransform;
	shared_ptr<Renderer> renderer;
	bool matrixIdChanged;
	bool colorTransformIdChanged;
	bool updated;

public:
	Object() {}
	Object(LWF *l, Movie *p, int t, int objId);
	virtual ~Object() {}

	virtual void Exec(int mId = 0, int cId = 0);
	virtual void Update(const Matrix *m, const ColorTransform *c);
	virtual void Render(bool v, int rOffset);
	virtual void Inspect(
		Inspector inspector, int hierarchy, int depth, int rOffset);
	virtual void Destroy();

	bool IsButton() {return type == Format::Object::BUTTON ? true : false;}
	bool IsMovie() {return (type == Format::Object::MOVIE ||
		type == Format::Object::ATTACHEDMOVIE) ? true : false;}
	bool IsParticle() {return type == Format::Object::PARTICLE ? true : false;}
	bool IsProgramObject()
		{return type == Format::Object::PROGRAMOBJECT ? true : false;}
	bool IsText() {return type == Format::Object::TEXT ? true : false;}
};

}	// namespace LWF

#endif
