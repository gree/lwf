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
#include "lwf_cocos2dx_factory.h"
#include "lwf_cocos2dx_particle.h"
#include "lwf_cocos2dx_resourcecache.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_text.h"
#include "lwf_particle.h"

namespace LWF {

class LWFParticleImpl : public cocos2d::LWFParticle
{
public:
    string path;
protected:
	Particle *m_particle;
	Matrix m_matrix;

public:
	static LWFParticleImpl *create(Particle *particle, const char *path)
	{
		LWFParticleImpl *ret = new LWFParticleImpl();
		if (ret && ret->initWithFile(particle, path)) {
			ret->autorelease();
			return ret;
		}
		CC_SAFE_DELETE(ret);
		return NULL;
	}

	LWFParticleImpl() : LWFParticle(), m_particle(0)
	{
	}

	virtual ~LWFParticleImpl()
	{
	}

	virtual Particle *GetParticle()
	{
		return m_particle;
	}

	bool initWithFile(Particle *particle, const char *filePath)
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

		m_particle = particle;

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
	LWFRendererFactory *factory =
		(LWFRendererFactory *)l->rendererFactory.get();
	string path = factory->GetBasePath() + filename;

	m_particle = LWFParticleImpl::create(particle, path.c_str());
	if (!m_particle) {
		cocos2d::LWFResourceCache *cache =
			cocos2d::LWFResourceCache::sharedLWFResourceCache();
		path = cache->getDefaultParticlePathPrefix() + filename;
		m_particle = LWFParticleImpl::create(particle, path.c_str());
		if (!m_particle)
			return;
	}

	const cocos2d::LWFNodeHandlers &h = node->getNodeHandlers();
	if (h.onParticleLoaded)
		h.onParticleLoaded(m_particle);

	node->addChild(m_particle);
}

LWFParticleRenderer::~LWFParticleRenderer()
{
}

void LWFParticleRenderer::Destruct()
{
	if (!m_particle)
		return;

	cocos2d::LWFNode::removeNodeFromParent(m_particle);
	cocos2d::LWFResourceCache::sharedLWFResourceCache(
		)->unloadParticle(m_particle->path);
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

	m_particle->setLocalZOrder(renderingIndex);
	m_particle->setMatrixAndColorTransform(matrix, colorTransform);
}

cocos2d::LWFParticle *LWFParticleRenderer::GetParticle()
{
	return dynamic_cast<cocos2d::LWFParticle *>(m_particle);
}

}   // namespace LWF
