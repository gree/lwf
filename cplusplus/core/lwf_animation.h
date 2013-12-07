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

#ifndef LWF_ANIMATION_H
#define	LWF_ANIMATION_H

namespace LWF {
namespace Animation {

enum Constnt {
	END = 0,
	PLAY,
	STOP,
	NEXTFRAME,
	PREVFRAME,
	GOTOFRAME,		// FRAMENO(4bytes)
	GOTOLABEL,		// LABELID(4bytes)
	SETTARGET,		// COUNT(1byte) INSTANCEID(4bytes) ...
					// SETTARGET 0           :myself
					// SETTARGET 1 ROOT      :root
					// SETTARGET 1 PARENT    :parent
					// SETTARGET 1 ID        :child
					// SETTARGET 2 PARENT ID :sibling
					// SETTARGET 2 ROOT ID   :root/child
	EVENT,			// EVENTID(4bytes)
	CALL,			// STRINGID(4bytes)

	INSTANCE_TARGET_ROOT = -1,
	INSTANCE_TARGET_PARENT = -2,
};

}	// namespace Animation
}	// namespace LWF

#endif
