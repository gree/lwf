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

namespace LWF {

public class Particle : Object
{
	public Particle(LWF lwf, Movie parent, int objId)
		: base(lwf, parent, Format.Object.Type.PARTICLE, objId)
	{
		m_dataMatrixId = lwf.data.particles[objId].matrixId;
		m_renderer = lwf.rendererFactory.ConstructParticle(lwf, objId, this);
	}

	public override void Update(Matrix m, ColorTransform c)
	{
		base.Update(m, c);
		if (m_renderer != null)
			m_renderer.Update(m_matrix, m_colorTransform);
	}
}

}	// namespace LWF
