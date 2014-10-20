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

#import "LWFRendererFactory.h"
#import "LWFBitmapRenderer.h"
//#import "LWFTextRenderer.h"
#import "LWFResourceCache.h"
#import "LWFShader.h"
#import "lwf_core.h"
#import "lwf_movie.h"
#import "lwf_property.h"

namespace LWF {

shared_ptr<Renderer> LWFRendererFactory::ConstructBitmap(
	LWF *lwf, int objId, Bitmap *bitmap)
{
	return make_shared<LWFBitmapRenderer>(this, lwf, bitmap);
}

shared_ptr<Renderer> LWFRendererFactory::ConstructBitmapEx(
	LWF *lwf, int objId, BitmapEx *bitmapEx)
{
	return make_shared<LWFBitmapRenderer>(this, lwf, bitmapEx);
}

shared_ptr<TextRenderer> LWFRendererFactory::ConstructText(
	LWF *lwf, int objId, Text *text)
{
	//return make_shared<LWFTextRenderer>(this, lwf, text);
    return 0;
}

shared_ptr<Renderer> LWFRendererFactory::ConstructParticle(
	LWF *lwf, int objId, Particle *particle)
{
	return shared_ptr<Renderer>();
}

void LWFRendererFactory::Init(LWF *lwf)
{
    m_lwf = lwf;
	m_blendMode = Format::BLEND_MODE_NORMAL;
	m_maskMode = Format::BLEND_MODE_NORMAL;
}

void LWFRendererFactory::InitGL()
{
	m_initialized = true;

    glGenVertexArraysOES(1, &m_vertexArray);

    GLuint buffers[2];
    glGenBuffers(2, buffers);
    m_vertexBuffer = buffers[0];
    m_indicesBuffer = buffers[1];

    glBindVertexArrayOES(m_vertexArray);
    glBindBuffer(GL_ARRAY_BUFFER, m_vertexBuffer);

    glBindVertexArrayOES(0);
}

void LWFRendererFactory::BeginRender(LWF *lwf)
{
	if (m_lwf->parent)
		return;

	m_updated = false;
}

void LWFRendererFactory::EndRender(LWF *lwf)
{
    if (m_lwf->parent)
        return;

	if (!m_initialized)
		InitGL();
    
    LWFShader *shader = LWFShader::shared();
	shader->load();

	float width = lwf->scaleByStage * lwf->width;
	float height = lwf->scaleByStage * lwf->height;

	float right = width;
	float left = 0;
	float top = 0;
	float bottom = height;
	float far = 1;
	float near = -1;
	GLKMatrix4 projection = GLKMatrix4Make(
		2.0f / (right - left), 0, 0, 0,
		0, 2.0f / (top - bottom), 0, 0,
		0, 0, -2.0f / (far - near), 0,
		-(right + left) / (right - left),
			-(top + bottom) / (top - bottom), -(far + near) / (far - near), 1
	);
    shader->setProjectionMatrix(projection);
    shader->setModelMatrix(GLKMatrix4Identity);
	glViewport(0, 0, width, height);

    glBindVertexArrayOES(m_vertexArray);
    glBindBuffer(GL_ARRAY_BUFFER, m_vertexBuffer);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_indicesBuffer);

    glVertexAttribPointer(shader->positionAttribute,
		3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (GLvoid *)0);
    glVertexAttribPointer(shader->texCoordAttribute,
		2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (GLvoid *)12);
    glVertexAttribPointer(shader->colorAttribute,
		4, GL_UNSIGNED_BYTE, GL_TRUE, sizeof(Vertex), (GLvoid *)20);

    glEnableVertexAttribArray(shader->positionAttribute);
    glEnableVertexAttribArray(shader->texCoordAttribute);
    glEnableVertexAttribArray(shader->colorAttribute);

    vector<Mesh>::iterator it(m_meshes.begin()), itend(m_meshes.end());
    for (; it != itend; ++it)
    {
		glBindTexture(GL_TEXTURE_2D, it->texture->textureId);
		glBlendFunc((GLenum)(it->preMultipliedAlpha ? GL_ONE : GL_SRC_ALPHA),
			(GLenum)(it->blendMode == Format::BLEND_MODE_ADD ?
				GL_ONE : GL_ONE_MINUS_SRC_ALPHA));

        size_t size = it->vertices.size() * sizeof(Vertex);
        glBufferData(GL_ARRAY_BUFFER, size, &it->vertices[0], GL_DYNAMIC_DRAW);
        
        size = it->indices.size() * sizeof(GLushort);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER,
			size, &it->indices[0], GL_DYNAMIC_DRAW);

        glDrawElements(GL_TRIANGLES,
			(GLsizei)it->indices.size(), GL_UNSIGNED_SHORT, 0);
    }

    glBindVertexArrayOES(0);
}

void LWFRendererFactory::Destruct()
{
	GLuint buffers[] = {m_vertexBuffer, m_indicesBuffer};
	glDeleteBuffers(2, buffers);
	glDeleteVertexArraysOES(1, &m_vertexArray);
}

void LWFRendererFactory::FitForHeight(class LWF *lwf, float w, float h)
{
	ScaleForHeight(lwf, w, h);
	float offsetX = (w - lwf->width * lwf->scaleByStage) / 2.0f;
	float offsetY = 0;
	lwf->property->MoveTo(offsetX, offsetY);
}

void LWFRendererFactory::FitForWidth(class LWF *lwf, float w, float h)
{
	ScaleForWidth(lwf, w, h);
	float offsetX = 0;
	float offsetY = (h - lwf->height * lwf->scaleByStage) / 2.0f;
	lwf->property->MoveTo(offsetX, offsetY);
}

void LWFRendererFactory::ScaleForHeight(class LWF *lwf, float w, float h)
{
	float scale = h / lwf->height;
	lwf->scaleByStage = scale;
	lwf->property->ScaleTo(scale, scale);
}

void LWFRendererFactory::ScaleForWidth(class LWF *lwf, float w, float h)
{
	float scale = w / lwf->width;
	lwf->scaleByStage = scale;
	lwf->property->ScaleTo(scale, scale);
}

void LWFRendererFactory::AddMesh(
	LWFBitmapRendererContext *context, int blendMode)
{
	m_meshes.resize(m_meshes.size() + 1);
	Mesh &mesh = m_meshes.back();
	mesh.texture = context->GetTexture();
	mesh.preMultipliedAlpha = context->IsPreMultipliedAlpha();
	mesh.blendMode = blendMode;
}

void LWFRendererFactory::PushVertices(
	LWFBitmapRendererContext *context, const Vertex *vertices)
{
	if (m_lwf->parent) {
		LWFRendererFactory *parent =
			(LWFRendererFactory *)m_lwf->parent->lwf->rendererFactory.get();
		parent->PushVertices(context, vertices);
		return;
	}

	if (!m_updated) {
		m_meshes.clear();
		AddMesh(context, m_blendMode);
		m_updated = true;
	}

	if (m_meshes.back().texture != context->GetTexture() ||
			m_meshes.back().blendMode != m_blendMode)
		AddMesh(context, m_blendMode);

	Mesh &mesh = m_meshes.back();

	size_t offset = mesh.vertices.size();
	mesh.vertices.resize(offset + 4);
	for (int i = 0; i < 4; ++i)
		mesh.vertices[offset + i] = vertices[i];

	offset = mesh.indices.size();
	unsigned short indexOffset = offset / 6 * 4;
	mesh.indices.resize(offset + 6);
	mesh.indices[offset + 0] = indexOffset + 0;
	mesh.indices[offset + 1] = indexOffset + 1;
	mesh.indices[offset + 2] = indexOffset + 2;
	mesh.indices[offset + 3] = indexOffset + 2;
	mesh.indices[offset + 4] = indexOffset + 1;
	mesh.indices[offset + 5] = indexOffset + 3;
}

}	// namespace LWF
