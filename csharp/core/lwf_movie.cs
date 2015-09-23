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

using Constant = Format.Constant;
using Type = Format.Object.Type;
using EventHandler = Action;
using EventHandlerDictionary = Dictionary<int, Action>;
using EventHandlers = Dictionary<string, Dictionary<int, Action>>;
using EventType = MovieEventHandlers.Type;
using ClipEvent = Format.MovieClipEvent.ClipEvent;
using MovieEventHandler = Action<Movie>;
using CalculateBoundsCallbacks = List<Action<Movie>>;
using AttachedMovies = Dictionary<string, Movie>;
using AttachedMovieList = SortedDictionary<int, Movie>;
using AttachedMovieDescendingList = SortedDictionary<int, int>;
using AttachedLWFs = Dictionary<string, LWFContainer>;
using AttachedLWFList = SortedDictionary<int, LWFContainer>;
using AttachedLWFDescendingList = SortedDictionary<int, int>;
using DetachDict = Dictionary<string, bool>;
using Inspector = System.Action<Object, int, int, int>;
using Texts = Dictionary<string, bool>;
using BitmapClips = SortedDictionary<int, BitmapClip>;
using CurrentLabels = List<LabelData>;
using CurrentLabelCache = Dictionary<int, string>;

public class LabelData
{
	public int frame;
	public string name;
}

public partial class Movie : IObject
{
	private Format.Movie m_data;
	private IObject m_instanceHead;
	private IObject m_instanceTail;
	private Object[] m_displayList;
	private EventHandlers m_eventHandlers;
	private MovieEventHandlers m_handler;
	private CalculateBoundsCallbacks m_calculateBoundsCallbacks;
	private AttachedMovies m_attachedMovies;
	private AttachedMovieList m_attachedMovieList;
	private AttachedMovieDescendingList m_attachedMovieDescendingList;
	private DetachDict m_detachedMovies;
	private AttachedLWFs m_attachedLWFs;
	private AttachedLWFList m_attachedLWFList;
	private AttachedLWFDescendingList m_attachedLWFDescendingList;
	private DetachDict m_detachedLWFs;
	private Texts m_texts;
	private BitmapClips m_bitmapClips;
	private Bounds m_bounds;
	private Bounds m_currentBounds;
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
	private int m_blendMode;
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
	private bool m_needsUpdateAttachedLWFs;
	private bool m_requestedCalculateBounds;
#if LWF_USE_LUA
	private bool m_isRoot;
#endif
	private Matrix m_matrix0;
	private Matrix m_matrix1;
	private Matrix m_matrixForAttachedLWFs;
	private ColorTransform m_colorTransform0;
	private ColorTransform m_colorTransform1;
	private ColorTransform m_colorTransformForAttachedLWFs;
	private CurrentLabels m_currentLabelsCache;
	private CurrentLabelCache m_currentLabelCache;

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
		m_blendMode = (int)Constant.BLEND_MODE_NORMAL;
		m_requestedCalculateBounds = false;
		m_calculateBoundsCallbacks = new CalculateBoundsCallbacks();

		m_property = new Property(lwf);

		m_matrix0 = new Matrix();
		m_matrix1 = new Matrix();
		m_matrixForAttachedLWFs = new Matrix();
		m_colorTransform0 = new ColorTransform();
		m_colorTransform1 = new ColorTransform();
		m_colorTransformForAttachedLWFs = new ColorTransform();

		m_displayList = new Object[m_data.depths];

		m_eventHandlers = new EventHandlers();
		m_handler = new MovieEventHandlers();
		m_handler.Add(lwf.GetMovieEventHandlers(this));
		m_handler.Add(handler);

#if LWF_USE_LUA
		m_isRoot = objId == lwf.data.header.rootMovieId;
		if (m_isRoot) {
			if (parent == null)
				lwf.CallFunctionLua("Init", this);
			lwf.GetFunctionsLua(objId, out m_rootLoadFunc,
				out m_rootPostLoadFunc, out m_rootUnloadFunc,
					out m_rootEnterFrameFunc, true);
		}
		lwf.GetFunctionsLua(objId, out m_loadFunc, out m_postLoadFunc,
			out m_unloadFunc, out m_enterFrameFunc, false);

		if (m_isRoot && !String.IsNullOrEmpty(m_rootLoadFunc))
			lwf.CallFunctionLua(m_rootLoadFunc, this);
		if (m_loadFunc != String.Empty)
			lwf.CallFunctionLua(m_loadFunc, this);
#endif

		PlayAnimation(ClipEvent.LOAD);
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
	public int blendMode {
		get {return m_blendMode;}
		set {m_blendMode = value;}
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
		int matrixId, int colorTransformId, int instId,
		int dlBlendMode, bool updateBlendMode = false)
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
				((Movie)obj).blendMode = dlBlendMode;
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

		if (obj.IsMovie() && updateBlendMode)
			((Movie)obj).blendMode = dlBlendMode;

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
								p.matrixId, 0, p.instanceId, p.blendMode);
						}
						break;

					case Format.Control.Type.MOVEM:
						{
							Format.ControlMoveM ctrl =
								data.controlMoveMs[control.controlId];
							Format.Place p = data.places[ctrl.placeId];
							ExecObject(p.depth, p.objectId,
								ctrl.matrixId, 0, p.instanceId, p.blendMode);
						}
						break;

					case Format.Control.Type.MOVEC:
						{
							Format.ControlMoveC ctrl =
								data.controlMoveCs[control.controlId];
							Format.Place p = data.places[ctrl.placeId];
							ExecObject(p.depth, p.objectId, p.matrixId,
								ctrl.colorTransformId, p.instanceId,
								p.blendMode);
						}
						break;

					case Format.Control.Type.MOVEMC:
						{
							Format.ControlMoveMC ctrl =
								data.controlMoveMCs[control.controlId];
							Format.Place p = data.places[ctrl.placeId];
							ExecObject(p.depth, p.objectId, ctrl.matrixId,
								ctrl.colorTransformId, p.instanceId,
								p.blendMode);
						}
						break;

					case Format.Control.Type.MOVEMCB:
						{
							Format.ControlMoveMCB ctrl =
								data.controlMoveMCBs[control.controlId];
							Format.Place p = data.places[ctrl.placeId];
							ExecObject(p.depth, p.objectId, ctrl.matrixId,
								ctrl.colorTransformId, p.instanceId,
								ctrl.blendMode, true);
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

			m_attachMoviePostExeced = true;
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
#if LWF_USE_LUA
				if (m_isRoot && !String.IsNullOrEmpty(m_rootPostLoadFunc))
					lwf.CallFunctionLua(m_rootPostLoadFunc, this);
				if (m_postLoadFunc != String.Empty)
					lwf.CallFunctionLua(m_postLoadFunc, this);
#endif
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

#if LWF_USE_LUA
		if (m_isRoot && !String.IsNullOrEmpty(m_rootEnterFrameFunc))
			lwf.CallFunctionLua(m_rootEnterFrameFunc, this);
		if (m_enterFrameFunc != String.Empty)
			lwf.CallFunctionLua(m_enterFrameFunc, this);
#endif
		PlayAnimation(ClipEvent.ENTERFRAME);
		if (!m_handler.Empty())
			m_handler.Call(EventType.ENTERFRAME, this);
		m_postExecCount = m_lwf.execCount;
	}

	public bool ExecAttachedLWF(float tick, float currentProgress)
	{
		bool hasButton = false;
		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsMovie()) {
				Movie movie = (Movie)instance;
				hasButton |= movie.ExecAttachedLWF(tick, currentProgress);
			}
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList.Values) {
				if (movie != null)
					hasButton |= movie.ExecAttachedLWF(tick, currentProgress);
			}
		}

		if (m_attachedLWFs != null) {
			foreach (KeyValuePair<string, bool> kvp in m_detachedLWFs) {
				string attachName = kvp.Key;
				LWFContainer lwfContainer;
				if (m_attachedLWFs.TryGetValue(attachName, out lwfContainer))
					DeleteAttachedLWF(this, lwfContainer, true, false);
			}
			m_detachedLWFs.Clear();
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				LWF child = lwfContainer.child;
				if (child.tick == m_lwf.tick)
					child.progress = currentProgress;
				m_lwf.RenderObject(child.ExecInternal(tick));
				hasButton |= child.rootMovie.hasButton;
			}
		}

		return hasButton;
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

		if (m_attachedLWFs != null) {
			m_needsUpdateAttachedLWFs = false;
			m_needsUpdateAttachedLWFs |=
				m_matrixForAttachedLWFs.SetWithComparing(m);
			m_needsUpdateAttachedLWFs |=
				m_colorTransformForAttachedLWFs.SetWithComparing(c);
		}

		for (int dlDepth = 0; dlDepth < m_data.depths; ++dlDepth) {
			Object obj = m_displayList[dlDepth];
			if (obj != null)
				UpdateObject(obj, m, c, matrixChanged, colorTransformChanged);
		}

		if (m_bitmapClips != null) {
			foreach (BitmapClip bitmapClip in m_bitmapClips.Values)
				if (bitmapClip != null)
					bitmapClip.Update(m, c);
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList.Values)
				if (movie != null)
					UpdateObject(movie,
						m, c, matrixChanged, colorTransformChanged);
		}
	}

	public void PostUpdate()
	{
		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsMovie()) {
				Movie movie = (Movie)instance;
				movie.PostUpdate();
			}
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList.Values) {
				if (movie != null)
					movie.PostUpdate();
			}
		}

		if (m_requestedCalculateBounds) {
			m_currentBounds = new Bounds(
				float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);
			Inspect((o, h, d, r) => {CalculateBounds(o);}, 0, 0, 0);
			if (lwf.property.hasMatrix) {
				Matrix invert = new Matrix();
				Utility.InvertMatrix(invert, lwf.property.matrix);
				float x;
				float y;
				Utility.CalcMatrixToPoint(out x, out y,
					m_currentBounds.xMin, m_currentBounds.yMin, invert);
				m_currentBounds.xMin = x;
				m_currentBounds.yMin = y;
				Utility.CalcMatrixToPoint(out x, out y,
					m_currentBounds.xMax, m_currentBounds.yMax, invert);
				m_currentBounds.xMax = x;
				m_currentBounds.yMax = y;
			}

			m_bounds = m_currentBounds;
			m_currentBounds = null;
			m_requestedCalculateBounds = false;
			if (m_calculateBoundsCallbacks.Count != 0) {
				foreach (MovieEventHandler h in m_calculateBoundsCallbacks)
					h(this);
				m_calculateBoundsCallbacks.Clear();
			}
		}

		if (!m_handler.Empty())
			m_handler.Call(EventType.UPDATE, this);
	}

	public void UpdateAttachedLWF()
	{
		for (IObject instance = m_instanceHead; instance != null;
				instance = instance.linkInstance) {
			if (instance.IsMovie()) {
				Movie movie = (Movie)instance;
				movie.UpdateAttachedLWF();
			}
		}

		if (m_attachedMovies != null) {
			foreach (Movie movie in m_attachedMovieList.Values) {
				if (movie != null)
					movie.UpdateAttachedLWF();
			}
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				if (lwfContainer == null)
					continue;
				LWF child = lwfContainer.child;
				bool needsUpdateAttachedLWFs =
					child.needsUpdate || m_needsUpdateAttachedLWFs;
				if (needsUpdateAttachedLWFs)
					child.Update(m_matrixForAttachedLWFs,
						m_colorTransformForAttachedLWFs);
				if (child.isLWFAttached)
					child.rootMovie.UpdateAttachedLWF();
				if (needsUpdateAttachedLWFs)
					child.rootMovie.PostUpdate();
			}
		}
	}

	private void CalculateBounds(Object o)
	{
		switch (o.type) {
		case Type.GRAPHIC:
			foreach (Object obj in ((Graphic)o).displayList)
				CalculateBounds(obj);
			break;

		case Type.BITMAP:
		case Type.BITMAPEX:
			int tfId = -1;
			if (o.type == Type.BITMAP) {
				if (o.objectId < o.lwf.data.bitmaps.Length)
					tfId = o.lwf.data.bitmaps[o.objectId].textureFragmentId;
			} else {
				if (o.objectId < o.lwf.data.bitmapExs.Length)
					tfId = o.lwf.data.bitmapExs[o.objectId].textureFragmentId;
			}
			if (tfId >= 0) {
				var tf = o.lwf.data.textureFragments[tfId];
				UpdateBounds(o.matrix, tf.x, tf.x + tf.w, tf.y, tf.y + tf.h);
			}
			break;

		case Type.BUTTON:
			var button = (Button)o;
			UpdateBounds(o.matrix, 0, button.width, 0, button.height);
			break;

		case Type.TEXT:
			var text = o.lwf.data.texts[o.objectId];
			UpdateBounds(o.matrix, 0, text.width, 0, text.height);
			break;

		case Type.PROGRAMOBJECT:
			var pobj = o.lwf.data.programObjects[o.objectId];
			UpdateBounds(o.matrix, 0, pobj.width, 0, pobj.height);
			break;
		}
	}

	private void UpdateBounds(
		Matrix m, float xMin, float xMax, float yMin, float yMax)
	{
		UpdateBounds(m, xMin, yMin);
		UpdateBounds(m, xMin, yMax);
		UpdateBounds(m, xMax, yMin);
		UpdateBounds(m, xMax, yMax);
	}

	private void UpdateBounds(Matrix m, float sx, float sy)
	{
		float x;
		float y;
		Utility.CalcMatrixToPoint(out x, out y, sx, sy, m);
		if (x < m_currentBounds.xMin)
			m_currentBounds.xMin = x;
		else if (x > m_currentBounds.xMax)
			m_currentBounds.xMax = x;
		if (y < m_currentBounds.yMin)
			m_currentBounds.yMin = y;
		else if (y > m_currentBounds.yMax)
			m_currentBounds.yMax = y;
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
			foreach (Movie movie in m_attachedMovieList.Values) {
				if (movie != null && movie.m_hasButton)
					movie.LinkButton();
			}
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				if (lwfContainer != null)
					lwfContainer.LinkButton();
			}
		}
	}

	public override void Render(bool v, int rOffset)
	{
		if (!m_visible || !m_active)
			v = false;

		bool useBlendMode = false;
		bool useMaskMode = false;
		if (m_blendMode != (int)Constant.BLEND_MODE_NORMAL) {
			switch (m_blendMode) {
			case (int)Constant.BLEND_MODE_ADD:
			case (int)Constant.BLEND_MODE_MULTIPLY:
			case (int)Constant.BLEND_MODE_SCREEN:
			case (int)Constant.BLEND_MODE_SUBTRACT:
				m_lwf.BeginBlendMode(m_blendMode);
				useBlendMode = true;
				break;
			case (int)Constant.BLEND_MODE_ERASE:
			case (int)Constant.BLEND_MODE_LAYER:
			case (int)Constant.BLEND_MODE_MASK:
				m_lwf.BeginMaskMode(m_blendMode);
				useMaskMode = true;
				break;
			}
		}

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

		if (m_bitmapClips != null) {
			foreach (BitmapClip bitmapClip in m_bitmapClips.Values)
				if (bitmapClip != null)
					bitmapClip.Render(v && bitmapClip.visible, rOffset);
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

		if (useBlendMode)
			m_lwf.EndBlendMode();
		if (useMaskMode)
			m_lwf.EndMaskMode();
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

		if (m_bitmapClips != null) {
			foreach (BitmapClip bitmapClip in m_bitmapClips.Values)
				if (bitmapClip != null)
					bitmapClip.Inspect(inspector, hierarchy, d++, rOffset);
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

		if (m_bitmapClips != null) {
			foreach (KeyValuePair<int, BitmapClip> kvp in m_bitmapClips)
				if (kvp.Value != null)
					kvp.Value.Destroy();
			m_bitmapClips = null;
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

#if LWF_USE_LUA
		if (m_isRoot && !String.IsNullOrEmpty(m_rootUnloadFunc))
			lwf.CallFunctionLua(m_rootUnloadFunc, this);
		if (m_unloadFunc != String.Empty)
			lwf.CallFunctionLua(m_unloadFunc, this);
#endif
		PlayAnimation(ClipEvent.UNLOAD);

		if (!m_handler.Empty())
			m_handler.Call(EventType.UNLOAD, this);

#if LWF_USE_LUA
		lwf.DestroyMovieLua(this);
#endif
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
					if (movie.attachName == instanceName) {
						return movie;
					} else if (recursive) {
						Movie descendant = movie.SearchMovieInstance(
							instanceName, recursive);
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
						Movie descendant = child.rootMovie.SearchMovieInstance(
							instanceName, recursive);
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
					Button button = movie.SearchButtonInstance(
						instanceName, recursive);
					if (button != null)
						return button;
				}
			}
		}

		if (m_attachedLWFs != null) {
			foreach (LWFContainer lwfContainer in m_attachedLWFList.Values) {
				if (lwfContainer != null) {
					LWF child = lwfContainer.child;
					Button button = child.rootMovie.SearchButtonInstance(
						instanceName, recursive);
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

	public int AddEventHandler(string eventName, MovieEventHandler handler)
	{
		int id = m_lwf.GetEventOffset();
		switch (eventName) {
		case "load": m_handler.Add(id, l:handler); return id;
		case "postLoad": m_handler.Add(id, p:handler); return id;
		case "unload": m_handler.Add(id, u:handler); return id;
		case "enterFrame": m_handler.Add(id, e:handler); return id;
		case "update": m_handler.Add(id, up:handler); return id;
		case "render": m_handler.Add(id, r:handler); return id;
		default: return -1;
		}
	}

	public int AddEventHandler(string eventName, EventHandler eventHandler)
	{
		int id = m_lwf.GetEventOffset();
		EventHandlerDictionary dict;
		if (!m_eventHandlers.TryGetValue(eventName, out dict)) {
			dict = new EventHandlerDictionary();
			m_eventHandlers[eventName] = dict;
		}
		dict.Add(id, eventHandler);

		return id;
	}

	public void RemoveEventHandler(string eventName, int id)
	{
		switch (eventName) {
		case "load":
		case "postLoad":
		case "unload":
		case "enterFrame":
		case "update":
		case "render":
			m_handler.Remove(id);
			break;

		default:
			{
				EventHandlerDictionary dict;
				if (m_eventHandlers.TryGetValue(eventName, out dict))
					dict.Remove(id);
			}
			break;
		}
	}

	public void ClearEventHandler(string eventName)
	{
		switch (eventName) {
		case "load":
			m_handler.Clear(EventType.LOAD);
			break;
		case "postLoad":
			m_handler.Clear(EventType.POSTLOAD);
			break;
		case "unload":
			m_handler.Clear(EventType.UNLOAD);
			break;
		case "enterFrame":
			m_handler.Clear(EventType.ENTERFRAME);
			break;
		case "update":
			m_handler.Clear(EventType.UPDATE);
			break;
		case "render":
			m_handler.Clear(EventType.RENDER);
			break;
		default:
			m_eventHandlers.Remove(eventName);
			break;
		}
	}

	public int SetEventHandler(string eventName, EventHandler eventHandler)
	{
		ClearEventHandler(eventName);
		return AddEventHandler(eventName, eventHandler);
	}

	public void DispatchEvent(string eventName)
	{
		switch (eventName) {
		case "load":
			m_handler.Call(EventType.LOAD, this);
			break;
		case "postLoad":
			m_handler.Call(EventType.POSTLOAD, this);
			break;
		case "unload":
			m_handler.Call(EventType.UNLOAD, this);
			break;
		case "enterFrame":
			m_handler.Call(EventType.ENTERFRAME, this);
			break;
		case "update":
			m_handler.Call(EventType.UPDATE, this);
			break;
		case "render":
			m_handler.Call(EventType.RENDER, this);
			break;
		default:
			{
				EventHandlerDictionary dict;
				if (m_eventHandlers.TryGetValue(eventName, out dict)) {
					dict = new EventHandlerDictionary(dict);
					foreach (var h in dict)
						h.Value();
				}
			}
			break;
		}
	}

	public void RequestCalculateBounds(MovieEventHandler callback = null)
	{
		m_requestedCalculateBounds = true;
		m_calculateBoundsCallbacks.Add(callback);
		m_bounds = null;
		return;
	}

	public Bounds GetBounds()
	{
		return m_bounds;
	}

	private void CacheCurrentLabels()
	{
		if (m_currentLabelsCache != null)
			return;

		m_currentLabelsCache = new CurrentLabels();
		Dictionary<int, int> labels = m_lwf.GetMovieLabels(this);
		if (labels == null)
			return;

		foreach (KeyValuePair<int, int> kvp in labels) {
			LabelData labelData = new LabelData{
				frame = kvp.Value + 1,
				name = m_lwf.data.strings[kvp.Key],
			};
			m_currentLabelsCache.Add(labelData);
		}
		m_currentLabelsCache.Sort((a, b) => {
			return a.frame - b.frame;
		});
	}

	public string GetCurrentLabel()
	{
		CacheCurrentLabels();

		if (m_currentLabelsCache.Count == 0)
			return null;

		int currentFrameTmp = currentFrame;
		if (currentFrameTmp < 1)
			currentFrameTmp = 1;

		if (m_currentLabelCache == null)
			m_currentLabelCache = new CurrentLabelCache();

		string labelName = null;
		if (!m_currentLabelCache.TryGetValue(currentFrameTmp, out labelName)) {
			LabelData firstLabel = m_currentLabelsCache[0];
			LabelData lastLabel =
				m_currentLabelsCache[m_currentLabelsCache.Count - 1];
			if (currentFrameTmp < firstLabel.frame) {
				labelName = "";
			} else if (currentFrameTmp == firstLabel.frame) {
				labelName = firstLabel.name;
			} else if (currentFrameTmp >= lastLabel.frame) {
				labelName = lastLabel.name;
			} else {
				int l = 0;
				int ln = m_currentLabelsCache[l].frame;
				int r = m_currentLabelsCache.Count - 1;
				int rn = m_currentLabelsCache[r].frame;
				for (;;) {
					if ((l == r) || (r - l == 1)) {
						if (currentFrameTmp < ln)
							labelName = "";
						else if (currentFrameTmp == rn)
							labelName = m_currentLabelsCache[r].name;
						else
							labelName = m_currentLabelsCache[l].name;
						break;
					}
					int n = (int)Math.Floor((r - l) / 2.0) + l;
					int nn = m_currentLabelsCache[n].frame;
					if (currentFrameTmp < nn) {
						r = n;
						rn = nn;
					} else if (currentFrameTmp > nn) {
						l = n;
						ln = nn;
					} else {
						labelName = m_currentLabelsCache[n].name;
						break;
					}
				}
			}
			m_currentLabelCache[currentFrameTmp] = labelName;
		}

		return String.IsNullOrEmpty(labelName) ? null : labelName;
	}

	public CurrentLabels GetCurrentLabels()
	{
		CacheCurrentLabels();
		return m_currentLabelsCache;
	}
}

}	// namespace LWF
