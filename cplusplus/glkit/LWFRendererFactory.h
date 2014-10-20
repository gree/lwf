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
#import "lwf_type.h"

namespace LWF {

class LWFBitmapRendererContext;
class LWFTexture;

class LWFRendererFactory : public IRendererFactory
{
public:
	struct Vertex {
		GLfloat x;
		GLfloat y;
		GLfloat z;
		GLfloat u;
		GLfloat v;
		GLubyte r;
		GLubyte g;
		GLubyte b;
		GLubyte a;

		Vertex() {}
		Vertex(GLfloat ax, GLfloat ay, GLfloat az, GLfloat au, GLfloat av,
				GLubyte ar, GLubyte ag, GLubyte ab, GLubyte aa)
			: x(ax), y(ay), z(az), u(au), v(av), r(ar), g(ag), b(ab), a(aa) {}
	};

	struct Mesh {
		const LWFTexture *texture;
		bool preMultipliedAlpha;
		int blendMode;
		vector<Vertex> vertices;
		vector<GLushort> indices;
	};

protected:
	LWF *m_lwf;
	std::string m_path;
	std::vector<Mesh> m_meshes;
	GLuint m_vertexArray;
	GLuint m_vertexBuffer;
	GLuint m_indicesBuffer;
	int m_blendMode;
	int m_maskMode;
	bool m_updated;
	bool m_initialized;

public:
	LWFRendererFactory(const std::string &path)
		: m_path(path), m_initialized(false) {}

	shared_ptr<Renderer> ConstructBitmap(
		LWF *lwf, int objId, Bitmap *bitmap);
	shared_ptr<Renderer> ConstructBitmapEx(
		LWF *lwf, int objId, BitmapEx *bitmapEx);
	shared_ptr<TextRenderer> ConstructText(
		LWF *lwf, int objId, Text *text);
	shared_ptr<Renderer> ConstructParticle(
		LWF *lwf, int objId, Particle *particle);

	void Init(LWF *lwf);
	void InitGL();
	void BeginRender(LWF *lwf);
	void EndRender(LWF *lwf);
	void Destruct();
	void SetBlendMode(int blendMode) {m_blendMode = blendMode;}
	void SetMaskMode(int maskMode) {m_maskMode = maskMode;}
	int GetBlendMode() {return m_blendMode;}
	int GetMaskMode() {return m_maskMode;}

	void FitForHeight(LWF *lwf, float w, float h);
	void FitForWidth(LWF *lwf, float w, float h);
	void ScaleForHeight(LWF *lwf, float w, float h);
	void ScaleForWidth(LWF *lwf, float w, float h);

	void PushVertices(
		LWFBitmapRendererContext *context, const Vertex *vertices);

private:
	void AddMesh(LWFBitmapRendererContext *context, int blendMode);
};

}	// namespace LWF
