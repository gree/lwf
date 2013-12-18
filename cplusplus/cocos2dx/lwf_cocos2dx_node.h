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

#ifndef LWF_COCOS2DX_NODE_H
#define LWF_COCOS2DX_NODE_H

#include "CCSprite.h"
#include "lwf_type.h"

namespace LWF {
class LWF;
}

NS_CC_BEGIN

class EventListenerTouchOneByOne;

class LWFNode : public Sprite
{
public:
	LWF::shared_ptr<LWF::LWF> lwf;
	LWF::string basePath;

private:
	EventListenerTouchOneByOne *_listener;
	bool _destructed;

public:
	static LWFNode *create(const char *pszFileName, void *l = 0);
	static void dump();

public:
	LWFNode();
	virtual ~LWFNode();

    bool initWithLWFFile(const std::string &filename, void *l = 0);

	virtual LWF::shared_ptr<LWF::LWF> attachLWF(
		const char *pszFilename, const char *pszTarget,
		const char *pszAttachName);

	void remove(Node *child);

	virtual void update(float dt) override;
	virtual void draw() override;

	virtual void onEnter() override;
	virtual void onExit() override;

	virtual bool handleTouch(Touch *touch, Event *event);
};

NS_CC_END

#endif
