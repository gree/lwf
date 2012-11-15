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

using Type = Format.Object.Type;
using EventType = MovieEventHandlers.Type;
using ClipEvent = Format.MovieClipEvent.ClipEvent;
using AttachedMovies = Dictionary<string, Movie>;
using AttachedMovieList = List<Movie>;
using AttachedLWFs = Dictionary<string, LWFContainer>;
using AttachedLWFList = List<LWFContainer>;
using DetachDict = Dictionary<string, bool>;
using Inspector = System.Action<Object, int, int, int>;

public partial class Movie : IObject
{
	private Format.Movie m_data;
	private IObject m_instanceHead;
	private IObject m_instanceTail;
	private Object[] m_displayList;
	private MovieEventHandlers m_handler;
	private AttachedMovies m_attachedMovies;
	private AttachedMovieList m_attachedMovieList;
	private DetachDict m_detachedMovies;
	private AttachedLWFs m_attachedLWFs;
	private AttachedLWFList m_attachedLWFList;
	private DetachDict m_detachedLWFs;
	private string m_attachName;
	private int m_totalFrames;
	private int m_currentFrameInternal;
	private int m_currentFrameCurrent;
	private int m_execedFrame;
	private int m_animationPlayedFrame;
	private int m_depth;
	private bool m_active;
	private bool m_visible;
	private bool m_playing;
	private bool m_jumped;
	private bool m_overriding;
	private bool m_hasButton;
	private bool m_postLoaded;
	private Matrix m_matrix0;
	private Matrix m_matrix1;
	private ColorTransform m_colorTransform0;
	private ColorTransform m_colorTransform1;

	private Property m_property;

	public Movie(LWF lwf, Movie parent, int objId,
			int instId, int matrixId = 0, int colorTransformId = 0,
			bool attached = false, MovieEventHandlers handler = null)
		: base(lwf, parent,
			attached ? Type.ATTACHEDMOVIE : Type.MOVIE, objId, instId)
	{
		m_data = lwf.data.movies[objId];
		m_matrixId = matrixId;
		m_colorTransformId = colorTransformId;
		m_totalFrames = m_data.frames;
		m_instanceHead = null;
		m_instanceTail = null;
		m_currentFrameInternal = -1;
		m_execedFrame = -1;
		m_animationPlayedFrame = -1;
		m_postLoaded = false;
		m_active = true;
		m_visible = true;
		m_playing = true;
		m_jumped = false;
		m_overriding = false;

		m_property = new Property(lwf);

		m_matrix0 = new Matrix();
		m_matrix1 = new Matrix();
		m_colorTransform0 = new ColorTransform();
		m_colorTransform1 = new ColorTransform();

		m_displayList = new Object[m_data.depths];

		PlayAnimation(ClipEvent.LOAD);

		m_handler = (handler != null ?
			handler : lwf.GetMovieEventHandlers(this));
		if (m_handler != null)
			m_handler.Call(EventType.LOAD, this);

		lwf.ExecMovieCommand();
	}

	public Format.Movie data {get {return m_data;}}
	public string attachName {
		get {return m_attachName;}
		set {m_attachName = value;}
	}
	public int depth {
		get {return m_depth;}
		set {m_depth = value;}
	}
	public int currentFrame {get {return m_currentFrameInternal + 1;}}
	public int totalFrames {get {return m_totalFrames;}}
	public bool playing {get {return m_playing;}}
	public bool visible {get {return m_visible;}}

	public void SetHandlers(MovieEventHandlers handler)
	{
		m_handler = handler;
	}

	public Point GlobalToLocal(Point point)
	{
		float px;
		float py;
		Matrix invert = new Matrix();
		Utility.InvertMatrix(invert, m_matrix);
		Utility.CalcMatrixToPoint(out px, out py, point.x, point.y, invert);
		return new Point(px, py);
	}

	public Point LocalToGlobal(Point point)
	{
		float px;
		float py;
		Utility.CalcMatrixToPoint(out px, out py, point.x, point.y, m_matrix);
		return new Point(px, py);
	}

	private void ExecObject(int dlDepth, int objId,
		int matrixId, int colorTransformId, int instId)
	{
		// Ignore error
		if (objId == -1)
			return;
		Data data = m_lwf.data;
		Format.Object dataObject = data.objects[objId];
		int dataObjectId = dataObject.objectId;
		Object obj = m_displayList[dlDepth];

		if (obj != null && (obj.type != (Type)dataObject.objectType ||
				obj.objectId != dataObjectId || (obj.IsMovie() &&
				((IObject)obj).instanceId != instId))) {
			obj.Destroy();
			obj = null;
		}

		if (obj == null) {
			switch ((Type)dataObject.objectType) {
			case Type.BUTTON:
				obj = new Button(m_lwf, this, dataObjectId, instId);
				break;

			case Type.GRAPHIC:
				obj = new Graphic(m_lwf, this, dataObjectId);
				break;

			case Type.MOVIE:
				obj = new Movie(m_lwf, this,
					dataObjectId, instId, matrixId, colorTransformId);
				break;

			case Type.BITMAP:
				obj = new Bitmap(m_lwf, this, dataObjectId);
				break;

			case Type.BITMAPEX:
				obj = new BitmapEx(m_lwf, this, dataObjectId);
				break;

			case Type.TEXT:
				obj = new Text(m_lwf, this, dataObjectId);
				break;

			case Type.PARTICLE:
				obj = new Particle(m_lwf, this, dataObjectId);
				break;

			case Type.PROGRAMOBJECT:
				obj = new ProgramObject(m_lwf, this, dataObjectId);
				break;
			}
		}

		if (obj.type == Type.MOVIE) {
			Movie mobj = (Movie)obj;
			mobj.m_linkInstance = null;
			if (m_instanceHead == null)
				m_instanceHead = mobj;
			else
				m_instanceTail.linkInstance = mobj;
			m_instanceTail = mobj;
		} else if (obj.type == Type.BUTTON) {
			m_hasButton = true;
		}

		m_displayList[dlDepth] = obj;
		obj.execCount = execCount;

		obj.Exec(matrixId, colorTransformId);
	}

	public void Override(bool overriding)
	{
		m_overriding = overriding;
	}

	public void PostExec(bool progressing)
	{
		m_hasButton = false;
		if (!m_active)
			return;

		m_instanceHead = null;
		m_instanceTail = null;
		m_execedFrame = -1;
		if (progressing && m_playing && !m_jumped)
			++m_currentFrameInternal;
		for (;;) {
			if (m_currentFrameInternal < 0 ||
					m_currentFrameInternal >= m_totalFrames)
				m_currentFrameInternal = 0;
			if (m_currentFrameInternal == m_execedFrame)
				break;

			m_instanceHead = null;
			m_instanceTail = null;

			m_currentFrameCurrent = m_currentFrameInternal;
			m_execedFrame = m_currentFrameCurrent;
			Data data = m_lwf.data;
			Format.Frame frame = data.frames[
				m_data.frameOffset + m_currentFrameCurrent];

			int controlAnimationOffset = -1;
			for (int i = 0; i < frame.controls; ++i) {
				Format.Control control =
					data.controls[frame.controlOffset + i];

				switch ((Format.Control.Type)control.controlType) {
				case Format.Control.Type.MOVE:
					{
						Format.Place p =
							data.places[control.controlId];
						ExecObject(p.depth, p.objectId,
							p.matrixId, 0, p.instanceId);
					}
					break;

				case Format.Control.Type.MOVEM:
					{
						Format.ControlMoveM ctrl =
							data.controlMoveMs[control.controlId];
						Format.Place p = data.places[ctrl.placeId];
						ExecObject(p.depth, p.objectId,
							ctrl.matrixId, 0, p.instanceId);
					}
					break;

				case Format.Control.Type.MOVEC:
					{
						Format.ControlMoveC ctrl =
							data.controlMoveCs[control.controlId];
						Format.Place p = data.places[ctrl.placeId];
						ExecObject(p.depth, p.objectId,
							p.matrixId, ctrl.colorTransformId, p.instanceId);
					}
					break;

				case Format.Control.Type.MOVEMC:
					{
						Format.ControlMoveMC ctrl =
							data.controlMoveMCs[control.controlId];
						Format.Place p = data.places[ctrl.placeId];
						ExecObject(p.depth, p.objectId,
							ctrl.matrixId, ctrl.colorTransformId, p.instanceId);
					}
					break;

				case Format.Control.Type.ANIMATION:
					if (controlAnimationOffset == -1)
						controlAnimationOffset = i;
					break;
				}
			}

			for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
				Object obj = m_displayList[dlDepth];
				if (obj != null) {
					if (obj.execCount != execCount) {
						obj.Destroy();
						m_displayList[dlDepth] = null;
					}
				}
			}

			if (m_attachedMovies != null) {
				foreach (Movie movie in m_attachedMovieList)
					if (movie != null)
						movie.Exec();
			}

			IObject instance = m_instanceHead;
			while (instance != null) {
				if (instance.IsMovie()) {
					Movie movie = (Movie)instance;
					movie.PostExec(progressing);
					if (!m_hasButton && movie.m_hasButton)
						m_hasButton = true;
				}
				instance = instance.linkInstance;
			}

			if (m_attachedMovies != null) {
				foreach (KeyValuePair<string, bool> kvp in m_detachedMovies) {
					string attachName = kvp.Key;
					Movie movie;
					if (m_attachedMovies.TryGetValue(attachName, out movie))
						DeleteAttachedMovie(this, movie, true, false);
				}
				m_detachedMovies.Clear();
				foreach (Movie movie in m_attachedMovieList) {
					if (movie != null) {
						movie.PostExec(progressing);
						if (!m_hasButton && movie.m_hasButton)
							m_hasButton = true;
					}
				}
			}

			if (!m_postLoaded) {
				m_postLoaded = true;
				if (m_handler != null)
					m_handler.Call(EventType.POSTLOAD, this);
			}

			if (controlAnimationOffset != -1 &&
					m_execedFrame == m_currentFrameInternal) {
				bool animationPlayed = m_animationPlayedFrame ==
					m_currentFrameCurrent && !m_jumped;
				if (!animationPlayed) {
					for (int i = controlAnimationOffset;
							i < frame.controls; ++i) {
						Format.Control control =
							data.controls[frame.controlOffset + i];
						m_lwf.PlayAnimation(control.controlId, this);
					}
				}
			}

			m_animationPlayedFrame = m_currentFrameCurrent;
			if (m_currentFrameCurrent == m_currentFrameInternal)
				m_jumped = false;
		}

		PlayAnimation(ClipEvent.ENTERFRAME);
		if (m_handler != null)
			m_handler.Call(EventType.ENTERFRAME, this);
	}

	public override void Update(Matrix m, ColorTransform c)
	{
		if (!m_active)
			return;

		if (m_overriding) {
			Utility.CopyMatrix(m_matrix, m_lwf.rootMovie.matrix);
			Utility.CopyColorTransform(
				m_colorTransform, m_lwf.rootMovie.colorTransform);
		} else {
			Utility.CopyMatrix(m_matrix, m);
			Utility.CopyColorTransform(m_colorTransform, c);
		}

		if (m_handler != null)
			m_handler.Call(EventType.UPDATE, this);

		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null) {
				Matrix objm = m_matrix0;
				bool objHasOwnMatrix =
					obj.type == Type.MOVIE && ((Movie)obj).m_property.hasMatrix;
				if (m_property.hasMatrix) {
					if (objHasOwnMatrix) {
						Utility.CalcMatrix(objm, m_matrix, m_property.matrix);
					} else {
						Utility.CalcMatrix(
							m_matrix1, m_matrix, m_property.matrix);
						Utility.CalcMatrix(
							m_lwf, objm, m_matrix1, obj.matrixId);
					}
				} else {
					if (objHasOwnMatrix) {
						Utility.CopyMatrix(objm, m_matrix);
					} else {
						Utility.CalcMatrix(m_lwf, objm, m_matrix, obj.matrixId);
					}
				}

				ColorTransform objc = m_colorTransform0;
				bool objHasOwnColorTransform = obj.type == Type.MOVIE &&
					((Movie)obj).m_property.hasColorTransform;
				if (m_property.hasColorTransform) {
					if (objHasOwnColorTransform) {
						Utility.CalcColorTransform(objc,
							m_colorTransform, m_property.colorTransform);
					} else {
						Utility.CalcColorTransform(m_colorTransform1,
							m_colorTransform, m_property.colorTransform);
						Utility.CalcColorTransform(m_lwf,
							objc, m_colorTransform1, obj.colorTransformId);
					}
				} else {
					if (objHasOwnColorTransform) {
						Utility.CopyColorTransform(objc, m_colorTransform);
					} else {
						Utility.CalcColorTransform(m_lwf,
							objc, m_colorTransform, obj.colorTransformId);
					}
				}

				obj.Update(objm, objc);
			}
		}

		if (m_attachedMovies != null || m_attachedLWFs != null) {
			m = m_matrix;
			if (m_property.hasMatrix) {
				Matrix m1 = m_matrix1.Set(m);
				Utility.CalcMatrix(m, m1, m_property.matrix);
			}

			c = m_colorTransform;
			if (m_property.hasColorTransform) {
				ColorTransform c1 = m_colorTransform1.Set(c);
				Utility.CalcColorTransform(c, c1, m_property.colorTransform);
			}

			if (m_attachedMovies != null) {
				foreach (Movie movie in m_attachedMovieList)
					if (movie != null)
						movie.Update(m, c);
			}

			if (m_attachedLWFs != null) {
				foreach (KeyValuePair<string, bool> kvp in m_detachedLWFs) {
					string attachName = kvp.Key;
					LWFContainer lwfContainer;
					if (m_attachedLWFs.TryGetValue(
							attachName, out lwfContainer))
						DeleteAttachedLWF(this, lwfContainer, true, false);
				}
				m_detachedLWFs.Clear();
				foreach (LWFContainer lwfContainer in m_attachedLWFList) {
					m_lwf.RenderObject(
						lwfContainer.child.Exec(m_lwf.thisTick, m, c));
				}
			}
		}
	}

	public override void LinkButton()
	{
		if (!m_visible || !m_active)
			return;

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList)
				if (lwfContainer != null)
					lwfContainer.LinkButton();
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList)
				if (movie != null && movie.m_hasButton)
					movie.LinkButton();
		}

		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null) {
				if (obj.type == Type.BUTTON) {
					((Button)obj).LinkButton();
				} else if (obj.type == Type.MOVIE) {
					Movie movie = (Movie)obj;
					if (movie.m_hasButton)
						movie.LinkButton();
				}
			}
		}
	}

	public override void Render(bool v, int rOffset)
	{
		if (!m_visible || !m_active)
			v = false;

		if (v && m_handler != null)
			m_handler.Call(EventType.RENDER, this);

		if (m_property.hasRenderingOffset) {
			m_lwf.RenderOffset();
			rOffset = m_property.renderingOffset;
		}
		if (rOffset == Int32.MinValue)
			m_lwf.ClearRenderOffset();

		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null)
				obj.Render(v, rOffset);
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList)
				if (movie != null)
					movie.Render(v, rOffset);
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList) {
				if (lwfContainer != null) {
					LWF child = lwfContainer.child;
					child.SetAttachVisible(v);
					m_lwf.RenderObject(child.Render(m_lwf.renderingIndex,
						m_lwf.renderingCount, rOffset));
				}
			}
		}
	}

	public override void Inspect(
		Inspector inspector, int hierarchy, int inspectDepth, int rOffset)
	{
		if (m_property.hasRenderingOffset) {
			m_lwf.RenderOffset();
			rOffset = m_property.renderingOffset;
		}
		if (rOffset == Int32.MinValue)
			m_lwf.ClearRenderOffset();

		inspector(this, hierarchy, inspectDepth, rOffset);

		++hierarchy;

		int d;
		for (d = 0; d < m_data.depths; ++d) {
			Object obj = m_displayList[d];
			if (obj != null)
				obj.Inspect(inspector, hierarchy, d, rOffset);
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList)
				if (movie != null)
					movie.Inspect(inspector, hierarchy, d++, rOffset);
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList) {
				if (lwfContainer != null) {
					LWF child = lwfContainer.child;
					m_lwf.RenderObject(
						child.Inspect(inspector, hierarchy, d++, rOffset));
				}
			}
		}
	}

	public override void Destroy()
	{
		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null)
				obj.Destroy();
		}

		if (m_attachedMovies != null) {
			foreach (KeyValuePair<string, Movie> kvp in m_attachedMovies)
				kvp.Value.Destroy();
			m_attachedMovies = null;
			m_detachedMovies = null;
			m_attachedMovieList = null;
		}

		if (m_attachedLWFs != null) {
			foreach (KeyValuePair<string, LWFContainer> kvp in m_attachedLWFs)
				if (kvp.Value.child.detachHandler != null)
					kvp.Value.child.detachHandler(kvp.Value.child);
			m_attachedLWFs = null;
			m_detachedLWFs = null;
			m_attachedLWFList = null;
		}

		PlayAnimation(ClipEvent.UNLOAD);

		if (m_handler != null)
			m_handler.Call(EventType.UNLOAD, this);

		m_displayList = null;
		m_property = null;

		base.Destroy();
	}

	public void PlayAnimation(ClipEvent clipEvent)
	{
		Format.MovieClipEvent[] clipEvents = m_lwf.data.movieClipEvents;
		for (int i = 0; i < m_data.clipEvents; ++i) {
			Format.MovieClipEvent c = clipEvents[m_data.clipEventId + i];
			if ((c.clipEvent & (int)clipEvent) != 0)
				m_lwf.PlayAnimation(c.animationId, this);
		}
	}

	public int SearchFrame(string label)
	{
		return m_lwf.SearchFrame(this, label);
	}

	public int SearchFrame(int stringId)
	{
		return m_lwf.SearchFrame(this, stringId);
	}

	public Movie SearchMovieInstance(int stringId, bool recursive = true)
	{
		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsMovie() && m_lwf.GetInstanceNameStringId(
					instance.instanceId) == stringId) {
				return (Movie)instance;
			} else if (recursive && instance.IsMovie()) {
				Movie i = ((Movie)instance).SearchMovieInstance(
					stringId, recursive);
				if (i != null)
					return i;
			}
		}
		return null;
	}

	public Movie SearchMovieInstance(string instanceName, bool recursive = true)
	{
		return SearchMovieInstance(m_lwf.GetStringId(instanceName), recursive);
	}

	public Movie this[string instanceName]
	{
		get {return SearchMovieInstance(instanceName);}
	}

	public Movie SearchMovieInstanceByInstanceId(int instId, bool recursive)
	{
		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsMovie() && instance.instanceId == instId) {
				return (Movie)instance;
			} else if (recursive && instance.IsMovie()) {
				Movie i = ((Movie)instance).SearchMovieInstanceByInstanceId(
					instId, recursive);
				if (i != null)
					return i;
			}
		}
		return null;
	}

	public Button SearchButtonInstance(int stringId, bool recursive = true)
	{
		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsButton() && m_lwf.GetInstanceNameStringId(
					instance.instanceId) == stringId) {
				return (Button)instance;
			} else if (recursive && instance.IsMovie()) {
				Button i = ((Movie)instance).SearchButtonInstance(
					stringId, recursive);
				if (i != null)
					return i;
			}
		}
		return null;
	}

	public Button SearchButtonInstance(
		string instanceName, bool recursive = true)
	{
		return SearchButtonInstance(m_lwf.GetStringId(instanceName), recursive);
	}

	public Button SearchButtonInstanceByInstanceId(int instId, bool recursive)
	{
		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsButton() && instance.instanceId == instId) {
				return (Button)instance;
			} else if (recursive && instance.IsMovie()) {
				Button i = ((Movie)instance).SearchButtonInstanceByInstanceId(
					instId, recursive);
				if (i != null)
					return i;
			}
		}
		return null;
	}
}

}	// namespace LWF
