#if defined(LWF_USE_LUA)
	
		#include <stdio.h>
		#include <string.h>
		#include <string>
		extern "C"
		{
			#include "lua.h"
			#include "lualib.h"
			#include "lauxlib.h"
			//#include "luadebug.h"
		}
		
#include "luna.h"
#include "lwf_luabinding.h"
	
template<>                                                    // 1220
 class impl_LunaTraits<LWF ::LWF > {
public:                                                       // 1223
    typedef Luna<LWF ::LWF > luna_t;                          // 1227
// : number denotes the line number of luna_gen.lua that generated the sentence // 1230
  inline static bool _lg_typecheck_setText(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isstring(L,3)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getText(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_playMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_stopMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_nextFrameMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_prevFrameMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setVisibleMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isboolean(L,3)==0) return false;                  // 546
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndStopMovie_overload_1(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndStopMovie_overload_2(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isstring(L,3)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndPlayMovie_overload_1(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndPlayMovie_overload_2(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isstring(L,3)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_moveMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=4) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    if( lua_isnumber(L,4)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_moveToMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=4) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    if( lua_isnumber(L,4)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_rotateMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_rotateToMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_scaleMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=4) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    if( lua_isnumber(L,4)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_scaleToMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=4) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    if( lua_isnumber(L,4)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setAlphaMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setColorTransformMovie(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=6) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    if( lua_isnumber(L,4)==0) return false;                   // 536
    if( lua_isnumber(L,5)==0) return false;                   // 536
    if( lua_isnumber(L,6)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_removeEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_clearEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_removeMovieEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_clearMovieEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_removeButtonEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_clearButtonEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getName(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getWidth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getHeight(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getPointX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getPointY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=8564116) return false; // LWF ::LWF // 534
    return true;
  }                                                           // 554
static std::string getName(LWF::LWF &o){return o.name;}
static float getWidth(LWF::LWF &o){return o.width;}
static float getHeight(LWF::LWF &o){return o.height;}
static float getPointX(LWF::LWF &o){return o.pointX;}
static float getPointY(LWF::LWF &o){return o.pointY;}

static int _bind_getRootMovie(lua_State *L)
{
    if (lua_gettop(L) != 1 ||
            Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::LWF>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.LWF.rootMovie");
    }
    LWF::LWF const &a = static_cast<LWF::LWF &>(*Luna<LWF::LWF>::check(L, 1));
    Luna<LWF::Movie>::push(L, a.rootMovie.get(), false);
    return 1;
}

static int _bind_get_root(lua_State *L)
{
    if (lua_gettop(L) != 1 ||
            Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::LWF>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.LWF._root");
    }
    LWF::LWF const &a = static_cast<LWF::LWF &>(*Luna<LWF::LWF>::check(L, 1));
    Luna<LWF::Movie>::push(L, a._root.get(), false);
    return 1;
}

static int addEventListener(lua_State *L)
{
    if (lua_gettop(L) != 3 ||
            Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::LWF>::uniqueID ||
            !lua_isstring(L, 2) || !lua_isfunction(L, 3)) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.addEventListener");
    }

    LWF::LWF &a = static_cast<LWF::LWF &>(*Luna<LWF::LWF>::check(L, 1));
    return a.AddEventHandlerLua();
}

static int addMovieEventListener(lua_State *L)
{
    if (lua_gettop(L) != 3 ||
            Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::LWF>::uniqueID ||
            !lua_isstring(L, 2) || !lua_istable(L, 3)) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.addMovieEventListener");
    }

    LWF::LWF &a = static_cast<LWF::LWF &>(*Luna<LWF::LWF>::check(L, 1));
    return a.AddMovieEventHandlerLua();
}

static int addButtonEventListener(lua_State *L)
{
    if (lua_gettop(L) != 3 ||
            Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::LWF>::uniqueID ||
            !lua_isstring(L, 2) || !lua_istable(L, 3)) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.addButtonEventListener");
    }

    LWF::LWF &a = static_cast<LWF::LWF &>(*Luna<LWF::LWF>::check(L, 1));
    return a.AddButtonEventHandlerLua();
}

                                                              // 1275
  static int _bind_setText(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setText(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setText(LWF ::LWF& self,std ::string textName,std ::string text,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string textName=(std ::string)lua_tostring(L,2);    // 507
    std ::string text=(std ::string)lua_tostring(L,3);        // 507
    try {                                                     // 254
    self.SetText( textName, text);                            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_getText(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getText(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getText(LWF ::LWF& self,std ::string textName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string textName=(std ::string)lua_tostring(L,2);    // 507
    try {                                                     // 305
    std ::string ret=self.GetText( textName);                 // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_playMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_playMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:playMovie(LWF ::LWF& self,std ::string instanceName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    try {                                                     // 254
    self.PlayMovie( instanceName);                            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_stopMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_stopMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:stopMovie(LWF ::LWF& self,std ::string instanceName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    try {                                                     // 254
    self.StopMovie( instanceName);                            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_nextFrameMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_nextFrameMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:nextFrameMovie(LWF ::LWF& self,std ::string instanceName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    try {                                                     // 254
    self.NextFrameMovie( instanceName);                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_prevFrameMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_prevFrameMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:prevFrameMovie(LWF ::LWF& self,std ::string instanceName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    try {                                                     // 254
    self.PrevFrameMovie( instanceName);                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setVisibleMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setVisibleMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setVisibleMovie(LWF ::LWF& self,std ::string instanceName,bool visible,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    bool visible=(bool)lua_toboolean(L,3);                    // 515
    try {                                                     // 254
    self.SetVisibleMovie( instanceName, visible);             // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndStopMovie_overload_1(lua_State *L)
  {                                                           // 1282
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    int frameNo=(int)lua_tonumber(L,3);                       // 512
    try {                                                     // 254
    self.GotoAndStopMovie( instanceName, frameNo);            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndStopMovie_overload_2(lua_State *L)
  {                                                           // 1282
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    std ::string label=(std ::string)lua_tostring(L,3);       // 507
    try {                                                     // 254
    self.GotoAndStopMovie( instanceName, label);              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndPlayMovie_overload_1(lua_State *L)
  {                                                           // 1282
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    int frameNo=(int)lua_tonumber(L,3);                       // 512
    try {                                                     // 254
    self.GotoAndPlayMovie( instanceName, frameNo);            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndPlayMovie_overload_2(lua_State *L)
  {                                                           // 1282
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    std ::string label=(std ::string)lua_tostring(L,3);       // 507
    try {                                                     // 254
    self.GotoAndPlayMovie( instanceName, label);              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_moveMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_moveMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:moveMovie(LWF ::LWF& self,std ::string instanceName,float vx,float vy,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float vx=(float)lua_tonumber(L,3);                        // 512
    float vy=(float)lua_tonumber(L,4);                        // 512
    try {                                                     // 254
    self.MoveMovie( instanceName, vx, vy);                    // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_moveToMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_moveToMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:moveToMovie(LWF ::LWF& self,std ::string instanceName,float vx,float vy,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float vx=(float)lua_tonumber(L,3);                        // 512
    float vy=(float)lua_tonumber(L,4);                        // 512
    try {                                                     // 254
    self.MoveToMovie( instanceName, vx, vy);                  // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_rotateMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_rotateMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:rotateMovie(LWF ::LWF& self,std ::string instanceName,float degree,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float degree=(float)lua_tonumber(L,3);                    // 512
    try {                                                     // 254
    self.RotateMovie( instanceName, degree);                  // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_rotateToMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_rotateToMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:rotateToMovie(LWF ::LWF& self,std ::string instanceName,float degree,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float degree=(float)lua_tonumber(L,3);                    // 512
    try {                                                     // 254
    self.RotateToMovie( instanceName, degree);                // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_scaleMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_scaleMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:scaleMovie(LWF ::LWF& self,std ::string instanceName,float vx,float vy,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float vx=(float)lua_tonumber(L,3);                        // 512
    float vy=(float)lua_tonumber(L,4);                        // 512
    try {                                                     // 254
    self.ScaleMovie( instanceName, vx, vy);                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_scaleToMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_scaleToMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:scaleToMovie(LWF ::LWF& self,std ::string instanceName,float vx,float vy,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float vx=(float)lua_tonumber(L,3);                        // 512
    float vy=(float)lua_tonumber(L,4);                        // 512
    try {                                                     // 254
    self.ScaleToMovie( instanceName, vx, vy);                 // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setAlphaMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setAlphaMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setAlphaMovie(LWF ::LWF& self,std ::string instanceName,float v,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float v=(float)lua_tonumber(L,3);                         // 512
    try {                                                     // 254
    self.SetAlphaMovie( instanceName, v);                     // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setColorTransformMovie(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setColorTransformMovie(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setColorTransformMovie(LWF ::LWF& self,std ::string instanceName,float vr,float vg,float vb,float va,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    float vr=(float)lua_tonumber(L,3);                        // 512
    float vg=(float)lua_tonumber(L,4);                        // 512
    float vb=(float)lua_tonumber(L,5);                        // 512
    float va=(float)lua_tonumber(L,6);                        // 512
    try {                                                     // 254
    self.SetColorTransformMovieLua( instanceName, vr, vg, vb, va); // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_removeEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_removeEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:removeEventListener(LWF ::LWF& self,std ::string eventName,int id,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string eventName=(std ::string)lua_tostring(L,2);   // 507
    int id=(int)lua_tonumber(L,3);                            // 512
    try {                                                     // 254
    self.RemoveEventHandler( eventName, id);                  // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_clearEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_clearEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:clearEventListener(LWF ::LWF& self,std ::string eventName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string eventName=(std ::string)lua_tostring(L,2);   // 507
    try {                                                     // 254
    self.ClearEventHandler( eventName);                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_removeMovieEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_removeMovieEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:removeMovieEventListener(LWF ::LWF& self,std ::string instanceName,int id,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    int id=(int)lua_tonumber(L,3);                            // 512
    try {                                                     // 254
    self.RemoveMovieEventHandler( instanceName, id);          // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_clearMovieEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_clearMovieEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:clearMovieEventListener(LWF ::LWF& self,std ::string instanceName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    try {                                                     // 254
    self.ClearMovieEventHandler( instanceName);               // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_removeButtonEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_removeButtonEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:removeButtonEventListener(LWF ::LWF& self,std ::string instanceName,int id,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    int id=(int)lua_tonumber(L,3);                            // 512
    try {                                                     // 254
    self.RemoveButtonEventHandler( instanceName, id);         // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_clearButtonEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_clearButtonEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:clearButtonEventListener(LWF ::LWF& self,std ::string instanceName,)"); }
                                                              // 487
    LWF ::LWF& self=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    std ::string instanceName=(std ::string)lua_tostring(L,2); // 507
    try {                                                     // 254
    self.ClearButtonEventHandler( instanceName);              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_getName(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getName(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getName(LWF ::LWF & o,)"); }
                                                              // 487
    LWF ::LWF & o=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=getName( o);                             // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_getWidth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getWidth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getWidth(LWF ::LWF & o,)"); }
                                                              // 487
    LWF ::LWF & o=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getWidth( o);                                   // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getHeight(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getHeight(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getHeight(LWF ::LWF & o,)"); }
                                                              // 487
    LWF ::LWF & o=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getHeight( o);                                  // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getPointX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getPointX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getPointX(LWF ::LWF & o,)"); }
                                                              // 487
    LWF ::LWF & o=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getPointX( o);                                  // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getPointY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getPointY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getPointY(LWF ::LWF & o,)"); }
                                                              // 487
    LWF ::LWF & o=static_cast<LWF ::LWF &>(*Luna<LWF ::LWF >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getPointY( o);                                  // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_gotoAndStopMovie(lua_State *L)
  {                                                           // 195
    if (_lg_typecheck_gotoAndStopMovie_overload_1(L)) return _bind_gotoAndStopMovie_overload_1(L); // 198
    if (_lg_typecheck_gotoAndStopMovie_overload_2(L)) return _bind_gotoAndStopMovie_overload_2(L); // 198
    luaL_error(L, "gotoAndStopMovie ( cannot find overloads:)\n(std ::string instanceName,int frameNo,)\n(std ::string instanceName,std ::string label,)\n");
                                                              // 205
    return 0;                                                 // 206
  }                                                           // 207
  static int _bind_gotoAndPlayMovie(lua_State *L)
  {                                                           // 195
    if (_lg_typecheck_gotoAndPlayMovie_overload_1(L)) return _bind_gotoAndPlayMovie_overload_1(L); // 198
    if (_lg_typecheck_gotoAndPlayMovie_overload_2(L)) return _bind_gotoAndPlayMovie_overload_2(L); // 198
    luaL_error(L, "gotoAndPlayMovie ( cannot find overloads:)\n(std ::string instanceName,int frameNo,)\n(std ::string instanceName,std ::string label,)\n");
                                                              // 205
    return 0;                                                 // 206
  }                                                           // 207
  static void luna_init_hashmap()
  {                                                           // 1324
    LunaTraits<LWF ::LWF >::properties["name"]=&_bind_getName; // 1326
    LunaTraits<LWF ::LWF >::properties["rootMovie"]=&_bind_getRootMovie; // 1326
    LunaTraits<LWF ::LWF >::properties["_root"]=&_bind_get_root; // 1326
    LunaTraits<LWF ::LWF >::properties["width"]=&_bind_getWidth; // 1326
    LunaTraits<LWF ::LWF >::properties["height"]=&_bind_getHeight; // 1326
    LunaTraits<LWF ::LWF >::properties["pointX"]=&_bind_getPointX; // 1326
    LunaTraits<LWF ::LWF >::properties["pointY"]=&_bind_getPointY; // 1326
  }                                                           // 1328
  static void luna_init_write_hashmap()
  {                                                           // 1329
  }                                                           // 1335
            static int __index(lua_State* L)
            {                                                 // 1339
                        {
                        luna__hashmap::iterator i=LunaTraits<LWF ::LWF >::properties.find((const char*)lua_tostring(L,2));

                        if (i!=LunaTraits<LWF ::LWF >::properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_pop(L,1); // remove self
                            return fnc(L); 
                        }
                            }
                                                              // 1352
                    int mt=lua_getmetatable(L, 1);
                    if(mt==0) luaL_error(L,"__index");//end
                    lua_pushstring(L, lua_tostring(L,2));
                    lua_rawget(L, -2);
                    return 1;
                                                              // 1385
            }                                                 // 1393
 
            static int __newindex(lua_State* L) {             // 1396
                    luaL_error(L,"__newindex doesn't allow defining non-property member");
                    return 0;
                                                              // 1441
            }                                                 // 1446
}; // end of class impl_LunaTraits<LWF ::LWF >                // 1450
  LWF ::LWF* LunaTraits<LWF ::LWF >::_bind_ctor(lua_State *L)
  {                                                           // 1467
   std::cerr<<"undefined contructor of LWF ::LWF called\n";   // 1468
    return NULL;                                              // 1469
  }                                                           // 1470
  void LunaTraits<LWF ::LWF >::_bind_dtor(LWF ::LWF* obj){    // 1471
   delete obj;                                                // 1472
  }                                                           // 1473
const char LunaTraits<LWF ::LWF >::className[] = "LWF_LWF";   // 1480
const int LunaTraits<LWF ::LWF >::uniqueID = 8564116;         // 1481
luna__hashmap LunaTraits<LWF ::LWF >::properties;             // 1484
luna__hashmap LunaTraits<LWF ::LWF >::write_properties;       // 1485
luna_RegType LunaTraits<LWF ::LWF >::methods[] = {            // 1487
    {"setText", &impl_LunaTraits<LWF ::LWF >::_bind_setText}, // 1492
    {"getText", &impl_LunaTraits<LWF ::LWF >::_bind_getText}, // 1492
    {"playMovie", &impl_LunaTraits<LWF ::LWF >::_bind_playMovie}, // 1492
    {"stopMovie", &impl_LunaTraits<LWF ::LWF >::_bind_stopMovie}, // 1492
    {"nextFrameMovie", &impl_LunaTraits<LWF ::LWF >::_bind_nextFrameMovie}, // 1492
    {"prevFrameMovie", &impl_LunaTraits<LWF ::LWF >::_bind_prevFrameMovie}, // 1492
    {"setVisibleMovie", &impl_LunaTraits<LWF ::LWF >::_bind_setVisibleMovie}, // 1492
    {"moveMovie", &impl_LunaTraits<LWF ::LWF >::_bind_moveMovie}, // 1492
    {"moveToMovie", &impl_LunaTraits<LWF ::LWF >::_bind_moveToMovie}, // 1492
    {"rotateMovie", &impl_LunaTraits<LWF ::LWF >::_bind_rotateMovie}, // 1492
    {"rotateToMovie", &impl_LunaTraits<LWF ::LWF >::_bind_rotateToMovie}, // 1492
    {"scaleMovie", &impl_LunaTraits<LWF ::LWF >::_bind_scaleMovie}, // 1492
    {"scaleToMovie", &impl_LunaTraits<LWF ::LWF >::_bind_scaleToMovie}, // 1492
    {"setAlphaMovie", &impl_LunaTraits<LWF ::LWF >::_bind_setAlphaMovie}, // 1492
    {"setColorTransformMovie", &impl_LunaTraits<LWF ::LWF >::_bind_setColorTransformMovie}, // 1492
    {"removeEventListener", &impl_LunaTraits<LWF ::LWF >::_bind_removeEventListener}, // 1492
    {"clearEventListener", &impl_LunaTraits<LWF ::LWF >::_bind_clearEventListener}, // 1492
    {"removeMovieEventListener", &impl_LunaTraits<LWF ::LWF >::_bind_removeMovieEventListener}, // 1492
    {"clearMovieEventListener", &impl_LunaTraits<LWF ::LWF >::_bind_clearMovieEventListener}, // 1492
    {"removeButtonEventListener", &impl_LunaTraits<LWF ::LWF >::_bind_removeButtonEventListener}, // 1492
    {"clearButtonEventListener", &impl_LunaTraits<LWF ::LWF >::_bind_clearButtonEventListener}, // 1492
    {"getName", &impl_LunaTraits<LWF ::LWF >::_bind_getName}, // 1492
    {"getWidth", &impl_LunaTraits<LWF ::LWF >::_bind_getWidth}, // 1492
    {"getHeight", &impl_LunaTraits<LWF ::LWF >::_bind_getHeight}, // 1492
    {"getPointX", &impl_LunaTraits<LWF ::LWF >::_bind_getPointX}, // 1492
    {"getPointY", &impl_LunaTraits<LWF ::LWF >::_bind_getPointY}, // 1492
    {"gotoAndStopMovie", &impl_LunaTraits<LWF ::LWF >::_bind_gotoAndStopMovie}, // 1492
    {"gotoAndPlayMovie", &impl_LunaTraits<LWF ::LWF >::_bind_gotoAndPlayMovie}, // 1492
    {"addEventListener", &impl_LunaTraits<LWF ::LWF >::addEventListener}, // 1492
    {"addMovieEventListener", &impl_LunaTraits<LWF ::LWF >::addMovieEventListener}, // 1492
    {"addButtonEventListener", &impl_LunaTraits<LWF ::LWF >::addButtonEventListener}, // 1492
    {"__index", &impl_LunaTraits<LWF ::LWF >::__index},       // 1492
    {"__newindex", &impl_LunaTraits<LWF ::LWF >::__newindex}, // 1492
    {0,0}                                                     // 1495
};                                                            // 1496
template<>                                                    // 1220
 class impl_LunaTraits<LWF ::Button > {
public:                                                       // 1223
    typedef Luna<LWF ::Button > luna_t;                       // 1227
// : number denotes the line number of luna_gen.lua that generated the sentence // 1230
  inline static bool _lg_typecheck_getFullName(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_removeEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_clearEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getName(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getHitX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getHitY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getWidth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getHeight(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=75773103) return false; // LWF ::Button // 534
    return true;
  }                                                           // 554
static std::string getName(LWF::Button &o){return o.name;}
static float getHitX(LWF::Button &o){return o.hitX;}
static float getHitY(LWF::Button &o){return o.hitY;}
static float getWidth(LWF::Button &o){return o.width;}
static float getHeight(LWF::Button &o){return o.height;}

static int _bind_getLWF(lua_State *L)
{
    if (lua_gettop(L) != 1 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Button>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Button.lwf");
    }
    LWF::Button const &a =
        static_cast<LWF::Button &>(*Luna<LWF::Button>::check(L, 1));
    Luna<LWF::LWF>::push(L, a.lwf, false);
    return 1;
}

static int _bind_getParent(lua_State *L)
{
    if (lua_gettop(L) != 1 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Button>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Button.parent");
    }
    LWF::Button const &a =
        static_cast<LWF::Button &>(*Luna<LWF::Button>::check(L, 1));
    if (a.parent)
        Luna<LWF::Movie>::push(L, a.parent, false);
    else
        lua_pushnil(L);
    return 1;
}

static int addEventListener(lua_State *L)
{
    if (lua_gettop(L) != 3 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Button>::uniqueID ||
            !lua_isstring(L, 2) || !lua_isfunction(L, 3)) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Button.addEventListener");
    }

    LWF::Button &a =
        static_cast<LWF::Button &>(*Luna<LWF::Button>::check(L, 1));
    return a.lwf->AddEventHandlerLua(0, &a);
}

                                                              // 1275
  static int _bind_getFullName(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getFullName(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getFullName(LWF ::Button& self,)"); }
                                                              // 487
    LWF ::Button& self=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=self.GetFullName();                      // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_removeEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_removeEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:removeEventListener(LWF ::Button& self,std ::string eventName,int id,)"); }
                                                              // 487
    LWF ::Button& self=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    std ::string eventName=(std ::string)lua_tostring(L,2);   // 507
    int id=(int)lua_tonumber(L,3);                            // 512
    try {                                                     // 254
    self.RemoveEventHandler( eventName, id);                  // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_clearEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_clearEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:clearEventListener(LWF ::Button& self,std ::string eventName,)"); }
                                                              // 487
    LWF ::Button& self=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    std ::string eventName=(std ::string)lua_tostring(L,2);   // 507
    try {                                                     // 254
    self.ClearEventHandler( eventName);                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_getName(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getName(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getName(LWF ::Button & o,)"); }
                                                              // 487
    LWF ::Button & o=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=getName( o);                             // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_getHitX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getHitX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getHitX(LWF ::Button & o,)"); }
                                                              // 487
    LWF ::Button & o=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getHitX( o);                                    // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getHitY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getHitY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getHitY(LWF ::Button & o,)"); }
                                                              // 487
    LWF ::Button & o=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getHitY( o);                                    // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getWidth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getWidth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getWidth(LWF ::Button & o,)"); }
                                                              // 487
    LWF ::Button & o=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getWidth( o);                                   // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getHeight(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getHeight(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getHeight(LWF ::Button & o,)"); }
                                                              // 487
    LWF ::Button & o=static_cast<LWF ::Button &>(*Luna<LWF ::Button >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getHeight( o);                                  // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static void luna_init_hashmap()
  {                                                           // 1324
    LunaTraits<LWF ::Button >::properties["name"]=&_bind_getName; // 1326
    LunaTraits<LWF ::Button >::properties["parent"]=&_bind_getParent; // 1326
    LunaTraits<LWF ::Button >::properties["lwf"]=&_bind_getLWF; // 1326
    LunaTraits<LWF ::Button >::properties["hitX"]=&_bind_getHitX; // 1326
    LunaTraits<LWF ::Button >::properties["hitY"]=&_bind_getHitY; // 1326
    LunaTraits<LWF ::Button >::properties["width"]=&_bind_getWidth; // 1326
    LunaTraits<LWF ::Button >::properties["height"]=&_bind_getHeight; // 1326
  }                                                           // 1328
  static void luna_init_write_hashmap()
  {                                                           // 1329
  }                                                           // 1335
            static int __index(lua_State* L)
            {                                                 // 1339
                        {
                        luna__hashmap::iterator i=LunaTraits<LWF ::Button >::properties.find((const char*)lua_tostring(L,2));

                        if (i!=LunaTraits<LWF ::Button >::properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_pop(L,1); // remove self
                            return fnc(L); 
                        }
                            }
                                                              // 1352
                    int mt=lua_getmetatable(L, 1);
                    if(mt==0) luaL_error(L,"__index");//end
                    lua_pushstring(L, lua_tostring(L,2));
                    lua_rawget(L, -2);
                    return 1;
                                                              // 1385
            }                                                 // 1393
 
            static int __newindex(lua_State* L) {             // 1396
                    luaL_error(L,"__newindex doesn't allow defining non-property member");
                    return 0;
                                                              // 1441
            }                                                 // 1446
}; // end of class impl_LunaTraits<LWF ::Button >             // 1450
  LWF ::Button* LunaTraits<LWF ::Button >::_bind_ctor(lua_State *L)
  {                                                           // 1467
   std::cerr<<"undefined contructor of LWF ::Button called\n"; // 1468
    return NULL;                                              // 1469
  }                                                           // 1470
  void LunaTraits<LWF ::Button >::_bind_dtor(LWF ::Button* obj){ // 1471
   delete obj;                                                // 1472
  }                                                           // 1473
const char LunaTraits<LWF ::Button >::className[] = "LWF_Button"; // 1480
const int LunaTraits<LWF ::Button >::uniqueID = 75773103;     // 1481
luna__hashmap LunaTraits<LWF ::Button >::properties;          // 1484
luna__hashmap LunaTraits<LWF ::Button >::write_properties;    // 1485
luna_RegType LunaTraits<LWF ::Button >::methods[] = {         // 1487
    {"getFullName", &impl_LunaTraits<LWF ::Button >::_bind_getFullName}, // 1492
    {"removeEventListener", &impl_LunaTraits<LWF ::Button >::_bind_removeEventListener}, // 1492
    {"clearEventListener", &impl_LunaTraits<LWF ::Button >::_bind_clearEventListener}, // 1492
    {"getName", &impl_LunaTraits<LWF ::Button >::_bind_getName}, // 1492
    {"getHitX", &impl_LunaTraits<LWF ::Button >::_bind_getHitX}, // 1492
    {"getHitY", &impl_LunaTraits<LWF ::Button >::_bind_getHitY}, // 1492
    {"getWidth", &impl_LunaTraits<LWF ::Button >::_bind_getWidth}, // 1492
    {"getHeight", &impl_LunaTraits<LWF ::Button >::_bind_getHeight}, // 1492
    {"addEventListener", &impl_LunaTraits<LWF ::Button >::addEventListener}, // 1492
    {"__index", &impl_LunaTraits<LWF ::Button >::__index},    // 1492
    {"__newindex", &impl_LunaTraits<LWF ::Button >::__newindex}, // 1492
    {0,0}                                                     // 1495
};                                                            // 1496
template<>                                                    // 1220
 class impl_LunaTraits<LWF ::Movie > {
public:                                                       // 1223
    typedef Luna<LWF ::Movie > luna_t;                        // 1227
// : number denotes the line number of luna_gen.lua that generated the sentence // 1230
  inline static bool _lg_typecheck_getFullName(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_globalToLocal(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( Luna<void>::get_uniqueid(L,2)!=34538949) return false; // LWF ::Point // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_localToGlobal(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( Luna<void>::get_uniqueid(L,2)!=34538949) return false; // LWF ::Point // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_play(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_stop(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_nextFrame(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_prevFrame(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoFrame(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndStop_overload_1(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndStop_overload_2(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndPlay_overload_1(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_gotoAndPlay_overload_2(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_move(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_moveTo(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_rotate(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_rotateTo(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_scale(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_scaleTo(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_removeEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_clearEventListener(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_swapAttachedMovieDepth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_detachMovie_overload_1(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_detachMovie_overload_2(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( Luna<void>::get_uniqueid(L,2)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_detachFromParent(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_detachLWF(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_detachAllLWFs(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_removeMovieClip(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_swapAttachedBitmapDepth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=3) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    if( lua_isnumber(L,3)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_detachBitmap(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getName(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getCurrentFrame(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getCurrentLabel(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getTotalFrames(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getVisible(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getScaleX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getScaleY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getRotation(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getAlpha(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getRed(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getGreen(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getBlue(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_getBlendMode(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setVisible(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isboolean(L,2)==0) return false;                  // 546
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setScaleX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setScaleY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setRotation(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setAlpha(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setRed(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setGreen(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setBlue(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_setBlendMode(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=31780709) return false; // LWF ::Movie // 534
    if( lua_isstring(L,2)==0) return false;                   // 541
    return true;
  }                                                           // 554
static std::string getName(LWF::Movie &o){return o.name;}
static int getCurrentFrame(LWF::Movie &o){return o.currentFrame;}
static std::string getCurrentLabel(LWF::Movie &o){return o.GetCurrentLabel();}
static int getTotalFrames(LWF::Movie &o){return o.totalFrames;}
static bool getVisible(LWF::Movie &o){return o.visible;}
static float getX(LWF::Movie &o){return o.GetX();}
static float getY(LWF::Movie &o){return o.GetY();}
static float getScaleX(LWF::Movie &o){return o.GetScaleX();}
static float getScaleY(LWF::Movie &o){return o.GetScaleY();}
static float getRotation(LWF::Movie &o){return o.GetRotation();}
static float getAlpha(LWF::Movie &o){return o.GetAlpha();}
static float getRed(LWF::Movie &o){return o.GetRed();}
static float getGreen(LWF::Movie &o){return o.GetGreen();}
static float getBlue(LWF::Movie &o){return o.GetBlue();}

static void setVisible(LWF::Movie &o, bool v){o.SetVisible(v);}
static void setX(LWF::Movie &o, float v){o.SetX(v);}
static void setY(LWF::Movie &o, float v){o.SetY(v);}
static void setScaleX(LWF::Movie &o, float v){o.SetScaleX(v);}
static void setScaleY(LWF::Movie &o, float v){o.SetScaleY(v);}
static void setRotation(LWF::Movie &o, float v){o.SetRotation(v);}
static void setAlpha(LWF::Movie &o, float v){o.SetAlpha(v);}
static void setRed(LWF::Movie &o, float v){o.SetRed(v);}
static void setGreen(LWF::Movie &o, float v){o.SetGreen(v);}
static void setBlue(LWF::Movie &o, float v){o.SetBlue(v);}

static std::string getBlendMode(LWF::Movie &o)
{
    switch (o.blendMode) {
    default:
        return "normal";
    case (int)LWF::Format::BLEND_MODE_ADD:
        return "add";
    case (int)LWF::Format::BLEND_MODE_ERASE:
        return "erase";
    case (int)LWF::Format::BLEND_MODE_LAYER:
        return "layer";
    case (int)LWF::Format::BLEND_MODE_MASK:
        return "mask";
    case (int)LWF::Format::BLEND_MODE_MULTIPLY:
        return "multiply";
    case (int)LWF::Format::BLEND_MODE_SCREEN:
        return "screen";
    case (int)LWF::Format::BLEND_MODE_SUBTRACT:
        return "subtract";
    }
}

static void setBlendMode(LWF::Movie &o, std::string v)
{
    static LWF::map<std::string, int> table;
    if (table.empty()) {
        table["normal"] = (int)LWF::Format::BLEND_MODE_NORMAL;
        table["add"] = (int)LWF::Format::BLEND_MODE_ADD;
        table["erase"] = (int)LWF::Format::BLEND_MODE_ERASE;
        table["layer"] = (int)LWF::Format::BLEND_MODE_LAYER;
        table["mask"] = (int)LWF::Format::BLEND_MODE_MASK;
        table["multiply"] = (int)LWF::Format::BLEND_MODE_MULTIPLY;
        table["screen"] = (int)LWF::Format::BLEND_MODE_SCREEN;
        table["subtract"] = (int)LWF::Format::BLEND_MODE_SUBTRACT;
    }

    LWF::map<std::string, int>::iterator it = table.find(v);
    if (it == table.end())
        o.blendMode = (int)LWF::Format::BLEND_MODE_NORMAL;
    else
        o.blendMode = it->second;
}

static int _bind_getLWF(lua_State *L)
{
    if (lua_gettop(L) != 1 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Movie>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Movie.lwf");
    }
    LWF::Movie const &a =
        static_cast<LWF::Movie &>(*Luna<LWF::Movie>::check(L, 1));
    Luna<LWF::LWF>::push(L, a.lwf, false);
    return 1;
}

static int _bind_getParent(lua_State *L)
{
    if (lua_gettop(L) != 1 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Movie>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Movie.parent");
    }
    LWF::Movie const &a =
        static_cast<LWF::Movie &>(*Luna<LWF::Movie>::check(L, 1));
    if (a.parent)
        Luna<LWF::Movie>::push(L, a.parent, false);
    else
        lua_pushnil(L);
    return 1;
}

static int _bind_getCurrentLabels(lua_State *L)
{
    if (lua_gettop(L) != 1 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Movie>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Movie.currentLabels");
    }
    LWF::Movie &a = static_cast<LWF::Movie &>(*Luna<LWF::Movie>::check(L, 1));
    const LWF::CurrentLabels currentLabels = a.GetCurrentLabels();

    lua_createtable(L, (int)currentLabels.size(), 0);
    /* -1: table */
    LWF::CurrentLabels::const_iterator
        it(currentLabels.begin()), itend(currentLabels.end());
    for (int i = 1; it != itend; ++it, ++i) {
        lua_pushnumber(L, i);
        /* -2: table */
        /* -1: index */
        lua_createtable(L, 0, 2);
        /* -3: table */
        /* -2: index */
        /* -1: table */
        lua_pushnumber(L, it->frame);
        /* -4: table */
        /* -3: index */
        /* -2: table */
        /* -1: frame */
        lua_setfield(L, -2, "frame");
        /* -3: table */
        /* -2: index */
        /* -1: table */
        lua_pushstring(L, it->name.c_str());
        /* -4: table */
        /* -3: index */
        /* -2: table */
        /* -1: name */
        lua_setfield(L, -2, "name");
        /* -3: table */
        /* -2: index */
        /* -1: table */
        lua_settable(L, -3);
        /* -1: table */
    }
    /* -1: table */
    return 1;
}

static int attachMovie(lua_State *L)
{ 
    LWF::Movie *a;
    int args = lua_gettop(L);
    if (args < 3 || args > 6)
        goto error;
    if (Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::Movie>::uniqueID)
        goto error;
    if (!lua_isstring(L, 2) || !lua_isstring(L, 3))
        goto error;
    if (args >= 4 && !lua_istable(L, 4))
        goto error;
    if (args >= 5 && !lua_isnumber(L, 5))
        goto error;
    if (args >= 6 && !lua_isboolean(L, 6))
        goto error;

    a = Luna<LWF::Movie>::check(L, 1);
    return a->lwf->AttachMovieLua(a, false);

error:
    luna_printStack(L);
    luaL_error(L, "luna typecheck failed: LWF.Movie.attachMovie");
    return 1;
}

static int attachEmptyMovie(lua_State *L)
{ 
    LWF::Movie *a;
    int args = lua_gettop(L);
    if (args < 2 || args > 5)
        goto error;
    if (Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::Movie>::uniqueID)
        goto error;
    if (!lua_isstring(L, 2))
        goto error;
    if (args >= 3 && !lua_istable(L, 3))
        goto error;
    if (args >= 4 && !lua_isnumber(L, 4))
        goto error;
    if (args >= 5 && !lua_isboolean(L, 5))
        goto error;

    a = Luna<LWF::Movie>::check(L, 1);
    return a->lwf->AttachMovieLua(a, true);

error:
    luna_printStack(L);
    luaL_error(L, "luna typecheck failed: LWF.Movie.attachEmptyMovie");
    return 1;
}

static int attachLWF(lua_State *L)
{ 
    LWF::Movie *a;
    int args = lua_gettop(L);
    if (args < 3 || args > 5)
        goto error;
    if (Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::Movie>::uniqueID)
        goto error;
    if (!lua_isstring(L, 2) || !lua_isstring(L, 3))
        goto error;
    if (args >= 4 && !lua_isnumber(L, 4))
        goto error;
    if (args >= 5 && !lua_isboolean(L, 5))
        goto error;

    a = Luna<LWF::Movie>::check(L, 1);
    return a->lwf->AttachLWFLua(a);

error:
    luna_printStack(L);
    luaL_error(L, "luna typecheck failed: LWF.Movie.attachLWF");
    return 1;
}

static int attachBitmap(lua_State *L)
{
    LWF::Movie *a;
    int args = lua_gettop(L);
    if (args != 3)
        goto error;
    if (Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::Movie>::uniqueID)
        goto error;
    if (!lua_isstring(L, 2) || !lua_isnumber(L, 3))
        goto error;

    a = Luna<LWF::Movie>::check(L, 1);
    return a->lwf->AttachBitmapLua(a);

error:
    luna_printStack(L);
    luaL_error(L, "luna typecheck failed: LWF.Movie.attachBitmap");
    return 1;
}

static int getAttachedBitmap(lua_State *L)
{
    LWF::Movie *a;
    if (lua_gettop(L) != 2 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Movie>::uniqueID || !lua_isnumber(L, 2)) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Movie.getAttachedBitmap");
    }
    a = Luna<LWF::Movie>::check(L, 1);
    LWF::shared_ptr<LWF::BitmapClip> bitmapClip =
        a->GetAttachedBitmap(lua_tonumber(L, 2));
    if (bitmapClip)
        Luna<LWF::BitmapClip>::push(L, bitmapClip.get(), false);
    else
        lua_pushnil(L);
    return 1;
}

static int addEventListener(lua_State *L)
{
    if (lua_gettop(L) != 3 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::Movie>::uniqueID ||
            !lua_isstring(L, 2) || !lua_isfunction(L, 3)) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.Movie.addEventListener");
    }

    LWF::Movie &a = static_cast<LWF::Movie &>(*Luna<LWF::Movie>::check(L, 1));
    return a.lwf->AddEventHandlerLua(&a);
}

static int dispatchEvent(lua_State *L)
{
    LWF::Movie *a;
    LWF::string eventName;
    int args = lua_gettop(L);
    if (args != 2)
        goto error;
    if (Luna<void>::get_uniqueid(L, 1) != LunaTraits<LWF::Movie>::uniqueID)
        goto error;
    if (lua_isstring(L, 2)) {
        eventName = lua_tostring(L, 2);
    } else if (lua_istable(L, 2)) {
        lua_getfield(L, 2, "type");
        if (!lua_isstring(L, -1))
            goto error;
        eventName = lua_tostring(L, -1);
        lua_pop(L, 1);
    } else {
        goto error;
    }

    a = Luna<LWF::Movie>::check(L, 1);
    a->DispatchEvent(eventName);
    return 0;

error:
    luna_printStack(L);
    luaL_error(L, "luna typecheck failed: LWF.Movie.dispatchEvent");
    return 1;
}

                                                              // 1275
  static int _bind_getFullName(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getFullName(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getFullName(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=self.GetFullName();                      // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_globalToLocal(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_globalToLocal(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:globalToLocal(LWF ::Movie& self,const LWF ::Point & point,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    const LWF ::Point & point=static_cast<LWF ::Point &>(*Luna<LWF ::Point >::check(L,2)); // 504
    try {                                                     // 287
    LWF ::Point* ret=new LWF ::Point(self.GlobalToLocal( point)); // 292
    Luna<LWF ::Point >::push(L,ret,true,"LWF_Point");         // 293
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 294
    return 1;                                                 // 297
  }                                                           // 333
  static int _bind_localToGlobal(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_localToGlobal(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:localToGlobal(LWF ::Movie& self,const LWF ::Point & point,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    const LWF ::Point & point=static_cast<LWF ::Point &>(*Luna<LWF ::Point >::check(L,2)); // 504
    try {                                                     // 287
    LWF ::Point* ret=new LWF ::Point(self.LocalToGlobal( point)); // 292
    Luna<LWF ::Point >::push(L,ret,true,"LWF_Point");         // 293
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 294
    return 1;                                                 // 297
  }                                                           // 333
  static int _bind_play(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_play(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:play(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 254
    self.Play();                                              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_stop(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_stop(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:stop(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 254
    self.Stop();                                              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_nextFrame(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_nextFrame(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:nextFrame(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 254
    self.NextFrame();                                         // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_prevFrame(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_prevFrame(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:prevFrame(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 254
    self.PrevFrame();                                         // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoFrame(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_gotoFrame(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:gotoFrame(LWF ::Movie& self,int frameNo,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    int frameNo=(int)lua_tonumber(L,2);                       // 512
    try {                                                     // 254
    self.GotoFrame( frameNo);                                 // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndStop_overload_1(lua_State *L)
  {                                                           // 1282
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    int frameNo=(int)lua_tonumber(L,2);                       // 512
    try {                                                     // 254
    self.GotoAndStop( frameNo);                               // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndStop_overload_2(lua_State *L)
  {                                                           // 1282
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    std ::string label=(std ::string)lua_tostring(L,2);       // 507
    try {                                                     // 254
    self.GotoAndStop( label);                                 // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndPlay_overload_1(lua_State *L)
  {                                                           // 1282
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    int frameNo=(int)lua_tonumber(L,2);                       // 512
    try {                                                     // 254
    self.GotoAndPlay( frameNo);                               // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_gotoAndPlay_overload_2(lua_State *L)
  {                                                           // 1282
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    std ::string label=(std ::string)lua_tostring(L,2);       // 507
    try {                                                     // 254
    self.GotoAndPlay( label);                                 // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_move(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_move(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:move(LWF ::Movie& self,float vx,float vy,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float vx=(float)lua_tonumber(L,2);                        // 512
    float vy=(float)lua_tonumber(L,3);                        // 512
    try {                                                     // 254
    self.Move( vx, vy);                                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_moveTo(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_moveTo(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:moveTo(LWF ::Movie& self,float vx,float vy,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float vx=(float)lua_tonumber(L,2);                        // 512
    float vy=(float)lua_tonumber(L,3);                        // 512
    try {                                                     // 254
    self.MoveTo( vx, vy);                                     // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_rotate(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_rotate(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:rotate(LWF ::Movie& self,float degree,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float degree=(float)lua_tonumber(L,2);                    // 512
    try {                                                     // 254
    self.Rotate( degree);                                     // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_rotateTo(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_rotateTo(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:rotateTo(LWF ::Movie& self,float degree,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float degree=(float)lua_tonumber(L,2);                    // 512
    try {                                                     // 254
    self.RotateTo( degree);                                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_scale(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_scale(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:scale(LWF ::Movie& self,float vx,float vy,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float vx=(float)lua_tonumber(L,2);                        // 512
    float vy=(float)lua_tonumber(L,3);                        // 512
    try {                                                     // 254
    self.Scale( vx, vy);                                      // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_scaleTo(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_scaleTo(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:scaleTo(LWF ::Movie& self,float vx,float vy,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float vx=(float)lua_tonumber(L,2);                        // 512
    float vy=(float)lua_tonumber(L,3);                        // 512
    try {                                                     // 254
    self.ScaleTo( vx, vy);                                    // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_removeEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_removeEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:removeEventListener(LWF ::Movie& self,std ::string eventName,int id,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    std ::string eventName=(std ::string)lua_tostring(L,2);   // 507
    int id=(int)lua_tonumber(L,3);                            // 512
    try {                                                     // 254
    self.RemoveEventHandler( eventName, id);                  // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_clearEventListener(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_clearEventListener(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:clearEventListener(LWF ::Movie& self,std ::string eventName,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    std ::string eventName=(std ::string)lua_tostring(L,2);   // 507
    try {                                                     // 254
    self.ClearEventHandler( eventName);                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_swapAttachedMovieDepth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_swapAttachedMovieDepth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:swapAttachedMovieDepth(LWF ::Movie& self,int depth0,int depth1,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    int depth0=(int)lua_tonumber(L,2);                        // 512
    int depth1=(int)lua_tonumber(L,3);                        // 512
    try {                                                     // 254
    self.SwapAttachedMovieDepth( depth0, depth1);             // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_detachMovie_overload_1(lua_State *L)
  {                                                           // 1282
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    std ::string aName=(std ::string)lua_tostring(L,2);       // 507
    try {                                                     // 254
    self.DetachMovie( aName);                                 // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_detachMovie_overload_2(lua_State *L)
  {                                                           // 1282
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    LWF ::Movie * movie=static_cast<LWF ::Movie *>(Luna<LWF ::Movie >::check(L,2)); // 500
    try {                                                     // 254
    self.DetachMovie( movie);                                 // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_detachFromParent(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_detachFromParent(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:detachFromParent(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 254
    self.DetachFromParent();                                  // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_detachLWF(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_detachLWF(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:detachLWF(LWF ::Movie& self,std ::string aName,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    std ::string aName=(std ::string)lua_tostring(L,2);       // 507
    try {                                                     // 254
    self.DetachLWF( aName);                                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_detachAllLWFs(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_detachAllLWFs(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:detachAllLWFs(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 254
    self.DetachAllLWFs();                                     // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_removeMovieClip(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_removeMovieClip(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:removeMovieClip(LWF ::Movie& self,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 254
    self.RemoveMovieClip();                                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_swapAttachedBitmapDepth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_swapAttachedBitmapDepth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:swapAttachedBitmapDepth(LWF ::Movie& self,int depth0,int depth1,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    int depth0=(int)lua_tonumber(L,2);                        // 512
    int depth1=(int)lua_tonumber(L,3);                        // 512
    try {                                                     // 254
    self.SwapAttachedBitmapDepth( depth0, depth1);            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_detachBitmap(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_detachBitmap(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:detachBitmap(LWF ::Movie& self,int depth,)"); }
                                                              // 487
    LWF ::Movie& self=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    int depth=(int)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    self.DetachBitmap( depth);                                // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_getName(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getName(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getName(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=getName( o);                             // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_getCurrentFrame(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getCurrentFrame(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getCurrentFrame(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    int ret=getCurrentFrame( o);                              // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getCurrentLabel(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getCurrentLabel(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getCurrentLabel(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=getCurrentLabel( o);                     // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_getTotalFrames(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getTotalFrames(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getTotalFrames(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    int ret=getTotalFrames( o);                               // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getVisible(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getVisible(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getVisible(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 317
    bool ret=getVisible( o);                                  // 318
    lua_pushboolean(L, ret);                                  // 319
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 320
    return 1;                                                 // 321
  }                                                           // 333
  static int _bind_getX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getX(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getX( o);                                       // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getY(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getY( o);                                       // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getScaleX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getScaleX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getScaleX(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getScaleX( o);                                  // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getScaleY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getScaleY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getScaleY(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getScaleY( o);                                  // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getRotation(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getRotation(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getRotation(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getRotation( o);                                // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getAlpha(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getAlpha(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getAlpha(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getAlpha( o);                                   // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getRed(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getRed(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getRed(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getRed( o);                                     // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getGreen(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getGreen(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getGreen(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getGreen( o);                                   // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getBlue(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getBlue(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getBlue(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 299
    float ret=getBlue( o);                                    // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind_getBlendMode(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getBlendMode(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getBlendMode(LWF ::Movie & o,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=getBlendMode( o);                        // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind_setVisible(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setVisible(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setVisible(LWF ::Movie & o,bool v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    bool v=(bool)lua_toboolean(L,2);                          // 515
    try {                                                     // 254
    setVisible( o, v);                                        // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setX(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setX( o, v);                                              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setY(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setY( o, v);                                              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setScaleX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setScaleX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setScaleX(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setScaleX( o, v);                                         // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setScaleY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setScaleY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setScaleY(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setScaleY( o, v);                                         // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setRotation(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setRotation(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setRotation(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setRotation( o, v);                                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setAlpha(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setAlpha(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setAlpha(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setAlpha( o, v);                                          // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setRed(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setRed(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setRed(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setRed( o, v);                                            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setGreen(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setGreen(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setGreen(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setGreen( o, v);                                          // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setBlue(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setBlue(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setBlue(LWF ::Movie & o,float v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    float v=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    setBlue( o, v);                                           // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_setBlendMode(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_setBlendMode(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:setBlendMode(LWF ::Movie & o,std ::string v,)"); }
                                                              // 487
    LWF ::Movie & o=static_cast<LWF ::Movie &>(*Luna<LWF ::Movie >::check(L,1)); // 504
    std ::string v=(std ::string)lua_tostring(L,2);           // 507
    try {                                                     // 254
    setBlendMode( o, v);                                      // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind_detachMovie(lua_State *L)
  {                                                           // 195
    if (_lg_typecheck_detachMovie_overload_1(L)) return _bind_detachMovie_overload_1(L); // 198
    if (_lg_typecheck_detachMovie_overload_2(L)) return _bind_detachMovie_overload_2(L); // 198
    luaL_error(L, "detachMovie ( cannot find overloads:)\n(std ::string aName,)\n(LWF ::Movie * movie,)\n");
                                                              // 205
    return 0;                                                 // 206
  }                                                           // 207
  static int _bind_gotoAndStop(lua_State *L)
  {                                                           // 195
    if (_lg_typecheck_gotoAndStop_overload_1(L)) return _bind_gotoAndStop_overload_1(L); // 198
    if (_lg_typecheck_gotoAndStop_overload_2(L)) return _bind_gotoAndStop_overload_2(L); // 198
    luaL_error(L, "gotoAndStop ( cannot find overloads:)\n(int frameNo,)\n(std ::string label,)\n");
                                                              // 205
    return 0;                                                 // 206
  }                                                           // 207
  static int _bind_gotoAndPlay(lua_State *L)
  {                                                           // 195
    if (_lg_typecheck_gotoAndPlay_overload_1(L)) return _bind_gotoAndPlay_overload_1(L); // 198
    if (_lg_typecheck_gotoAndPlay_overload_2(L)) return _bind_gotoAndPlay_overload_2(L); // 198
    luaL_error(L, "gotoAndPlay ( cannot find overloads:)\n(int frameNo,)\n(std ::string label,)\n");
                                                              // 205
    return 0;                                                 // 206
  }                                                           // 207
  static void luna_init_hashmap()
  {                                                           // 1324
    LunaTraits<LWF ::Movie >::properties["name"]=&_bind_getName; // 1326
    LunaTraits<LWF ::Movie >::properties["parent"]=&_bind_getParent; // 1326
    LunaTraits<LWF ::Movie >::properties["currentFrame"]=&_bind_getCurrentFrame; // 1326
    LunaTraits<LWF ::Movie >::properties["currentLabel"]=&_bind_getCurrentLabel; // 1326
    LunaTraits<LWF ::Movie >::properties["currentLabels"]=&_bind_getCurrentLabels; // 1326
    LunaTraits<LWF ::Movie >::properties["totalFrames"]=&_bind_getTotalFrames; // 1326
    LunaTraits<LWF ::Movie >::properties["visible"]=&_bind_getVisible; // 1326
    LunaTraits<LWF ::Movie >::properties["x"]=&_bind_getX;    // 1326
    LunaTraits<LWF ::Movie >::properties["y"]=&_bind_getY;    // 1326
    LunaTraits<LWF ::Movie >::properties["scaleX"]=&_bind_getScaleX; // 1326
    LunaTraits<LWF ::Movie >::properties["scaleY"]=&_bind_getScaleY; // 1326
    LunaTraits<LWF ::Movie >::properties["rotation"]=&_bind_getRotation; // 1326
    LunaTraits<LWF ::Movie >::properties["alpha"]=&_bind_getAlpha; // 1326
    LunaTraits<LWF ::Movie >::properties["red"]=&_bind_getRed; // 1326
    LunaTraits<LWF ::Movie >::properties["green"]=&_bind_getGreen; // 1326
    LunaTraits<LWF ::Movie >::properties["blue"]=&_bind_getBlue; // 1326
    LunaTraits<LWF ::Movie >::properties["lwf"]=&_bind_getLWF; // 1326
    LunaTraits<LWF ::Movie >::properties["blendMode"]=&_bind_getBlendMode; // 1326
  }                                                           // 1328
  static void luna_init_write_hashmap()
  {                                                           // 1329
    LunaTraits<LWF ::Movie >::write_properties["visible"]=&_bind_setVisible; // 1332
    LunaTraits<LWF ::Movie >::write_properties["x"]=&_bind_setX; // 1332
    LunaTraits<LWF ::Movie >::write_properties["y"]=&_bind_setY; // 1332
    LunaTraits<LWF ::Movie >::write_properties["scaleX"]=&_bind_setScaleX; // 1332
    LunaTraits<LWF ::Movie >::write_properties["scaleY"]=&_bind_setScaleY; // 1332
    LunaTraits<LWF ::Movie >::write_properties["rotation"]=&_bind_setRotation; // 1332
    LunaTraits<LWF ::Movie >::write_properties["alpha"]=&_bind_setAlpha; // 1332
    LunaTraits<LWF ::Movie >::write_properties["red"]=&_bind_setRed; // 1332
    LunaTraits<LWF ::Movie >::write_properties["green"]=&_bind_setGreen; // 1332
    LunaTraits<LWF ::Movie >::write_properties["blue"]=&_bind_setBlue; // 1332
    LunaTraits<LWF ::Movie >::write_properties["blendMode"]=&_bind_setBlendMode; // 1332
  }                                                           // 1335
            static int __index(lua_State* L)
            {                                                 // 1339
if (lua_gettop(L) == 2 && Luna<void>::get_uniqueid(L, 1) ==
        LunaTraits<LWF::Movie>::uniqueID) {
    LWF::Movie &o =
        static_cast<LWF::Movie &>(*Luna<LWF::Movie>::check(L, 1));
    std::string name = lua_tostring(L, 2);
    if (o.lwf->GetFieldLua(&o, name))
        return 1;
    LWF::Movie *movie = o.SearchMovieInstance(name, false);
    if (movie) {
        lua_pop(L, 1);
        Luna<LWF::Movie>::push(L, movie, false);
        return 1;
    }
    LWF::Button *button = o.SearchButtonInstance(name, false);
    if (button) {
        lua_pop(L, 1);
        Luna<LWF::Button>::push(L, button, false);
        return 1;
    }
}
                                                              // 1347
                        {
                        luna__hashmap::iterator i=LunaTraits<LWF ::Movie >::properties.find((const char*)lua_tostring(L,2));

                        if (i!=LunaTraits<LWF ::Movie >::properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_pop(L,1); // remove self
                            return fnc(L); 
                        }
                            }
                                                              // 1352
                    int mt=lua_getmetatable(L, 1);
                    if(mt==0) luaL_error(L,"__index");//end
                    lua_pushstring(L, lua_tostring(L,2));
                    lua_rawget(L, -2);
                    return 1;
                                                              // 1385
            }                                                 // 1393
 
            static int __newindex(lua_State* L) {             // 1396
                        luna__hashmap::iterator i=LunaTraits<LWF ::Movie >::write_properties.find((const char*)lua_tostring(L,2));
                        if (i!=LunaTraits<LWF ::Movie >::write_properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_insert(L,2); // swap key and value
                            lua_settop(L,2); // delete key
                            return fnc(L); 
                        }
                                                              // 1403
if (lua_gettop(L) == 3 && Luna<void>::get_uniqueid(L, 1) ==
        LunaTraits<LWF::Movie>::uniqueID) {
    LWF::Movie &o =
        static_cast<LWF::Movie &>(*Luna<LWF::Movie>::check(L, 1));
    std::string name = lua_tostring(L, 2);
    if (o.lwf->SetFieldLua(&o, name))
        return 0;
}
                                                              // 1421
                    luaL_error(L,"__newindex doesn't allow defining non-property member");
                    return 0;
                                                              // 1441
            }                                                 // 1446
}; // end of class impl_LunaTraits<LWF ::Movie >              // 1450
  LWF ::Movie* LunaTraits<LWF ::Movie >::_bind_ctor(lua_State *L)
  {                                                           // 1467
   std::cerr<<"undefined contructor of LWF ::Movie called\n"; // 1468
    return NULL;                                              // 1469
  }                                                           // 1470
  void LunaTraits<LWF ::Movie >::_bind_dtor(LWF ::Movie* obj){ // 1471
   delete obj;                                                // 1472
  }                                                           // 1473
const char LunaTraits<LWF ::Movie >::className[] = "LWF_Movie"; // 1480
const int LunaTraits<LWF ::Movie >::uniqueID = 31780709;      // 1481
luna__hashmap LunaTraits<LWF ::Movie >::properties;           // 1484
luna__hashmap LunaTraits<LWF ::Movie >::write_properties;     // 1485
luna_RegType LunaTraits<LWF ::Movie >::methods[] = {          // 1487
    {"getFullName", &impl_LunaTraits<LWF ::Movie >::_bind_getFullName}, // 1492
    {"globalToLocal", &impl_LunaTraits<LWF ::Movie >::_bind_globalToLocal}, // 1492
    {"localToGlobal", &impl_LunaTraits<LWF ::Movie >::_bind_localToGlobal}, // 1492
    {"play", &impl_LunaTraits<LWF ::Movie >::_bind_play},     // 1492
    {"stop", &impl_LunaTraits<LWF ::Movie >::_bind_stop},     // 1492
    {"nextFrame", &impl_LunaTraits<LWF ::Movie >::_bind_nextFrame}, // 1492
    {"prevFrame", &impl_LunaTraits<LWF ::Movie >::_bind_prevFrame}, // 1492
    {"gotoFrame", &impl_LunaTraits<LWF ::Movie >::_bind_gotoFrame}, // 1492
    {"move", &impl_LunaTraits<LWF ::Movie >::_bind_move},     // 1492
    {"moveTo", &impl_LunaTraits<LWF ::Movie >::_bind_moveTo}, // 1492
    {"rotate", &impl_LunaTraits<LWF ::Movie >::_bind_rotate}, // 1492
    {"rotateTo", &impl_LunaTraits<LWF ::Movie >::_bind_rotateTo}, // 1492
    {"scale", &impl_LunaTraits<LWF ::Movie >::_bind_scale},   // 1492
    {"scaleTo", &impl_LunaTraits<LWF ::Movie >::_bind_scaleTo}, // 1492
    {"removeEventListener", &impl_LunaTraits<LWF ::Movie >::_bind_removeEventListener}, // 1492
    {"clearEventListener", &impl_LunaTraits<LWF ::Movie >::_bind_clearEventListener}, // 1492
    {"swapAttachedMovieDepth", &impl_LunaTraits<LWF ::Movie >::_bind_swapAttachedMovieDepth}, // 1492
    {"detachFromParent", &impl_LunaTraits<LWF ::Movie >::_bind_detachFromParent}, // 1492
    {"detachLWF", &impl_LunaTraits<LWF ::Movie >::_bind_detachLWF}, // 1492
    {"detachAllLWFs", &impl_LunaTraits<LWF ::Movie >::_bind_detachAllLWFs}, // 1492
    {"removeMovieClip", &impl_LunaTraits<LWF ::Movie >::_bind_removeMovieClip}, // 1492
    {"swapAttachedBitmapDepth", &impl_LunaTraits<LWF ::Movie >::_bind_swapAttachedBitmapDepth}, // 1492
    {"detachBitmap", &impl_LunaTraits<LWF ::Movie >::_bind_detachBitmap}, // 1492
    {"getName", &impl_LunaTraits<LWF ::Movie >::_bind_getName}, // 1492
    {"getCurrentFrame", &impl_LunaTraits<LWF ::Movie >::_bind_getCurrentFrame}, // 1492
    {"getCurrentLabel", &impl_LunaTraits<LWF ::Movie >::_bind_getCurrentLabel}, // 1492
    {"getTotalFrames", &impl_LunaTraits<LWF ::Movie >::_bind_getTotalFrames}, // 1492
    {"getVisible", &impl_LunaTraits<LWF ::Movie >::_bind_getVisible}, // 1492
    {"getX", &impl_LunaTraits<LWF ::Movie >::_bind_getX},     // 1492
    {"getY", &impl_LunaTraits<LWF ::Movie >::_bind_getY},     // 1492
    {"getScaleX", &impl_LunaTraits<LWF ::Movie >::_bind_getScaleX}, // 1492
    {"getScaleY", &impl_LunaTraits<LWF ::Movie >::_bind_getScaleY}, // 1492
    {"getRotation", &impl_LunaTraits<LWF ::Movie >::_bind_getRotation}, // 1492
    {"getAlpha", &impl_LunaTraits<LWF ::Movie >::_bind_getAlpha}, // 1492
    {"getRed", &impl_LunaTraits<LWF ::Movie >::_bind_getRed}, // 1492
    {"getGreen", &impl_LunaTraits<LWF ::Movie >::_bind_getGreen}, // 1492
    {"getBlue", &impl_LunaTraits<LWF ::Movie >::_bind_getBlue}, // 1492
    {"getBlendMode", &impl_LunaTraits<LWF ::Movie >::_bind_getBlendMode}, // 1492
    {"setVisible", &impl_LunaTraits<LWF ::Movie >::_bind_setVisible}, // 1492
    {"setX", &impl_LunaTraits<LWF ::Movie >::_bind_setX},     // 1492
    {"setY", &impl_LunaTraits<LWF ::Movie >::_bind_setY},     // 1492
    {"setScaleX", &impl_LunaTraits<LWF ::Movie >::_bind_setScaleX}, // 1492
    {"setScaleY", &impl_LunaTraits<LWF ::Movie >::_bind_setScaleY}, // 1492
    {"setRotation", &impl_LunaTraits<LWF ::Movie >::_bind_setRotation}, // 1492
    {"setAlpha", &impl_LunaTraits<LWF ::Movie >::_bind_setAlpha}, // 1492
    {"setRed", &impl_LunaTraits<LWF ::Movie >::_bind_setRed}, // 1492
    {"setGreen", &impl_LunaTraits<LWF ::Movie >::_bind_setGreen}, // 1492
    {"setBlue", &impl_LunaTraits<LWF ::Movie >::_bind_setBlue}, // 1492
    {"setBlendMode", &impl_LunaTraits<LWF ::Movie >::_bind_setBlendMode}, // 1492
    {"detachMovie", &impl_LunaTraits<LWF ::Movie >::_bind_detachMovie}, // 1492
    {"gotoAndStop", &impl_LunaTraits<LWF ::Movie >::_bind_gotoAndStop}, // 1492
    {"gotoAndPlay", &impl_LunaTraits<LWF ::Movie >::_bind_gotoAndPlay}, // 1492
    {"addEventListener", &impl_LunaTraits<LWF ::Movie >::addEventListener}, // 1492
    {"attachMovie", &impl_LunaTraits<LWF ::Movie >::attachMovie}, // 1492
    {"attachEmptyMovie", &impl_LunaTraits<LWF ::Movie >::attachEmptyMovie}, // 1492
    {"attachLWF", &impl_LunaTraits<LWF ::Movie >::attachLWF}, // 1492
    {"attachBitmap", &impl_LunaTraits<LWF ::Movie >::attachBitmap}, // 1492
    {"getAttachedBitmap", &impl_LunaTraits<LWF ::Movie >::getAttachedBitmap}, // 1492
    {"dispatchEvent", &impl_LunaTraits<LWF ::Movie >::dispatchEvent}, // 1492
    {"__index", &impl_LunaTraits<LWF ::Movie >::__index},     // 1492
    {"__newindex", &impl_LunaTraits<LWF ::Movie >::__newindex}, // 1492
    {0,0}                                                     // 1495
};                                                            // 1496
template<>                                                    // 1220
 class impl_LunaTraits<LWF ::BitmapClip > {
public:                                                       // 1223
    typedef Luna<LWF ::BitmapClip > luna_t;                   // 1227
// : number denotes the line number of luna_gen.lua that generated the sentence // 1230
  inline static bool _lg_typecheck_getName(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_depth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_depth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_visible(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_visible(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isboolean(L,2)==0) return false;                  // 546
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_width(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_width(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_height(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_height(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_regX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_regX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_regY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_regY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_x(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_x(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_y(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_y(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_scaleX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_scaleX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_scaleY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_scaleY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_rotation(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_rotation(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_alpha(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_alpha(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_offsetX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_offsetX(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_offsetY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_offsetY(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_originalWidth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_originalWidth(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_originalHeight(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_originalHeight(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=22208526) return false; // LWF ::BitmapClip // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
static std::string getName(LWF::BitmapClip &o){return o.name;}

static int _bind_getLWF(lua_State *L)
{
    if (lua_gettop(L) != 1 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::BitmapClip>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.BitmapClip.lwf");
    }
    LWF::BitmapClip const &a =
        static_cast<LWF::BitmapClip &>(*Luna<LWF::BitmapClip>::check(L, 1));
    Luna<LWF::LWF>::push(L, a.lwf, false);
    return 1;
}

static int _bind_getParent(lua_State *L)
{
    if (lua_gettop(L) != 1 || Luna<void>::get_uniqueid(L, 1) !=
            LunaTraits<LWF::BitmapClip>::uniqueID) {
        luna_printStack(L);
        luaL_error(L, "luna typecheck failed: LWF.BitmapClip.parent");
    }
    LWF::BitmapClip const &a =
        static_cast<LWF::BitmapClip &>(*Luna<LWF::BitmapClip>::check(L, 1));
    if (a.parent)
        Luna<LWF::Movie>::push(L, a.parent, false);
    else
        lua_pushnil(L);
    return 1;
}
            inline static int _property_get_depth(LWF ::BitmapClip const& a) { return a.depth; }inline static void _property_set_depth(LWF ::BitmapClip & a, int b){ a.depth=b;}
inline static bool _property_get_visible(LWF ::BitmapClip const& a) { return a.visible; }inline static void _property_set_visible(LWF ::BitmapClip & a, bool b){ a.visible=b;}
inline static float _property_get_width(LWF ::BitmapClip const& a) { return a.width; }inline static void _property_set_width(LWF ::BitmapClip & a, float b){ a.width=b;}
inline static float _property_get_height(LWF ::BitmapClip const& a) { return a.height; }inline static void _property_set_height(LWF ::BitmapClip & a, float b){ a.height=b;}
inline static float _property_get_regX(LWF ::BitmapClip const& a) { return a.regX; }inline static void _property_set_regX(LWF ::BitmapClip & a, float b){ a.regX=b;}
inline static float _property_get_regY(LWF ::BitmapClip const& a) { return a.regY; }inline static void _property_set_regY(LWF ::BitmapClip & a, float b){ a.regY=b;}
inline static float _property_get_x(LWF ::BitmapClip const& a) { return a.x; }inline static void _property_set_x(LWF ::BitmapClip & a, float b){ a.x=b;}
inline static float _property_get_y(LWF ::BitmapClip const& a) { return a.y; }inline static void _property_set_y(LWF ::BitmapClip & a, float b){ a.y=b;}
inline static float _property_get_scaleX(LWF ::BitmapClip const& a) { return a.scaleX; }inline static void _property_set_scaleX(LWF ::BitmapClip & a, float b){ a.scaleX=b;}
inline static float _property_get_scaleY(LWF ::BitmapClip const& a) { return a.scaleY; }inline static void _property_set_scaleY(LWF ::BitmapClip & a, float b){ a.scaleY=b;}
inline static float _property_get_rotation(LWF ::BitmapClip const& a) { return a.rotation; }inline static void _property_set_rotation(LWF ::BitmapClip & a, float b){ a.rotation=b;}
inline static float _property_get_alpha(LWF ::BitmapClip const& a) { return a.alpha; }inline static void _property_set_alpha(LWF ::BitmapClip & a, float b){ a.alpha=b;}
inline static float _property_get_offsetX(LWF ::BitmapClip const& a) { return a.offsetX; }inline static void _property_set_offsetX(LWF ::BitmapClip & a, float b){ a.offsetX=b;}
inline static float _property_get_offsetY(LWF ::BitmapClip const& a) { return a.offsetY; }inline static void _property_set_offsetY(LWF ::BitmapClip & a, float b){ a.offsetY=b;}
inline static float _property_get_originalWidth(LWF ::BitmapClip const& a) { return a.originalWidth; }inline static void _property_set_originalWidth(LWF ::BitmapClip & a, float b){ a.originalWidth=b;}
inline static float _property_get_originalHeight(LWF ::BitmapClip const& a) { return a.originalHeight; }inline static void _property_set_originalHeight(LWF ::BitmapClip & a, float b){ a.originalHeight=b;}
                                                              // 1275
  static int _bind_getName(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck_getName(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:getName(LWF ::BitmapClip & o,)"); }
                                                              // 487
    LWF ::BitmapClip & o=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 305
    std ::string ret=getName( o);                             // 306
    lua_pushstring(L, ret.c_str());                           // 309
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 313
    return 1;                                                 // 314
  }                                                           // 333
  static int _bind__property_get_depth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_depth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_depth(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    int ret=_property_get_depth( a);                          // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_depth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_depth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_depth(LWF ::BitmapClip & a,int b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    int b=(int)lua_tonumber(L,2);                             // 512
    try {                                                     // 254
    _property_set_depth( a, b);                               // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_visible(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_visible(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_visible(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 317
    bool ret=_property_get_visible( a);                       // 318
    lua_pushboolean(L, ret);                                  // 319
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 320
    return 1;                                                 // 321
  }                                                           // 333
  static int _bind__property_set_visible(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_visible(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_visible(LWF ::BitmapClip & a,bool b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    bool b=(bool)lua_toboolean(L,2);                          // 515
    try {                                                     // 254
    _property_set_visible( a, b);                             // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_width(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_width(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_width(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_width( a);                        // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_width(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_width(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_width(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_width( a, b);                               // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_height(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_height(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_height(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_height( a);                       // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_height(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_height(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_height(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_height( a, b);                              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_regX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_regX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_regX(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_regX( a);                         // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_regX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_regX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_regX(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_regX( a, b);                                // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_regY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_regY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_regY(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_regY( a);                         // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_regY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_regY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_regY(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_regY( a, b);                                // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_x(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_x(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_x(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_x( a);                            // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_x(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_x(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_x(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_x( a, b);                                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_y(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_y(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_y(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_y( a);                            // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_y(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_y(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_y(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_y( a, b);                                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_scaleX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_scaleX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_scaleX(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_scaleX( a);                       // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_scaleX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_scaleX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_scaleX(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_scaleX( a, b);                              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_scaleY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_scaleY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_scaleY(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_scaleY( a);                       // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_scaleY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_scaleY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_scaleY(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_scaleY( a, b);                              // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_rotation(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_rotation(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_rotation(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_rotation( a);                     // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_rotation(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_rotation(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_rotation(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_rotation( a, b);                            // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_alpha(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_alpha(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_alpha(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_alpha( a);                        // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_alpha(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_alpha(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_alpha(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_alpha( a, b);                               // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_offsetX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_offsetX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_offsetX(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_offsetX( a);                      // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_offsetX(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_offsetX(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_offsetX(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_offsetX( a, b);                             // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_offsetY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_offsetY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_offsetY(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_offsetY( a);                      // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_offsetY(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_offsetY(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_offsetY(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_offsetY( a, b);                             // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_originalWidth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_originalWidth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_originalWidth(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_originalWidth( a);                // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_originalWidth(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_originalWidth(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_originalWidth(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_originalWidth( a, b);                       // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_originalHeight(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_originalHeight(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_originalHeight(LWF ::BitmapClip const & a,)"); }
                                                              // 487
    LWF ::BitmapClip const & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_originalHeight( a);               // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_originalHeight(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_originalHeight(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_originalHeight(LWF ::BitmapClip & a,float b,)"); }
                                                              // 487
    LWF ::BitmapClip & a=static_cast<LWF ::BitmapClip &>(*Luna<LWF ::BitmapClip >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_originalHeight( a, b);                      // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static void luna_init_hashmap()
  {                                                           // 1324
    LunaTraits<LWF ::BitmapClip >::properties["name"]=&_bind_getName; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["parent"]=&_bind_getParent; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["lwf"]=&_bind_getLWF; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["depth"]=&_bind__property_get_depth; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["visible"]=&_bind__property_get_visible; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["width"]=&_bind__property_get_width; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["height"]=&_bind__property_get_height; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["regX"]=&_bind__property_get_regX; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["regY"]=&_bind__property_get_regY; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["x"]=&_bind__property_get_x; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["y"]=&_bind__property_get_y; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["scaleX"]=&_bind__property_get_scaleX; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["scaleY"]=&_bind__property_get_scaleY; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["rotation"]=&_bind__property_get_rotation; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["alpha"]=&_bind__property_get_alpha; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["offsetX"]=&_bind__property_get_offsetX; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["offsetY"]=&_bind__property_get_offsetY; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["originalWidth"]=&_bind__property_get_originalWidth; // 1326
    LunaTraits<LWF ::BitmapClip >::properties["originalHeight"]=&_bind__property_get_originalHeight; // 1326
  }                                                           // 1328
  static void luna_init_write_hashmap()
  {                                                           // 1329
    LunaTraits<LWF ::BitmapClip >::write_properties["depth"]=&_bind__property_set_depth; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["visible"]=&_bind__property_set_visible; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["width"]=&_bind__property_set_width; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["height"]=&_bind__property_set_height; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["regX"]=&_bind__property_set_regX; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["regY"]=&_bind__property_set_regY; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["x"]=&_bind__property_set_x; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["y"]=&_bind__property_set_y; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["scaleX"]=&_bind__property_set_scaleX; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["scaleY"]=&_bind__property_set_scaleY; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["rotation"]=&_bind__property_set_rotation; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["alpha"]=&_bind__property_set_alpha; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["offsetX"]=&_bind__property_set_offsetX; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["offsetY"]=&_bind__property_set_offsetY; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["originalWidth"]=&_bind__property_set_originalWidth; // 1332
    LunaTraits<LWF ::BitmapClip >::write_properties["originalHeight"]=&_bind__property_set_originalHeight; // 1332
  }                                                           // 1335
            static int __index(lua_State* L)
            {                                                 // 1339
                        {
                        luna__hashmap::iterator i=LunaTraits<LWF ::BitmapClip >::properties.find((const char*)lua_tostring(L,2));

                        if (i!=LunaTraits<LWF ::BitmapClip >::properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_pop(L,1); // remove self
                            return fnc(L); 
                        }
                            }
                                                              // 1352
                    int mt=lua_getmetatable(L, 1);
                    if(mt==0) luaL_error(L,"__index");//end
                    lua_pushstring(L, lua_tostring(L,2));
                    lua_rawget(L, -2);
                    return 1;
                                                              // 1385
            }                                                 // 1393
 
            static int __newindex(lua_State* L) {             // 1396
                        luna__hashmap::iterator i=LunaTraits<LWF ::BitmapClip >::write_properties.find((const char*)lua_tostring(L,2));
                        if (i!=LunaTraits<LWF ::BitmapClip >::write_properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_insert(L,2); // swap key and value
                            lua_settop(L,2); // delete key
                            return fnc(L); 
                        }
                                                              // 1403
                    luaL_error(L,"__newindex doesn't allow defining non-property member");
                    return 0;
                                                              // 1441
            }                                                 // 1446
}; // end of class impl_LunaTraits<LWF ::BitmapClip >         // 1450
  LWF ::BitmapClip* LunaTraits<LWF ::BitmapClip >::_bind_ctor(lua_State *L)
  {                                                           // 1467
   std::cerr<<"undefined contructor of LWF ::BitmapClip called\n"; // 1468
    return NULL;                                              // 1469
  }                                                           // 1470
  void LunaTraits<LWF ::BitmapClip >::_bind_dtor(LWF ::BitmapClip* obj){ // 1471
   delete obj;                                                // 1472
  }                                                           // 1473
const char LunaTraits<LWF ::BitmapClip >::className[] = "LWF_BitmapClip"; // 1480
const int LunaTraits<LWF ::BitmapClip >::uniqueID = 22208526; // 1481
luna__hashmap LunaTraits<LWF ::BitmapClip >::properties;      // 1484
luna__hashmap LunaTraits<LWF ::BitmapClip >::write_properties; // 1485
luna_RegType LunaTraits<LWF ::BitmapClip >::methods[] = {     // 1487
    {"getName", &impl_LunaTraits<LWF ::BitmapClip >::_bind_getName}, // 1492
    {"_property_get_depth", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_depth}, // 1492
    {"_property_set_depth", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_depth}, // 1492
    {"_property_get_visible", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_visible}, // 1492
    {"_property_set_visible", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_visible}, // 1492
    {"_property_get_width", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_width}, // 1492
    {"_property_set_width", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_width}, // 1492
    {"_property_get_height", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_height}, // 1492
    {"_property_set_height", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_height}, // 1492
    {"_property_get_regX", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_regX}, // 1492
    {"_property_set_regX", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_regX}, // 1492
    {"_property_get_regY", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_regY}, // 1492
    {"_property_set_regY", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_regY}, // 1492
    {"_property_get_x", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_x}, // 1492
    {"_property_set_x", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_x}, // 1492
    {"_property_get_y", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_y}, // 1492
    {"_property_set_y", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_y}, // 1492
    {"_property_get_scaleX", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_scaleX}, // 1492
    {"_property_set_scaleX", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_scaleX}, // 1492
    {"_property_get_scaleY", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_scaleY}, // 1492
    {"_property_set_scaleY", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_scaleY}, // 1492
    {"_property_get_rotation", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_rotation}, // 1492
    {"_property_set_rotation", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_rotation}, // 1492
    {"_property_get_alpha", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_alpha}, // 1492
    {"_property_set_alpha", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_alpha}, // 1492
    {"_property_get_offsetX", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_offsetX}, // 1492
    {"_property_set_offsetX", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_offsetX}, // 1492
    {"_property_get_offsetY", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_offsetY}, // 1492
    {"_property_set_offsetY", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_offsetY}, // 1492
    {"_property_get_originalWidth", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_originalWidth}, // 1492
    {"_property_set_originalWidth", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_originalWidth}, // 1492
    {"_property_get_originalHeight", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_get_originalHeight}, // 1492
    {"_property_set_originalHeight", &impl_LunaTraits<LWF ::BitmapClip >::_bind__property_set_originalHeight}, // 1492
    {"__index", &impl_LunaTraits<LWF ::BitmapClip >::__index}, // 1492
    {"__newindex", &impl_LunaTraits<LWF ::BitmapClip >::__newindex}, // 1492
    {0,0}                                                     // 1495
};                                                            // 1496
template<>                                                    // 1220
 class impl_LunaTraits<LWF ::Point > {
public:                                                       // 1223
    typedef Luna<LWF ::Point > luna_t;                        // 1227
// : number denotes the line number of luna_gen.lua that generated the sentence // 1230
  inline static bool _lg_typecheck_ctor_overload_1(lua_State *L)
  {                                                           // 1239
    if( lua_gettop(L)!=0) return false;                       // 527
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck_ctor_overload_2(lua_State *L)
  {                                                           // 1239
    if( lua_gettop(L)!=2) return false;                       // 527
    if( lua_isnumber(L,1)==0) return false;                   // 536
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_x(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=34538949) return false; // LWF ::Point // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_x(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=34538949) return false; // LWF ::Point // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_get_y(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=1) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=34538949) return false; // LWF ::Point // 534
    return true;
  }                                                           // 554
  inline static bool _lg_typecheck__property_set_y(lua_State *L)
  {                                                           // 1249
    if( lua_gettop(L)!=2) return false;                       // 527
    if( Luna<void>::get_uniqueid(L,1)!=34538949) return false; // LWF ::Point // 534
    if( lua_isnumber(L,2)==0) return false;                   // 536
    return true;
  }                                                           // 554
  inline static LWF ::Point* _bind_ctor_overload_1(lua_State *L)
  {                                                           // 1261
    return new LWF ::Point();                                 // 1266
  }                                                           // 1267
  inline static LWF ::Point* _bind_ctor_overload_2(lua_State *L)
  {                                                           // 1261
    float x=(float)lua_tonumber(L,1);                         // 512
    float y=(float)lua_tonumber(L,2);                         // 512
    return new LWF ::Point( x, y);                            // 1266
  }                                                           // 1267
  static LWF ::Point* _bind_ctor(lua_State *L)
  {                                                           // 195
    if (_lg_typecheck_ctor_overload_1(L)) return _bind_ctor_overload_1(L); // 198
    if (_lg_typecheck_ctor_overload_2(L)) return _bind_ctor_overload_2(L); // 198
    luaL_error(L, "ctor ( cannot find overloads:)\n()\n(float x,float y,)\n");
                                                              // 205
    return NULL;                                              // 206
  }                                                           // 207
inline static float _property_get_x(LWF ::Point const& a) { return a.x; }inline static void _property_set_x(LWF ::Point & a, float b){ a.x=b;}
inline static float _property_get_y(LWF ::Point const& a) { return a.y; }inline static void _property_set_y(LWF ::Point & a, float b){ a.y=b;}
                                                              // 1275
  static int _bind__property_get_x(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_x(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_x(LWF ::Point const & a,)"); }
                                                              // 487
    LWF ::Point const & a=static_cast<LWF ::Point &>(*Luna<LWF ::Point >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_x( a);                            // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_x(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_x(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_x(LWF ::Point & a,float b,)"); }
                                                              // 487
    LWF ::Point & a=static_cast<LWF ::Point &>(*Luna<LWF ::Point >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_x( a, b);                                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static int _bind__property_get_y(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_get_y(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_get_y(LWF ::Point const & a,)"); }
                                                              // 487
    LWF ::Point const & a=static_cast<LWF ::Point &>(*Luna<LWF ::Point >::check(L,1)); // 504
    try {                                                     // 299
    float ret=_property_get_y( a);                            // 300
    lua_pushnumber(L, ret);                                   // 301
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 302
    return 1;                                                 // 303
  }                                                           // 333
  static int _bind__property_set_y(lua_State *L)
  {                                                           // 1282
    if (!_lg_typecheck__property_set_y(L)) { luna_printStack(L); luaL_error(L, "luna typecheck failed:_property_set_y(LWF ::Point & a,float b,)"); }
                                                              // 487
    LWF ::Point & a=static_cast<LWF ::Point &>(*Luna<LWF ::Point >::check(L,1)); // 504
    float b=(float)lua_tonumber(L,2);                         // 512
    try {                                                     // 254
    _property_set_y( a, b);                                   // 255
    } catch(std::exception& e) { luaL_error( L,e.what()); }   // 256
    return 0;                                                 // 257
  }                                                           // 333
  static void luna_init_hashmap()
  {                                                           // 1324
    LunaTraits<LWF ::Point >::properties["x"]=&_bind__property_get_x; // 1326
    LunaTraits<LWF ::Point >::properties["y"]=&_bind__property_get_y; // 1326
  }                                                           // 1328
  static void luna_init_write_hashmap()
  {                                                           // 1329
    LunaTraits<LWF ::Point >::write_properties["x"]=&_bind__property_set_x; // 1332
    LunaTraits<LWF ::Point >::write_properties["y"]=&_bind__property_set_y; // 1332
  }                                                           // 1335
            static int __index(lua_State* L)
            {                                                 // 1339
                        {
                        luna__hashmap::iterator i=LunaTraits<LWF ::Point >::properties.find((const char*)lua_tostring(L,2));

                        if (i!=LunaTraits<LWF ::Point >::properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_pop(L,1); // remove self
                            return fnc(L); 
                        }
                            }
                                                              // 1352
                    int mt=lua_getmetatable(L, 1);
                    if(mt==0) luaL_error(L,"__index");//end
                    lua_pushstring(L, lua_tostring(L,2));
                    lua_rawget(L, -2);
                    return 1;
                                                              // 1385
            }                                                 // 1393
 
            static int __newindex(lua_State* L) {             // 1396
                        luna__hashmap::iterator i=LunaTraits<LWF ::Point >::write_properties.find((const char*)lua_tostring(L,2));
                        if (i!=LunaTraits<LWF ::Point >::write_properties.end())
                        {
                            luna_mfp fnc=i->second;
                            lua_insert(L,2); // swap key and value
                            lua_settop(L,2); // delete key
                            return fnc(L); 
                        }
                                                              // 1403
                    luaL_error(L,"__newindex doesn't allow defining non-property member");
                    return 0;
                                                              // 1441
            }                                                 // 1446
}; // end of class impl_LunaTraits<LWF ::Point >              // 1450
  LWF ::Point* LunaTraits<LWF ::Point >::_bind_ctor(lua_State *L)
  {                                                           // 1456
    return impl_LunaTraits<LWF ::Point >::_bind_ctor(L);      // 1457
  }                                                           // 1458
  void LunaTraits<LWF ::Point >::_bind_dtor(LWF ::Point* obj){ // 1460
    delete obj;                                               // 1461
  }                                                           // 1462
const char LunaTraits<LWF ::Point >::className[] = "LWF_Point"; // 1480
const int LunaTraits<LWF ::Point >::uniqueID = 34538949;      // 1481
luna__hashmap LunaTraits<LWF ::Point >::properties;           // 1484
luna__hashmap LunaTraits<LWF ::Point >::write_properties;     // 1485
luna_RegType LunaTraits<LWF ::Point >::methods[] = {          // 1487
    {"_property_get_x", &impl_LunaTraits<LWF ::Point >::_bind__property_get_x}, // 1492
    {"_property_set_x", &impl_LunaTraits<LWF ::Point >::_bind__property_set_x}, // 1492
    {"_property_get_y", &impl_LunaTraits<LWF ::Point >::_bind__property_get_y}, // 1492
    {"_property_set_y", &impl_LunaTraits<LWF ::Point >::_bind__property_set_y}, // 1492
    {"__index", &impl_LunaTraits<LWF ::Point >::__index},     // 1492
    {"__newindex", &impl_LunaTraits<LWF ::Point >::__newindex}, // 1492
    {0,0}                                                     // 1495
};                                                            // 1496
void luaopen_LWF(lua_State* L) {                              // 1503
    luna_dostring(L,"if __luna==nil then __luna={} end");     // 1504
    luna_dostring(L,"    if __luna.copyMethodsFrom==nil then\n        function __luna.copyMethodsFrom(methodsChild, methodsParent)\n            for k,v in pairs(methodsParent) do\n                if k~='__index' and k~='__newindex' and methodsChild[k]==nil then\n                    methodsChild[k]=v\n                end\n            end\n        end\n        function __luna.overwriteMethodsFrom(methodsChild, methodsParent)\n            for k,v in pairs(methodsParent) do\n                if k~='__index' and k~='__newindex' then\n                    if verbose then print('registering', k, methodsChild[k]) end\n                    methodsChild[k]=v\n                end\n            end\n        end\n    end\n    "); // 1505
   impl_LunaTraits<LWF ::LWF >::luna_init_hashmap();          // 1540
   impl_LunaTraits<LWF ::LWF >::luna_init_write_hashmap();    // 1541
    Luna<LWF ::LWF >::Register(L);                            // 1546
    luna_dostring(L, "if not LWF then LWF={} end LWF.LWF=__luna.LWF_LWF"); // 1553
    luna_dostring(L,"                __luna.LWF_LWF.luna_class='.LWF'"); // 1554
   impl_LunaTraits<LWF ::Button >::luna_init_hashmap();       // 1540
   impl_LunaTraits<LWF ::Button >::luna_init_write_hashmap(); // 1541
    Luna<LWF ::Button >::Register(L);                         // 1546
    luna_dostring(L, "if not LWF then LWF={} end LWF.Button=__luna.LWF_Button"); // 1553
    luna_dostring(L,"                __luna.LWF_Button.luna_class='.Button'"); // 1554
   impl_LunaTraits<LWF ::Movie >::luna_init_hashmap();        // 1540
   impl_LunaTraits<LWF ::Movie >::luna_init_write_hashmap();  // 1541
    Luna<LWF ::Movie >::Register(L);                          // 1546
    luna_dostring(L, "if not LWF then LWF={} end LWF.Movie=__luna.LWF_Movie"); // 1553
    luna_dostring(L,"                __luna.LWF_Movie.luna_class='.Movie'"); // 1554
   impl_LunaTraits<LWF ::BitmapClip >::luna_init_hashmap();   // 1540
   impl_LunaTraits<LWF ::BitmapClip >::luna_init_write_hashmap(); // 1541
    Luna<LWF ::BitmapClip >::Register(L);                     // 1546
    luna_dostring(L, "if not LWF then LWF={} end LWF.BitmapClip=__luna.LWF_BitmapClip"); // 1553
    luna_dostring(L,"                __luna.LWF_BitmapClip.luna_class='.BitmapClip'"); // 1554
   impl_LunaTraits<LWF ::Point >::luna_init_hashmap();        // 1540
   impl_LunaTraits<LWF ::Point >::luna_init_write_hashmap();  // 1541
    Luna<LWF ::Point >::Register(L);                          // 1546
    luna_dostring(L, "if not LWF then LWF={} end LWF.Point=__luna.LWF_Point"); // 1553
    luna_dostring(L,"                __luna.LWF_Point.luna_class='.Point'"); // 1554
}                                                             // 1596
#endif // LWF_USE_LUA
	