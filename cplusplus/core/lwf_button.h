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

#ifndef LWF_BUTTON_H
#define	LWF_BUTTON_H

#include "lwf_eventbutton.h"
#include "lwf_iobject.h"

namespace LWF {

class Button : public IObject
{
public:
	const Format::Button *data;
	Button *buttonLink;
	float hitX;
	float hitY;
	float width;
	float height;

private:
	Matrix m_invert;
	ButtonEventHandlers m_handler;

public:
	Button() {};
	Button(LWF *l, Movie *p, int objId, int instId, int mId = -1, int cId = -1);
	virtual ~Button() {};

	void AddHandlers(const ButtonEventHandlers *h);
	void Exec(int mId = 0, int cId = 0);
	void Update(const Matrix *m, const ColorTransform *c);
	void Render(bool v, int rOffset);
	void Destroy();
	void LinkButton();
	virtual bool CheckHit(float px, float py);
	virtual void EnterFrame();
	virtual void RollOver();
	virtual void RollOut();
	virtual void Press();
	virtual void Release();
	virtual void KeyPress(int code);
	void PlayAnimation(int condition, int code = 0);

	int AddEventHandler(string eventName, ButtonEventHandler eventHandler);
	void RemoveEventHandler(string eventName, int id);
	void ClearEventHandler(string eventName);
	int SetEventHandler(string eventName, ButtonEventHandler eventHandler);

	int AddKeyPressHandler(int key, ButtonKeyPressHandler eventHandler);
	void RemoveKeyPressHandler(int key, int id);
	void ClearKeyPressHandler(int key);
	int SetKeyPressHandler(int key, ButtonKeyPressHandler eventHandler);
};

}	// namespace LWF

#endif
