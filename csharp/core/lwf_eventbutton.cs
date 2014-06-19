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

using ButtonEventHandler = Action<Button>;
using ButtonKeyPressHandler = Action<Button, int>;
using ButtonEventHandlerDictionary = Dictionary<int, Action<Button>>;
using ButtonKeyPressHandlerDictionary = Dictionary<int, Action<Button, int>>;

public class ButtonEventHandlers
{
	public enum Type {
		LOAD,
		UNLOAD,
		ENTERFRAME,
		UPDATE,
		RENDER,
		PRESS,
		RELEASE,
		ROLLOVER,
		ROLLOUT,
		KEYPRESS
	}

	ButtonEventHandlerDictionary load;
	ButtonEventHandlerDictionary unload;
	ButtonEventHandlerDictionary enterFrame;
	ButtonEventHandlerDictionary update;
	ButtonEventHandlerDictionary render;
	ButtonEventHandlerDictionary press;
	ButtonEventHandlerDictionary release;
	ButtonEventHandlerDictionary rollOver;
	ButtonEventHandlerDictionary rollOut;
	ButtonKeyPressHandlerDictionary keyPress;

	public ButtonEventHandlers()
	{
		load = new ButtonEventHandlerDictionary();
		unload = new ButtonEventHandlerDictionary();
		enterFrame = new ButtonEventHandlerDictionary();
		update = new ButtonEventHandlerDictionary();
		render = new ButtonEventHandlerDictionary();
		press = new ButtonEventHandlerDictionary();
		release = new ButtonEventHandlerDictionary();
		rollOver = new ButtonEventHandlerDictionary();
		rollOut = new ButtonEventHandlerDictionary();
		keyPress = new ButtonKeyPressHandlerDictionary();
	}

	public void Clear()
	{
		load.Clear();
		unload.Clear();
		enterFrame.Clear();
		update.Clear();
		render.Clear();
		press.Clear();
		release.Clear();
		rollOver.Clear();
		rollOut.Clear();
		keyPress.Clear();
	}

	public void Clear(Type type)
	{
		switch (type) {
		case Type.LOAD: load.Clear(); break;
		case Type.UNLOAD: unload.Clear(); break;
		case Type.ENTERFRAME: enterFrame.Clear(); break;
		case Type.UPDATE: update.Clear(); break;
		case Type.RENDER: render.Clear(); break;
		case Type.PRESS: press.Clear(); break;
		case Type.RELEASE: release.Clear(); break;
		case Type.ROLLOVER: rollOver.Clear(); break;
		case Type.ROLLOUT: rollOut.Clear(); break;
		case Type.KEYPRESS: keyPress.Clear(); break;
		}
	}

	public void Add(ButtonEventHandlers handlers)
	{
		if (handlers == null)
			return;

		foreach (var h in handlers.load)
			load.Add(h.Key, h.Value);
		foreach (var h in handlers.unload)
			unload.Add(h.Key, h.Value);
		foreach (var h in handlers.enterFrame)
			enterFrame.Add(h.Key, h.Value);
		foreach (var h in handlers.update)
			update.Add(h.Key, h.Value);
		foreach (var h in handlers.render)
			render.Add(h.Key, h.Value);
		foreach (var h in handlers.press)
			press.Add(h.Key, h.Value);
		foreach (var h in handlers.release)
			release.Add(h.Key, h.Value);
		foreach (var h in handlers.rollOver)
			rollOver.Add(h.Key, h.Value);
		foreach (var h in handlers.rollOut)
			rollOut.Add(h.Key, h.Value);
		foreach (var h in handlers.keyPress)
			keyPress.Add(h.Key, h.Value);
	}

	public void Add(int key,
		ButtonEventHandler l = null, ButtonEventHandler u = null,
		ButtonEventHandler e = null, ButtonEventHandler up = null,
		ButtonEventHandler r = null, ButtonEventHandler p = null,
		ButtonEventHandler rl = null, ButtonEventHandler rOver = null,
		ButtonEventHandler rOut = null, ButtonKeyPressHandler k = null)
	{
		if (l != null)
			load.Add(key, l);
		if (u != null)
			unload.Add(key, u);
		if (e != null)
			enterFrame.Add(key, e);
		if (up != null)
			update.Add(key, up);
		if (r != null)
			render.Add(key, r);
		if (p != null)
			press.Add(key, p);
		if (rl != null)
			release.Add(key, rl);
		if (rOver != null)
			rollOver.Add(key, rOver);
		if (rOut != null)
			rollOut.Add(key, rOut);
		if (k != null)
			keyPress.Add(key, k);
	}

	public void Remove(int key)
	{
		load.Remove(key);
		unload.Remove(key);
		enterFrame.Remove(key);
		update.Remove(key);
		render.Remove(key);
		press.Remove(key);
		release.Remove(key);
		rollOver.Remove(key);
		rollOut.Remove(key);
		keyPress.Remove(key);
	}

	public void Call(Type type, Button target)
	{
		ButtonEventHandlerDictionary dict = null; 
		switch (type) {
		case Type.LOAD: dict = load; break;
		case Type.UNLOAD: dict = unload; break;
		case Type.ENTERFRAME: dict = enterFrame; break;
		case Type.UPDATE: dict = update; break;
		case Type.RENDER: dict = render; break;
		case Type.PRESS: dict = press; break;
		case Type.RELEASE: dict = release; break;
		case Type.ROLLOVER: dict = rollOver; break;
		case Type.ROLLOUT: dict = rollOut; break;
		}
		if (dict != null) {
			dict = new ButtonEventHandlerDictionary(dict);
			foreach (var h in dict)
				h.Value(target);
		}
	}

	public void CallKEYPRESS(Button target, int code)
	{
		ButtonKeyPressHandlerDictionary dict =
			new ButtonKeyPressHandlerDictionary(keyPress);
		foreach (var h in dict)
			h.Value(target, code);
	}
}

}	// namespace LWF
