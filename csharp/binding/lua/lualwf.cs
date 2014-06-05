#if LWF_USE_LUA

using System;
using KopiLua;


public class LuaLWF
{
	public static void open(Lua.lua_State L)
	{
		Luna.dostring(L,"if __luna==nil then __luna={} end");
		Luna.dostring(L,"    if __luna.copyMethodsFrom==nil then\n        function __luna.copyMethodsFrom(methodsChild, methodsParent)\n            for k,v in pairs(methodsParent) do\n                if k~='__index' and k~='__newindex' and methodsChild[k]==nil then\n                    methodsChild[k]=v\n                end\n            end\n        end\n        function __luna.overwriteMethodsFrom(methodsChild, methodsParent)\n            for k,v in pairs(methodsParent) do\n                if k~='__index' and k~='__newindex' then\n                    if verbose then print('registering', k, methodsChild[k]) end\n                    methodsChild[k]=v\n                end\n            end\n        end\n    end\n    ");
		impl_LunaTraits_LWF_LWF.luna_init_hashmap();
		impl_LunaTraits_LWF_LWF.luna_init_write_hashmap();
		Luna_LWF_LWF.Register(L);
		Luna.dostring(L, "if not LWF then LWF={} end LWF.LWF=__luna.LWF_LWF");
		Luna.dostring(L,"                __luna.LWF_LWF.luna_class='.LWF'");
		impl_LunaTraits_LWF_Button.luna_init_hashmap();
		impl_LunaTraits_LWF_Button.luna_init_write_hashmap();
		Luna_LWF_Button.Register(L);
		Luna.dostring(L, "if not LWF then LWF={} end LWF.Button=__luna.LWF_Button");
		Luna.dostring(L,"                __luna.LWF_Button.luna_class='.Button'");
		impl_LunaTraits_LWF_Movie.luna_init_hashmap();
		impl_LunaTraits_LWF_Movie.luna_init_write_hashmap();
		Luna_LWF_Movie.Register(L);
		Luna.dostring(L, "if not LWF then LWF={} end LWF.Movie=__luna.LWF_Movie");
		Luna.dostring(L,"                __luna.LWF_Movie.luna_class='.Movie'");
		impl_LunaTraits_LWF_Point.luna_init_hashmap();
		impl_LunaTraits_LWF_Point.luna_init_write_hashmap();
		Luna_LWF_Point.Register(L);
		Luna.dostring(L, "if not LWF then LWF={} end LWF.Point=__luna.LWF_Point");
		Luna.dostring(L,"                __luna.LWF_Point.luna_class='.Point'");
	}

	public static void close(Lua.lua_State L)
	{
		Luna_LWF_LWF.Unregister(L);
		Luna_LWF_Button.Unregister(L);
		Luna_LWF_Movie.Unregister(L);
		Luna_LWF_Point.Unregister(L);
	}
}

#endif
