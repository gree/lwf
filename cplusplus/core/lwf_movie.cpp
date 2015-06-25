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

#include "lwf_bitmap.h"
#include "lwf_bitmapex.h"
#include "lwf_button.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_eventbutton.h"
#include "lwf_graphic.h"
#include "lwf_lwfcontainer.h"
#include "lwf_movie.h"
#include "lwf_particle.h"
#include "lwf_property.h"
#include "lwf_programobj.h"
#include "lwf_text.h"
#include "lwf_utility.h"
#include "lwf_compat.h"

namespace LWF {

typedef ButtonEventHandlers BEType;
typedef MovieEventHandlers METype;

class CalculateBoundsWrapper
{
private:
	Movie *m_movie;
public:
	CalculateBoundsWrapper(Movie *m) : m_movie(m) {}
	void operator()(Object *o, int, int, int)
	{
		m_movie->CalculateBounds(o);
	}
};

Movie::Movie(LWF *l, Movie *p, int objId, int instId, int mId, int cId,
		bool attached, const MovieEventHandlers *handler, string n)
	: IObject(l, p,
		attached ? OType::ATTACHEDMOVIE : OType::MOVIE, objId, instId)
{
	matrixId = mId;
	colorTransformId = cId;

	data = &lwf->data->movies[objId];
	totalFrames = data->frames;

	if (!n.empty())
		name = n;

	depth = 0;
	blendMode = Format::BLEND_MODE_NORMAL;
	visible = true;
	playing = true;
	active = true;
	hasButton = false;

	m_property = make_shared<Property>(lwf);
	m_instanceHead = 0;
	m_instanceTail = 0;

	m_currentFrameInternal = -1;
	currentFrame = 0;
	m_currentFrameCurrent = -1;
	m_execedFrame = -1;
	m_animationPlayedFrame = -1;
	m_lastControlOffset = -1;
	m_lastControls = -1;
	m_lastControlAnimationOffset = -1;
	m_movieExecCount = -1;
	m_postExecCount = -1;
	m_jumped = false;
	m_overriding = false;
	m_postLoaded = false;
	m_lastHasButton = false;
	m_skipped = false;
	m_attachMovieExeced = false;
	m_attachMoviePostExeced = false;
	m_isRoot = objId == lwf->data->header.rootMovieId;
	m_requestedCalculateBounds = false;
	m_currentLabelsCached = false;

	m_displayList.resize(data->depths);

#if defined(LWF_USE_LUA)
	if (m_isRoot) {
		if (!parent)
			lwf->CallFunctionLua("Init", this);
		lwf->GetFunctionsLua(objId, m_rootLoadFunc, m_rootPostLoadFunc,
			m_rootUnloadFunc, m_rootEnterFrameFunc, true);
	}
	lwf->GetFunctionsLua(objId,
		m_loadFunc, m_postLoadFunc, m_unloadFunc, m_enterFrameFunc, false);

	if (m_isRoot && !m_rootLoadFunc.empty())
		lwf->CallFunctionLua(m_rootLoadFunc, this);
	if (!m_loadFunc.empty())
		lwf->CallFunctionLua(m_loadFunc, this);
#endif
	PlayAnimation(ClipEvent::LOAD);

	m_handler.Add(lwf->GetMovieEventHandlers(this));
	m_handler.Add(handler);
	if (!m_handler.Empty())
		m_handler.Call(METype::LOAD, this);

	lwf->ExecMovieCommand();
}

void Movie::AddHandlers(const MovieEventHandlers *h)
{
	m_handler.Add(h);
}

Point Movie::GlobalToLocal(const Point &point) const
{
	float px;
	float py;
	Matrix invert;
	Utility::InvertMatrix(&invert, &matrix);
	Utility::CalcMatrixToPoint(px, py, point.x, point.y, &invert);
	Point p(px, py);
	return p;
}

Point Movie::LocalToGlobal(const Point &point) const
{
	float px;
	float py;
	Utility::CalcMatrixToPoint(px, py, point.x, point.y, &matrix);
	Point p(px, py);
	return p;
}

void Movie::ExecObject(int dlDepth, int objId, int matrixId,
	int colorTransformId, int instId, int dlBlendMode, bool updateBlendMode)
{
	// Ignore error
	if (objId == -1)
		return;
	const Data *d = lwf->data.get();
	const Format::Object &dataObject = d->objects[objId];
	int dataObjectId = dataObject.objectId;
	shared_ptr<Object> obj = m_displayList[dlDepth];

	if (obj && (obj->type != dataObject.objectType ||
			obj->objectId != dataObjectId || (obj->IsMovie() &&
			((IObject *)obj.get())->instanceId != instId))) {
		if (m_texts && obj->IsText())
			EraseText(obj->objectId);
		obj->Destroy();
		obj.reset();
	}

	if (!obj) {
		switch (dataObject.objectType) {
		case OType::BUTTON:
			obj = make_shared<Button>(lwf,
				this, dataObjectId, instId, matrixId, colorTransformId);
			break;

		case OType::GRAPHIC:
			obj = make_shared<Graphic>(lwf, this, dataObjectId);
			break;

		case OType::MOVIE:
			obj = make_shared<Movie>(lwf, this,
				dataObjectId, instId, matrixId, colorTransformId);
			((Movie *)obj.get())->blendMode = dlBlendMode;
			break;

		case OType::BITMAP:
			obj = make_shared<Bitmap>(lwf, this, dataObjectId);
			break;

		case OType::BITMAPEX:
			obj = make_shared<BitmapEx>(lwf, this, dataObjectId);
			break;

		case OType::TEXT:
			obj = make_shared<Text>(lwf, this, dataObjectId, instId);
			break;

		case OType::PARTICLE:
			obj = make_shared<Particle>(lwf, this, dataObjectId);
			break;

		case OType::PROGRAMOBJECT:
			obj = make_shared<ProgramObject>(lwf, this, dataObjectId);
			break;
		}
	}

	if (obj->IsMovie() && updateBlendMode)
		((Movie *)obj.get())->blendMode = dlBlendMode;

	if (obj->IsMovie() || obj->IsButton()) {
		IObject *instance = (IObject *)obj.get();
		instance->linkInstance = 0;
		if (!m_instanceHead)
			m_instanceHead = instance;
		else
			m_instanceTail->linkInstance = instance;
		m_instanceTail = instance;
		if (obj->IsButton())
			hasButton = true;
	}

	if (m_texts && obj->IsText())
		InsertText(obj->objectId);

	m_displayList[dlDepth] = obj;
	obj->execCount = m_movieExecCount;
	obj->Exec(matrixId, colorTransformId);
}

void Movie::Override(bool overriding)
{
	m_overriding = overriding;
}

void Movie::Exec(int matrixId, int colorTransformId)
{
	m_attachMovieExeced = false;
	m_attachMoviePostExeced = false;
	IObject::Exec(matrixId, colorTransformId);
}

void Movie::PostExec(bool progressing)
{
	hasButton = false;
	if (!active)
		return;

	m_execedFrame = -1;
	bool postExeced = m_postExecCount == lwf->execCount;
	if (progressing && playing && !m_jumped && !postExeced) {
		++m_currentFrameInternal;
		currentFrame = m_currentFrameInternal + 1;
	}
	for (;;) {
		if (m_currentFrameInternal < 0 ||
				m_currentFrameInternal >= totalFrames) {
			m_currentFrameInternal = 0;
			currentFrame = m_currentFrameInternal + 1;
		}
		if (m_currentFrameInternal == m_execedFrame)
			break;

		m_currentFrameCurrent = m_currentFrameInternal;
		m_execedFrame = m_currentFrameCurrent;
		const Data *d = lwf->data.get();
		const Format::Frame &frame = d->frames[
			data->frameOffset + m_currentFrameCurrent];

		int controlAnimationOffset;
		IObject *instance;

		if (m_lastControlOffset == frame.controlOffset &&
				m_lastControls == frame.controls) {

			controlAnimationOffset = m_lastControlAnimationOffset;

			if (m_skipped) {
				instance = m_instanceHead;
				while (instance) {
					if (instance->IsMovie()) {
						Movie *movie = (Movie *)instance;
						movie->m_attachMovieExeced = false;
						movie->m_attachMoviePostExeced = false;
					} else if (instance->IsButton()) {
						((Button *)instance)->EnterFrame();
					}
					instance = instance->linkInstance;
				}
				hasButton = m_lastHasButton;
			} else {
				for (int dlDepth = 0; dlDepth < data->depths; ++dlDepth) {
					Object *obj = m_displayList[dlDepth].get();
					if (obj) {
						if (!postExeced) {
							obj->matrixIdChanged = false;
							obj->colorTransformIdChanged = false;
						}
						if (obj->IsMovie()) {
							Movie *movie = (Movie *)obj;
							movie->m_attachMovieExeced = false;
							movie->m_attachMoviePostExeced = false;
						} else if (obj->IsButton()) {
							((Button *)obj)->EnterFrame();
							hasButton = true;
						}
					}
				}
				m_lastHasButton = hasButton;
				m_skipped = true;
			}

		} else {
			++m_movieExecCount;
			m_instanceHead = 0;
			m_instanceTail = 0;
			m_lastControlOffset = frame.controlOffset;
			m_lastControls = frame.controls;
			controlAnimationOffset = -1;
			for (int i = 0; i < frame.controls; ++i) {
				const Format::Control &control =
					d->controls[frame.controlOffset + i];

				switch (control.controlType) {
				case Format::Control::MOVE:
					{
						const Format::Place &p = d->places[control.controlId];
						ExecObject(p.depth, p.objectId,
							p.matrixId, 0, p.instanceId, p.blendMode);
					}
					break;

				case Format::Control::MOVEM:
					{
						const Format::ControlMoveM &ctrl =
							d->controlMoveMs[control.controlId];
						const Format::Place &p = d->places[ctrl.placeId];
						ExecObject(p.depth, p.objectId,
							ctrl.matrixId, 0, p.instanceId, p.blendMode);
					}
					break;

				case Format::Control::MOVEC:
					{
						const Format::ControlMoveC &ctrl =
							d->controlMoveCs[control.controlId];
						const Format::Place &p = d->places[ctrl.placeId];
						ExecObject(p.depth, p.objectId, p.matrixId,
							ctrl.colorTransformId, p.instanceId, p.blendMode);
					}
					break;

				case Format::Control::MOVEMC:
					{
						const Format::ControlMoveMC &ctrl =
							d->controlMoveMCs[control.controlId];
						const Format::Place &p = d->places[ctrl.placeId];
						ExecObject(p.depth, p.objectId, ctrl.matrixId,
							ctrl.colorTransformId, p.instanceId, p.blendMode);
					}
					break;

				case Format::Control::MOVEMCB:
					{
						const Format::ControlMoveMCB &ctrl =
							d->controlMoveMCBs[control.controlId];
						const Format::Place &p = d->places[ctrl.placeId];
						ExecObject(p.depth, p.objectId, ctrl.matrixId,
							ctrl.colorTransformId, p.instanceId,
							ctrl.blendMode);
					}
					break;

				case Format::Control::ANIMATION:
					if (controlAnimationOffset == -1)
						controlAnimationOffset = i;
					break;

				case Format::Control::CONTROL_MAX:
					// SUPPRESS WARNING
					break;
				}
			}

			m_lastControlAnimationOffset = controlAnimationOffset;
			m_lastHasButton = hasButton;

			for (int dlDepth = 0; dlDepth < data->depths; ++dlDepth) {
				Object *obj = m_displayList[dlDepth].get();
				if (obj && obj->execCount != m_movieExecCount) {
					if (m_texts && obj->IsText())
						EraseText(obj->objectId);
					obj->Destroy();
					m_displayList[dlDepth].reset();
				}
			}
		}

		m_attachMovieExeced = true;
		if (!m_attachedMovies.empty()) {
			AttachedMovieList::iterator it(m_attachedMovieList.begin()),
				itend(m_attachedMovieList.end());
			for (; it != itend; ++it)
				it->second->Exec();
		}

		instance = m_instanceHead;
		while (instance) {
			if (instance->IsMovie()) {
				Movie *movie = (Movie *)instance;
				movie->PostExec(progressing);
				if (!hasButton && movie->hasButton)
					hasButton = true;
			}
			instance = instance->linkInstance;
		}

		m_attachMoviePostExeced = true;
		if (!m_attachedMovies.empty()) {
			DetachDict::const_iterator
				dit(m_detachedMovies.begin()), ditend(m_detachedMovies.end());
			for (; dit != ditend; ++dit) {
				AttachedMovies::iterator it = m_attachedMovies.find(dit->first);
				if (it != m_attachedMovies.end())
					DeleteAttachedMovie(this, it->second, true, false);
			}
			m_detachedMovies.clear();

			AttachedMovieList::iterator it(m_attachedMovieList.begin()),
				itend(m_attachedMovieList.end());
			for (; it != itend; ++it) {
				it->second->PostExec(progressing);
				if (!hasButton && it->second->hasButton)
					hasButton = true;
			}
		}

		if (!m_attachedLWFs.empty())
			hasButton = true;

		if (!m_postLoaded) {
			m_postLoaded = true;
#if defined(LWF_USE_LUA)
			if (m_isRoot && !m_rootPostLoadFunc.empty())
				lwf->CallFunctionLua(m_rootPostLoadFunc, this);
			if (!m_postLoadFunc.empty())
				lwf->CallFunctionLua(m_postLoadFunc, this);
#endif
			if (!m_handler.Empty())
				m_handler.Call(METype::POSTLOAD, this);
		}

		if (controlAnimationOffset != -1 &&
				m_execedFrame == m_currentFrameInternal) {
			bool animationPlayed = m_animationPlayedFrame ==
				m_currentFrameCurrent && !m_jumped;
			if (!animationPlayed) {
				for (int i = controlAnimationOffset;
						i < frame.controls; ++i) {
					const Format::Control &control =
						d->controls[frame.controlOffset + i];
					lwf->PlayAnimation(control.controlId, this);
				}
			}
		}

		m_animationPlayedFrame = m_currentFrameCurrent;
		if (m_currentFrameCurrent == m_currentFrameInternal)
			m_jumped = false;
	}

#if defined(LWF_USE_LUA)
	if (m_isRoot && !m_rootEnterFrameFunc.empty())
		lwf->CallFunctionLua(m_rootEnterFrameFunc, this);
	if (!m_enterFrameFunc.empty())
		lwf->CallFunctionLua(m_enterFrameFunc, this);
#endif
	PlayAnimation(ClipEvent::ENTERFRAME);
	if (!m_handler.Empty())
		m_handler.Call(METype::ENTERFRAME, this);
	m_postExecCount = lwf->execCount;
}

bool Movie::ExecAttachedLWF(float tick, float currentProgress)
{
	bool hasBtn = false;
	for (IObject *instance = m_instanceHead; instance;
			instance = instance->linkInstance) {
		if (instance->IsMovie()) {
			Movie *movie = (Movie *)instance;
			hasBtn |= movie->ExecAttachedLWF(tick, currentProgress);
		}
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it)
			hasBtn |= it->second->ExecAttachedLWF(tick, currentProgress);
	}

	if (!m_attachedLWFs.empty()) {
		DetachDict::const_iterator
			dit(m_detachedLWFs.begin()), ditend(m_detachedLWFs.end());
		for (; dit != ditend; ++dit) {
			AttachedLWFs::iterator it = m_attachedLWFs.find(dit->first);
			if (it != m_attachedLWFs.end())
				DeleteAttachedLWF(this, it->second, true, false);
		}
		m_detachedLWFs.clear();

		AttachedLWFList::iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it) {
			shared_ptr<LWF> &child = it->second->child;
			if (child->tick == lwf->tick)
				child->SetProgress(currentProgress);
			lwf->RenderObject(child->ExecInternal(tick));
			hasBtn |= child->rootMovie->hasButton;
		}
	}

	return hasBtn;
}

void Movie::UpdateObject(Object *obj, const Matrix *m, const ColorTransform *c,
	bool matrixChanged, bool colorTransformChanged)
{
	const Matrix *objm;
	if (obj->IsMovie() && ((Movie *)obj)->m_property->hasMatrix)
		objm = m;
	else if (matrixChanged || !obj->updated || obj->matrixIdChanged)
		objm = Utility::CalcMatrix(lwf, &m_matrix1, m, obj->matrixId);
	else
		objm = 0;

	const ColorTransform *objc;
	if (obj->IsMovie() && ((Movie *)obj)->m_property->hasColorTransform)
		objc = c;
	else if (colorTransformChanged ||
			!obj->updated || obj->colorTransformIdChanged)
		objc = Utility::CalcColorTransform(
			lwf, &m_colorTransform1, c, obj->colorTransformId);
	else
		objc = 0;

	obj->Update(objm, objc);
}

void Movie::Update(const Matrix *m, const ColorTransform *c)
{
	if (!active)
		return;

	bool matrixChanged;
	bool colorTransformChanged;

	if (m_overriding) {
		matrixChanged = true;
		colorTransformChanged = true;
	} else {
		matrixChanged = matrix.SetWithComparing(m);
		colorTransformChanged = colorTransform.SetWithComparing(c);
	}

	if (m_property->hasMatrix) {
		matrixChanged = true;
		m = Utility::CalcMatrix(&m_matrix0, &matrix, &m_property->matrix);
	} else {
		m = &matrix;
	}

	if (m_property->hasColorTransform) {
		colorTransformChanged = true;
		c = Utility::CalcColorTransform(
			&m_colorTransform0, &colorTransform, &m_property->colorTransform);
	} else {
		c = &colorTransform;
	}

	if (!m_attachedLWFs.empty()) {
		m_needsUpdateAttachedLWFs = false;
		m_needsUpdateAttachedLWFs |=
			m_matrixForAttachedLWFs.SetWithComparing(m);
		m_needsUpdateAttachedLWFs |=
			m_colorTransformForAttachedLWFs.SetWithComparing(c);
	}

	for (int dlDepth = 0; dlDepth < data->depths; ++dlDepth) {
		Object *obj = m_displayList[dlDepth].get();
		if (obj)
			UpdateObject(obj, m, c, matrixChanged, colorTransformChanged);
	}

	if (!m_bitmapClips.empty()) {
		BitmapClips::iterator it(m_bitmapClips.begin()),
			itend(m_bitmapClips.end());
		for (; it != itend; ++it)
			it->second->Update(m, c);
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it)
			it->second->UpdateObject(it->second.get(),
				m, c, matrixChanged, colorTransformChanged);
	}
}

void Movie::PostUpdate()
{
	for (IObject *instance = m_instanceHead; instance;
			instance = instance->linkInstance) {
		if (instance->IsMovie())
			((Movie *)instance)->PostUpdate();
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it)
			it->second->PostUpdate();
	}

	if (m_requestedCalculateBounds) {
		m_currentBounds.xMin = FLT_MAX;
		m_currentBounds.xMax = -FLT_MAX;
		m_currentBounds.yMin = FLT_MAX;
		m_currentBounds.yMax = -FLT_MAX;

		Inspect(CalculateBoundsWrapper(this), 0, 0, 0);
		if (lwf->property->hasMatrix) {
			Matrix invert;
			Utility::InvertMatrix(&invert, &lwf->property->matrix);
			float px;
			float py;
			Utility::CalcMatrixToPoint(
				px, py, m_currentBounds.xMin, m_currentBounds.yMin, &invert);
			m_currentBounds.xMin = px;
			m_currentBounds.yMin = py;
			Utility::CalcMatrixToPoint(
				px, py, m_currentBounds.xMax, m_currentBounds.yMax, &invert);
			m_currentBounds.xMax = px;
			m_currentBounds.yMax = py;
		}

		m_bounds = m_currentBounds;
		for (const auto& callback : m_calculateBoundsCallbacks) {
			callback(this);
		}
		m_requestedCalculateBounds = false;
		m_calculateBoundsCallbacks.clear();		
	}

	if (!m_handler.Empty())
		m_handler.Call(METype::UPDATE, this);
}

void Movie::UpdateAttachedLWF()
{
	for (IObject *instance = m_instanceHead; instance;
			instance = instance->linkInstance) {
		if (instance->IsMovie()) {
			Movie *movie = (Movie *)instance;
			movie->UpdateAttachedLWF();
		}
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it)
			it->second->UpdateAttachedLWF();
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFList::iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it) {
			shared_ptr<LWF> &child = it->second->child;
			bool needsUpdateAttachedLWFs =
				child->needsUpdate || m_needsUpdateAttachedLWFs;
			if (needsUpdateAttachedLWFs)
				child->Update(&m_matrixForAttachedLWFs,
					&m_colorTransformForAttachedLWFs);
			if (child->isLWFAttached)
				child->rootMovie->UpdateAttachedLWF();
			if (needsUpdateAttachedLWFs)
				child->rootMovie->PostUpdate();
		}
	}
}

void Movie::CalculateBounds(Object *o)
{
	switch (o->type) {
	case OType::GRAPHIC:
		{
			Graphic::DisplayList &d = ((Graphic *)o)->displayList;
			Graphic::DisplayList::iterator it(d.begin()), itend(d.end());
			for (; it != itend; ++it)
				CalculateBounds(it->get());
		}
		break;

	case OType::BITMAP:
	case OType::BITMAPEX:
		{
			int tfId = -1;
			if (o->type == OType::BITMAP) {
				if (o->objectId < o->lwf->data->bitmaps.size())
					tfId = o->lwf->data->bitmaps[o->objectId].textureFragmentId;
			} else {
				if (o->objectId < o->lwf->data->bitmapExs.size())
					tfId = o->lwf->data->bitmapExs[
						o->objectId].textureFragmentId;
			}
			if (tfId >= 0) {
				const Format::TextureFragment &tf =
					o->lwf->data->textureFragments[tfId];
				UpdateBounds(&o->matrix, tf.x, tf.x + tf.w, tf.y, tf.y + tf.h);
			}
		}
		break;

	case OType::BUTTON:
		{
			Button *button = (Button *)o;
			UpdateBounds(&o->matrix, 0, button->width, 0, button->height);
		}
		break;

	case OType::TEXT:
		{
			const Format::Text &text = o->lwf->data->texts[o->objectId];
			UpdateBounds(&o->matrix, 0, text.width, 0, text.height);
		}
		break;

	case OType::PROGRAMOBJECT:
		{
			const Format::ProgramObject &pobj =
				o->lwf->data->programObjects[o->objectId];
			UpdateBounds(&o->matrix, 0, pobj.width, 0, pobj.height);
		}
		break;
	}
}

void Movie::UpdateBounds(
	const Matrix *m, float xmin, float xmax, float ymin, float ymax)
{
	UpdateBounds(m, xmin, ymin);
	UpdateBounds(m, xmin, ymax);
	UpdateBounds(m, xmax, ymin);
	UpdateBounds(m, xmax, ymax);
}

void Movie::UpdateBounds(const Matrix *m, float sx, float sy)
{
	float px;
	float py;
	Utility::CalcMatrixToPoint(px, py, sx, sy, m);
	if (px < m_currentBounds.xMin)
		m_currentBounds.xMin = px;
	else if (px > m_currentBounds.xMax)
		m_currentBounds.xMax = px;
	if (py < m_currentBounds.yMin)
		m_currentBounds.yMin = py;
	else if (py > m_currentBounds.yMax)
		m_currentBounds.yMax = py;
}

void Movie::LinkButton()
{
	if (!visible || !active || !hasButton)
		return;

	for (int dlDepth = 0; dlDepth < data->depths; ++dlDepth) {
		Object *obj = m_displayList[dlDepth].get();
		if (obj) {
			if (obj->IsButton()) {
				((Button *)obj)->LinkButton();
			} else if (obj->IsMovie()) {
				Movie *movie = (Movie *)obj;
				if (movie->hasButton)
					movie->LinkButton();
			}
		}
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it) {
			if (it->second && it->second->hasButton)
				it->second->LinkButton();
		}
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFList::iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it)
			it->second->LinkButton();
	}
}

void Movie::Render(bool v, int rOffset)
{
	if (!visible || !active)
		v = false;

	bool useBlendMode = false;
	bool useMaskMode = false;
	if (blendMode != Format::BLEND_MODE_NORMAL) {
		switch (blendMode) {
		case Format::BLEND_MODE_ADD:
		case Format::BLEND_MODE_MULTIPLY:
		case Format::BLEND_MODE_SCREEN:
		case Format::BLEND_MODE_SUBTRACT:
			lwf->BeginBlendMode(blendMode);
			useBlendMode = true;
			break;
		case Format::BLEND_MODE_ERASE:
		case Format::BLEND_MODE_LAYER:
		case Format::BLEND_MODE_MASK:
			lwf->BeginMaskMode(blendMode);
			useMaskMode = true;
			break;
		}
	}

	if (v && !m_handler.Empty())
		m_handler.Call(METype::RENDER, this);

	if (m_property->hasRenderingOffset) {
		lwf->RenderOffset();
		rOffset = m_property->renderingOffset;
	}
	if (rOffset == INT_MIN)
		lwf->ClearRenderOffset();

	for (int dlDepth = 0; dlDepth < data->depths; ++dlDepth) {
		Object *obj = m_displayList[dlDepth].get();
		if (obj)
			obj->Render(v, rOffset);
	}

	if (!m_bitmapClips.empty()) {
		BitmapClips::iterator it(m_bitmapClips.begin()),
			itend(m_bitmapClips.end());
		for (; it != itend; ++it) {
			shared_ptr<BitmapClip> &bitmapClip = it->second;
			bitmapClip->Render(v && bitmapClip->visible, rOffset);
		}
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it)
			it->second->Render(v, rOffset);
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFList::iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it) {
			LWF *child = it->second->child.get();
			child->SetAttachVisible(v);
			lwf->RenderObject(child->Render(lwf->renderingIndex,
				lwf->renderingCount, rOffset));
		}
	}

	if (useBlendMode)
		lwf->EndBlendMode();
	if (useMaskMode)
		lwf->EndMaskMode();
}

void Movie::Inspect(
	Inspector inspector, int hierarchy, int inspectDepth, int rOffset)
{
	if (m_property->hasRenderingOffset) {
		lwf->RenderOffset();
		rOffset = m_property->renderingOffset;
	}
	if (rOffset == INT_MIN)
		lwf->ClearRenderOffset();

	inspector(this, hierarchy, inspectDepth, rOffset);

	++hierarchy;

	int d;
	for (d = 0; d < data->depths; ++d) {
		Object *obj = m_displayList[d].get();
		if (obj)
			obj->Inspect(inspector, hierarchy, d, rOffset);
	}

	if (!m_bitmapClips.empty()) {
		BitmapClips::iterator it(m_bitmapClips.begin()),
			itend(m_bitmapClips.end());
		for (; it != itend; ++it) {
			it->second->Inspect(inspector, hierarchy, d++, rOffset);
		}
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::iterator
			it(m_attachedMovieList.begin()), itend(m_attachedMovieList.end());
		for (; it != itend; ++it)
			it->second->Inspect(inspector, hierarchy, d++, rOffset);
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFList::iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it) {
			lwf->RenderObject(it->second->child->Inspect(
				inspector, hierarchy, d++, rOffset));
		}
	}
}

void Movie::Destroy()
{
	for (int dlDepth = 0; dlDepth < data->depths; ++dlDepth) {
		Object *obj = m_displayList[dlDepth].get();
		if (obj)
			obj->Destroy();
	}

	if (!m_bitmapClips.empty()) {
		BitmapClips::iterator it(m_bitmapClips.begin()),
			itend(m_bitmapClips.end());
		for (; it != itend; ++it) {
			it->second->Destroy();
		}
		m_bitmapClips.clear();
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovies::iterator
			it(m_attachedMovies.begin()), itend(m_attachedMovies.end());
		for (; it != itend; ++it)
			it->second->Destroy();
		m_attachedMovies.clear();
		m_attachedMovieList.clear();
		m_detachedMovies.clear();
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFs::iterator
			it(m_attachedLWFs.begin()), itend(m_attachedLWFs.end());
		for (; it != itend; ++it) {
			if (it->second->child->detachHandler) {
				if (it->second->child->detachHandler(it->second->child.get()))
					it->second->child.get()->Destroy();
			} else {
				it->second->child.get()->Destroy();
			}
		}
		m_attachedLWFs.clear();
		m_attachedLWFList.clear();
		m_detachedLWFs.clear();
	}

#if defined(LWF_USE_LUA)
	if (m_isRoot && !m_rootUnloadFunc.empty())
		lwf->CallFunctionLua(m_rootUnloadFunc, this);
	if (!m_unloadFunc.empty())
		lwf->CallFunctionLua(m_unloadFunc, this);
#endif
	PlayAnimation(ClipEvent::UNLOAD);

	if (!m_handler.Empty())
		m_handler.Call(METype::UNLOAD, this);

#if defined(LWF_USE_LUA)
	lwf->DestroyMovieLua(this);
#endif

	m_displayList.clear();
	m_property.reset();

	IObject::Destroy();
}

void Movie::PlayAnimation(int clipEvent)
{
	const vector<Format::MovieClipEvent> &clipEvents =
		lwf->data->movieClipEvents;
	for (int i = 0; i < data->clipEvents; ++i) {
		const Format::MovieClipEvent &c = clipEvents[data->clipEventId + i];
		if ((c.clipEvent & (int)clipEvent) != 0)
			lwf->PlayAnimation(c.animationId, this);
	}
}

int Movie::SearchFrame(string label) const
{
	return lwf->SearchFrame(this, label);
}

int Movie::SearchFrame(int stringId) const
{
	return lwf->SearchFrame(this, stringId);
}

Movie *Movie::SearchMovieInstance(int stringId, bool recursive) const
{
	if (stringId == -1)
		return 0;

	for (IObject *instance = m_instanceHead; instance;
			instance = instance->linkInstance) {
		if (instance->IsMovie() && lwf->GetInstanceNameStringId(
				instance->instanceId) == stringId) {
			return (Movie *)instance;
		} else if (recursive && instance->IsMovie()) {
			Movie *i = ((Movie *)instance)->SearchMovieInstance(
				stringId, recursive);
			if (i)
				return i;
		}
	}
	return 0;
}

Movie *Movie::SearchMovieInstance(string instanceName, bool recursive) const
{
	int stringId = lwf->GetStringId(instanceName);
	if (stringId != -1)
		return SearchMovieInstance(lwf->GetStringId(instanceName), recursive);

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::const_iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it) {
			if (it->second->attachName == instanceName) {
				return it->second.get();
			} else if (recursive) {
				Movie *movie = it->second->SearchMovieInstance(
					instanceName, recursive);
				if (movie)
					return movie;
			}
		}
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFList::const_iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it) {
			LWF *child = it->second->child.get();
			if (child->attachName == instanceName) {
				return child->rootMovie.get();
			} else if (recursive) {
				Movie *movie = child->rootMovie->SearchMovieInstance(
					instanceName, recursive);
				if (movie)
					return movie;
			}
		}
	}

	return 0;
}

Movie *Movie::operator[](string instanceName) const
{
	return SearchMovieInstance(instanceName, false);
}

Movie *Movie::SearchMovieInstanceByInstanceId(int instId, bool recursive) const
{
	for (IObject *instance = m_instanceHead; instance;
			instance = instance->linkInstance) {
		if (instance->IsMovie() && instance->instanceId == instId) {
			return (Movie *)instance;
		} else if (recursive && instance->IsMovie()) {
			Movie *i = ((Movie *)instance)->SearchMovieInstanceByInstanceId(
				instId, recursive);
			if (i)
				return i;
		}
	}
	return 0;
}

Button *Movie::SearchButtonInstance(int stringId, bool recursive) const
{
	if (stringId == -1)
		return 0;

	for (IObject *instance = m_instanceHead; instance;
			instance = instance->linkInstance) {
		if (instance->IsButton() && lwf->GetInstanceNameStringId(
				instance->instanceId) == stringId) {
			return (Button *)instance;
		} else if (recursive && instance->IsMovie()) {
			Button *i = ((Movie *)instance)->SearchButtonInstance(
				stringId, recursive);
			if (i)
				return i;
		}
	}
	return 0;
}

Button *Movie::SearchButtonInstance(string instanceName, bool recursive) const
{
	int stringId = lwf->GetStringId(instanceName);
	if (stringId != -1)
		return SearchButtonInstance(lwf->GetStringId(instanceName), recursive);

	if (!m_attachedMovies.empty() && recursive) {
		AttachedMovieList::const_iterator it(m_attachedMovieList.begin()),
			itend(m_attachedMovieList.end());
		for (; it != itend; ++it) {
			Button *button =
				it->second->SearchButtonInstance(instanceName, recursive);
			if (button)
				return button;
		}
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFList::const_iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it) {
			LWF *child = it->second->child.get();
			Button *button = child->rootMovie->SearchButtonInstance(
				instanceName, recursive);
			if (button)
				return button;
		}
	}

	return 0;
}

Button *Movie::SearchButtonInstanceByInstanceId(
	int instId, bool recursive) const
{
	for (IObject *instance = m_instanceHead; instance;
			instance = instance->linkInstance) {
		if (instance->IsButton() && instance->instanceId == instId) {
			return (Button *)instance;
		} else if (recursive && instance->IsMovie()) {
			Button *i = ((Movie *)instance)->SearchButtonInstanceByInstanceId(
				instId, recursive);
			if (i)
				return i;
		}
	}
	return 0;
}

void Movie::InsertText(int objId)
{
	const Format::Text &text = lwf->data->texts[objId];
	if (text.nameStringId != -1)
		m_texts->insert(make_pair(lwf->data->strings[text.nameStringId], true));
}

void Movie::EraseText(int objId)
{
	const Format::Text &text = lwf->data->texts[objId];
	if (text.nameStringId != -1)
		m_texts->erase(lwf->data->strings[text.nameStringId]);
}

bool Movie::SearchText(string textName)
{
	if (!m_texts) {
		m_texts = make_shared<Texts>();
		for (int dlDepth = 0; dlDepth < data->depths; ++dlDepth) {
			Object *obj = m_displayList[dlDepth].get();
			if (obj && obj->IsText())
				InsertText(obj->objectId);
		}
	}

	if (m_texts->find(textName) != m_texts->end())
		return true;
	return false;
}

int Movie::AddEventHandler(string eventName, MovieEventHandler eventHandler)
{
	int id = lwf->GetEventOffset();
	if (m_handler.Add(id, eventName, eventHandler))
		return id;

	MovieEventHandlerListDictionary::iterator it =
		m_eventHandlers.find(eventName);
	if (it == m_eventHandlers.end()) {
		m_eventHandlers[eventName] = MovieEventHandlerList();
		it = m_eventHandlers.find(eventName);
	}
	it->second.push_back(make_pair(id, eventHandler));
	return id;
}

class Pred
{
private:
	int id;
public:
	Pred(int i) : id(i) {}
	bool operator()(const pair<int, MovieEventHandler> &h)
	{
		return h.first == id;
	}
};

void Movie::RemoveEventHandler(string eventName, int id)
{
	MovieEventHandlerListDictionary::iterator it =
		m_eventHandlers.find(eventName);
	if (it == m_eventHandlers.end()) {
		m_handler.Remove(id);
		return;
	}

	MovieEventHandlerList& list = it->second;
	list.erase(remove_if(list.begin(), list.end(), Pred(id)), list.end());
}

void Movie::RemoveMovieEventHandler(int id)
{
	m_handler.Remove(id);
}

void Movie::ClearEventHandler(string eventName)
{
	m_eventHandlers.erase(eventName);
	m_handler.Clear(eventName);
}

void Movie::ClearMovieEventHandler()
{
	m_handler.Clear();
}

void Movie::ClearAllEventHandler()
{
	m_eventHandlers.clear();
	m_handler.Clear();
}

int Movie::SetEventHandler(string eventName, MovieEventHandler eventHandler)
{
	ClearEventHandler(eventName);
	return AddEventHandler(eventName, eventHandler);
}

void Movie::DispatchEvent(string eventName)
{
	if (m_handler.Call(eventName, this))
		return;

	scoped_ptr<MovieEventHandlerList> list(
		new MovieEventHandlerList(m_eventHandlers[eventName]));
	MovieEventHandlerList::iterator it(list->begin()), itend(list->end());
	for (; it != itend; ++it)
		it->second(this);
}

void Movie::RequestCalculateBounds(MovieEventHandler callback)
{
	if (!m_requestedCalculateBounds) {
		m_bounds.Clear();
		m_requestedCalculateBounds = true;
	}
	if (callback) {
		m_calculateBoundsCallbacks.push_back(callback);
	}
}

Bounds Movie::GetBounds()
{
	return m_bounds;
}

static struct {
	bool operator()(const LabelData &a, const LabelData &b) {   
		return a.frame < b.frame;
	}
} LabelDataComparator;

void Movie::CacheCurrentLabels()
{
	if (m_currentLabelsCached)
		return;

	m_currentLabelsCached = true;
	const map<int, int> *labels = lwf->GetMovieLabels(this);
	if (labels == 0)
		return;

	map<int, int>::const_iterator it(labels->begin()), itend(labels->end());
	for (; it != itend; ++it) {
		LabelData labelData;
		labelData.frame = it->second + 1;
		labelData.name = lwf->data->strings[it->first];
		m_currentLabelsCache.emplace_back(labelData);
	}
	std::sort(m_currentLabelsCache.begin(),
		m_currentLabelsCache.end(), LabelDataComparator);
}

string Movie::GetCurrentLabel()
{
	CacheCurrentLabels();

	if (m_currentLabelsCache.empty())
		return string();

	int currentFrameTmp = m_currentFrameInternal + 1;
	if (currentFrameTmp < 1)
		currentFrameTmp = 1;

	string labelName;
	CurrentLabelCache::const_iterator it =
		m_currentLabelCache.find(currentFrameTmp);
	if (it != m_currentLabelCache.end()) {
		labelName = it->second;
	} else {
		const LabelData &firstLabel = m_currentLabelsCache.front();
		const LabelData &lastLabel = m_currentLabelsCache.back();
		if (currentFrameTmp < firstLabel.frame) {
			labelName = string();
		} else if (currentFrameTmp == firstLabel.frame) {
			labelName = firstLabel.name;
		} else if (currentFrameTmp >= lastLabel.frame) {
			labelName = lastLabel.name;
		} else {
			int l = 0;
			int ln = m_currentLabelsCache[l].frame;
			int r = (int)m_currentLabelsCache.size() - 1;
			int rn = m_currentLabelsCache[r].frame;
			for (;;) {
				if ((l == r) || (r - l == 1)) {
					if (currentFrameTmp < ln)
						labelName = string();
					else if (currentFrameTmp == rn)
						labelName = m_currentLabelsCache[r].name;
					else
						labelName = m_currentLabelsCache[l].name;
					break;
				}
				int n = (int)floorf((r - l) / 2.0f) + l;
				int nn = m_currentLabelsCache[n].frame;
				if (currentFrameTmp < nn) {
					r = n;
					rn = nn;
				} else if (currentFrameTmp > nn) {
					l = n;
					ln = nn;
				} else {
					labelName = m_currentLabelsCache[n].name;
					break;
				}
			}
		}
		m_currentLabelCache[currentFrameTmp] = labelName;
	}

	return labelName;
}

const CurrentLabels Movie::GetCurrentLabels()
{
	CacheCurrentLabels();
	return m_currentLabelsCache;
}

}	// namespace LWF
