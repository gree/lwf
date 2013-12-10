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

#import "lwf_core.h"
#import "lwf_data.h"
#import "lwf_text.h"
#import "LWFTextRenderer.h"
#import "LWFResourceCache.h"
#import "LWFRendererFactory.h"
#import "LWFView.h"

using namespace std;

namespace LWF {

LWFTextRenderer::LWFTextRenderer(
		LWFRendererFactory *factory, LWF *l, Text *text)
	: TextRenderer(l), m_factory(factory)
{
	const Format::Text &t = lwf->data->texts[text->objectId];
	const Format::TextProperty &p = lwf->data->textProperties[t.textPropertyId];
	const Format::Font &f = lwf->data->fonts[p.fontId];
	const Color &c = l->data->colors[t.colorId];
	string fontName = lwf->data->strings[f.stringId];

	UIFont *font = [UIFont fontWithName:[NSString
		stringWithUTF8String:fontName.c_str()] size:p.fontHeight];
	if (!font)
		font = [UIFont systemFontOfSize:p.fontHeight];

	m_label = [[UILabel alloc]
		initWithFrame:CGRectMake(0, 0, t.width, t.height)];
	m_label.hidden = YES;
	m_label.font = font;
	m_label.text =
		[NSString stringWithUTF8String:l->data->strings[t.stringId].c_str()];
	m_label.layer.position = CGPointMake(0, 0);
	m_label.layer.anchorPoint = CGPointMake(0, 0);
	m_label.textColor =
		[UIColor colorWithRed:c.red green:c.green blue:c.blue alpha:c.alpha];
	m_label.baselineAdjustment = UIBaselineAdjustmentNone;

	switch (p.align & Format::TextProperty::ALIGN_MASK) {
	default:
	case Format::TextProperty::LEFT:
	 	m_label.textAlignment = NSTextAlignmentLeft;
		break;
	case Format::TextProperty::RIGHT:
	 	m_label.textAlignment = NSTextAlignmentRight;
		break;
	case Format::TextProperty::CENTER:
	 	m_label.textAlignment = NSTextAlignmentCenter;
		break;
	}

	if (p.shadowColorId != -1) {
		const Color &sc = l->data->colors[p.shadowColorId];
		m_label.shadowColor = [UIColor
			colorWithRed:sc.red green:sc.green blue:sc.blue alpha:sc.alpha];
		m_label.shadowOffset = CGSizeMake(p.shadowOffsetX, p.shadowOffsetY);
		if (p.shadowBlur > 0) {
			m_label.layer.shadowRadius = p.shadowBlur;
			m_label.layer.shadowOpacity = 0.5;
		}
	}

	[factory->GetView() addSubview:m_label];
}

void LWFTextRenderer::Destruct()
{
	[m_label removeFromSuperview];
	m_label = nil;
}

void LWFTextRenderer::Update(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
}

void LWFTextRenderer::Render(
	const Matrix *matrix, const ColorTransform *colorTransform,
	int renderingIndex, int renderingCount, bool visible)
{
	if (!visible || colorTransform->multi.alpha == 0)
		return;

	m_label.hidden = NO;
	m_label.alpha = colorTransform->multi.alpha;

	const Matrix *&m = matrix;
	CATransform3D t;
	t.m11 = m->scaleX;
	t.m12 = m->skew1;
	t.m13 = 0;
	t.m14 = 0;

	t.m21 = m->skew0;
	t.m22 = m->scaleY;
	t.m23 = 0;
	t.m24 = 0;

	t.m31 = 0;
	t.m31 = 0;
	t.m31 = 1;
	t.m31 = 0;

	t.m41 = m->translateX;
	t.m42 = m->translateY;
	t.m43 = renderingIndex;
	t.m44 = 1;

	m_label.layer.transform = t;
}

void LWFTextRenderer::SetText(string text)
{
	m_label.text = [NSString stringWithUTF8String:text.c_str()];
}

}   // namespace LWF
