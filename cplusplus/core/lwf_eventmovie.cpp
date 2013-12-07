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

#include "lwf_eventmovie.h"
#include "lwf_compat.h"
#include "lwf_core.h"
#include "lwf_movie.h"

namespace LWF {

#define EType MovieEventHandlers
typedef map<string, int> table_t;
static table_t table;

static void PrepareTable()
{
	if (table.empty()) {
		const char *names[] = {
			"load",
			"postLoad",
			"unload",
			"enterFrame",
			"update",
			"render",
			0,
		};
		const int vals[] = {
			EType::LOAD,
			EType::POSTLOAD,
			EType::UNLOAD,
			EType::ENTERFRAME,
			EType::UPDATE,
			EType::RENDER,
		};

		for (int i = 0; names[i]; ++i)
			table[names[i]] = vals[i];
	}
};

void MovieEventHandlers::Clear()
{
	for (int i = 0; i < EVENTS; ++i)
		m_handlers[i].clear();
	m_empty = true;
}

void MovieEventHandlers::Clear(string type)
{
	PrepareTable();
	const table_t::iterator it = table.find(type);
	if (it == table.end())
		return;

	m_handlers[it->second].clear();
	UpdateEmpty();
}

void MovieEventHandlers::Add(const MovieEventHandlers *h)
{
	if (!h)
		return;

	for (int i = 0; i < EVENTS; ++i)
		m_handlers[i].insert(m_handlers[i].end(),
			h->m_handlers[i].begin(), h->m_handlers[i].end());

	if (m_empty)
		m_empty = h->Empty();
}

void MovieEventHandlers::Add(int eventId, const MovieEventHandlerDictionary &h)
{
	MovieEventHandlerDictionary::const_iterator it(h.begin()), itend(h.end());
	PrepareTable();
	table_t::const_iterator titend(table.end());
	for (; it != itend; ++it) {
		table_t::const_iterator tit(table.begin());
		for (; tit != titend; ++tit) {
			if (it->first == tit->first) {
				m_handlers[tit->second].push_back(
					make_pair(eventId, it->second));
			}
		}
	}
	if (m_empty)
		UpdateEmpty();
}

bool MovieEventHandlers::Add(
	int eventId, string type, const MovieEventHandler &h)
{
	PrepareTable();
	const table_t::iterator it = table.find(type);
	if (it == table.end())
		return false;

	MovieEventHandlerDictionary handlers;
	handlers[type] = h;
	Add(eventId, handlers);
	return true;
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

void MovieEventHandlers::Remove(int id)
{
	if (id < 0)
		return;

	for (int i = 0; i < EVENTS; ++i)
		remove_if(m_handlers[i].begin(), m_handlers[i].end(), Pred(id));

	UpdateEmpty();
}

class Exec
{
private:
	Movie *target;
public:
	Exec(Movie *t) : target(t) {}
	void operator()(const pair<int, MovieEventHandler> &h)
	{
		h.second(target);
	}
};

void MovieEventHandlers::Call(Type type, Movie *target)
{
	scoped_ptr<MovieEventHandlerList>
		p(new MovieEventHandlerList(m_handlers[type]));
	for_each(p->begin(), p->end(), Exec(target));
}

bool MovieEventHandlers::Call(string type, Movie *target)
{
	PrepareTable();
	const table_t::iterator it = table.find(type);
	if (it == table.end())
		return false;
	Call((Type)it->second, target);
	return true;
}

void MovieEventHandlers::UpdateEmpty()
{
	m_empty = true;
	for (int i = 0; i < EVENTS; ++i) {
		if (!m_handlers[i].empty()) {
			m_empty = false;
			break;
		}
	}
}

class LoadHandlerWrapper
{
public:
	string instanceName;
	int handlerId;
	MovieEventHandler handler;
	LoadHandlerWrapper(string i, MovieEventHandler h)
		: instanceName(i), handlerId(-1), handler(h) {}
	void operator()(Movie *m)
	{
		m->lwf->RemoveMovieEventHandler(instanceName, handlerId);
		handler(m);
	}
};

void LWF::SetMovieLoadCommand(string instanceName, MovieEventHandler handler)
{
	Movie *movie = SearchMovieInstance(instanceName);
	if (movie) {
		handler(movie);
	} else {
		LoadHandlerWrapper h(instanceName, handler);
		MovieEventHandlerDictionary d;
		d["load"] = h;
		h.handlerId = AddMovieEventHandler(instanceName, d);
	}
}

void LWF::SetMoviePostLoadCommand(string instanceName, MovieEventHandler handler)
{
	Movie *movie = SearchMovieInstance(instanceName);
	if (movie) {
		handler(movie);
	} else {
		LoadHandlerWrapper h(instanceName, handler);
		MovieEventHandlerDictionary d;
		d["postLoad"] = h;
		h.handlerId = AddMovieEventHandler(instanceName, d);
	}
}

class PlayWrapper
{
public:
	void operator()(Movie *m)
	{
		m->Play();
	}
};

void LWF::PlayMovie(string instanceName)
{
	SetMovieLoadCommand(instanceName, PlayWrapper());
}

class StopWrapper
{
public:
	void operator()(Movie *m)
	{
		m->Stop();
	}
};

void LWF::StopMovie(string instanceName)
{
	SetMovieLoadCommand(instanceName, StopWrapper());
}

class NextFrameWrapper
{
public:
	void operator()(Movie *m)
	{
		m->NextFrame();
	}
};

void LWF::NextFrameMovie(string instanceName)
{
	SetMovieLoadCommand(instanceName, NextFrameWrapper());
}

class PrevFrameWrapper
{
public:
	void operator()(Movie *m)
	{
		m->PrevFrame();
	}
};

void LWF::PrevFrameMovie(string instanceName)
{
	SetMovieLoadCommand(instanceName, PrevFrameWrapper());
}

class SetVisibleWrapper
{
public:
	bool visible;
	SetVisibleWrapper(bool v) : visible(v) {}
	void operator()(Movie *m)
	{
		m->SetVisible(visible);
	}
};

void LWF::SetVisibleMovie(string instanceName, bool visible)
{
	SetMovieLoadCommand(instanceName, SetVisibleWrapper(visible));
}

template<typename T> class GotoAndStopWrapper
{
public:
	T target;
	GotoAndStopWrapper(T t) : target(t) {}
	void operator()(Movie *m)
	{
		m->GotoAndStop(target);
	}
};

void LWF::GotoAndStopMovie(string instanceName, string label)
{
	SetMovieLoadCommand(instanceName, GotoAndStopWrapper<string>(label));
}

void LWF::GotoAndStopMovie(string instanceName, int frameNo)
{
	SetMovieLoadCommand(instanceName, GotoAndStopWrapper<int>(frameNo));
}

template<typename T> class GotoAndPlayWrapper
{
public:
	T target;
	GotoAndPlayWrapper(T t) : target(t) {}
	void operator()(Movie *m)
	{
		m->GotoAndPlay(target);
	}
};

void LWF::GotoAndPlayMovie(string instanceName, string label)
{
	SetMovieLoadCommand(instanceName, GotoAndPlayWrapper<string>(label));
}

void LWF::GotoAndPlayMovie(string instanceName, int frameNo)
{
	SetMovieLoadCommand(instanceName, GotoAndPlayWrapper<int>(frameNo));
}

class MoveWrapper
{
public:
	float vx;
	float vy;
	MoveWrapper(float x, float y) : vx(x), vy(y) {}
	void operator()(Movie *m)
	{
		m->Move(vx, vy);
	}
};

void LWF::MoveMovie(string instanceName, float vx, float vy)
{
	SetMovieLoadCommand(instanceName, MoveWrapper(vx, vy));
}

class MoveToWrapper
{
public:
	float vx;
	float vy;
	MoveToWrapper(float x, float y) : vx(x), vy(y) {}
	void operator()(Movie *m)
	{
		m->MoveTo(vx, vy);
	}
};

void LWF::MoveToMovie(string instanceName, float vx, float vy)
{
	SetMovieLoadCommand(instanceName, MoveToWrapper(vx, vy));
}

class RotateWrapper
{
public:
	float degree;
	RotateWrapper(float d) : degree(d) {}
	void operator()(Movie *m)
	{
		m->Rotate(degree);
	}
};

void LWF::RotateMovie(string instanceName, float degree)
{
	SetMovieLoadCommand(instanceName, RotateWrapper(degree));
}

class RotateToWrapper
{
public:
	float degree;
	RotateToWrapper(float d) : degree(d) {}
	void operator()(Movie *m)
	{
		m->RotateTo(degree);
	}
};

void LWF::RotateToMovie(string instanceName, float degree)
{
	SetMovieLoadCommand(instanceName, RotateToWrapper(degree));
}

class ScaleWrapper
{
public:
	float vx;
	float vy;
	ScaleWrapper(float x, float y) : vx(x), vy(y) {}
	void operator()(Movie *m)
	{
		m->Scale(vx, vy);
	}
};

void LWF::ScaleMovie(string instanceName, float vx, float vy)
{
	SetMovieLoadCommand(instanceName, ScaleWrapper(vx, vy));
}

class ScaleToWrapper
{
public:
	float vx;
	float vy;
	ScaleToWrapper(float x, float y) : vx(x), vy(y) {}
	void operator()(Movie *m)
	{
		m->ScaleTo(vx, vy);
	}
};

void LWF::ScaleToMovie(string instanceName, float vx, float vy)
{
	SetMovieLoadCommand(instanceName, ScaleToWrapper(vx, vy));
}

class SetMatrixWrapper
{
public:
	const Matrix matrix;
	float sx;
	float sy;
	float r;
	SetMatrixWrapper(const Matrix *m, float x, float y, float r_)
		: matrix(*m), sx(x), sy(y), r(r_) {}
	void operator()(Movie *m)
	{
		m->SetMatrix(&matrix, sx, sy, r);
	}
};

void LWF::SetMatrixMovie(string instanceName, const Matrix *matrix,
	float sx, float sy, float r)
{
	SetMovieLoadCommand(instanceName, SetMatrixWrapper(matrix, sx, sy, r));
}

class SetAlphaWrapper
{
public:
	float alpha;
	SetAlphaWrapper(float a) : alpha(a) {}
	void operator()(Movie *m)
	{
		m->SetAlpha(alpha);
	}
};

void LWF::SetAlphaMovie(string instanceName, float v)
{
	SetMovieLoadCommand(instanceName, SetAlphaWrapper(v));
}

class SetColorTransformWrapper
{
public:
	const ColorTransform colorTransform;
	SetColorTransformWrapper(const ColorTransform *c) : colorTransform(*c) {}
	void operator()(Movie *m)
	{
		m->SetColorTransform(&colorTransform);
	}
};

void LWF::SetColorTransformMovie(string instanceName, const ColorTransform *c)
{
	SetMovieLoadCommand(instanceName, SetColorTransformWrapper(c));
}

#if defined(LWF_USE_LUA)
void LWF::SetColorTransformMovieLua(
	string instanceName, float vr, float vg, float vb, float va)
{
	ColorTransform colorTransform(vr, vg, vb, va);
	SetColorTransformMovie(instanceName, &colorTransform);
}
#endif

}	// namespace LWF
