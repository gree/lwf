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
	public void Play()
	{
		m_playing = true;
	}

	public void Stop()
	{
		m_playing = false;
	}

	public void GotoNextFrame()
	{
		m_jumped = true;
		Stop();
		++m_currentFrameInternal;
	}

	public void GotoPrevFrame()
	{
		m_jumped = true;
		Stop();
		--m_currentFrameInternal;
	}

	public void GotoFrame(int frameNo)
	{
		GotoFrameInternal(frameNo - 1);
	}

	public void GotoFrameInternal(int frameNo)
	{
		m_jumped = true;
		Stop();
		m_currentFrameInternal = frameNo;
	}

	public void SetVisible(bool visible)
	{
		m_visible = visible;
	}

	public void GotoLabel(string label)
	{
		GotoLabel(m_lwf.GetStringId(label));
	}

	public void GotoLabel(int stringId)
	{
		GotoFrame(m_lwf.SearchFrame(this, stringId));
	}

	public void GotoAndStop(string label)
	{
		GotoFrame(m_lwf.SearchFrame(this, m_lwf.GetStringId(label)));
		Stop();
	}

	public void GotoAndStop(int frameNo)
	{
		GotoFrame(frameNo);
		Stop();
	}

	public void GotoAndPlay(string label)
	{
		GotoFrame(m_lwf.SearchFrame(this, m_lwf.GetStringId(label)));
		Play();
	}

	public void GotoAndPlay(int frameNo)
	{
		GotoFrame(frameNo);
		Play();
	}

	public Movie Move(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.GetMatrix(this);
		m_property.Move(vx, vy);
		return this;
	}

	public Movie MoveTo(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.GetMatrix(this);
		m_property.MoveTo(vx, vy);
		return this;
	}

	public Movie Rotate(float degree)
	{
		if (!m_property.hasMatrix)
			Utility.GetMatrix(this);
		m_property.Rotate(degree);
		return this;
	}

	public Movie RotateTo(float degree)
	{
		if (!m_property.hasMatrix)
			Utility.GetMatrix(this);
		m_property.RotateTo(degree);
		return this;
	}

	public Movie Scale(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.GetMatrix(this);
		m_property.Scale(vx, vy);
		return this;
	}

	public Movie ScaleTo(float vx, float vy)
	{
		if (!m_property.hasMatrix)
			Utility.GetMatrix(this);
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
			Utility.GetColorTransform(this);
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
