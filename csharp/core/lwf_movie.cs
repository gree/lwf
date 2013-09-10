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
using EventHandler = Action;
using EventHandlerList = List<Action>;
using EventHandlerDictionary = Dictionary<string, List<Action>>;
using EventType = MovieEventHandlers.Type;
using ClipEvent = Format.MovieClipEvent.ClipEvent;
using AttachedMovies = Dictionary<string, Movie>;
using AttachedMovieList = SortedDictionary<int, Movie>;
using AttachedMovieDescendingList = SortedDictionary<int, int>;
using AttachedLWFs = Dictionary<string, LWFContainer>;
using AttachedLWFList = SortedDictionary<int, LWFContainer>;
using AttachedLWFDescendingList = SortedDictionary<int, int>;
using DetachDict = Dictionary<string, bool>;
using Inspector = System.Action<Object, int, int, int>;
using Texts = Dictionary<string, bool>;

public partial class Movie : IObject
{
	private Format.Movie m_data;
	private IObject m_instanceHead;
	private IObject m_instanceTail;
	private Object[] m_displayList;
	private EventHandlerDictionary m_eventHandlers;
	private MovieEventHandlers m_handler;
	private AttachedMovies m_attachedMovies;
	private AttachedMovieList m_attachedMovieList;
	private AttachedMovieDescendingList m_attachedMovieDescendingList;
	private DetachDict m_detachedMovies;
	private AttachedLWFs m_attachedLWFs;
	private AttachedLWFList m_attachedLWFList;
	private AttachedLWFDescendingList m_attachedLWFDescendingList;
	private DetachDict m_detachedLWFs;
	private Texts m_texts;
	private string m_attachName;
	private int m_totalFrames;
	private int m_currentFrameInternal;
	private int m_currentFrameCurrent;
	private int m_execedFrame;
	private int m_animationPlayedFrame;
	private int m_depth;
	private int m_lastControlOffset;
	private int m_lastControls;
	private int m_lastControlAnimationOffset;
	private int m_movieExecCount;
	private int m_postExecCount;
	private bool m_active;
	private bool m_visible;
	private bool m_playing;
	private bool m_jumped;
	private bool m_overriding;
	private bool m_hasButton;
	private bool m_postLoaded;
	private bool m_lastHasButton;
	private bool m_skipped;
	private bool m_attachMovieExeced;
	private bool m_attachMoviePostExeced;
	private Matrix m_matrix0;
	private Matrix m_matrix1;
	private ColorTransform m_colorTransform0;
	private ColorTransform m_colorTransform1;

	private Property m_property;

	public Movie(LWF lwf, Movie parent, int objId, int instId, int matrixId = 0,
			int colorTransformId = 0, bool attached = false,
			MovieEventHandlers handler = null, string n = null)
		: base(lwf, parent,
			attached ? Type.ATTACHEDMOVIE : Type.MOVIE, objId, instId)
	{
		m_data = lwf.data.movies[objId];
		m_matrixId = matrixId;
		m_colorTransformId = colorTransformId;
		m_totalFrames = m_data.frames;

		if (!String.IsNullOrEmpty(n))
			m_name = n;
		m_instanceHead = null;
		m_instanceTail = null;
		m_currentFrameInternal = -1;
		m_execedFrame = -1;
		m_animationPlayedFrame = -1;
		m_lastControlOffset = -1;
		m_lastControls = -1;
		m_lastHasButton = false;
		m_lastControlAnimationOffset = -1;
		m_skipped = false;
		m_postLoaded = false;
		m_active = true;
		m_visible = true;
		m_playing = true;
		m_jumped = false;
		m_overriding = false;
		m_attachMovieExeced = false;
		m_attachMoviePostExeced = false;
		m_movieExecCount = -1;
		m_postExecCount = -1;

		m_property = new Property(lwf);

		m_matrix0 = new Matrix();
		m_matrix1 = new Matrix();
		m_colorTransform0 = new ColorTransform();
		m_colorTransform1 = new ColorTransform();

		m_displayList = new Object[m_data.depths];

		PlayAnimation(ClipEvent.LOAD);

		m_eventHandlers = new EventHandlerDictionary();
		m_handler = new MovieEventHandlers();
		m_handler.Add(lwf.GetMovieEventHandlers(this));
		m_handler.Add(handler);
		if (!m_handler.Empty())
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
	public bool hasButton {get {return m_hasButton;}}

	public void SetHandlers(MovieEventHandlers handler)
	{
		m_handler.Clear();
		m_handler.Add(handler);
	}

	public Point GlobalToLocal(Point point)
	{
		float px;
		float py;
		Matrix invert = new Matrix();
		Matrix m;
		if (m_property.hasMatrix) {
			m = new Matrix();
			m = Utility.CalcMatrix(m, m_matrix, m_property.matrix);
		} else {
			m = m_matrix;
		}
		Utility.InvertMatrix(invert, m);
		Utility.CalcMatrixToPoint(out px, out py, point.x, point.y, invert);
		return new Point(px, py);
	}

	public Point LocalToGlobal(Point point)
	{
		float px;
		float py;
		Matrix m;
		if (m_property.hasMatrix) {
			m = new Matrix();
			m = Utility.CalcMatrix(m, m_matrix, m_property.matrix);
		} else {
			m = m_matrix;
		}
		Utility.CalcMatrixToPoint(out px, out py, point.x, point.y, m);
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
			if (m_texts != null && obj.IsText())
				EraseText(obj.objectId);
			obj.Destroy();
			obj = null;
		}

		if (obj == null) {
			switch ((Type)dataObject.objectType) {
			case Type.BUTTON:
				obj = new Button(m_lwf,
					this, dataObjectId, instId, matrixId, colorTransformId);
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
				obj = new Text(m_lwf, this, dataObjectId, instId);
				break;

			case Type.PARTICLE:
				obj = new Particle(m_lwf, this, dataObjectId);
				break;

			case Type.PROGRAMOBJECT:
				obj = new ProgramObject(m_lwf, this, dataObjectId);
				break;
			}
		}

		if (obj.IsMovie() || obj.IsButton()) {
			IObject instance = (IObject)obj;
			instance.linkInstance = null;
			if (m_instanceHead == null)
				m_instanceHead = instance;
			else
				m_instanceTail.linkInstance = instance;
			m_instanceTail = instance;
			if (obj.IsButton())
				m_hasButton = true;
		}

		if (m_texts != null && obj.IsText())
			InsertText(obj.objectId);

		m_displayList[dlDepth] = obj;
		obj.execCount = m_movieExecCount;
		obj.Exec(matrixId, colorTransformId);
	}

	public void Override(bool overriding)
	{
		m_overriding = overriding;
	}

	public override void Exec(int matrixId = 0, int colorTransformId = 0)
	{
		m_attachMovieExeced = false;
		m_attachMoviePostExeced = false;
		base.Exec(matrixId, colorTransformId);
	}

	public void PostExec(bool progressing)
	{
		m_hasButton = false;
		if (!m_active)
			return;

		m_execedFrame = -1;
		bool postExeced = m_postExecCount == m_lwf.execCount;
		if (progressing && m_playing && !m_jumped && !postExeced)
			++m_currentFrameInternal;
		for (;;) {
			if (m_currentFrameInternal < 0 ||
					m_currentFrameInternal >= m_totalFrames)
				m_currentFrameInternal = 0;
			if (m_currentFrameInternal == m_execedFrame)
				break;

			m_currentFrameCurrent = m_currentFrameInternal;
			m_execedFrame = m_currentFrameCurrent;
			Data data = m_lwf.data;
			Format.Frame frame = data.frames[
				m_data.frameOffset + m_currentFrameCurrent];

			int controlAnimationOffset;
			IObject instance;

			if (m_lastControlOffset == frame.controlOffset &&
					m_lastControls == frame.controls) {

				controlAnimationOffset = m_lastControlAnimationOffset;

				if (m_skipped) {
					instance = m_instanceHead;
					while (instance != null) {
						if (instance.IsMovie()) {
							Movie movie = (Movie)instance;
							movie.m_attachMovieExeced = false;
							movie.m_attachMoviePostExeced = false;
						} else if (instance.IsButton()) {
							((Button)instance).EnterFrame();
						}
						instance = instance.linkInstance;
					}
					m_hasButton = m_lastHasButton;
				} else {
					for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
						Object obj = m_displayList[dlDepth];
						if (obj != null) {
							if (!postExeced) {
								obj.matrixIdChanged = false;
								obj.colorTransformIdChanged = false;
							}
							if (obj.IsMovie()) {
								Movie movie = (Movie)obj;
								movie.m_attachMovieExeced = false;
								movie.m_attachMoviePostExeced = false;
							} else if (obj.IsButton()) {
								((Button)obj).EnterFrame();
								m_hasButton = true;
							}
						}
					}
					m_lastHasButton = m_hasButton;
					m_skipped = true;
				}

			} else {
				++m_movieExecCount;
				m_instanceHead = null;
				m_instanceTail = null;
				m_lastControlOffset = frame.controlOffset;
				m_lastControls = frame.controls;
				controlAnimationOffset = -1;
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
							ExecObject(p.depth, p.objectId, p.matrixId,
								ctrl.colorTransformId, p.instanceId);
						}
						break;

					case Format.Control.Type.MOVEMC:
						{
							Format.ControlMoveMC ctrl =
								data.controlMoveMCs[control.controlId];
							Format.Place p = data.places[ctrl.placeId];
							ExecObject(p.depth, p.objectId, ctrl.matrixId,
								ctrl.colorTransformId, p.instanceId);
						}
						break;

					case Format.Control.Type.ANIMATION:
						if (controlAnimationOffset == -1)
							controlAnimationOffset = i;
						break;
					}
				}

				m_lastControlAnimationOffset = controlAnimationOffset;
				m_lastHasButton = m_hasButton;

				for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
					Object obj = m_displayList[dlDepth];
					if (obj != null && obj.execCount != m_movieExecCount) {
						if (m_texts != null && obj.IsText())
							EraseText(obj.objectId);
						obj.Destroy();
						m_displayList[dlDepth] = null;
					}
				}
			}

			m_attachMovieExeced = true;
			if (m_attachedMovies != null) {
				foreach (Movie movie in m_attachedMovieList.Values)
					if (movie != null)
						movie.Exec();
			}

			m_attachMoviePostExeced = true;
			instance = m_instanceHead;
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
				foreach (Movie movie in m_attachedMovieList.Values) {
					if (movie != null) {
						movie.PostExec(progressing);
						if (!m_hasButton && movie.m_hasButton)
							m_hasButton = true;
					}
				}
			}

			if (m_attachedLWFs != null)
				m_hasButton = true;

			if (!m_postLoaded) {
				m_postLoaded = true;
				if (!m_handler.Empty())
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
		if (!m_handler.Empty())
			m_handler.Call(EventType.ENTERFRAME, this);
		m_postExecCount = m_lwf.execCount;
	}

	private void UpdateObject(Object obj, Matrix m, ColorTransform c,
		bool matrixChanged, bool colorTransformChanged)
	{
		Matrix objm;
		if (obj.IsMovie() && ((Movie)obj).m_property.hasMatrix)
			objm = m;
		else if (matrixChanged || !obj.updated || obj.matrixIdChanged)
			objm = Utility.CalcMatrix(m_lwf, m_matrix1, m, obj.matrixId);
		else
			objm = null;

		ColorTransform objc;
		if (obj.IsMovie() && ((Movie)obj).m_property.hasColorTransform)
			objc = c;
		else if (colorTransformChanged ||
				!obj.updated || obj.colorTransformIdChanged)
			objc = Utility.CalcColorTransform(
				m_lwf, m_colorTransform1, c, obj.colorTransformId);
		else
			objc = null;

		obj.Update(objm, objc);
	}

	public override void Update(Matrix m, ColorTransform c)
	{
		if (!m_active)
			return;

		bool matrixChanged;
		bool colorTransformChanged;

		if (m_overriding) {
			matrixChanged = true;
			colorTransformChanged = true;
		} else {
			matrixChanged = m_matrix.SetWithComparing(m);
			colorTransformChanged = m_colorTransform.SetWithComparing(c);
		}

		if (!m_handler.Empty())
			m_handler.Call(EventType.UPDATE, this);

		if (m_property.hasMatrix) {
			matrixChanged = true;
			m = Utility.CalcMatrix(m_matrix0, m_matrix, m_property.matrix);
		} else {
			m = m_matrix;
		}

		if (m_property.hasColorTransform) {
			colorTransformChanged = true;
			c = Utility.CalcColorTransform(
				m_colorTransform0, m_colorTransform, m_property.colorTransform);
		} else {
			c = m_colorTransform;
		}

		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null)
				UpdateObject(obj, m, c, matrixChanged, colorTransformChanged);
		}

		if (m_attachedMovies != null || m_attachedLWFs != null) {
			if (m_attachedMovies != null) {
				foreach (Movie movie in m_attachedMovieList.Values)
					if (movie != null)
						UpdateObject(movie,
							m, c, matrixChanged, colorTransformChanged);
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
				foreach (LWFContainer lwfContainer
						in m_attachedLWFList.Values) {
					m_lwf.RenderObject(
						lwfContainer.child.Exec(m_lwf.thisTick, m, c));
				}
			}
		}
	}

	public override void LinkButton()
	{
		if (!m_visible || !m_active || !m_hasButton)
			return;

		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null) {
				if (obj.IsButton()) {
					((Button)obj).LinkButton();
				} else if (obj.IsMovie()) {
					Movie movie = (Movie)obj;
					if (movie.m_hasButton)
						movie.LinkButton();
				}
			}
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList.Values)
				if (movie != null && movie.m_hasButton)
					movie.LinkButton();
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values)
				if (lwfContainer != null)
					lwfContainer.LinkButton();
		}
	}

	public override void Render(bool v, int rOffset)
	{
		if (!m_visible || !m_active)
			v = false;

		if (v && !m_handler.Empty())
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
			foreach (Movie movie in m_attachedMovieList.Values)
				if (movie != null)
					movie.Render(v, rOffset);
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				if (lwfContainer != null) {
					LWF child = lwfContainer.child;
					child.SetAttachVisible(v);
					m_lwf.RenderObject(child.Render(m_lwf.renderingIndex,
						m_lwf.renderingCount, rOffset));
				}
			}
		}
	}

#if UNITY_EDITOR
	public override void RenderNow()
	{
		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null)
				obj.RenderNow();
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList.Values)
				if (movie != null)
					movie.RenderNow();
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				if (lwfContainer != null) {
					LWF child = lwfContainer.child;
					child.RenderNow();
				}
			}
		}
	}
#endif

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
			foreach (Movie movie in m_attachedMovieList.Values)
				if (movie != null)
					movie.Inspect(inspector, hierarchy, d++, rOffset);
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
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

		if (!m_handler.Empty())
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
		if (stringId == -1)
			return null;

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
		int stringId = m_lwf.GetStringId(instanceName);
		if (stringId != -1)
			return SearchMovieInstance(stringId, recursive);

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList.Values) {
				if (movie != null) {
					if (movie.attachName == instanceName)
						return movie;
					else if (recursive) {
						Movie descendant = movie.SearchMovieInstance(instanceName, recursive);
						if (descendant != null)
							return descendant;
					}
				}
			}
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				if (lwfContainer != null) {
					LWF child = lwfContainer.child;
					if (child.attachName == instanceName) {
						return child.rootMovie;
					} else if (recursive) {
						Movie descendant = child.rootMovie.SearchMovieInstance(instanceName, recursive);
						if (descendant != null)
							return descendant;
					}
				}
			}
		}

		return null;
	}

	public Movie this[string instanceName]
	{
		get {return SearchMovieInstance(instanceName, false);}
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
		if (stringId == -1)
			return null;

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
		int stringId = m_lwf.GetStringId(instanceName);
		if (stringId != -1)
			return SearchButtonInstance(stringId, recursive);

		if (m_attachedMovies != null && recursive) {
			foreach (Movie movie in m_attachedMovieList.Values) {
				if (movie != null) {
					Button button = movie.SearchButtonInstance(instanceName, recursive);
					if (button != null)
						return button;
				}
			}
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				if (lwfContainer != null) {
					LWF child = lwfContainer.child;
					Button button = child.rootMovie.SearchButtonInstance(instanceName, recursive);
					if (button != null)
						return button;
				}
			}
		}

		return null;
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

	public void InsertText(int objId)
	{
		Format.Text text = lwf.data.texts[objId];
		if (text.nameStringId != -1)
			m_texts[lwf.data.strings[text.nameStringId]] = true;
	}

	public void EraseText(int objId)
	{
		Format.Text text = lwf.data.texts[objId];
		if (text.nameStringId != -1)
			m_texts.Remove(lwf.data.strings[text.nameStringId]);
	}

	public bool SearchText(string textName)
	{
		if (m_texts != null) {
			m_texts = new Texts();
			for (int dlDepth = 0; dlDepth < data.depths; ++dlDepth) {
				Object obj = m_displayList[dlDepth];
				if (obj != null && obj.IsText())
					InsertText(obj.objectId);
			}
		}

		bool v;
		if (m_texts.TryGetValue(textName, out v))
			return true;
		return false;
	}

	public void AddEventHandler(string eventName, EventHandler eventHandler)
	{
		EventHandlerList list;
		if (!m_eventHandlers.TryGetValue(eventName, out list)) {
			list = new EventHandlerList();
			m_eventHandlers[eventName] = list;
		}
		list.Add(eventHandler);
	}

	public void RemoveEventHandler(string eventName, EventHandler eventHandler)
	{
		EventHandlerList list = m_eventHandlers[eventName];
		if (list == null)
			return;
		list.RemoveAll(h => h == eventHandler);
	}

	public void ClearEventHandler(string eventName)
	{
		m_eventHandlers.Remove(eventName);
	}

	public void SetEventHandler(string eventName, EventHandler eventHandler)
	{
		ClearEventHandler(eventName);
		AddEventHandler(eventName, eventHandler);
	}

	public void DispatchEvent(string eventName)
	{
		EventHandlerList list =
			new EventHandlerList(m_eventHandlers[eventName]);
		foreach (EventHandler h in list)
			h();
	}
}

}	// namespace LWF
