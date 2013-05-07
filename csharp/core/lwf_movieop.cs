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
	public Movie Play()
	{
		m_playing = true;
		return this;
	}

	public Movie Stop()
	{
		m_playing = false;
		return this;
	}

	public Movie NextFrame()
	{
		m_jumped = true;
		Stop();
		++m_currentFrameInternal;
		return this;
	}

	public Movie PrevFrame()
	{
		m_jumped = true;
		Stop();
		--m_currentFrameInternal;
		return this;
	}

	public Movie GotoFrame(int frameNo)
	{
		GotoFrameInternal(frameNo - 1);
		return this;
	}

	public Movie GotoFrameInternal(int frameNo)
	{
		m_jumped = true;
		Stop();
		m_currentFrameInternal = frameNo;
		return this;
	}

	public Movie SetVisible(bool visible)
	{
		m_visible = visible;
		m_lwf.SetPropertyDirty();
		return this;
	}

	public Movie GotoLabel(string label)
	{
		GotoLabel(m_lwf.GetStringId(label));
		return this;
	}

	public Movie GotoLabel(int stringId)
	{
		GotoFrame(m_lwf.SearchFrame(this, stringId));
		return this;
	}

	public Movie GotoAndStop(string label)
	{
		GotoFrame(m_lwf.SearchFrame(this, m_lwf.GetStringId(label)));
		Stop();
		return this;
	}

	public Movie GotoAndStop(int frameNo)
	{
		GotoFrame(frameNo);
		Stop();
		return this;
	}

	public Movie GotoAndPlay(string label)
	{
		GotoFrame(m_lwf.SearchFrame(this, m_lwf.GetStringId(label)));
		Play();
		return this;
	}

	public Movie GotoAndPlay(int frameNo)
	{
		GotoFrame(frameNo);
		Play();
		return this;
	}

	public Movie Move(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.SyncMatrix(this);
		m_property.Move(vx, vy);
		return this;
	}

	public Movie MoveTo(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.SyncMatrix(this);
		m_property.MoveTo(vx, vy);
		return this;
	}

	public Movie Rotate(float degree)
	{
		if (!m_property.hasMatrix)
			Utility.SyncMatrix(this);
		m_property.Rotate(degree);
		return this;
	}

	public Movie RotateTo(float degree)
	{
		if (!m_property.hasMatrix)
			Utility.SyncMatrix(this);
		m_property.RotateTo(degree);
		return this;
	}

	public Movie Scale(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.SyncMatrix(this);
		m_property.Scale(vx, vy);
		return this;
	}

	public Movie ScaleTo(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.SyncMatrix(this);
		m_property.ScaleTo(vx, vy);
		return this;
	}

	public Movie SetMatrix(Matrix m, float sx = 1, float sy = 1, float r = 0)
	{
		m_property.SetMatrix(m, sx, sy, r);
		return this;
	}

	public Movie SetAlpha(float v)
	{
		if (!m_property.hasColorTransform)
			Utility.SyncColorTransform(this);
		m_property.SetAlpha(v);
		return this;
	}

	public Movie SetColorTransform(ColorTransform c)
	{
		m_property.SetColorTransform(c);
		return this;
	}

	public Movie SetRenderingOffset(int rOffset)
	{
		m_property.SetRenderingOffset(rOffset);
		return this;
	}
}

}	// namespace LWF
