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
#include "lwf_cocos2dx_factory.h"
#include "lwf_cocos2dx_node.h"
#include "lwf_cocos2dx_textbmfont.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_text.h"

namespace LWF {

class LWFTextBMFont : public cocos2d::Label, public BlendEquationProtocol
{
protected:
	cocos2d::Mat4 m_nodeToParentTransform;
	Matrix m_matrix;
	float m_offsetY;
	float m_scale;
	float m_red;
	float m_green;
	float m_blue;
	cocos2d::TextVAlignment m_vAlignment;

public:
	static LWFTextBMFont *create(const char *string,
		const char *fontPath, float fontHeight, float width, float height,
		cocos2d::TextHAlignment hAlignment, cocos2d::TextVAlignment vAlignment,
		float red, float green, float blue)
	{
		LWFTextBMFont *ret = new LWFTextBMFont(nullptr, hAlignment);
		if (!ret)
			return nullptr;

		if (!ret->setBMFontFilePath(fontPath)) {
			delete ret;
			return nullptr;
		}

		ret->setParameter(
			fontHeight, width, height, vAlignment, red, green, blue);
		ret->setString(string);
		ret->autorelease();

		return ret;
	}

	LWFTextBMFont(cocos2d::FontAtlas *atlas, cocos2d::TextHAlignment hAlignment)
		: Label(atlas, hAlignment)
	{
		m_matrix.Invalidate();
	}

	virtual ~LWFTextBMFont()
	{
	}

	void setParameter(float fontHeight, float width, float height,
		cocos2d::TextVAlignment vAlignment, float red, float green, float blue)
	{
		cocos2d::Size visibleSize =
			cocos2d::Director::getInstance()->getVisibleSize();
		cocos2d::Size winSize =
			cocos2d::Director::getInstance()->getWinSize();
		float contentScaleFactor =
			cocos2d::Director::getInstance()->getContentScaleFactor();
		m_scale = fontHeight / getLineHeight() * 96.0f / 72.0f *
			visibleSize.height / winSize.height;
		m_vAlignment = vAlignment;
		m_red = red;
		m_green = green;
		m_blue = blue;
		setDimensions(width / m_scale / contentScaleFactor,
			height / m_scale / contentScaleFactor);
	}

	virtual void setVisible(bool bVisible) override
	{
		if (bVisible && !isVisible())
			m_matrix.Invalidate();
		cocos2d::Label::setVisible(bVisible);
	}

	virtual const cocos2d::Mat4& getNodeToParentTransform() const override
	{
		return m_nodeToParentTransform;
	}

	void setMatrixAndColorTransform(
		cocos2d::LWFNode *node, const Matrix *m, const ColorTransform *cx)
	{
		bool changed = m_matrix.SetWithComparing(m);
		if (changed) {
			float scale = m_scale / node->lwf->scaleByStage;
			m_nodeToParentTransform = cocos2d::Mat4(
				m->scaleX * scale, -m->skew0 * scale, 0,
					m->translateX + m->skew0 * m_offsetY -
						m->skew1 * getHeight() * scale,
				-m->skew1 * scale, m->scaleY * scale, 0,
					-m->translateY - m->scaleY * m_offsetY -
						m->scaleY * getHeight() * scale,
				0, 0, 1, 0,
				0, 0, 0, 1);
			setNodeToParentTransform(m_nodeToParentTransform);
		}

		const Color &c = cx->multi;
		const cocos2d::Color3B &dc = node->getDisplayedColor();
		setColor((cocos2d::Color3B){
			(GLubyte)(c.red * m_red * dc.r),
			(GLubyte)(c.green * m_green * dc.g),
			(GLubyte)(c.blue * m_blue * dc.b)});
		setOpacity((GLubyte)(c.alpha * node->getDisplayedOpacity()));
	}

	void setString(const std::string &label)
	{
		cocos2d::Label::setString(label);

		float height = (float)getHeight();
		switch (m_vAlignment) {
		case cocos2d::TextVAlignment::TOP:
			m_offsetY = 0;
			break;
		case cocos2d::TextVAlignment::BOTTOM:
			m_offsetY = height;
			break;
		case cocos2d::TextVAlignment::CENTER:
			m_offsetY = height / 2.0f;
			break;
		}
	}

	virtual void draw(cocos2d::Renderer *renderer,
		const cocos2d::Mat4 &transform, uint32_t flags) override
	{
		if (m_blendEquation)
			BlendEquationProtocol::addBeginCommand(
				renderer, transform, flags, _globalZOrder);

		cocos2d::Label::draw(renderer, transform, flags);

		if (m_blendEquation)
			BlendEquationProtocol::addEndCommand(
				renderer, transform, flags, _globalZOrder);
	}
};

LWFTextBMFontRenderer::LWFTextBMFontRenderer(
		LWF *l, Text *text, const char *fontName, cocos2d::LWFNode *node)
	: TextRenderer(l), m_label(0)
{
	const Format::Text &t = l->data->texts[text->objectId];
	const Color &c = l->data->colors[t.colorId];
	const Format::TextProperty &p = l->data->textProperties[t.textPropertyId];
	cocos2d::TextHAlignment hAlignment;
	cocos2d::TextVAlignment vAlignment;

	switch (p.align & Format::TextProperty::ALIGN_MASK) {
	default:
	case Format::TextProperty::LEFT:
		hAlignment = cocos2d::TextHAlignment::LEFT;
		break;
	case Format::TextProperty::RIGHT:
		hAlignment = cocos2d::TextHAlignment::RIGHT;
		break;
	case Format::TextProperty::CENTER:
		hAlignment = cocos2d::TextHAlignment::CENTER;
		break;
	}

	switch (p.align & Format::TextProperty::VERTICAL_MASK) {
	default:
		vAlignment = cocos2d::TextVAlignment::TOP;
		break;
	case Format::TextProperty::VERTICAL_BOTTOM:
		vAlignment = cocos2d::TextVAlignment::BOTTOM;
		break;
	case Format::TextProperty::VERTICAL_MIDDLE:
		vAlignment = cocos2d::TextVAlignment::CENTER;
		break;
	}

	m_label = LWFTextBMFont::create(l->data->strings[t.stringId].c_str(),
		fontName, p.fontHeight, t.width, t.height,
		hAlignment, vAlignment, c.red, c.green, c.blue);

	if (!m_label)
		return;

	m_factory = (LWFRendererFactory *)l->rendererFactory.get();
	node->addChild(m_label);
}

LWFTextBMFontRenderer::~LWFTextBMFontRenderer()
{
}

void LWFTextBMFontRenderer::Destruct()
{
	if (!m_label)
		return;

	cocos2d::LWFNode::removeNodeFromParent(m_label);
	m_label = 0;
}

void LWFTextBMFontRenderer::Update(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
}

void LWFTextBMFontRenderer::Render(
	const Matrix *matrix, const ColorTransform *colorTransform,
	int renderingIndex, int renderingCount, bool visible)
{
	if (!m_label)
		return;

	if (!m_factory->Render(lwf, m_label, m_label, renderingIndex, visible))
		return;

	m_label->setMatrixAndColorTransform(
		m_factory->GetNode(), matrix, colorTransform);
}

void LWFTextBMFontRenderer::SetText(string text)
{
	if (!m_label)
		return;

	m_label->setString(text.c_str());
}

}   // namespace LWF
