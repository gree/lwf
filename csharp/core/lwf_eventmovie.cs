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

using MovieEventHandler = Action<Movie>;
using MovieEventHandlerList = List<Action<Movie>>;

public class MovieEventHandlers
{
	public enum Type {
		LOAD,
		POSTLOAD,
		UNLOAD,
		ENTERFRAME,
		UPDATE,
		RENDER
	}

	MovieEventHandlerList load;
	MovieEventHandlerList postLoad;
	MovieEventHandlerList unload;
	MovieEventHandlerList enterFrame;
	MovieEventHandlerList update;
	MovieEventHandlerList render;
	bool empty;

	public MovieEventHandlers()
	{
		load = new MovieEventHandlerList();
		postLoad = new MovieEventHandlerList();
		unload = new MovieEventHandlerList();
		enterFrame = new MovieEventHandlerList();
		update = new MovieEventHandlerList();
		render = new MovieEventHandlerList();
		empty = true;
	}

	public void Clear()
	{
		load.Clear();
		postLoad.Clear();
		unload.Clear();
		enterFrame.Clear();
		update.Clear();
		render.Clear();
		empty = true;
	}

	public void Clear(Type type)
	{
		switch (type) {
		case Type.LOAD: load.Clear(); break;
		case Type.POSTLOAD: postLoad.Clear(); break;
		case Type.UNLOAD: unload.Clear(); break;
		case Type.ENTERFRAME: enterFrame.Clear(); break;
		case Type.UPDATE: update.Clear(); break;
		case Type.RENDER: render.Clear(); break;
		}
		UpdateEmpty();
	}

	public void Add(MovieEventHandlers handlers)
	{
		if (handlers == null)
			return;

		handlers.load.ForEach(h => load.Add(h));
		handlers.postLoad.ForEach(h => postLoad.Add(h));
		handlers.unload.ForEach(h => unload.Add(h));
		handlers.enterFrame.ForEach(h => enterFrame.Add(h));
		handlers.update.ForEach(h => update.Add(h));
		handlers.render.ForEach(h => render.Add(h));
		UpdateEmpty();
	}

	public void Add(
		MovieEventHandler l = null, MovieEventHandler p = null,
		MovieEventHandler u = null, MovieEventHandler e = null,
		MovieEventHandler up = null, MovieEventHandler r = null)
	{
		if (l != null)
			load.Add(l);
		if (p != null)
			postLoad.Add(p);
		if (u != null)
			unload.Add(u);
		if (e != null)
			enterFrame.Add(e);
		if (up != null)
			update.Add(up);
		if (r != null)
			render.Add(r);
		UpdateEmpty();
	}

	public void Remove(
		MovieEventHandler l = null, MovieEventHandler p = null,
		MovieEventHandler u = null, MovieEventHandler e = null,
		MovieEventHandler up = null, MovieEventHandler r = null)
	{
		if (l != null)
			load.RemoveAll(h => h == l);
		if (p != null)
			postLoad.RemoveAll(h => h == p);
		if (u != null)
			unload.RemoveAll(h => h == u);
		if (e != null)
			enterFrame.RemoveAll(h => h == e);
		if (up != null)
			update.RemoveAll(h => h == up);
		if (r != null)
			render.RemoveAll(h => h == r);
		UpdateEmpty();
	}

	public void Call(Type type, Movie target)
	{
		MovieEventHandlerList list = null;
		switch (type) {
		case Type.LOAD: list = load; break;
		case Type.POSTLOAD: list = postLoad; break;
		case Type.UNLOAD: list = unload; break;
		case Type.ENTERFRAME: list = enterFrame; break;
		case Type.UPDATE: list = update; break;
		case Type.RENDER: list = render; break;
		}
		if (list != null) {
			list = new MovieEventHandlerList(list);
			list.ForEach(h => h(target));
		}
	}

	private void UpdateEmpty()
	{
		empty = true;
		if (load.Count > 0) {
			empty = false;
			return;
		}
		if (postLoad.Count > 0) {
			empty = false;
			return;
		}
		if (unload.Count > 0) {
			empty = false;
			return;
		}
		if (enterFrame.Count > 0) {
			empty = false;
			return;
		}
		if (update.Count > 0) {
			empty = false;
			return;
		}
		if (render.Count > 0) {
			empty = false;
			return;
		}
	}

	public bool Empty()
	{
		return empty;
	}
}

}	// namespace LWF
