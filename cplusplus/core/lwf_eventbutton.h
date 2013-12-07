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

#ifndef LWF_EVENTBUTTON_H
#define	LWF_EVENTBUTTON_H

#include "lwf_type.h"

namespace LWF {

class Button;

typedef function<void (Button *)> ButtonEventHandler;
typedef function<void (Button *, int)> ButtonKeyPressHandler;
typedef vector<pair<int, ButtonEventHandler> > ButtonEventHandlerList;
typedef vector<pair<int, ButtonKeyPressHandler> > ButtonKeyPressHandlerList;
typedef map<string, ButtonEventHandler> ButtonEventHandlerDictionary;

class ButtonEventHandlers
{
public:
	enum Type {
		LOAD,
		UNLOAD,
		ENTERFRAME,
		UPDATE,
		RENDER,
		PRESS,
		RELEASE,
		ROLLOVER,
		ROLLOUT,
		KEYPRESS,
		EVENTS = KEYPRESS,
	};

private:
	bool m_empty;
	ButtonEventHandlerList m_handlers[EVENTS];
	ButtonKeyPressHandlerList m_keyPressHandler;

public:
	ButtonEventHandlers() : m_empty(true) {}
	bool Empty() const {return m_empty;}
	void Clear();
	void Clear(string type);
	void Add(const ButtonEventHandlers *h);
	void Add(int eventId,
		const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh);
	bool Add(int eventId, string type, const ButtonEventHandler &h);
	void Remove(int id);
	void Call(Type type, Button *target);
	void CallKEYPRESS(Button *target, int code);

private:
	void UpdateEmpty();
};

}	// namespace LWF

#endif
