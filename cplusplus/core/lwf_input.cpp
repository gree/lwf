/*
 * Copyright (C) 2013 GREE, Inc.
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

#include "lwf_core.h"
#include "lwf_button.h"

namespace LWF {

Button *LWF::InputPoint(int px, int py)
{
	intercepted = false;

	if (!interactive)
		return 0;

	float x = px;
	float y = py;

	pointX = x;
	pointY = y;

	bool found = false;
	for (Button *button = buttonHead; button; button = button->buttonLink) {
		if (button->CheckHit(x, y)) {
			if (!m_allowButtonList.empty()) {
				if (m_allowButtonList.find(button->instanceId) ==
						m_allowButtonList.end()) {
					if (interceptByNotAllowOrDenyButtons) {
						intercepted = true;
						break;
					} else {
						continue;
					}
				}
			} else if (!m_denyButtonList.empty()) {
				if (m_denyButtonList.find(button->instanceId) !=
						m_denyButtonList.end()) {
					if (interceptByNotAllowOrDenyButtons) {
						intercepted = true;
						break;
					} else {
						continue;
					}
				}
			}

			found = true;
			if (focus != button) {
				if (focus)
					focus->RollOut();
				focus = button;
				focus->RollOver();
			}
			break;
		}
	}
	if (!found && focus) {
		focus->RollOut();
		focus = 0;
	}

	return focus;
}

void LWF::InputPress()
{
	if (!interactive)
		return;

	pressing = true;

	if (focus) {
		pressed = focus;
		focus->Press();
	}
}

void LWF::InputRelease()
{
	if (!interactive)
		return;

	pressing = false;

	if (focus && pressed == focus) {
		focus->Release();
		pressed = 0;
	}
}

void LWF::InputKeyPress(int code)
{
	if (!interactive)
		return;

	for (Button *button = buttonHead; button; button = button->buttonLink) {
		button->KeyPress(code);
	}
}

}	// namespace LWF
