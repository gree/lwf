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

#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_programobj.h"
#include "lwf_renderer.h"

namespace LWF {

ProgramObject::ProgramObject(LWF *l, Movie *p, int objId)
	: Object(l, p, Format::Object::PROGRAMOBJECT, objId)
{
	const ProgramObjectConstructor ctor =
		lwf->GetProgramObjectConstructor(objId);
	if (!ctor)
		return;

	const Format::ProgramObject &data = lwf->data->programObjects[objId];
	dataMatrixId = data.matrixId;
	renderer = ctor(this, objId, data.width, data.height);
}

void ProgramObject::Update(const Matrix *m, const ColorTransform *c)
{
	Object::Update(m, c);
	if (renderer)
		renderer->Update(&matrix, &colorTransform);
}

}	// namespace LWF
