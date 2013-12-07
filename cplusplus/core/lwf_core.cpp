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

#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_movie.h"
#include "lwf_property.h"
#include "lwf_renderer.h"
#include "lwf_utility.h"

namespace LWF {

int LWF::m_instanceOffset = 0;
int LWF::m_iObjectOffset = 0;
float LWF::ROUND_OFF_TICK_RATE = 0.05f;

LWF::LWF(shared_ptr<Data> d, shared_ptr<IRendererFactory> r, void *l)
	: data(d)
{
	property = make_shared<Property>(this);

	parent = 0;
	name = data->strings[data->header.nameStringId];

	frameRate = data->header.frameRate;
	execLimit = 3;
	renderingIndex = 0;
	renderingIndexOffsetted = 0;
	renderingCount = 0;
	depth = 0;
	execCount = 0;
	updateCount = 0;

	scaleByStage = 1.0f;
	tick = 1.0f / frameRate;
	thisTick = 0;
	height = data->header.height;
	width = data->header.width;
	pointX = FLT_MIN;
	pointY = FLT_MIN;
	interactive = !data->buttonConditions.empty();
	isExecDisabled = false;
	pressing = false;
	attachVisible = true;
	isPropertyDirty = false;
	isLWFAttached = false;
	interceptByNotAllowOrDenyButtons = true;
	intercepted = false;
	playing = true;
	alive = true;
	privateData = 0;
	instanceId = ++m_instanceOffset;
	luaState = l;
	char buff[32];
	snprintf(buff, sizeof(buff), "%d", instanceId);
	instanceIdString = buff;
#if defined(LWF_USE_LUA)
	InitLua();
#endif

	m_roundOffTick = tick * ROUND_OFF_TICK_RATE;
	m_executedForExecDisabled = false;
	m_eventOffset = 0;

	if (!interactive && data->frames.size() == 1)
		DisableExec();

	InitEvent();
	m_programObjectConstructors.resize(data->programObjects.size());

	Init();

	SetRendererFactory(r);
}

void LWF::SetRendererFactory(shared_ptr<IRendererFactory> r)
{
	rendererFactory = r;
	rendererFactory->Init(this);
}

void LWF::SetFrameRate(int f)
{
	if (f == 0)
		return;
	frameRate = f;
	tick = 1.0f / frameRate;
}

void LWF::SetPreferredFrameRate(int f, int eLimit)
{
	if (f == 0)
		return;
	execLimit = (int)ceilf(frameRate / (float)f) + eLimit;
}

void LWF::FitForHeight(float stageWidth, float stageHeight)
{
	rendererFactory->FitForHeight(this, stageWidth, stageHeight);
}

void LWF::FitForWidth(float stageWidth, float stageHeight)
{
	rendererFactory->FitForWidth(this, stageWidth, stageHeight);
}

void LWF::ScaleForHeight(float stageWidth, float stageHeight)
{
	rendererFactory->ScaleForHeight(this, stageWidth, stageHeight);
}

void LWF::ScaleForWidth(float stageWidth, float stageHeight)
{
	rendererFactory->ScaleForWidth(this, stageWidth, stageHeight);
}

void LWF::RenderOffset()
{
	renderingIndexOffsetted = 0;
}

void LWF::ClearRenderOffset()
{
	renderingIndexOffsetted = renderingIndex;
}

int LWF::RenderObject(int count)
{
	renderingIndex += count;
	renderingIndexOffsetted += count;
	return renderingIndex;
}

void LWF::SetAttachVisible(bool visible)
{
	attachVisible = visible;
}

void LWF::ClearFocus(Button *button)
{
	if (focus == button)
		focus = 0;
}

void LWF::ClearPressed(Button *button)
{
	if (pressed == button)
		pressed = 0;
}

void LWF::ClearIntercepted()
{
	intercepted = false;
}

void LWF::Init()
{
	time = 0;
	m_progress = 0;

	m_instances.clear();
	m_instances.resize(data->instanceNames.size());
	focus = 0;
	pressed = 0;
	buttonHead = 0;

	m_movieCommands.clear();

	m_rootMovieStringId = GetStringId("_root");
	if (rootMovie)
		rootMovie->Destroy();
	rootMovie = make_shared<Movie>(this, (Movie *)0,
		data->header.rootMovieId, SearchInstanceId(m_rootMovieStringId));
}

const Matrix *LWF::CalcMatrix(const Matrix *matrix)
{
	const Matrix *m;
	const shared_ptr<Property> &p = property;
	if (p->hasMatrix) {
		if (matrix) {
			m = Utility::CalcMatrix(&m_matrix, matrix, &p->matrix);
		} else {
			m = &p->matrix;
		}
	} else {
		m = !matrix ? &m_matrixIdentity : matrix;
	}
	return m;
}

const ColorTransform *LWF::CalcColorTransform(
	const ColorTransform *colorTransform)
{
	const ColorTransform *c;
	const shared_ptr<Property> &p = property;
	if (p->hasColorTransform) {
		if (colorTransform) {
			c = Utility::CalcColorTransform(
				&m_colorTransform, colorTransform, &p->colorTransform);
		} else {
			c = &p->colorTransform;
		}
	} else {
		c = !colorTransform ? &m_colorTransformIdentity : colorTransform;
	}
	return c;
}

int LWF::Exec(
	float t, const Matrix *matrix, const ColorTransform *colorTransform)
{
	if (!playing)
		return renderingCount;

	bool execed = false;
	float currentProgress = m_progress;
	thisTick = t;

	if (isExecDisabled/* TODO && tweens == 0 */) {
		if (!m_executedForExecDisabled) {
			++execCount;
			rootMovie->Exec();
			rootMovie->PostExec(true);
			m_executedForExecDisabled = true;
			execed = true;
		}
	} else {
		bool progressing = true;
		if (t == 0) {
			m_progress = tick;
		} else if (t < 0) {
			m_progress = tick;
			progressing = false;
		} else {
			if (time == 0) {
				time += (double)tick;
				m_progress += tick;
			} else {
				time += (double)t;
				m_progress += t;
			}
		}

		ExecHandlerList::iterator
			it(m_execHandlers.begin()), itend(m_execHandlers.end());
		for (; it != itend; ++it)
			it->second(this);

		int eLimit = execLimit;
		while (m_progress >= tick - m_roundOffTick) {
			if (--eLimit < 0) {
				m_progress = 0;
				break;
			}
			m_progress -= tick;
			++execCount;
			rootMovie->Exec();
			rootMovie->PostExec(progressing);
			execed = true;
		}

		if (m_progress < m_roundOffTick)
			m_progress = 0;
	}

	buttonHead = 0;
	if (interactive && rootMovie->hasButton)
		rootMovie->LinkButton();

	if (execed || isLWFAttached || isPropertyDirty || matrix || colorTransform)
		Update(matrix, colorTransform);

	if (!isExecDisabled) {
		if (t < 0)
			m_progress = currentProgress;
	}

	return renderingCount;
}

int LWF::ForceExec(const Matrix *matrix, const ColorTransform *colorTransform)
{
	return Exec(0, matrix, colorTransform);
}

int LWF::ForceExecWithoutProgress(
	const Matrix *matrix, const ColorTransform *colorTransform)
{
	return Exec(-1, matrix, colorTransform);
}

void LWF::Update(const Matrix *matrix, const ColorTransform *colorTransform)
{
	++updateCount;
	const Matrix *m = CalcMatrix(matrix);
	const ColorTransform *c = CalcColorTransform(colorTransform);
	renderingIndex = 0;
	renderingIndexOffsetted = 0;
	rootMovie->Update(m, c);
	renderingCount = renderingIndex;
	isPropertyDirty = false;
}

int LWF::Render(int rIndex, int rCount, int rOffset)
{
	int renderingCountBackup = renderingCount;
	if (rCount > 0)
		renderingCount = rCount;
	renderingIndex = rIndex;
	renderingIndexOffsetted = rIndex;
	if (property->hasRenderingOffset) {
		RenderOffset();
		rOffset = property->renderingOffset;
	}
	rendererFactory->BeginRender(this);
	rootMovie->Render(attachVisible, rOffset);
	rendererFactory->EndRender(this);
	renderingCount = renderingCountBackup;
	return renderingCount;
}

int LWF::Inspect(Inspector inspector,
	int hierarchy, int inspectDepth, int rIndex, int rCount, int rOffset)
{
	int renderingCountBackup = renderingCount;
	if (rCount > 0)
		renderingCount = rCount;
	renderingIndex = rIndex;
	renderingIndexOffsetted = rIndex;
	if (property->hasRenderingOffset) {
		RenderOffset();
		rOffset = property->renderingOffset;
	}

	rootMovie->Inspect(inspector, hierarchy, inspectDepth, rOffset);
	renderingCount = renderingCountBackup;
	return renderingCount;
}

void LWF::Destroy()
{
	rootMovie->Destroy();
#if defined(LWF_USE_LUA)
	DestroyLua();
#endif
	alive = false;
}

Movie *LWF::SearchMovieInstance(int stringId) const
{
	return SearchMovieInstanceByInstanceId(SearchInstanceId(stringId));
}

Movie *LWF::SearchMovieInstance(string instanceName) const
{
	size_t pos = instanceName.find(".");
	if (pos != string::npos) {
		vector<string> names = Utility::Split(instanceName, '.');
		if (names[0] != data->strings[m_rootMovieStringId])
			return 0;

		Movie *m = rootMovie.get();
		for (size_t i = 1; i < names.size(); ++i) {
			m = m->SearchMovieInstance(names[i], false);
			if (!m)
				return 0;
		}

		return m;
	}

	int stringId = GetStringId(instanceName);
	if (stringId == -1)
		return rootMovie->SearchMovieInstance(instanceName, true);

	return SearchMovieInstance(stringId);
}

Movie *LWF::operator[](string instanceName) const
{
	return SearchMovieInstance(instanceName);
}

Movie *LWF::SearchMovieInstanceByInstanceId(int instId) const
{
	if (instId < 0 || instId >= (int)m_instances.size())
		return 0;
	IObject *obj = m_instances[instId];
	while (obj) {
		if (obj->IsMovie())
			return (Movie *)obj;
		obj = obj->nextInstance;
	}
	return 0;
}

Button *LWF::SearchButtonInstance(int stringId) const
{
	return SearchButtonInstanceByInstanceId(SearchInstanceId(stringId));
}

Button *LWF::SearchButtonInstance(string instanceName) const
{
	size_t pos = instanceName.find(".");
	if (pos != string::npos) {
		vector<string> names = Utility::Split(instanceName, '.');
		if (names[0] != data->strings[m_rootMovieStringId])
			return 0;

		Movie *m = rootMovie.get();
		for (size_t i = 1; i < names.size(); ++i) {
			if (i == names.size() - 1) {
				return m->SearchButtonInstance(names[i], false);
			} else {
				m = m->SearchMovieInstance(names[i], false);
				if (!m)
					return 0;
			}
		}

		return 0;
	}

	int stringId = GetStringId(instanceName);
	if (stringId == -1)
		return rootMovie->SearchButtonInstance(instanceName, true);

	return SearchButtonInstance(stringId);
}

Button *LWF::SearchButtonInstanceByInstanceId(int instId) const
{
	if (instId < 0 || instId >= (int)m_instances.size())
		return 0;
	IObject *obj = m_instances[instId];
	while (obj) {
		if (obj->IsButton())
			return (Button *)obj;
		obj = obj->nextInstance;
	}
	return 0;
}

IObject *LWF::GetInstance(int instId) const
{
	return m_instances[instId];
}

void LWF::SetInstance(int instId, IObject *instance)
{
	m_instances[instId] = instance;
}

ProgramObjectConstructor LWF::GetProgramObjectConstructor(
	string programObjectName) const
{
	return GetProgramObjectConstructor(
		SearchProgramObjectId(programObjectName));
}

ProgramObjectConstructor LWF::GetProgramObjectConstructor(
	int programObjectId) const
{
	if (programObjectId < 0 ||
			programObjectId >= (int)data->programObjects.size())
		return 0;
	return m_programObjectConstructors[programObjectId];
}

void LWF::SetProgramObjectConstructor(string programObjectName,
	ProgramObjectConstructor programObjectConstructor)
{
	SetProgramObjectConstructor(
		SearchProgramObjectId(programObjectName), programObjectConstructor);
}

void LWF::SetProgramObjectConstructor(int programObjectId,
	ProgramObjectConstructor programObjectConstructor)
{
	if (programObjectId < 0 ||
			programObjectId >= (int)data->programObjects.size())
		return;
	m_programObjectConstructors[programObjectId] = programObjectConstructor;
}

void LWF::ExecMovieCommand()
{
	if (m_movieCommands.empty())
		return;

	vector<int> deletes;
	int i = 0;

	MovieCommands::iterator
		it(m_movieCommands.begin()), itend(m_movieCommands.end());
	for (; it != itend; ++it) {
		bool available = true;
		Movie *movie = rootMovie.get();

		vector<string>::iterator
			sit(it->first.begin()), sitend(it->first.end());
		for (; sit != sitend; ++sit) {
			movie = movie->SearchMovieInstance(*sit);
			if (!movie) {
				available = false;
				break;
			}
		}
		if (available) {
			it->second(movie);
			deletes.push_back(i);
		}
		++i;
	}
	for (vector<int>::reverse_iterator rit = deletes.rbegin();
			rit != deletes.rend(); ++rit) {
		it = m_movieCommands.begin();
		advance(it, *rit);
		m_movieCommands.erase(it);
	}
}

void LWF::SetMovieCommand(vector<string> instanceNames, MovieCommand cmd)
{
	m_movieCommands.push_back(make_pair(instanceNames, cmd));
	ExecMovieCommand();
}

bool LWF::AddAllowButton(string buttonName)
{
	int instId = SearchInstanceId(GetStringId(buttonName));
	if (instId < 0)
		return false;

	m_allowButtonList[instId] = true;
	return true;
}

bool LWF::RemoveAllowButton(string buttonName)
{
	int instId = SearchInstanceId(GetStringId(buttonName));
	if (instId < 0)
		return false;

	return m_allowButtonList.erase(instId) != 0;
}

void LWF::ClearAllowButton()
{
	m_allowButtonList.clear();
}

bool LWF::AddDenyButton(string buttonName)
{
	int instId = SearchInstanceId(GetStringId(buttonName));
	if (instId < 0)
		return false;

	m_denyButtonList[instId] = true;
	return true;
}

void LWF::DenyAllButtons()
{
	for (size_t instId = 0; instId < m_instances.size(); ++instId)
		m_denyButtonList[(int)instId] = true;
}

bool LWF::RemoveDenyButton(string buttonName)
{
	int instId = SearchInstanceId(GetStringId(buttonName));
	if (instId < 0)
		return false;

	return m_denyButtonList.erase(instId) != 0;
}

void LWF::ClearDenyButton()
{
	m_denyButtonList.clear();
}

void LWF::DisableExec()
{
	isExecDisabled = true;
	m_executedForExecDisabled = false;
}

void LWF::EnableExec()
{
	isExecDisabled = false;
}

void LWF::SetPropertyDirty()
{
	isPropertyDirty = true;
	if (parent)
		parent->lwf->SetPropertyDirty();
}

int LWF::AddExecHandler(ExecHandler execHandler)
{
	int id = GetEventOffset();
	m_execHandlers.push_back(make_pair(id, execHandler));
	return id;
}

class Pred
{
private:
	int id;
public:
	Pred(int i) : id(i) {}
	bool operator()(const pair<int, ExecHandler> &h)
	{
		return h.first == id;
	}
};

void LWF::RemoveExecHandler(int id)
{
	if (m_execHandlers.empty())
		return;
	remove_if(m_execHandlers.begin(), m_execHandlers.end(), Pred(id));
}

void LWF::ClearExecHandler()
{
	m_execHandlers.clear();
}

int LWF::SetExecHandler(ExecHandler execHandler)
{
	ClearExecHandler();
	return AddExecHandler(execHandler);
}

void LWF::SetText(string textName, string text)
{
	TextDictionary::iterator it = m_textDictionary.find(textName);
	if (it == m_textDictionary.end()) {
		m_textDictionary[textName] = make_pair(text, (TextRenderer *)0);
	} else {
		if (it->second.second != 0)
			it->second.second->SetText(text);
		it->second.first = text;
	}
}

string LWF::GetText(string textName)
{
	TextDictionary::iterator it = m_textDictionary.find(textName);
	if (it != m_textDictionary.end())
		return it->second.first;
	return string();
}

void LWF::SetTextRenderer(string fullPath,
	string textName, string text, TextRenderer *textRenderer)
{
	bool setText = false;
	string fullName = fullPath + "." + textName;
	TextDictionary::iterator it = m_textDictionary.find(fullName);
	if (it != m_textDictionary.end()) {
		it->second.second = textRenderer;
		if (!it->second.first.empty()) {
			textRenderer->SetText(it->second.first);
			setText = true;
		}
	} else {
		m_textDictionary[fullName] = make_pair(string(), textRenderer);
	}

	it = m_textDictionary.find(textName);
	if (it != m_textDictionary.end()) {
		it->second.second = textRenderer;
		if (!setText && !it->second.first.empty()) {
			textRenderer->SetText(it->second.first);
			setText = true;
		}
	} else {
		m_textDictionary[textName] = make_pair(string(), textRenderer);
	}

	if (!setText)
		textRenderer->SetText(text);
}

void LWF::ClearTextRenderer(string textName)
{
	TextDictionary::iterator it = m_textDictionary.find(textName);
	if (it != m_textDictionary.end())
		it->second.second = 0;
}

}	// namespace LWF
