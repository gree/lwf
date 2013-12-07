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

#ifndef LWF_EVENTMOVIE_H
#define LWF_EVENTMOVIE_H

#include "lwf_type.h"

namespace LWF {

class Movie;

typedef function<void (Movie *)> MovieEventHandler;
typedef vector<pair<int, MovieEventHandler> > MovieEventHandlerList;
typedef map<string, MovieEventHandler> MovieEventHandlerDictionary;

class MovieEventHandlers
{
public:
	enum Type {
		LOAD,
		POSTLOAD,
		UNLOAD,
		ENTERFRAME,
		UPDATE,
		RENDER,
		EVENTS,
	};

private:
	bool m_empty;
	MovieEventHandlerList m_handlers[EVENTS];

public:
	MovieEventHandlers() : m_empty(true) {};
	bool Empty() const {return m_empty;}
	void Clear();
	void Clear(string type);
	void Add(const MovieEventHandlers *h);
	void Add(int eventId, const MovieEventHandlerDictionary &h);
	bool Add(int evetnId, string type, const MovieEventHandler &h);
	void Remove(int id);
	void Call(Type type, Movie *target);
	bool Call(string type, Movie *target);

private:
	void UpdateEmpty();
};

}	// namespace LWF

#endif
