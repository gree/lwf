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

#ifndef LWF_TYPE_H
#define LWF_TYPE_H

#include <algorithm>
#include <cmath>
#include <cfloat>
#include <functional>
#include <limits>
#include <map>
#include <memory>
#include <string>
#include <utility>
#include <vector>

#if __cplusplus <= 199711L
#include <boost/function.hpp>
#include <boost/make_shared.hpp>
#include <boost/scoped_ptr.hpp>
#include <boost/shared_ptr.hpp>
#endif

namespace LWF {

using std::advance;
using std::for_each;
using std::make_pair;
using std::map;
using std::pair;
using std::remove_if;
using std::string;
using std::vector;

#if __cplusplus > 199711L
using std::function;
using std::make_shared;
using std::shared_ptr;
using std::unique_ptr;
#else
using boost::function;
using boost::make_shared;
using boost::scoped_ptr;
using boost::shared_ptr;
#endif

class LWF;
class Object;
class Movie;
class Button;

typedef function<void (Object *, int, int, int)> Inspector;
typedef function<bool (LWF *)> DetachHandler;
typedef function<void (Movie *, Button *)> EventHandler;
typedef vector<pair<int, EventHandler> > EventHandlerList;
typedef function<void ()> PreloadCallback;

class Point
{
public:
	float x;
	float y;

	Point(float px = 0, float py = 0)
	{
		x = px;
		y = py;
	}
};

class Translate
{
public:
	float translateX;
	float translateY;

	Translate()
	{
		translateX = 0;
		translateY = 0;
	}
};

class Matrix
{
public:
	float scaleX;
	float scaleY;
	float skew0;
	float skew1;
	float translateX;
	float translateY;

	Matrix()
	{
		Clear();
	}

	Matrix(const Matrix &m)
	{
		Set(&m);
	}

	Matrix(float scX, float scY, float sk0, float sk1, float tX, float tY)
	{
		scaleX = scX;
		scaleY = scY;
		skew0 = sk0;
		skew1 = sk1;
		translateX = tX;
		translateY = tY;
	}

	void Clear()
	{
		scaleX = 1;
		scaleY = 1;
		skew0 = 0;
		skew1 = 0;
		translateX = 0;
		translateY = 0;
	}

	Matrix &Set(const Matrix *m)
	{
		scaleX = m->scaleX;
		scaleY = m->scaleY;
		skew0 = m->skew0;
		skew1 = m->skew1;
		translateX = m->translateX;
		translateY = m->translateY;
		return *this;
	}

	Matrix &Set(float scX, float scY, float sk0, float sk1, float tX, float tY)
	{
		scaleX = scX;
		scaleY = scY;
		skew0 = sk0;
		skew1 = sk1;
		translateX = tX;
		translateY = tY;
		return *this;
	}

	bool SetWithComparing(const Matrix *m)
	{
		if (!m)
			return false;

		float sX = m->scaleX;
		float sY = m->scaleY;
		float s0 = m->skew0;
		float s1 = m->skew1;
		float tX = m->translateX;
		float tY = m->translateY;
		bool changed = false;
		if (scaleX != sX) {
			scaleX = sX;
			changed = true;
		}
		if (scaleY != sY) {
			scaleY = sY;
			changed = true;
		}
		if (skew0 != s0) {
			skew0 = s0;
			changed = true;
		}
		if (skew1 != s1) {
			skew1 = s1;
			changed = true;
		}
		if (translateX != tX) {
			translateX = tX;
			changed = true;
		}
		if (translateY != tY) {
			translateY = tY;
			changed = true;
		}
		return changed;
	}
};

class Color
{
public:
	float red, green, blue, alpha;

	Color() {}

	Color(float r, float g, float b, float a)
	{
		Set(r, g, b, a);
	}

	void Set(float r, float g, float b, float a)
	{
		red = r;
		green = g;
		blue = b;
		alpha = a;
	}

	void Set(const Color *c)
	{
		red = c->red;
		green = c->green;
		blue = c->blue;
		alpha = c->alpha;
	}
};

class AlphaTransform
{
public:
	float alpha;

	AlphaTransform() {alpha = 1;}
	AlphaTransform(float a) {alpha = a;}
};

class ColorTransform
{
public:
	Color multi;
	Color add;

	ColorTransform(
		float multiRed = 1,
		float multiGreen = 1,
		float multiBlue = 1,
		float multiAlpha = 1
#if LWF_USE_ADDITIONALCOLOR
		,
		float addRed = 0,
		float addGreen = 0,
		float addBlue = 0,
		float addAlpha = 0
#endif
		)
	{
		multi.Set(multiRed, multiGreen, multiBlue, multiAlpha);
#if LWF_USE_ADDITIONALCOLOR
		add.Set(addRed, addGreen, addBlue, addAlpha);
#endif
	}

	ColorTransform(const ColorTransform &c)
	{
		Set(&c);
	}

	void Clear()
	{
		multi.Set(1, 1, 1, 1);
#if LWF_USE_ADDITIONALCOLOR
		add.Set(0, 0, 0, 0);
#endif
	}

	ColorTransform &Set(const ColorTransform *c)
	{
		multi.Set(&c->multi);
#if LWF_USE_ADDITIONALCOLOR
		add.Set(&c->add);
#endif
		return *this;
	}

	ColorTransform &Set(
		float multiRed = 1,
		float multiGreen = 1,
		float multiBlue = 1,
		float multiAlpha = 1
#if LWF_USE_ADDITIONALCOLOR
		,
		float addRed = 0,
		float addGreen = 0,
		float addBlue = 0,
		float addAlpha = 0
#endif
		)
	{
		multi.Set(multiRed, multiGreen, multiBlue, multiAlpha);
#if LWF_USE_ADDITIONALCOLOR
		add.Set(addRed, addGreen, addBlue, addAlpha);
#endif
		return *this;
	}

	bool SetWithComparing(const ColorTransform *c)
	{
		if (!c)
			return false;

		const Color &cm = c->multi;
		float red = cm.red;
		float green = cm.green;
		float blue = cm.blue;
		float alpha = cm.alpha;
		bool changed = false;
		Color &m = multi;
		if (m.red != red) {
			m.red = red;
			changed = true;
		}
		if (m.green != green) {
			m.green = green;
			changed = true;
		}
		if (m.blue != blue) {
			m.blue = blue;
			changed = true;
		}
		if (m.alpha != alpha) {
			m.alpha = alpha;
			changed = true;
		}
#if LWF_USE_ADDITIONALCOLOR
		const Color &ca = c->add;
		red = ca.red;
		green = ca.green;
		blue = ca.blue;
		alpha = ca.alpha;
		Color &a = add;
		if (a.red != red) {
			a.red = red;
			changed = true;
		}
		if (a.green != green) {
			a.green = green;
			changed = true;
		}
		if (a.blue != blue) {
			a.blue = blue;
			changed = true;
		}
		if (a.alpha != alpha) {
			a.alpha = alpha;
			changed = true;
		}
#endif
		return changed;
	}
};

}	// namespace LWF

#endif
