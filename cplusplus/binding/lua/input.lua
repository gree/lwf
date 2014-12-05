--
-- Copyright (C) 2013 GREE, Inc.
-- 
-- This software is provided 'as-is', without any express or implied
-- warranty.  In no event will the authors be held liable for any damages
-- arising from the use of this software.
-- 
-- Permission is granted to anyone to use this software for any purpose,
-- including commercial applications, and to alter it and redistribute it
-- freely, subject to the following restrictions:
-- 
-- 1. The origin of this software must not be misrepresented; you must not
--    claim that you wrote the original software. If you use this software
--    in a product, an acknowledgment in the product documentation would be
--    appreciated but is not required.
-- 2. Altered source versions must be plainly marked as such, and must not be
--    misrepresented as being the original software.
-- 3. This notice may not be removed or altered from any source distribution.
--

bindTarget={
	classes={
		{
			name='LWF.LWF',
			className='LWF::LWF',
			read_properties={
				{'name', 'getName'},
				{'rootMovie', 'getRootMovie'},
				{'_root', 'get_root'},
				{'width', 'getWidth'},
				{'height', 'getHeight'},
				{'pointX', 'getPointX'},
				{'pointY', 'getPointY'},
			},
			memberFunctions={[[
void SetText(std::string textName, std::string text) @ setText
std::string GetText(std::string textName) @ getText
void PlayMovie(std::string instanceName) @ playMovie
void StopMovie(std::string instanceName) @ stopMovie
void NextFrameMovie(std::string instanceName) @ nextFrameMovie
void PrevFrameMovie(std::string instanceName) @ prevFrameMovie
void SetVisibleMovie(std::string instanceName, bool visible) @ setVisibleMovie
void GotoAndStopMovie(std::string instanceName, int frameNo) @ gotoAndStopMovie
void GotoAndStopMovie(std::string instanceName, std::string label) @ gotoAndStopMovie
void GotoAndPlayMovie(std::string instanceName, int frameNo) @ gotoAndPlayMovie
void GotoAndPlayMovie(std::string instanceName, std::string label) @ gotoAndPlayMovie
void MoveMovie(std::string instanceName, float vx, float vy) @ moveMovie
void MoveToMovie(std::string instanceName, float vx, float vy) @ moveToMovie
void RotateMovie(std::string instanceName, float degree) @ rotateMovie
void RotateToMovie(std::string instanceName, float degree) @ rotateToMovie
void ScaleMovie(std::string instanceName, float vx, float vy) @ scaleMovie
void ScaleToMovie(std::string instanceName, float vx, float vy) @ scaleToMovie
void SetAlphaMovie(std::string instanceName, float v) @ setAlphaMovie
void SetColorTransformMovieLua(std::string instanceName, float vr, float vg, float vb, float va) @ setColorTransformMovie
void RemoveEventHandler(std::string eventName, int id) @ removeEventListener
void ClearEventHandler(std::string eventName) @ clearEventListener
void RemoveMovieEventHandler(std::string instanceName, int id) @ removeMovieEventListener
void ClearMovieEventHandler(std::string instanceName) @ clearMovieEventListener
void RemoveButtonEventHandler(std::string instanceName, int id) @ removeButtonEventListener
void ClearButtonEventHandler(std::string instanceName) @ clearButtonEventListener
			]]},
			staticMemberFunctions={[[
static std::string getName(LWF::LWF &o);
static float getWidth(LWF::LWF &o);
static float getHeight(LWF::LWF &o);
static float getPointX(LWF::LWF &o);
static float getPointY(LWF::LWF &o);
			]]},
			customFunctionsToRegister={
				'addEventListener',
				'addMovieEventListener',
				'addButtonEventListener',
			},
			wrapperCode=[[
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

			]],
		},
		{
			name='LWF.Button',
			className='LWF::Button',
			read_properties={
				{'name', 'getName'},
				{'parent', 'getParent'},
				{'lwf', 'getLWF'},
				{'hitX', 'getHitX'},
				{'hitY', 'getHitY'},
				{'width', 'getWidth'},
				{'height', 'getHeight'},
			},
			memberFunctions={[[
std::string GetFullName() const @ getFullName
void RemoveEventHandler(std::string eventName, int id) @ removeEventListener
void ClearEventHandler(std::string eventName) @ clearEventListener
			]]},
			staticMemberFunctions={[[
static std::string getName(LWF::Button &o);
static float getHitX(LWF::Button &o);
static float getHitY(LWF::Button &o);
static float getWidth(LWF::Button &o);
static float getHeight(LWF::Button &o);
			]]},
			customFunctionsToRegister={
				'addEventListener',
			},
			wrapperCode=[[
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

			]],
		},
		{
			name='LWF.Movie',
			className='LWF::Movie',
			read_properties={
				{'name', 'getName'},
				{'parent', 'getParent'},
				{'currentFrame', 'getCurrentFrame'},
				{'currentLabel', 'getCurrentLabel'},
				{'currentLabels', 'getCurrentLabels'},
				{'totalFrames', 'getTotalFrames'},
				{'visible', 'getVisible'},
				{'x', 'getX'},
				{'y', 'getY'},
				{'scaleX', 'getScaleX'},
				{'scaleY', 'getScaleY'},
				{'rotation', 'getRotation'},
				{'alpha', 'getAlpha'},
				{'red', 'getRed'},
				{'green', 'getGreen'},
				{'blue', 'getBlue'},
				{'lwf', 'getLWF'},
			},
			write_properties={
				{'visible', 'setVisible'},
				{'x', 'setX'},
				{'y', 'setY'},
				{'scaleX', 'setScaleX'},
				{'scaleY', 'setScaleY'},
				{'rotation', 'setRotation'},
				{'alpha', 'setAlpha'},
				{'red', 'setRed'},
				{'green', 'setGreen'},
				{'blue', 'setBlue'},
			},
			memberFunctions={[[
std::string GetFullName() const @ getFullName
LWF::Point GlobalToLocal(const LWF::Point &point) const @ globalToLocal
LWF::Point LocalToGlobal(const LWF::Point &point) const @ localToGlobal
void Play() @ play
void Stop() @ stop
void NextFrame() @ nextFrame
void PrevFrame() @ prevFrame
void GotoFrame(int frameNo) @ gotoFrame
void GotoAndStop(int frameNo) @ gotoAndStop
void GotoAndStop(std::string label) @ gotoAndStop
void GotoAndPlay(int frameNo) @ gotoAndPlay
void GotoAndPlay(std::string label) @ gotoAndPlay
void Move(float vx, float vy) @ move
void MoveTo(float vx, float vy) @ moveTo
void Rotate(float degree) @ rotate
void RotateTo(float degree) @ rotateTo
void Scale(float vx, float vy) @ scale
void ScaleTo(float vx, float vy) @ scaleTo
void RemoveEventHandler(std::string eventName, int id) @ removeEventListener
void ClearEventHandler(std::string eventName) @ clearEventListener
void SwapAttachedMovieDepth(int depth0, int depth1) @ swapAttachedMovieDepth
void DetachMovie(std::string aName) @ detachMovie
void DetachMovie(LWF::Movie *movie) @ detachMovie
void DetachFromParent() @ detachFromParent
void DetachLWF(std::string aName) @ detachLWF
void DetachAllLWFs() @ detachAllLWFs
void RemoveMovieClip() @ removeMovieClip
void SwapAttachedBitmapDepth(int depth0, int depth1) @ swapAttachedBitmapDepth
void DetachBitmap(int depth) @ detachBitmap
			]]},
			staticMemberFunctions={[[
static std::string getName(LWF::Movie &o);
static int getCurrentFrame(LWF::Movie &o);
static std::string getCurrentLabel(LWF::Movie &o);
static int getTotalFrames(LWF::Movie &o);
static bool getVisible(LWF::Movie &o);
static float getX(LWF::Movie &o);
static float getY(LWF::Movie &o);
static float getScaleX(LWF::Movie &o);
static float getScaleY(LWF::Movie &o);
static float getRotation(LWF::Movie &o);
static float getAlpha(LWF::Movie &o);
static float getRed(LWF::Movie &o);
static float getGreen(LWF::Movie &o);
static float getBlue(LWF::Movie &o);

static void setVisible(LWF::Movie &o, bool v);
static void setX(LWF::Movie &o, float v);
static void setY(LWF::Movie &o, float v);
static void setScaleX(LWF::Movie &o, float v);
static void setScaleY(LWF::Movie &o, float v);
static void setRotation(LWF::Movie &o, float v);
static void setAlpha(LWF::Movie &o, float v);
static void setRed(LWF::Movie &o, float v);
static void setGreen(LWF::Movie &o, float v);
static void setBlue(LWF::Movie &o, float v);
			]]},
			customIndex=[[
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
			]],
			customNewIndex=[[
if (lua_gettop(L) == 3 && Luna<void>::get_uniqueid(L, 1) ==
		LunaTraits<LWF::Movie>::uniqueID) {
	LWF::Movie &o =
		static_cast<LWF::Movie &>(*Luna<LWF::Movie>::check(L, 1));
	std::string name = lua_tostring(L, 2);
	if (o.lwf->SetFieldLua(&o, name))
		return 0;
}
			]],
			customFunctionsToRegister={
				'addEventListener',
				'attachMovie',
				'attachEmptyMovie',
				'attachLWF',
				'attachBitmap',
				'getAttachedBitmap',
				'dispatchEvent',
			},
			wrapperCode=[[
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

			]],
		},
		{
			name='LWF.BitmapClip',
			className='LWF::BitmapClip',
			properties={
				'int depth',
				'bool visible',
				'float width',
				'float height',
				'float regX',
				'float regY',
				'float x',
				'float y',
				'float scaleX',
				'float scaleY',
				'float rotation',
				'float alpha',
			},
			read_properties={
				{'name', 'getName'},
				{'parent', 'getParent'},
				{'lwf', 'getLWF'},
			},
			staticMemberFunctions={[[
static std::string getName(LWF::BitmapClip &o);
			]]},
			wrapperCode=[[
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
			]],
		},
		{
			name='LWF.Point',
			className='LWF::Point',
			ctors={'()', '(float x, float y)'},
			properties={
				'float x',
				'float y',
			},
		},
	},
}

function generate()
	buildDefinitionDB(bindTarget)
	write([[
#if defined(LWF_USE_LUA)
#include "lwf.h"
	]])
	writeHeader(bindTarget)
	write([[
#endif // LWF_USE_LUA
	]])
	flushWritten('lwf_luabinding.h')
	write([[
#if defined(LWF_USE_LUA)
	]])
	writeIncludeBlock()
	write([[
#include "lwf_luabinding.h"
	]])
	writeDefinitions(bindTarget, 'luaopen_LWF')
	write([[
#endif // LWF_USE_LUA
	]])
	flushWritten('lwf_luabinding.cpp')
end
