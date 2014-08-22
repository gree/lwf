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

namespace LWF {

using ButtonEventHandler = Action<Button>;
using ButtonKeyPressHandler = Action<Button, int>;
using EventType = ButtonEventHandlers.Type;
using Condition = Format.ButtonCondition.Condition;

public partial class Button : IObject
{
	private Format.Button m_data;
	private Button m_buttonLink;
	private ButtonEventHandlers m_handler;
	private Matrix m_invert;
	private float m_hitX;
	private float m_hitY;

	public Format.Button data {get {return m_data;}}
	public float width
		{get {return m_data != null ? (float)m_data.width : 0;}}
	public float height
		{get {return m_data != null ? (float)m_data.height : 0;}}
	public float hitX {get {return m_hitX;}}
	public float hitY {get {return m_hitY;}}
	public virtual Button buttonLink {
		get {return m_buttonLink;}
		set {m_buttonLink = value;}
	}

	public Button() {}

	public Button(LWF lwf, Movie parent, int objId, int instId,
			int matrixId = -1, int colorTransformId = -1)
		: base(lwf, parent, Format.Object.Type.BUTTON, objId, instId)
	{
		m_matrixId = matrixId;
		m_colorTransformId = colorTransformId;

		m_invert = new Matrix();
		m_hitX = Int32.MinValue;
		m_hitY = Int32.MinValue;

		if (objId >= 0) {
			m_data = lwf.data.buttons[objId];
			m_dataMatrixId = m_data.matrixId;
		}

		ButtonEventHandlers handler = lwf.GetButtonEventHandlers(this);
		if (handler != null) {
			m_handler = new ButtonEventHandlers();
			m_handler.Add(handler);
			m_handler.Call(EventType.LOAD, this);
		}
	}

	public void SetHandlers(ButtonEventHandlers handler)
	{
		if (m_handler == null)
			m_handler = new ButtonEventHandlers();
		else
			m_handler.Clear();
		m_handler.Add(handler);
	}

	public override void Exec(int matrixId = 0, int colorTransformId = 0)
	{
		base.Exec(matrixId, colorTransformId);

		EnterFrame();
	}

	public override void Update(Matrix m, ColorTransform c)
	{
		base.Update(m, c);

		if (m_handler != null)
			m_handler.Call(EventType.UPDATE, this);
	}

	public override void Render(bool v, int rOffset)
	{
		if (v && m_handler != null)
			m_handler.Call(EventType.RENDER, this);
	}

	public override void Destroy()
	{
		m_lwf.ClearFocus(this);
		m_lwf.ClearPressed(this);

		if (m_handler != null)
			m_handler.Call(EventType.UNLOAD, this);

		base.Destroy();
	}

	public override void LinkButton()
	{
		if (m_lwf.focus == this)
			m_lwf.focusOnLink = true;
		m_buttonLink = m_lwf.buttonHead;
		m_lwf.buttonHead = this;
	}

	public virtual bool CheckHit(float px, float py)
	{
		float x, y;
		Utility.InvertMatrix(m_invert, m_matrix);
		Utility.CalcMatrixToPoint(out x, out y, px, py, m_invert);
		if (x >= 0.0f && x < (float)m_data.width &&
				y >= 0.0f && y < (float)m_data.height) {
			m_hitX = x;
			m_hitY = y;
			return true;
		}
		m_hitX = Int32.MinValue;
		m_hitY = Int32.MinValue;
		return false;
	}

	public void EnterFrame()
	{
		if (m_handler != null)
			m_handler.Call(EventType.ENTERFRAME, this);
	}

	public virtual void RollOver()
	{
		if (m_handler != null)
			m_handler.Call(EventType.ROLLOVER, this);

		PlayAnimation(Condition.ROLLOVER);
	}

	public virtual void RollOut()
	{
		if (m_handler != null)
			m_handler.Call(EventType.ROLLOUT, this);

		PlayAnimation(Condition.ROLLOUT);
	}

	public virtual void Press()
	{
		if (m_handler != null)
			m_handler.Call(EventType.PRESS, this);

		PlayAnimation(Condition.PRESS);
	}

	public virtual void Release()
	{
		if (m_handler != null)
			m_handler.Call(EventType.RELEASE, this);

		PlayAnimation(Condition.RELEASE);
	}

	public virtual void KeyPress(int code)
	{
		if (m_handler != null)
			m_handler.CallKEYPRESS(this, code);

		PlayAnimation(Condition.KEYPRESS, code);
	}

	public void PlayAnimation(Condition condition, int code = 0)
	{
		Format.ButtonCondition[] conditions = m_lwf.data.buttonConditions;
		for (int i = 0; i < m_data.conditions; ++i) {
			Format.ButtonCondition c = conditions[m_data.conditionId + i];
			if ((c.condition & (int)condition) != 0 &&
					(condition != Condition.KEYPRESS || c.keyCode == code)) {
				m_lwf.PlayAnimation(c.animationId, m_parent, this);
			}
		}
	}

	public int AddEventHandler(string eventName, ButtonEventHandler handler)
	{
		int id = m_lwf.GetEventOffset();
		switch (eventName) {
		case "load": m_handler.Add(id, l:handler); return id;
		case "unload": m_handler.Add(id, u:handler); return id;
		case "enterFrame": m_handler.Add(id, e:handler); return id;
		case "update": m_handler.Add(id, up:handler); return id;
		case "render": m_handler.Add(id, r:handler); return id;
		case "press": m_handler.Add(id, p:handler); return id;
		case "release": m_handler.Add(id, rl:handler); return id;
		case "rollOver": m_handler.Add(id, rOver:handler); return id;
		case "rollOut": m_handler.Add(id, rOut:handler); return id;
		default: return -1;
		}
	}

	public int AddEventHandler(string eventName, ButtonKeyPressHandler handler)
	{
		int id = -1;
		if (string.Compare(eventName, "keyPress") == 0) {
			id = m_lwf.GetEventOffset();
			m_handler.Add(id, k:handler);
		}
		return id;
	}

	public void RemoveEventHandler(string eventName, int id)
	{
		switch (eventName) {
		case "load":
		case "unload":
		case "enterFrame":
		case "update":
		case "render":
		case "press":
		case "release":
		case "rollOver":
		case "rollOut":
		case "keyPress":
			m_handler.Remove(id);
			break;
		}
	}

	public void ClearEventHandler(string eventName)
	{
		switch (eventName) {
		case "load": m_handler.Clear(EventType.LOAD); break;
		case "unload": m_handler.Clear(EventType.UNLOAD); break;
		case "enterFrame": m_handler.Clear(EventType.ENTERFRAME); break;
		case "update": m_handler.Clear(EventType.UPDATE); break;
		case "render": m_handler.Clear(EventType.RENDER); break;
		case "press": m_handler.Clear(EventType.PRESS); break;
		case "release": m_handler.Clear(EventType.RELEASE); break;
		case "rollOver": m_handler.Clear(EventType.ROLLOVER); break;
		case "rollOut": m_handler.Clear(EventType.ROLLOUT); break;
		case "keyPress": m_handler.Clear(EventType.KEYPRESS); break;
		}
	}
}

}	// namespace LWF
