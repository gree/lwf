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
using ButtonEventHandlerList = List<Action<Button>>;
using ButtonKeyPressHandlerList = List<Action<Button, int>>;

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

	ButtonEventHandlerList load;
	ButtonEventHandlerList unload;
	ButtonEventHandlerList enterFrame;
	ButtonEventHandlerList update;
	ButtonEventHandlerList render;
	ButtonEventHandlerList press;
	ButtonEventHandlerList release;
	ButtonEventHandlerList rollOver;
	ButtonEventHandlerList rollOut;
	ButtonKeyPressHandlerList keyPress;

	public ButtonEventHandlers()
	{
		load = new ButtonEventHandlerList();
		unload = new ButtonEventHandlerList();
		enterFrame = new ButtonEventHandlerList();
		update = new ButtonEventHandlerList();
		render = new ButtonEventHandlerList();
		press = new ButtonEventHandlerList();
		release = new ButtonEventHandlerList();
		rollOver = new ButtonEventHandlerList();
		rollOut = new ButtonEventHandlerList();
		keyPress = new ButtonKeyPressHandlerList();
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

		handlers.load.ForEach(h => load.Add(h));
		handlers.unload.ForEach(h => unload.Add(h));
		handlers.enterFrame.ForEach(h => enterFrame.Add(h));
		handlers.update.ForEach(h => update.Add(h));
		handlers.render.ForEach(h => render.Add(h));
		handlers.press.ForEach(h => press.Add(h));
		handlers.release.ForEach(h => release.Add(h));
		handlers.rollOver.ForEach(h => rollOver.Add(h));
		handlers.rollOut.ForEach(h => rollOut.Add(h));
		handlers.keyPress.ForEach(h => keyPress.Add(h));
	}

	public void Add(
		ButtonEventHandler l = null, ButtonEventHandler u = null,
		ButtonEventHandler e = null, ButtonEventHandler up = null,
		ButtonEventHandler r = null, ButtonEventHandler p = null,
		ButtonEventHandler rl = null, ButtonEventHandler rOver = null,
		ButtonEventHandler rOut = null, ButtonKeyPressHandler k = null)
	{
		if (l != null)
			load.Add(l);
		if (u != null)
			unload.Add(u);
		if (e != null)
			enterFrame.Add(e);
		if (up != null)
			update.Add(up);
		if (r != null)
			render.Add(r);
		if (p != null)
			press.Add(p);
		if (rl != null)
			release.Add(rl);
		if (rOver != null)
			rollOver.Add(rOver);
		if (rOut != null)
			rollOut.Add(rOut);
		if (k != null)
			keyPress.Add(k);
	}

	public void Remove(
		ButtonEventHandler l = null, ButtonEventHandler u = null,
		ButtonEventHandler e = null, ButtonEventHandler up = null,
		ButtonEventHandler r = null, ButtonEventHandler p = null,
		ButtonEventHandler rl = null, ButtonEventHandler rOver = null,
		ButtonEventHandler rOut = null, ButtonKeyPressHandler k = null)
	{
		if (l != null)
			load.RemoveAll(h => h == l);
		if (u != null)
			unload.RemoveAll(h => h == u);
		if (e != null)
			enterFrame.RemoveAll(h => h == e);
		if (up != null)
			update.RemoveAll(h => h == up);
		if (r != null)
			render.RemoveAll(h => h == r);
		if (p != null)
			press.RemoveAll(h => h == p);
		if (rl != null)
			release.RemoveAll(h => h == rl);
		if (rOver != null)
			rollOver.RemoveAll(h => h == rOver);
		if (rOut != null)
			rollOut.RemoveAll(h => h == rOut);
		if (k != null)
			keyPress.RemoveAll(h => h == k);
	}

	public void Call(Type type, Button target)
	{
		ButtonEventHandlerList list = null; 
		switch (type) {
		case Type.LOAD: list = load; break;
		case Type.UNLOAD: list = unload; break;
		case Type.ENTERFRAME: list = enterFrame; break;
		case Type.UPDATE: list = update; break;
		case Type.RENDER: list = render; break;
		case Type.PRESS: list = press; break;
		case Type.RELEASE: list = release; break;
		case Type.ROLLOVER: list = rollOver; break;
		case Type.ROLLOUT: list = rollOut; break;
		}
		if (list != null) {
			list = new ButtonEventHandlerList(list);
			list.ForEach(h => h(target));
		}
	}

	public void CallKEYPRESS(Button target, int code)
	{
		ButtonKeyPressHandlerList list =
			new ButtonKeyPressHandlerList(keyPress);
		list.ForEach(h => h(target, code));
	}
}

}	// namespace LWF
