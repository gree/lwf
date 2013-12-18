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
	LWF *lwf;

public:
	Renderer(LWF *l) : lwf(l) {}
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
	TextRenderer(LWF *l) : Renderer(l) {}
	virtual ~TextRenderer() {}

	virtual void SetText(string text) = 0;
};

class IRendererFactory
{
public:
	IRendererFactory() {}
	virtual ~IRendererFactory() {}
	virtual shared_ptr<Renderer> ConstructBitmap(
		LWF *lwf, int objId, Bitmap *bitmap) = 0;
	virtual shared_ptr<Renderer> ConstructBitmapEx(
		LWF *lwf, int objId, BitmapEx *bitmapEx) = 0;
	virtual shared_ptr<TextRenderer> ConstructText(
		LWF *lwf, int objId, Text *text) = 0;
	virtual shared_ptr<Renderer> ConstructParticle(
		LWF *lwf, int objId, Particle *particle) = 0;
	virtual void Init(LWF *lwf) = 0;
	virtual void BeginRender(LWF *lwf) = 0;
	virtual void EndRender(LWF *lwf) = 0;
	virtual void Destruct() = 0;
	virtual void SetBlendMode(int blendMode) = 0;
	virtual void SetMaskMode(int maskMode) = 0;

	virtual void FitForHeight(LWF *lwf, float w, float h) = 0;
	virtual void FitForWidth(LWF *lwf, float w, float h) = 0;
	virtual void ScaleForHeight(LWF *lwf, float w, float h) = 0;
	virtual void ScaleForWidth(LWF *lwf, float w, float h) = 0;
};

class NullRendererFactory : public IRendererFactory
{
public:
	shared_ptr<Renderer> ConstructBitmap(LWF *lwf,
		int objId, Bitmap *bitmap) {return shared_ptr<Renderer>();}
	shared_ptr<Renderer> ConstructBitmapEx(LWF *lwf,
		int objId, BitmapEx *bitmapEx) {return shared_ptr<Renderer>();}
	shared_ptr<TextRenderer> ConstructText(LWF *lwf,
		int objId, Text *text) {return shared_ptr<TextRenderer>();}
	shared_ptr<Renderer> ConstructParticle(LWF *lwf,
		int objId, Particle *particle) {return shared_ptr<Renderer>();}
	void Init(LWF *lwf) {}
	void BeginRender(LWF *lwf) {}
	void EndRender(LWF *lwf) {}
	void Destruct() {}
	void SetBlendMode(int blendMode) {}
	void SetMaskMode(int maskMode) {}

	void FitForHeight(LWF *lwf, float w, float h) {}
	void FitForWidth(LWF *lwf, float w, float h) {}
	void ScaleForHeight(LWF *lwf, float w, float h) {}
	void ScaleForWidth(LWF *lwf, float w, float h) {}
};

}	// namespace LWF

#endif
