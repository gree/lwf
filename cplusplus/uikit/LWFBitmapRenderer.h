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

#import <UIKit/UIKit.h>
#import "lwf_renderer.h"

namespace LWF {

class LWFRendererFactory;

class LWFBitmapRendererContext
{
protected:
	UIImage *m_image;
	float m_x;
	float m_y;

public:
	LWFBitmapRendererContext() : m_image(0) {}
	LWFBitmapRendererContext(const Data *data,
		const Format::BitmapEx &bx, const std::string &path);
	~LWFBitmapRendererContext();
	UIImage *GetImage() {return m_image;}
	float GetX() const {return m_x;}
	float GetY() const {return m_y;}
};

class LWFBitmapRenderer : public Renderer
{
protected:
	LWFBitmapRendererContext *m_context;
	LWFRendererFactory *m_factory;
	UIImageView *m_view;
	UIView *m_wrapper;
	CALayer *m_layer;

public:
	LWFBitmapRenderer(LWFRendererFactory *factory, LWF *l, Bitmap *bitmap);
	LWFBitmapRenderer(LWFRendererFactory *factory, LWF *l, BitmapEx *bitmapEx);

	void Destruct();
	void Update(const Matrix *matrix, const ColorTransform *colorTransform);
	void Render(const Matrix *matrix, const ColorTransform *colorTransform,
		int renderingIndex, int renderingCount, bool visible);
};

}   // namespace LWF
