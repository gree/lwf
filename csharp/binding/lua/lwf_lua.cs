#if LWF_USE_LUA

using System;
using System.Collections.Generic;
using KopiLua;

namespace LWF {

using MovieEventHandlerDictionary = Dictionary<string, Action<Movie>>;
using ButtonEventHandlerDictionary = Dictionary<string, Action<Button>>;
using EventFunctions = Dictionary<int, bool>;

public partial class Movie {
	string m_rootLoadFunc;
	string m_rootPostLoadFunc;
	string m_rootUnloadFunc;
	string m_rootEnterFrameFunc;
	string m_loadFunc;
	string m_postLoadFunc;
	string m_unloadFunc;
	string m_enterFrameFunc;
}

public partial class LWF
{
	private string m_instanceIdString;
	private EventFunctions m_eventFunctions;
	private object m_luaState;

	public string instanceIdString {get {return m_instanceIdString;}}
	public object luaState {get {return m_luaState;}}

	public void InitLua()
	{
		m_eventFunctions = new EventFunctions();
		if (luaState == null)
			return;

		Lua.lua_State l = (Lua.lua_State)luaState;
		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			LuaLWF.open(l);
			Lua.lua_getglobal(l, "LWF");
			/* -1: LWF */
		}

		Lua.lua_getfield(l, -1, "LWF");
		/* -2: LWF */
		/* -1: LWF.LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 2);
			/* 0 */
			LuaLWF.open(l);
			Lua.lua_getglobal(l, "LWF");
			/* -1: LWF */
		} else {
			Lua.lua_pop(l, 1);
			/* -1: LWF */
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* -1: LWF */
			Lua.lua_newtable(l);
			/* -2: LWF */
			/* -1: table */
			Lua.lua_pushvalue(l, -1);
			/* -3: LWF */
			/* -2: table */
			/* -1: table */
			Lua.lua_setfield(l, -3, "Instances");
			/* -2: LWF */
			/* -1: table (LWF.Instances) */
		}
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		Lua.lua_newtable(l);
		/* -2: LWF.Instances */
		/* -1: table */
		Lua.lua_pushvalue(l, -1);
		/* -3: LWF.Instances */
		/* -2: table */
		/* -1: table */
		Lua.lua_setfield(l, -3, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_newtable(l);
		/* -2: LWF.Instances.<instanceId> */
		/* -1: table */
		Lua.lua_setfield(l, -2, "Handlers");
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_newtable(l);
		/* LWF.Instances.<instanceId>.Handlers = {} */
		/* -2: LWF.Instances.<instanceId> */
		/* -1: table */
		Lua.lua_setfield(l, -2, "Movies");
		/* LWF.Instances.<instanceId>.Movies = {} */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_pop(l, 1);
		/* 0 */

		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		Lua.lua_getfield(l, -1, "Script");
		/* -2: LWF */
		/* -1: LWF.Script */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, name);
		/* -2: LWF.Script */
		/* -1: LWF.Script.<name> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script.<name> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}

		string ev = "Event_";
		for (int eventId = 0; eventId < data.events.Length; ++eventId) {
			Lua.lua_getfield(l, -1, (ev +
				data.strings[data.events[eventId].stringId]));
			/* -2: LWF.Script.<name> */
			/* -1: function or nil: LWF.Script.<name>.Event_<eventname> */
			if (Lua.lua_isfunction(l, -1))
				m_eventFunctions[eventId] = true;
			Lua.lua_pop(l, 1);
			/* -1: LWF.Script.<name> */
		}
		Lua.lua_pop(l, 1);
		/* 0 */
	}

	public void DestroyLua()
	{
		if (luaState==null)
			return;

		Lua.lua_State l = (Lua.lua_State)luaState;
		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_pushnil(l);
		/* -2: LWF.Instances */
		/* -1: nil */
		Lua.lua_setfield(l, -2, instanceIdString);
		/* LWF.Instances.<instanceId> = nil */
		/* -1: LWF.Instances */
		Lua.lua_pop(l, 1);
		/* 0 */
		Luna_LWF_LWF.Destroy(l, this);
		LuaLWF.close(l);
	}

	public void DestroyMovieLua(Movie movie)
	{
		if (luaState==null)
			return;

		Lua.lua_State l = (Lua.lua_State)luaState;
		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, "Movies");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Movies */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Movies */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_pushnil(l);
		/* -2: LWF.Instances.<instanceId>.Movies */
		/* -1: nil */
		Lua.lua_setfield(l, -2, movie.iObjectId.ToString());
		/* LWF.Instances.<instanceId>.Movies.<iObjectId> = nil */
		/* -1: LWF.Instances.<instanceId>.Movies */
		Lua.lua_pop(l, 1);
		/* 0 */
		Luna_LWF_Movie.Destroy(l, movie);
		return;
	}

	public bool GetFieldLua(Movie movie, string key)
	{
		if (luaState==null)
			return false;

		Lua.lua_State l = (Lua.lua_State)luaState;
		/* 1: LWF_Movie instance */
		/* 2: key */

		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, "Movies");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Movies */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Movies */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, movie.iObjectId.ToString());
		/* -2: LWF.Instances.<instanceId>.Movies */
		/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
		Lua.lua_getfield(l, -1, key);
		/* -2: LWF.Instances.<instanceId>.Movies.<iObjectId> */
		/* -1: value */
		Lua.lua_remove(l, -2);
		/* -1: value */
		if (Lua.lua_isnil(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		return true;
	}

	public bool SetFieldLua(Movie movie, string key)
	{
		if (luaState==null)
			return false;

		Lua.lua_State l = (Lua.lua_State)luaState;
		/* 1: LWF_Movie instance */
		/* 2: key */
		/* 3: value */

		if (Lua.lua_isstring(l, 3)!=0 && movie.SearchText(key)) {
			movie.lwf.SetText(
				movie.GetFullName() + "." + key, Lua.lua_tostring(l, 3).ToString());
		}

		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, "Movies");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Movies */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Movies */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			return false;
		}
		string s = movie.iObjectId.ToString();
		Lua.lua_getfield(l, -1, s);
		/* -2: LWF.Instances.<instanceId>.Movies */
		/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* -1: LWF.Instances.<instanceId>.Movies */
			Lua.lua_newtable(l);
			/* -2: LWF.Instances.<instanceId>.Movies */
			/* -1: table */
			Lua.lua_pushvalue(l, -1);
			/* -3: LWF.Instances.<instanceId>.Movies */
			/* -2: table */
			/* -1: table */
			Lua.lua_setfield(l, -3, s);
			/* -2: LWF.Instances.<instanceId>.Movies */
			/* -1: table LWF.Instances.<instanceId>.Movies.<iObjectId> */
		}
		Lua.lua_pushvalue(l, 3);
		/* -2: LWF.Instances.<instanceId>.Movies.<iObjectId> */
		/* -1: value */
		Lua.lua_setfield(l, -2, key);
		/* -1: LWF.Instances.<instanceId>.Movies.<iObjectId> */
		Lua.lua_pop(l, 1);
		/* 0 */
		return true;
	}

	public string GetTextLua(Movie movie, string textName)
	{
		if (luaState==null)
			return "";

		Lua.lua_State l = (Lua.lua_State)luaState;
		if (!GetFieldLua(movie, textName) || Lua.lua_isstring(l, -1)==0) {
			/* -1: nil or not text */
			Lua.lua_pop(l, 1);
			return "";
		}
		/* -1: text */
		string text = Lua.lua_tostring(l, -1).ToString();
		Lua.lua_pop(l, 1);
		/* 0 */
		return text;
	}

	public bool PushHandlerLua(int handlerId)
	{
		if (luaState==null)
			return false;

		Lua.lua_State l = (Lua.lua_State)luaState;
		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return false;
		}
		Lua.lua_getfield(l, -1, "Handlers");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Handlers */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Handlers */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			return false;
		}
		Lua.lua_getfield(l, -1, handlerId.ToString());
		/* -2: LWF.Instances.<instanceId>.Handlers */
		/* -1: LWF.Instances.<instanceId>.Handlers.<handlerId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Handlers.<handlerId> */
		if (!Lua.lua_isfunction(l, -1)) {
			Lua.lua_pop(l, 0);
			/* 0 */
			return false;
		}
		/* -1: LWF.Instances.<instanceId>.Handlers.<handlerId>: function */
		return true;
	}

	public int AddEventHandlerLua()
	{
		if (luaState==null)
			return 0;

		Lua.lua_State l = (Lua.lua_State)luaState;
		string ev;
		int luaHandlerId;
		int handlerId;

		/* 1: LWF_LWF instance */
		/* 2: string */
		/* 3: function */
		ev = Lua.lua_tostring(l, 2).ToString();

		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, "Handlers");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Handlers */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Handlers */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_pushvalue(l, 3);
		/* -2: LWF.Instances.<instanceId>.Handlers */
		/* -1: function */
		luaHandlerId = GetEventOffset();
		Lua.lua_setfield(l, -2, luaHandlerId.ToString());
		/* LWF.Instances.<instanceId>.Handlers.<luaHandlerId> = function */
		/* -1: LWF.Instances.<instanceId>.Handlers */
		Lua.lua_pop(l, 1);
		/* 0 */

		handlerId = AddEventHandler(ev, (Movie m, Button b) => {
			if (!m.lwf.PushHandlerLua(luaHandlerId))
				return;

			/* -1: function */
			Lua.lua_State _l = (Lua.lua_State)m.lwf.luaState;
			Luna_LWF_Movie.push(_l, m, false);
			Luna_LWF_Button.push(_l, b, false);
			/* -3: function */
			/* -2: Movie */
			/* -1: Button */
			if (Lua.lua_pcall(l, 2, 0, 0)!=0)
				Lua.lua_pop(l, 1);
			/* 0 */
		});
		Lua.lua_pushnumber(l, handlerId);
		/* -1: handlerId */
		return 1;

	error:
		Lua.lua_pushnumber(l, -1);
		/* -1: -1 */
		return 1;
	}

	public int AddMovieEventHandlerLua()
	{
		if (luaState==null)
			return 0;

		Lua.lua_State l = (Lua.lua_State)luaState;
		string instanceName;
		MovieEventHandlerDictionary handlers = new MovieEventHandlerDictionary(){
			{"load", null},
			{"postLoad", null},
			{"unload", null},
			{"enterFrame", null},
			{"update", null},
			{"render", null}
		};

		int handlerId;

		/* 1: LWF_LWF instance */
		/* 2: instanceName:string */
		/* 3: table {key:string, handler:function} */
		instanceName = Lua.lua_tostring(l, 2).ToString();

		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, "Handlers");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Handlers */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Handlers */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}

		Lua.lua_pushnil(l);
		/* -2: LWF.Instances.<instanceId>.Handlers */
		/* -1: nil */
		while (Lua.lua_next(l, 3)!=0) {
			/* -3: LWF.Instances.<instanceId>.Handlers */
			/* -2: key: eventName string */
			/* -1: value: handler function */
			string key = Lua.lua_tostring(l, -2).ToString();
			if (key != null && Lua.lua_isfunction(l, -1)) {
				int luaHandlerId = GetEventOffset();
				handlers[key] = (Movie a) => {
					if (!a.lwf.PushHandlerLua(luaHandlerId))
						return;

					/* -1: function */
					Lua.lua_State ls = (Lua.lua_State)a.lwf.luaState;
					Luna_LWF_Movie.push(ls, a, false);
					/* -2: function */
					/* -1: Movie or Button */
					if (Lua.lua_pcall(ls, 1, 0, 0)!=0)
						Lua.lua_pop(ls, 1);
					/* 0 */
				};
				Lua.lua_setfield(l, -3, luaHandlerId.ToString());
				/* LWF.Instances.<instanceId>.Handlers.<luaHandlerId> = function */
				/* -2: LWF.Instances.<instanceId>.Handlers */
				/* -1: key */
			} else {
				Lua.lua_pop(l, 1);
				/* -2: LWF.Instances.<instanceId>.Handlers */
				/* -1: key: eventName string */
			}
		}
		/* -1: LWF.Instances.<instanceId>.Handlers */
		Lua.lua_pop(l, 1);
		/* 0 */

		handlerId = AddMovieEventHandler(instanceName,
			handlers["load"], handlers["postLoad"],
			handlers["unload"], handlers["enterFrame"],
			handlers["update"], handlers["render"]);
		Lua.lua_pushnumber(l, handlerId);
		/* handlerId */
		return 1;

	error:
		Lua.lua_pushnumber(l, -1);
		/* -1: -1 */
		return 1;
	}

	int AddButtonEventHandlerLua()
	{
		if (luaState == null)
			return 0;

		Lua.lua_State l = (Lua.lua_State)luaState;
		string instanceName;
		ButtonEventHandlerDictionary handlers = new ButtonEventHandlerDictionary() {
			{"load", null},
			{"unload", null},
			{"enterFrame", null},
			{"update", null},
			{"render", null},
			{"press", null},
			{"release", null},
			{"rollOver", null},
			{"rollOut", null},
			{"keyPress", null}
		};
		int handlerId;

		/* 1: LWF_LWF instance */
		/* 2: instanceName:string */
		/* 3: table {key:string, handler:function} */
		instanceName = Lua.lua_tostring(l, 2).ToString();

		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, "Instances");
		/* -2: LWF */
		/* -1: LWF.Instances */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, instanceIdString);
		/* -2: LWF.Instances */
		/* -1: LWF.Instances.<instanceId> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}
		Lua.lua_getfield(l, -1, "Handlers");
		/* -2: LWF.Instances.<instanceId> */
		/* -1: LWF.Instances.<instanceId>.Handlers */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Instances.<instanceId>.Handlers */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			goto error;
		}

		Lua.lua_pushnil(l);
		/* -2: LWF.Instances.<instanceId>.Handlers */
		/* -1: nil */
		while (Lua.lua_next(l, 3)!=0) {
			/* -3: LWF.Instances.<instanceId>.Handlers */
			/* -2: key: eventName string */
			/* -1: value: handler function */
			string key = Lua.lua_tostring(l, -2).ToString();
			if (key != null && Lua.lua_isfunction(l, -1)) {
				int luaHandlerId = GetEventOffset();
				handlers[key] = (Button a) => {
					if (!a.lwf.PushHandlerLua(luaHandlerId))
						return;

					/* -1: function */
					Lua.lua_State ls = (Lua.lua_State)a.lwf.luaState;
					Luna_LWF_Button.push(ls, a, false);
					/* -2: function */
					/* -1: Movie or Button */
					if (Lua.lua_pcall(ls, 1, 0, 0)!=0)
						Lua.lua_pop(ls, 1);
					/* 0 */
				};

				Lua.lua_setfield(l, -3, luaHandlerId.ToString());
				/* LWF.Instances.<instanceId>.Handlers.<luaHandlerId> = function */
				/* -2: LWF.Instances.<instanceId>.Handlers */
				/* -1: key */
			} else {
				Lua.lua_pop(l, 2);
				/* 0 */
				goto error;
			}
		}
		/* -1: LWF.Instances.<instanceId>.Handlers */
		Lua.lua_pop(l, 1);
		/* 0 */

		handlerId = AddButtonEventHandler(instanceName,
			handlers["load"], handlers["unload"],
			handlers["enterFrame"], handlers["update"],
			handlers["render"], handlers["press"],
			handlers["release"], handlers["rollOver"],
			handlers["rollOut"]);
		Lua.lua_pushnumber(l, handlerId);
		/* handlerId */
		return 1;

	error:
		Lua.lua_pushnumber(l, -1);
		/* -1: -1 */
		return 1;
	}

	public int AttachMovieLua(Movie movie)
	{
		if (luaState==null)
			return 0;

		Lua.lua_State l = (Lua.lua_State)luaState;
		int args = Lua.lua_gettop(l);
		string linkageName;
		string attachName;
		int attachDepth = -1;
		bool reorder = false;
		MovieEventHandlerDictionary handlers = new MovieEventHandlerDictionary() {
			{"load",null},
			{"postLoad",null},
			{"unload",null},
			{"enterFrame",null},
			{"update",null},
			{"render",null}
		};

		Movie child;

		/* 1: LWF_Movie instance */
		/* 2: linkageName:string */
		/* 3: attachName:string */
		/* 4: table {key:string, handler:function} */
		/* 5: attachDepth:number (option) */
		/* 6: reorder:boolean (option) */
		linkageName = Lua.lua_tostring(l, 2).ToString();
		attachName = Lua.lua_tostring(l, 3).ToString();
		if (args >= 4) {
			Lua.lua_getglobal(l, "LWF");
			/* -1: LWF */
			if (!Lua.lua_istable(l, -1)) {
				Lua.lua_pop(l, 1);
				/* 0 */
				goto error;
			}
			Lua.lua_getfield(l, -1, "Instances");
			/* -2: LWF */
			/* -1: LWF.Instances */
			Lua.lua_remove(l, -2);
			/* -1: LWF.Instances */
			if (!Lua.lua_istable(l, -1)) {
				Lua.lua_pop(l, 1);
				/* 0 */
				goto error;
			}
			Lua.lua_getfield(l, -1, instanceIdString);
			/* -2: LWF.Instances */
			/* -1: LWF.Instances.<instanceId> */
			Lua.lua_remove(l, -2);
			/* -1: LWF.Instances.<instanceId> */
			if (!Lua.lua_istable(l, -1)) {
				Lua.lua_pop(l, 1);
				/* 0 */
				goto error;
			}
			Lua.lua_getfield(l, -1, "Handlers");
			/* -2: LWF.Instances.<instanceId> */
			/* -1: LWF.Instances.<instanceId>.Handlers */
			Lua.lua_remove(l, -2);
			/* -1: LWF.Instances.<instanceId>.Handlers */
			if (!Lua.lua_istable(l, -1)) {
				Lua.lua_pop(l, 1);
				/* 0 */
				goto error;
			}

			Lua.lua_pushnil(l);
			/* -2: LWF.Instances.<instanceId>.Handlers */
			/* -1: nil */
			while (Lua.lua_next(l, 4) != 0) {
				/* -3: LWF.Instances.<instanceId>.Handlers */
				/* -2: key: eventName string */
				/* -1: value: handler function */
				string key = Lua.lua_tostring(l, -2).ToString();
				if (key != null && Lua.lua_isfunction(l, -1)) {
					int luaHandlerId = GetEventOffset();
					handlers[key] = (Movie a) => {
						if (!a.lwf.PushHandlerLua(luaHandlerId))
							return;

						/* -1: function */
						Lua.lua_State ls = (Lua.lua_State)a.lwf.luaState;
						Luna_LWF_Movie.push(ls, a, false);
						/* -2: function */
						/* -1: Movie or Button */
						if (Lua.lua_pcall(ls, 1, 0, 0)!=0)
							Lua.lua_pop(ls, 1);
						/* 0 */
					};


					Lua.lua_setfield(l, -3, luaHandlerId.ToString());
					/* LWF.Instances.<instanceId>.Handlers.
						<luaHandlerId> = function */
					/* -2: LWF.Instances.<instanceId>.Handlers */
					/* -1: key */
				} else {
					Lua.lua_pop(l, 1);
					/* -2: LWF.Instances.<instanceId>.Handlers */
					/* -1: key: eventName string */
				}
			}
			/* -1: LWF.Instances.<instanceId>.Handlers */
			Lua.lua_pop(l, 1);
			/* 0 */
		}
		if (args >= 5)
			attachDepth = (int)Lua.lua_tonumber(l, 5);
		if (args >= 6)
			reorder = Lua.lua_toboolean(l, 6)!=0;

		child = movie.AttachMovie(
			linkageName,
			attachName,
			attachDepth,
			reorder,
			handlers["load"],
			handlers["postLoad"],
			handlers["unload"],
			handlers["enterFrame"],
			handlers["update"],
			handlers["render"]);
		Luna_LWF_Movie.push(l, child, false);
		/* -1: LWF_Movie child */
		return 1;

	error:
		Lua.lua_pushnil(l);
		/* -1: nil */
		return 1;
	}

	public void GetFunctionsLua(int movieId, out string loadFunc,
		out string postLoadFunc, out string unloadFunc, out string enterFrameFunc,
		bool forRoot)
	{
		loadFunc = postLoadFunc = unloadFunc = enterFrameFunc = null;
		if (luaState==null)
			return;

		string linkageName = GetMovieLinkageName(movieId);
		if (linkageName == string.Empty)
			return;

		Lua.lua_State l = (Lua.lua_State)luaState;
		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, "Script");
		/* -2: LWF */
		/* -1: LWF.Script */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, name);
		/* -2: LWF.Script */
		/* -1: LWF.Script.<name> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script.<name> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}

		string func = forRoot ? "Load" : linkageName + "_load";
		Lua.lua_getfield(l, -1, func);
		/* -2: LWF.Script.<name> */
		/* -1: function or nil */
		if (Lua.lua_isfunction(l, -1))
			loadFunc = func;
		Lua.lua_pop(l, 1);
		/* -1: LWF.Script.<name> */

		func = forRoot ? "PostLoad" : linkageName + "_postLoad";
		Lua.lua_getfield(l, -1, func);
		/* -2: LWF.Script.<name> */
		/* -1: function or nil */
		if (Lua.lua_isfunction(l, -1))
			postLoadFunc = func;
		Lua.lua_pop(l, 1);
		/* -1: LWF.Script.<name> */

		func = forRoot ? "Unload" : linkageName + "_unload";
		Lua.lua_getfield(l, -1, func);
		/* -2: LWF.Script.<name> */
		/* -1: function or nil */
		if (Lua.lua_isfunction(l, -1))
			unloadFunc = func;
		Lua.lua_pop(l, 1);
		/* -1: LWF.Script.<name> */

		func = forRoot ? "EnterFrame" : linkageName + "_enterFrame";
		Lua.lua_getfield(l, -1, func);
		/* -2: LWF.Script.<name> */
		/* -1: function or nil */
		if (Lua.lua_isfunction(l, -1))
			enterFrameFunc = func;
		Lua.lua_pop(l, 2);
		/* 0 */
	}

	public void CallFunctionLua(string function, Movie movie)
	{
		if (luaState==null)
			return;

		Lua.lua_State l = (Lua.lua_State)luaState;
		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, "Script");
		/* -2: LWF */
		/* -1: LWF.Script */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, name);
		/* -2: LWF.Script */
		/* -1: LWF.Script.<name> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script.<name> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, function);
		/* -2: LWF.Script.<name> */
		/* -1: LWF.Script.<name>.<function> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script.<name>.<function> */
		if (!Lua.lua_isfunction(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Luna_LWF_Movie.push(l, movie, false);
		/* -2: LWF.Script.<name>.<function> */
		/* -1: LWF_Movie instance */
		if (Lua.lua_pcall(l, 1, 0, 0)!=0)
			Lua.lua_pop(l, 1);
		/* 0 */
	}

	public void CallEventFunctionLua(int eventId, Movie movie, Button button)
	{
		if (luaState==null)
			return;

		if (!m_eventFunctions.ContainsKey(eventId))
			return;

		Lua.lua_State l = (Lua.lua_State)luaState;
		Lua.lua_getglobal(l, "LWF");
		/* -1: LWF */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, "Script");
		/* -2: LWF */
		/* -1: LWF.Script */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Lua.lua_getfield(l, -1, name);
		/* -2: LWF.Script */
		/* -1: LWF.Script.<name> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script.<name> */
		if (!Lua.lua_istable(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		string ev = "Event_";
		Lua.lua_getfield(l, -1,
			(ev + data.strings[data.events[eventId].stringId]));
		/* -2: LWF.Script.<name> */
		/* -1: LWF.Script.<name>.Event_<eventName> */
		Lua.lua_remove(l, -2);
		/* -1: LWF.Script.<name>.Event_<eventName>*/

		if (!Lua.lua_isfunction(l, -1)) {
			Lua.lua_pop(l, 1);
			/* 0 */
			return;
		}
		Luna_LWF_Movie.push(l, movie, false);
		/* -2: LWF.Script.<name>.Event_<eventName> */
		/* -1: LWF_Movie instance */
		Luna_LWF_Button.push(l, button, false);
		/* -3: LWF.Script.<name>.Event_<eventName> */
		/* -2: LWF_Movie instance */
		/* -1: LWF_Button instance */
		if (Lua.lua_pcall(l, 2, 0, 0)!=0)
			Lua.lua_pop(l, 1);
		/* 0 */
	}
}

}	// namespace LWF

#endif
