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
#include "lwf_data.h"
#include "lwf_iobject.h"
#include "lwf_movie.h"

namespace LWF {

IObject::IObject(LWF *lwf, Movie *p, int t, int objId, int instId)
	: Object(lwf, p, t, objId)
{
	alive = true;
	instanceId = (instId >= (int)lwf->data->instanceNames.size()) ? -1 : instId;
	iObjectId = lwf->GetIObjectOffset();

	prevInstance = 0;
	nextInstance = 0;
	linkInstance = 0;

	if (instanceId >= 0) {
		int stringId = lwf->GetInstanceNameStringId(instanceId);
		if (stringId != -1)
			name = lwf->data->strings[stringId];

		IObject *head = lwf->GetInstance(instanceId);
		if (head)
			head->prevInstance = this;
		nextInstance = head;
		lwf->SetInstance(instanceId, this);
	}
}

void IObject::Destroy()
{
	if (type != OType::ATTACHEDMOVIE && instanceId >= 0) {
		IObject *head = lwf->GetInstance(instanceId);
		if (head == this)
			lwf->SetInstance(instanceId, nextInstance);
		if (nextInstance)
			nextInstance->prevInstance = prevInstance;
		if (prevInstance)
			prevInstance->nextInstance = nextInstance;
	}

	Object::Destroy();
	alive = false;
}

string IObject::GetFullName() const
{
	string fullPath;
	string splitter;
	for (const IObject *o = this; o; o = o->parent) {
		if (o->name.empty())
			return string();
		fullPath = o->name + splitter + fullPath;
		if (splitter.empty())
			splitter = ".";
	}
	return fullPath;
}

}	// namespace LWF
