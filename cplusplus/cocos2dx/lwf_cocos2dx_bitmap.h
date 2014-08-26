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

#ifndef LWF_COCOS2DX_BITMAP_H
#define LWF_COCOS2DX_BITMAP_H

#include "lwf_renderer.h"

NS_CC_BEGIN
class LWFNode;
NS_CC_END

namespace LWF {

class ILWFBitmap;
class LWFRendererFactory;

class LWFBitmapRenderer : public Renderer
{
protected:
	LWFRendererFactory *m_factory;
	cocos2d::Node *m_sprite;

public:
	LWFBitmapRenderer(LWF *l, Bitmap *bitmap, cocos2d::LWFNode *node);
	LWFBitmapRenderer(LWF *l, BitmapEx *bitmapEx, cocos2d::LWFNode *node);

	void Destruct();
	void Update(const Matrix *matrix, const ColorTransform *colorTransform);
	void Render(const Matrix *matrix, const ColorTransform *colorTransform,
		int renderingIndex, int renderingCount, bool visible);

	cocos2d::Node *GetNode() {return m_sprite;}
    
private:
    void createNodeBitmap(Movie * parent, const char *filename,
          const Format::Texture &t,
          const Format::TextureFragment &f,
          const Format::BitmapEx &bx);
};

}   // namespace LWF

#endif
