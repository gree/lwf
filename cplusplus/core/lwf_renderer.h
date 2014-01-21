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

#ifndef LWF_RENDERER_H
#define LWF_RENDERER_H

#include "lwf_format.h"

namespace LWF {

class Bitmap;
class BitmapEx;
class Particle;
class Text;

class Renderer
{
public:
	LWFCore *lwf;

public:
	Renderer(LWFCore *l) : lwf(l) {}
	virtual ~Renderer() {}

	virtual void Destruct() = 0;
	virtual void Update(
		const Matrix *matrix, const ColorTransform *colorTransform) = 0;
	virtual void Render(
		const Matrix *matrix, const ColorTransform *colorTransform,
		int renderingIndex, int renderingCount, bool visible) = 0;
};

class TextRenderer : public Renderer
{
public:
	TextRenderer(LWFCore *l) : Renderer(l) {}
	virtual ~TextRenderer() {}

	virtual void SetText(string text) = 0;
};

class IRendererFactory
{
public:
	IRendererFactory() {}
	virtual ~IRendererFactory() {}
	virtual shared_ptr<Renderer> ConstructBitmap(
		LWFCore *lwf, int objId, Bitmap *bitmap) = 0;
	virtual shared_ptr<Renderer> ConstructBitmapEx(
		LWFCore *lwf, int objId, BitmapEx *bitmapEx) = 0;
	virtual shared_ptr<TextRenderer> ConstructText(
		LWFCore *lwf, int objId, Text *text) = 0;
	virtual shared_ptr<Renderer> ConstructParticle(
		LWFCore *lwf, int objId, Particle *particle) = 0;
	virtual void Init(LWFCore *lwf) = 0;
	virtual void BeginRender(LWFCore *lwf) = 0;
	virtual void EndRender(LWFCore *lwf) = 0;
	virtual void Destruct() = 0;
	virtual void SetBlendMode(int blendMode) = 0;
	virtual void SetMaskMode(int maskMode) = 0;

	virtual void FitForHeight(LWFCore *lwf, float w, float h) = 0;
	virtual void FitForWidth(LWFCore *lwf, float w, float h) = 0;
	virtual void ScaleForHeight(LWFCore *lwf, float w, float h) = 0;
	virtual void ScaleForWidth(LWFCore *lwf, float w, float h) = 0;
};

class NullRendererFactory : public IRendererFactory
{
public:
	shared_ptr<Renderer> ConstructBitmap(LWFCore *lwf,
		int objId, Bitmap *bitmap) {return 0;}
	shared_ptr<Renderer> ConstructBitmapEx(LWFCore *lwf,
		int objId, BitmapEx *bitmapEx) {return 0;}
	shared_ptr<TextRenderer> ConstructText(LWFCore *lwf,
		int objId, Text *text) {return 0;}
	shared_ptr<Renderer> ConstructParticle(LWFCore *lwf,
		int objId, Particle *particle) {return 0;}
	void Init(LWFCore *lwf) {}
	void BeginRender(LWFCore *lwf) {}
	void EndRender(LWFCore *lwf) {}
	void Destruct() {}
	void SetBlendMode(int blendMode) {}
	void SetMaskMode(int maskMode) {}

	void FitForHeight(LWFCore *lwf, float w, float h) {}
	void FitForWidth(LWFCore *lwf, float w, float h) {}
	void ScaleForHeight(LWFCore *lwf, float w, float h) {}
	void ScaleForWidth(LWFCore *lwf, float w, float h) {}
};

}	// namespace LWF

#endif
