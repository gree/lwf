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

namespace LWF {

LWFBitmapRendererContext::LWFBitmapRendererContext(const Data *data,
	const Format::BitmapEx &bx, const string &path)
{
	const Format::TextureFragment &f =
		data->textureFragments[bx.textureFragmentId];
	const Format::Texture &t = data->textures[f.textureId];
	string filename = t.GetFilename(data);

	UIImage *image = LWFResourceCache::shared()->loadTexture(path, filename);
	if (!image)
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
	
 	float scale = image.size.width / t.width;
	float tu;
	float tv;
	float tw;
	float th;
	UIImageOrientation orientation;
	if (f.rotated == 0) {
		tu = u * scale;
		tv = v * scale;
		tw = w * scale;
		th = h * scale;
		orientation = UIImageOrientationUp;
	} else {
		tu = u * scale;
		tv = v * scale;
		tw = h * scale;
		th = w * scale;
		orientation = UIImageOrientationLeft;
	}

	if (tu == 0 && tv == 0 && tw == image.size.width &&
			th == image.size.height && f.rotated == 0) {
		m_image = image;
	} else {
		CGImageRef fragment = CGImageCreateWithImageInRect(
			[image CGImage], CGRectMake(tu, tv, tw, th));
		m_image = [UIImage
			imageWithCGImage:fragment scale:scale orientation:orientation];
		CGImageRelease(fragment);
	}

	m_x = x;
	m_y = y;
}

LWFBitmapRendererContext::~LWFBitmapRendererContext()
{
}

LWFBitmapRenderer::LWFBitmapRenderer(
		LWFRendererFactory *factory, LWF *l, Bitmap *bitmap)
	: Renderer(l), m_factory(factory), m_context(0), m_view(nil)
{
	const LWFResourceCache::DataContext *dataContext =
		LWFResourceCache::shared()->getDataContext(l->data);
	int objId = bitmap->objectId;
	if (!dataContext || objId < 0 ||
			objId >= (int)dataContext->bitmapContexts.size() ||
			!dataContext->bitmapContexts[objId])
		return;

	m_context = dataContext->bitmapContexts[objId].get();

	m_view = [[UIImageView alloc] initWithImage:m_context->GetImage()];
	m_view.hidden = YES;

	float x = m_context->GetX();
	float y = m_context->GetY();
	if (x != 0 || y != 0) {
		UIView *view = [[UIView alloc] init];
		[view addSubview:m_view];
		m_view.frame = CGRectMake(
			x, y, m_view.frame.size.width, m_view.frame.size.height);
		[factory->GetView() addSubview:view];
		m_layer = view.layer;
	} else {
		m_layer = m_view.layer;
		[factory->GetView() addSubview:m_view];
	}
	m_layer.position = CGPointMake(0, 0);
	m_layer.anchorPoint = CGPointMake(0, 0);
}

LWFBitmapRenderer::LWFBitmapRenderer(
		LWFRendererFactory *factory, LWF *l, BitmapEx *bitmapEx)
	: Renderer(l), m_factory(factory), m_context(0), m_view(nil)
{
	const LWFResourceCache::DataContext *dataContext =
		LWFResourceCache::shared()->getDataContext(l->data);
	int objId = bitmapEx->objectId;
	if (!dataContext || objId < 0 ||
			objId >= (int)dataContext->bitmapExContexts.size() ||
			!dataContext->bitmapExContexts[objId])
		return;

	m_context = dataContext->bitmapExContexts[objId].get();
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
	if (!m_context || !visible || colorTransform->multi.alpha == 0)
		return;

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

	m_layer.transform = t;
}

}   // namespace LWF
