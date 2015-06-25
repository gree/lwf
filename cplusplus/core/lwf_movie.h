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

#ifndef LWF_MOVIE_H
#define LWF_MOVIE_H

#include "lwf_bitmapclip.h"
#include "lwf_eventmovie.h"
#include "lwf_iobject.h"

namespace LWF {

class Button;
class Property;
class LWFContainer;
struct LabelData;

typedef map<string, shared_ptr<Movie> > AttachedMovies;
typedef map<int, shared_ptr<Movie> > AttachedMovieList;
typedef map<string, shared_ptr<LWFContainer> > AttachedLWFs;
typedef map<int, shared_ptr<LWFContainer> > AttachedLWFList;
typedef map<string, bool> DetachDict;
typedef map<string, bool> Texts;
typedef map<int, shared_ptr<BitmapClip> > BitmapClips;
typedef map<string, MovieEventHandlerList> MovieEventHandlerListDictionary;
typedef vector<MovieEventHandler> CalculateBoundsCallbacks;
typedef vector<LabelData> CurrentLabels;
typedef map<int, string> CurrentLabelCache;

struct LabelData {
	int frame;
	string name;
};

class Movie : public IObject
{
private:
	typedef Format::MovieClipEvent ClipEvent;

public:
	const Format::Movie *data;
	string attachName;
	int totalFrames;
	int currentFrame;
	int depth;
	int blendMode;
	bool visible;
	bool playing;
	bool active;
	bool hasButton;

private:
	shared_ptr<Property> m_property;
	IObject *m_instanceHead;
	IObject *m_instanceTail;
	vector<shared_ptr<Object> > m_displayList;
	MovieEventHandlerListDictionary m_eventHandlers;
	MovieEventHandlers m_handler;
	AttachedMovies m_attachedMovies;
	AttachedMovieList m_attachedMovieList;
	DetachDict m_detachedMovies;
	AttachedLWFs m_attachedLWFs;
	AttachedLWFList m_attachedLWFList;
	DetachDict m_detachedLWFs;
	BitmapClips m_bitmapClips;
	CalculateBoundsCallbacks m_calculateBoundsCallbacks;
	shared_ptr<Texts> m_texts;
	int m_currentFrameInternal;
	int m_currentFrameCurrent;
	int m_execedFrame;
	int m_animationPlayedFrame;
	int m_lastControlOffset;
	int m_lastControls;
	int m_lastControlAnimationOffset;
	int m_movieExecCount;
	int m_postExecCount;
	bool m_jumped;
	bool m_overriding;
	bool m_postLoaded;
	bool m_lastHasButton;
	bool m_skipped;
	bool m_attachMovieExeced;
	bool m_attachMoviePostExeced;
	bool m_isRoot;
	bool m_needsUpdateAttachedLWFs;
	bool m_requestedCalculateBounds;
	Matrix m_matrix0;
	Matrix m_matrix1;
	Matrix m_matrixForAttachedLWFs;
	ColorTransform m_colorTransform0;
	ColorTransform m_colorTransform1;
	ColorTransform m_colorTransformForAttachedLWFs;
	Bounds m_bounds;
	Bounds m_currentBounds;
	CurrentLabels m_currentLabelsCache;
	CurrentLabelCache m_currentLabelCache;
	bool m_currentLabelsCached;
#if defined(LWF_USE_LUA)
	string m_rootLoadFunc;
	string m_rootPostLoadFunc;
	string m_rootUnloadFunc;
	string m_rootEnterFrameFunc;
	string m_loadFunc;
	string m_postLoadFunc;
	string m_unloadFunc;
	string m_enterFrameFunc;
#endif

public:
	Movie(LWF *l, Movie *p, int objId, int instId, int mId = 0, int cId = 0,
		bool attached = false, const MovieEventHandlers *handler = 0,
		string n = string());

	void AddHandlers(const MovieEventHandlers *h);

	Point GlobalToLocal(const Point &point) const;
	Point LocalToGlobal(const Point &point) const;

	void Override(bool overriding);
	void Exec(int matrixId = 0, int colorTransformId = 0);
	void PostExec(bool progressing);
	bool ExecAttachedLWF(float tick, float currentProgress);

	void Update(const Matrix *m, const ColorTransform *c);
	void PostUpdate();
	void UpdateAttachedLWF();
	void CalculateBounds(Object *o);
	void LinkButton();
	void Render(bool v, int rOffset);
	void Inspect(Inspector inspector,
		int hierarchy, int inspectDepth, int rOffset);

	void Destroy();

	int SearchFrame(string label) const;
	int SearchFrame(int stringId) const;

	Movie *SearchMovieInstance(int stringId, bool recursive = true) const;
	Movie *SearchMovieInstance(
		string instanceName, bool recursive = true) const;
	Movie *operator[](string instanceName) const;
	Movie *SearchMovieInstanceByInstanceId(
		int instId, bool recursive = true) const;

	Button *SearchButtonInstance(int stringId, bool recursive = true) const;
	Button *SearchButtonInstance(
		string instanceName, bool recursive = true) const;
	Button *SearchButtonInstanceByInstanceId(
		int instId, bool recursive = true) const;

	void InsertText(int objId);
	void EraseText(int objId);
	bool SearchText(string textName);

	int AddEventHandler(string eventName, MovieEventHandler eventHandler);
	void RemoveEventHandler(string eventName, int id);
	void RemoveMovieEventHandler(int id);
	void ClearEventHandler(string eventName);
	void ClearMovieEventHandler();
	virtual void ClearAllEventHandler();
	int SetEventHandler(string eventName, MovieEventHandler eventHandler);
	void DispatchEvent(string eventName);

	Movie *Play();
	Movie *Stop();
	Movie *NextFrame();
	Movie *PrevFrame();
	Movie *GotoFrame(int frameNo);
	Movie *GotoFrameInternal(int frameNo);
	Movie *SetVisible(bool visible);
	Movie *GotoLabel(string label);
	Movie *GotoLabel(int stringId);
	Movie *GotoAndStop(string label);
	Movie *GotoAndStop(int frameNo);
	Movie *GotoAndPlay(string label);
	Movie *GotoAndPlay(int frameNo);
	Movie *Move(float vx, float vy);
	Movie *MoveTo(float vx, float vy);
	Movie *Rotate(float degree);
	Movie *RotateTo(float degree);
	Movie *Scale(float vx, float vy);
	Movie *ScaleTo(float vx, float vy);
	Movie *SetMatrix(const Matrix *m, float sx = 1, float sy = 1, float r = 0);
	Movie *SetAlphaValue(float v);
	Movie *SetColorTransform(const ColorTransform *c);
	Movie *SetRenderingOffset(int rOffset);

	float GetX() const;
	void SetX(float value);
	float GetY() const;
	void SetY(float value);
	float GetScaleX() const;
	void SetScaleX(float value);
	float GetScaleY() const;
	void SetScaleY(float value);
	float GetRotation() const;
	void SetRotation(float value);
	float GetAlpha() const;
	void SetAlpha(float value);
	float GetRed() const;
	void SetRed(float value);
	float GetGreen() const;
	void SetGreen(float value);
	float GetBlue() const;
	void SetBlue(float value);

	Movie *AttachMovie(string linkageName, string attachName,
		const MovieEventHandlerDictionary &h,
		int attachDepth = -1, bool reorder = false);
	Movie *AttachMovie(string linkageName, string attachName,
		int attachDepth = -1, bool reorder = false);
	Movie *AttachEmptyMovie(string attachName,
		const MovieEventHandlerDictionary &h,
		int attachDepth = -1, bool reorder = false);
	Movie *AttachEmptyMovie(string attachName,
		int attachDepth = -1, bool reorder = false);
	void SwapAttachedMovieDepth(int depth0, int depth1);
	Movie *GetAttachedMovie(string aName) const;
	Movie *GetAttachedMovie(int aDepth) const;
	Movie *SearchAttachedMovie(string aName, bool recursive = true) const;
	void DetachMovie(string aName);
	void DetachMovie(int aDepth);
	void DetachMovie(Movie *movie);
	void DetachFromParent();

	shared_ptr<LWF> AttachLWF(
		string path, string aName, int aDepth = -1, bool reorder = false);
	void AttachLWF(shared_ptr<LWF> attachLWF, string aName,
		DetachHandler detachHandler, int aDepth = -1, bool reorder = false);
	void AttachLWF(shared_ptr<LWF> attachLWF, string aName,
		int aDepth = -1, bool reorder = false);
	void SwapAttachedLWFDepth(int depth0, int depth1);
	shared_ptr<LWF> GetAttachedLWF(string aName) const;
	shared_ptr<LWF> GetAttachedLWF(int aDepth) const;
	shared_ptr<LWF> SearchAttachedLWF(
		string aName, bool recursive = true) const;
	void DetachLWF(string aName);
	void DetachLWF(int aDepth);
	void DetachLWF(shared_ptr<LWF> detachLWF);
	void DetachAllLWFs();
	void RemoveMovieClip();

	shared_ptr<BitmapClip> AttachBitmap(string linkageName, int depth);
	BitmapClips GetAttachedBitmaps();
	shared_ptr<BitmapClip> GetAttachedBitmap(int depth);
	void SwapAttachedBitmapDepth(int depth0, int depth1);
	void DetachBitmap(int depth);

	void RequestCalculateBounds(MovieEventHandler callback = 0);
	Bounds GetBounds();

	string GetCurrentLabel();
	const CurrentLabels GetCurrentLabels();

private:
	void ExecObject(int dlDepth, int objId, int matrixId, int colorTransformId,
		int instId, int dlBlendMode, bool updateBlendMode = false);
	void UpdateObject(Object *obj, const Matrix *m, const ColorTransform *c,
		bool matrixChanged, bool colorTransformChanged);
	void PlayAnimation(int clipEvent);
	void ReorderAttachedMovieList(
		bool reorder, int index, shared_ptr<Movie> movie);
	void DeleteAttachedMovie(Movie *p, shared_ptr<Movie> movie,
		bool destroy = true, bool deleteFromDetachedMovies = true);
	void ReorderAttachedLWFList(bool reorder,
		int index, shared_ptr<LWFContainer> lwfContainer);
	void DeleteAttachedLWF(Movie *p, shared_ptr<LWFContainer> lwfContainer,
		bool destroy = true, bool deleteFromDetachedLWFs = true);
	void UpdateBounds(
		const Matrix *m, float xmin, float xmax, float ymin, float ymax);
	void UpdateBounds(const Matrix *m, float sx, float sy);
	void CacheCurrentLabels();
};

}	// namespace LWF

#endif
