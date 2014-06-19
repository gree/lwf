/*
 * Copyright (C) 2014 GREE, Inc.
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

using MovieEventHandler = Action<Movie>;

class HandlerWrapper
{
	public int id;
}

public partial class LWF
{
	public void SetMovieLoadCommand(
		string instanceName, MovieEventHandler handler)
	{
		Movie movie = SearchMovieInstance(instanceName);
		if (movie != null) {
			handler(movie);
		} else {
			HandlerWrapper w = new HandlerWrapper();
			MovieEventHandler h = (m) => {
				RemoveMovieEventHandler(instanceName, w.id);
				handler(m);
			};
			w.id = AddMovieEventHandler(instanceName, load:h);
		}
	}

	public void SetMoviePostLoadCommand(
		string instanceName, MovieEventHandler handler)
	{
		Movie movie = SearchMovieInstance(instanceName);
		if (movie != null) {
			handler(movie);
		} else {
			HandlerWrapper w = new HandlerWrapper();
			MovieEventHandler h = (m) => {
				RemoveMovieEventHandler(instanceName, w.id);
				handler(m);
			};
			w.id = AddMovieEventHandler(instanceName, postLoad:h);
		}
	}

	public void PlayMovie(string instanceName)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.Play();});
	}

	public void StopMovie(string instanceName)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.Stop();});
	}

	public void NextFrameMovie(string instanceName)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.NextFrame();});
	}

	public void PrevFrameMovie(string instanceName)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.PrevFrame();});
	}

	public void SetVisibleMovie(string instanceName, bool visible)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.SetVisible(visible);});
	}

	public void GotoAndStopMovie(string instanceName, string label)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.GotoAndStop(label);});
	}

	public void GotoAndStopMovie(string instanceName, int frameNo)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.GotoAndStop(frameNo);});
	}

	public void GotoAndPlayMovie(string instanceName, string label)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.GotoAndPlay(label);});
	}

	public void GotoAndPlayMovie(string instanceName, int frameNo)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.GotoAndPlay(frameNo);});
	}

	public void MoveMovie(string instanceName, float vx, float vy)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.Move(vx, vy);});
	}

	public void MoveToMovie(string instanceName, float vx, float vy)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.MoveTo(vx, vy);});
	}

	public void RotateMovie(string instanceName, float degree)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.Rotate(degree);});
	}

	public void RotateToMovie(string instanceName, float degree)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.RotateTo(degree);});
	}

	public void ScaleMovie(string instanceName, float vx, float vy)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.Scale(vx, vy);});
	}

	public void ScaleToMovie(string instanceName, float vx, float vy)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.ScaleTo(vx, vy);});
	}

	public void SetAlphaMovie(string instanceName, float v)
	{
		SetMovieLoadCommand(instanceName, (m) => {m.SetAlpha(v);});
	}

	public void SetColorTransformMovie(
		string instanceName, float vr, float vg, float vb, float va)
	{
		SetMovieLoadCommand(instanceName, (m) => {
			ColorTransform c = new ColorTransform(vr, vg, vb, va);
			m.SetColorTransform(c);
		});
	}

	public void SetColorTransformMovie(
		string instanceName, float vr, float vg, float vb, float va,
			float ar, float ag, float ab, float aa)
	{
		SetMovieLoadCommand(instanceName, (m) => {
			ColorTransform c =
				new ColorTransform(vr, vg, vb, va, ar, ag, ab, aa);
			m.SetColorTransform(c);
		});
	}
}

}	// namespace LWF
