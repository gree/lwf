#if LWF_USE_LUA

using System;
using System.Collections.Generic;
using KopiLua;

public class LunaTraits_LWF_Point
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

	public static Lua.CharPtr className = "LWF_Point";
	public static int uniqueID = 32383421;
	public static RegType[] methods = new RegType[]
	{
        new RegType("_property_get_x", impl_LunaTraits_LWF_Point._bind__property_get_x),
        new RegType("_property_get_y", impl_LunaTraits_LWF_Point._bind__property_get_y),
        new RegType("_property_set_x", impl_LunaTraits_LWF_Point._bind__property_set_x),
        new RegType("_property_set_y", impl_LunaTraits_LWF_Point._bind__property_set_y),

		new RegType("__index", impl_LunaTraits_LWF_Point.__index),
		new RegType("__newindex", impl_LunaTraits_LWF_Point.__newindex),
		new RegType(null,null)
	};

	public static LWF.Point _bind_ctor(Lua.lua_State L)
	{
		Luna.print("undefined contructor of LWF.Point called\n");
		return null;
	}

	public static void _bind_dtor(LWF.Point obj)
	{
	}

	public static Dictionary<string, Lua.lua_CFunction> properties = new Dictionary<string, Lua.lua_CFunction>();
	public static Dictionary<string, Lua.lua_CFunction> write_properties = new Dictionary<string, Lua.lua_CFunction>();
}

public class impl_LunaTraits_LWF_Point
{


	public static LWF.Point _bind_ctor_overload_1(Lua.lua_State L)
	{
		return new LWF.Point();
	}
	public static LWF.Point _bind_ctor_overload_2(Lua.lua_State L)
	{
		float x=(float)Lua.lua_tonumber(L,1);
		float y=(float)Lua.lua_tonumber(L,2);
		return new LWF.Point(x, y);
	}
	public static LWF.Point _bind_ctor(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 0) return _bind_ctor_overload_1(L);
		if (Lua.lua_gettop(L)!= 2
            || Lua.lua_isnumber(L, 1)==0
            || Lua.lua_isnumber(L, 2)==0) return _bind_ctor_overload_2(L);
		Lua.luaL_error(L, "ctor ( cannot find overloads:)");

		return null;
	}
	public static float _property_get_x(LWF.Point a) { return a.x; }
	public static void _property_set_x(LWF.Point a, float b) { a.x=b; }
	public static float _property_get_y(LWF.Point a) { return a.y; }
	public static void _property_set_y(LWF.Point a, float b) { a.y=b; }
	public static int _bind__property_get_x(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=32383421)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_x(LWF.Point a)");
		}

		LWF.Point a=Luna_LWF_Point.check(L,1);
		try {
			float ret=_property_get_x(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_x(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=32383421 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_x(LWF.Point a, float b)");
		}

		LWF.Point a=Luna_LWF_Point.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_x(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

	public static int _bind__property_get_y(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 1
            || Luna.get_uniqueid(L,1)!=32383421)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_get_y(LWF.Point a)");
		}

		LWF.Point a=Luna_LWF_Point.check(L,1);
		try {
			float ret=_property_get_y(a);
			Lua.lua_pushnumber(L, ret);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind__property_set_y(Lua.lua_State L)
	{
		if (Lua.lua_gettop(L)!= 2
            || Luna.get_uniqueid(L,1)!=32383421 
            || Lua.lua_isnumber(L, 2)==0)
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:_property_set_y(LWF.Point a, float b)");
		}

		LWF.Point a=Luna_LWF_Point.check(L,1);
		float b=(float)(float)Lua.lua_tonumber(L,2);
		try {
			_property_set_y(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}



	public static void luna_init_hashmap()
	{
        LunaTraits_LWF_Point.properties["x"]=_bind__property_get_x;
        LunaTraits_LWF_Point.properties["y"]=_bind__property_get_y;

	}

	public static void luna_init_write_hashmap()
	{
         LunaTraits_LWF_Point.write_properties["x"]=_bind__property_set_x;
         LunaTraits_LWF_Point.write_properties["y"]=_bind__property_set_y;

	}

	public static int __index(Lua.lua_State L)
	{

		{
			Lua.lua_CFunction fnc = null;
			if (LunaTraits_LWF_Point.properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
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
		if (LunaTraits_LWF_Point.write_properties.TryGetValue(Lua.lua_tostring(L,2).ToString(), out fnc))
		{
			Lua.lua_insert(L,2); // swap key and value
			Lua.lua_settop(L,2); // delete key
			return fnc(L);
		}

		Lua.luaL_error(L,"__newindex doesn't allow defining non-property member");
		return 0;
	}
}

class Luna_LWF_Point
{
	private static int idOffset = 0;
	private static Dictionary<Lua.lua_State, Dictionary<int, LWF.Point>> objects = new Dictionary<Lua.lua_State, Dictionary<int, LWF.Point>>();
	private static Dictionary<Lua.lua_State, Dictionary<LWF.Point, int>> objectIdentifiers = new Dictionary<Lua.lua_State, Dictionary<LWF.Point, int>>();

	public static void set(Lua.lua_State L, int table_index, Lua.CharPtr key)
	{
		Lua.lua_pushstring(L, key);
		Lua.lua_insert(L, -2);  // swap value and key
		Lua.lua_settable(L, table_index);
	}

	public static void Register(Lua.lua_State L)
	{
		objects.Add(L, new Dictionary<int, LWF.Point>());
		objectIdentifiers.Add(L, new Dictionary<LWF.Point, int>());
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
		Lua.lua_pushstring(L, LunaTraits_LWF_Point.className);
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
			LunaTraits_LWF_Point.RegType l = LunaTraits_LWF_Point.methods[i];
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

	public static void Destroy(Lua.lua_State L, LWF.Point obj)
	{

		int objectId = -1;
		if (objectIdentifiers[L].TryGetValue(obj, out objectId))
		{
			objectIdentifiers[L].Remove(obj);
			objects[L].Remove(objectId);
		}
	}

	static public LWF.Point check(Lua.lua_State L, int narg)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L,narg);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, narg, LunaTraits_LWF_Point.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		if(ud.TypeId !=LunaTraits_LWF_Point.uniqueID) // type checking with almost no overhead
		{
			Luna.print(String.Format("ud.uid: {0} != interface::uid : {1}\n", ud.TypeId, LunaTraits_LWF_Point.uniqueID));
			Lua.luaL_typerror(L, narg, LunaTraits_LWF_Point.className);
		}
		LWF.Point obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return null;
		return obj;
	}

	// use lunaStack::push if possible.
	public static void push(Lua.lua_State L, LWF.Point obj, bool gc, Lua.CharPtr metatable=null)
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
				metatable = LunaTraits_LWF_Point.className;
		Lua.lua_pushstring(L,"__luna");
		Lua.lua_gettable(L, Lua.LUA_GLOBALSINDEX);
		int __luna= Lua.lua_gettop(L);

		Luna.userdataType ud = new Luna.userdataType(
			objectId:objectId,  // store object in userdata
			gc:gc,   // collect garbage
			has_env:false, // does this userdata has a table attached to it?
			typeId:LunaTraits_LWF_Point.uniqueID
		);

		ud.ToBytes((byte[])Lua.lua_newuserdata(L, Luna.userdataType.Size));

		Lua.lua_pushstring(L, metatable);
		Lua.lua_gettable(L, __luna);
		Lua.lua_setmetatable(L, -2);
		//Luna.printStack(L);
		Lua.lua_insert(L, -2);  // swap __luna and userdata
		Lua.lua_pop(L,1);
	}

	private Luna_LWF_Point(){}  // hide default constructor

	// create a new T object and
	// push onto the Lua stack a userdata containing a pointer to T object
	private static int new_T(Lua.lua_State L)
	{
		Lua.lua_remove(L, 1);   // use classname:new(), instead of classname.new()
		LWF.Point obj = LunaTraits_LWF_Point._bind_ctor(L);  // call constructor for T objects
		push(L,obj,true);
		return 1;  // userdata containing pointer to T object
	}

	// garbage collection metamethod
	private static int gc_T(Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_Point.className); }
		Luna.userdataType ud = new Luna.userdataType(d);

		LWF.Point obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		if (ud.Gc) {
			LunaTraits_LWF_Point._bind_dtor(obj);  // call constructor for T objects
			Destroy(L, obj);
		}

		return 0;
	}

	private static int tostring_T (Lua.lua_State L)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L, 1);
		if(d == null) { Luna.print("checkRaw: ud==nil\n"); Lua.luaL_typerror(L, 1, LunaTraits_LWF_Point.className); }
		Luna.userdataType ud = new Luna.userdataType(d);
		LWF.Point obj = null;
		if (!objects[L].TryGetValue(ud.ObjectId, out obj))
			return 0;

		char[] buff = obj.ToString().ToCharArray(0,32);
		Lua.lua_pushfstring(L, "%s (%s)", new object[] {LunaTraits_LWF_Point.className, buff});
		return 1;
	}
}

#endif
