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

#include "lwf_bitmap.h"
#include "lwf_bitmapex.h"
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_format.h"
#include "lwf_graphic.h"
#include "lwf_text.h"

namespace LWF {

typedef Format::GraphicObject GType;

Graphic::Graphic(LWF *l, Movie *p, int objId)
	: Object(l, p, Format::Object::GRAPHIC, objId)
{
	const Format::Graphic &data = lwf->data->graphics[objId];
	int n = data.graphicObjects;
	m_displayList.resize(n);

	const vector<Format::GraphicObject> &graphicObjects =
		lwf->data->graphicObjects;
	for (int i = 0; i < n; ++i) {
		const Format::GraphicObject &gobj =
			graphicObjects[data.graphicObjectId + i];
		shared_ptr<Object> obj;
		int graphicObjectId = gobj.graphicObjectId;

		// Ignore error
		if (graphicObjectId == -1)
			continue;

		switch (gobj.graphicObjectType) {
		case GType::BITMAP:
			obj = make_shared<Bitmap>(lwf, parent, graphicObjectId);
			break;

		case GType::BITMAPEX:
			obj = make_shared<BitmapEx>(lwf, parent, graphicObjectId);
			break;

		case GType::TEXT:
			obj = make_shared<Text>(lwf, parent, graphicObjectId);
			break;
		}

		obj->Exec();
		m_displayList[i] = obj;
	}
}

void Graphic::Update(const Matrix *m, const ColorTransform *c)
{
	DisplayList::iterator it(m_displayList.begin()), itend(m_displayList.end());
	for (; it != itend; ++it)
		(*it)->Update(m, c);
}

void Graphic::Render(bool v, int rOffset)
{
	if (!v)
		return;
	DisplayList::iterator it(m_displayList.begin()), itend(m_displayList.end());
	for (; it != itend; ++it)
		(*it)->Render(v, rOffset);
}

void Graphic::Destroy()
{
	DisplayList::iterator it(m_displayList.begin()), itend(m_displayList.end());
	for (; it != itend; ++it)
		(*it)->Destroy();
	m_displayList.clear();
}

}	// namespace LWF
