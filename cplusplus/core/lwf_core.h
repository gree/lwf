/*
 * Copyright (C) 2013 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#ifndef LWF_CORE_H
#define	LWF_CORE_H

#include "lwf_eventmovie.h"
#include "lwf_eventbutton.h"

namespace LWF {

class Button;
class ButtonEventHandlers;
struct Data;
class IObject;
class IRendererFactory;
class Movie;
class ProgramObject;
class Property;
class Renderer;
class TextRenderer;

typedef function<shared_ptr<Renderer> (ProgramObject *, int, int, int)>
	ProgramObjectConstructor;
typedef map<string, EventHandlerList> GenericEventHandlerDictionary;
typedef map<string, MovieEventHandlers> MovieEventHandlersDictionary;
typedef map<string, ButtonEventHandlers> ButtonEventHandlersDictionary;
typedef map<string, MovieEventHandler> MovieEventHandlerDictionary;
typedef map<string, ButtonEventHandler> ButtonEventHandlerDictionary;
typedef function<void (Movie *)> MovieCommand;
typedef vector<pair<vector<string>, MovieCommand> > MovieCommands;
typedef map<int, bool> AllowButtonList;
typedef map<int, bool> DenyButtonList;
typedef function<void (LWF *)> ExecHandler;
typedef vector<pair<int, ExecHandler> > ExecHandlerList;
typedef map<string, pair<string, TextRenderer *> > TextDictionary;
typedef map<int, bool> EventFunctions;

class LWF
{
public:
	enum TweenMode {
		TweenModeMovie,
		TweenModeLWF,
	};

	static float ROUND_OFF_TICK_RATE;

private:
	static int m_instanceOffset;
	static int m_iObjectOffset;

public:
	shared_ptr<Data> data;
	shared_ptr<IRendererFactory> rendererFactory;
	shared_ptr<Property> property;
	shared_ptr<Movie> rootMovie;
	Button *focus;
	Button *pressed;
	Button *buttonHead;
	DetachHandler detachHandler;
	Movie *parent;
	EventFunctions eventFunctions;
	string name;
	string attachName;
	int frameRate;
	int execLimit;
	int renderingIndex;
	int renderingIndexOffsetted;
	int renderingCount;
	int depth;
	int execCount;
	int updateCount;
	int instanceId;
	string instanceIdString;
	double time;
	float scaleByStage;
	float tick;
	float thisTick;
	float height;
	float width;
	float pointX;
	float pointY;
	bool interactive;
	bool isExecDisabled;
	bool pressing;
	bool attachVisible;
	bool isPropertyDirty;
	bool isLWFAttached;
	bool interceptByNotAllowOrDenyButtons;
	bool intercepted;
	bool playing;
	bool alive;
	void *privateData;
	void *luaState;
	string luaError;

	//TODO
	//TweenMode tweenMode;
	//tweens

private:
	vector<IObject *> m_instances;
	vector<EventHandlerList> m_eventHandlers;
	GenericEventHandlerDictionary m_genericEventHandlerDictionary;
	vector<MovieEventHandlers> m_movieEventHandlers;
	vector<ButtonEventHandlers> m_buttonEventHandlers;
	MovieEventHandlersDictionary m_movieEventHandlersByFullName;
	ButtonEventHandlersDictionary m_buttonEventHandlersByFullName;
	MovieCommands m_movieCommands;
	vector<ProgramObjectConstructor> m_programObjectConstructors;
	AllowButtonList m_allowButtonList;
	DenyButtonList m_denyButtonList;
	ExecHandlerList m_execHandlers;
	TextDictionary m_textDictionary;
	float m_progress;
	float m_roundOffTick;
	bool m_executedForExecDisabled;
	Matrix m_matrix;
	Matrix m_matrixIdentity;
	ColorTransform m_colorTransform;
	ColorTransform m_colorTransformIdentity;
	int m_rootMovieStringId;
	int m_eventOffset;

public:
	LWF(shared_ptr<Data> d, shared_ptr<IRendererFactory> r, void *l = 0);

	void SetRendererFactory(shared_ptr<IRendererFactory> r);
	void SetFrameRate(int f);
	void SetPreferredFrameRate(int f, int eLimit = 2);

	void FitForHeight(float stageWidth, float stageHeight);
	void FitForWidth(float stageWidth, float stageHeight);
	void ScaleForHeight(float stageWidth, float stageHeight);
	void ScaleForWidth(float stageWidth, float stageHeight);

	void RenderOffset();
	void ClearRenderOffset();
	int RenderObject(int count = 1);

	void SetAttachVisible(bool visible);
	void ClearFocus(Button *button);
	void ClearPressed(Button *button);
	void ClearIntercepted();

	void Init();

	int Exec(float tick = 0,
		const Matrix *matrix = 0, const ColorTransform *colorTransform = 0);
	int ForceExec(
		const Matrix *matrix = 0, const ColorTransform *colorTransform = 0);
	int ForceExecWithoutProgress(
		const Matrix *matrix = 0, const ColorTransform *colorTransform = 0);
	void Update(
		const Matrix *matrix = 0, const ColorTransform *colorTransform = 0);
	int Render(int rIndex = 0, int rCount = 0, int rOffset = INT_MIN);
	int Inspect(Inspector inspector, int hierarchy = 0, int inspectDepth = 0,
		int rIndex = 0, int rCount = 0, int rOffset = INT_MIN);

	void Destroy();

	int GetIObjectOffset() {return ++m_iObjectOffset;}

	Movie *SearchMovieInstance(int stringId) const;
	Movie *SearchMovieInstance(string instanceName) const;
	Movie *operator[](string instanceName) const;
	Movie *SearchMovieInstanceByInstanceId(int instId) const;

	Button *SearchButtonInstance(int stringId) const;
	Button *SearchButtonInstance(string instanceName) const;
	Button *SearchButtonInstanceByInstanceId(int instId) const;

	IObject *GetInstance(int instId) const;
	void SetInstance(int instId, IObject *instance);

	ProgramObjectConstructor GetProgramObjectConstructor(
		string programObjectName) const;
	ProgramObjectConstructor GetProgramObjectConstructor(
		int programObjectId) const;
	void SetProgramObjectConstructor(string programObjectName,
		ProgramObjectConstructor programObjectConstructor);
	void SetProgramObjectConstructor(int programObjectId,
		ProgramObjectConstructor programObjectConstructor);

	void ExecMovieCommand();
	void SetMovieCommand(vector<string> instanceNames, MovieCommand cmd);

	bool AddAllowButton(string buttonName);
	bool RemoveAllowButton(string buttonName);
	void ClearAllowButton();
	bool AddDenyButton(string buttonName);
	void DenyAllButtons();
	bool RemoveDenyButton(string buttonName);
	void ClearDenyButton();

	void DisableExec();
	void EnableExec();
	void SetPropertyDirty();
	int AddExecHandler(ExecHandler execHandler);
	void RemoveExecHandler(int id);
	void ClearExecHandler();
	int SetExecHandler(ExecHandler execHandler);

	void PlayAnimation(int animationId, Movie *movie, Button *button = 0);

	Button *InputPoint(int px, int py);
	void InputPress();
	void InputRelease();
	void InputKeyPress(int code);

	int GetInstanceNameStringId(int instId) const;
	int GetStringId(string str) const;
	int SearchInstanceId(int stringId) const;
	int SearchFrame(const Movie *movie, string label) const;
	int SearchFrame(const Movie *movie, int stringId) const;
	const map<int, int> *GetMovieLabels(string linkageName) const;
	const map<int, int> *GetMovieLabels(const Movie *movie) const;
	int SearchMovieLinkage(int stringId) const;
	string GetMovieLinkageName(int movieId) const;
	int SearchEventId(string eventName) const;
	int SearchEventId(int stringId) const;
	int SearchProgramObjectId(string programObjectName) const;
	int SearchProgramObjectId(int stringId) const;

	void InitEvent();
	int GetEventOffset() {return ++m_eventOffset;}
	int AddEventHandler(string eventName, EventHandler eventHandler);
	int AddEventHandler(int eventId, EventHandler eventHandler);
	void RemoveEventHandler(string eventName, int id);
	void RemoveEventHandler(int eventId, int id);
	void ClearEventHandler(string eventName);
	void ClearEventHandler(int eventId);
	int SetEventHandler(string eventName, EventHandler eventHandler);
	int SetEventHandler(int eventId, EventHandler eventHandler);
	void DispatchEvent(string eventName, Movie *m, Button *b);
	MovieEventHandlers *GetMovieEventHandlers(const Movie *m);
	int AddMovieEventHandler(
		string instanceName, const MovieEventHandlerDictionary &h);
	int AddMovieEventHandler(
		int instId, const MovieEventHandlerDictionary &h);
	void RemoveMovieEventHandler(string instanceName, int id);
	void RemoveMovieEventHandler(int instId, int id);
	void ClearMovieEventHandler(string instanceName);
	void ClearMovieEventHandler(int instId);
	void ClearMovieEventHandler(string instanceName, string type);
	void ClearMovieEventHandler(int instId, string type);
	void SetMovieEventHandler(
		string instanceName, const MovieEventHandlerDictionary &h);
	void SetMovieEventHandler(int instId, const MovieEventHandlerDictionary &h);
	ButtonEventHandlers *GetButtonEventHandlers(const Button *b);
	int AddButtonEventHandler(string instanceName,
		const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh = 0);
	int AddButtonEventHandler(int instId,
		const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh = 0);
	void RemoveButtonEventHandler(string instanceName, int id);
	void RemoveButtonEventHandler(int instId, int id);
	void ClearButtonEventHandler(string instanceName);
	void ClearButtonEventHandler(int instId);
	void ClearButtonEventHandler(string instanceName, string type);
	void ClearButtonEventHandler(int instId, string type);
	void SetButtonEventHandler(string instanceName,
		const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh);
	void SetButtonEventHandler(int instId,
		const ButtonEventHandlerDictionary &h, ButtonKeyPressHandler kh);
	void ClearAllEventHandlers();

	void SetText(string textName, string text);
	string GetText(string textName);
	void SetTextRenderer(string fullPath,
		string textName, string text, TextRenderer *textRenderer);
	void ClearTextRenderer(string textName);

	void SetMovieLoadCommand(string instanceName, MovieEventHandler handler);
	void SetMoviePostLoadCommand(string instanceName,
		MovieEventHandler handler);
	void PlayMovie(string instanceName);
	void StopMovie(string instanceName);
	void NextFrameMovie(string instanceName);
	void PrevFrameMovie(string instanceName);
	void SetVisibleMovie(string instanceName, bool visible);
	void GotoAndStopMovie(string instanceName, string label);
	void GotoAndStopMovie(string instanceName, int frameNo);
	void GotoAndPlayMovie(string instanceName, string label);
	void GotoAndPlayMovie(string instanceName, int frameNo);
	void MoveMovie(string instanceName, float vx, float vy);
	void MoveToMovie(string instanceName, float vx, float vy);
	void RotateMovie(string instanceName, float degree);
	void RotateToMovie(string instanceName, float degree);
	void ScaleMovie(string instanceName, float vx, float vy);
	void ScaleToMovie(string instanceName, float vx, float vy);
	void SetMatrixMovie(string instanceName, const Matrix *matrix,
		float sx = 1, float sy = 1, float r = 0);
	void SetAlphaMovie(string instanceName, float v);
	void SetColorTransformMovie(string instanceName, const ColorTransform *c);

#if defined(LWF_USE_LUA)
	void InitLua();
	void DestroyLua();
	void CallLua(int nargs);
	void DestroyMovieLua(Movie *movie);
	bool GetFieldLua(Movie *movie, string key);
	bool SetFieldLua(Movie *movie, string key);
	string GetTextLua(Movie *movie, string textName);
	int AddEventHandlerLua(Movie *movie = 0, Button *button = 0);
	int AddMovieEventHandlerLua();
	int AddButtonEventHandlerLua();
	int AttachMovieLua(Movie *movie);
	bool PushHandlerLua(int handlerId);
	void GetFunctionsLua(int movieId, string &loadFunc, string &postLoadFunc,
		string &unloadFunc, string &enterFrameFunc, bool forRoot);
	void CallFunctionLua(string function, Movie *movie);
	void CallEventFunctionLua(int eventId, Movie *movie, Button *button);
	void SetColorTransformMovieLua(
		string instanceName, float vr, float vg, float vb, float va);
	string GetLuaError() {
		string e = luaError;
		luaError = "";
		return e;
	}
#endif

private:
	const Matrix *CalcMatrix(const Matrix *matrix);
	const ColorTransform *CalcColorTransform(
		const ColorTransform *colorTransform);
};

}	// namespace LWF

#endif
