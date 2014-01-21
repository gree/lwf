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

#ifndef LWF_GRAPHIC_H
#define LWF_GRAPHIC_H

#include "lwf_object.h"

namespace LWF {

class Graphic : public Object
{
public:
	typedef vector<shared_ptr<Object> > DisplayList;

private:
	DisplayList m_displayList;

public:
	Graphic(LWFCore *l, Movie *p, int objId);
	void Update(const Matrix *m, const ColorTransform *c);
	void Render(bool v, int rOffset);
	void Destroy();
};

}	// namespace LWF

#endif
