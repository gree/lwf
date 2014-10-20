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

#import "lwf_renderer.h"

namespace LWF {

class LWFRendererFactory;
class LWFTexture;

class LWFBitmapRendererContext
{
public:
	struct Vector2 {
		GLfloat x;
		GLfloat y;

		Vector2() {}
		Vector2(GLfloat ax, GLfloat ay) : x(ax), y(ay) {}
	};

protected:
	const LWFTexture *m_texture;
	Vector2 m_vertices[4];
	Vector2 m_coordinates[4];
	bool m_preMultipliedAlpha;
	float m_height;

public:
	LWFBitmapRendererContext() : m_texture(0) {}
	LWFBitmapRendererContext(const Data *data, const Format::BitmapEx &bx,
		const std::string &path, EAGLContext *context);
	~LWFBitmapRendererContext();
	bool IsPreMultipliedAlpha() const {return m_preMultipliedAlpha;}
	float GetHeight() const {return m_height;}
	const LWFTexture *GetTexture() const {return m_texture;}
	const Vector2 *GetVertices() const {return m_vertices;}
	const Vector2 *GetCoordinates() const {return m_coordinates;}
};

class LWFBitmapRenderer : public Renderer
{
protected:
	LWFRendererFactory *m_factory;
	LWFBitmapRendererContext *m_context;
	int m_updateCount;
	bool m_added;

public:
	LWFBitmapRenderer(LWFRendererFactory *factory, LWF *l, Bitmap *bitmap);
	LWFBitmapRenderer(LWFRendererFactory *factory, LWF *l, BitmapEx *bitmapEx);

	void Destruct();
	void Update(const Matrix *matrix, const ColorTransform *colorTransform);
	void Render(const Matrix *matrix, const ColorTransform *colorTransform,
		int renderingIndex, int renderingCount, bool visible);
};

}   // namespace LWF
