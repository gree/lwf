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
#include "lwf_cocos2dx_node.h"
#include "lwf_cocos2dx_particle.h"
#include "lwf_cocos2dx_resourcecache.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_text.h"
#include "lwf_particle.h"

namespace LWF {

class LWFParticle : public cocos2d::ParticleSystemQuad
{
public:
    string path;
protected:
	Matrix m_matrix;

public:
	static LWFParticle *create(const char *path)
	{
		LWFParticle *particle = new LWFParticle();
		if (particle && particle->initWithFile(path)) {
			particle->autorelease();
			return particle;
		}
		CC_SAFE_DELETE(particle);
		return NULL;
	}

	bool initWithFile(const char *filePath)
	{
		cocos2d::LWFResourceCache *cache =
			cocos2d::LWFResourceCache::sharedLWFResourceCache();
		cocos2d::ValueMap dict = cache->loadParticle(filePath);
		if (dict.empty())
			return false;
		path = dict["path"].asString();
		if (!initWithDictionary(dict)) {
			cache->unloadParticle(path);
			return false;
		}

		return true;
	}

	void setMatrixAndColorTransform(const Matrix *m, const ColorTransform *)
	{
		bool changed = m_matrix.SetWithComparing(m);
		if (changed)
			setPosition(cocos2d::Point(m->translateX, -m->translateY));
	}
};

LWFParticleRenderer::LWFParticleRenderer(
		LWF *l, Particle *particle, cocos2d::LWFNode *node)
	: Renderer(l), m_particle(0)
{
	const Format::Particle &p = l->data->particles[particle->objectId];
	const Format::ParticleData &d = l->data->particleDatas[p.particleDataId];
	string filename = l->data->strings[d.stringId];
	filename += ".plist";
	string path = node->basePath + filename;

	m_particle = LWFParticle::create(path.c_str());
	if (!m_particle) {
		cocos2d::LWFResourceCache *cache =
			cocos2d::LWFResourceCache::sharedLWFResourceCache();
		path = cache->getDefaultParticlePathPrefix() + filename;
		m_particle = LWFParticle::create(path.c_str());
		if (!m_particle)
			return;
	}

	node->addChild(m_particle);
}

LWFParticleRenderer::~LWFParticleRenderer()
{
}

void LWFParticleRenderer::Destruct()
{
	if (!m_particle)
		return;

	cocos2d::LWFNode *node =
		dynamic_cast<cocos2d::LWFNode *>(m_particle->getParent());
	if (node)
		node->remove(m_particle);
	cocos2d::LWFResourceCache::sharedLWFResourceCache()->unloadParticle(m_particle->path);
	m_particle = 0;
}

void LWFParticleRenderer::Update(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
}

void LWFParticleRenderer::Render(
	const Matrix *matrix, const ColorTransform *colorTransform,
	int renderingIndex, int renderingCount, bool visible)
{
	if (!m_particle)
		return;

	m_particle->setVisible(visible);
	if (!visible)
		return;

	m_particle->setZOrder(renderingIndex);
	m_particle->setMatrixAndColorTransform(matrix, colorTransform);
}

}   // namespace LWF
