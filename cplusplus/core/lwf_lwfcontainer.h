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

#ifndef LWF_LWFCONTAINER_H
#define LWF_LWFCONTAINER_H

#include "lwf_button.h"

namespace LWF {

class LWFCore;
class Movie;

class LWFContainer : public Button
{
public:
	shared_ptr<LWFCore> child;

public:
	LWFContainer(Movie *p, shared_ptr<LWFCore> c);
	bool CheckHit(float px, float py);
	void RollOver();
	void RollOut();
	void Press();
	void Release();
	void KeyPress(int code);
};

}	// namespace LWF

#endif
