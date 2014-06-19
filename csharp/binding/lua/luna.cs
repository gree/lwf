#if LWF_USE_LUA

using System;
using System.Collections.Generic;
using KopiLua;

public class Luna
{
	public struct userdataType
	{
		private int typeId;
		private int objectId;
		private bool gc;
		private bool has_env;

		public int TypeId { get {return typeId;}}
		public int ObjectId { get {return objectId;}}
		public bool Gc { get {return gc;}}
		public bool HasEnv { get {return has_env;}}

		public userdataType(int typeId, int objectId, bool gc, bool has_env)
		{
			this.typeId = typeId;
			this.objectId = objectId;
			this.gc = gc;
			this.has_env = has_env;
		}

		public userdataType(byte[] src)
		{
			this.typeId = BitConverter.ToInt32(src, 0);
			this.objectId = BitConverter.ToInt32(src, 4);
			this.gc = BitConverter.ToBoolean(src, 8);
			this.has_env = BitConverter.ToBoolean(src, 9);
		}

		public void ToBytes(byte[] dst)
		{
			BitConverter.GetBytes(typeId).CopyTo(dst, 0);
			BitConverter.GetBytes(objectId).CopyTo(dst, 4);
			BitConverter.GetBytes(gc).CopyTo(dst, 8);
			BitConverter.GetBytes(has_env).CopyTo(dst, 9);
		}

		public static uint Size {
			get { return 4+4+1+1;}
		}
	}

	public static void printStack(Lua.lua_State L, bool compact = false)
	{
		if(compact)
			print(String.Format("stack top:{0} - ", Lua.lua_gettop(L)));
		else
			print(String.Format("stack trace: top {0}\n", Lua.lua_gettop(L)));

		for(int ist=1; ist<=Lua.lua_gettop(L); ist++) {
			if(compact)
				print("" + ist + ":" + Lua.luaL_typename(L,ist)[0]);
			else
				print("" + ist + ":" + Lua.luaL_typename(L,ist).ToString());
			if(Lua.lua_isnumber(L,ist) ==1)
				print("="+ (float)Lua.lua_tonumber(L,ist));
			else if(Lua.lua_isstring(L,ist) ==1)
				print("="+ Lua.lua_tostring(L,ist).ToString());
			else
				print(" ");
			if( !compact)print("\n");
		}
		print("\n");
	}

	public static void dostring(Lua.lua_State L, Lua.CharPtr luacode)
	{
		// Lua.luaL_dostring followed by pcall error checking
		if (Lua.luaL_loadstring(L, luacode) != 0 || Lua.lua_pcall(L, 0, Lua.LUA_MULTRET, 0) != 0)
		{
			print("Lua error: stack :");
			printStack(L,false);
		}
	}

	public static int get_uniqueid(Lua.lua_State L, int narg)
	{
		byte[] d = (byte[])Lua.lua_touserdata(L,narg);
		if (d == null) return -1;
		return new userdataType(d).TypeId;
	}

	public static void print(string s)
	{
		UnityEngine.Debug.Log(s);
	}
}

#endif
