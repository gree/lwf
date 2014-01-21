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

#ifndef LWF_COCOS2DX_FACTORY_H
#define LWF_COCOS2DX_FACTORY_H

#include "lwf_renderer.h"

namespace cocos2d {
class LWFNode;
}

namespace LWF {

class LWFRendererFactory : public IRendererFactory
{
protected:
	cocos2d::LWFNode *m_node;
	int m_blendMode;
	int m_maskMode;

public:
	LWFRendererFactory(cocos2d::LWFNode *node)
		: m_node(node)
	{
	}

	shared_ptr<Renderer> ConstructBitmap(
		LWFCore *lwf, int objId, Bitmap *bitmap);
	shared_ptr<Renderer> ConstructBitmapEx(
		LWFCore *lwf, int objId, BitmapEx *bitmapEx);
	shared_ptr<TextRenderer> ConstructText(
		LWFCore *lwf, int objId, Text *text);
	shared_ptr<Renderer> ConstructParticle(
		LWFCore *lwf, int objId, Particle *particle);

	void Init(LWFCore *lwf);
	void BeginRender(LWFCore *lwf);
	void EndRender(LWFCore *lwf);
	void Destruct();
	void SetBlendMode(int blendMode) {m_blendMode = blendMode;}
	void SetMaskMode(int maskMode) {m_maskMode = maskMode;}
	int GetBlendMode() {return m_blendMode;}
	int GetMaskMode() {return m_maskMode;}

	void FitForHeight(LWFCore *lwf, float w, float h);
	void FitForWidth(LWFCore *lwf, float w, float h);
	void ScaleForHeight(LWFCore *lwf, float w, float h);
	void ScaleForWidth(LWFCore *lwf, float w, float h);
};

}	// namespace LWFCore

#endif
