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

#import "LWFBitmapRenderer.h"
#import "LWFRendererFactory.h"
#import "LWFResourceCache.h"
#import "LWFView.h"
#import "lwf_bitmap.h"
#import "lwf_bitmapex.h"
#import "lwf_core.h"
#import "lwf_data.h"

using namespace std;

@interface LWFBitmapView : UIView
- (id)initWithContext:(LWF::LWFBitmapRendererContext *)context;
@end

@implementation LWFBitmapView
- (id)initWithContext:(LWF::LWFBitmapRendererContext *)context
{
 	self = [super init];
	self.backgroundColor = nil;
	self.hidden = YES;
	self.opaque = NO;
	self.layer.position = CGPointMake(0, 0);
	self.layer.anchorPoint = CGPointMake(0, 0);

	CALayer *layer = [CALayer layer];
	layer.position = CGPointMake(0, 0);
	layer.anchorPoint = CGPointMake(0, 0);
	layer.frame = context->frame;
	layer.contents = (id)context->uiImage.CGImage;
	layer.contentsRect = context->uvwh;
	CATransform3D t = CATransform3DIdentity;
	CGFloat x = context->position.x;
	CGFloat y = context->position.y;
	if (context->rotated) {
		t = CATransform3DTranslate(t, x, y + context->frame.size.width, 0);
		t = CATransform3DRotate(t, -M_PI / 2.0f, 0, 0, 1);
	} else {
		t = CATransform3DTranslate(t, x, y, 0);
	}
	layer.transform = t;
	[self.layer addSublayer:layer];

	return self;
}
@end

namespace LWF {

LWFBitmapRendererContext::LWFBitmapRendererContext(const Data *data,
	const Format::BitmapEx &bx, const string &path)
{
	const Format::TextureFragment &f =
		data->textureFragments[bx.textureFragmentId];
	const Format::Texture &t = data->textures[f.textureId];
	string filename = t.GetFilename(data);

	uiImage = LWFResourceCache::shared()->loadTexture(path, filename);
	if (!uiImage)
		return;

	float x = (float)f.x;
	float y = (float)f.y;
	float u = (float)f.u;
	float v = (float)f.v;
	float w = (float)f.w;
	float h = (float)f.h;

	float bu = bx.u * w;
	float bv = bx.v * h;
	float bw = bx.w;
	float bh = bx.h;

	x += bu;
	y += bv;
	u += bu;
	v += bv;
	w *= bw;
	h *= bh;

 	float scale = uiImage.size.width / t.width;
	position = CGPointMake(x, y);
	if (f.rotated == 0) {
		frame = CGRectMake(0, 0, w * scale, h * scale);
		uvwh = CGRectMake(u / t.width, v / t.height, w / t.width, h / t.height);
		rotated = false;
	} else {
		frame = CGRectMake(0, 0, h * scale, w * scale);
		uvwh = CGRectMake(u / t.width, v / t.height, h / t.width, w / t.height);
		rotated = true;
	}
}

LWFBitmapRendererContext::~LWFBitmapRendererContext()
{
	if (uiImage)
		LWFResourceCache::shared()->unloadTexture(uiImage);
}

LWFBitmapRenderer::LWFBitmapRenderer(
		LWFRendererFactory *factory, LWF *l, Bitmap *bitmap)
	: Renderer(l), m_view(nil)
{
	const LWFResourceCache::DataContext *dataContext =
		LWFResourceCache::shared()->getDataContext(l->data);
	int objId = bitmap->objectId;
	if (!dataContext || objId < 0 ||
			objId >= (int)dataContext->bitmapContexts.size() ||
			!dataContext->bitmapContexts[objId])
		return;

	m_view = [[LWFBitmapView alloc]
		initWithContext:dataContext->bitmapContexts[objId].get()];
	[factory->GetView() addSubview:m_view];
}

LWFBitmapRenderer::LWFBitmapRenderer(
		LWFRendererFactory *factory, LWF *l, BitmapEx *bitmapEx)
	: Renderer(l), m_view(nil)
{
	const LWFResourceCache::DataContext *dataContext =
		LWFResourceCache::shared()->getDataContext(l->data);
	int objId = bitmapEx->objectId;
	if (!dataContext || objId < 0 ||
			objId >= (int)dataContext->bitmapExContexts.size() ||
			!dataContext->bitmapExContexts[objId])
		return;

	m_view = [[LWFBitmapView alloc]
		initWithContext:dataContext->bitmapExContexts[objId].get()];
	[factory->GetView() addSubview:m_view];
}

void LWFBitmapRenderer::Destruct()
{
	[m_view removeFromSuperview];
	m_view = nil;
}

void LWFBitmapRenderer::Update(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
}

void LWFBitmapRenderer::Render(
	const Matrix *matrix, const ColorTransform *colorTransform,
	int renderingIndex, int renderingCount, bool visible)
{
	if (!m_view)
		return;

	if (!visible || colorTransform->multi.alpha == 0) {
		m_view.hidden = YES;
		return;
	}

	m_view.hidden = NO;
	m_view.alpha = colorTransform->multi.alpha;

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
	t.m32 = 0;
	t.m33 = 1;
	t.m34 = 0;

	t.m41 = m->translateX;
	t.m42 = m->translateY;
	t.m43 = renderingIndex;
	t.m44 = 1;

	m_view.layer.transform = t;
}

}   // namespace LWF
