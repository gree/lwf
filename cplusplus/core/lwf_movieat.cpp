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

Movie *Movie::AttachEmptyMovie(string aName,
	const MovieEventHandlerDictionary &h, int aDepth, bool reorder)
{
	return AttachMovie("_empty", aName, h, aDepth, reorder);
}

Movie *Movie::AttachEmptyMovie(string aName, int aDepth, bool reorder)
{
	MovieEventHandlerDictionary h;
	return AttachEmptyMovie(aName, h, aDepth, reorder);
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

Movie *Movie::GetAttachedMovie(string aName) const
{
	AttachedMovies::const_iterator it = m_attachedMovies.find(aName);
	if (it == m_attachedMovies.end())
		return nullptr;
	return it->second.get();
}

Movie *Movie::GetAttachedMovie(int aDepth) const
{
	AttachedMovieList::const_iterator it = m_attachedMovieList.find(aDepth);
	if (it == m_attachedMovieList.end())
		return nullptr;
	return it->second.get();
}

Movie *Movie::SearchAttachedMovie(string aName, bool recursive) const
{
	Movie *movie = GetAttachedMovie(aName);
	if (movie)
		return movie;

	if (!recursive)
		return nullptr;

	for (const IObject *instance = m_instanceHead;
			instance != nullptr; instance = instance->linkInstance) {
		if (instance->IsMovie()) {
			Movie *i =
				((Movie *)instance)->SearchAttachedMovie(aName, recursive);
			if (i)
				return i;
		}
	}

	if (!m_attachedMovies.empty()) {
		AttachedMovieList::const_iterator
			it(m_attachedMovieList.begin()), itend(m_attachedMovieList.end());
		for (; it != itend; ++it) {
			Movie *movie = it->second->SearchAttachedMovie(aName, recursive);
			if (movie)
				return movie;
		}
	}

	if (!m_attachedLWFs.empty()) {
		AttachedLWFList::const_iterator
			it(m_attachedLWFList.begin()), itend(m_attachedLWFList.end());
		for (; it != itend; ++it) {
			LWF *child = it->second->child.get();
			if (child->attachName == aName) {
				return child->rootMovie.get();
			} else {
				Movie *movie =
					child->rootMovie->SearchAttachedMovie(aName, recursive);
				if (movie)
					return movie;
			}
		}
	}

	return nullptr;
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
	if (type != OType::ATTACHEDMOVIE)
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
		l->_root = 0;
		l->detachHandler = nullptr;
		l->attachName.clear();
		l->depth = -1;
		lwfContainer->Destroy();
	}
}

shared_ptr<LWF> Movie::AttachLWF(
	string path, string aName, int aDepth, bool reorder)
{
	if (!lwf->lwfLoader)
		return shared_ptr<LWF>();

	shared_ptr<LWF> child = lwf->lwfLoader(path);
	if (!child)
		return child;

	AttachLWF(child, aName, aDepth, reorder);

	return child;
}

void Movie::AttachLWF(shared_ptr<LWF> child, string aName,
	DetachHandler detachHandler, int aDepth, bool reorder)
{
	AttachedLWFs::iterator it;
	if (child->parent) {
		it = child->parent->m_attachedLWFs.find(child->attachName);
		if (it != child->parent->m_attachedLWFs.end())
			DeleteAttachedLWF(child->parent, it->second, false);
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
		make_shared<LWFContainer>(this, child);

	if (child->interactive == true)
		lwf->interactive = true;

	if (aDepth < 0) {
		if (m_attachedLWFList.empty())
			aDepth = 0;
		else
			aDepth = m_attachedLWFList.rbegin()->first + 1;
	}

	child->parent = this;
	child->_root = lwf->_root;
	child->scaleByStage = lwf->scaleByStage;
	child->detachHandler = detachHandler;
	child->attachName = aName;
	child->depth = aDepth;
	m_attachedLWFs[aName] = lwfContainer;
	ReorderAttachedLWFList(reorder, child->depth, lwfContainer);

	lwf->SetLWFAttached();
}

void Movie::AttachLWF(shared_ptr<LWF> child,
	string aName, int aDepth, bool reorder)
{
	DetachHandler detachHandler;
	AttachLWF(child, aName, detachHandler, aDepth, reorder);
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

shared_ptr<LWF> Movie::GetAttachedLWF(string aName) const
{
	AttachedLWFs::const_iterator it = m_attachedLWFs.find(aName);
	if (it == m_attachedLWFs.end())
		return shared_ptr<LWF>();
	return it->second->child;
}

shared_ptr<LWF> Movie::GetAttachedLWF(int aDepth) const
{
	AttachedLWFList::const_iterator it = m_attachedLWFList.find(aDepth);
	if (it == m_attachedLWFList.end())
		return shared_ptr<LWF>();
	return it->second->child;
}

shared_ptr<LWF> Movie::SearchAttachedLWF(string aName, bool recursive) const
{
	shared_ptr<LWF> attachedLWF = GetAttachedLWF(aName);
	if (attachedLWF)
		return attachedLWF;

	if (!recursive)
		return shared_ptr<LWF>();

	for (const IObject *instance = m_instanceHead;
			instance != nullptr; instance = instance->linkInstance) {
		if (instance->IsMovie()) {
			attachedLWF =
				((Movie *)instance)->SearchAttachedLWF(aName, recursive);
			if (attachedLWF)
				return attachedLWF;
		}
	}
	return shared_ptr<LWF>();
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

void Movie::RemoveMovieClip()
{
	if (type == OType::ATTACHEDMOVIE) {
		DetachFromParent();
	} else if (!lwf->attachName.empty() && lwf->parent) {
		lwf->parent->DetachLWF(lwf->attachName);
	}
}

shared_ptr<BitmapClip> Movie::AttachBitmap(string linkageName, int aDepth)
{
	map<string, int>::iterator it = lwf->data->bitmapMap.find(linkageName);
	if (it == lwf->data->bitmapMap.end())
		return shared_ptr<BitmapClip>();

	shared_ptr<BitmapClip> bitmapClip =
		make_shared<BitmapClip>(lwf, this, it->second);

	DetachBitmap(aDepth);
	m_bitmapClips[aDepth] = bitmapClip;
	bitmapClip->depth = aDepth;
	bitmapClip->name = linkageName;

	return bitmapClip;
}

BitmapClips Movie::GetAttachedBitmaps()
{
	return m_bitmapClips;
}

shared_ptr<BitmapClip> Movie::GetAttachedBitmap(int aDepth)
{
	BitmapClips::iterator it = m_bitmapClips.find(aDepth);
	if (it == m_bitmapClips.end())
		return shared_ptr<BitmapClip>();
	return it->second;
}

void Movie::SwapAttachedBitmapDepth(int depth0, int depth1)
{
	if (m_bitmapClips.empty())
		return;

	shared_ptr<BitmapClip> attachedBitmapClip0;
	shared_ptr<BitmapClip> attachedBitmapClip1;
	BitmapClips::iterator it0 = m_bitmapClips.find(depth0);
	BitmapClips::iterator it1 = m_bitmapClips.find(depth1);
	if (it0 != m_bitmapClips.end())
		attachedBitmapClip0 = it0->second;
	if (it1 != m_bitmapClips.end())
		attachedBitmapClip1 = it1->second;
	if (attachedBitmapClip0) {
		attachedBitmapClip0->depth = depth1;
		m_bitmapClips[depth1] = attachedBitmapClip0;
	} else {
		m_bitmapClips.erase(depth1);
	}
	if (attachedBitmapClip1) {
		attachedBitmapClip1->depth = depth0;
		m_bitmapClips[depth0] = attachedBitmapClip1;
	} else {
		m_bitmapClips.erase(depth0);
	}
}

void Movie::DetachBitmap(int aDepth)
{
	BitmapClips::iterator it = m_bitmapClips.find(aDepth);
	if (it == m_bitmapClips.end())
		return;
	it->second->Destroy();
	m_bitmapClips.erase(aDepth);
}

}	// namespace LWF
