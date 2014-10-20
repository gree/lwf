/*
 * Copyright (C) 2014 GREE, Inc.
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

#import "lwf_bitmap.h"
#import "lwf_bitmapex.h"
#import "lwf_core.h"
#import "lwf_data.h"
#import "LWFBitmapRenderer.h"
#import "LWFResourceCache.h"
#import "LWFRendererFactory.h"

using namespace std;

namespace LWF {

LWFBitmapRendererContext::LWFBitmapRendererContext(const Data *data,
	const Format::BitmapEx &bx, const string &path, EAGLContext *context)
{
	const Format::TextureFragment &f =
		data->textureFragments[bx.textureFragmentId];
	const Format::Texture &t = data->textures[f.textureId];
	string filename = t.GetFilename(data);

	m_texture =
		LWFResourceCache::shared()->loadTexture(path, filename, context);
	if (!m_texture)
		return;

	m_preMultipliedAlpha = YES;
	// always uses pre-multiplied-alpha-textures
	// t.format == Format::TEXTUREFORMAT_PREMULTIPLIEDALPHA;

	float tw = m_texture->width;
	float th = m_texture->height;
	
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
	
	m_height = h / t.scale;
	
	float x0 = x / t.scale;
	float y0 = y / t.scale;
	float x1 = (x + w) / t.scale;
	float y1 = (y + h) / t.scale;

	m_vertices[0] = Vector2(x1, y1);
	m_vertices[1] = Vector2(x1, y0);
	m_vertices[2] = Vector2(x0, y1);
	m_vertices[3] = Vector2(x0, y0);

	if (f.rotated == 0) {
		float u0 = u / tw;
		float v0 = v / th;
		float u1 = (u + w) / tw;
		float v1 = (v + h) / th;
		m_coordinates[0] = Vector2(u1, v1);
		m_coordinates[1] = Vector2(u1, v0);
		m_coordinates[2] = Vector2(u0, v1);
		m_coordinates[3] = Vector2(u0, v0);
	} else {
		float u0 = u / tw;
		float v0 = v / th;
		float u1 = (u + h) / tw;
		float v1 = (v + w) / th;
		m_coordinates[0] = Vector2(u0, v1);
		m_coordinates[1] = Vector2(u1, v1);
		m_coordinates[2] = Vector2(u0, v0);
		m_coordinates[3] = Vector2(u1, v0);
	}
}

LWFBitmapRendererContext::~LWFBitmapRendererContext()
{
	if (!m_texture)
		return;

	LWFResourceCache::shared()->unloadTexture(m_texture);
}

LWFBitmapRenderer::LWFBitmapRenderer(
		LWFRendererFactory *factory, LWF *l, Bitmap *bitmap)
	: Renderer(l), m_factory(factory), m_context(0)
{
	const LWFResourceCache::DataContext *dataContext =
		LWFResourceCache::shared()->getDataContext(l->data);
	int objId = bitmap->objectId;
	if (!dataContext || objId < 0 ||
			objId >= (int)dataContext->bitmapContexts.size() ||
			!dataContext->bitmapContexts[objId])
		return;

	m_context = dataContext->bitmapContexts[objId].get();
	m_updateCount = -1;
}

LWFBitmapRenderer::LWFBitmapRenderer(
		LWFRendererFactory *factory, LWF *l, BitmapEx *bitmapEx)
	: Renderer(l), m_factory(factory), m_context(0)
{
	const LWFResourceCache::DataContext *dataContext =
		LWFResourceCache::shared()->getDataContext(l->data);
	int objId = bitmapEx->objectId;
	if (!dataContext || objId < 0 ||
			objId >= (int)dataContext->bitmapExContexts.size() ||
			!dataContext->bitmapExContexts[objId])
		return;

	m_context = dataContext->bitmapExContexts[objId].get();
	m_updateCount = -1;
}

void LWFBitmapRenderer::Destruct()
{
}

void LWFBitmapRenderer::Update(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
}

void LWFBitmapRenderer::Render(
	const Matrix *matrix, const ColorTransform *colorTransform,
	int renderingIndex, int renderingCount, bool visible)
{
	if (!m_context || lwf->updateCount == m_updateCount ||
			!visible || colorTransform->multi.alpha == 0)
		return;

	m_updateCount = lwf->updateCount;

	float red = colorTransform->multi.red;
	float green = colorTransform->multi.green;
	float blue = colorTransform->multi.blue;
	float alpha = colorTransform->multi.alpha;

	if (m_context->IsPreMultipliedAlpha()) {
		red *= alpha;
		green *= alpha;
		blue *= alpha;
	}

	const Matrix *&m = matrix;
	float scaleX = m->scaleX;
	float skew0 = m->skew0;
	float translateX = m->translateX;

	float skew1 = m->skew1;
	float scaleY = m->scaleY;
	float translateY = m->translateY;

	float translateZ = 0;//renderingCount - renderingIndex;

	const LWFBitmapRendererContext::Vector2 *vertices =
		m_context->GetVertices();
	const LWFBitmapRendererContext::Vector2 *coordinates =
		m_context->GetCoordinates();
	LWFRendererFactory::Vertex v[4];
	for (int i = 0; i < 4; ++i) {
		GLfloat x = vertices[i].x;
		GLfloat y = vertices[i].y;

		GLfloat px = x * scaleX + y * skew0 + translateX;
		GLfloat py = x * skew1 + y * scaleY + translateY;
		GLfloat pz = translateZ;

		v[i] = LWFRendererFactory::Vertex(
			px, py, pz, coordinates[i].x, coordinates[i].y,
			(unsigned char)(255 * red),
			(unsigned char)(255 * green),
			(unsigned char)(255 * blue),
			(unsigned char)(255 * alpha));
	}

	m_factory->PushVertices(m_context, v);
}

}   // namespace LWF
