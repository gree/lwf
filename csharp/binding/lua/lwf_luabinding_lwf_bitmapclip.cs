#if LWF_USE_LUA

using System;
using System.Collections.Generic;
using KopiLua;

public class LunaTraits_LWF_BitmapClip
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

	public static Lua.CharPtr className = "LWF_BitmapClip";
	public static int uniqueID = 55459900;
	public static RegType[] methods = new RegType[]
	{
        new RegType("detachFromParent", impl_LunaTraits_LWF_BitmapClip._bind_detachFromParent),
        new RegType("_property_get_depth", impl_LunaTraits_LWF_BitmapClip._bind__property_get_depth),
        new RegType("_property_get_visible", impl_LunaTraits_LWF_BitmapClip._bind__property_get_visible),
        new RegType("_property_get_width", impl_LunaTraits_LWF_BitmapClip._bind__property_get_width),
        new RegType("_property_get_height", impl_LunaTraits_LWF_BitmapClip._bind__property_get_height),
        new RegType("_property_get_regX", impl_LunaTraits_LWF_BitmapClip._bind__property_get_regX),
        new RegType("_property_get_regY", impl_LunaTraits_LWF_BitmapClip._bind__property_get_regY),
        new RegType("_property_get_x", impl_LunaTraits_LWF_BitmapClip._bind__property_get_x),
        new RegType("_property_get_y", impl_LunaTraits_LWF_BitmapClip._bind__property_get_y),
        new RegType("_property_get_scaleX", impl_LunaTraits_LWF_BitmapClip._bind__property_get_scaleX),
        new RegType("_property_get_scaleY", impl_LunaTraits_LWF_BitmapClip._bind__property_get_scaleY),
        new RegType("_property_get_rotation", impl_LunaTraits_LWF_BitmapClip._bind__property_get_rotation),
        new RegType("_property_get_alpha", impl_LunaTraits_LWF_BitmapClip._bind__property_get_alpha),
        new RegType("_property_get_offsetX", impl_LunaTraits_LWF_BitmapClip._bind__property_get_offsetX),
        new RegType("_property_get_offsetY", impl_LunaTraits_LWF_BitmapClip._bind__property_get_offsetY),
        new RegType("_property_get_originalWidth", impl_LunaTraits_LWF_BitmapClip._bind__property_get_originalWidth),
        new RegType("_property_get_originalHeight", impl_LunaTraits_LWF_BitmapClip._bind__property_get_originalHeight),
        new RegType("getName", impl_LunaTraits_LWF_BitmapClip._bind_getName),
        new RegType("getParent", impl_LunaTraits_LWF_BitmapClip._bind_getParent),
        new RegType("getLWF", impl_LunaTraits_LWF_BitmapClip._bind_getLWF),
        new RegType("_property_set_depth", impl_LunaTraits_LWF_BitmapClip._bind__property_set_depth),
        new RegType("_property_set_visible", impl_LunaTraits_LWF_BitmapClip._bind__property_set_visible),
        new RegType("_property_set_width", impl_LunaTraits_LWF_BitmapClip._bind__property_set_width),
        new RegType("_property_set_height", impl_LunaTraits_LWF_BitmapClip._bind__property_set_height),
        new RegType("_property_set_regX", impl_LunaTraits_LWF_BitmapClip._bind__property_set_regX),
        new RegType("_property_set_regY", impl_LunaTraits_LWF_BitmapClip._bind__property_set_regY),
        new RegType("_property_set_x", impl_LunaTraits_LWF_BitmapClip._bind__property_set_x),
        new RegType("_property_set_y", impl_LunaTraits_LWF_BitmapClip._bind__property_set_y),
        new RegType("_property_set_scaleX", impl_LunaTraits_LWF_BitmapClip._bind__property_set_scaleX),
        new RegType("_property_set_scaleY", impl_LunaTraits_LWF_BitmapClip._bind__property_set_scaleY),
        new RegType("_property_set_rotation", impl_LunaTraits_LWF_BitmapClip._bind__property_set_rotation),
        new RegType("_property_set_alpha", impl_LunaTraits_LWF_BitmapClip._bind__property_set_alpha),
        new RegType("_property_set_offsetX", impl_LunaTraits_LWF_BitmapClip._bind__property_set_offsetX),
        new RegType("_property_set_offsetY", impl_LunaTraits_LWF_BitmapClip._bind__property_set_offsetY),
        new RegType("_property_set_originalWidth", impl_LunaTraits_LWF_BitmapClip._bind__property_set_originalWidth),
        new RegType("_property_set_originalHeight", impl_LunaTraits_LWF_BitmapClip._bind__property_set_originalHeight),

		new RegType("__index", impl_LunaTraits_LWF_BitmapClip.__index),
		new RegType("__newindex", impl_LunaTraits_LWF_BitmapClip.__newindex),
		new RegType(null,null)
	};

	public static LWF.BitmapClip _bind_ctor(Lua.lua_State L)
	{
		Luna.print("undefined contructor of LWF.BitmapClip called\n");
		return null;
	}

	public static void _bind_dtor(LWF.BitmapClip obj)
	{
	}

	public static Dictionary<string, Lua.lua_CFunction> properties = new Dictionary<string, Lua.lua_CFunction>();
	public static Dictionary<string, Lua.lua_CFunction> write_properties = new Dictionary<string, Lua.lua_CFunction>();
}

public class impl_LunaTraits_LWF_BitmapClip
{
	static string getName(LWF.BitmapClip o){return o.name;}

	public static int _bind_getLWF(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 1 || Luna.get_uniqueid(L, 1) !=
				LunaTraits_LWF_BitmapClip.uniqueID) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.BitmapClip.lwf");
		}
		LWF.BitmapClip a =
			Luna_LWF_BitmapClip.check(L, 1);
		Luna_LWF_LWF.push(L, a.lwf, false);
		return 1;
	}

	public static int _bind_getParent(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L) != 1 || Luna.get_uniqueid(L, 1) !=
				LunaTraits_LWF_BitmapClip.uniqueID) {
			Luna.printStack(L);
			Lua.luaL_error(L, "luna typecheck failed: LWF.BitmapClip.parent");
		}
		LWF.BitmapClip a =
			Luna_LWF_BitmapClip.check(L, 1);
		Luna_LWF_Movie.push(L, a.parent, false);
		return 1;
	}



	public static int _property_get_depth(LWF.BitmapClip a) { return a.depth; }
	public static void _property_set_depth(LWF.BitmapClip a, int b) { a.depth=b; }
	public static bool _property_get_visible(LWF.BitmapClip a) { return a.visible; }
	public static void _property_set_visible(LWF.BitmapClip a, bool b) { a.visible=b; }
	public static float _property_get_width(LWF.BitmapClip a) { return a.width; }
	public static void _property_set_width(LWF.BitmapClip a, float b) { a.width=b; }
	public static float _property_get_height(LWF.BitmapClip a) { return a.height; }
	public static void _property_set_height(LWF.BitmapClip a, float b) { a.height=b; }
	public static float _property_get_regX(LWF.BitmapClip a) { return a.regX; }
	public static void _property_set_regX(LWF.BitmapClip a, float b) { a.regX=b; }
	public static float _property_get_regY(LWF.BitmapClip a) { return a.regY; }
	public static void _property_set_regY(LWF.BitmapClip a, float b) { a.regY=b; }
	public static float _property_get_x(LWF.BitmapClip a) { return a.x; }
	public static void _property_set_x(LWF.BitmapClip a, float b) { a.x=b; }
	public static float _property_get_y(LWF.BitmapClip a) { return a.y; }
	public static void _property_set_y(LWF.BitmapClip a, float b) { a.y=b; }
	public static float _property_get_scaleX(LWF.BitmapClip a) { return a.scaleX; }
	public static void _property_set_scaleX(LWF.BitmapClip a, float b) { a.scaleX=b; }
	public static float _property_get_scaleY(LWF.BitmapClip a) { return a.scaleY; }
	public static void _property_set_scaleY(LWF.BitmapClip a, float b) { a.scaleY=b; }
	public static float _property_get_rotation(LWF.BitmapClip a) { return a.rotation; }
	public static void _property_set_rotation(LWF.BitmapClip a, float b) { a.rotation=b; }
	public static float _property_get_alpha(LWF.BitmapClip a) { return a.alpha; }
	public static void _property_set_alpha(LWF.BitmapClip a, float b) { a.alpha=b; }
	public static float _property_get_offsetX(LWF.BitmapClip a) { return a.offsetX; }
	public static void _property_set_offsetX(LWF.BitmapClip a, float b) { a.offsetX=b; }
	public static float _property_get_offsetY(LWF.BitmapClip a) { return a.offsetY; }
	public static void _property_set_offsetY(LWF.BitmapClip a, float b) { a.offsetY=b; }
	public static float _property_get_originalWidth(LWF.BitmapClip a) { return a.originalWidth; }
	public static void _property_set_originalWidth(LWF.BitmapClip a, float b) { a.originalWidth=b; }
	public static float _property_get_originalHeight(LWF.BitmapClip a) { return a.originalHeight; }
	public static void _property_set_originalHeight(LWF.BitmapClip a, float b) { a.originalHeight=b; }
	public static int _bind__property_get_depth(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_depth(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			int ret=_property_get_depth(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_depth(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_depth(LWF.BitmapClip a, int b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		int b=(int)(int)Lua.lua_tonumber(L,2);
		try {
			_property_set_depth(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_visible(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_visible(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			bool ret=_property_get_visible(a);
			Lua.lua_pushboolean(L, ret?1:0);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_visible(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || !Lua.lua_isboolean(L,2))
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_visible(LWF.BitmapClip a, bool b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		bool b=(bool)(Lua.lua_toboolean(L,2) != 0);
		try {
			_property_set_visible(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_width(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_width(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_width(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_width(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_width(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_width(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_height(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_height(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_height(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_height(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_height(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_height(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_regX(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_regX(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_regX(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_regX(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_regX(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_regX(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_regY(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_regY(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_regY(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_regY(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_regY(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_regY(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_x(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_x(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_x(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_x(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_x(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_x(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_y(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_y(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_y(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_y(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_y(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_y(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_scaleX(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_scaleX(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_scaleX(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_scaleX(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_scaleX(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_scaleX(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_scaleY(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_scaleY(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_scaleY(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_scaleY(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_scaleY(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_scaleY(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_rotation(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_rotation(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_rotation(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_rotation(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_rotation(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_rotation(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_alpha(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_alpha(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_alpha(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_alpha(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_alpha(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_alpha(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_offsetX(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_offsetX(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_offsetX(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_offsetX(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_offsetX(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_offsetX(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_offsetY(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_offsetY(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_offsetY(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_offsetY(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_offsetY(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_offsetY(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_originalWidth(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_originalWidth(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_originalWidth(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_originalWidth(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_originalWidth(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_originalWidth(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_originalHeight(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=55459900)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_originalHeight(LWF.BitmapClip a)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		try {
			float ret=_property_get_originalHeight(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_originalHeight(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=55459900 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_originalHeight(LWF.BitmapClip a, float b)");
		}

		LWF.BitmapClip a=Luna_LWF_BitmapClip.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_originalHeight(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

  public static int _bind_detachFromParent(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=55459900 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:detachFromParent(LWF.BitmapClip self)"); }

	LWF.BitmapClip self=Luna_LWF_BitmapClip.check(L,1);
	try {
		self.DetachFromParent();
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
  public static int _bind_getName(Lua.lua_State L)
  {
	if (Lua.lua_gettop(L)!=1
            || Luna.get_uniqueid(L,1)!=55459900 ) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:getName(LWF.BitmapClip self ...)"); }
		LWF.BitmapClip o=Luna_LWF_BitmapClip.check(L,1);
	try {
		string ret=getName(o);
		Lua.lua_pushstring(L, ret);
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }


	public static void luna_init_hashmap()
	{
        LunaTraits_LWF_BitmapClip.properties["depth"]=_bind__property_get_depth;
        LunaTraits_LWF_BitmapClip.properties["visible"]=_bind__property_get_visible;
        LunaTraits_LWF_BitmapClip.properties["width"]=_bind__property_get_width;
        LunaTraits_LWF_BitmapClip.properties["height"]=_bind__property_get_height;
        LunaTraits_LWF_BitmapClip.properties["regX"]=_bind__property_get_regX;
        LunaTraits_LWF_BitmapClip.properties["regY"]=_bind__property_get_regY;
        LunaTraits_LWF_BitmapClip.properties["x"]=_bind__property_get_x;
        LunaTraits_LWF_BitmapClip.properties["y"]=_bind__property_get_y;
        LunaTraits_LWF_BitmapClip.properties["scaleX"]=_bind__property_get_scaleX;
        LunaTraits_LWF_BitmapClip.properties["scaleY"]=_bind__property_get_scaleY;
        LunaTraits_LWF_BitmapClip.properties["rotation"]=_bind__property_get_rotation;
        LunaTraits_LWF_BitmapClip.properties["alpha"]=_bind__property_get_alpha;
        LunaTraits_LWF_BitmapClip.properties["offsetX"]=_bind__property_get_offsetX;
        LunaTraits_LWF_BitmapClip.properties["offsetY"]=_bind__property_get_offsetY;
        LunaTraits_LWF_BitmapClip.properties["originalWidth"]=_bind__property_get_originalWidth;
        LunaTraits_LWF_BitmapClip.properties["originalHeight"]=_bind__property_get_originalHeight;
        LunaTraits_LWF_BitmapClip.properties["name"]=_bind_getName;
        LunaTraits_LWF_BitmapClip.properties["parent"]=_bind_getParent;
        LunaTraits_LWF_BitmapClip.properties["lwf"]=_bind_getLWF;

	}

	public static void luna_init_write_hashmap()
	{
         LunaTraits_LWF_BitmapClip.write_properties["depth"]=_bind__property_set_depth;
         LunaTraits_LWF_BitmapClip.write_properties["visible"]=_bind__property_set_visible;
         LunaTraits_LWF_BitmapClip.write_properties["width"]=_bind__property_set_width;
         LunaTraits_LWF_BitmapClip.write_properties["height"]=_bind__property_set_height;
         LunaTraits_LWF_BitmapClip.write_properties["regX"]=_bind__property_set_regX;
         LunaTraits_LWF_BitmapClip.write_properties["regY"]=_bind__property_set_regY;
         LunaTraits_LWF_BitmapClip.write_properties["x"]=_bind__property_set_x;
         LunaTraits_LWF_BitmapClip.write_properties["y"]=_bind__property_set_y;
         LunaTraits_LWF_BitmapClip.write_properties["scaleX"]=_bind__property_set_scaleX;
         LunaTraits_LWF_BitmapClip.write_properties["scaleY"]=_bind__property_set_scaleY;
         LunaTraits_LWF_BitmapClip.write_properties["rotation"]=_bind__property_set_rotation;
         LunaTraits_LWF_BitmapClip.write_properties["alpha"]=_bind__property_set_alpha;
         LunaTraits_LWF_BitmapClip.write_properties["offsetX"]=_bind__property_set_offsetX;
         LunaTraits_LWF_BitmapClip.write_properties["offsetY"]=_bind__property_set_offsetY;
         LunaTraits_LWF_BitmapClip.write_properties["originalWidth"]=_bind__property_set_originalWidth;
         LunaTraits_LWF_BitmapClip.write_properties["originalHeight"]=_bind__property_set_originalHeight;

	}

	public static int __index(Lua.lua_State L)
	{

		{
			Lua.lua_CFunction fnc = null;
			if (LunaTraits_LWF_BitmapClip.properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
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
		if (LunaTraits_LWF_BitmapClip.write_properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
		{
			Lua.lua_insert(L,2); // swap key and value
			Lua.lua_settop(L,2); // delete key
			return fnc(L);
		}

		Lua.luaL_error(L,"__newindex doesn't allow defining non-property member");
		return 0;
	}
}

class Luna_LWF_BitmapClip
{
	private static int idOffset = 0;
	private static Dictionary<Lua.lua_State, Dictionary<int, LWF.BitmapClip>> objects = new Dictionary<Lua.lua_State, Dictionary<int, LWF.BitmapClip>>();
	private static Dictionary<Lua.lua_State, Dictionary<LWF.BitmapClip, int>> objectIdentifiers = new Dictionary<Lua.lua_State, Dictionary<LWF.BitmapClip, int>>();

	public static void set(Lua.lua_State L, int table_index, Lua.CharPtr key)
	{
		Lua.lua_pushstring(L, key);
		Lua.lua_insert(L, -2);  // swap value and key
		Lua.lua_settable(L, table_index);
	}

	public static void Register(Lua.lua_State L)
	{
		objects.Add(L, new Dictionary<int, LWF.BitmapClip>());
		objectIdentifiers.Add(L, new Dictionary<LWF.BitmapClip, int>());
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
		Lua.lua_pushstring(L, LunaTraits_LWF_BitmapClip.className);
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
			LunaTraits_LWF_BitmapClip.RegType l = LunaTraits_LWF_BitmapClip.methods[i];
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

	public static void Destroy(Lua.lua_State L, LWF.BitmapClip obj)
	{

		int objectId = -1;
		if (objectIdentifiers[L].TryGetValue(obj, out objectId))
		{
			objectIdentifiers[L].Remove(obj);
			objects[L].Remove(objectId);
		}
	}

	static public LWF.BitmapClip check(Lua.lua_State L, int narg)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L,narg);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, narg, LunaTraits_LWF_BitmapClip.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		if(ud.TypeId !=LunaTraits_LWF_BitmapClip.uniqueID) // type checking with almost no overhead
		{
			Luna.print(String.Format("ud.uid: {0} != interface::uid : {1}\n", ud.TypeId, LunaTraits_LWF_BitmapClip.uniqueID));
			Lua.luaL_typerror(L, narg, LunaTraits_LWF_BitmapClip.className);
		}
		LWF.BitmapClip obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return null;
		return obj;
	}

	// use lunaStack::push if possible.
	public static void push(Lua.lua_State L, LWF.BitmapClip obj, bool gc, Lua.CharPtr metatable=null)
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
				metatable = LunaTraits_LWF_BitmapClip.className;
		Lua.lua_pushstring(L,"__luna");
		Lua.lua_gettable(L, Lua.LUA_GLOBALSINDEX);
		int __luna= Lua.lua_gettop(L);

		Luna.userdataType ud = new Luna.userdataType(
			objectId:objectId,  // store object in userdata
			gc:gc,   // collect garbage
			has_env:false, // does this userdata has a table attached to it?
			typeId:LunaTraits_LWF_BitmapClip.uniqueID
		);

		ud.ToBytes((byte[])Lua.lua_newuserdata(L, Luna.userdataType.Size));

		Lua.lua_pushstring(L, metatable);
		Lua.lua_gettable(L, __luna);
		Lua.lua_setmetatable(L, -2);
		//Luna.printStack(L);
		Lua.lua_insert(L, -2);  // swap __luna and userdata
		Lua.lua_pop(L,1);
	}

	private Luna_LWF_BitmapClip(){}  // hide default constructor

	// create a new T object and
	// push onto the Lua stack a userdata containing a pointer to T object
	private static int new_T(Lua.lua_State L)
	{
		Lua.lua_remove(L, 1);   // use classname:new(), instead of classname.new()
		LWF.BitmapClip obj = LunaTraits_LWF_BitmapClip._bind_ctor(L);  // call constructor for T objects
		push(L,obj,true);
		return 1;  // userdata containing pointer to T object
	}

	// garbage collection metamethod
	private static int gc_T(Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_BitmapClip.className); }
		Luna.userdataType ud = new Luna.userdataType(d);

		LWF.BitmapClip obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		if (ud.Gc) {
			LunaTraits_LWF_BitmapClip._bind_dtor(obj);  // call constructor for T objects
			Destroy(L, obj);
		}

		return 0;
	}

	private static int tostring_T (Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_BitmapClip.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		LWF.BitmapClip obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		char[] buff = obj.ToString().ToCharArray(0,32);
		Lua.lua_pushfstring(L, "%s (%s)", new object[] {LunaTraits_LWF_BitmapClip.className, buff});
		return 1;
	}
}

#endif
