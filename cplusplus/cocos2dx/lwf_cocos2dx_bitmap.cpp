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
#include "lwf_bitmap.h"
#include "lwf_bitmapex.h"
#include "lwf_cocos2dx_bitmap.h"
#include "lwf_cocos2dx_factory.h"
#include "lwf_cocos2dx_node.h"
#include "lwf_core.h"
#include "lwf_data.h"

namespace LWF {

class LWFBitmap : public cocos2d::Sprite
{
protected:
	Matrix m_matrix;
	cocos2d::V3F_C4B_T2F_Quad m_quad;
	cocos2d::BlendFunc m_baseBlendFunc;

public:
	static LWFBitmap *create(const char *filename,
			const Format::Texture &texture,
			const Format::TextureFragment &fragment,
			const Format::BitmapEx &bitmapEx) {
		LWFBitmap *bitmap = new LWFBitmap();
		if (bitmap && bitmap->initWithFileEx(
				filename, texture, fragment, bitmapEx)) {
			bitmap->autorelease();
			return bitmap;
		}
		CC_SAFE_DELETE(bitmap);
		return NULL;
	}

	bool initWithFileEx(const char *filename,
		const Format::Texture &t,
		const Format::TextureFragment &f,
		const Format::BitmapEx &bx)
	{
		if (!cocos2d::Sprite::initWithFile(filename))
			return false;

		bool hasPremultipliedAlpha = getTexture()->hasPremultipliedAlpha() ||
			t.format == Format::TEXTUREFORMAT_PREMULTIPLIEDALPHA;
		m_baseBlendFunc = {(GLenum)(hasPremultipliedAlpha ?
			GL_ONE : GL_SRC_ALPHA), GL_ONE_MINUS_SRC_ALPHA};

		float tw = (float)t.width;
		float th = (float)t.height;

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

		float x0 = x / t.scale;
		float y0 = y / t.scale;
		float x1 = (x + w) / t.scale;
		float y1 = (y + h) / t.scale;

		m_quad.bl.vertices = cocos2d::Vec3(x1, y1, 0);
		m_quad.br.vertices = cocos2d::Vec3(x0, y1, 0);
		m_quad.tl.vertices = cocos2d::Vec3(x1, y0, 0);
		m_quad.tr.vertices = cocos2d::Vec3(x0, y0, 0);

		if (f.rotated == 0) {
			float u0 = u / tw;
			float v0 = v / th;
			float u1 = (u + w) / tw;
			float v1 = (v + h) / th;
			m_quad.bl.texCoords.u = u1;
			m_quad.bl.texCoords.v = v1;
			m_quad.br.texCoords.u = u0;
			m_quad.br.texCoords.v = v1;
			m_quad.tl.texCoords.u = u1;
			m_quad.tl.texCoords.v = v0;
			m_quad.tr.texCoords.u = u0;
			m_quad.tr.texCoords.v = v0;
		} else {
			float u0 = u / tw;
			float v0 = v / th;
			float u1 = (u + h) / tw;
			float v1 = (v + w) / th;
			m_quad.bl.texCoords.u = u0;
			m_quad.bl.texCoords.v = v1;
			m_quad.br.texCoords.u = u0;
			m_quad.br.texCoords.v = v0;
			m_quad.tl.texCoords.u = u1;
			m_quad.tl.texCoords.v = v1;
			m_quad.tr.texCoords.u = u1;
			m_quad.tr.texCoords.v = v0;
		}

		_quad = m_quad;

		return true;
	}

	virtual void setVisible(bool bVisible) override
	{
		if (bVisible && !isVisible())
			m_matrix.Invalidate();
		cocos2d::Sprite::setVisible(bVisible);
	}

	void setMatrixAndColorTransform(const Matrix *m, const ColorTransform *cx)
	{
		bool changed = m_matrix.SetWithComparing(m);
		if (changed) {
            cocos2d::Mat4 mat = cocos2d::Mat4(
				m->scaleX, m->skew0, 0, m->translateX,
				-m->skew1, -m->scaleY, 0, -m->translateY,
				0, 0, 1, 0,
				0, 0, 0, 1);
			setNodeToParentTransform(mat);
		}

		cocos2d::Node *node = getParent();
		const Color &c = cx->multi;
		const cocos2d::Color3B &dc = node->getDisplayedColor();
		setColor((cocos2d::Color3B){
			(GLubyte)(c.red * dc.r),
			(GLubyte)(c.green * dc.g),
			(GLubyte)(c.blue * dc.b)});
		setOpacity((GLubyte)(c.alpha * node->getDisplayedOpacity()));
	}

	virtual void setBatchNode(
		cocos2d::SpriteBatchNode *spriteBatchNode) override
	{
		if (spriteBatchNode) {
			float x0 = m_quad.tr.vertices.x;
			float y0 = m_quad.tr.vertices.y;
			float x1 = m_quad.bl.vertices.x;
			float y1 = m_quad.bl.vertices.y;
			_offsetPosition.x = x0;
			_offsetPosition.y = y0;
			_rect = cocos2d::Rect(0, 0, x1 - x0, y1 - y0);

			_quad.bl.texCoords.u = m_quad.tr.texCoords.u;
			_quad.bl.texCoords.v = m_quad.tr.texCoords.v;
			_quad.br.texCoords.u = m_quad.tl.texCoords.u;
			_quad.br.texCoords.v = m_quad.tl.texCoords.v;
			_quad.tl.texCoords.u = m_quad.br.texCoords.u;
			_quad.tl.texCoords.v = m_quad.br.texCoords.v;
			_quad.tr.texCoords.u = m_quad.bl.texCoords.u;
			_quad.tr.texCoords.v = m_quad.bl.texCoords.v;
		}

		Sprite::setBatchNode(spriteBatchNode);
	}

	const cocos2d::BlendFunc &getBaseBlendFunc() const
	{
		return m_baseBlendFunc;
	}
};

LWFBitmapRenderer::LWFBitmapRenderer(
		LWF *l, Bitmap *bitmap, cocos2d::LWFNode *node)
	: Renderer(l), m_sprite(0)
{
	const Format::Bitmap &b = l->data->bitmaps[bitmap->objectId];
	if (b.textureFragmentId == -1)
		return;

	Format::BitmapEx bx;
	bx.matrixId = b.matrixId;
	bx.textureFragmentId = b.textureFragmentId;
	bx.u = 0;
	bx.v = 0;
	bx.w = 1;
	bx.h = 1;

	const Format::TextureFragment &f =
		l->data->textureFragments[b.textureFragmentId];
	const Format::Texture &t = l->data->textures[f.textureId];
	string texturePath = t.GetFilename(l->data.get());
	string filename = node->basePath + texturePath;

	if (LWF::GetTextureLoadHandler())
		filename = LWF::GetTextureLoadHandler()(
			filename, node->basePath, texturePath);

	m_sprite = LWFBitmap::create(filename.c_str(), t, f, bx);
	if (!m_sprite)
		return;

	l->data->resourceCache[filename] = true;
	m_factory = (LWFRendererFactory *)l->rendererFactory.get();
	node->addChild(m_sprite);
}

LWFBitmapRenderer::LWFBitmapRenderer(
		LWF *l, BitmapEx *bitmapEx, cocos2d::LWFNode *node)
	: Renderer(l), m_sprite(0)
{
	const Format::BitmapEx &bx = l->data->bitmapExs[bitmapEx->objectId];
	if (bx.textureFragmentId == -1)
		return;

	const Format::TextureFragment &f =
		l->data->textureFragments[bx.textureFragmentId];
	const Format::Texture &t = l->data->textures[f.textureId];
	string texturePath = t.GetFilename(l->data.get());
	string filename = node->basePath + texturePath;

	if (LWF::GetTextureLoadHandler())
		filename = LWF::GetTextureLoadHandler()(
			filename, node->basePath, texturePath);

	m_sprite = LWFBitmap::create(filename.c_str(), t, f, bx);
	if (!m_sprite)
		return;

	m_factory = (LWFRendererFactory *)l->rendererFactory.get();
	node->addChild(m_sprite);
}

void LWFBitmapRenderer::Destruct()
{
	if (!m_sprite)
		return;

	cocos2d::LWFNode::removeFromParent(m_sprite);
	m_sprite = 0;
}

void LWFBitmapRenderer::Update(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
}

void LWFBitmapRenderer::Render(
	const Matrix *matrix, const ColorTransform *colorTransform,
	int renderingIndex, int renderingCount, bool visible)
{
	if (!m_sprite)
		return;

    cocos2d::BlendFunc baseBlendFunc = m_sprite->getBaseBlendFunc();
	if (!m_factory->Render(
			lwf, m_sprite, renderingIndex, visible, &baseBlendFunc))
		return;

	m_sprite->setMatrixAndColorTransform(matrix, colorTransform);
}

}   // namespace LWF
