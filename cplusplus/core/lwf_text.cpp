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
#include "lwf_movie.h"
#include "lwf_renderer.h"
#include "lwf_text.h"

namespace LWF {

Text::Text(LWF *lwf, Movie *p, int objId, int instId)
	: Object(lwf, p, Format::Object::TEXT, objId)
{
	const Format::Text &text = lwf->data->texts[objId];
	dataMatrixId = text.matrixId;

	if (text.nameStringId != -1) {
		name = lwf->data->strings[text.nameStringId];
	} else {
		if (instId >= 0 && instId < lwf->data->instanceNames.size()) {
			int stringId = lwf->GetInstanceNameStringId(instId);
			if (stringId != -1)
				name = lwf->data->strings[stringId];
		}
	}

	shared_ptr<TextRenderer> textRenderer =
		lwf->rendererFactory->ConstructText(lwf, objId, this);

	string t;
	if (text.stringId != -1)
		t = lwf->data->strings[text.stringId];

	if (text.nameStringId == -1 && name.empty()) {
		if (text.stringId != -1)
			textRenderer->SetText(t);
	} else {
#if defined(LWF_USE_LUA)
		string lt = lwf->GetTextLua(p, name);
		if (!lt.empty())
			t = lt;
#endif
		lwf->SetTextRenderer(p->GetFullName(), name, t, textRenderer.get());
	}

	renderer = textRenderer;
}

}	// namespace LWF
