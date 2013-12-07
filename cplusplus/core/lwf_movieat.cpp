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
#include "lwf_movie.h"
#include "lwf_lwfcontainer.h"

namespace LWF {

void Movie::ReorderAttachedMovieList(
	bool reorder, int index, shared_ptr<Movie> movie)
{
	m_attachedMovieList[index] = movie;
	if (reorder) {
		AttachedMovieList list(m_attachedMovieList);
		m_attachedMovieList.clear();
		int i = 0;
		AttachedMovieList::iterator it(list.begin()), itend(list.end());
		for (; it != itend; ++it) {
			it->second->depth = i;
			m_attachedMovieList[i] = it->second;
			++i;
		}
	}
}

void Movie::DeleteAttachedMovie(Movie *p,
	shared_ptr<Movie> movie, bool destroy, bool deleteFromDetachedMovies)
{
	string aName = movie->attachName;
	int aDepth = movie->depth;
	p->m_attachedMovies.erase(aName);
	p->m_attachedMovieList.erase(aDepth);
	if (deleteFromDetachedMovies)
		p->m_detachedMovies.erase(aName);
	if (destroy)
		movie->Destroy();
}

Movie *Movie::AttachMovie(string linkageName, string aName,
	const MovieEventHandlerDictionary &h, int aDepth, bool reorder)
{
	int movieId = lwf->SearchMovieLinkage(lwf->GetStringId(linkageName));
	if (movieId == -1)
		return 0;

	MovieEventHandlers handlers;
	handlers.Add(lwf->GetEventOffset(), h);
	shared_ptr<Movie> movie = make_shared<Movie>(
		lwf, this, movieId, -1, 0, 0, true, &handlers, aName);
	if (m_attachMovieExeced)
		movie->Exec();
	if (m_attachMoviePostExeced)
		movie->PostExec(true);

	AttachedMovies::iterator it = m_attachedMovies.find(aName);
	if (it != m_attachedMovies.end())
		DeleteAttachedMovie(this, it->second);

	if (!reorder && aDepth >= 0) {
		AttachedMovieList::iterator lit = m_attachedMovieList.find(aDepth);
		if (lit != m_attachedMovieList.end())
			DeleteAttachedMovie(this, lit->second);
	}

	if (aDepth < 0) {
		if (m_attachedMovieList.empty())
			aDepth = 0;
		else
			aDepth = m_attachedMovieList.rbegin()->first + 1;
	}

	movie->attachName = aName;
	movie->depth = aDepth;
	m_attachedMovies[aName] = movie;
	ReorderAttachedMovieList(reorder, movie->depth, movie);

	return movie.get();
}

Movie *Movie::AttachMovie(
	string linkageName, string aName, int aDepth, bool reorder)
{
	MovieEventHandlerDictionary h;
	return AttachMovie(linkageName, aName, h, aDepth, reorder);
}

void Movie::SwapAttachedMovieDepth(int depth0, int depth1)
{
	if (m_attachedMovies.empty())
		return;

	shared_ptr<Movie> attachedMovie0;
	shared_ptr<Movie> attachedMovie1;
	AttachedMovieList::iterator it0 = m_attachedMovieList.find(depth0);
	AttachedMovieList::iterator it1 = m_attachedMovieList.find(depth1);
	if (it0 != m_attachedMovieList.end())
		attachedMovie0 = it0->second;
	if (it1 != m_attachedMovieList.end())
		attachedMovie1 = it1->second;
	if (attachedMovie0) {
		attachedMovie0->depth = depth1;
		m_attachedMovieList[depth1] = attachedMovie0;
	} else {
		m_attachedMovieList.erase(depth1);
	}
	if (attachedMovie1) {
		attachedMovie1->depth = depth0;
		m_attachedMovieList[depth0] = attachedMovie1;
	} else {
		m_attachedMovieList.erase(depth0);
	}
}

void Movie::DetachMovie(string aName)
{
	m_detachedMovies[aName] = true;
}

void Movie::DetachMovie(int aDepth)
{
	AttachedMovieList::iterator it = m_attachedMovieList.find(aDepth);
	if (it != m_attachedMovieList.end())
		m_detachedMovies[it->second->attachName] = true;
}

void Movie::DetachMovie(Movie *movie)
{
	if (movie && !movie->attachName.empty())
		m_detachedMovies[movie->attachName] = true;
}

void Movie::DetachFromParent()
{
	if (type != Format::Object::PROGRAMOBJECT)
		return;

	active = false;
	if (parent)
		parent->DetachMovie(attachName);
}

void Movie::ReorderAttachedLWFList(
	bool reorder, int index, shared_ptr<LWFContainer> lwfContainer)
{
	m_attachedLWFList[index] = lwfContainer;
	if (reorder) {
		AttachedLWFList list(m_attachedLWFList);
		m_attachedLWFList.clear();
		int i = 0;
		AttachedLWFList::iterator it(list.begin()), itend(list.end());
		for (; it != itend; ++it) {
			it->second->child->depth = i;
			m_attachedLWFList[i] = it->second;
			++i;
		}
	}
}

void Movie::DeleteAttachedLWF(Movie *p, shared_ptr<LWFContainer> lwfContainer,
	bool destroy, bool deleteFromDetachedLWFs)
{
	string aName = lwfContainer->child->attachName;
	int aDepth = lwfContainer->child->depth;
	p->m_attachedLWFs.erase(aName);
	p->m_attachedLWFList.erase(aDepth);
	if (deleteFromDetachedLWFs)
		p->m_detachedLWFs.erase(aName);
	if (destroy) {
		LWF *l = lwfContainer->child.get();
		if (l->detachHandler) {
			if (l->detachHandler(l))
				l->Destroy();
		} else {
			l->Destroy();
		}
		l->parent = 0;
		l->detachHandler = 0;
		l->attachName.clear();
		l->depth = -1;
		lwfContainer->Destroy();
	}
}

void Movie::AttachLWF(shared_ptr<LWF> attachLWF, string aName,
	DetachHandler detachHandler, int aDepth, bool reorder)
{
	AttachedLWFs::iterator it;
	if (attachLWF->parent) {
		it = attachLWF->parent->m_attachedLWFs.find(attachLWF->attachName);
		if (it != attachLWF->parent->m_attachedLWFs.end())
			DeleteAttachedLWF(attachLWF->parent, it->second, false);
	}

	it = m_attachedLWFs.find(aName);
	if (it != m_attachedLWFs.end())
		DeleteAttachedLWF(this, it->second);

	if (!reorder && aDepth >= 0) {
		AttachedLWFList::iterator lit = m_attachedLWFList.find(aDepth);
		if (lit != m_attachedLWFList.end())
			DeleteAttachedLWF(this, lit->second);
	}

	shared_ptr<LWFContainer> lwfContainer =
		make_shared<LWFContainer>(this, attachLWF);

	if (attachLWF->interactive == true)
		lwf->interactive = true;

	if (aDepth < 0) {
		if (m_attachedLWFList.empty())
			aDepth = 0;
		else
			aDepth = m_attachedLWFList.rbegin()->first + 1;
	}

	attachLWF->parent = this;
	attachLWF->scaleByStage = lwf->scaleByStage;
	attachLWF->detachHandler = detachHandler;
	attachLWF->attachName = aName;
	attachLWF->depth = aDepth;
	m_attachedLWFs[aName] = lwfContainer;
	ReorderAttachedLWFList(reorder, attachLWF->depth, lwfContainer);

	lwf->isLWFAttached = true;
}

void Movie::AttachLWF(shared_ptr<LWF> attachLWF,
	string aName, int aDepth, bool reorder)
{
	DetachHandler detachHandler;
	AttachLWF(attachLWF, aName, detachHandler, aDepth, reorder);
}

void Movie::SwapAttachedLWFDepth(int depth0, int depth1)
{
	if (m_attachedLWFs.empty())
		return;

	shared_ptr<LWFContainer> attachedLWF0;
	shared_ptr<LWFContainer> attachedLWF1;
	AttachedLWFList::iterator it0 = m_attachedLWFList.find(depth0);
	AttachedLWFList::iterator it1 = m_attachedLWFList.find(depth1);
	if (it0 != m_attachedLWFList.end())
		attachedLWF0 = it0->second;
	if (it1 != m_attachedLWFList.end())
		attachedLWF1 = it1->second;
	if (attachedLWF0) {
		attachedLWF0->child->depth = depth1;
		m_attachedLWFList[depth1] = attachedLWF0;
	} else {
		m_attachedLWFList.erase(depth1);
	}
	if (attachedLWF1) {
		attachedLWF1->child->depth = depth0;
		m_attachedLWFList[depth0] = attachedLWF1;
	} else {
		m_attachedLWFList.erase(depth0);
	}
}

void Movie::DetachLWF(string aName)
{
	m_detachedLWFs[aName] = true;
}

void Movie::DetachLWF(int aDepth)
{
	AttachedLWFList::const_iterator it = m_attachedLWFList.find(aDepth);
	if (it != m_attachedLWFList.end())
		m_detachedLWFs[it->second->child->attachName] = true;
}

void Movie::DetachLWF(shared_ptr<LWF> detachLWF)
{
	if (detachLWF.get() && !detachLWF->attachName.empty())
		m_detachedLWFs[detachLWF->attachName] = true;
}

void Movie::DetachAllLWFs()
{
	AttachedLWFs::const_iterator
		it(m_attachedLWFs.begin()), itend(m_attachedLWFs.end());
	for (; it != itend; ++it)
		m_detachedLWFs[it->second->child->attachName] = true;
}

}	// namespace LWF
