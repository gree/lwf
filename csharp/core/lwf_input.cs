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

namespace LWF {

public partial class LWF
{
	public Button InputPoint(int px, int py)
	{
		m_intercepted = false;

		if (!interactive)
			return null;

		float x = px;
		float y = py;

		m_pointX = x;
		m_pointY = y;

		bool found = false;
		for (Button button = m_buttonHead;
				button != null; button = button.buttonLink) {
			if (button.CheckHit(x, y)) {
				if (m_allowButtonList != null) {
					bool v;
					if (!m_allowButtonList.TryGetValue(
							button.instanceId, out v)) {
						if (m_interceptByNotAllowOrDenyButtons) {
							m_intercepted = true;
							break;
						} else {
							continue;
						}
					}
				} else if (m_denyButtonList != null) {
					bool v;
					if (m_denyButtonList.TryGetValue(
							button.instanceId, out v)) {
						if (m_interceptByNotAllowOrDenyButtons) {
							m_intercepted = true;
							break;
						} else {
							continue;
						}
					}
				}

				found = true;
				if (m_focus != button) {
					if (m_focus != null)
						m_focus.RollOut();
					m_focus = button;
					m_focus.RollOver();
				}
				break;
			}
		}
		if (!found && m_focus != null) {
			m_focus.RollOut();
			m_focus = null;
		}

		return m_focus;
	}

	public void InputPress()
	{
		if (!interactive)
			return;

		m_pressing = true;

		if (m_focus != null) {
			m_pressed = m_focus;
			m_focus.Press();
		}
	}

	public void InputRelease()
	{
		if (!interactive)
			return;

		m_pressing = false;

		if (m_focus != null && m_pressed == m_focus) {
			m_focus.Release();
			m_pressed = null;
		}
	}

	public void InputKeyPress(int code)
	{
		if (!interactive)
			return;

		for (Button button = m_buttonHead;
				button != null; button = button.buttonLink) {
			button.KeyPress(code);
		}
	}
}

}	// namespace LWF
