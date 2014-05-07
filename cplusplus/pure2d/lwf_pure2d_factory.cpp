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

#include <android/log.h>
#include <GLES/gl.h>
#include "lwf_core.h"
#include "lwf_movie.h"
#include "lwf_property.h"
#include "lwf_pure2d_bitmap.h"
#include "lwf_pure2d_factory.h"

#define LOG_TAG "pure2d::LWF"
#define LOG(...) __android_log_print(ANDROID_LOG_INFO,LOG_TAG,__VA_ARGS__)

namespace LWF {

shared_ptr<Renderer> Pure2DRendererFactory::ConstructBitmap(
	LWF *lwf, int objId, Bitmap *bitmap)
{
	return make_shared<Pure2DRendererBitmapRenderer>(this, lwf, bitmap);
}

shared_ptr<Renderer> Pure2DRendererFactory::ConstructBitmapEx(
	LWF *lwf, int objId, BitmapEx *bitmapEx)
{
	return make_shared<Pure2DRendererBitmapRenderer>(this, lwf, bitmapEx);
}

shared_ptr<TextRenderer> Pure2DRendererFactory::ConstructText(
	LWF *lwf, int objId, Text *text)
{
	return shared_ptr<TextRenderer>();
}

shared_ptr<Renderer> Pure2DRendererFactory::ConstructParticle(
	LWF *lwf, int objId, Particle *particle)
{
	return shared_ptr<Renderer>();
}

void Pure2DRendererFactory::Init(LWF *lwf)
{
	m_lwf = lwf;
	m_bufferGenerated = false;
	m_updateCount = -1;
}

void Pure2DRendererFactory::BeginRender(LWF *lwf)
{
	if (!m_bufferGenerated) {
		GLuint buffers[2];
		glGenBuffers(2, buffers);
		m_vertexBuffer = buffers[0];
		m_indicesBuffer = buffers[1];
		m_bufferGenerated = true;
	}

	if (m_updateCount != lwf->updateCount) {
		m_updateCount = lwf->updateCount;
		m_updated = true;
	} else {
		m_updated = false;
	}

	if (m_lwf->parent)
		return;

	if (m_updated)
		m_buffers.clear();
}

void Pure2DRendererFactory::EndRender(LWF *lwf)
{
	if (m_lwf->parent)
		return;

	glEnable(GL_TEXTURE_2D);

	vector<Buffer>::iterator it(m_buffers.begin()), itend(m_buffers.end());
	for (; it != itend; ++it) {
		glBindTexture(GL_TEXTURE_2D, it->glTextureId);
		glBlendFunc(it->preMultipliedAlpha ?
			GL_ONE : GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		size_t size = sizeof(Vertex) * 4 * it->index;
		glBindBuffer(GL_ARRAY_BUFFER, m_vertexBuffer);
		glBufferData(GL_ARRAY_BUFFER, size, &it->vertices[0], GL_DYNAMIC_DRAW);

		glEnableClientState(GL_VERTEX_ARRAY);
		glEnableClientState(GL_TEXTURE_COORD_ARRAY);
		glEnableClientState(GL_COLOR_ARRAY);

		glVertexPointer(3, GL_FLOAT, sizeof(Vertex), (GLvoid *)0);
		glTexCoordPointer(2, GL_FLOAT, sizeof(Vertex), (GLvoid *)12);
		glColorPointer(4, GL_UNSIGNED_BYTE, sizeof(Vertex), (GLvoid *)20);

		size = sizeof(GLushort) * 6 * it->index;
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_indicesBuffer);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER,
			size, &it->indices[0], GL_DYNAMIC_DRAW);

		glDrawElements(GL_TRIANGLES,
			(GLsizei)it->index * 6, GL_UNSIGNED_SHORT, 0);
	}

	glDisableClientState(GL_VERTEX_ARRAY);
	glDisableClientState(GL_TEXTURE_COORD_ARRAY);
	glDisableClientState(GL_COLOR_ARRAY);

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
}

void Pure2DRendererFactory::Destruct()
{
	if (!m_bufferGenerated)
		return;

	GLuint buffers[] = {m_vertexBuffer, m_indicesBuffer};
	glDeleteBuffers(2, buffers);
}

void Pure2DRendererFactory::FitForHeight(LWF *lwf, float w, float h)
{
	ScaleForHeight(lwf, w, h);
	float offsetX = (w - lwf->width * lwf->scaleByStage) / 2.0f;
	float offsetY = -h;
	lwf->property->Move(offsetX, offsetY);
}

void Pure2DRendererFactory::FitForWidth(LWF *lwf, float w, float h)
{
	ScaleForWidth(lwf, w, h);
	float offsetX = (w - lwf->width * lwf->scaleByStage) / 2.0f;
	float offsetY = -h;
	lwf->property->Move(offsetX, offsetY);
}

void Pure2DRendererFactory::ScaleForHeight(LWF *lwf, float w, float h)
{
	float scale = h / lwf->height;
	lwf->scaleByStage = scale;
	lwf->property->Scale(scale, scale);
}

void Pure2DRendererFactory::ScaleForWidth(LWF *lwf, float w, float h)
{
	float scale = w / lwf->width;
	lwf->scaleByStage = scale;
	lwf->property->Scale(scale, scale);
}

int Pure2DRendererFactory::GetBufferIndex(Pure2DRendererBitmapContext *context)
{
	if (m_lwf->parent) {
		Pure2DRendererFactory *parent =
			(Pure2DRendererFactory *)m_lwf->parent->lwf->rendererFactory.get();
		return parent->GetBufferIndex(context);
	}

	if (m_buffers.empty() ||
			m_buffers.back().glTextureId != context->GetGLTextureId()) {
		Buffer buffer(context->GetGLTextureId(),
			context->IsPreMultipliedAlpha(), context->GetHeight());
		m_buffers.push_back(buffer);
	}

	return m_buffers.back().index++;
}

void Pure2DRendererFactory::SetVertex(int offset, float x, float y,
	float z, float u, float v, float r, float g, float b, float a)
{
	if (m_lwf->parent) {
		Pure2DRendererFactory *parent =
			(Pure2DRendererFactory *)m_lwf->parent->lwf->rendererFactory.get();
		parent->SetVertex(offset, x, y, z, u, v, r, g, b, a);
		return;
	}

	vector<Vertex> &vertices = m_buffers.back().vertices;
	vertices.resize(offset + 1);
	vertices[offset] =
		Vertex(x, y, z, u, v, 255 * r, 255 * g, 255 * b, 255 * a);
}

void Pure2DRendererFactory::SetIndex(int offset, unsigned short index)
{
	if (m_lwf->parent) {
		Pure2DRendererFactory *parent =
			(Pure2DRendererFactory *)m_lwf->parent->lwf->rendererFactory.get();
		parent->SetIndex(offset, index);
		return;
	}

	vector<unsigned short> &indices = m_buffers.back().indices;
	indices.resize(offset + 1);
	indices[offset] = index;
}

}	// namespace LWF
