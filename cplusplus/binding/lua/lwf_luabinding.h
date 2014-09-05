#if defined(LWF_USE_LUA)
#include "lwf.h"
	
#ifndef genlua_input_lua471589_def1                           // 1141
#define genlua_input_lua471589_def1                           // 1142
// declare all classes before including this file             // 1143
// e.g. class LMat; class LMatView; ....                      // 1144
// The forward declaration is not included here because luna_gen cannot distinguish struct, class, or namespace. // 1145
// : number denotes the line number of luna_gen.lua which generated that line // 1148
template<>                                                    // 1163
 class LunaTraits<LWF ::LWF > {
public:                                                       // 1165
    static const char className[];                            // 1174
    static const int uniqueID;                                // 1175
    static luna_RegType methods[];                            // 1176
    static LWF ::LWF* _bind_ctor(lua_State *L);               // 1178
    static void _bind_dtor(LWF ::LWF* obj);                   // 1179
    typedef LWF ::LWF base_t;                                 // 1181
static luna__hashmap properties;                              // 1183
static luna__hashmap write_properties;                        // 1184
};                                                            // 1187
template<>                                                    // 1163
 class LunaTraits<LWF ::Button > {
public:                                                       // 1165
    static const char className[];                            // 1174
    static const int uniqueID;                                // 1175
    static luna_RegType methods[];                            // 1176
    static LWF ::Button* _bind_ctor(lua_State *L);            // 1178
    static void _bind_dtor(LWF ::Button* obj);                // 1179
    typedef LWF ::Button base_t;                              // 1181
static luna__hashmap properties;                              // 1183
static luna__hashmap write_properties;                        // 1184
};                                                            // 1187
template<>                                                    // 1163
 class LunaTraits<LWF ::Movie > {
public:                                                       // 1165
    static const char className[];                            // 1174
    static const int uniqueID;                                // 1175
    static luna_RegType methods[];                            // 1176
    static LWF ::Movie* _bind_ctor(lua_State *L);             // 1178
    static void _bind_dtor(LWF ::Movie* obj);                 // 1179
    typedef LWF ::Movie base_t;                               // 1181
static luna__hashmap properties;                              // 1183
static luna__hashmap write_properties;                        // 1184
};                                                            // 1187
template<>                                                    // 1163
 class LunaTraits<LWF ::BitmapClip > {
public:                                                       // 1165
    static const char className[];                            // 1174
    static const int uniqueID;                                // 1175
    static luna_RegType methods[];                            // 1176
    static LWF ::BitmapClip* _bind_ctor(lua_State *L);        // 1178
    static void _bind_dtor(LWF ::BitmapClip* obj);            // 1179
    typedef LWF ::BitmapClip base_t;                          // 1181
static luna__hashmap properties;                              // 1183
static luna__hashmap write_properties;                        // 1184
};                                                            // 1187
template<>                                                    // 1163
 class LunaTraits<LWF ::Point > {
public:                                                       // 1165
    static const char className[];                            // 1174
    static const int uniqueID;                                // 1175
    static luna_RegType methods[];                            // 1176
    static LWF ::Point* _bind_ctor(lua_State *L);             // 1178
    static void _bind_dtor(LWF ::Point* obj);                 // 1179
    typedef LWF ::Point base_t;                               // 1181
static luna__hashmap properties;                              // 1183
static luna__hashmap write_properties;                        // 1184
};                                                            // 1187
#endif                                                        // 1192
#endif // LWF_USE_LUA
	