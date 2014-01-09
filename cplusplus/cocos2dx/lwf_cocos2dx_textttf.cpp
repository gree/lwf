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

class LWFTextTTF : public cocos2d::LabelTTF
{
protected:
	Matrix m_matrix;
	float m_red;
	float m_green;
	float m_blue;

public:
	static LWFTextTTF *create(const char *string,
		const char *fontName, float fontSize,
		const cocos2d::Size& dimensions, cocos2d::TextHAlignment hAlignment, 
		cocos2d::TextVAlignment vAlignment, float red, float green, float blue)
	{
		LWFTextTTF *text = new LWFTextTTF();
		if (text && text->initWithString(string, fontName, fontSize,
				dimensions, hAlignment, vAlignment, red, green, blue)) {
			text->autorelease();
			return text;
		}
		CC_SAFE_DELETE(text);
		return NULL;
	}

	bool initWithString(const char *string, const char *fontName,
			float fontSize, const cocos2d::Size& dimensions,
			cocos2d::TextHAlignment hAlignment,
			cocos2d::TextVAlignment vAlignment,
			float red, float green, float blue)
	{
		if (!cocos2d::LabelTTF::initWithString(string,
				fontName, fontSize, dimensions, hAlignment, vAlignment))
			return false;

		setFlippedY(true);
		setRGB(red, green, blue);
		return true;
	}

	void setRGB(float red, float green, float blue)
	{
		m_red = red;
		m_green = green;
		m_blue = blue;
	}

	void setMatrixAndColorTransform(const Matrix *m, const ColorTransform *cx)
	{
		bool changed = m_matrix.SetWithComparing(m);
		if (changed) {
            kmScalar mat[] = {
				m->scaleX, -m->skew1, 0, 0,
				m->skew0, -m->scaleY, 0, 0,
                0, 0, 1, 0,
				m->translateX, -m->translateY, 0, 1};
            kmMat4Fill(&_transform, mat);
			setDirty(true);
		}

		cocos2d::LWFNode *node = (cocos2d::LWFNode *)getParent();
		const Color &c = cx->multi;
		const cocos2d::Color3B &dc = node->getDisplayedColor();
		setColor((cocos2d::Color3B){
			(GLubyte)(c.red * m_red * dc.r),
			(GLubyte)(c.green * m_green * dc.g),
			(GLubyte)(c.blue * m_blue * dc.b)});
		setOpacity((GLubyte)(c.alpha * node->getDisplayedOpacity()));
	}

	const kmMat4 &getNodeToParentTransform() const
	{
		return _transform;
	}
};

LWFTextTTFRenderer::LWFTextTTFRenderer(
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

	cocos2d::Size s = cocos2d::Size(t.width, t.height);

	m_label = LWFTextTTF::create(l->data->strings[t.stringId].c_str(),
		fontName, p.fontHeight, s, hAlignment, vAlignment,
		c.red, c.green, c.blue);

	if (!m_label)
		return;

	m_factory = (LWFRendererFactory *)l->rendererFactory.get();
	node->addChild(m_label);
}

LWFTextTTFRenderer::~LWFTextTTFRenderer()
{
}

void LWFTextTTFRenderer::Destruct()
{
	if (!m_label)
		return;

	cocos2d::LWFNode *node =
		dynamic_cast<cocos2d::LWFNode *>(m_label->getParent());
	if (node)
		node->remove(m_label);
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

	m_label->setVisible(visible);
	if (!visible)
		return;

	m_label->setZOrder(renderingIndex);
	m_label->setMatrixAndColorTransform(matrix, colorTransform);

	cocos2d::BlendFunc blendFunc = m_label->getBlendFunc();
	blendFunc.dst = (GLenum)(m_factory->GetBlendMode() ==
		Format::BLEND_MODE_ADD ? GL_ONE : GL_ONE_MINUS_SRC_ALPHA);
	m_label->setBlendFunc(blendFunc);
}

void LWFTextTTFRenderer::SetText(string text)
{
	if (!m_label)
		return;

	m_label->setString(text.c_str());
}

}   // namespace LWF
