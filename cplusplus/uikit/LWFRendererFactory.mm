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

#import "LWFRendererFactory.h"
#import "LWFBitmapRenderer.h"
#import "LWFTextRenderer.h"
#import "lwf_core.h"
#import "lwf_property.h"

namespace LWF {

shared_ptr<Renderer> LWFRendererFactory::ConstructBitmap(
	LWF *lwf, int objId, Bitmap *bitmap)
{
	return make_shared<LWFBitmapRenderer>(this, lwf, bitmap);
}

shared_ptr<Renderer> LWFRendererFactory::ConstructBitmapEx(
	LWF *lwf, int objId, BitmapEx *bitmapEx)
{
	return make_shared<LWFBitmapRenderer>(this, lwf, bitmapEx);
}

shared_ptr<TextRenderer> LWFRendererFactory::ConstructText(
	LWF *lwf, int objId, Text *text)
{
	return make_shared<LWFTextRenderer>(this, lwf, text);
}

shared_ptr<Renderer> LWFRendererFactory::ConstructParticle(
	LWF *lwf, int objId, Particle *particle)
{
	return shared_ptr<Renderer>();
}

void LWFRendererFactory::FitForHeight(class LWF *lwf, float w, float h)
{
	ScaleForHeight(lwf, w, h);
	float offsetX = (w - lwf->width * lwf->scaleByStage) / 2.0f;
	float offsetY = 0;
	lwf->property->MoveTo(offsetX, offsetY);
}

void LWFRendererFactory::FitForWidth(class LWF *lwf, float w, float h)
{
	ScaleForWidth(lwf, w, h);
	float offsetX = 0;
	float offsetY = (h - lwf->height * lwf->scaleByStage) / 2.0f;
	lwf->property->MoveTo(offsetX, offsetY);
}

void LWFRendererFactory::ScaleForHeight(class LWF *lwf, float w, float h)
{
	float scale = h / lwf->height;
	lwf->scaleByStage = scale;
	lwf->property->ScaleTo(scale, scale);
}

void LWFRendererFactory::ScaleForWidth(class LWF *lwf, float w, float h)
{
	float scale = w / lwf->width;
	lwf->scaleByStage = scale;
	lwf->property->ScaleTo(scale, scale);
}

}	// namespace LWF
