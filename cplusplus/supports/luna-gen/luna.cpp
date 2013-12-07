

#include "luna.h"
void luna_printStack(lua_State* L, bool compact)
{
	if(compact)
		printf("stack top:%d - ", lua_gettop(L)); 
	else
		printf("stack trace: top %d\n", lua_gettop(L)); 

	for(int ist=1; ist<=lua_gettop(L); ist++) {
		if(compact)
			printf("%d:%c",ist,luaL_typename(L,ist)[0]);
		else
			printf("%d:%s",ist,luaL_typename(L,ist));
		if(lua_isnumber(L,ist) ==1) {
			printf("=%f ",(float)lua_tonumber(L,ist));
		} else if(lua_isstring(L,ist) ==1){
			printf("=%s ",lua_tostring(L,ist));
		} else {
			printf(" ");
		}
		if( !compact)printf("\n");
	}
	printf("\n");
}
void luna_dostring(lua_State* L, const char* luacode)
{
	// luaL_dostring followed by pcall error checking 
	if (luaL_dostring(L, luacode)==1)
	{
		printf("Lua error: stack :\n");
		luna_printStack(L,false);
	}
}
lunaStack::~lunaStack()
{
}
int lunaStack::arraySize(int tblindex)
{
	if (tblindex==-1) tblindex=gettop();
	for (int i=1; 1; i++){
		gettable(tblindex,i);
		if(lua_isnil(L,-1))
		{
			lua_pop(L,1);
			return i-1;
		}
		lua_pop(L,1);
		//luna_printStack(L, true);
	}
}
int lunaStack::treeSize(int tblindex)
{
	if(tblindex==-1) tblindex=gettop();
	if (lua_type(L,tblindex)!=LUA_TTABLE)
		return 1;
	int count=0;
	int arrSize=arraySize(tblindex);
	for (int i=1; i<=arrSize; i++)
	{
		gettable(tblindex,i);
		count+=treeSize(-1);
		//printf("count%d\n", count);
		pop();
	}
	return count+1;
}

int lunaStack::beginCall(int numIn){
	//printStack();
	int func=gettop()-numIn;
	lua_call(L, numIn, LUA_MULTRET);
	// prepare to read-out results in proper order
	setCheckFromBottom();
	//printf("%d %d\n", gettop(), func);
	currArg=func;
	return gettop()-func+1; // returns numOut
}
void lunaStack::endCall(int numOut){
	lua_pop(L,numOut);
	setCheckFromTop(); // return to the normal stack mode.
}

