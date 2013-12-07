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

#include "cocos2d.h"
#include "lwf_cocos2dx_bitmap.h"
#include "lwf_cocos2dx_factory.h"
#include "lwf_cocos2dx_particle.h"
#include "lwf_cocos2dx_textbmfont.h"
#include "lwf_cocos2dx_textttf.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_property.h"
#include "lwf_text.h"

USING_NS_CC;

namespace LWF {

shared_ptr<Renderer> LWFRendererFactory::ConstructBitmap(
	LWF *lwf, int objId, Bitmap *bitmap)
{
	return make_shared<LWFBitmapRenderer>(lwf, bitmap, m_node);
}

shared_ptr<Renderer> LWFRendererFactory::ConstructBitmapEx(
	LWF *lwf, int objId, BitmapEx *bitmapEx)
{
	return make_shared<LWFBitmapRenderer>(lwf, bitmapEx, m_node);
}

shared_ptr<TextRenderer> LWFRendererFactory::ConstructText(
	LWF *lwf, int objId, Text *text)
{
	const Format::Text &t = lwf->data->texts[text->objectId];
	const Format::TextProperty &p = lwf->data->textProperties[t.textPropertyId];
	const Format::Font &f = lwf->data->fonts[p.fontId];
	string fontName = lwf->data->strings[f.stringId];

	if (fontName[0] == '_') {
		fontName = fontName.substr(1);
		return make_shared<LWFTextTTFRenderer>(
			lwf, text, fontName.c_str(), m_node);
	} else {
		return make_shared<LWFTextBMFontRenderer>(
			lwf, text, fontName.c_str(), m_node);
	}
}

shared_ptr<Renderer> LWFRendererFactory::ConstructParticle(
	LWF *lwf, int objId, Particle *particle)
{
	return make_shared<LWFParticleRenderer>(lwf, particle, m_node);
}

void LWFRendererFactory::Init(LWF *lwf)
{
}

void LWFRendererFactory::BeginRender(LWF *lwf)
{
}

void LWFRendererFactory::EndRender(LWF *lwf)
{
}

void LWFRendererFactory::Destruct()
{
}

void LWFRendererFactory::FitForHeight(class LWF *lwf, float w, float h)
{
	ScaleForHeight(lwf, w, h);
	float offsetX = (w - lwf->width * lwf->scaleByStage) / 2.0f;
	float offsetY = -h;
	lwf->property->Move(offsetX, offsetY);
}

void LWFRendererFactory::FitForWidth(class LWF *lwf, float w, float h)
{
	ScaleForWidth(lwf, w, h);
	float offsetX = (w - lwf->width * lwf->scaleByStage) / 2.0f;
	float offsetY = -h + (h - lwf->height * lwf->scaleByStage) / 2.0f;
	lwf->property->Move(offsetX, offsetY);
}

void LWFRendererFactory::ScaleForHeight(class LWF *lwf, float w, float h)
{
	float scale = h / lwf->height;
	lwf->scaleByStage = scale;
	lwf->property->Scale(scale, scale);
}

void LWFRendererFactory::ScaleForWidth(class LWF *lwf, float w, float h)
{
	float scale = w / lwf->width;
	lwf->scaleByStage = scale;
	lwf->property->Scale(scale, scale);
}

}	// namespace LWF
