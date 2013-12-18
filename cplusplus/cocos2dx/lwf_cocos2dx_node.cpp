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

#include "CCDirector.h"
#include "CCEventListenerTouch.h"
#include "lwf_cocos2dx_factory.h"
#include "lwf_cocos2dx_node.h"
#include "lwf_cocos2dx_resourcecache.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_movie.h"

using LWFData = ::LWF::Data;

NS_CC_BEGIN;

using namespace LWF;

LWFNode *LWFNode::create(const char *pszFileName, void *l)
{
	LWFNode *node = new LWFNode();
	if (node && node->initWithLWFFile(pszFileName, l)) {
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
	: _listener(0), _destructed(false)
{
}

LWFNode::~LWFNode()
{
	_destructed = true;
	if (lwf) {
		shared_ptr<LWFData> data = lwf->data;
		lwf->Destroy();
		CC_SAFE_RELEASE(_texture);
		_texture = 0;
		LWFResourceCache::sharedLWFResourceCache()->unloadLWFData(data);
	}
}

bool LWFNode::initWithLWFFile(const string &path, void *l)
{
	shared_ptr<LWFData> data =
		LWFResourceCache::sharedLWFResourceCache()->loadLWFData(path);
	if (!data)
		return false;

	size_t pos = path.find_last_of('/');
	if (pos != string::npos)
		basePath = path.substr(0, pos + 1);

	bool result;
	if (data->textures.size() != 1) {
		result = Sprite::init();
	} else {
		const Format::Texture &t = data->textures[0];
		string filename = basePath + t.GetFilename(data.get());
		result = Sprite::initWithFile(filename.c_str());
	}
	if (!result) {
		LWFResourceCache::sharedLWFResourceCache()->unloadLWFData(data);
		return false;
	}

	setContentSize(Size(0, 0));
	setVertexRect(Rect(0, 0, 0, 0));

	shared_ptr<LWFRendererFactory> factory =
		make_shared<LWFRendererFactory>(this);
	lwf = make_shared<class LWF>(data, factory, l);

	scheduleUpdate();

	return true;
}

shared_ptr<class LWF> LWFNode::attachLWF(
	const char *pszFilename, const char *pszTarget, const char *pszAttachName)
{
	if (!lwf)
		return shared_ptr<class LWF>();

	shared_ptr<LWFData> data =
		LWFResourceCache::sharedLWFResourceCache()->loadLWFData(pszFilename);
	if (!data)
		return shared_ptr<class LWF>();

	shared_ptr<LWFRendererFactory> factory =
		make_shared<LWFRendererFactory>(this);
	shared_ptr<class LWF> child = make_shared<class LWF>(data, factory);
	if (!child) {
		LWFResourceCache::sharedLWFResourceCache()->unloadLWFData(data);
		return child;
	}

	Movie *movie = lwf->SearchMovieInstance(pszTarget);
	if (!movie) {
		LWFResourceCache::sharedLWFResourceCache()->unloadLWFData(data);
		return shared_ptr<class LWF>();
	}

	movie->AttachLWF(child, pszAttachName);

	return child;
}

void LWFNode::remove(Node *child)
{
	if (!_destructed)
		child->removeFromParent();
}

void LWFNode::update(float dt)
{
	if (lwf) {
		lwf->Exec(dt);
		lwf->Render();
	}
}

void LWFNode::draw()
{
	// NOTHING TO DO
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

NS_CC_END
