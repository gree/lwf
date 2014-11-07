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
using AttachedMovieList = SortedDictionary<int, Movie>;
using AttachedMovieDescendingList = SortedDictionary<int, int>;
using AttachedLWFs = Dictionary<string, LWFContainer>;
using AttachedLWFList = SortedDictionary<int, LWFContainer>;
using AttachedLWFDescendingList = SortedDictionary<int, int>;
using DetachDict = Dictionary<string, bool>;
using BitmapClips = SortedDictionary<int, BitmapClip>;

class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
{
	public int Compare(T x, T y)
	{
		return y.CompareTo(x);
	}
}

public partial class Movie : IObject
{
	private void ReorderAttachedMovieList(bool reorder, int index, Movie movie)
	{
		m_attachedMovieList = new AttachedMovieList(m_attachedMovieList);
		m_attachedMovieList[index] = movie;
		m_attachedMovieDescendingList[index] = index;
		if (reorder) {
			AttachedMovieList list = m_attachedMovieList;
			m_attachedMovieList = new AttachedMovieList();
			m_attachedMovieDescendingList =
				new AttachedMovieDescendingList(new DescendingComparer<int>());
			int i = 0;
			foreach (Movie m in list.Values) {
				m.depth = i;
				m_attachedMovieList[i] = m;
				m_attachedMovieDescendingList[i] = i;
				++i;
			}
		}
	}

	private void DeleteAttachedMovie(Movie parent, Movie movie,
		bool destroy = true, bool deleteFromDetachedMovies = true)
	{
		string attachName = movie.attachName;
		int attachDepth = movie.depth;
		parent.m_attachedMovies.Remove(attachName);
		parent.m_attachedMovieList.Remove(attachDepth);
		parent.m_attachedMovieDescendingList.Remove(attachDepth);
		if (deleteFromDetachedMovies)
			parent.m_detachedMovies.Remove(attachName);
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
			m_attachedMovieDescendingList =
				new AttachedMovieDescendingList(new DescendingComparer<int>());
		}

		Movie attachedMovie;
		if (m_attachedMovies.TryGetValue(attachName, out attachedMovie))
			DeleteAttachedMovie(this, attachedMovie);

		if (!reorder && attachDepth >= 0)
			if (m_attachedMovieList.TryGetValue(attachDepth, out attachedMovie))
				DeleteAttachedMovie(this, attachedMovie);

		movie.m_attachName = attachName;
		if (attachDepth >= 0) {
			movie.depth = attachDepth;
		} else {
			AttachedMovieDescendingList.KeyCollection.Enumerator e =
				m_attachedMovieDescendingList.Keys.GetEnumerator();
			if (e.MoveNext())
				movie.depth = e.Current + 1;
			else
				movie.depth = 0;
		}
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
		handlers.Add(m_lwf.GetEventOffset(),
			load, postLoad, unload, enterFrame, update, render);
		Movie movie = new Movie(m_lwf,
			this, movieId, -1, 0, 0, true, handlers, attachName);
		if (m_attachMovieExeced)
			movie.Exec();
		if (m_attachMoviePostExeced)
			movie.PostExec(true);

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
		handlers.Add(m_lwf.GetEventOffset(),
			load, postLoad, unload, enterFrame, update, render);
		movie.SetHandlers(handlers);

		movie.m_name = attachName;
		return AttachMovieInternal(movie, attachName, attachDepth, reorder);
	}

	public Movie AttachEmptyMovie(string attachName,
		int attachDepth = -1, bool reorder = false,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		return AttachMovie("_empty", attachName, attachDepth, reorder,
			load, postLoad, unload, enterFrame, update, render);
	}

	public void SwapAttachedMovieDepth(int depth0, int depth1)
	{
		if (m_attachedMovies == null)
			return;

		Movie attachedMovie0;
		m_attachedMovieList.TryGetValue(depth0, out attachedMovie0);
		Movie attachedMovie1;
		m_attachedMovieList.TryGetValue(depth1, out attachedMovie1);
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
			Movie movie;
			if (m_attachedMovieList.TryGetValue(attachDepth, out movie))
				return movie;
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
		if (m_detachedMovies != null) {
			Movie movie;
			if (m_attachedMovieList.TryGetValue(attachDepth, out movie))
				m_detachedMovies[movie.attachName] = true;
		}
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

	private void ReorderAttachedLWFList(
		bool reorder, int index, LWFContainer lwfContainer)
	{
		m_attachedLWFList = new AttachedLWFList(m_attachedLWFList);
		m_attachedLWFList[index] = lwfContainer;
		m_attachedLWFDescendingList[index] = index;
		if (reorder) {
			AttachedLWFList list = m_attachedLWFList;
			m_attachedLWFList = new AttachedLWFList();
			m_attachedLWFDescendingList =
				new AttachedLWFDescendingList(new DescendingComparer<int>());
			int i = 0;
			foreach (LWFContainer l in list.Values) {
				l.child.depth = i;
				m_attachedLWFList[i] = l;
				m_attachedLWFDescendingList[i] = i;
				++i;
			}
		}
	}

	private void DeleteAttachedLWF(Movie parent, LWFContainer lwfContainer,
		bool destroy = true, bool deleteFromDetachedLWFs = true)
	{
		string attachName = lwfContainer.child.attachName;
		int attachDepth = lwfContainer.child.depth;
		parent.m_attachedLWFs.Remove(attachName);
		parent.m_attachedLWFList.Remove(attachDepth);
		parent.m_attachedLWFDescendingList.Remove(attachDepth);
		if (deleteFromDetachedLWFs)
			parent.m_detachedLWFs.Remove(attachName);
		if (destroy) {
			if (lwfContainer.child.detachHandler != null) {
				lwfContainer.child.detachHandler(lwfContainer.child);
				lwfContainer.child.parent = null;
				lwfContainer.child.detachHandler = null;
				lwfContainer.child.attachName = null;
				lwfContainer.child.depth = -1;
			} else {
				lwfContainer.child.Destroy();
			}
			lwfContainer.Destroy();
		}
	}

	public LWF AttachLWF(string path, string attachName,
		int attachDepth = -1, bool reorder = false, string texturePrefix = null)
	{
		if (m_lwf.lwfLoader == null)
			return null;

		LWF child = m_lwf.lwfLoader(path, texturePrefix);
		if (child == null)
			return null;

		AttachLWF(child,
			attachName, attachDepth, reorder, (l) => {l.Destroy();});
		return child;
	}

	public void AttachLWF(LWF child, string attachName, int attachDepth = -1,
		bool reorder = false, DetachHandler detachHandler = null)
	{
		if (m_attachedLWFs == null) {
			m_attachedLWFs = new AttachedLWFs();
			m_detachedLWFs = new DetachDict();
			m_attachedLWFList = new AttachedLWFList();
			m_attachedLWFDescendingList =
				new AttachedLWFDescendingList(new DescendingComparer<int>());
		}

		LWFContainer lwfContainer;
		if (child.parent != null) {
			child.parent.m_attachedLWFs.TryGetValue(
				child.attachName, out lwfContainer);
			DeleteAttachedLWF(child.parent, lwfContainer, false);
		}
		if (m_attachedLWFs.TryGetValue(attachName, out lwfContainer))
			DeleteAttachedLWF(this, lwfContainer);

		if (!reorder && attachDepth >= 0)
			if (m_attachedLWFList.TryGetValue(attachDepth, out lwfContainer))
				DeleteAttachedLWF(this, lwfContainer);

		lwfContainer = new LWFContainer(this, child);

		if (child.interactive == true)
			m_lwf.SetInteractive();
		child.parent = this;
		child.SetRoot(m_lwf._root);
		child.detachHandler = detachHandler;
		child.attachName = attachName;
		if (attachDepth >= 0) {
			child.depth = attachDepth;
		} else {
			AttachedLWFDescendingList.KeyCollection.Enumerator e =
				m_attachedLWFDescendingList.Keys.GetEnumerator();
			if (e.MoveNext())
				child.depth = e.Current + 1;
			else
				child.depth = 0;
		}
		m_attachedLWFs[attachName] = lwfContainer;
		ReorderAttachedLWFList(reorder, child.depth, lwfContainer);

		m_lwf.SetLWFAttached();

		return;
	}

	public void SwapAttachedLWFDepth(int depth0, int depth1)
	{
		if (m_attachedLWFs == null)
			return;

		LWFContainer attachedLWF0;
		m_attachedLWFList.TryGetValue(depth0, out attachedLWF0);
		LWFContainer attachedLWF1;
		m_attachedLWFList.TryGetValue(depth1, out attachedLWF1);
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
			LWFContainer lwfContainer;
			if (m_attachedLWFList.TryGetValue(attachDepth, out lwfContainer))
				return lwfContainer.child;
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
		if (m_detachedLWFs != null) {
			LWFContainer lwfContainer;
			if (m_attachedLWFList.TryGetValue(attachDepth, out lwfContainer))
				m_detachedLWFs[lwfContainer.child.attachName] = true;
		}
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

	public void RemoveMovieClip()
	{
		if (m_type == Type.ATTACHEDMOVIE) {
			DetachFromParent();
		} else if (m_lwf.attachName != null && m_lwf.parent != null) {
			m_lwf.parent.DetachLWF(m_lwf.attachName);
		}
	}

	public BitmapClip AttachBitmap(string linkageName, int depth)
	{
		int bitmapId;
		if (!m_lwf.data.bitmapMap.TryGetValue(linkageName, out bitmapId))
			return null;
		var bitmap = new BitmapClip(m_lwf, this, bitmapId);
		if (m_bitmapClips != null)
			DetachBitmap(depth);
		else
			m_bitmapClips = new BitmapClips();
		m_bitmapClips[depth] = bitmap;
		bitmap.depth = depth;
		bitmap.name = linkageName;
		return bitmap;
	}

	public BitmapClips GetAttachedBitmaps()
	{
		return m_bitmapClips;
	}

	public BitmapClip GetAttachedBitmap(int depth)
	{
		if (m_bitmapClips == null)
			return null;
		BitmapClip bitmap = null;
		m_bitmapClips.TryGetValue(depth, out bitmap);
		return bitmap;
	}

	public void SwapAttachedBitmapDepth(int depth0, int depth1)
	{
		if (m_bitmapClips == null)
			return;

		BitmapClip bitmapClip0;
		m_bitmapClips.TryGetValue(depth0, out bitmapClip0);
		BitmapClip bitmapClip1;
		m_bitmapClips.TryGetValue(depth1, out bitmapClip1);
		if (bitmapClip0 != null) {
			bitmapClip0.depth = depth1;
			m_bitmapClips[depth1] = bitmapClip0;
		} else {
			m_bitmapClips.Remove(depth1);
		}
		if (bitmapClip1 != null) {
			bitmapClip1.depth = depth0;
			m_bitmapClips[depth0] = bitmapClip1;
		} else {
			m_bitmapClips.Remove(depth0);
		}
	}

	public void DetachBitmap(int depth)
	{
		if (m_bitmapClips == null)
			return;
		BitmapClip bitmapClip = null;
		if (!m_bitmapClips.TryGetValue(depth, out bitmapClip))
			return;
		bitmapClip.Destroy();
		m_bitmapClips.Remove(depth);
	}
}

}	// namespace LWF
