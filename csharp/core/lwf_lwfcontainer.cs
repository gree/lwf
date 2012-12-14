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

public class LWFContainer : Button
{
	private LWF m_child;

	public LWF child {get {return m_child;}}

	public LWFContainer(Movie parent, LWF child)
	{
		m_lwf = parent.lwf;
		m_parent = parent;
		m_child = child;
	}

	public override bool CheckHit(float px, float py)
	{
		Button button = m_child.InputPoint((int)px, (int)py);
		return button != null ? true : false;
	}

	public override void RollOver()
	{
		// NOTHING TO DO
	}

	public override void RollOut()
	{
		if (m_child.focus != null) {
			m_child.focus.RollOut();
			m_child.ClearFocus(m_child.focus);
		}
	}

	public override void Press()
	{
		m_child.InputPress();
	}

	public override void Release()
	{
		m_child.InputRelease();
	}

	public override void KeyPress(int code)
	{
		m_child.InputKeyPress(code);
	}
}

}	// namespace LWF
