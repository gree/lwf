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

#include "lwf_button.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_movie.h"
#include "lwf_compat.h"

namespace LWF {

void LWF::InitEvent()
{
	m_eventHandlers.resize(data->events.size());
	m_movieEventHandlers.resize(data->instanceNames.size());
	m_buttonEventHandlers.resize(data->instanceNames.size());
}

int LWF::AddEventHandler(string eventName, EventHandler eventHandler)
{
	int eventId = SearchEventId(eventName);
	int id;
	if (eventId >= 0 && eventId < (int)data->events.size()) {
		id = AddEventHandler(eventId, eventHandler);
	} else {
		GenericEventHandlerDictionary::iterator it =
			m_genericEventHandlerDictionary.find(eventName);
		if (it == m_genericEventHandlerDictionary.end()) {
			m_genericEventHandlerDictionary[eventName] = EventHandlerList();
			it = m_genericEventHandlerDictionary.find(eventName);
		}
		id = GetEventOffset();
		it->second.push_back(make_pair(id, eventHandler));
	}
	return id;
}

int LWF::AddEventHandler(int eventId, EventHandler eventHandler)
{
	if (eventId < 0 || eventId >= (int)data->events.size())
		return -1;
	int id = GetEventOffset();
	m_eventHandlers[eventId].push_back(make_pair(id, eventHandler));
	return id;
}

class Pred
{
private:
	int id;
public:
	Pred(int i) : id(i) {}
	bool operator()(const pair<int, EventHandler> &h)
	{
		return h.first == id;
	}
};

void LWF::RemoveEventHandler(string eventName, int id)
{
	if (id < 0)
		return;
	int eventId = SearchEventId(eventName);
	if (eventId >= 0 && eventId < (int)data->events.size()) {
		RemoveEventHandler(eventId, id);
	} else {
		GenericEventHandlerDictionary::iterator it =
			m_genericEventHandlerDictionary.find(eventName);
		if (it != m_genericEventHandlerDictionary.end())
			remove_if(it->second.begin(), it->second.end(), Pred(id));
	}
}

void LWF::RemoveEventHandler(int eventId, int id)
{
	if (id < 0)
		return;
	if (eventId < 0 || eventId >= (int)data->events.size())
		return;
	EventHandlerList &list = m_eventHandlers[eventId];
	remove_if(list.begin(), list.end(), Pred(id));
}

void LWF::ClearEventHandler(string eventName)
{
	int eventId = SearchEventId(eventName);
	if (eventId >= 0 && eventId < (int)data->events.size()) {
		ClearEventHandler(eventId);
	} else {
		m_genericEventHandlerDictionary.erase(eventName);
	}
}

void LWF::ClearEventHandler(int eventId)
{
	if (eventId < 0 || eventId >= (int)data->events.size())
		return;
	m_eventHandlers[eventId].clear();
}

int LWF::SetEventHandler(string eventName, EventHandler eventHandler)
{
	return SetEventHandler(SearchEventId(eventName), eventHandler);
}

int LWF::SetEventHandler(int eventId, EventHandler eventHandler)
{
	ClearEventHandler(eventId);
	return AddEventHandler(eventId, eventHandler);
}

void LWF::DispatchEvent(string eventName, Movie *m, Button *b)
{
	if (m == 0)
		m = rootMovie.get();
	int eventId = SearchEventId(eventName);
	EventHandlerList *list = 0;
	if (eventId >= 0 && eventId < (int)data->events.size()) {
		list = &m_eventHandlers[eventId];
	} else {
		GenericEventHandlerDictionary::iterator it =
			m_genericEventHandlerDictionary.find(eventName);
		if (it != m_genericEventHandlerDictionary.end())
			list = &it->second;
	}

	if (list && !list->empty()) {
		scoped_ptr<EventHandlerList> l(new EventHandlerList(*list));
		EventHandlerList::iterator it(l->begin()), itend(l->end());
		for (; it != itend; ++it)
			it->second(m, b);
	}
}

MovieEventHandlers *LWF::GetMovieEventHandlers(const Movie *m)
{
	if (!m_movieEventHandlersByFullName.empty()) {
		string fullName = m->GetFullName();
		if (!fullName.empty()) {
			MovieEventHandlersDictionary::iterator it =
				m_movieEventHandlersByFullName.find(fullName);
			if (it != m_movieEventHandlersByFullName.end())
				return &it->second;
		}
	}

	int instId = m->instanceId;
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return 0;
	return &m_movieEventHandlers[instId];
}

int LWF::AddMovieEventHandler(
	string instanceName, const MovieEventHandlerDictionary &h)
{
	if (h.empty())
		return -1;

	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0)
		return AddMovieEventHandler(instId, h);

	if (instanceName.find('.') == string::npos)
		return -1;

	MovieEventHandlersDictionary::iterator it =
		m_movieEventHandlersByFullName.find(instanceName);
	if (it == m_movieEventHandlersByFullName.end()) {
		m_movieEventHandlersByFullName[instanceName] = MovieEventHandlers();
		it = m_movieEventHandlersByFullName.find(instanceName);
	}
	int id = GetEventOffset();
	it->second.Add(id, h);

	Movie *m = SearchMovieInstance(instId);
	if (m)
		m->AddHandlers(&it->second);

	return id;
}

int LWF::AddMovieEventHandler(
	int instId, const MovieEventHandlerDictionary &h)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return -1;

	int id = GetEventOffset();
	m_movieEventHandlers[instId].Add(id, h);

	Movie *m = SearchMovieInstanceByInstanceId(instId);
	if (m)
		m->AddHandlers(&m_movieEventHandlers[instId]);

	return id;
}

void LWF::RemoveMovieEventHandler(string instanceName, int id)
{
	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0) {
		RemoveMovieEventHandler(instId, id);
		return;
	}

	if (m_movieEventHandlersByFullName.empty())
		return;

	MovieEventHandlersDictionary::iterator it =
		m_movieEventHandlersByFullName.find(instanceName);
	if (it == m_movieEventHandlersByFullName.end())
		return;

	it->second.Remove(id);
}

void LWF::RemoveMovieEventHandler(int instId, int id)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return;

	m_movieEventHandlers[instId].Remove(id);
}

void LWF::ClearMovieEventHandler(string instanceName)
{
	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0) {
		ClearMovieEventHandler(instId);
		return;
	}

	if (m_movieEventHandlersByFullName.empty())
		return;

	MovieEventHandlersDictionary::iterator it =
		m_movieEventHandlersByFullName.find(instanceName);
	if (it == m_movieEventHandlersByFullName.end())
		return;

	it->second.Clear();
}

void LWF::ClearMovieEventHandler(int instId)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return;

	m_movieEventHandlers[instId].Clear();
}

void LWF::ClearMovieEventHandler(string instanceName, string type)
{
	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0) {
		ClearMovieEventHandler(instId, type);
		return;
	}

	if (m_movieEventHandlersByFullName.empty())
		return;

	MovieEventHandlersDictionary::iterator it =
		m_movieEventHandlersByFullName.find(instanceName);
	if (it == m_movieEventHandlersByFullName.end())
		return;

	it->second.Clear(type);
}

void LWF::ClearMovieEventHandler(int instId, string type)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return;

	m_movieEventHandlers[instId].Clear(type);
}

void LWF::SetMovieEventHandler(
	string instanceName, const MovieEventHandlerDictionary &h)
{
	ClearMovieEventHandler(instanceName);
	AddMovieEventHandler(instanceName, h);
}

void LWF::SetMovieEventHandler(int instId, const MovieEventHandlerDictionary &h)
{
	ClearMovieEventHandler(instId);
	AddMovieEventHandler(instId, h);
}

ButtonEventHandlers *LWF::GetButtonEventHandlers(const Button *b)
{
	if (!m_buttonEventHandlersByFullName.empty()) {
		string fullName = b->GetFullName();
		if (!fullName.empty()) {
			ButtonEventHandlersDictionary::iterator it =
				m_buttonEventHandlersByFullName.find(fullName);
			if (it != m_buttonEventHandlersByFullName.end())
				return &it->second;
		}
	}

	int instId = b->instanceId;
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return 0;
	return &m_buttonEventHandlers[instId];
}

int LWF::AddButtonEventHandler(string instanceName,
	const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh)
{
	if (h.empty())
		return -1;

	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0)
		return AddButtonEventHandler(instId, h, kh);

	if (instanceName.find('.') == string::npos)
		return -1;

	ButtonEventHandlersDictionary::iterator it =
		m_buttonEventHandlersByFullName.find(instanceName);
	if (it == m_buttonEventHandlersByFullName.end()) {
		m_buttonEventHandlersByFullName[instanceName] = ButtonEventHandlers();
		it = m_buttonEventHandlersByFullName.find(instanceName);
	}
	int id = GetEventOffset();
	it->second.Add(id, h, kh);

	Button *b = SearchButtonInstance(instId);
	if (b)
		b->AddHandlers(&it->second);

	return id;
}

int LWF::AddButtonEventHandler(
	int instId, const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return -1;

	int id = GetEventOffset();
	m_buttonEventHandlers[instId].Add(id, h, kh);

	Button *b = SearchButtonInstanceByInstanceId(instId);
	if (b)
		b->AddHandlers(&m_buttonEventHandlers[instId]);

	return id;
}

void LWF::RemoveButtonEventHandler(string instanceName, int id)
{
	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0) {
		RemoveButtonEventHandler(instId, id);
		return;
	}

	if (m_buttonEventHandlersByFullName.empty())
		return;

	ButtonEventHandlersDictionary::iterator it =
		m_buttonEventHandlersByFullName.find(instanceName);
	if (it == m_buttonEventHandlersByFullName.end())
		return;

	it->second.Remove(id);
}

void LWF::RemoveButtonEventHandler(int instId, int id)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return;

	m_buttonEventHandlers[instId].Remove(id);
}

void LWF::ClearButtonEventHandler(string instanceName)
{
	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0) {
		ClearButtonEventHandler(instId);
		return;
	}

	if (m_buttonEventHandlersByFullName.empty())
		return;

	ButtonEventHandlersDictionary::iterator it =
		m_buttonEventHandlersByFullName.find(instanceName);
	if (it == m_buttonEventHandlersByFullName.end())
		return;

	it->second.Clear();
}

void LWF::ClearButtonEventHandler(int instId)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return;

	m_buttonEventHandlers[instId].Clear();
}

void LWF::ClearButtonEventHandler(string instanceName, string type)
{
	int instId = SearchInstanceId(GetStringId(instanceName));
	if (instId >= 0) {
		ClearButtonEventHandler(instId, type);
		return;
	}

	if (m_buttonEventHandlersByFullName.empty())
		return;

	ButtonEventHandlersDictionary::iterator it =
		m_buttonEventHandlersByFullName.find(instanceName);
	if (it == m_buttonEventHandlersByFullName.end())
		return;

	it->second.Clear(type);
}

void LWF::ClearButtonEventHandler(int instId, string type)
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return;

	m_buttonEventHandlers[instId].Clear(type);
}

void LWF::SetButtonEventHandler(string instanceName,
	const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh)
{
	ClearButtonEventHandler(instanceName);
	AddButtonEventHandler(instanceName, h, kh);
}

void LWF::SetButtonEventHandler(int instId,
	const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh)
{
	ClearButtonEventHandler(instId);
	AddButtonEventHandler(instId, h, kh);
}

void LWF::ClearAllEventHandlers()
{
	m_eventHandlers.clear();
	m_movieEventHandlers.clear();
	m_buttonEventHandlers.clear();
	InitEvent();
}

}	// namespace LWF
