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

using EventHandlerDictionary = Dictionary<int, Action<Movie, Button>>;

enum Animation {
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
}

public partial class LWF
{
	public void PlayAnimation(
		int animationId, Movie movie, Button button = null)
	{
		int i = 0;
		int[] animations = m_data.animations[animationId];
		Movie target = movie;

		for (;;) {
			switch ((Animation)animations[i++]) {
			case Animation.END:
				return;

			case Animation.PLAY:
				target.Play();
				break;

			case Animation.STOP:
				target.Stop();
				break;

			case Animation.NEXTFRAME:
				target.NextFrame();
				break;

			case Animation.PREVFRAME:
				target.PrevFrame();
				break;

			case Animation.GOTOFRAME:
				target.GotoFrameInternal(animations[i++]);
				break;

			case Animation.GOTOLABEL:
				target.GotoFrame(SearchFrame(target, animations[i++]));
				break;

			case Animation.SETTARGET:
				{
					target = movie;

					int count = animations[i++];
					if (count == 0)
						break;

					for (int j = 0; j < count; ++j) {
						int instId = animations[i++];

						switch ((Animation)instId) {
						case Animation.INSTANCE_TARGET_ROOT:
							target = m_rootMovie;
							break;

						case Animation.INSTANCE_TARGET_PARENT:
							target = target.parent;
							if (target == null)
								target = m_rootMovie;
							break;

						default:
							{
								target = target.SearchMovieInstanceByInstanceId(
									instId, false);
								if (target == null)
									target = movie;
								break;
							}
						}
					}
				}
				break;

			case Animation.EVENT:
				{
					int eventId = animations[i++];
#if LWF_USE_LUA
					CallEventFunctionLua(eventId, movie, button);
#endif
					if (m_eventHandlers[eventId] != null) {
						var handlers = new EventHandlerDictionary(
							m_eventHandlers[eventId]);
						foreach (var h in handlers)
							h.Value(movie, button);
					}
				}
				break;

			case Animation.CALL:
#if LWF_USE_LUA
				{
					int stringId = animations[i++];
					if (stringId < 0 || stringId >= data.strings.Length)
						break;
					CallFunctionLua(data.strings[stringId], target);
				}
#else
				i++;
#endif
				break;
			}
		}
	}
}

}	// namespace LWF
