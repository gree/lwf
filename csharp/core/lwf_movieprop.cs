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
			if (m_property.hasMatrix)
				return m_property.matrix.translateX;
			else
				return Utility.GetX(this);
		}
		set {
			if (!m_property.hasMatrix)
				Utility.SyncMatrix(this);
			m_property.MoveTo(value, m_property.matrix.translateY);
		}
	}

	public float y {
		get {
			if (m_property.hasMatrix)
				return m_property.matrix.translateY;
			else
				return Utility.GetY(this);
		}
		set {
			if (!m_property.hasMatrix)
				Utility.SyncMatrix(this);
			m_property.MoveTo(m_property.matrix.translateX, value);
		}
	}

	public float scaleX {
		get {
			if (m_property.hasMatrix)
				return m_property.m_scaleX;
			else
				return Utility.GetScaleX(this);
		}
		set {
			if (!m_property.hasMatrix)
				Utility.SyncMatrix(this);
			m_property.ScaleTo(value, m_property.m_scaleY);
		}
	}

	public float scaleY {
		get {
			if (m_property.hasMatrix)
				return m_property.m_scaleY;
			else
				return Utility.GetScaleY(this);
		}
		set {
			if (!m_property.hasMatrix)
				Utility.SyncMatrix(this);
			m_property.ScaleTo(m_property.m_scaleX, value);
		}
	}

	public float rotation {
		get {
			if (m_property.hasMatrix)
				return m_property.m_rotation;
			else
				return Utility.GetRotation(this);
		}
		set {
			if (!m_property.hasMatrix)
				Utility.SyncMatrix(this);
			m_property.RotateTo(value);
		}
	}

	public float alpha {
		get {
			if (m_property.hasColorTransform)
				return m_property.colorTransform.multi.alpha;
			else
				return Utility.GetAlpha(this);
		}
		set {
			if (!m_property.hasColorTransform)
				Utility.SyncColorTransform(this);
			m_property.SetAlpha(value);
		}
	}

	public float red {
		get {
			if (m_property.hasColorTransform)
				return m_property.colorTransform.multi.red;
			else
				return Utility.GetRed(this);
		}
		set {
			if (!m_property.hasColorTransform)
				Utility.SyncColorTransform(this);
			m_property.SetRed(value);
		}
	}

	public float green {
		get {
			if (m_property.hasColorTransform)
				return m_property.colorTransform.multi.green;
			else
				return Utility.GetGreen(this);
		}
		set {
			if (!m_property.hasColorTransform)
				Utility.SyncColorTransform(this);
			m_property.SetGreen(value);
		}
	}

	public float blue {
		get {
			if (m_property.hasColorTransform)
				return m_property.colorTransform.multi.blue;
			else
				return Utility.GetBlue(this);
		}
		set {
			if (!m_property.hasColorTransform)
				Utility.SyncColorTransform(this);
			m_property.SetBlue(value);
		}
	}
}

}	// namespace LWF
