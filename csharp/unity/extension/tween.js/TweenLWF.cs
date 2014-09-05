/**
 * TweenLWF.cs is derived from tween.js
 *
 * @author sole / http://soledadpenades.com
 * @author mrdoob / http://mrdoob.com
 * @author Robert Eisele / http://www.xarg.org
 * @author Philippe / http://philippe.elsass.me
 * @author Robert Penner / http://www.robertpenner.com/easing_terms_of_use.html
 * @author Paul Lewis / http://www.aerotwist.com/
 * @author lechecacharro
 * @author Josh Faul / http://jocafa.com/
 * @author egraether / http://egraether.com/
 * @author GREE, Inc.
 *
 * The MIT License
 *
 * Copyright (c) 2010-2012 Tween.js authors.
 * Copyright (c) 2012 GREE, Inc.
 *
 * Easing equations
 *   Copyright (c) 2001 Robert Penner http://robertpenner.com/easing/
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections.Generic;

using Values =
	System.Collections.Generic.Dictionary<TweenLWF.Tween.Property, object>;
using StartCallback = System.Action<LWF.Movie>;
using UpdateCallback = System.Action<LWF.Movie, float>;
using CompleteCallback = System.Action<LWF.Movie>;
using EasingFunction = System.Func<float, float>;
using InterpolationFunction = System.Func<float[], float, float>;
using Tweens = System.Collections.Generic.List<TweenLWF.Tween>;
using T = TweenLWF.Tween.Property;

namespace TweenLWF {

public class Tween
{
	public enum Property {
		X,
		Y,
		ScaleX,
		ScaleY,
		Rotation,
		Alpha,
		Red,
		Green,
		Blue,
	}

	LWF.LWF m_lwf;
	LWF.Movie m_target;
	Values m_valuesStart;
	Values m_valuesEnd;
	double m_duration;
	double m_delayTime;
	double m_startTime;
	EasingFunction m_easingFunction;
	InterpolationFunction m_interpolationFunction;
	Tweens m_chainedTweens;
	StartCallback m_onStartCallback;
	bool m_onStartCallbackFired;
	UpdateCallback m_onUpdateCallback;
	CompleteCallback m_onCompleteCallback;
	bool m_useInterpolation;

	public LWF.Movie target {get {return m_target;}}

	public Tween(LWF.Movie movie)
	{
		m_lwf = movie.lwf;
		m_target = movie;
		m_valuesStart = new Values();
		m_valuesEnd = new Values();
		m_duration = 1;
		m_delayTime = 0;
		m_startTime = 0;
		m_easingFunction = Tween.Easing.Linear.None;
		m_interpolationFunction = Tween.Interpolation.Linear;
		m_chainedTweens = new Tweens();
		m_onStartCallbackFired = false;
		m_useInterpolation = false;

		if (m_lwf.tweens == null) {
			m_lwf.tweens = new Tweens();

			if (m_lwf.tweenMode == LWF.LWF.TweenMode.LWF) {
				m_lwf.AddExecHandler(LWFTween.TweenExecHandler);
			} else {
				m_lwf.tweenEventId = m_lwf.AddMovieEventHandler(
					"_root", enterFrame:LWFTween.TweenMovieHandler);
			}
		}
	}

	public Tween ToValue(T property, float value)
	{
		if (m_valuesEnd == null)
			m_valuesEnd = new Values();
		m_valuesEnd[property] = value;
		return this;
	}

	public Tween ToValue(T property, float[] value)
	{
		if (m_valuesEnd == null)
			m_valuesEnd = new Values();
		m_valuesEnd[property] = value;
		m_useInterpolation = true;
		return this;
	}

	public Tween X(float value)
	{
		return ToValue(T.X, value);
	}

	public Tween X(float[] value)
	{
		return ToValue(T.X, value);
	}

	public Tween Y(float value)
	{
		return ToValue(T.Y, value);
	}

	public Tween Y(float[] value)
	{
		return ToValue(T.Y, value);
	}

	public Tween ScaleX(float value)
	{
		return ToValue(T.ScaleX, value);
	}

	public Tween ScaleX(float[] value)
	{
		return ToValue(T.ScaleX, value);
	}

	public Tween ScaleY(float value)
	{
		return ToValue(T.ScaleY, value);
	}

	public Tween ScaleY(float[] value)
	{
		return ToValue(T.ScaleY, value);
	}

	public Tween Rotation(float value)
	{
		return ToValue(T.Rotation, value);
	}

	public Tween Rotation(float[] value)
	{
		return ToValue(T.Rotation, value);
	}

	public Tween Alpha(float value)
	{
		return ToValue(T.Alpha, value);
	}

	public Tween Alpha(float[] value)
	{
		return ToValue(T.Alpha, value);
	}

	public Tween Red(float value)
	{
		return ToValue(T.Red, value);
	}

	public Tween Red(float[] value)
	{
		return ToValue(T.Red, value);
	}

	public Tween Green(float value)
	{
		return ToValue(T.Green, value);
	}

	public Tween Green(float[] value)
	{
		return ToValue(T.Green, value);
	}

	public Tween Blue(float value)
	{
		return ToValue(T.Blue, value);
	}

	public Tween Blue(float[] value)
	{
		return ToValue(T.Blue, value);
	}

	public Tween Duration(int duration = 0)
	{
		m_duration = duration * m_lwf.tick;
		return this;
	}

	public float Get(T property)
	{
		switch (property) {
		case T.X:
			return m_target.x;
		case T.Y:
			return m_target.y;
		case T.ScaleX:
			return m_target.scaleX;
		case T.ScaleY:
			return m_target.scaleY;
		case T.Rotation:
			return m_target.rotation;
		case T.Alpha:
			return m_target.alpha;
		case T.Red:
			return m_target.red;
		case T.Green:
			return m_target.green;
		case T.Blue:
			return m_target.blue;
		}
		return 0;
	}

	public void Set(T property, float value)
	{
		switch (property) {
		case T.X:
			m_target.x = value;
			break;
		case T.Y:
			m_target.y = value;
			break;
		case T.ScaleX:
			m_target.scaleX = value;
			break;
		case T.ScaleY:
			m_target.scaleY = value;
			break;
		case T.Rotation:
			m_target.rotation = value;
			break;
		case T.Alpha:
			m_target.alpha = value;
			break;
		case T.Red:
			m_target.red = value;
			break;
		case T.Green:
			m_target.green = value;
			break;
		case T.Blue:
			m_target.blue = value;
			break;
		}
	}

	public Tween Start()
	{
		Tweens tweens = (Tweens)m_lwf.tweens;
		tweens.Add(this);

		m_onStartCallbackFired = false;

		m_startTime = m_lwf.time;
		m_startTime += m_delayTime;

		Values orgValues = m_valuesEnd;
		if (m_useInterpolation)
			m_valuesEnd = new Values();
		foreach (KeyValuePair<Property, object> kvp in orgValues) {
			Property property = kvp.Key;
			float value = Get(property);
			object obj = kvp.Value;
			Type type = obj.GetType();
			if (type == typeof(float[])) {
				float[] array = (float[])obj;
				if (array.Length == 0)
					continue;
				float[] newArray = new float[array.Length + 1];
				newArray[0] = value;
				Array.Copy(array, 0, newArray, 1, array.Length);
				m_valuesEnd[property] = newArray;
			} else if (m_useInterpolation) {
				m_valuesEnd[property] = obj;
			}

			m_valuesStart[property] = value;
		}

		return this;
	}

	public Tween Stop()
	{
		Tweens tweens = (Tweens)m_lwf.tweens;
		int i = tweens.IndexOf(this);

		if (i != -1) {
			tweens.RemoveAt(i);

			if (tweens.Count == 0)
				m_lwf.StopTweens();
		}

		return this;
	}

	public Tween Delay(int amount)
	{
		m_delayTime = (double)amount * m_lwf.tick;
		return this;
	}

	public Tween SetEasing(EasingFunction easing)
	{
		m_easingFunction = easing;
		return this;
	}

	public Tween SetInterpolation(InterpolationFunction interpolation)
	{
		m_interpolationFunction = interpolation;
		return this;
	}

	public Tween Chain(Tween chainedTween = null)
	{
		if (chainedTween != null) {
			m_chainedTweens.Add(chainedTween);
			return this;
		} else {
			chainedTween = new Tween(m_target);
			m_chainedTweens.Add(chainedTween);
			return chainedTween;
		}
	}

	public Tween OnStart(StartCallback onStartCallback)
	{
		m_onStartCallback = onStartCallback;
		return this;
	}

	public Tween OnUpdate(UpdateCallback onUpdateCallback)
	{
		m_onUpdateCallback = onUpdateCallback;
		return this;
	}

	public Tween OnComplete(CompleteCallback onCompleteCallback)
	{
		m_onCompleteCallback = onCompleteCallback;
		return this;
	}

	public bool Update(double time)
	{
		if (time < m_startTime)
			return true;

		if (!m_onStartCallbackFired) {
			if (m_onStartCallback != null)
				m_onStartCallback(m_target);
			m_onStartCallbackFired = true;
		}

		float elapsed = (float)((time - m_startTime) / m_duration);
		elapsed = elapsed > 1 ? 1 : elapsed;

		float value = m_easingFunction(elapsed);

		foreach (KeyValuePair<Property, object> kvp in m_valuesStart) {
			Property property = kvp.Key;
			object oStart = kvp.Value;
			object oEnd = m_valuesEnd[property];

			if (oEnd.GetType() == typeof(float[])) {
				float[] end = (float[])oEnd;
				Set(property, m_interpolationFunction(end, value));
			} else {
				float start = (float)oStart;
				float end = (float)oEnd;
				Set(property, start + (end - start) * value);
			}
		}

		if (m_onUpdateCallback != null)
			m_onUpdateCallback(m_target, value);

		if (elapsed == 1) {
			if (m_onCompleteCallback != null)
				m_onCompleteCallback(m_target);

			foreach (var t in m_chainedTweens)
				t.Start();

			return false;
		}

		return true;
	}

	public class Easing
	{
		public class Linear
		{
			public static float None(float k)
			{
				return k;
			}
		}

		public class Quadratic
		{
			public static float In(float k)
			{
				return k * k;
			}

			public static float Out(float k)
			{
				return k * (2 - k);
			}

			public static float InOut(float k)
			{
				if ((k *= 2) < 1)
					return 0.5f * k * k;
				return - 0.5f * (--k * (k - 2) - 1);
			}
		}

		public class Cubic
		{
			public static float In(float k)
			{
				return k * k * k;
			}

			public static float Out(float k)
			{
				return --k * k * k + 1;
			}

			public static float InOut(float k)
			{
				if ((k *= 2) < 1)
					return 0.5f * k * k * k;
				return 0.5f * ((k -= 2) * k * k + 2);
			}
		}

		public class Quartic
		{
			public static float In(float k)
			{
				return k * k * k * k;
			}

			public static float Out(float k)
			{
				return 1 - (--k * k * k * k);
			}

			public static float InOut(float k)
			{
				if ((k *= 2) < 1)
					return 0.5f * k * k * k * k;
				return - 0.5f * ((k -= 2) * k * k * k - 2);
			}
		}

		public class Quintic
		{
			public static float In(float k)
			{
				return k * k * k * k * k;
			}

			public static float Out(float k)
			{
				return --k * k * k * k * k + 1;
			}

			public static float InOut(float k)
			{
				if ((k *= 2) < 1)
					return 0.5f * k * k * k * k * k;
				return 0.5f * ((k -= 2) * k * k * k * k + 2);
			}
		}

		public class Sinusoidal
		{
			public static float In(float k)
			{
				return 1 - Mathf.Cos(k * Mathf.PI / 2);
			}

			public static float Out(float k)
			{
				return Mathf.Sin(k * Mathf.PI / 2);
			}

			public static float InOut(float k)
			{
				return 0.5f * (1 - Mathf.Cos(Mathf.PI * k));
			}
		}

		public class Exponential
		{
			public static float In(float k)
			{
				return k == 0 ? 0 : Mathf.Pow(1024, k - 1);
			}

			public static float Out(float k)
			{
				return k == 1 ? 1 : 1 - Mathf.Pow(2, - 10 * k);
			}

			public static float InOut(float k)
			{
				if (k == 0)
					return 0;
				if (k == 1)
					return 1;
				if ((k *= 2) < 1)
					return 0.5f * Mathf.Pow(1024, k - 1);
				return 0.5f * (- Mathf.Pow(2, - 10 * (k - 1)) + 2);
			}
		}

		public class Circular
		{
			public static float In(float k)
			{
				return 1 - Mathf.Sqrt(1 - k * k);
			}

			public static float Out(float k)
			{
				return Mathf.Sqrt(1 - (--k * k));
			}

			public static float InOut(float k)
			{
				if ((k *= 2) < 1)
					return - 0.5f * (Mathf.Sqrt(1 - k * k) - 1);
				return 0.5f * (Mathf.Sqrt(1 - (k -= 2) * k) + 1);
			}
		}

		public class Elastic
		{
			public static float In(float k)
			{
				float s;
				float a = 0.1f;
				float p = 0.4f;
				if (k == 0)
					return 0;
				if (k == 1)
					return 1;
				if (true) {//!a || a < 1 ) {
					a = 1;
					s = p / 4;
				}/* else
					s = p * Mathf.Asin(1 / a) / (2 * Mathf.PI);*/
				return - (a * Mathf.Pow(2, 10 * (k -= 1)) *
					Mathf.Sin((k - s) * (2 * Mathf.PI) / p));
			}

			public static float Out(float k)
			{
				float s;
				float a = 0.1f;
				float p = 0.4f;
				if (k == 0)
					return 0;
				if (k == 1)
					return 1;
				if (true) {//!a || a < 1 ) {
					a = 1;
					s = p / 4;
				}/* else
					s = p * Mathf.Asin(1 / a) / (2 * Mathf.PI);*/
				return (a * Mathf.Pow(2, - 10 * k) *
					Mathf.Sin((k - s) * (2 * Mathf.PI) / p) + 1);
			}

			public static float InOut(float k)
			{
				float s;
				float a = 0.1f;
				float p = 0.4f;
				if (k == 0)
					return 0;
				if (k == 1)
					return 1;
				if (true) {//!a || a < 1 ) {
					a = 1;
					s = p / 4;
				}/* else
					s = p * Mathf.Asin(1 / a) / (2 * Mathf.PI);*/
				if ((k *= 2) < 1)
					return - 0.5f * (a * Mathf.Pow(2, 10 * (k -= 1)) *
						Mathf.Sin((k - s) * (2 * Mathf.PI) / p));
				return a * Mathf.Pow(2, -10 * (k -= 1)) *
					Mathf.Sin((k - s) * (2 * Mathf.PI) / p) * 0.5f + 1;
			}
		}

		public class Back
		{
			public static float In(float k)
			{
				float s = 1.70158f;
				return k * k * ((s + 1) * k - s);
			}

			public static float Out(float k)
			{
				float s = 1.70158f;
				return --k * k * ((s + 1) * k + s) + 1;
			}

			public static float InOut(float k)
			{
				float s = 1.70158f * 1.525f;
				if ((k *= 2) < 1)
					return 0.5f * (k * k * ((s + 1) * k - s));
				return 0.5f * ((k -= 2) * k * ((s + 1) * k + s) + 2);
			}
		}

		public class Bounce
		{
			public static float In(float k)
			{
				return 1 - Out(1 - k);

			}

			public static float Out(float k)
			{
				if (k < (1 / 2.75f))
					return 7.5625f * k * k;
				else if (k < (2 / 2.75f))
					return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
				else if (k < (2.5f / 2.75f))
					return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
				else
					return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
			}

			public static float InOut(float k)
			{
				if (k < 0.5f)
					return In(k * 2) * 0.5f;
				return Out(k * 2 - 1) * 0.5f + 0.5f;
			}
		}
	}

	public class Interpolation
	{
		public static float Linear(float[] v, float k)
		{
			int m = v.Length - 1;
			float f = m * k;
			int i = (int)Mathf.Floor(f);

			if (k < 0)
				return Utils.Linear(v[0], v[1], f);
			if (k > 1)
				return Utils.Linear(v[m], v[m - 1], m - f);

			return Utils.Linear(v[i], v[i + 1 > m ? m : i + 1], f - i);
		}

		public static float Bezier(float[] v, float k)
		{
			float b = 0;
			int n = v.Length - 1;

			for (int i = 0; i <= n; ++i) {
				b += Mathf.Pow(1 - k, n - i) *
					Mathf.Pow(k, i) * v[i] * Utils.Bernstein(n, i);
			}

			return b;
		}

		public static float CatmullRom(float[] v, float k)
		{
			int m = v.Length - 1;
			float f = m * k;
			int i = (int)Mathf.Floor(f);

			if (v[0] == v[m]) {
				if (k < 0)
					i = (int)Mathf.Floor(f = m * (1 + k));

				return Utils.CatmullRom(v[(i - 1 + m) % m],
					v[i], v[(i + 1) % m], v[(i + 2) % m], f - i);
			} else {
				if (k < 0)
					return v[0] -
						(Utils.CatmullRom(v[0], v[0], v[1], v[1], -f) - v[0]);
				if (k > 1)
					return v[m] - (Utils.CatmullRom(v[m],
						v[m], v[m - 1], v[m - 1], f - m) - v[m]);

				return Utils.CatmullRom(v[i != 0 ? i - 1 : 0], v[i],
					v[m < i + 1 ? m : i + 1], v[m < i + 2 ? m : i + 2], f - i);
			}
		}

		public class Utils
		{
			static List<float> a = new List<float>(){1};

			public static float Linear(float p0, float p1, float t)
			{
				return (p1 - p0) * t + p0;
			}

			public static float Bernstein(int n, int i)
			{
				return Factorial(n) / Factorial(i) / Factorial(n - i);
			}

			public static float Factorial(int n)
			{
				float s = 1;
				if (a.Count > n)
					return a[n];
				for (int i = n; i > 1; --i) {
					s *= i;
					a[i] = s;
				}
				return a[n];
			}

			public static float CatmullRom(
				float p0, float p1, float p2, float p3, float t)
			{
				float v0 = (p2 - p0) * 0.5f;
				float v1 = (p3 - p1) * 0.5f;
				float t2 = t * t;
				float t3 = t * t2;
				return (2 * p1 - 2 * p2 + v0 + v1) * t3 +
					(- 3 * p1 + 3 * p2 - 2 * v0 - v1) * t2 + v0 * t + p1;
			}
		}
	}
}

public static class LWFTween
{
	public static void TweenUpdater(LWF.LWF lwf)
	{
		int i = 0;
		Tweens tweens = (Tweens)lwf.tweens;
		int num_tweens = tweens.Count;
		double time = lwf.time;
	
		while (i < num_tweens) {
			if (tweens[i].Update(time)) {
				++i;
			} else {
				tweens.RemoveAt(i);
				--num_tweens;
			}
		}

		if (tweens.Count == 0)
			lwf.StopTweens();
	}

	public static void TweenExecHandler(LWF.LWF lwf)
	{
		TweenUpdater(lwf);
	}

	public static void TweenMovieHandler(LWF.Movie movie)
	{
		TweenUpdater(movie.lwf);
	}

	public static void StopTweens(this LWF.LWF lwf)
	{
		if (lwf.tweens != null) {
			lwf.tweens = null;

			lwf.RemoveExecHandler(LWFTween.TweenExecHandler);
			lwf.RemoveMovieEventHandler("_root", lwf.tweenEventId);
		}
	}

	public static Tween AddTween(this LWF.Movie movie)
	{
		Tween tween = new Tween(movie);
		return tween;
	}

	public static LWF.Movie StopTweens(this LWF.Movie movie)
	{
		if (movie.lwf.tweens == null)
			return movie;

		Tweens tweens = (Tweens)movie.lwf.tweens;

		int i = 0;
		int num_tweens = tweens.Count;

		while (i < num_tweens) {
			if (tweens[i].target == movie) {
				tweens.RemoveAt(i);
				--num_tweens;
			} else {
				++i;
			}
		}

		if (tweens.Count == 0)
			movie.lwf.StopTweens();

		return movie;
	}
}

}
