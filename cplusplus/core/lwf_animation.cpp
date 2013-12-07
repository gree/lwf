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

#include "lwf_animation.h"
#include "lwf_button.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_movie.h"

namespace LWF {

void LWF::PlayAnimation(int animationId, Movie *movie, Button *button)
{
	int i = 0;
	const vector<int> &animations = data->animations[animationId];
	Movie *target = movie;

	for (;;) {
		switch (animations[i++]) {
		case Animation::END:
			return;

		case Animation::PLAY:
			target->Play();
			break;

		case Animation::STOP:
			target->Stop();
			break;

		case Animation::NEXTFRAME:
			target->NextFrame();
			break;

		case Animation::PREVFRAME:
			target->PrevFrame();
			break;

		case Animation::GOTOFRAME:
			target->GotoFrameInternal(animations[i++]);
			break;

		case Animation::GOTOLABEL:
			target->GotoFrame(SearchFrame(target, animations[i++]));
			break;

		case Animation::SETTARGET:
			{
				target = movie;

				int count = animations[i++];
				if (count == 0)
					break;

				for (int j = 0; j < count; ++j) {
					int instId = animations[i++];

					switch (instId) {
					case Animation::INSTANCE_TARGET_ROOT:
						target = rootMovie.get();
						break;

					case Animation::INSTANCE_TARGET_PARENT:
						target = target->parent;
						if (!target)
							target = rootMovie.get();
						break;

					default:
						{
							target = target->SearchMovieInstanceByInstanceId(
								instId, false);
							if (!target)
								target = movie;
							break;
						}
					}
				}
			}
			break;

		case Animation::EVENT:
			{
				int eventId = animations[i++];
#if defined(LWF_USE_LUA)
				CallEventFunctionLua(eventId, movie, button);
#endif
				EventHandlerList &v(m_eventHandlers[eventId]);
				EventHandlerList::iterator it(v.begin()), itend(v.end());
				for (; it != itend; ++it)
					it->second(movie, button);
			}
			break;

		case Animation::CALL:
#if defined(LWF_USE_LUA)
			{
				int stringId = animations[i++];
				if (stringId < 0 || stringId >= data->strings.size())
					break;
				CallFunctionLua(data->strings[stringId], target);
			}
#else
			i++;
#endif
			break;
		}
	}
}

}	// namespace LWF
