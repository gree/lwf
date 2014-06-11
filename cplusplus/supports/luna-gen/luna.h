#ifndef _LUNA_H_TAESOO_MOD
#define _LUNA_H_TAESOO_MOD
// an optimized version of luna by Taesoo Kwon.
// This luna class is faster than the original luna, lunar. (Faster than OOLUA and luabind too.)

extern "C" {
#include "lua.h"
#include "lauxlib.h"
}

#include <stdio.h>
#include <string.h>
#include <string>
#include <iostream>
#include <sstream>

#if LUA_VERSION_NUM >= 502
static inline int
luaL_typerror(lua_State *L, int narg, const char *tname)
{
	const char *msg = lua_pushfstring(L, "%s expected, got %s",
		tname, luaL_typename(L, narg));
	return luaL_argerror(L, narg, msg);
}
#endif

struct luna_eqstr{
	bool operator()(const char* s1, const char* s2) const {
		return strcmp(s1,s2)==0;
	}
};

typedef int (*luna_mfp)(lua_State *L);
#ifdef NO_HASH_MAP
#include <map>
struct luna_ltsz: std::binary_function<char* const &, char* const &, bool>
{
	bool operator()(const char* _X, const char* _Y) const
	{
		return strcmp(_X, _Y)<0;
	}
};
typedef std::map<const char*, luna_mfp, luna_ltsz> luna__hashmap;
#else // not defined NO_HASH_MAP
#ifdef _MSC_VER
#include <hash_map>

class luna_stringhasher : public stdext::hash_compare <const char*>
{
public:
  size_t operator() (const char* in) const
  {
    size_t h = 0;
	const char* p;
    for(p = in; *p != 0; ++p)
		h = 31 * h + (*p);
    return h;
  }
  
  bool operator() (const char* s1, const char* s2) const
  {
    return strcmp(s1, s2)<0;
  }
};

typedef stdext::hash_map<const char*, luna_mfp, luna_stringhasher> luna__hashmap;
#else // UNIX
#if __cplusplus < 201103L
#include <ext/hash_map> 
typedef __gnu_cxx::hash<const char*> luna_hash_t;
typedef __gnu_cxx::hash_map<const char*, luna_mfp, luna_hash_t, luna_eqstr> luna__hashmap;
#else
#include <unordered_map>
typedef std::unordered_map<std::string, luna_mfp> luna__hashmap;
#endif
#endif // UNIX
#endif // not defined NO_HASH_MAP

typedef struct { const char *name; luna_mfp mfunc; } luna_RegType;

template <typename T_interface> class LunaModule {
	public:
		static void Register(lua_State* L)
		{
			// T_interface::className is namespace rather than class here. (no constructor, destructor, userdata)
			int methods; 
			luaL_dostring(L, "if not __luna then __luna={} end");
			std::string temp;
			temp="if not __luna.";
			temp+=	T_interface::moduleName; 
			temp+=" then __luna.";
			temp+=T_interface::moduleName;
			temp+=" ={} end";

			luaL_dostring(L,temp.c_str());
#if LUA_VERSION_NUM >= 502
			lua_getglobal(L, "__luna");
#else
			lua_pushstring(L, "__luna");
			lua_gettable(L, LUA_GLOBALSINDEX);
#endif
			int __luna= lua_gettop(L);
			lua_pushstring(L, T_interface::moduleName);
			lua_gettable(L, __luna);

			methods= lua_gettop(L);

			// fill method table 
			for (const luna_RegType *l = T_interface::methods; l->name; l++) 
			{
				lua_pushstring(L, l->name);
				lua_pushcfunction(L, l->mfunc);
				lua_settable(L, methods);
			}

			lua_pop(L, 2);  // drop methods and __luna
		}
};

void luna_printStack(lua_State* L, bool compact=false);
void luna_dostring(lua_State* L, const char* luacode);
template <typename T> class Luna;
template <typename T>
class LunaTraits
{
	public:
		typedef Luna<T > luna_t;
		static const char className[];                            // 1051
		static const int uniqueID;                                // 1052
};

template <typename T>
class impl_LunaTraits
{
 public:
};


template <typename T> class Luna {
	typedef LunaTraits<T > T_interface;
	public:
	typedef struct {int uniqueID; T *pT; bool gc; bool has_env;} userdataType;

	inline static void set(lua_State *L, int table_index, const char *key) {
		lua_pushstring(L, key);
		lua_insert(L, -2);  // swap value and key
		lua_settable(L, table_index);
	}

	static void Register(lua_State *L) {
		int methods;
		lua_newtable(L);
		methods = lua_gettop(L);
#define METHOD_TABLE_IS_METATABLE  // luna_gen.lua is written assuming this is defined.
#ifdef METHOD_TABLE_IS_METATABLE 
		// use a single table 
		// sometimes more convenient 
		int metatable=methods;
#else   // use two seperate tables
		luaL_newmetatable(L, T_interface::className);
		int metatable=lua_gettop(L);
#endif

		luaL_dostring(L, "if not __luna then __luna={} end");

#if LUA_VERSION_NUM >= 502
		lua_getglobal(L, "__luna");
#else
		lua_pushstring(L, "__luna");
		lua_gettable(L, LUA_GLOBALSINDEX);
#endif
		// unlike original luna class, this class uses the same table for methods and metatable
		// store methods table in __luna global table so that
		// scripts can add functions written in Lua.
		lua_pushstring(L, T_interface::className);
		lua_pushvalue(L, methods);
		lua_settable(L, -3); // __luna[className]=methods

		lua_pushliteral(L, "__index");
		lua_pushvalue(L, methods);
		lua_settable(L, metatable); // metatable.__index=methods

		/* lua_pushliteral(L, "__tostring"); */
		/* lua_pushcfunction(L, tostring_T); */
		/* lua_settable(L, metatable);// metatable.__tostring=tostring_T */

		lua_pushliteral(L, "__gc");
		lua_pushcfunction(L, gc_T);
		lua_settable(L, metatable);

		if (0)
		{
			// ctor supports only classname:new
			lua_pushliteral(L, "new");
			lua_pushcfunction(L, new_T);
			lua_settable(L, methods);       // add new_T to metatable table
		}
		else
		{   
			// ctor supports both classname:new(...) and classname(...)
			// very slight memory and performance overhead, so 
			// no reason to support only one
			lua_newtable(L);                // mt for method table
			{
				lua_pushcfunction(L, new_T);
				lua_pushvalue(L, -1);           // dup new_T function
				set(L, methods, "new");         // add new_T to method table
			}
			set(L, -3, "__call");           // mt.__call = new_T
			lua_setmetatable(L, methods);
		}

		// fill method table with metatable from class T
		for (const luna_RegType *l = T_interface::methods; l->name; l++) {
			lua_pushstring(L, l->name);
			lua_pushcclosure(L, l->mfunc, 0);
			lua_settable(L, methods);
		}

		lua_pop(L, 2);  // drop methods and __luna
	}

	inline static int get_uniqueid(lua_State *L, int narg) {
		userdataType* ud=static_cast<userdataType*>(lua_touserdata(L,narg));
		if (!ud ) return -1;
		return ud->uniqueID;
	}

	inline static userdataType* checkRaw(lua_State *L, int narg){
		userdataType* ud=static_cast<userdataType*>(lua_touserdata(L,narg));
		if(!ud) { printf("checkRaw: ud==nil\n"); luaL_typerror(L, narg, T_interface::className); }
		if(ud->uniqueID !=T_interface::uniqueID) // type checking with almost no overhead
			{
				printf("ud->uid: %d != interface::uid : %d\n", ud->uniqueID, T_interface::uniqueID);
				luaL_typerror(L, narg, T_interface::className);
			}
		return ud;  // pointer to T object
	}
	// get userdata from Lua stack and return pointer to T object
	inline static T *check(lua_State *L, int narg) {
		return checkRaw(L, narg)->pT;
	}


	// use lunaStack::push if possible. 
	inline static void push(lua_State *L, const T* obj, bool gc, const char* metatable=T_interface::className)
	{
#if defined(METHOD_TABLE_IS_METATABLE) 
#if LUA_VERSION_NUM >= 502
		lua_getglobal(L,"__luna");
#else
		lua_pushstring(L,"__luna");
		lua_gettable(L, LUA_GLOBALSINDEX);
#endif
		int __luna= lua_gettop(L);
		userdataType *ud =
			static_cast<userdataType*>(lua_newuserdata(L, sizeof(userdataType)));
		ud->pT = (T*)obj;  // store pointer to object in userdata
		ud->gc=gc;   // collect garbage by default
		ud->has_env=false; // does this userdata has a table attached to it?
		ud->uniqueID=T_interface::uniqueID;
		lua_pushstring(L, metatable);
		lua_gettable(L, __luna);
		lua_setmetatable(L, -2);
		//luna_printStack(L);
		lua_insert(L, -2);  // swap __luna and userdata 
		lua_pop(L,1);
#else
		luaL_getmetatable(L, T_interface::className);  // lookup metatable in Lua registry
		if (lua_isnil(L, -1)) luaL_error(L, "%s missing metatable", T_interface::className);
		int mt = lua_gettop(L);
		userdataType *ud =
			static_cast<userdataType*>(lua_newuserdata(L, sizeof(userdataType)));
		ud->pT = obj;  // store pointer to object in userdata
		ud->gc=gc;
		ud->has_env=false;
		ud->uniqueID=T_interface::uniqueID;

		lua_pushvalue(L, mt);
		lua_setmetatable(L, -2);
#endif
	}
	static int new_modified_T(lua_State *L);

	private:
	Luna();  // hide default constructor

	// create a new T object and
	// push onto the Lua stack a userdata containing a pointer to T object
	static int new_T(lua_State *L) {
		lua_remove(L, 1);   // use classname:new(), instead of classname.new()
		T *obj = T_interface::_bind_ctor(L);  // call constructor for T objects
		push(L,obj,true);
		return 1;  // userdata containing pointer to T object
	}


	// garbage collection metamethod
	static int gc_T(lua_State *L) {
		userdataType *ud = static_cast<userdataType*>(lua_touserdata(L, 1));
		T *obj = ud->pT;
		if (ud->gc)
		{
			T_interface::_bind_dtor(obj);  // call constructor for T objects
		}
		return 0;
	}

	static int tostring_T (lua_State *L) {
		char buff[32];
		userdataType *ud = static_cast<userdataType*>(lua_touserdata(L, 1));
		T *obj = ud->pT;
		sprintf(buff, "%p", obj);
		lua_pushfstring(L, "%s (%s)", T_interface::className, buff);
		return 1;
	}
};

// helper class for easy use.
// wraps lua-stack and/or lua-environment
class lunaStack
{
 public:
	lua_State* L;
	int currArg;
	int delta;
	lunaStack():L(NULL){} // setCheck is necessary before use.
	lunaStack(lua_State* l, bool upward=true):L(l){
		if (upward){
			// useful for retrieving function argument. Retreieving (operator>>) doesn't remove elements from stack
			setCheckFromBottom();
		} else {
			// Retrieving (operator>>) pops elements from stack
			setCheckFromTop();
		}
	}
	~lunaStack();
	inline void setCheckFromTop() { delta=0; currArg=-1;}// gettop();}
	inline void setPop() { setCheckFromTop();}
	inline void setCheckFromBottom() { delta=1; currArg=1;}
	
	inline void printStack(bool compact=true)
	{
		luna_printStack(L,compact);
	}
	// check
	inline int gettop() { return lua_gettop(L);}
	inline double tonumber(int i) { return luaL_checknumber(L, i);}
	inline const char* tostring(int i) { return luaL_checkstring(L, i);}
	inline bool toboolean(int i) { return lua_toboolean(L, i)==1;}
	template <class T> T* topointer(int i) { return (T*)Luna<typename LunaTraits<T>::base_t>::check(L,i);}  

	inline void _incr(){
		currArg+=delta;
		if (delta==0) lua_pop(L,1);
	}

	// retrieve (or pop)
	friend lunaStack& operator>>( lunaStack& os, double& a)      
	{ a=os.tonumber(os.currArg); os._incr(); return os;}
	friend lunaStack& operator>>( lunaStack& os, std::string& a) 
	{ a=os.tostring(os.currArg); os._incr(); return os;}
	friend lunaStack& operator>>( lunaStack& os, bool& a)        
	{ a=os.toboolean(os.currArg); os._incr(); return os;}
	friend lunaStack& operator>>( lunaStack& os, int& a)        
	{ a=(int)os.tonumber(os.currArg); os._incr(); return os;}
	// check and pop 
	template <class T> T* check() { 
		T* a=topointer<T>(currArg);_incr(); return a;}

	inline void pop() { lua_pop(L,1);}

	friend lunaStack& operator<<( lunaStack& os, double a)		    	{ lua_pushnumber(os.L, a); return os;}
	friend lunaStack& operator<<( lunaStack& os, bool a)		    	{ lua_pushboolean(os.L, a); return os;}
	friend lunaStack& operator<<( lunaStack& os, std::string const &a)	{ lua_pushstring(os.L,a.c_str()); return os;}

	// set garbageCollection=true when lua environment needs to adopt the object. 
	// e.g. push<OBJ>(new OBJ(), true);
	//      push<OBJ>(pointerToExistingOBJmanagedInsideCpp, false);
	template <class T> void push(T const* c,bool garbageCollection=false) { Luna<typename LunaTraits<T>::base_t>::push(L,(typename LunaTraits<T>::base_t*)c,garbageCollection, LunaTraits<T>::className);}
	template <class T> void push(T const& c) { Luna<typename LunaTraits<T>::base_t>::push(L,(typename LunaTraits<T>::base_t*)&c,false, LunaTraits<T>::className);}

	// stack[top]=stack[tblindex][index]
	inline void gettable(int tblindex, int index)
	{
		lua_pushnumber(L, index);
		lua_gettable(L, tblindex);
	}
	// stack[top]=stack[tblindex][key]
	inline void gettable(int tblindex, const char* key)
	{
		lua_pushstring(L, key);
		lua_gettable(L, tblindex);
	}
	// stack[top]=_G[key]
	void getglobal(const char* key){
#if LUA_VERSION_NUM >= 502
		lua_getglobal(L, key);
#else
		lua_pushstring(L, key);
		lua_gettable(L,LUA_GLOBALSINDEX); // stack top becomes _G[key] 
#endif
	}

	// stack[top]=_G[key1][key2]
	inline void getglobal(const char* key1, const char* key2){
		getglobal(key1);
		replaceTop(key2);
	}
	// stack[top]=_G[key1][key2][key3]
	inline void getglobal(const char* key1, const char* key2, const char* key3){
		getglobal(key1);
		replaceTop(key2);
		replaceTop(key3);
	}
	inline void getglobal(const char* key1, const char* key2, const char* key3, const char* key4){
		getglobal(key1);
		replaceTop(key2);
		replaceTop(key3);
		replaceTop(key4);
	}

	// stack[top]=stack[top][key]
	inline void replaceTop(const char* key){
		if (!lua_istable(L,-1)) luaL_error(L, "Luna<>::replaceTop: non-table object cannot be accessed");
		lua_pushstring(L, key);
		lua_gettable(L, -2);
		lua_insert(L, -2);  // swap table and value 
		lua_pop(L,1); // pop-out prev table
	}
	inline void replaceTopLUD(void* key){
		if (!lua_istable(L,-1)) luaL_error(L, "Luna<>::replaceTop: non-table object cannot be accessed");
		lua_pushlightuserdata(L, key);
		lua_gettable(L, -2);
		lua_insert(L, -2);  // swap table and value 
		lua_pop(L,1); // pop-out prev table
	}
	
	// assuming stack[-1-numIn]==function. (stack: function -> arg1 -> arg2 -> arg_numIn )
	inline void call(int numIn, int numOut){
		lua_call(L, numIn, numOut);
		// prepare to read-out results in reverse order
		setCheckFromTop(); 
	}
	// usage:
	// l.getglobal("functionName")
	// l << param1 << param2 << param3;
	// int numOut=l.beginCall(3);
	// l >> ret1 >> ret2;
	// l.endCall(numOut); // cleans the stack
	
	int beginCall(int numIn);
	void endCall(int numOut);

#if LUA_VERSION_NUM >= 502
	template <class T> T* get(const char* key, int table_index){
		//luna_printStack(L);
		lua_pushstring(L, key);
		lua_gettable(L, table_index);
		T* ptr= topointer<T>(gettop());
		lua_pop(L,1);
		//luna_printStack(L);
		return ptr;
	}

	template <class T> void set(const char* key, T* ptr, int table_index, bool garbageCollection=false) {
		push<T>(ptr,garbageCollection);
		lua_pushstring(L, key);
		lua_insert(L, -2);  // swap value and key
		lua_settable(L, table_index);
		//printf("%x %x\n", (unsigned int)ptr,(unsigned int) get<T>(key));
	}
#else
	template <class T> T* get(const char* key, int table_index=LUA_GLOBALSINDEX){
		//luna_printStack(L);
		lua_pushstring(L, key);
		lua_gettable(L, table_index);
		T* ptr= topointer<T>(gettop());
		lua_pop(L,1);
		//luna_printStack(L);
		return ptr;
	}

	template <class T> void set(const char* key, T* ptr,  int table_index=LUA_GLOBALSINDEX, bool garbageCollection=false) {
		push<T>(ptr,garbageCollection);
		lua_pushstring(L, key);
		lua_insert(L, -2);  // swap value and key
		lua_settable(L, table_index);
		//printf("%x %x\n", (unsigned int)ptr,(unsigned int) get<T>(key));
	}
	inline void settable(int table_index=LUA_GLOBALSINDEX){
		lua_settable(L, table_index);
	}
#endif

	// linear search. returns #tbl
	int arraySize(int tblindex);
	// a={{"a","b"}, "c"} -> treeSize of (a)=5 : ( Root, LeftInternalNode, "a", "b", "c")
	//									   where Root=a, LeftInternalNode={"a","b"}
	int treeSize(int tblindex);
};

class luna_wrap_object // inherit this object to enable inheritance from lua
{
 public:
	luna_wrap_object(){}
	std::string _custumMT; // custum metatable
	void setCustumMT(lua_State* L, const char* mt) { _custumMT=mt;}
    // push a member function (__luna[_custumMT][funcName]) and self object to stack
	template <class T>
	bool pushMemberFunc(lunaStack & l, const char* funcName )
	{ 
		/* easy to read but slower version
		l.getglobal("__luna", _custumMT.c_str(), funcName);
		if(!lua_isnil(l.L,-1)){
			l.getglobal("__luna",_custumMT.c_str(), "aUserdata");
			l.replaceTopLUD((void*)(static_cast<typename LunaTraits<T>::base_t*>(this)));
			return true;
		}
		lua_pop(l.L,1);
		return false;
		*/
		lua_State* L=l.L;
		l.getglobal("__luna", _custumMT.c_str()); // get metatable
		lua_pushstring(L, funcName);
		lua_gettable(L,-2);
		if(lua_isnil(L,-1)){
			lua_pop(L,2);
			return false;
		}
		lua_pushstring(L, "aUserdata");
		lua_gettable(L,-3);
		l.replaceTopLUD((void*)(static_cast<typename LunaTraits<T>::base_t*>((T*)this)));
		lua_remove(L,-3); // pop-out metatable
		return true;	
	}
};
template <class T>
int Luna<T>::new_modified_T(lua_State *L) {
		//luna_printStack(L);
		std::string metatable=lua_tostring(L,2);
		lua_remove(L, 1);   // use classname:new(), instead of classname.new()
		lua_remove(L, 1);  
		T *obj = T_interface::_bind_ctor(L);  // call constructor for T objects
		obj->setCustumMT(L, metatable.c_str());
		push(L,obj,true, metatable.c_str());
		//luna_printStack(L);
		lunaStack l(L);
		l.getglobal("__luna", metatable.c_str(), "aUserdata");
		if(lua_isnil(L,-1))
		{
			lua_pop(L,1);
			l.getglobal("__luna", metatable.c_str());
			//printf("getglobal"); luna_printStack(L);
			lua_pushstring(L,"aUserdata");
			lua_newtable(L);
			//luna_printStack(L);
			lua_settable(L,-3);
			//printf("asdf "); luna_printStack(L);
			lua_pushstring(L,"aUserdata");
			lua_gettable(L,-2);
			lua_insert(L,-2); // swap __luna and aUserdata
			lua_pop(L,1);
			//luna_printStack(L);
		}

		//printf("kk ");luna_printStack(L);
		// aUserdata[obj]=userdata
		lua_pushlightuserdata(L, (void*)obj);
		lua_pushvalue(L, -3); // dup userdata
		//luna_printStack(L);
		lua_settable(L, -3);
		lua_pop(L,1);
		//luna_printStack(L);
		return 1;  // userdata containing pointer to T object
	}
#endif
