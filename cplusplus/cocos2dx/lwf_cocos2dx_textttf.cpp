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
#include "lwf_cocos2dx_textttf.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_text.h"

namespace LWF {

class LWFTextTTFImpl : public cocos2d::LWFText, public BlendEquationProtocol
{
protected:
	Text *m_text;
	cocos2d::Mat4 m_nodeToParentTransform;
	Matrix m_matrix;
	float m_fontHeight;

public:
	static LWFTextTTFImpl *create(Text *text, bool useTTF, const char *string,
		const char *fontName, float fontSize,
		const cocos2d::Size& dimensions, cocos2d::TextHAlignment hAlignment, 
		cocos2d::TextVAlignment vAlignment, float red, float green, float blue)
	{
		LWFTextTTFImpl *ret =
			new LWFTextTTFImpl(text, nullptr, hAlignment, vAlignment);
		if (!ret)
			return nullptr;

		if (useTTF) {
			cocos2d::TTFConfig ttfConfig(
				fontName, fontSize, cocos2d::GlyphCollection::DYNAMIC);
			if (!ret->setTTFConfig(ttfConfig)) {
				delete ret;
				return nullptr;
			}
		} else {
			ret->setSystemFontName(fontName);
			ret->setSystemFontSize(fontSize);
			ret->setBlendFunc(cocos2d::BlendFunc::ALPHA_NON_PREMULTIPLIED);
		}

		ret->setDimensions(dimensions.width, dimensions.height);
		ret->setParameter(useTTF, fontSize, red, green, blue);
		ret->setString(string);
		ret->autorelease();

		return ret;
	}

	LWFTextTTFImpl(Text *text, cocos2d::FontAtlas *atlas,
			cocos2d::TextHAlignment hAlignment,
			cocos2d::TextVAlignment vAlignment)
		: LWFText(atlas, hAlignment, vAlignment),
			BlendEquationProtocol(), m_text(text)
	{
		m_matrix.Invalidate();
	}

	virtual ~LWFTextTTFImpl()
	{
	}

	virtual Text *GetText()
	{
		return m_text;
	}

	void setParameter(
		bool useTTF, float fontHeight, float red, float green, float blue)
	{
		m_fontHeight = fontHeight;
		if (!useTTF)
			m_fontHeight *= 96.0f / 72.0f;
		setTextColor(cocos2d::Color4B(cocos2d::Color4F(red, green, blue, 1)));
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
			m_nodeToParentTransform = cocos2d::Mat4(
				m->scaleX, -m->skew0, 0,
					m->translateX + m->skew0 * getHeight(),
				-m->skew1, m->scaleY, 0,
					-m->translateY - m->scaleY * getHeight(),
				0, 0, 1, 0,
				0, 0, 0, 1);
			setNodeToParentTransform(m_nodeToParentTransform);
		}

		const Color &c = cx->multi;
		const cocos2d::Color3B &dc = node->getDisplayedColor();
		setColor({
			(GLubyte)(c.red * dc.r),
			(GLubyte)(c.green * dc.g),
			(GLubyte)(c.blue * dc.b)});
		setOpacity((GLubyte)(c.alpha * node->getDisplayedOpacity()));
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

LWFTextTTFRenderer::LWFTextTTFRenderer(LWF *l, Text *text, bool useTTF,
		const char *fontName, cocos2d::LWFNode *node)
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

	cocos2d::Size s = cocos2d::Size(t.width, t.height);

	m_label = LWFTextTTFImpl::create(text, useTTF,
		l->data->strings[t.stringId].c_str(), fontName, p.fontHeight, s,
		hAlignment, vAlignment, c.red, c.green, c.blue);

	if (!m_label)
		return;

	m_factory = (LWFRendererFactory *)l->rendererFactory.get();

	const cocos2d::LWFNodeHandlers &h = node->getNodeHandlers();
	if (h.onTextLoaded)
		h.onTextLoaded(m_label);

	node->addChild(m_label);
}

LWFTextTTFRenderer::~LWFTextTTFRenderer()
{
}

void LWFTextTTFRenderer::Destruct()
{
	if (!m_label)
		return;

	cocos2d::LWFNode::removeNodeFromParent(m_label);
	m_label = 0;
}

void LWFTextTTFRenderer::Update(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
}

void LWFTextTTFRenderer::Render(
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

void LWFTextTTFRenderer::SetText(string text)
{
	if (!m_label)
		return;

	m_label->setString(text.c_str());
}

cocos2d::LWFText *LWFTextTTFRenderer::GetLabel()
{
	return dynamic_cast<cocos2d::LWFText *>(m_label);
}

}   // namespace LWF
