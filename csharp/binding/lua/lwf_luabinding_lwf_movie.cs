#if LWF_USE_LUA

using System;
using System.Collections.Generic;
using KopiLua;

public class LunaTraits_LWF_Movie
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

	public static Lua.CharPtr className = "LWF_Movie";
	public static int uniqueID = 29625181;
	public static RegType[] methods = new RegType[]
	{
        new RegType("getFullName", impl_LunaTraits_LWF_Movie._bind_getFullName),
        new RegType("globalToLocal", impl_LunaTraits_LWF_Movie._bind_globalToLocal),
        new RegType("localToGlobal", impl_LunaTraits_LWF_Movie._bind_localToGlobal),
        new RegType("play", impl_LunaTraits_LWF_Movie._bind_play),
        new RegType("stop", impl_LunaTraits_LWF_Movie._bind_stop),
        new RegType("nextFrame", impl_LunaTraits_LWF_Movie._bind_nextFrame),
        new RegType("prevFrame", impl_LunaTraits_LWF_Movie._bind_prevFrame),
        new RegType("gotoFrame", impl_LunaTraits_LWF_Movie._bind_gotoFrame),
        new RegType("gotoAndStop", impl_LunaTraits_LWF_Movie._bind_gotoAndStop),
        new RegType("gotoAndPlay", impl_LunaTraits_LWF_Movie._bind_gotoAndPlay),
        new RegType("move", impl_LunaTraits_LWF_Movie._bind_move),
        new RegType("moveTo", impl_LunaTraits_LWF_Movie._bind_moveTo),
        new RegType("rotate", impl_LunaTraits_LWF_Movie._bind_rotate),
        new RegType("rotateTo", impl_LunaTraits_LWF_Movie._bind_rotateTo),
        new RegType("scale", impl_LunaTraits_LWF_Movie._bind_scale),
        new RegType("scaleTo", impl_LunaTraits_LWF_Movie._bind_scaleTo),
        new RegType("removeEventListener", impl_LunaTraits_LWF_Movie._bind_removeEventListener),
        new RegType("clearEventListener", impl_LunaTraits_LWF_Movie._bind_clearEventListener),
        new RegType("swapAttachedMovieDepth", impl_LunaTraits_LWF_Movie._bind_swapAttachedMovieDepth),
        new RegType("detachMovie", impl_LunaTraits_LWF_Movie._bind_detachMovie),
        new RegType("detachFromParent", impl_LunaTraits_LWF_Movie._bind_detachFromParent),
        new RegType("detachLWF", impl_LunaTraits_LWF_Movie._bind_detachLWF),
        new RegType("detachAllLWFs", impl_LunaTraits_LWF_Movie._bind_detachAllLWFs),
        new RegType("removeMovieClip", impl_LunaTraits_LWF_Movie._bind_removeMovieClip),
        new RegType("attachBitmap", impl_LunaTraits_LWF_Movie._bind_attachBitmap),
        new RegType("getAttachedBitmap", impl_LunaTraits_LWF_Movie._bind_getAttachedBitmap),
        new RegType("swapAttachedBitmapDepth", impl_LunaTraits_LWF_Movie._bind_swapAttachedBitmapDepth),
        new RegType("detachBitmap", impl_LunaTraits_LWF_Movie._bind_detachBitmap),
        new RegType("getName", impl_LunaTraits_LWF_Movie._bind_getName),
        new RegType("getParent", impl_LunaTraits_LWF_Movie._bind_getParent),
        new RegType("getCurrentFrame", impl_LunaTraits_LWF_Movie._bind_getCurrentFrame),
        new RegType("getCurrentLabel", impl_LunaTraits_LWF_Movie._bind_getCurrentLabel),
        new RegType("getCurrentLabels", impl_LunaTraits_LWF_Movie._bind_getCurrentLabels),
        new RegType("getTotalFrames", impl_LunaTraits_LWF_Movie._bind_getTotalFrames),
        new RegType("getVisible", impl_LunaTraits_LWF_Movie._bind_getVisible),
        new RegType("getX", impl_LunaTraits_LWF_Movie._bind_getX),
        new RegType("getY", impl_LunaTraits_LWF_Movie._bind_getY),
        new RegType("getScaleX", impl_LunaTraits_LWF_Movie._bind_getScaleX),
        new RegType("getScaleY", impl_LunaTraits_LWF_Movie._bind_getScaleY),
        new RegType("getRotation", impl_LunaTraits_LWF_Movie._bind_getRotation),
        new RegType("getAlpha", impl_LunaTraits_LWF_Movie._bind_getAlpha),
        new RegType("getRed", impl_LunaTraits_LWF_Movie._bind_getRed),
        new RegType("getGreen", impl_LunaTraits_LWF_Movie._bind_getGreen),
        new RegType("getBlue", impl_LunaTraits_LWF_Movie._bind_getBlue),
        new RegType("getLWF", impl_LunaTraits_LWF_Movie._bind_getLWF),
        new RegType("getBlendMode", impl_LunaTraits_LWF_Movie._bind_getBlendMode),
        new RegType("setVisible", impl_LunaTraits_LWF_Movie._bind_setVisible),
        new RegType("setX", impl_LunaTraits_LWF_Movie._bind_setX),
        new RegType("setY", impl_LunaTraits_LWF_Movie._bind_setY),
        new RegType("setScaleX", impl_LunaTraits_LWF_Movie._bind_setScaleX),
        new RegType("setScaleY", impl_LunaTraits_LWF_Movie._bind_setScaleY),
        new RegType("setRotation", impl_LunaTraits_LWF_Movie._bind_setRotation),
        new RegType("setAlpha", impl_LunaTraits_LWF_Movie._bind_setAlpha),
        new RegType("setRed", impl_LunaTraits_LWF_Movie._bind_setRed),
        new RegType("setGreen", impl_LunaTraits_LWF_Movie._bind_setGreen),
        new RegType("setBlue", impl_LunaTraits_LWF_Movie._bind_setBlue),
        new RegType("setBlendMode", impl_LunaTraits_LWF_Movie._bind_setBlendMode),
        new RegType("addEventListener", impl_LunaTraits_LWF_Movie.addEventListener),
        new RegType("attachMovie", impl_LunaTraits_LWF_Movie.attachMovie),
        new RegType("attachEmptyMovie", impl_LunaTraits_LWF_Movie.attachEmptyMovie),
        new RegType("attachLWF", impl_LunaTraits_LWF_Movie.attachLWF),
        new RegType("dispatchEvent", impl_LunaTraits_LWF_Movie.dispatchEvent),

		new RegType("__index", impl_LunaTraits_LWF_Movie.__index),
		new RegType("__newindex", impl_LunaTraits_LWF_Movie.__newindex),
		new RegType(null,null)
	};

	public static LWF.Movie _bind_ctor(Lua.lua_State L)
	{
		Luna.print("undefined contructor of LWF.Movie called\n");
		return null;
	}

	public static void _bind_dtor(LWF.Movie obj)
	{
	}

	public static Dictionary<string, Lua.lua_CFunction> properties = new Dictionary<string, Lua.lua_CFunction>();
	public static Dictionary<string, Lua.lua_CFunction> write_properties = new Dictionary<string, Lua.lua_CFunction>();
}

public class impl_LunaTraits_LWF_Movie
{
	static string getName(LWF.Movie o){return o.name;}
	static int getCurrentFrame(LWF.Movie o){return o.currentFrame;}
	static string getCurrentLabel(LWF.Movie o){return o.GetCurrentLabel();}
	static int getTotalFrames(LWF.Movie o){return o.totalFrames;}
	static bool getVisible(LWF.Movie o){return o.visible;}
	static float getX(LWF.Movie o){return o.x;}
	static float getY(LWF.Movie o){return o.y;}
	static float getScaleX(LWF.Movie o){return o.scaleX;}
	static float getScaleY(LWF.Movie o){return o.scaleY;}
	static float getRotation(LWF.Movie o){return o.rotation;}
	static float getAlpha(LWF.Movie o){return o.alpha;}
	static float getRed(LWF.Movie o){return o.red;}
	static float getGreen(LWF.Movie o){return o.green;}
	static float getBlue(LWF.Movie o){return o.blue;}

	static void setVisible(LWF.Movie o, bool v){o.SetVisible(v);}
	static void setX(LWF.Movie o, float v){o.x=v;}
	static void setY(LWF.Movie o, float v){o.y=v;}
	static void setScaleX(LWF.Movie o, float v){o.scaleX=v;}
	static void setScaleY(LWF.Movie o, float v){o.scaleY=v;}
	static void setRotation(LWF.Movie o, float v){o.rotation=v;}
	static void setAlpha(LWF.Movie o, float v){o.alpha=v;}
	static void setRed(LWF.Movie o, float v){o.red=v;}
	static void setGreen(LWF.Movie o, float v){o.green=v;}
	static void setBlue(LWF.Movie o, float v){o.blue=v;}

	static string getBlendMode(LWF.Movie o)
	{
		switch (o.blendMode) {
			default:
				return "normal";
			case (int)LWF.Format.Constant.BLEND_MODE_ADD:
				return "add";
			case (int)LWF.Format.Constant.BLEND_MODE_ERASE:
				return "erase";
			case (int)LWF.Format.Constant.BLEND_MODE_LAYER:
				return "layer";
			case (int)LWF.Format.Constant.BLEND_MODE_MASK:
				return "mask";
			case (int)LWF.Format.Constant.BLEND_MODE_MULTIPLY:
				return "multiply";
			case (int)LWF.Format.Constant.BLEND_MODE_SCREEN:
				return "screen";
			case (int)LWF.Format.Constant.BLEND_MODE_SUBTRACT:
				return "subtract";
		}
	}

	static void setBlendMode(LWF.Movie o, string v)
	{
		switch (v.ToLower()) {
		default:
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_NORMAL;
			break;
		case "add":
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_ADD;
			break;
		case "erase":
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_ERASE;
			break;
		case "layer":
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_LAYER;
			break;
		case "mask":
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_MASK;
			break;
		case "multiply":
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_MULTIPLY;
			break;
		case "screen":
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_SCREEN;
			break;
		case "subtract":
			o.blendMode = (int)LWF.Format.Constant.BLEND_MODE_SUBTRACT;
			break;
		}
	}

	public static int _bind_getLWF(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 1 || Luna.get_uniqueid(L, 1) !=
				LunaTraits_LWF_Movie.uniqueID) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.lwf");
		}
		LWF.Movie a =
			Luna_LWF_Movie.check(L, 1);
		Luna_LWF_LWF.push(L, a.lwf, false);
		return 1;
	}

	public static int _bind_getParent(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 1 || Luna.get_uniqueid(L, 1) !=
				LunaTraits_LWF_Movie.uniqueID) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.parent");
		}
		LWF.Movie a =
			Luna_LWF_Movie.check(L, 1);
		Luna_LWF_Movie.push(L, a.parent, false);
		return 1;
	}

	public static int _bind_getCurrentLabels(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 1 || Luna.get_uniqueid(L, 1) !=
				LunaTraits_LWF_Movie.uniqueID) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.currentLabels");
		}
		LWF.Movie a =
			Luna_LWF_Movie.check(L, 1);
		List<LWF.LabelData> currentLabels = a.GetCurrentLabels();
	
		Lua.lua_createtable(L, currentLabels.Count, 0);
		/* -1: table */
		int i = 1;
		foreach(LWF.LabelData labelData in currentLabels) {
			Lua.lua_pushnumber(L, i);
			/* -2: table */
			/* -1: index */
			Lua.lua_createtable(L, 0, 2);
			/* -3: table */
			/* -2: index */
			/* -1: table */
			Lua.lua_pushnumber(L, labelData.frame);
			/* -4: table */
			/* -3: index */
			/* -2: table */
			/* -1: frame */
			Lua.lua_setfield(L, -2, "frame");
			/* -3: table */
			/* -2: index */
			/* -1: table */
			Lua.lua_pushstring(L, labelData.name);
			/* -4: table */
			/* -3: index */
			/* -2: table */
			/* -1: name */
			Lua.lua_setfield(L, -2, "name");
			/* -3: table */
			/* -2: index */
			/* -1: table */
			Lua.lua_settable(L, -3);
			/* -1: table */
			++i;
		}
		/* -1: table */
		return 1;
	}

	public static int attachMovie(Lua.lua_State L)
	{
		LWF.Movie a;
		int args = Lua.lua_gettop(L);
		if (args < 3 || args > 6)
			goto error;
		if (Luna.get_uniqueid(L, 1) != LunaTraits_LWF_Movie.uniqueID)
			goto error;
		if (Lua.lua_isstring(L, 2)==0 || Lua.lua_isstring(L, 3)==0)
			goto error;
		if (args >= 4 && !Lua.lua_istable(L, 4))
			goto error;
		if (args >= 5 && Lua.lua_isnumber(L, 5)==0)
			goto error;
		if (args >= 6 && !Lua.lua_isboolean(L, 6))
			goto error;

		a = Luna_LWF_Movie.check(L, 1);
		return a.lwf.AttachMovieLua(a, false);

	error:
		Luna.printStack(L);
		Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.attachMovie");
		return 1;
	}

	public static int attachEmptyMovie(Lua.lua_State L)
	{
		LWF.Movie a;
		int args = Lua.lua_gettop(L);
		if (args < 2 || args > 5)
			goto error;
		if (Luna.get_uniqueid(L, 1) != LunaTraits_LWF_Movie.uniqueID)
			goto error;
		if (Lua.lua_isstring(L, 2)==0)
			goto error;
		if (args >= 3 && !Lua.lua_istable(L, 3))
			goto error;
		if (args >= 4 && Lua.lua_isnumber(L, 4)==0)
			goto error;
		if (args >= 5 && !Lua.lua_isboolean(L, 5))
			goto error;

		a = Luna_LWF_Movie.check(L, 1);
		return a.lwf.AttachMovieLua(a, true);

	error:
		Luna.printStack(L);
		Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.attachMovie");
		return 1;
	}

	public static int attachLWF(Lua.lua_State L)
	{
		LWF.Movie a;
		int args = Lua.lua_gettop(L);
		if (args < 3 || args > 6)
			goto error;
		if (Luna.get_uniqueid(L, 1) != LunaTraits_LWF_Movie.uniqueID)
			goto error;
		if (Lua.lua_isstring(L, 2)==0 || Lua.lua_isstring(L, 3)==0)
			goto error;
		if (args >= 4 && Lua.lua_isnumber(L, 4)==0)
			goto error;
		if (args >= 5 && !Lua.lua_isboolean(L, 5))
			goto error;
		if (args >= 6 && Lua.lua_isstring(L, 6)==0)
			goto error;

		a = Luna_LWF_Movie.check(L, 1);
		return a.lwf.AttachLWFLua(a);

	error:
		Luna.printStack(L);
		Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.attachLWF");
		return 1;
	}

	public static int addEventListener(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 3 ||
				Luna.get_uniqueid(L, 1) != LunaTraits_LWF_Movie.uniqueID ||
				Lua.lua_isstring(L, 2) == 0 || !Lua.lua_isfunction(L, 3)) {
			Luna.printStack(L);
      Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.addEventListener");
		}

		LWF.Movie a = Luna_LWF_Movie.check(L, 1);
    return a.lwf.AddEventHandlerLua(a);
	}

	public static int dispatchEvent(Lua.lua_State L)
	{
    LWF.Movie a;
    string eventName;
		if (Lua.lua_gettop(L) != 2)
      goto error;
		if (Luna.get_uniqueid(L, 1) != LunaTraits_LWF_Movie.uniqueID)
      goto error;
    if (Lua.lua_isstring(L, 2)!=0) {
      eventName = Lua.lua_tostring(L, 2).ToString();
    } else if (Lua.lua_istable(L, 2)) {
      Lua.lua_getfield(L, 2, "type");
      if (Lua.lua_isstring(L, -1)==0)
        goto error;
      eventName = Lua.lua_tostring(L, -1).ToString();
      Lua.lua_pop(L, 1);
    } else {
      goto error;
    }

		a = Luna_LWF_Movie.check(L, 1);
		a.DispatchEvent(eventName);
    return 0;

	error:
		Luna.printStack(L);
    Lua.luaL_error(L, "luna typecheck failed: LWF.Movie.dispatchEvent");
    return 1;
	}



  public static int _bind_getFullName(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getFullName(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		string ret=self.GetFullName();
		Lua.lua_pushstring(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_globalToLocal(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Luna.get_uniqueid(L,2)!= LunaTraits_LWF_Point.uniqueID) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:globalToLocal(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		LWF.Point point=Luna_LWF_Point.check(L,2);
	try {
		LWF.Point ret=self.GlobalToLocal(point);
		Luna_LWF_Point.push(L,ret,true,"LWF_Point");
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_localToGlobal(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Luna.get_uniqueid(L,2)!= LunaTraits_LWF_Point.uniqueID) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:localToGlobal(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		LWF.Point point=Luna_LWF_Point.check(L,2);
	try {
		LWF.Point ret=self.LocalToGlobal(point);
		Luna_LWF_Point.push(L,ret,true,"LWF_Point");
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_play(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:play(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		self.Play();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_stop(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:stop(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		self.Stop();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_nextFrame(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:nextFrame(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		self.NextFrame();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_prevFrame(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:prevFrame(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		self.PrevFrame();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoFrame(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:gotoFrame(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		int frameNo=(int)Lua.lua_tonumber(L,2);
	try {
		self.GotoFrame(frameNo);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndStop_overload_1(Lua.lua_State L)
  {

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		int frameNo=(int)Lua.lua_tonumber(L,2);
	try {
		self.GotoAndStop(frameNo);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndStop_overload_2(Lua.lua_State L)
  {

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		string label=Lua.lua_tostring(L,2).ToString();
	try {
		self.GotoAndStop(label);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndPlay_overload_1(Lua.lua_State L)
  {

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		int frameNo=(int)Lua.lua_tonumber(L,2);
	try {
		self.GotoAndPlay(frameNo);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndPlay_overload_2(Lua.lua_State L)
  {

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		string label=Lua.lua_tostring(L,2).ToString();
	try {
		self.GotoAndPlay(label);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_move(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:move(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		float vx=(float)Lua.lua_tonumber(L,2);
		float vy=(float)Lua.lua_tonumber(L,3);
	try {
		self.Move(vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_moveTo(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:moveTo(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		float vx=(float)Lua.lua_tonumber(L,2);
		float vy=(float)Lua.lua_tonumber(L,3);
	try {
		self.MoveTo(vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_rotate(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:rotate(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		float degree=(float)Lua.lua_tonumber(L,2);
	try {
		self.Rotate(degree);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_rotateTo(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:rotateTo(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		float degree=(float)Lua.lua_tonumber(L,2);
	try {
		self.RotateTo(degree);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_scale(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:scale(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		float vx=(float)Lua.lua_tonumber(L,2);
		float vy=(float)Lua.lua_tonumber(L,3);
	try {
		self.Scale(vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_scaleTo(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:scaleTo(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		float vx=(float)Lua.lua_tonumber(L,2);
		float vy=(float)Lua.lua_tonumber(L,3);
	try {
		self.ScaleTo(vx, vy);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_removeEventListener(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:removeEventListener(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
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
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:clearEventListener(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		string eventName=Lua.lua_tostring(L,2).ToString();
	try {
		self.ClearEventHandler(eventName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_swapAttachedMovieDepth(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:swapAttachedMovieDepth(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		int depth0=(int)Lua.lua_tonumber(L,2);
		int depth1=(int)Lua.lua_tonumber(L,3);
	try {
		self.SwapAttachedMovieDepth(depth0, depth1);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_detachMovie_overload_1(Lua.lua_State L)
  {

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		string aName=Lua.lua_tostring(L,2).ToString();
	try {
		self.DetachMovie(aName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_detachMovie_overload_2(Lua.lua_State L)
  {

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		LWF.Movie movie=Luna_LWF_Movie.check(L,2);
	try {
		self.DetachMovie(movie);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_detachFromParent(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:detachFromParent(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		self.DetachFromParent();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_detachLWF(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:detachLWF(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		string aName=Lua.lua_tostring(L,2).ToString();
	try {
		self.DetachLWF(aName);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_detachAllLWFs(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:detachAllLWFs(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		self.DetachAllLWFs();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_removeMovieClip(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:removeMovieClip(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
	try {
		self.RemoveMovieClip();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_attachBitmap(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isstring(L,2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:attachBitmap(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		string linkageName=Lua.lua_tostring(L,2).ToString();
		int depth=(int)Lua.lua_tonumber(L,3);
	try {
		LWF.BitmapClip ret=self.AttachBitmap(linkageName, depth);
		Luna_LWF_BitmapClip.push(L,ret,true,"LWF_BitmapClip");
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getAttachedBitmap(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getAttachedBitmap(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		int depth=(int)Lua.lua_tonumber(L,2);
	try {
		LWF.BitmapClip ret=self.GetAttachedBitmap(depth);
		Luna_LWF_BitmapClip.push(L,ret,true,"LWF_BitmapClip");
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_swapAttachedBitmapDepth(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=3
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0
            || Lua.lua_isnumber(L, 3)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:swapAttachedBitmapDepth(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		int depth0=(int)Lua.lua_tonumber(L,2);
		int depth1=(int)Lua.lua_tonumber(L,3);
	try {
		self.SwapAttachedBitmapDepth(depth0, depth1);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_detachBitmap(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:detachBitmap(LWF.Movie self)"); }

	LWF.Movie self=Luna_LWF_Movie.check(L,1);
		int depth=(int)Lua.lua_tonumber(L,2);
	try {
		self.DetachBitmap(depth);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_gotoAndStop(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)==2
            && Luna.get_uniqueid(L,1)==29625181 
            && Lua.lua_isnumber(L, 2)==1) return _bind_gotoAndStop_overload_1(L);
	if (Lua.lua_gettop(L)==2
            && Luna.get_uniqueid(L,1)==29625181 
            && Lua.lua_isstring(L,2)==1) return _bind_gotoAndStop_overload_2(L);
	Lua.luaL_error(L, "gotoAndStop cannot find overloads.");

	return 0;
  }
  public static int _bind_gotoAndPlay(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)==2
            && Luna.get_uniqueid(L,1)==29625181 
            && Lua.lua_isnumber(L, 2)==1) return _bind_gotoAndPlay_overload_1(L);
	if (Lua.lua_gettop(L)==2
            && Luna.get_uniqueid(L,1)==29625181 
            && Lua.lua_isstring(L,2)==1) return _bind_gotoAndPlay_overload_2(L);
	Lua.luaL_error(L, "gotoAndPlay cannot find overloads.");

	return 0;
  }
  public static int _bind_detachMovie(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)==2
            && Luna.get_uniqueid(L,1)==29625181 
            && Lua.lua_isstring(L,2)==1) return _bind_detachMovie_overload_1(L);
	if (Lua.lua_gettop(L)==2
            && Luna.get_uniqueid(L,1)==29625181 
            && Luna.get_uniqueid(L,2)== LunaTraits_LWF_Movie.uniqueID) return _bind_detachMovie_overload_2(L);
	Lua.luaL_error(L, "detachMovie cannot find overloads.");

	return 0;
  }
  public static int _bind_getName(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getName(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		string ret=getName(o);
		Lua.lua_pushstring(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getCurrentFrame(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getCurrentFrame(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		int ret=getCurrentFrame(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getCurrentLabel(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getCurrentLabel(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		string ret=getCurrentLabel(o);
		Lua.lua_pushstring(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getTotalFrames(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getTotalFrames(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		int ret=getTotalFrames(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getVisible(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getVisible(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		bool ret=getVisible(o);
		Lua.lua_pushboolean(L, ret?1:0);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getX(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getX(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getX(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getY(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getY(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getY(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getScaleX(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getScaleX(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getScaleX(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getScaleY(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getScaleY(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getScaleY(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getRotation(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getRotation(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getRotation(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getAlpha(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getAlpha(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getAlpha(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getRed(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getRed(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getRed(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getGreen(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getGreen(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getGreen(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getBlue(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getBlue(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		float ret=getBlue(o);
		Lua.lua_pushnumber(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_getBlendMode(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=29625181 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getBlendMode(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
	try {
		string ret=getBlendMode(o);
		Lua.lua_pushstring(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
  public static int _bind_setVisible(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || !Lua.lua_isboolean(L,2)) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setVisible(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		bool v=(Lua.lua_toboolean(L,2) != 0);
	try {
		setVisible(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setX(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setX(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setX(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setY(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setY(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setY(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setScaleX(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setScaleX(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setScaleX(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setScaleY(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setScaleY(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setScaleY(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setRotation(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setRotation(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setRotation(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setAlpha(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setAlpha(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setAlpha(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setRed(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setRed(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setRed(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setGreen(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setGreen(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setGreen(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setBlue(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isnumber(L, 2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setBlue(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		float v=(float)Lua.lua_tonumber(L,2);
	try {
		setBlue(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_setBlendMode(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=2
            || Luna.get_uniqueid(L,1)!=29625181 
            || Lua.lua_isstring(L,2)==0) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:setBlendMode(LWF.Movie self ...)"); }
		LWF.Movie o=Luna_LWF_Movie.check(L,1);
		string v=Lua.lua_tostring(L,2).ToString();
	try {
		setBlendMode(o, v);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }


	public static void luna_init_hashmap()
	{
        LunaTraits_LWF_Movie.properties["name"]=_bind_getName;
        LunaTraits_LWF_Movie.properties["parent"]=_bind_getParent;
        LunaTraits_LWF_Movie.properties["currentFrame"]=_bind_getCurrentFrame;
        LunaTraits_LWF_Movie.properties["currentLabel"]=_bind_getCurrentLabel;
        LunaTraits_LWF_Movie.properties["currentLabels"]=_bind_getCurrentLabels;
        LunaTraits_LWF_Movie.properties["totalFrames"]=_bind_getTotalFrames;
        LunaTraits_LWF_Movie.properties["visible"]=_bind_getVisible;
        LunaTraits_LWF_Movie.properties["x"]=_bind_getX;
        LunaTraits_LWF_Movie.properties["y"]=_bind_getY;
        LunaTraits_LWF_Movie.properties["scaleX"]=_bind_getScaleX;
        LunaTraits_LWF_Movie.properties["scaleY"]=_bind_getScaleY;
        LunaTraits_LWF_Movie.properties["rotation"]=_bind_getRotation;
        LunaTraits_LWF_Movie.properties["alpha"]=_bind_getAlpha;
        LunaTraits_LWF_Movie.properties["red"]=_bind_getRed;
        LunaTraits_LWF_Movie.properties["green"]=_bind_getGreen;
        LunaTraits_LWF_Movie.properties["blue"]=_bind_getBlue;
        LunaTraits_LWF_Movie.properties["lwf"]=_bind_getLWF;
        LunaTraits_LWF_Movie.properties["blendMode"]=_bind_getBlendMode;

	}

	public static void luna_init_write_hashmap()
	{
         LunaTraits_LWF_Movie.write_properties["visible"]=_bind_setVisible;
         LunaTraits_LWF_Movie.write_properties["x"]=_bind_setX;
         LunaTraits_LWF_Movie.write_properties["y"]=_bind_setY;
         LunaTraits_LWF_Movie.write_properties["scaleX"]=_bind_setScaleX;
         LunaTraits_LWF_Movie.write_properties["scaleY"]=_bind_setScaleY;
         LunaTraits_LWF_Movie.write_properties["rotation"]=_bind_setRotation;
         LunaTraits_LWF_Movie.write_properties["alpha"]=_bind_setAlpha;
         LunaTraits_LWF_Movie.write_properties["red"]=_bind_setRed;
         LunaTraits_LWF_Movie.write_properties["green"]=_bind_setGreen;
         LunaTraits_LWF_Movie.write_properties["blue"]=_bind_setBlue;
         LunaTraits_LWF_Movie.write_properties["blendMode"]=_bind_setBlendMode;

	}

	public static int __index(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) == 2 && Luna.get_uniqueid(L, 1) ==
				LunaTraits_LWF_Movie.uniqueID) {
			LWF.Movie o =
				Luna_LWF_Movie.check(L, 1);
			string name = Lua.lua_tostring(L, 2).ToString();
			if (o.lwf.GetFieldLua(o, name))
				return 1;
			LWF.Movie movie = o.SearchMovieInstance(name, false);
			if (movie != null) {
				Lua.lua_pop(L, 1);
				Luna_LWF_Movie.push(L, movie, false);
				return 1;
			}
			LWF.Button button = o.SearchButtonInstance(name, false);
			if (button != null) {
				Lua.lua_pop(L, 1);
				Luna_LWF_Button.push(L, button, false);
				return 1;
			}
		}


		{
			Lua.lua_CFunction fnc = null;
			if (LunaTraits_LWF_Movie.properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
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
		if (LunaTraits_LWF_Movie.write_properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
		{
			Lua.lua_insert(L,2); // swap key and value
			Lua.lua_settop(L,2); // delete key
			return fnc(L);
		}
		if (Lua.lua_gettop(L) == 3 && Luna.get_uniqueid(L, 1) ==
			LunaTraits_LWF_Movie.uniqueID)
		{
			LWF.Movie o =
				Luna_LWF_Movie.check(L, 1);
			string name = Lua.lua_tostring(L, 2).ToString();
			if (o.lwf.SetFieldLua(o, name))
			return 0;
		}

		Lua.luaL_error(L,"__newindex doesn't allow defining non-property member");
		return 0;
	}
}

class Luna_LWF_Movie
{
	private static int idOffset = 0;
	private static Dictionary<Lua.lua_State, Dictionary<int, LWF.Movie>> objects = new Dictionary<Lua.lua_State, Dictionary<int, LWF.Movie>>();
	private static Dictionary<Lua.lua_State, Dictionary<LWF.Movie, int>> objectIdentifiers = new Dictionary<Lua.lua_State, Dictionary<LWF.Movie, int>>();

	public static void set(Lua.lua_State L, int table_index, Lua.CharPtr key)
	{
		Lua.lua_pushstring(L, key);
		Lua.lua_insert(L, -2);  // swap value and key
		Lua.lua_settable(L, table_index);
	}

	public static void Register(Lua.lua_State L)
	{
		objects.Add(L, new Dictionary<int, LWF.Movie>());
		objectIdentifiers.Add(L, new Dictionary<LWF.Movie, int>());
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
		Lua.lua_pushstring(L, LunaTraits_LWF_Movie.className);
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
			LunaTraits_LWF_Movie.RegType l = LunaTraits_LWF_Movie.methods[i];
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

	public static void Destroy(Lua.lua_State L, LWF.Movie obj)
	{

		int objectId = -1;
		if (objectIdentifiers[L].TryGetValue(obj, out objectId))
		{
			objectIdentifiers[L].Remove(obj);
			objects[L].Remove(objectId);
		}
	}

	static public LWF.Movie check(Lua.lua_State L, int narg)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L,narg);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, narg, LunaTraits_LWF_Movie.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		if(ud.TypeId !=LunaTraits_LWF_Movie.uniqueID) // type checking with almost no overhead
		{
			Luna.print(String.Format("ud.uid: {0} != interface::uid : {1}\n", ud.TypeId, LunaTraits_LWF_Movie.uniqueID));
			Lua.luaL_typerror(L, narg, LunaTraits_LWF_Movie.className);
		}
		LWF.Movie obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return null;
		return obj;
	}

	// use lunaStack::push if possible.
	public static void push(Lua.lua_State L, LWF.Movie obj, bool gc, Lua.CharPtr metatable=null)
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
				metatable = LunaTraits_LWF_Movie.className;
		Lua.lua_pushstring(L,"__luna");
		Lua.lua_gettable(L, Lua.LUA_GLOBALSINDEX);
		int __luna= Lua.lua_gettop(L);

		Luna.userdataType ud = new Luna.userdataType(
			objectId:objectId,  // store object in userdata
			gc:gc,   // collect garbage
			has_env:false, // does this userdata has a table attached to it?
			typeId:LunaTraits_LWF_Movie.uniqueID
		);

		ud.ToBytes((byte[])Lua.lua_newuserdata(L, Luna.userdataType.Size));

		Lua.lua_pushstring(L, metatable);
		Lua.lua_gettable(L, __luna);
		Lua.lua_setmetatable(L, -2);
		//Luna.printStack(L);
		Lua.lua_insert(L, -2);  // swap __luna and userdata
		Lua.lua_pop(L,1);
	}

	private Luna_LWF_Movie(){}  // hide default constructor

	// create a new T object and
	// push onto the Lua stack a userdata containing a pointer to T object
	private static int new_T(Lua.lua_State L)
	{
		Lua.lua_remove(L, 1);   // use classname:new(), instead of classname.new()
		LWF.Movie obj = LunaTraits_LWF_Movie._bind_ctor(L);  // call constructor for T objects
		push(L,obj,true);
		return 1;  // userdata containing pointer to T object
	}

	// garbage collection metamethod
	private static int gc_T(Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_Movie.className); }
		Luna.userdataType ud = new Luna.userdataType(d);

		LWF.Movie obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		if (ud.Gc) {
			LunaTraits_LWF_Movie._bind_dtor(obj);  // call constructor for T objects
			Destroy(L, obj);
		}

		return 0;
	}

	private static int tostring_T (Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_Movie.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		LWF.Movie obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		char[] buff = obj.ToString().ToCharArray(0,32);
		Lua.lua_pushfstring(L, "%s (%s)", new object[] {LunaTraits_LWF_Movie.className, buff});
		return 1;
	}
}

#endif
