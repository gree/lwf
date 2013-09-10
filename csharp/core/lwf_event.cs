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
using System.IO;

namespace LWF {

using EventHandler = Action<Movie, Button>;
using EventHandlerList = List<Action<Movie, Button>>;
using GenericEventHandlerDictionary =
	Dictionary<string, List<Action<Movie, Button>>>;
using MovieEventHandler = Action<Movie>;
using ButtonEventHandler = Action<Button>;
using ButtonKeyPressHandler = Action<Button, int>;
using MovieCommand = System.Action<Movie>;
using MovieCommands = Dictionary<List<string>, System.Action<Movie>>;
using ProgramObjectConstructor = Func<ProgramObject, int, int, int, Renderer>;
using Condition = Format.ButtonCondition.Condition;
using MovieEventHandlersDictionary = Dictionary<string, MovieEventHandlers>;
using ButtonEventHandlersDictionary = Dictionary<string, ButtonEventHandlers>;
using DetachHandler = Action<LWF>;
using Inspector = System.Action<Object, int, int, int>;
using AllowButtonList = Dictionary<int, bool>;
using DenyButtonList = Dictionary<int, bool>;

public partial class LWF
{
	private EventHandlerList[] m_eventHandlers;
	private GenericEventHandlerDictionary m_genericEventHandlerDictionary;
	private MovieEventHandlers[] m_movieEventHandlers;
	private ButtonEventHandlers[] m_buttonEventHandlers;
	private MovieEventHandlersDictionary m_movieEventHandlersByFullName;
	private ButtonEventHandlersDictionary m_buttonEventHandlersByFullName;

	private void InitEvent()
	{
		m_eventHandlers = new EventHandlerList[m_data.events.Length];
		m_genericEventHandlerDictionary = new GenericEventHandlerDictionary();
		m_movieEventHandlers = new MovieEventHandlers[m_instances.Length];
		m_buttonEventHandlers = new ButtonEventHandlers[m_instances.Length];
	}

	private int GetEventOffset()
	{
		return m_eventOffset;
	}

	public int AddEventHandler(string eventName, EventHandler eventHandler)
	{
		int eventId = SearchEventId(eventName);
		int id;
		if (eventId >= 0 && eventId < m_data.events.Length) {
			id = AddEventHandler(eventId, eventHandler);
		} else {
			EventHandlerList list;
			if (!m_genericEventHandlerDictionary.TryGetValue(
					eventName, out list)) {
				list = new EventHandlerList();
				m_genericEventHandlerDictionary[eventName] = list;
			}
			id = ++m_eventOffset;
			list.Add(eventHandler);
		}
		return id;
	}

	public int AddEventHandler(int eventId, EventHandler eventHandler)
	{
		if (eventId < 0 || eventId >= m_data.events.Length)
			return -1;
		EventHandlerList list = m_eventHandlers[eventId];
		if (list == null) {
			list = new EventHandlerList();
			m_eventHandlers[eventId] = list;
		}
		list.Add(eventHandler);
		return ++m_eventOffset;
	}

	public void RemoveEventHandler(string eventName, EventHandler eventHandler)
	{
		int eventId = SearchEventId(eventName);
		if (eventId >= 0 && eventId < m_data.events.Length) {
			RemoveEventHandler(eventId, eventHandler);
		} else {
			EventHandlerList list = m_genericEventHandlerDictionary[eventName];
			if (list == null)
				return;
			list.RemoveAll(h => h == eventHandler);
		}
	}

	public void RemoveEventHandler(int eventId, EventHandler eventHandler)
	{
		if (eventId < 0 || eventId >= m_data.events.Length)
			return;
		EventHandlerList list = m_eventHandlers[eventId];
		if (list == null)
			return;
		list.RemoveAll(h => h == eventHandler);
	}

	public void ClearEventHandler(string eventName)
	{
		int eventId = SearchEventId(eventName);
		if (eventId >= 0 && eventId < m_data.events.Length) {
			ClearEventHandler(eventId);
		} else {
			m_genericEventHandlerDictionary.Remove(eventName);
		}
	}

	public void ClearEventHandler(int eventId)
	{
		if (eventId < 0 || eventId >= m_data.events.Length)
			return;
		m_eventHandlers[eventId] = null;
	}

	public void SetEventHandler(string eventName, EventHandler eventHandler)
	{
		SetEventHandler(SearchEventId(eventName), eventHandler);
	}

	public void SetEventHandler(int eventId, EventHandler eventHandler)
	{
		ClearEventHandler(eventId);
		AddEventHandler(eventId, eventHandler);
	}

	public void DispatchEvent(string eventName, Movie m = null, Button b = null)
	{
		if (m == null)
			m = m_rootMovie;
		int eventId = SearchEventId(eventName);
		if (eventId >= 0 && eventId < m_data.events.Length) {
			EventHandlerList list =
				new EventHandlerList(m_eventHandlers[eventId]);
			list.ForEach(h => h(m, b));
		} else {
			EventHandlerList list = new EventHandlerList(
				m_genericEventHandlerDictionary[eventName]);
			foreach (EventHandler h in list)
				h(m, b);
		}
	}

	public MovieEventHandlers GetMovieEventHandlers(Movie m)
	{
		if (m_movieEventHandlersByFullName != null) {
			string fullName = m.GetFullName();
			if (fullName != null) {
				MovieEventHandlers handlers;
				if (m_movieEventHandlersByFullName.TryGetValue(
						fullName, out handlers)) {
					return handlers;
				}
			}
		}

		int instId = m.instanceId;
		if (instId < 0 || instId >= m_instances.Length)
			return null;
		return m_movieEventHandlers[instId];
	}

	public int AddMovieEventHandler(string instanceName,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			AddMovieEventHandler(
				instId, load, postLoad, unload, enterFrame, update, render);
			return -1;
		}

		if (!instanceName.Contains("."))
			return -1;

		if (m_movieEventHandlersByFullName == null)
			m_movieEventHandlersByFullName = new MovieEventHandlersDictionary();

		MovieEventHandlers handlers;
		if (!m_movieEventHandlersByFullName.TryGetValue(
				instanceName, out handlers)) {
			handlers = new MovieEventHandlers();
			m_movieEventHandlersByFullName[instanceName] = handlers;
		}

		handlers.Add(load, postLoad, unload, enterFrame, update, render);

		Movie movie = SearchMovieInstance(instanceName);
		if (movie != null)
			movie.SetHandlers(handlers);
		return ++m_eventOffset;
	}

	public int AddMovieEventHandler(int instId,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return -1;

		MovieEventHandlers handlers = m_movieEventHandlers[instId];
		if (handlers == null) {
			handlers = new MovieEventHandlers();
			m_movieEventHandlers[instId] = handlers;
		}

		handlers.Add(load, postLoad, unload, enterFrame, update, render);

		Movie movie = SearchMovieInstanceByInstanceId(instId);
		if (movie != null)
			movie.SetHandlers(handlers);
		return ++m_eventOffset;
	}

	public void RemoveMovieEventHandler(string instanceName,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			RemoveMovieEventHandler(
				instId, load, postLoad, unload, enterFrame, update, render);
			return;
		}

		if (m_movieEventHandlersByFullName == null)
			return;

		MovieEventHandlers handlers;
		if (!m_movieEventHandlersByFullName.TryGetValue(
				instanceName, out handlers))
			return;

		handlers.Remove(load, postLoad, unload, enterFrame, update, render);
		Movie movie = SearchMovieInstance(instanceName);
		if (movie != null)
			movie.SetHandlers(handlers);
	}

	public void RemoveMovieEventHandler(int instId,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		MovieEventHandlers handlers = m_movieEventHandlers[instId];
		if (handlers == null)
			return;

		handlers.Remove(load, postLoad, unload, enterFrame, update, render);
		Movie movie = SearchMovieInstanceByInstanceId(instId);
		if (movie != null)
			movie.SetHandlers(handlers);
	}

	public void ClearMovieEventHandler(string instanceName)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			ClearMovieEventHandler(instId);
			return;
		}

		if (m_movieEventHandlersByFullName == null)
			return;

		MovieEventHandlers handlers;
		if (!m_movieEventHandlersByFullName.TryGetValue(
				instanceName, out handlers))
			return;

		handlers.Clear();
		Movie movie = SearchMovieInstance(instanceName);
		if (movie != null)
			movie.SetHandlers(handlers);
	}

	public void ClearMovieEventHandler(int instId)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		MovieEventHandlers handlers = m_movieEventHandlers[instId];
		if (handlers == null)
			return;

		handlers.Clear();
		Movie movie = SearchMovieInstanceByInstanceId(instId);
		if (movie != null)
			movie.SetHandlers(handlers);
	}

	public void ClearMovieEventHandler(
		string instanceName, MovieEventHandlers.Type type)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			ClearMovieEventHandler(instId, type);
			return;
		}

		if (m_movieEventHandlersByFullName == null)
			return;

		MovieEventHandlers handlers;
		if (!m_movieEventHandlersByFullName.TryGetValue(
				instanceName, out handlers))
			return;

		handlers.Clear(type);
		Movie movie = SearchMovieInstance(instanceName);
		if (movie != null)
			movie.SetHandlers(handlers);
	}

	public void ClearMovieEventHandler(
		int instId, MovieEventHandlers.Type type)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		MovieEventHandlers handlers = m_movieEventHandlers[instId];
		if (handlers == null)
			return;

		handlers.Clear(type);
		Movie movie = SearchMovieInstanceByInstanceId(instId);
		if (movie != null)
			movie.SetHandlers(handlers);
	}

	public void SetMovieEventHandler(string instanceName,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		ClearMovieEventHandler(instanceName);
		AddMovieEventHandler(instanceName,
			load, postLoad, unload, enterFrame, update, render);
	}

	public void SetMovieEventHandler(int instId,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		ClearMovieEventHandler(instId);
		AddMovieEventHandler(instId,
			load, postLoad, unload, enterFrame, update, render);
	}

	public ButtonEventHandlers GetButtonEventHandlers(Button b)
	{
		if (m_buttonEventHandlersByFullName != null) {
			string fullName = b.GetFullName();
			if (fullName != null) {
				ButtonEventHandlers handlers;
				if (m_buttonEventHandlersByFullName.TryGetValue(
						fullName, out handlers)) {
					return handlers;
				}
			}
		}

		int instId = b.instanceId;
		if (instId < 0 || instId >= m_instances.Length)
			return null;
		return m_buttonEventHandlers[instId];
	}

	public int AddButtonEventHandler(string instanceName,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null)
	{
		interactive = true;

		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			return AddButtonEventHandler(instId,
				load, unload, enterFrame, update, render,
				press, release, rollOver, rollOut, keyPress);
		}

		if (!instanceName.Contains("."))
			return -1;

		if (m_buttonEventHandlersByFullName == null)
			m_buttonEventHandlersByFullName =
				new ButtonEventHandlersDictionary();

		ButtonEventHandlers handlers;
		if (!m_buttonEventHandlersByFullName.TryGetValue(
				instanceName, out handlers)) {
			handlers = new ButtonEventHandlers();
			m_buttonEventHandlersByFullName[instanceName] = handlers;
		}

		handlers.Add(load, unload, enterFrame, update, render,
			press, release, rollOver, rollOut, keyPress);

		Button button = SearchButtonInstance(instanceName);
		if (button != null)
			button.SetHandlers(handlers);
		return ++m_eventOffset;
	}

	public int AddButtonEventHandler(int instId,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null)
	{
		interactive = true;

		if (instId < 0 || instId >= m_instances.Length)
			return -1;

		ButtonEventHandlers handlers = m_buttonEventHandlers[instId];
		if (handlers == null) {
			handlers = new ButtonEventHandlers();
			m_buttonEventHandlers[instId] = handlers;
		}

		handlers.Add(load, unload, enterFrame, update, render,
			press, release, rollOver, rollOut, keyPress);

		Button button = SearchButtonInstanceByInstanceId(instId);
		if (button != null)
			button.SetHandlers(handlers);
		return ++m_eventOffset;
	}

	public void RemoveButtonEventHandler(string instanceName,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			RemoveButtonEventHandler(instId,
				load, unload, enterFrame, update, render,
				press, release, rollOver, rollOut, keyPress);
			return;
		}

		if (m_buttonEventHandlersByFullName == null)
			return;

		ButtonEventHandlers handlers;
		if (!m_buttonEventHandlersByFullName.TryGetValue(
				instanceName, out handlers))
			return;

		handlers.Remove(load, unload, enterFrame, update, render,
			press, release, rollOver, rollOut, keyPress);

		Button button = SearchButtonInstance(instanceName);
		if (button != null)
			button.SetHandlers(handlers);
	}

	public void RemoveButtonEventHandler(int instId,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		ButtonEventHandlers handlers = m_buttonEventHandlers[instId];
		if (handlers == null)
			return;

		handlers.Remove(load, unload, enterFrame, update, render,
			press, release, rollOver, rollOut, keyPress);

		Button button = SearchButtonInstanceByInstanceId(instId);
		if (button != null)
			button.SetHandlers(handlers);
	}

	public void ClearButtonEventHandler(string instanceName)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			ClearButtonEventHandler(instId);
			return;
		}

		if (m_buttonEventHandlersByFullName == null)
			return;

		ButtonEventHandlers handlers;
		if (!m_buttonEventHandlersByFullName.TryGetValue(
				instanceName, out handlers))
			return;

		handlers.Clear();

		Button button = SearchButtonInstance(instanceName);
		if (button != null)
			button.SetHandlers(handlers);
	}

	public void ClearButtonEventHandler(int instId)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		ButtonEventHandlers handlers = m_buttonEventHandlers[instId];
		if (handlers == null)
			return;

		handlers.Clear();

		Button button = SearchButtonInstanceByInstanceId(instId);
		if (button != null)
			button.SetHandlers(handlers);
	}

	public void ClearButtonEventHandler(
		string instanceName, ButtonEventHandlers.Type type)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			ClearButtonEventHandler(instId, type);
			return;
		}

		if (m_buttonEventHandlersByFullName == null)
			return;

		ButtonEventHandlers handlers;
		if (!m_buttonEventHandlersByFullName.TryGetValue(
				instanceName, out handlers))
			return;

		handlers.Clear(type);

		Button button = SearchButtonInstance(instanceName);
		if (button != null)
			button.SetHandlers(handlers);
	}

	public void ClearButtonEventHandler(
		int instId, ButtonEventHandlers.Type type)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		ButtonEventHandlers handlers = m_buttonEventHandlers[instId];
		if (handlers == null)
			return;

		handlers.Clear(type);

		Button button = SearchButtonInstanceByInstanceId(instId);
		if (button != null)
			button.SetHandlers(handlers);
	}

	public void SetButtonEventHandler(string instanceName,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null)
	{
		ClearButtonEventHandler(instanceName);
		AddButtonEventHandler(instanceName,
			load, unload, enterFrame, update, render,
			press, release, rollOver, rollOut, keyPress);
	}

	public void SetButtonEventHandler(int instId,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null)
	{
		ClearButtonEventHandler(instId);
		AddButtonEventHandler(instId,
			load, unload, enterFrame, update, render,
			press, release, rollOver, rollOut, keyPress);
	}
}

}	// namespace LWF
