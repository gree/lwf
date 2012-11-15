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

public partial class Movie : IObject
{
	public float x {
		get {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			return m_property.matrix.translateX;
		}
		set {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			m_property.MoveTo(value, m_property.matrix.translateY);
		}
	}

	public float y {
		get {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			return m_property.matrix.translateY;
		}
		set {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			m_property.MoveTo(m_property.matrix.translateX, value);
		}
	}

	public float scaleX {
		get {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			return m_property.m_scaleX;
		}
		set {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			m_property.ScaleTo(value, m_property.m_scaleY);
		}
	}

	public float scaleY {
		get {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			return m_property.m_scaleY;
		}
		set {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			m_property.ScaleTo(m_property.m_scaleX, value);
		}
	}

	public float rotation {
		get {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			return m_property.m_rotation;
		}
		set {
			if (!m_property.hasMatrix)
				Utility.GetMatrix(this);
			m_property.RotateTo(value);
		}
	}

	public float alpha {
		get {
			if (!m_property.hasColorTransform)
				Utility.GetColorTransform(this);
			return m_property.colorTransform.multi.alpha;
		}
		set {
			if (!m_property.hasColorTransform)
				Utility.GetColorTransform(this);
			m_property.SetAlpha(value);
		}
	}
}

}	// namespace LWF
