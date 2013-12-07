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

#include "lwf_eventbutton.h"
#include "lwf_compat.h"

namespace LWF {

#define EType ButtonEventHandlers
typedef map<string, int> table_t;
static table_t table;

static void PrepareTable()
{
	if (table.empty()) {
		const char *names[] = {
			"load",
			"unload",
			"enterFrame",
			"update",
			"render",
			"press",
			"release",
			"rollOver",
			"rollOut",
			0,
		};
		const int vals[] = {
			EType::LOAD,
			EType::UNLOAD,
			EType::ENTERFRAME,
			EType::UPDATE,
			EType::RENDER,
			EType::PRESS,
			EType::RELEASE,
			EType::ROLLOVER,
			EType::ROLLOUT,
		};

		for (int i = 0; names[i]; ++i)
			table[names[i]] = vals[i];
	}
};

void ButtonEventHandlers::Clear()
{
	for (int i = 0; i < EVENTS; ++i)
		m_handlers[i].clear();
	m_keyPressHandler.clear();
	m_empty = true;
}

void ButtonEventHandlers::Clear(string type)
{
	if (type == "keyPress") {
		m_keyPressHandler.clear();
	} else {
		PrepareTable();
		const table_t::iterator it = table.find(type);
		if (it == table.end())
			return;
		m_handlers[it->second].clear();
	}
	UpdateEmpty();
}

void ButtonEventHandlers::Add(const ButtonEventHandlers *h)
{
	if (!h)
		return;

	for (int i = 0; i < EVENTS; ++i)
		m_handlers[i].insert(m_handlers[i].end(),
			h->m_handlers[i].begin(), h->m_handlers[i].end());
	m_keyPressHandler.insert(m_keyPressHandler.end(),
			h->m_keyPressHandler.begin(), h->m_keyPressHandler.end());

	if (m_empty)
		m_empty = h->Empty();
}

void ButtonEventHandlers::Add(int eventId,
	const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh)
{
	ButtonEventHandlerDictionary::const_iterator it(h.begin()), itend(h.end());
	PrepareTable();
	table_t::const_iterator titend(table.end());
	for (; it != itend; ++it) {
		table_t::const_iterator tit(table.begin());
		for (; tit != titend; ++tit) {
			if (it->first == tit->first) {
				m_handlers[tit->second].push_back(
					make_pair(eventId, it->second));
			}
		}
	}
	if (kh)
		m_keyPressHandler.push_back(make_pair(eventId, kh));

	if (m_empty)
		UpdateEmpty();
}

bool ButtonEventHandlers::Add(
	int eventId, string type, const ButtonEventHandler &h)
{
	PrepareTable();
	const table_t::iterator it = table.find(type);
	if (it == table.end())
		return false;
	ButtonEventHandlerDictionary handlers;
	handlers[type] = h;
	Add(it->second, handlers, 0);
	return true;
}

class Pred
{
private:
	int id;
public:
	Pred(int i) : id(i) {}
	bool operator()(const pair<int, ButtonEventHandler> &h)
	{
		return h.first == id;
	}
};

class KPred
{
private:
	int id;
public:
	KPred(int i) : id(i) {}
	bool operator()(const pair<int, ButtonKeyPressHandler> &h)
	{
		return h.first == id;
	}
};

void ButtonEventHandlers::Remove(int id)
{
	for (int i = 0; i < EVENTS; ++i)
		remove_if(m_handlers[i].begin(), m_handlers[i].end(), Pred(id));
	remove_if(m_keyPressHandler.begin(), m_keyPressHandler.end(), KPred(id));

	UpdateEmpty();
}

class Exec
{
private:
	Button *target;
public:
	Exec(Button *t) : target(t) {}
	void operator()(const pair<int, ButtonEventHandler> &h)
	{
		h.second(target);
	}
};

void ButtonEventHandlers::Call(Type type, Button *target)
{
	scoped_ptr<ButtonEventHandlerList>
		p(new ButtonEventHandlerList(m_handlers[type]));
	for_each(p->begin(), p->end(), Exec(target));
}

class KExec
{
private:
	Button *target;
	int code;
public:
	KExec(Button *t, int c) : target(t), code(c) {}
	void operator()(const pair<int, ButtonKeyPressHandler> &h)
	{
		h.second(target, code);
	}
};

void ButtonEventHandlers::CallKEYPRESS(Button *target, int code)
{
	scoped_ptr<ButtonKeyPressHandlerList>
		p(new ButtonKeyPressHandlerList(m_keyPressHandler));
	for_each(p->begin(), p->end(), KExec(target, code));
}

void ButtonEventHandlers::UpdateEmpty()
{
	m_empty = true;
	for (int i = 0; i < EVENTS; ++i) {
		if (!m_handlers[i].empty()) {
			m_empty = false;
			break;
		}
	}
	if (m_empty)
		m_empty = m_keyPressHandler.empty();
}

}	// namespace LWF
