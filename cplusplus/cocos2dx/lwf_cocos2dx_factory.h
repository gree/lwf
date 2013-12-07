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

public:
	LWFRendererFactory(cocos2d::LWFNode *node)
		: m_node(node)
	{
	}

	shared_ptr<Renderer> ConstructBitmap(
		LWF *lwf, int objId, Bitmap *bitmap);
	shared_ptr<Renderer> ConstructBitmapEx(
		LWF *lwf, int objId, BitmapEx *bitmapEx);
	shared_ptr<TextRenderer> ConstructText(
		LWF *lwf, int objId, Text *text);
	shared_ptr<Renderer> ConstructParticle(
		LWF *lwf, int objId, Particle *particle);

	void Init(LWF *lwf);
	void BeginRender(LWF *lwf);
	void EndRender(LWF *lwf);
	void Destruct();

	void FitForHeight(LWF *lwf, float w, float h);
	void FitForWidth(LWF *lwf, float w, float h);
	void ScaleForHeight(LWF *lwf, float w, float h);
	void ScaleForWidth(LWF *lwf, float w, float h);
};

}	// namespace LWF

#endif
