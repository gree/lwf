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
#include "renderer/CCCustomCommand.h"
#include "renderer/CCRenderer.h"

namespace cocos2d {
class LWFNode;
class LWFMask;
}

namespace LWF {

class BlendEquationProtocol;

class LWFRendererFactory : public IRendererFactory
{
protected:
	typedef std::vector<cocos2d::LWFMask *> Masks_t;

protected:
	cocos2d::LWFNode *m_node;
	string m_basePath;
	Masks_t m_masks;
	int m_blendMode;
	int m_maskMode;
	int m_lastMaskMode;
	int m_maskNo;
	int m_renderingIndex;

public:
	LWFRendererFactory(cocos2d::LWFNode *node, string basePath)
		: m_node(node), m_basePath(basePath),
			m_blendMode(Format::BLEND_MODE_NORMAL),
			m_maskMode(Format::BLEND_MODE_NORMAL),
			m_lastMaskMode(Format::BLEND_MODE_NORMAL)
	{
	}

	virtual ~LWFRendererFactory()
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
	void SetBlendMode(int blendMode) {m_blendMode = blendMode;}
	void SetMaskMode(int maskMode) {m_maskMode = maskMode;}
	int GetBlendMode() {return m_blendMode;}

	void FitForHeight(LWF *lwf, float w, float h);
	void FitForWidth(LWF *lwf, float w, float h);
	void ScaleForHeight(LWF *lwf, float w, float h);
	void ScaleForWidth(LWF *lwf, float w, float h);

	bool Render(LWF *lwf, cocos2d::Node *node,
		BlendEquationProtocol *be, int renderingIndex,
		bool visible, cocos2d::BlendFunc *baseBlendFunc = 0);

	const string &GetBasePath() const {return m_basePath;}
	cocos2d::LWFNode *GetNode() {return m_node;}
};

class BlendEquationProtocol
{
protected:
	int m_blendEquation;
	cocos2d::CustomCommand m_beginCommand;
	cocos2d::CustomCommand m_endCommand;

public:
	BlendEquationProtocol();
	virtual ~BlendEquationProtocol() {}

	void setBlendEquation(int blendMode);
	void addBeginCommand(cocos2d::Renderer *renderer,
		const cocos2d::Mat4 &transform, uint32_t flags, float globalZOrder);
	void addEndCommand(cocos2d::Renderer *renderer,
		const cocos2d::Mat4 &transform, uint32_t flags, float globalZOrder);
	void onBlendEquationBegin(const cocos2d::Mat4 &transform, uint32_t flags);
	void onBlendEquationEnd(const cocos2d::Mat4 &transform, uint32_t flags);
};

}	// namespace LWF

#endif
