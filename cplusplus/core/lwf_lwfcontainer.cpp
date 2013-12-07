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

#include "lwf_lwfcontainer.h"
#include "lwf_core.h"
#include "lwf_movie.h"

namespace LWF {

LWFContainer::LWFContainer(Movie *p, shared_ptr<LWF> c)
{
	lwf = p->lwf;
	parent = p;
	child = c;
}

bool LWFContainer::CheckHit(float px, float py)
{
	Button *button = child->InputPoint(px, py);
	return button ? true : false;
}

void LWFContainer::RollOver()
{
	// NOTHING TO DO
}

void LWFContainer::RollOut()
{
	if (child->focus) {
		child->focus->RollOut();
		child->ClearFocus(child->focus);
	}
}

void LWFContainer::Press()
{
	child->InputPress();
}

void LWFContainer::Release()
{
	child->InputRelease();
}

void LWFContainer::KeyPress(int code)
{
	child->InputKeyPress(code);
}

}	// namespace LWF
