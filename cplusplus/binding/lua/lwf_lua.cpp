#if defined(LWF_USE_LUA)

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
#include "lwf_property.h"
#include "lwf_renderer.h"
#include "lwf_utility.h"

#include "luna.h"
extern void luaopen_LWF(lua_State* L);
extern "C" {
# include "lualib.h"
}

namespace LWF {

class EventHandlerWrapper
{
public:
	int handlerId;
public:
	EventHandlerWrapper(int h) : handlerId(h) {}
	void operator()(Movie *m, Button *b) {
		if (!m->lwf->PushHandlerLua(handlerId))
			return;

		/* -1: function */
		lua_State *l = (lua_State *)m->lwf->luaState;
		Luna<Movie>::push(l, m, false);
		Luna<Button>::push(l, b, false);
		/* -3: function */
		/* -2: Movie */
		/* -1: Button */
		m->lwf->CallLua(2);
		/* 0 */
	}

	void operator()(Movie *m) {
		lua_State *l = (lua_State *)m->lwf->luaState;
		int args = lua_gettop(l);
		if (!m->lwf->PushHandlerLua(handlerId))
			return;

		/* -1: function */
		Luna<Movie>::push(l, m, false);
		/* -2: function */
		/* -1: Movie */
		if (args == 3) {
			/* with argument */
			lua_pushvalue(l, 3);
			/* -3: function */
			/* -2: Movie */
			/* -1: Argument */
			m->lwf->CallLua(2);
		} else {
			/* without argument */
			m->lwf->CallLua(1);
		}
		/* 0 */
	}

	void operator()(Button *b) {
		if (!b->lwf->PushHandlerLua(handlerId))
			return;

		/* -1: function */
		lua_State *l = (lua_State *)b->lwf->luaState;
		Luna<Button>::push(l, b, false);
		/* -2: function */
		/* -1: Button */
		b->lwf->CallLua(1);
		/* 0 */
	}

	void operator()(Button *b, int k) {
		if (!b->lwf->PushHandlerLua(handlerId))
			return;

		/* -1: function */
		lua_State *l = (lua_State *)b->lwf->luaState;
		Luna<Button>::push(l, b, false);
		lua_pushnumber(l, k);
		/* -3: function */
		/* -2: Button */
		/* -1: int */
		b->lwf->CallLua(2);
		/* 0 */
	}
};

template <class T>
class HandlerWrapper
{
public:
	int handlerId;
public:
	HandlerWrapper(int h) : handlerId(h) {}
	void operator()(T *a) {
		if (!a->lwf->PushHandlerLua(handlerId))
			return;

		/* -1: function */
		lua_State *l = (lua_State *)a->lwf->luaState;
		Luna<T>::push(l, a, false);
		/* -2: function */
		/* -1: Movie or Button */
		a->lwf->CallLua(1);
		/* 0 */
	}
};

void LWF::InitLua()
{
	if (!luaState)
		return;

	lua_State *l = (lua_State *)luaState;
	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		luaopen_LWF(l);
		lua_getglobal(l, "LWF");
		/* -1: LWF */
	}
	lua_getfield(l, -1, "LWF");
	/* -2: LWF */
	/* -1: LWF.LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 2);
		/* 0 */
		luaopen_LWF(l);
		lua_getglobal(l, "LWF");
		/* -1: LWF */
	} else {
		lua_pop(l, 1);
		/* -1: LWF */
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* -1: LWF */
		lua_newtable(l);
		/* -2: LWF */
		/* -1: table */
		lua_pushvalue(l, -1);
		/* -3: LWF */
		/* -2: table */
		/* -1: table */
		lua_setfield(l, -3, "Instances");
		/* -2: LWF */
		/* -1: table (LWF.Instances) */
	}
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	lua_newtable(l);
	/* -2: LWF.Instances */
	/* -1: table */
	lua_pushvalue(l, -1);
	/* -3: LWF.Instances */
	/* -2: table */
	/* -1: table */
	lua_setfield(l, -3, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	lua_newtable(l);
	/* -2: LWF.Instances.<instanceId> */
	/* -1: table */
	lua_setfield(l, -2, "Handlers");
	/* -1: LWF.Instances.<instanceId> */
	lua_newtable(l);
	/* LWF.Instances.<instanceId>.Handlers = {} */
	/* -2: LWF.Instances.<instanceId> */
	/* -1: table */
	lua_setfield(l, -2, "Movies");
	/* LWF.Instances.<instanceId>.Movies = {} */
	/* -1: LWF.Instances.<instanceId> */
	lua_pop(l, 1);
	/* 0 */

	lua_getglobal(l, "LWF");
	/* -1: LWF */
	lua_getfield(l, -1, "Script");
	/* -2: LWF */
	/* -1: LWF.Script */
	lua_remove(l, -2);
	/* -1: LWF.Script */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, name.c_str());
	/* -2: LWF.Script */
	/* -1: LWF.Script.<name> */
	lua_remove(l, -2);
	/* -1: LWF.Script.<name> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}

	string event = "Event_";
	for (int eventId = 0; eventId < data->events.size(); ++eventId) {
		lua_getfield(l, -1, (event +
			data->strings[data->events[eventId].stringId]).c_str());
		/* -2: LWF.Script.<name> */
		/* -1: function or nil: LWF.Script.<name>.Event_<eventname> */
		if (lua_isfunction(l, -1))
			eventFunctions[eventId] = true;
		lua_pop(l, 1);
		/* -1: LWF.Script.<name> */
	}

	lua_pop(l, 1);
	/* 0 */
}

void LWF::DestroyLua()
{
	if (!luaState)
		return;

	lua_State *l = (lua_State *)luaState;
	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Script");
	/* -2: LWF */
	/* -1: LWF.Script */
	lua_remove(l, -2);
	/* -1: LWF.Script */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, name.c_str());
	/* -2: LWF.Script */
	/* -1: LWF.Script.<name> */
	lua_remove(l, -2);
	/* -1: LWF.Script.<name> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Destroy");
	/* -2: LWF.Script.<name> */
	/* -1: function or nil: LWF.Script.<name>.Destroy */
	lua_remove(l, -2);
	/* -1: function or nil: LWF.Script.<name>.Destroy */
	if (lua_isfunction(l, -1)) {
		Luna<LWF::LWF>::push(l, this, false);
		/* -2: LWF.Script.<name>.Destroy */
		/* -1: LWF instance */
		CallLua(1);
		/* 0 */
	}
	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_pushnil(l);
	/* -2: LWF.Instances */
	/* -1: nil */
	lua_setfield(l, -2, instanceIdString.c_str());
	/* LWF.Instances.<instanceId> = nil */
	/* -1: LWF.Instances */
	lua_pop(l, 1);
	/* 0 */
}

void LWF::CallLua(int nargs)
{
	lua_State *l = (lua_State *)luaState;
	luaError = "";
	if (lua_pcall(l, nargs, 0, 0)) {
		luaError += lua_tostring(l, -1);
		luaError += "\n";
		lua_pop(l, 1);
	}
}

void LWF::DestroyMovieLua(Movie *movie)
{
	if (!luaState)
		return;

	lua_State *l = (lua_State *)luaState;
	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Movies");
	/* -2: LWF.Instances.<instanceId> */
	/* -1: LWF.Instances.<instanceId>.Movies */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Movies */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_pushnil(l);
	/* -2: LWF.Instances.<instanceId>.Movies */
	/* -1: nil */
	char buff[32];
	snprintf(buff, sizeof(buff), "%d", movie->iObjectId);
	lua_setfield(l, -2, buff);
	/* LWF.Instances.<instanceId>.Movies.<iObjectId> = nil */
	/* -1: LWF.Instances.<instanceId>.Movies */
	lua_pop(l, 1);
	/* 0 */
	return;
}

bool LWF::GetFieldLua(Movie *movie, string key)
{
	if (!luaState)
		return false;

	lua_State *l = (lua_State *)luaState;
	/* 1: LWF_Movie instance */
	/* 2: key */

	lua_getglobal(l, "LWF");
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, "Movies");
	/* -2: LWF.Instances.<instanceId> */
	/* -1: LWF.Instances.<instanceId>.Movies */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Movies */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	char buff[32];
	snprintf(buff, sizeof(buff), "%d", movie->iObjectId);
	lua_getfield(l, -1, buff);
	/* -2: LWF.Instances.<instanceId>.Movies */
	/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
	lua_getfield(l, -1, key.c_str());
	/* -2: LWF.Instances.<instanceId>.Movies.<iObjectId> */
	/* -1: value */
	lua_remove(l, -2);
	/* -1: value */
	if (lua_isnil(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	return true;
}

bool LWF::SetFieldLua(Movie *movie, string key)
{
	if (!luaState)
		return false;

	lua_State *l = (lua_State *)luaState;
	/* 1: LWF_Movie instance */
	/* 2: key */
	/* 3: value */

	if (lua_isstring(l, 3) && movie->SearchText(key)) {
		movie->lwf->SetText(
			movie->GetFullName() + "." + key, lua_tostring(l, 3));
	}

	lua_getglobal(l, "LWF");
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, "Movies");
	/* -2: LWF.Instances.<instanceId> */
	/* -1: LWF.Instances.<instanceId>.Movies */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Movies */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		return false;
	}
	char buff[32];
	snprintf(buff, sizeof(buff), "%d", movie->iObjectId);
	lua_getfield(l, -1, buff);
	/* -2: LWF.Instances.<instanceId>.Movies */
	/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* -1: LWF.Instances.<instanceId>.Movies */
		lua_newtable(l);
		/* -2: LWF.Instances.<instanceId>.Movies */
		/* -1: table */
		lua_pushvalue(l, -1);
		/* -3: LWF.Instances.<instanceId>.Movies */
		/* -2: table */
		/* -1: table */
		lua_setfield(l, -3, buff);
		/* -2: LWF.Instances.<instanceId>.Movies */
		/* -1: table LWF.Instances.<instanceId>.Movies.<iObjectId> */
	}
	lua_pushvalue(l, 3);
	/* -2: LWF.Instances.<instanceId>.Movies.<iObjectId> */
	/* -1: value */
	lua_setfield(l, -2, key.c_str());
	/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
	lua_pop(l, 1);
	/* 0 */
	return true;
}

string LWF::GetTextLua(Movie *movie, string textName)
{
	if (!luaState)
		return string();

	lua_State *l = (lua_State *)luaState;
	if (!GetFieldLua(movie, textName)) {
		/* 0: failed */
		return string();
	}
	if (!lua_isstring(l, -1)) {
		/* -1: nil or not text */
		lua_pop(l, 1);
		return string();
	}
	/* -1: text */
	string text = lua_tostring(l, -1);
	lua_pop(l, 1);
	/* 0 */
	return text;
}

bool LWF::PushHandlerLua(int handlerId)
{
	if (!luaState)
		return false;

	lua_State *l = (lua_State *)luaState;
	lua_getglobal(l, "LWF");
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return false;
	}
	lua_getfield(l, -1, "Handlers");
	/* -2: LWF.Instances.<instanceId> */
	/* -1: LWF.Instances.<instanceId>.Handlers */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Handlers */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		return false;
	}
	char buff[32];
	snprintf(buff, sizeof(buff), "%d", handlerId);
	lua_getfield(l, -1, buff);
	/* -2: LWF.Instances.<instanceId>.Handlers */
	/* -1: LWF.Instances.<instanceId>.Handlers.<handlerId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Handlers.<handlerId> */
	if (!lua_isfunction(l, -1)) {
		lua_pop(l, 0);
		/* 0 */
		return false;
	}
	/* -1: LWF.Instances.<instanceId>.Handlers.<handlerId>: function */
	return true;
}

int LWF::AddEventHandlerLua(Movie *movie, Button *button)
{
	if (!luaState)
		return 0;

	lua_State *l = (lua_State *)luaState;
	string event;
	char buff[32];
	int luaHandlerId;
	int handlerId;

	/* 1: LWF_LWF or LWF_Movie or LWF_Button instance */
	/* 2: string */
	/* 3: function */
	event = lua_tostring(l, 2);

	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, "Handlers");
	/* -2: LWF.Instances.<instanceId> */
	/* -1: LWF.Instances.<instanceId>.Handlers */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Handlers */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_pushvalue(l, 3);
	/* -2: LWF.Instances.<instanceId>.Handlers */
	/* -1: function */
	luaHandlerId = GetEventOffset();
	snprintf(buff, sizeof(buff), "%d", luaHandlerId);
	lua_setfield(l, -2, buff);
	/* LWF.Instances.<instanceId>.Handlers.<luaHandlerId> = function */
	/* -1: LWF.Instances.<instanceId>.Handlers */
	lua_pop(l, 1);
	/* 0 */

	if (movie) {
		handlerId = movie->AddEventHandler(
			event, EventHandlerWrapper(luaHandlerId));
	} else if (button) {
		handlerId = button->AddEventHandler(
			event, EventHandlerWrapper(luaHandlerId));
	} else {
		handlerId = AddEventHandler(event, EventHandlerWrapper(luaHandlerId));
	}

	lua_pushnumber(l, handlerId);
	/* -1: handlerId */
	return 1;

error:
	lua_pushnumber(l, -1);
	/* -1: -1 */
	return 1;
}

int LWF::AddMovieEventHandlerLua()
{
	if (!luaState)
		return 0;

	lua_State *l = (lua_State *)luaState;
	string instanceName;
	MovieEventHandlerDictionary handlers;
	int handlerId;

	/* 1: LWF_LWF instance */
	/* 2: instanceName:string */
	/* 3: table {key:string, handler:function} */
	instanceName = lua_tostring(l, 2);

	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, "Handlers");
	/* -2: LWF.Instances.<instanceId> */
	/* -1: LWF.Instances.<instanceId>.Handlers */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Handlers */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}

	lua_pushnil(l);
	/* -2: LWF.Instances.<instanceId>.Handlers */
	/* -1: nil */
	while (lua_next(l, 3)) {
		/* -3: LWF.Instances.<instanceId>.Handlers */
		/* -2: key: eventName string */
		/* -1: value: handler function */
		const char *key = lua_tostring(l, -2);
		if (key && lua_isfunction(l, -1)) {
			int luaHandlerId = GetEventOffset();
			handlers[key] = HandlerWrapper<Movie>(luaHandlerId);
			char buff[32];
			snprintf(buff, sizeof(buff), "%d", luaHandlerId);
			lua_setfield(l, -3, buff);
			/* LWF.Instances.<instanceId>.Handlers.<luaHandlerId> = function */
			/* -2: LWF.Instances.<instanceId>.Handlers */
			/* -1: key */
		} else {
			lua_pop(l, 1);
			/* -2: LWF.Instances.<instanceId>.Handlers */
			/* -1: key: eventName string */
		}
	}
	/* -1: LWF.Instances.<instanceId>.Handlers */
	lua_pop(l, 1);
	/* 0 */

	handlerId = AddMovieEventHandler(instanceName, handlers);
	lua_pushnumber(l, handlerId);
	/* handlerId */
	return 1;

error:
	lua_pushnumber(l, -1);
	/* -1: -1 */
	return 1;
}

int LWF::AddButtonEventHandlerLua()
{
	if (!luaState)
		return 0;

	lua_State *l = (lua_State *)luaState;
	string instanceName;
	ButtonEventHandlerDictionary handlers;
	int handlerId;

	/* 1: LWF_LWF instance */
	/* 2: instanceName:string */
	/* 3: table {key:string, handler:function} */
	instanceName = lua_tostring(l, 2);

	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, "Instances");
	/* -2: LWF */
	/* -1: LWF.Instances */
	lua_remove(l, -2);
	/* -1: LWF.Instances */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, instanceIdString.c_str());
	/* -2: LWF.Instances */
	/* -1: LWF.Instances.<instanceId> */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}
	lua_getfield(l, -1, "Handlers");
	/* -2: LWF.Instances.<instanceId> */
	/* -1: LWF.Instances.<instanceId>.Handlers */
	lua_remove(l, -2);
	/* -1: LWF.Instances.<instanceId>.Handlers */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		goto error;
	}

	lua_pushnil(l);
	/* -2: LWF.Instances.<instanceId>.Handlers */
	/* -1: nil */
	while (lua_next(l, 3)) {
		/* -3: LWF.Instances.<instanceId>.Handlers */
		/* -2: key: eventName string */
		/* -1: value: handler function */
		const char *key = lua_tostring(l, -2);
		if (key && lua_isfunction(l, -1)) {
			int luaHandlerId = GetEventOffset();
			handlers[key] = HandlerWrapper<Button>(luaHandlerId);
			char buff[32];
			snprintf(buff, sizeof(buff), "%d", luaHandlerId);
			lua_setfield(l, -3, buff);
			/* LWF.Instances.<instanceId>.Handlers.<luaHandlerId> = function */
			/* -2: LWF.Instances.<instanceId>.Handlers */
			/* -1: key */
		} else {
			lua_pop(l, 2);
			/* 0 */
			goto error;
		}
	}
	/* -1: LWF.Instances.<instanceId>.Handlers */
	lua_pop(l, 1);
	/* 0 */

	handlerId = AddButtonEventHandler(instanceName, handlers);
	lua_pushnumber(l, handlerId);
	/* handlerId */
	return 1;

error:
	lua_pushnumber(l, -1);
	/* -1: -1 */
	return 1;
}

int LWF::AttachMovieLua(Movie *movie, bool empty)
{
	if (!luaState)
		return 0;

	lua_State *l = (lua_State *)luaState;
	int args = lua_gettop(l);
	string linkageName;
	string aName;
	int attachDepth = -1;
	bool reorder = false;
	MovieEventHandlerDictionary handlers;
	Movie *child;
	int offset = empty ? 2 : 3;

	/* 1: LWF_Movie instance */
	/* 2: linkageName:string */
	/* 3: aName:string */
	/* 4: table {key:string, handler:function} */
	/* 5: attachDepth:number (option) */
	/* 6: reorder:boolean (option) */
	/* or */
	/* 1: LWF_Movie instance */
	/* 2: aName:string */
	/* 3: table {key:string, handler:function} */
	/* 4: attachDepth:number (option) */
	/* 5: reorder:boolean (option) */
	linkageName = empty ? "_empty" : lua_tostring(l, 2);
	aName = lua_tostring(l, offset);
	if (args >= offset + 1) {
		lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!lua_istable(l, -1)) {
			lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!lua_istable(l, -1)) {
			lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		lua_getfield(l, -1, instanceIdString.c_str());
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!lua_istable(l, -1)) {
			lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		lua_getfield(l, -1, "Handlers");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Handlers */
		lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Handlers */
		if (!lua_istable(l, -1)) {
			lua_pop(l, 1);
			/* 0 */
			goto error;
		}

		lua_pushnil(l);
		/* -2: LWF.Instances.<instanceId>.Handlers */
		/* -1: nil */
		while (lua_next(l, 4)) {
			/* -3: LWF.Instances.<instanceId>.Handlers */
			/* -2: key: eventName string */
			/* -1: value: handler function */
			const char *key = lua_tostring(l, -2);
			if (key && lua_isfunction(l, -1)) {
				int luaHandlerId = GetEventOffset();
				handlers[key] = HandlerWrapper<Movie>(luaHandlerId);
				char buff[32];
				snprintf(buff, sizeof(buff), "%d", luaHandlerId);
				lua_setfield(l, -3, buff);
				/* LWF.Instances.<instanceId>.Handlers.
					<luaHandlerId> = function */
				/* -2: LWF.Instances.<instanceId>.Handlers */
				/* -1: key */
			} else {
				lua_pop(l, 1);
				/* -2: LWF.Instances.<instanceId>.Handlers */
				/* -1: key: eventName string */
			}
		}
		/* -1: LWF.Instances.<instanceId>.Handlers */
		lua_pop(l, 1);
		/* 0 */
	}
	if (args >= offset + 2)
		attachDepth = lua_tonumber(l, offset + 2);
	if (args >= offset + 3)
		reorder = lua_toboolean(l, offset + 3);

	child = movie->AttachMovie(
		linkageName, aName, handlers, attachDepth, reorder);
	Luna<Movie>::push(l, child, false);
	/* -1: LWF_Movie child */
	return 1;

error:
	lua_pushnil(l);
	/* -1: nil */
	return 1;
}

int LWF::AttachLWFLua(Movie *movie)
{
	if (!luaState)
		return 0;

	lua_State *l = (lua_State *)luaState;
	int args = lua_gettop(l);
	string path;
	string aName;
	int attachDepth = -1;
	bool reorder = false;
	shared_ptr<LWF> child;

	/* 1: LWF_Movie instance */
	/* 2: path:string */
	/* 3: aName:string */
	/* 4: attachDepth:number (option) */
	/* 5: reorder:boolean (option) */
	path = lua_tostring(l, 2);
	aName = lua_tostring(l, 3);
	if (args >= 4)
		attachDepth = lua_tonumber(l, 4);
	if (args >= 5)
		reorder = lua_toboolean(l, 5);

	child = movie->AttachLWF(path, aName, attachDepth, reorder);
	if (!child)
		goto error;
	Luna<LWF>::push(l, child.get(), false);
	/* -1: LWF_LWF child */
	return 1;

error:
	lua_pushnil(l);
	/* -1: nil */
	return 1;
}

int LWF::AttachBitmapLua(Movie *movie)
{
	if (!luaState)
		return 0;

	lua_State *l = (lua_State *)luaState;
	string linkageName;
	int attachDepth;
	shared_ptr<BitmapClip> bitmapClip;

	/* 1: LWF_Movie instance */
	/* 2: linkageName:string */
	/* 3: attachDepth:number */
	linkageName = lua_tostring(l, 2);
	attachDepth = lua_tonumber(l, 3);

	bitmapClip = movie->AttachBitmap(linkageName, attachDepth);
	if (!bitmapClip)
		goto error;
	Luna<BitmapClip>::push(l, bitmapClip.get(), false);
	/* -1: LWF_BitmapClip bitmapClip */
	return 1;

error:
	lua_pushnil(l);
	/* -1: nil */
	return 1;
}

void LWF::GetFunctionsLua(int movieId, string &loadFunc,
	string &postLoadFunc, string &unloadFunc, string &enterFrameFunc,
	bool forRoot)
{
	if (!luaState)
		return;

	string linkageName = GetMovieLinkageName(movieId);
	if (linkageName.empty())
		return;

	lua_State *l = (lua_State *)luaState;
	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Script");
	/* -2: LWF */
	/* -1: LWF.Script */
	lua_remove(l, -2);
	/* -1: LWF.Script */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, name.c_str());
	/* -2: LWF.Script */
	/* -1: LWF.Script.<name> */
	lua_remove(l, -2);
	/* -1: LWF.Script.<name> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}

	string func = forRoot ? "Load" : linkageName + "_load";
	lua_getfield(l, -1, func.c_str());
	/* -2: LWF.Script.<name> */
	/* -1: function or nil */
	if (lua_isfunction(l, -1))
		loadFunc = func;
	lua_pop(l, 1);
	/* -1: LWF.Script.<name> */

	func = forRoot ? "PostLoad" : linkageName + "_postLoad";
	lua_getfield(l, -1, func.c_str());
	/* -2: LWF.Script.<name> */
	/* -1: function or nil */
	if (lua_isfunction(l, -1))
		postLoadFunc = func;
	lua_pop(l, 1);
	/* -1: LWF.Script.<name> */

	func = forRoot ? "Unload" : linkageName + "_unload";
	lua_getfield(l, -1, func.c_str());
	/* -2: LWF.Script.<name> */
	/* -1: function or nil */
	if (lua_isfunction(l, -1))
		unloadFunc = func;
	lua_pop(l, 1);
	/* -1: LWF.Script.<name> */

	func = forRoot ? "EnterFrame" : linkageName + "_enterFrame";
	lua_getfield(l, -1, func.c_str());
	/* -2: LWF.Script.<name> */
	/* -1: function or nil */
	if (lua_isfunction(l, -1))
		enterFrameFunc = func;
	lua_pop(l, 2);
	/* 0 */
}

void LWF::CallFunctionLua(string function, Movie *movie)
{
	if (!luaState)
		return;

	lua_State *l = (lua_State *)luaState;
	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Script");
	/* -2: LWF */
	/* -1: LWF.Script */
	lua_remove(l, -2);
	/* -1: LWF.Script */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, name.c_str());
	/* -2: LWF.Script */
	/* -1: LWF.Script.<name> */
	lua_remove(l, -2);
	/* -1: LWF.Script.<name> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, function.c_str());
	/* -2: LWF.Script.<name> */
	/* -1: LWF.Script.<name>.<function> */
	lua_remove(l, -2);
	/* -1: LWF.Script.<name>.<function> */
	if (!lua_isfunction(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	Luna<Movie>::push(l, movie, false);
	/* -2: LWF.Script.<name>.<function> */
	/* -1: LWF_Movie instance */
	CallLua(1);
	/* 0 */
}

void LWF::CallEventFunctionLua(int eventId, Movie *movie, Button *button)
{
	if (!luaState)
		return;

	if (eventFunctions.find(eventId) == eventFunctions.end())
		return;

	lua_State *l = (lua_State *)luaState;
	lua_getglobal(l, "LWF");
	/* -1: LWF */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, "Script");
	/* -2: LWF */
	/* -1: LWF.Script */
	lua_remove(l, -2);
	/* -1: LWF.Script */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	lua_getfield(l, -1, name.c_str());
	/* -2: LWF.Script */
	/* -1: LWF.Script.<name> */
	lua_remove(l, -2);
	/* -1: LWF.Script.<name> */
	if (!lua_istable(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	string event = "Event_";
	lua_getfield(l, -1,
		(event + data->strings[data->events[eventId].stringId]).c_str());
	/* -2: LWF.Script.<name> */
	/* -1: LWF.Script.<name>.Event_<eventName> */
	lua_remove(l, -2);
	/* -1: LWF.Script.<name>.Event_<eventName> */
	if (!lua_isfunction(l, -1)) {
		lua_pop(l, 1);
		/* 0 */
		return;
	}
	Luna<Movie>::push(l, movie, false);
	/* -2: LWF.Script.<name>.Event_<eventName> */
	/* -1: LWF_Movie instance */
	Luna<Button>::push(l, button, false);
	/* -3: LWF.Script.<name>.Event_<eventName> */
	/* -2: LWF_Movie instance */
	/* -1: LWF_Button instance */
	CallLua(2);
	/* 0 */
}

}	// namespace LWF

#endif // LWF_USE_LUA
