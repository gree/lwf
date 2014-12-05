#if LWF_USE_LUA

using System;
using System.Collections.Generic;
using KopiLua;

public class LunaTraits_LWF_LWF
{
	public class RegType
	{
		public RegType(Lua.CharPtr name, Lua.lua_CFunction func)
		{
			this.name = name;
			this.mfunc = func;
		}
		public Lua.CharPtr name;
		public Lua.lua_CFunction mfunc;
	}

	public static Lua.CharPtr className = "LWF_LWF";
	public static int uniqueID = 7105034;
	public static RegType[] methods = new RegType[]
	{
        new RegType("setText", impl_LunaTraits_LWF_LWF._bind_setText),
        new RegType("getText", impl_LunaTraits_LWF_LWF._bind_getText),
        new RegType("playMovie", impl_LunaTraits_LWF_LWF._bind_playMovie),
        new RegType("stopMovie", impl_LunaTraits_LWF_LWF._bind_stopMovie),
        new RegType("nextFrameMovie", impl_LunaTraits_LWF_LWF._bind_nextFrameMovie),
        new RegType("prevFrameMovie", impl_LunaTraits_LWF_LWF._bind_prevFrameMovie),
        new RegType("setVisibleMovie", impl_LunaTraits_LWF_LWF._bind_setVisibleMovie),
        new RegType("gotoAndStopMovie", impl_LunaTraits_LWF_LWF._bind_gotoAndStopMovie),
        new RegType("gotoAndPlayMovie", impl_LunaTraits_LWF_LWF._bind_gotoAndPlayMovie),
        new RegType("moveMovie", impl_LunaTraits_LWF_LWF._bind_moveMovie),
        new RegType("moveToMovie", impl_LunaTraits_LWF_LWF._bind_moveToMovie),
        new RegType("rotateMovie", impl_LunaTraits_LWF_LWF._bind_rotateMovie),
        new RegType("rotateToMovie", impl_LunaTraits_LWF_LWF._bind_rotateToMovie),
        new RegType("scaleMovie", impl_LunaTraits_LWF_LWF._bind_scaleMovie),
        new RegType("scaleToMovie", impl_LunaTraits_LWF_LWF._bind_scaleToMovie),
        new RegType("setAlphaMovie", impl_LunaTraits_LWF_LWF._bind_setAlphaMovie),
        new RegType("setColorTransformMovie", impl_LunaTraits_LWF_LWF._bind_setColorTransformMovie),
        new RegType("removeEventListener", impl_LunaTraits_LWF_LWF._bind_removeEventListener),
        new RegType("clearEventListener", impl_LunaTraits_LWF_LWF._bind_clearEventListener),
        new RegType("removeMovieEventListener", impl_LunaTraits_LWF_LWF._bind_removeMovieEventListener),
        new RegType("clearMovieEventListener", impl_LunaTraits_LWF_LWF._bind_clearMovieEventListener),
        new RegType("removeButtonEventListener", impl_LunaTraits_LWF_LWF._bind_removeButtonEventListener),
        new RegType("clearButtonEventListener", impl_LunaTraits_LWF_LWF._bind_clearButtonEventListener),
        new RegType("getName", impl_LunaTraits_LWF_LWF._bind_getName),
        new RegType("getRootMovie", impl_LunaTraits_LWF_LWF._bind_getRootMovie),
        new RegType("get_root", impl_LunaTraits_LWF_LWF._bind_get_root),
        new RegType("getWidth", impl_LunaTraits_LWF_LWF._bind_getWidth),
        new RegType("getHeight", impl_LunaTraits_LWF_LWF._bind_getHeight),
        new RegType("getPointX", impl_LunaTraits_LWF_LWF._bind_getPointX),
        new RegType("getPointY", impl_LunaTraits_LWF_LWF._bind_getPointY),
        new RegType("addEventListener", impl_LunaTraits_LWF_LWF.addEventListener),
        new RegType("addMovieEventListener", impl_LunaTraits_LWF_LWF.addMovieEventListener),
        new RegType("addButtonEventListener", impl_LunaTraits_LWF_LWF.addButtonEventListener),

		new RegType("__index", impl_LunaTraits_LWF_LWF.__index),
		new RegType("__newindex", impl_LunaTraits_LWF_LWF.__newindex),
		new RegType(null,null)
	};

	public static LWF.LWF _bind_ctor(Lua.lua_State L)
	{
		Luna.print("undefined contructor of LWF.LWF called\n");
		return null;
	}

	public static void _bind_dtor(LWF.LWF obj)
	{
	}

	public static Dictionary<string, Lua.lua_CFunction> properties = new Dictionary<string, Lua.lua_CFunction>();
	public static Dictionary<string, Lua.lua_CFunction> write_properties = new Dictionary<string, Lua.lua_CFunction>();
}

public class impl_LunaTraits_LWF_LWF
{
	public static string getName(LWF.LWF o){return o.name;}
	public static float getWidth(LWF.LWF o){return o.width;}
	public static float getHeight(LWF.LWF o){return o.height;}
	public static float getPointX(LWF.LWF o){return o.pointX;}
	public static float getPointY(LWF.LWF o){return o.pointY;}

	public static int _bind_getRootMovie(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 1 ||
				Luna.get_uniqueid(L, 1) != LunaTraits_LWF_LWF.uniqueID) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.LWF.rootMovie");
		}

		LWF.LWF a = Luna_LWF_LWF.check(L, 1);
		Luna_LWF_Movie.push(L, a.rootMovie, false);
		return 1;
	}

	public static int _bind_get_root(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 1 ||
				Luna.get_uniqueid(L, 1) != LunaTraits_LWF_LWF.uniqueID) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.LWF._root");
		}

		LWF.LWF a = Luna_LWF_LWF.check(L, 1);
		Luna_LWF_Movie.push(L, a._root, false);
		return 1;
	}

	public static int addEventListener(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 3 ||
				Luna.get_uniqueid(L, 1) != LunaTraits_LWF_LWF.uniqueID ||
				Lua.lua_isstring(L, 2) == 0 || !Lua.lua_isfunction(L, 3)) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.addEventListener");
		}

		LWF.LWF a = Luna_LWF_LWF.check(L, 1);
		return a.AddEventHandlerLua();
	}

	public static int addMovieEventListener(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 3 ||
				Luna.get_uniqueid(L, 1) != LunaTraits_LWF_LWF.uniqueID ||
				Lua.lua_isstring(L, 2)==0 || !Lua.lua_istable(L, 3)) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.addMovieEventListener");
		}

		LWF.LWF a = Luna_LWF_LWF.check(L, 1);
		return a.AddMovieEventHandlerLua();
	}

	public static int addButtonEventListener(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 3 ||
				Luna.get_uniqueid(L, 1) != LunaTraits_LWF_LWF.uniqueID ||
				Lua.lua_isstring(L, 2)==0 || !Lua.lua_istable(L, 3)) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.addButtonEventListener");
		}

		LWF.LWF a = Luna_LWF_LWF.check(L, 1);
		return a.AddButtonEventHandlerLua();
	}



  public static int _bind_setText(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isstring(L,3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setText(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string textName=Lua.lua_tostring(L,2).ToString();
		string text=Lua.lua_tostring(L,3).ToString();
	try {
		self.SetText(textName, text);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_getText(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getText(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string textName=Lua.lua_tostring(L,2).ToString();
	try {
		string ret=self.GetText(textName);
		Lua.lua_pushstring(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_playMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:playMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
	try {
		self.PlayMovie(instanceName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_stopMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:stopMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
	try {
		self.StopMovie(instanceName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_nextFrameMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:nextFrameMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
	try {
		self.NextFrameMovie(instanceName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_prevFrameMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:prevFrameMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
	try {
		self.PrevFrameMovie(instanceName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setVisibleMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || !Lua.lua_isboolean(L,3)) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setVisibleMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		bool visible=(Lua.lua_toboolean(L,3) != 0);
	try {
		self.SetVisibleMovie(instanceName, visible);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndStopMovie_overload_1(Lua.lua_State L)
  {

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		int frameNo=(int)Lua.lua_tonumber(L,3);
	try {
		self.GotoAndStopMovie(instanceName, frameNo);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndStopMovie_overload_2(Lua.lua_State L)
  {

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		string label=Lua.lua_tostring(L,3).ToString();
	try {
		self.GotoAndStopMovie(instanceName, label);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndPlayMovie_overload_1(Lua.lua_State L)
  {

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		int frameNo=(int)Lua.lua_tonumber(L,3);
	try {
		self.GotoAndPlayMovie(instanceName, frameNo);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndPlayMovie_overload_2(Lua.lua_State L)
  {

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		string label=Lua.lua_tostring(L,3).ToString();
	try {
		self.GotoAndPlayMovie(instanceName, label);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_moveMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=4
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0
            || Lua.lua_isnumber(L, 4)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:moveMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float vx=(float)Lua.lua_tonumber(L,3);
		float vy=(float)Lua.lua_tonumber(L,4);
	try {
		self.MoveMovie(instanceName, vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_moveToMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=4
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0
            || Lua.lua_isnumber(L, 4)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:moveToMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float vx=(float)Lua.lua_tonumber(L,3);
		float vy=(float)Lua.lua_tonumber(L,4);
	try {
		self.MoveToMovie(instanceName, vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_rotateMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:rotateMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float degree=(float)Lua.lua_tonumber(L,3);
	try {
		self.RotateMovie(instanceName, degree);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_rotateToMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:rotateToMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float degree=(float)Lua.lua_tonumber(L,3);
	try {
		self.RotateToMovie(instanceName, degree);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_scaleMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=4
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0
            || Lua.lua_isnumber(L, 4)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:scaleMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float vx=(float)Lua.lua_tonumber(L,3);
		float vy=(float)Lua.lua_tonumber(L,4);
	try {
		self.ScaleMovie(instanceName, vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_scaleToMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=4
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0
            || Lua.lua_isnumber(L, 4)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:scaleToMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float vx=(float)Lua.lua_tonumber(L,3);
		float vy=(float)Lua.lua_tonumber(L,4);
	try {
		self.ScaleToMovie(instanceName, vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setAlphaMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setAlphaMovie(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float v=(float)Lua.lua_tonumber(L,3);
	try {
		self.SetAlphaMovie(instanceName, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setColorTransformMovie_overload_1(Lua.lua_State L)
  {

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float vr=(float)Lua.lua_tonumber(L,3);
		float vg=(float)Lua.lua_tonumber(L,4);
		float vb=(float)Lua.lua_tonumber(L,5);
		float va=(float)Lua.lua_tonumber(L,6);
	try {
		self.SetColorTransformMovie(instanceName, vr, vg, vb, va);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setColorTransformMovie_overload_2(Lua.lua_State L)
  {

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		float vr=(float)Lua.lua_tonumber(L,3);
		float vg=(float)Lua.lua_tonumber(L,4);
		float vb=(float)Lua.lua_tonumber(L,5);
		float va=(float)Lua.lua_tonumber(L,6);
		float ar=(float)Lua.lua_tonumber(L,7);
		float ag=(float)Lua.lua_tonumber(L,8);
		float ab=(float)Lua.lua_tonumber(L,9);
		float aa=(float)Lua.lua_tonumber(L,10);
	try {
		self.SetColorTransformMovie(instanceName, vr, vg, vb, va, ar, ag, ab, aa);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_removeEventListener(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:removeEventListener(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string eventName=Lua.lua_tostring(L,2).ToString();
		int id=(int)Lua.lua_tonumber(L,3);
	try {
		self.RemoveEventHandler(eventName, id);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_clearEventListener(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:clearEventListener(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string eventName=Lua.lua_tostring(L,2).ToString();
	try {
		self.ClearEventHandler(eventName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_removeMovieEventListener(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:removeMovieEventListener(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		int id=(int)Lua.lua_tonumber(L,3);
	try {
		self.RemoveMovieEventHandler(instanceName, id);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_clearMovieEventListener(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:clearMovieEventListener(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
	try {
		self.ClearMovieEventHandler(instanceName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_removeButtonEventListener(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:removeButtonEventListener(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
		int id=(int)Lua.lua_tonumber(L,3);
	try {
		self.RemoveButtonEventHandler(instanceName, id);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_clearButtonEventListener(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=7105034 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:clearButtonEventListener(LWF.LWF self)"); }

	LWF.LWF self=Luna_LWF_LWF.check(L,1);
		string instanceName=Lua.lua_tostring(L,2).ToString();
	try {
		self.ClearButtonEventHandler(instanceName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndStopMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)==3
            && Luna.get_uniqueid(L,1)==7105034 
            && Lua.lua_isstring(L,2)==1
            && Lua.lua_isnumber(L, 3)==1) return _bind_gotoAndStopMovie_overload_1(L);
	if (Lua.lua_gettop(L)==3
            && Luna.get_uniqueid(L,1)==7105034 
            && Lua.lua_isstring(L,2)==1
            && Lua.lua_isstring(L,3)==1) return _bind_gotoAndStopMovie_overload_2(L);
	Lua.luaL_error(L, "gotoAndStopMovie cannot find overloads.");

	return 0;
  }
  public static int _bind_gotoAndPlayMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)==3
            && Luna.get_uniqueid(L,1)==7105034 
            && Lua.lua_isstring(L,2)==1
            && Lua.lua_isnumber(L, 3)==1) return _bind_gotoAndPlayMovie_overload_1(L);
	if (Lua.lua_gettop(L)==3
            && Luna.get_uniqueid(L,1)==7105034 
            && Lua.lua_isstring(L,2)==1
            && Lua.lua_isstring(L,3)==1) return _bind_gotoAndPlayMovie_overload_2(L);
	Lua.luaL_error(L, "gotoAndPlayMovie cannot find overloads.");

	return 0;
  }
  public static int _bind_setColorTransformMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)==6
            && Luna.get_uniqueid(L,1)==7105034 
            && Lua.lua_isstring(L,2)==1
            && Lua.lua_isnumber(L, 3)==1
            && Lua.lua_isnumber(L, 4)==1
            && Lua.lua_isnumber(L, 5)==1
            && Lua.lua_isnumber(L, 6)==1) return _bind_setColorTransformMovie_overload_1(L);
	if (Lua.lua_gettop(L)==10
            && Luna.get_uniqueid(L,1)==7105034 
            && Lua.lua_isstring(L,2)==1
            && Lua.lua_isnumber(L, 3)==1
            && Lua.lua_isnumber(L, 4)==1
            && Lua.lua_isnumber(L, 5)==1
            && Lua.lua_isnumber(L, 6)==1
            && Lua.lua_isnumber(L, 7)==1
            && Lua.lua_isnumber(L, 8)==1
            && Lua.lua_isnumber(L, 9)==1
            && Lua.lua_isnumber(L, 10)==1) return _bind_setColorTransformMovie_overload_2(L);
	Lua.luaL_error(L, "setColorTransformMovie cannot find overloads.");

	return 0;
  }
  public static int _bind_getName(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=7105034 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getName(LWF.LWF self ...)"); }
		LWF.LWF o=Luna_LWF_LWF.check(L,1);
	try {
		string ret=getName(o);
		Lua.lua_pushstring(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getWidth(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=7105034 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getWidth(LWF.LWF self ...)"); }
		LWF.LWF o=Luna_LWF_LWF.check(L,1);
	try {
		float ret=getWidth(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getHeight(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=7105034 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getHeight(LWF.LWF self ...)"); }
		LWF.LWF o=Luna_LWF_LWF.check(L,1);
	try {
		float ret=getHeight(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getPointX(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=7105034 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getPointX(LWF.LWF self ...)"); }
		LWF.LWF o=Luna_LWF_LWF.check(L,1);
	try {
		float ret=getPointX(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getPointY(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=7105034 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getPointY(LWF.LWF self ...)"); }
		LWF.LWF o=Luna_LWF_LWF.check(L,1);
	try {
		float ret=getPointY(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }


	public static void luna_init_hashmap()
	{
        LunaTraits_LWF_LWF.properties["name"]=_bind_getName;
        LunaTraits_LWF_LWF.properties["rootMovie"]=_bind_getRootMovie;
        LunaTraits_LWF_LWF.properties["_root"]=_bind_get_root;
        LunaTraits_LWF_LWF.properties["width"]=_bind_getWidth;
        LunaTraits_LWF_LWF.properties["height"]=_bind_getHeight;
        LunaTraits_LWF_LWF.properties["pointX"]=_bind_getPointX;
        LunaTraits_LWF_LWF.properties["pointY"]=_bind_getPointY;

	}

	public static void luna_init_write_hashmap()
	{

	}

	public static int __index(Lua.lua_State L)
	{

		{
			Lua.lua_CFunction fnc = null;
			if (LunaTraits_LWF_LWF.properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
			{
				Lua.lua_pop(L,1); // remove self
				return fnc(L);
			}
		}

		int mt=Lua.lua_getmetatable(L, 1);
		if(mt==0) Lua.luaL_error(L,"__index");//end
		Lua.lua_pushstring(L, Lua.lua_tostring(L,2));
		Lua.lua_rawget(L, -2);
		return 1;
	}

	public static int __newindex(Lua.lua_State L)
	{
		Lua.lua_CFunction fnc = null;
		if (LunaTraits_LWF_LWF.write_properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
		{
			Lua.lua_insert(L,2); // swap key and value
			Lua.lua_settop(L,2); // delete key
			return fnc(L);
		}

		Lua.luaL_error(L,"__newindex doesn't allow defining non-property member");
		return 0;
	}
}

class Luna_LWF_LWF
{
	private static int idOffset = 0;
	private static Dictionary<Lua.lua_State, Dictionary<int, LWF.LWF>> objects = new Dictionary<Lua.lua_State, Dictionary<int, LWF.LWF>>();
	private static Dictionary<Lua.lua_State, Dictionary<LWF.LWF, int>> objectIdentifiers = new Dictionary<Lua.lua_State, Dictionary<LWF.LWF, int>>();

	public static void set(Lua.lua_State L, int table_index, Lua.CharPtr key)
	{
		Lua.lua_pushstring(L, key);
		Lua.lua_insert(L, -2);  // swap value and key
		Lua.lua_settable(L, table_index);
	}

	public static void Register(Lua.lua_State L)
	{
		objects.Add(L, new Dictionary<int, LWF.LWF>());
		objectIdentifiers.Add(L, new Dictionary<LWF.LWF, int>());
		int methods;
		Lua.lua_newtable(L);
		methods = Lua.lua_gettop(L);
		// use a single table
		// sometimes more convenient
		int metatable=methods;

		Lua.luaL_loadstring(L, "if not __luna then __luna={} end");
		Lua.lua_pcall(L, 0, Lua.LUA_MULTRET, 0);

		Lua.lua_pushstring(L, "__luna");
		Lua.lua_gettable(L, Lua.LUA_GLOBALSINDEX);
		// unlike original luna class, this class uses the same table for methods and metatable
		// store methods table in __luna global table so that
		// scripts can add functions written in Lua.
		Lua.lua_pushstring(L, LunaTraits_LWF_LWF.className);
		Lua.lua_pushvalue(L, methods);
		Lua.lua_settable(L, -3); // __luna[className]=methods

		Lua.lua_pushliteral(L, "__index");
		Lua.lua_pushvalue(L, methods);
		Lua.lua_settable(L, metatable); // metatable.__index=methods

		/* Lua.lua_pushliteral(L, "__tostring"); */
		/* Lua.lua_pushcfunction(L, tostring_T); */
		/* Lua.lua_settable(L, metatable);// metatable.__tostring=tostring_T */

		Lua.lua_pushliteral(L, "__gc");
		Lua.lua_pushcfunction(L, gc_T);
		Lua.lua_settable(L, metatable);

		/*if (false)
		{
			// ctor supports only classname:new
			Lua.lua_pushliteral(L, "new");
			Lua.lua_pushcfunction(L, new_T);
			Lua.lua_settable(L, methods);       // add new_T to metatable table
		}
		else
		*/
		{
			// ctor supports both classname:new(...) and classname(...)
			// very slight memory and performance overhead, so
			// no reason to support only one
			Lua.lua_newtable(L);                // mt for method table
			{
				Lua.lua_pushcfunction(L, new_T);
				Lua.lua_pushvalue(L, -1);           // dup new_T function
				set(L, methods, "new");         // add new_T to method table
			}
			set(L, -3, "__call");           // mt.__call = new_T
			Lua.lua_setmetatable(L, methods);
		}

		// fill method table with metatable from class T
		for (int i = 0;; i++)
		{
			LunaTraits_LWF_LWF.RegType l = LunaTraits_LWF_LWF.methods[i];
			if (l.name == null) break;
			Lua.lua_pushstring(L, l.name);
			Lua.lua_pushcclosure(L, l.mfunc, 0);
			Lua.lua_settable(L, methods);
		}

		Lua.lua_pop(L, 2);  // drop methods and __luna
	}

	public static void Unregister(Lua.lua_State L)
	{
		objects.Remove(L);
		objectIdentifiers.Remove(L);
	}

	public static void Destroy(Lua.lua_State L, LWF.LWF obj)
	{

		int objectId = -1;
		if (objectIdentifiers[L].TryGetValue(obj, out objectId))
		{
			objectIdentifiers[L].Remove(obj);
			objects[L].Remove(objectId);
		}
	}

	static public LWF.LWF check(Lua.lua_State L, int narg)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L,narg);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, narg, LunaTraits_LWF_LWF.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		if(ud.TypeId !=LunaTraits_LWF_LWF.uniqueID) // type checking with almost no overhead
		{
			Luna.print(String.Format("ud.uid: {0} != interface::uid : {1}\n", ud.TypeId, LunaTraits_LWF_LWF.uniqueID));
			Lua.luaL_typerror(L, narg, LunaTraits_LWF_LWF.className);
		}
		LWF.LWF obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return null;
		return obj;
	}

	// use lunaStack::push if possible.
	public static void push(Lua.lua_State L, LWF.LWF obj, bool gc, Lua.CharPtr metatable=null)
	{
		if (obj == null) {
			Lua.lua_pushnil(L);
			return;
		}

		int objectId = -1;
		if (!objectIdentifiers[L].TryGetValue(obj, out objectId))
		{
			objectId = idOffset ++;
			objectIdentifiers[L].Add(obj, objectId);
			objects[L].Add(objectId, obj);
		}

		if (metatable == null)
				metatable = LunaTraits_LWF_LWF.className;
		Lua.lua_pushstring(L,"__luna");
		Lua.lua_gettable(L, Lua.LUA_GLOBALSINDEX);
		int __luna= Lua.lua_gettop(L);

		Luna.userdataType ud = new Luna.userdataType(
			objectId:objectId,  // store object in userdata
			gc:gc,   // collect garbage
			has_env:false, // does this userdata has a table attached to it?
			typeId:LunaTraits_LWF_LWF.uniqueID
		);

		ud.ToBytes((byte[])Lua.lua_newuserdata(L, Luna.userdataType.Size));

		Lua.lua_pushstring(L, metatable);
		Lua.lua_gettable(L, __luna);
		Lua.lua_setmetatable(L, -2);
		//Luna.printStack(L);
		Lua.lua_insert(L, -2);  // swap __luna and userdata
		Lua.lua_pop(L,1);
	}

	private Luna_LWF_LWF(){}  // hide default constructor

	// create a new T object and
	// push onto the Lua stack a userdata containing a pointer to T object
	private static int new_T(Lua.lua_State L)
	{
		Lua.lua_remove(L, 1);   // use classname:new(), instead of classname.new()
		LWF.LWF obj = LunaTraits_LWF_LWF._bind_ctor(L);  // call constructor for T objects
		push(L,obj,true);
		return 1;  // userdata containing pointer to T object
	}

	// garbage collection metamethod
	private static int gc_T(Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_LWF.className); }
		Luna.userdataType ud = new Luna.userdataType(d);

		LWF.LWF obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		if (ud.Gc) {
			LunaTraits_LWF_LWF._bind_dtor(obj);  // call constructor for T objects
			Destroy(L, obj);
		}

		return 0;
	}

	private static int tostring_T (Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_LWF.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		LWF.LWF obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		char[] buff = obj.ToString().ToCharArray(0,32);
		Lua.lua_pushfstring(L, "%s (%s)", new object[] {LunaTraits_LWF_LWF.className, buff});
		return 1;
	}
}

#endif
