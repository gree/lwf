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

namespace LWF {

using ProgramObjectConstructor = Func<ProgramObject, int, int, int, Renderer>;

public class ProgramObject : Object
{
	public ProgramObject(LWF lwf, Movie parent, int objId)
		: base(lwf, parent, Format.Object.Type.PROGRAMOBJECT, objId)
	{
		Format.ProgramObject data = lwf.data.programObjects[objId];
		m_dataMatrixId = data.matrixId;
		ProgramObjectConstructor ctor = lwf.GetProgramObjectConstructor(objId);
		if (ctor != null)
			m_renderer = ctor(this, objId, data.width, data.height);
	}

	public override void Update(Matrix m, ColorTransform c)
	{
		base.Update(m, c);
		if (m_renderer != null)
			m_renderer.Update(m_matrix, m_colorTransform);
	}
}

}	// namespace LWF
