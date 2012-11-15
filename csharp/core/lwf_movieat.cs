/*
 * Copyright (C) 2012 GREE, Inc.
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

using System;
using System.Collections.Generic;

namespace LWF {

using Type = Format.Object.Type;
using MovieEventHandler = Action<Movie>;
using DetachHandler = Action<LWF>;
using AttachedMovies = Dictionary<string, Movie>;
using AttachedMovieList = List<Movie>;
using AttachedLWFs = Dictionary<string, LWFContainer>;
using AttachedLWFList = List<LWFContainer>;
using DetachDict = Dictionary<string, bool>;

public partial class Movie : IObject
{
	private void ShrinkAttachedMovieList()
	{
		for (int i = m_attachedMovieList.Count - 1; i >= 0; --i) {
			if (m_attachedMovieList[i] != null) {
				if (i != m_attachedMovieList.Count - 1)
					m_attachedMovieList.RemoveRange(i,
						m_attachedMovieList.Count - i);
				return;
			}
		}
		m_attachedMovieList.Clear();
	}

	private void ReorderAttachedMovieList(bool reorder, int index, Movie movie)
	{
		if (!reorder || index >= m_attachedMovieList.Count) {
			for (int i = m_attachedMovieList.Count; i < index; ++i)
				m_attachedMovieList.Add(null);
			m_attachedMovieList.Add(movie);
		} else {
			m_attachedMovieList.Insert(index, movie);
			if (reorder) {
				m_attachedMovieList.Remove(null);
				for (int i = 0; i < m_attachedMovieList.Count; ++i)
					m_attachedMovieList[i].depth = i;
			}
		}
	}

	private void DeleteAttachedMovie(Movie parent, Movie movie,
		bool destroy = true, bool deleteFromDetachedMovies = true)
	{
		string attachName = movie.attachName;
		int attachDepth = movie.depth;
		parent.m_attachedMovies.Remove(attachName);
		parent.m_attachedMovieList[attachDepth] = null;
		if (deleteFromDetachedMovies)
			parent.m_detachedMovies.Remove(attachName);
		parent.ShrinkAttachedMovieList();
		if (destroy)
			movie.Destroy();
	}

	public Movie AttachMovieInternal(Movie movie, string attachName,
		int attachDepth = -1, bool reorder = false)
	{
		if (m_attachedMovies == null) {
			m_attachedMovies = new AttachedMovies();
			m_detachedMovies = new DetachDict();
			m_attachedMovieList = new AttachedMovieList();
		}

		Movie attachedMovie;
		if (m_attachedMovies.TryGetValue(attachName, out attachedMovie))
			DeleteAttachedMovie(this, attachedMovie);

		if (!reorder && attachDepth >= 0 &&
				attachDepth <= m_attachedMovieList.Count - 1) {
			attachedMovie = m_attachedMovieList[attachDepth];
			if (attachedMovie != null)
				DeleteAttachedMovie(this, attachedMovie);
		}

		movie.m_attachName = attachName;
		movie.depth = attachDepth >= 0 ?
			attachDepth : m_attachedMovieList.Count;
		movie.m_name = attachName;
		m_attachedMovies[attachName] = movie;
		ReorderAttachedMovieList(reorder, movie.depth, movie);

		return movie;
	}

	public Movie AttachMovie(string linkageName, string attachName,
		int attachDepth = -1, bool reorder = false,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		int movieId = m_lwf.SearchMovieLinkage(m_lwf.GetStringId(linkageName));
		if (movieId == -1)
			return null;

		MovieEventHandlers handlers = new MovieEventHandlers();
		handlers.Add(load, postLoad, unload, enterFrame, update, render);
		Movie movie = new Movie(m_lwf, this, movieId, -1, 0, 0, true, handlers);

		return AttachMovieInternal(movie, attachName, attachDepth, reorder);
	}

	public Movie AttachMovie(Movie movie, string attachName,
		int attachDepth = -1, bool reorder = false,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		DeleteAttachedMovie(movie.parent, movie, false);

		MovieEventHandlers handlers = new MovieEventHandlers();
		handlers.Add(load, postLoad, unload, enterFrame, update, render);
		movie.SetHandlers(handlers);

		return AttachMovieInternal(movie, attachName, attachDepth, reorder);
	}

	public void SwapAttachedMovieDepth(int depth0, int depth1)
	{
		if (m_attachedMovies == null)
			return;

		int d = depth0;
		if (depth1 > d)
			d = depth1;
		for (int i = m_attachedMovieList.Count; i <= d; ++i)
			m_attachedMovieList.Add(null);

		Movie attachedMovie0 = m_attachedMovieList[depth0];
		Movie attachedMovie1 = m_attachedMovieList[depth1];
		if (attachedMovie0 != null)
			attachedMovie0.depth = depth1;
		if (attachedMovie1 != null)
			attachedMovie1.depth = depth0;
		m_attachedMovieList[depth0] = attachedMovie1;
		m_attachedMovieList[depth1] = attachedMovie0;
	}

	public Movie GetAttachedMovie(string attachName)
	{
		if (m_attachedMovies != null) {
			Movie movie;
			if (m_attachedMovies.TryGetValue(attachName, out movie))
				return movie;
		}
		return null;
	}

	public Movie GetAttachedMovie(int attachDepth)
	{
		if (m_attachedMovies != null) {
			if (attachDepth < 0 || attachDepth >= m_attachedMovieList.Count)
				return null;
			return m_attachedMovieList[attachDepth];
		}
		return null;
	}

	public Movie SearchAttachedMovie(string attachName, bool recursive = true)
	{
		Movie movie = GetAttachedMovie(attachName);
		if (movie != null)
			return movie;

		if (!recursive)
			return null;

		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsMovie()) {
				Movie i = ((Movie)instance).SearchAttachedMovie(
					attachName, recursive);
				if (i != null)
					return i;
			}
		}
		return null;
	}

	public void DetachMovie(string attachName)
	{
		if (m_detachedMovies != null)
			m_detachedMovies[attachName] = true;
	}

	public void DetachMovie(int attachDepth)
	{
		if (m_detachedMovies != null &&
				attachDepth >= 0 && attachDepth < m_attachedMovieList.Count &&
				m_attachedMovieList[attachDepth] != null)
			m_detachedMovies[
				m_attachedMovieList[attachDepth].attachName] = true;
	}

	public void DetachMovie(Movie movie)
	{
		if (m_detachedMovies != null &&
				movie != null && movie.attachName != null)
			m_detachedMovies[movie.attachName] = true;
	}

	public void DetachFromParent()
	{
		if (m_type != Type.ATTACHEDMOVIE)
			return;

		m_active = false;
		if (m_parent != null)
			m_parent.DetachMovie(this);
	}

	private void ShrinkAttachedLWFList()
	{
		for (int i = m_attachedLWFList.Count - 1; i >= 0; --i) {
			if (m_attachedLWFList[i] != null) {
				if (i != m_attachedLWFList.Count - 1)
					m_attachedLWFList.RemoveRange(i,
						m_attachedLWFList.Count - i);
				return;
			}
		}
		m_attachedLWFList.Clear();
	}

	private void ReorderAttachedLWFList(
		bool reorder, int index, LWFContainer lwfContainer)
	{
		if (!reorder || index >= m_attachedLWFList.Count) {
			for (int i = m_attachedLWFList.Count; i < index; ++i)
				m_attachedLWFList.Add(null);
			m_attachedLWFList.Add(lwfContainer);
		} else {
			m_attachedLWFList.Insert(index, lwfContainer);
			if (reorder) {
				m_attachedLWFList.Remove(null);
				for (int i = 0; i < m_attachedLWFList.Count; ++i)
					m_attachedLWFList[i].child.depth = i;
			}
		}
	}

	private void DeleteAttachedLWF(Movie parent, LWFContainer lwfContainer,
		bool destroy = true, bool deleteFromDetachedLWFs = true)
	{
		string attachName = lwfContainer.child.attachName;
		int attachDepth = lwfContainer.child.depth;
		parent.m_attachedLWFs.Remove(attachName);
		parent.m_attachedLWFList[attachDepth] = null;
		if (deleteFromDetachedLWFs)
			parent.m_detachedLWFs.Remove(attachName);
		parent.ShrinkAttachedLWFList();
		if (destroy && lwfContainer.child.detachHandler != null) {
			lwfContainer.child.detachHandler(lwfContainer.child);
			lwfContainer.child.parent = null;
			lwfContainer.child.detachHandler = null;
			lwfContainer.child.attachName = null;
			lwfContainer.child.depth = -1;
		}
	}

	public void AttachLWF(LWF attachLWF, string attachName,
		int attachDepth = -1, bool reorder = false,
		DetachHandler detachHandler = null)
	{
		if (m_attachedLWFs == null) {
			m_attachedLWFs = new AttachedLWFs();
			m_detachedLWFs = new DetachDict();
			m_attachedLWFList = new AttachedLWFList();
		}

		LWFContainer lwfContainer;
		if (attachLWF.parent != null) {
			attachLWF.parent.m_attachedLWFs.TryGetValue(
				attachLWF.attachName, out lwfContainer);
			DeleteAttachedLWF(attachLWF.parent, lwfContainer, false);
		} else {
			if (m_attachedLWFs.TryGetValue(attachName, out lwfContainer))
				DeleteAttachedLWF(this, lwfContainer);
		}

		if (!reorder && attachDepth >= 0 &&
				attachDepth <= m_attachedLWFList.Count - 1) {
			lwfContainer = m_attachedLWFList[attachDepth];
			if (lwfContainer != null)
				DeleteAttachedLWF(this, lwfContainer);
		}

		lwfContainer = new LWFContainer(this, attachLWF);

		if (attachLWF.interactive == true)
			m_lwf.interactive = true;
		attachLWF.parent = this;
		attachLWF.detachHandler = detachHandler;
		attachLWF.attachName = attachName;
		attachLWF.depth = attachDepth >= 0 ?
			attachDepth : m_attachedLWFList.Count;
		m_attachedLWFs[attachName] = lwfContainer;
		ReorderAttachedLWFList(reorder, attachLWF.depth, lwfContainer);

		m_lwf.isLWFAttached = true;
	}

	public void SwapAttachedLWFDepth(int depth0, int depth1)
	{
		if (m_attachedLWFs == null)
			return;

		int d = depth0;
		if (depth1 > d)
			d = depth1;
		for (int i = m_attachedLWFList.Count; i <= d; ++i)
			m_attachedLWFList.Add(null);

		LWFContainer attachedLWF0 = m_attachedLWFList[depth0];
		LWFContainer attachedLWF1 = m_attachedLWFList[depth1];
		if (attachedLWF0 != null)
			attachedLWF0.child.depth = depth1;
		if (attachedLWF1 != null)
			attachedLWF1.child.depth = depth0;
		m_attachedLWFList[depth0] = attachedLWF1;
		m_attachedLWFList[depth1] = attachedLWF0;
	}

	public LWF GetAttachedLWF(string attachName)
	{
		if (m_attachedLWFs != null) {
			LWFContainer lwfContainer;
			if (m_attachedLWFs.TryGetValue(attachName, out lwfContainer))
				return lwfContainer.child;
		}
		return null;
	}

	public LWF GetAttachedLWF(int attachDepth)
	{
		if (m_attachedLWFs != null) {
			if (attachDepth < 0 || attachDepth >= m_attachedLWFList.Count)
				return null;
			LWFContainer lwfContainer = m_attachedLWFList[attachDepth];
			return lwfContainer == null ? null : lwfContainer.child;
		}
		return null;
	}

	public LWF SearchAttachedLWF(string attachName, bool recursive = true)
	{
		LWF attachedLWF = GetAttachedLWF(attachName);
		if (attachedLWF != null)
			return attachedLWF;

		if (!recursive)
			return null;

		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsMovie()) {
				LWF i = ((Movie)instance).SearchAttachedLWF(
					attachName, recursive);
				if (i != null)
					return i;
			}
		}
		return null;
	}

	public void DetachLWF(string attachName)
	{
		if (m_detachedLWFs != null)
			m_detachedLWFs[attachName] = true;
	}

	public void DetachLWF(int attachDepth)
	{
		if (m_detachedLWFs != null &&
				attachDepth >= 0 && attachDepth < m_attachedLWFList.Count &&
				m_attachedLWFList[attachDepth] != null)
			m_detachedLWFs[
				m_attachedLWFList[attachDepth].child.attachName] = true;
	}

	public void DetachLWF(LWF detachLWF)
	{
		if (m_detachedLWFs != null &&
				detachLWF != null && detachLWF.attachName != null)
			m_detachedLWFs[detachLWF.attachName] = true;
	}

	public void DetachAllLWFs()
	{
		if (m_detachedLWFs != null)
			foreach (LWFContainer lwfContainer in m_attachedLWFs.Values)
				m_detachedLWFs[lwfContainer.child.attachName] = true;
	}
}

}	// namespace LWF
