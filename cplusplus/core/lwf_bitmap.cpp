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
#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_renderer.h"

namespace LWF {

Bitmap::Bitmap(LWF *lwf, Movie *p, int objId)
	: Object(lwf, p, Format::Object::BITMAP, objId)
{
	dataMatrixId = lwf->data->bitmaps[objId].matrixId;
	renderer = lwf->rendererFactory->ConstructBitmap(lwf, objId, this);
}

}	// namespace LWF
