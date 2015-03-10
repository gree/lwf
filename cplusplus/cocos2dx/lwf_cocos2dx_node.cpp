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

#include "base/CCEventDispatcher.h"
#include "base/CCEventListenerTouch.h"
#include "lwf_cocos2dx_factory.h"
#include "lwf_cocos2dx_node.h"
#include "lwf_cocos2dx_resourcecache.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_movie.h"

using LWFData = ::LWF::Data;

NS_CC_BEGIN;

using namespace LWF;

LWFNode *LWFNode::create(
	const char *pszFileName, void *l, TextureLoadHandler textureLoadHandler)
{
	LWFNodeHandlers h;
	return createWithHandlers(pszFileName, h, l, textureLoadHandler);
}

LWFNode *LWFNode::createWithHandlers(const char *pszFileName,
	LWFNodeHandlers h, void *l, TextureLoadHandler textureLoadHandler)
{
	LWFNode *node = new LWFNode();
	if (node && node->initWithLWFFile(pszFileName, h, l, textureLoadHandler)) {
		node->autorelease();
		return node;
	}
	CC_SAFE_DELETE(node);
	return NULL;
}


void LWFNode::dump()
{
	LWFResourceCache::sharedLWFResourceCache()->dump();
}

LWFNode::LWFNode()
	: _listener(0), _destructed(false), _removeFromParentRequested(false)
{
}

LWFNode::~LWFNode()
{
	if (lwf) {
		shared_ptr<LWFData> data = lwf->data;
		lwf->Destroy();
		CC_SAFE_RELEASE_NULL(_texture);
		LWFResourceCache::sharedLWFResourceCache()->unloadLWFData(data);
	}
	_destructed = true;
}

class LWFLoader
{
private:
	LWFNode *m_node;
	void *m_l;

public:
	LWFLoader(LWFNode *node, void *l) : m_node(node), m_l(l) {}

	shared_ptr<class LWF> operator()(const string &path)
	{
		shared_ptr<LWFData> data =
			LWFResourceCache::sharedLWFResourceCache()->loadLWFData(path);
		if (!data)
			return shared_ptr<class LWF>();

		size_t pos = path.find_last_of('/');
		string basePath;
		if (pos != string::npos)
			basePath = path.substr(0, pos + 1);

		shared_ptr<LWFRendererFactory> factory =
			make_shared<LWFRendererFactory>(m_node, basePath);
		shared_ptr<class LWF> child =
			make_shared<class LWF>(data, factory, m_l);
		return child;
	}
};

bool LWFNode::initWithLWFFile(const string &path, LWFNodeHandlers h,
	void *l, TextureLoadHandler textureLoadHandler)
{
	shared_ptr<LWFData> data =
		LWFResourceCache::sharedLWFResourceCache()->loadLWFData(path);
	if (!data)
		return false;

	size_t pos = path.find_last_of('/');
	string basePath;
	if (pos != string::npos)
		basePath = path.substr(0, pos + 1);

	bool result;
	if (data->textures.size() != 1) {
		result = Sprite::init();
	} else {
		const Format::Texture &t = data->textures[0];
		string texturePath = t.GetFilename(data.get());
		string filename = basePath + texturePath;
		if (textureLoadHandler) {
			filename = textureLoadHandler(filename, basePath, texturePath);
		} else if (::LWF::LWF::GetTextureLoadHandler()) {
			filename = ::LWF::LWF::GetTextureLoadHandler()(
				filename, basePath, texturePath);
		}
		result = Sprite::initWithFile(filename.c_str());
		data->resourceCache[filename] = true;
	}
	if (!result) {
		LWFResourceCache::sharedLWFResourceCache()->unloadLWFData(data);
		return false;
	}

	setContentSize(Size(0, 0));
	setVertexRect(Rect(0, 0, 0, 0));

	shared_ptr<LWFRendererFactory> factory =
		make_shared<LWFRendererFactory>(this, basePath);
	lwf = make_shared<class LWF>(data, factory, l);
	lwf->lwfLoader = LWFLoader(this, l);

	_nodeHandlers = h;
	_textureLoadHandler = textureLoadHandler;

	scheduleUpdate();

	return true;
}

shared_ptr<class LWF> LWFNode::attachLWF(
	const char *pszFilename, const char *pszTarget, const char *pszAttachName)
{
	if (!lwf)
		return shared_ptr<class LWF>();

	Movie *movie = lwf->SearchMovieInstance(pszTarget);
	if (!movie)
		return shared_ptr<class LWF>();

	shared_ptr<class LWF> child = lwf->lwfLoader(pszFilename);
	if (!child)
		return shared_ptr<class LWF>();

	movie->AttachLWF(child, pszAttachName);

	return child;
}

void LWFNode::update(float dt)
{
	if (_removeFromParentRequested) {
		removeFromParent();
		return;
	}

	if (lwf) {
		lwf->Exec(dt);
		if (_removeFromParentRequested) {
			removeFromParent();
			return;
		}

		lwf->Render();
	}
}

void LWFNode::draw(Renderer *renderer, const Mat4& transform, uint32_t flags)
{
}

void LWFNode::onEnter()
{
	if (lwf && lwf->interactive) {
		_listener = EventListenerTouchOneByOne::create();
		_listener->setSwallowTouches(true);
		_listener->onTouchBegan = [&](Touch *touch, Event *event) {
			bool result = handleTouch(touch, event);
			if (result)
				lwf->InputPress();
			return result;
		};
		_listener->onTouchMoved = [&](Touch *touch, Event *event) {
			handleTouch(touch, event);
		};
		_listener->onTouchEnded = [&](Touch *touch, Event *event) {
			handleTouch(touch, event);
			if (lwf)
				lwf->InputRelease();
		};
		getEventDispatcher()->addEventListenerWithSceneGraphPriority(
			_listener, this);
	}
	Node::onEnter();
}

void LWFNode::onExit()
{
	if (_listener) {
		getEventDispatcher()->removeEventListener(_listener);
		_listener = 0;
	}
	Node::onExit();
}

bool LWFNode::handleTouch(Touch *touch, Event *event)
{
	if (!lwf || !lwf->interactive)
		return false;

	Point point = convertTouchToNodeSpace(touch);
	Button *button = lwf->InputPoint(point.x, -point.y);

	return button ? true : false;
}

void LWFNode::requestRemoveFromParent()
{
	_removeFromParentRequested = true;
}

void LWFNode::removeNodeFromParent(Node *node)
{
	LWFNode *lwfNode = dynamic_cast<LWFNode *>(node->getParent());
	if (lwfNode && lwfNode->isDestructed())
		return;

	node->removeFromParent();
}

NS_CC_END
