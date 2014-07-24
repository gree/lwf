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
using MovieEventHandlerDictionary = Dictionary<int, Action<Movie>>;

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

	MovieEventHandlerDictionary load;
	MovieEventHandlerDictionary postLoad;
	MovieEventHandlerDictionary unload;
	MovieEventHandlerDictionary enterFrame;
	MovieEventHandlerDictionary update;
	MovieEventHandlerDictionary render;
	bool empty;

	public MovieEventHandlers()
	{
		load = new MovieEventHandlerDictionary();
		postLoad = new MovieEventHandlerDictionary();
		unload = new MovieEventHandlerDictionary();
		enterFrame = new MovieEventHandlerDictionary();
		update = new MovieEventHandlerDictionary();
		render = new MovieEventHandlerDictionary();
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

		foreach (var h in handlers.load)
			load.Add(h.Key, h.Value);
		foreach (var h in handlers.postLoad)
			postLoad.Add(h.Key, h.Value);
		foreach (var h in handlers.unload)
			unload.Add(h.Key, h.Value);
		foreach (var h in handlers.enterFrame)
			enterFrame.Add(h.Key, h.Value);
		foreach (var h in handlers.update)
			update.Add(h.Key, h.Value);
		foreach (var h in handlers.render)
			render.Add(h.Key, h.Value);
		UpdateEmpty();
	}

	public void Add(int key,
		MovieEventHandler l = null, MovieEventHandler p = null,
		MovieEventHandler u = null, MovieEventHandler e = null,
		MovieEventHandler up = null, MovieEventHandler r = null)
	{
		if (l != null)
			load.Add(key, l);
		if (p != null)
			postLoad.Add(key, p);
		if (u != null)
			unload.Add(key, u);
		if (e != null)
			enterFrame.Add(key, e);
		if (up != null)
			update.Add(key, up);
		if (r != null)
			render.Add(key, r);
		UpdateEmpty();
	}

	public void Remove(int key)
	{
		load.Remove(key);
		postLoad.Remove(key);
		unload.Remove(key);
		enterFrame.Remove(key);
		update.Remove(key);
		render.Remove(key);
		UpdateEmpty();
	}

	public void Call(Type type, Movie target)
	{
		MovieEventHandlerDictionary dict = null;
		switch (type) {
		case Type.LOAD: dict = load; break;
		case Type.POSTLOAD: dict = postLoad; break;
		case Type.UNLOAD: dict = unload; break;
		case Type.ENTERFRAME: dict = enterFrame; break;
		case Type.UPDATE: dict = update; break;
		case Type.RENDER: dict = render; break;
		}
		if (dict != null) {
			dict = new MovieEventHandlerDictionary(dict);
			foreach (var h in dict)
				h.Value(target);
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
