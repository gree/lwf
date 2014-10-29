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
using System.IO;

namespace LWF {

using MovieCommand = System.Action<Movie>;
using MovieCommands = Dictionary<List<string>, System.Action<Movie>>;
using ProgramObjectConstructor = Func<ProgramObject, int, int, int, Renderer>;
using Condition = Format.ButtonCondition.Condition;
using DetachHandler = Action<LWF>;
using Inspector = System.Action<Object, int, int, int>;
using AllowButtonList = Dictionary<int, bool>;
using DenyButtonList = Dictionary<int, bool>;
using ExecHandler = Action<LWF>;
using ExecHandlerList = List<Action<LWF>>;
using TextDictionary = Dictionary<string, TextDictionaryItem>;
using BlendModes = List<int>;
using MaskModes = List<int>;
using LWFLoader = Func<string, string, LWF>;
using LWFUnloader = Action;
using TextureLoadHandler = Func<string, string, string, string>;

public class TextDictionaryItem
{
	public string text;
	public TextRenderer renderer;

	public TextDictionaryItem(string t) {
		text = t;
	}

	public TextDictionaryItem(string t, TextRenderer r) {
		text = t;
		renderer = r;
	}
}

public partial class LWF
{
	public enum TweenMode {
		Movie,
		LWF,
	}

	private static int m_instanceOffset = 0;
	private static int m_iObjectOffset = 0;
	private static float ROUND_OFF_TICK_RATE = 0.05f;
	private static TextureLoadHandler m_textureLoadHandler;

	private Data m_data;
	private IRendererFactory m_rendererFactory;
	private int m_rootMovieStringId;
	private Property m_property;
	private Movie m_rootMovie;
	private Movie m__root;
	private IObject[] m_instances;
	private Button m_focus;
	private Button m_pressed;
	private Button m_buttonHead;
	private MovieCommands m_movieCommands;
	private ProgramObjectConstructor[] m_programObjectConstructors;
	private DetachHandler m_detachHandler;
	private AllowButtonList m_allowButtonList;
	private DenyButtonList m_denyButtonList;
	private ExecHandlerList m_execHandlers;
	private TextDictionary m_textDictionary;
	private BlendModes m_blendModes;
	private MaskModes m_maskModes;
	private int m_frameRate;
	private int m_fastForwardTimeout;
	private bool m_fastForward;
	private bool m_fastForwardCurrent;
	private bool m_frameSkip;
	private int m_execLimit;
	private int m_renderingIndex;
	private int m_renderingIndexOffsetted;
	private int m_renderingCount;
	private int m_depth;
	private int m_execCount;
	private int m_updateCount;
	private int m_instanceId;
	private double m_time;
	private float m_progress;
	private float m_tick;
	private float m_roundOffTick;
	private Movie m_parent;
	private string m_attachName;
	private bool m_attachVisible;
	private bool m_execDisabled;
	private bool m_executedForExecDisabled;
	private bool m_interceptByNotAllowOrDenyButtons;
	private bool m_intercepted;
	private bool m_propertyDirty;
	private bool m_focusOnLink;
	private bool m_needsUpdate;
	private bool m_needsUpdateForAttachLWF;
	private float m_pointX;
	private float m_pointY;
	private bool m_pressing;
	private Matrix m_matrix;
	private Matrix m_matrixIdentity;
	private Matrix m_execMatrix;
	private ColorTransform m_colorTransform;
	private ColorTransform m_colorTransformIdentity;
	private ColorTransform m_execColorTransform;
	private bool m_alive;
	private int m_eventOffset;

	public Data data {get {return m_data;}}
	public bool interactive {get; set;}
	public float scaleByStage {get; set;}
	public bool isExecDisabled {get {return m_execDisabled;}}
	public bool attachVisible {get {return m_attachVisible;}}
	public bool isPropertyDirty {get {return m_propertyDirty;}}
	public bool isLWFAttached {get; set;}
	public LWFLoader lwfLoader {get; set;}
	public LWFUnloader lwfUnloader {get; set;}
	public object privateData {get; set;}
	public TweenMode tweenMode {get; set;}
	public object tweens {get; set;}
	public int tweenEventId {get; set;}
	public IRendererFactory rendererFactory
		{get {return m_rendererFactory;}}
	public Property property {get {return m_property;}}
	public Movie rootMovie {get {return m_rootMovie;}}
	public Movie _root {get {return m__root;}}
	public Button focus {get {return m_focus;}}
	public Button pressed {get {return m_pressed;}}
	public Button buttonHead {
		get {return m_buttonHead;}
		set {m_buttonHead = value;}
	}
	public bool needsUpdate {get {return m_needsUpdate;}}
	public bool needsUpdateForAttachLWF
		{get {return m_needsUpdateForAttachLWF;}}
	public float pointX {get {return m_pointX;}}
	public float pointY {get {return m_pointY;}}
	public bool pressing {get {return m_pressing;}}
	public int frameRate {get {return m_frameRate;}}
	public int renderingIndex {get {return m_renderingIndex;}}
	public int renderingIndexOffsetted {get {return m_renderingIndexOffsetted;}}
	public int renderingCount {get {return m_renderingCount;}}
	public int execCount {get {return m_execCount;}}
	public int updateCount {get {return m_updateCount;}}
	public int instanceId {get {return m_instanceId;}}
	public float width {get {return m_data.header.width;}}
	public float height {get {return m_data.header.height;}}
	public double time {get {return m_time;}}
	public float progress {
		get {return m_progress;}
		set {m_progress = value;}
	}
	public float tick {get {return m_tick;}}
	public bool alive {get {return m_alive;}}
	public bool focusOnLink {
		get {return m_focusOnLink;}
		set {m_focusOnLink = value;}
	}
	public Movie parent {
		get {return m_parent;}
		set {m_parent = value;}
	}
	public string name
		{get {return m_data.strings[m_data.header.nameStringId];}}
	public string attachName {
		get {return m_attachName;}
		set {m_attachName = value;}
	}
	public int depth {
		get {return m_depth;}
		set {m_depth = value;}
	}
	public DetachHandler detachHandler {
		get {return m_detachHandler;}
		set {m_detachHandler = value;}
	}
	public bool interceptByNotAllowOrDenyButtons {
		get {return m_interceptByNotAllowOrDenyButtons;}
		set {m_interceptByNotAllowOrDenyButtons = value;}
	}
	public bool intercepted {get {return interactive && m_intercepted;}}

#if LWF_USE_LUA
	public LWF(Data lwfData, IRendererFactory r, object l = null)
#else
	public LWF(Data lwfData, IRendererFactory r)
#endif
	{
		m_data = lwfData;

		interactive = m_data.buttonConditions.Length > 0;
		m_frameRate = m_data.header.frameRate;
		m_execLimit = 3;
		m_frameSkip = true;
		m_tick = 1.0f / m_frameRate;
		m_roundOffTick = m_tick * ROUND_OFF_TICK_RATE;
		m_attachVisible = true;
		m_interceptByNotAllowOrDenyButtons = true;
		m_intercepted = false;
		scaleByStage = 1.0f;
		m_needsUpdate = false;
		m_needsUpdateForAttachLWF = false;
		m_pointX = Single.MinValue;
		m_pointY = Single.MinValue;
		m_pressing = false;
		m_instanceId = ++m_instanceOffset;
		m_alive = true;
#if LWF_USE_LUA
		m_luaState = l;
		m_instanceIdString = instanceId.ToString();
		InitLua();
#endif

		if (!interactive && m_data.frames.Length == 1)
			DisableExec();

		m_property = new Property(this);
		m_instances = new IObject[m_data.instanceNames.Length];
		InitEvent();
		m_movieCommands = new MovieCommands();
		m_programObjectConstructors =
			new ProgramObjectConstructor[m_data.programObjects.Length];
		m_textDictionary = new TextDictionary();

		m_matrix = new Matrix();
		m_matrixIdentity = new Matrix();
		m_execMatrix = new Matrix();
		m_colorTransform = new ColorTransform();
		m_colorTransformIdentity = new ColorTransform();
		m_execColorTransform = new ColorTransform();
		m_blendModes = new BlendModes();
		m_maskModes = new MaskModes();

		Init();

		SetRendererFactory(r);
	}

	public void SetRendererFactory(IRendererFactory rendererFactory = null)
	{
		if (rendererFactory == null)
			rendererFactory = new NullRendererFactory();
		m_rendererFactory = rendererFactory;
		m_rendererFactory.Init(this);
	}

	public void SetFrameRate(int f)
	{
		if (f == 0)
			return;
		m_frameRate = f;
		m_tick = 1.0f / m_frameRate;
	}

	public void SetPreferredFrameRate(int f, int execLimit = 2)
	{
		if (f == 0)
			return;
		m_execLimit = (int)Math.Ceiling(
			(double)m_frameRate / (double)f) + execLimit;
	}

	public void FitForHeight(int stageHeight)
	{
		Utility.FitForHeight(this, stageHeight);
	}

	public void FitForWidth(int stageWidth)
	{
		Utility.FitForWidth(this, stageWidth);
	}

	public void ScaleForHeight(int stageHeight)
	{
		Utility.ScaleForHeight(this, stageHeight);
	}

	public void ScaleForWidth(int stageWidth)
	{
		Utility.ScaleForWidth(this, stageWidth);
	}

	public void RenderOffset()
	{
		m_renderingIndexOffsetted = 0;
	}

	public void ClearRenderOffset()
	{
		m_renderingIndexOffsetted = m_renderingIndex;
	}

	public int RenderObject(int count = 1)
	{
		m_renderingIndex += count;
		m_renderingIndexOffsetted += count;
		return m_renderingIndex;
	}

	public void BeginBlendMode(int blendMode)
	{
		m_blendModes.Add(blendMode);
		m_rendererFactory.SetBlendMode(blendMode);
	}

	public void EndBlendMode()
	{
		m_blendModes.RemoveAt(m_blendModes.Count - 1);
		m_rendererFactory.SetBlendMode(m_blendModes.Count > 0 ?
			m_blendModes[m_blendModes.Count - 1] :
			(int)Format.Constant.BLEND_MODE_NORMAL);
	}

	public void BeginMaskMode(int maskMode)
	{
		m_maskModes.Add(maskMode);
		m_rendererFactory.SetMaskMode(maskMode);
	}

	public void EndMaskMode()
	{
		m_maskModes.RemoveAt(m_maskModes.Count - 1);
		m_rendererFactory.SetMaskMode(m_maskModes.Count > 0 ?
			m_maskModes[m_maskModes.Count - 1] :
			(int)Format.Constant.BLEND_MODE_NORMAL);
	}

	public void SetAttachVisible(bool visible)
	{
		m_attachVisible = visible;
	}

	public void ClearFocus(Button button)
	{
		if (m_focus == button)
			m_focus = null;
	}

	public void ClearPressed(Button button)
	{
		if (m_pressed == button)
			m_pressed = null;
	}

	public void ClearIntercepted()
	{
		m_intercepted = false;
	}

	public void Init()
	{
		m_time = 0;
		m_progress = 0;

		Array.Clear(m_instances, 0, m_instances.Length);
		m_focus = null;

		m_movieCommands.Clear();

		m_rootMovieStringId = GetStringId("_root");
		if (m_rootMovie != null)
			m_rootMovie.Destroy();
		m_rootMovie = new Movie(this, null,
			m_data.header.rootMovieId, SearchInstanceId(m_rootMovieStringId));
		m__root = m_rootMovie;
	}

	public void SetRoot(Movie root)
	{
		m__root = root;
	}

	private Matrix CalcMatrix(Matrix matrix)
	{
		Matrix m;
		Property p = m_property;
		if (p.hasMatrix) {
			if (matrix != null) {
				m = Utility.CalcMatrix(m_matrix, matrix, p.matrix);
			} else {
				m = p.matrix;
			}
		} else {
			m = matrix == null ? m_matrixIdentity : matrix;
		}
		return m;
	}

	private ColorTransform CalcColorTransform(ColorTransform colorTransform)
	{
		ColorTransform c;
		Property p = m_property;
		if (p.hasColorTransform) {
			if (colorTransform != null) {
				c = Utility.CalcColorTransform(
					m_colorTransform, colorTransform, p.colorTransform);
			} else {
				c = p.colorTransform;
			}
		} else {
			c = colorTransform == null ?
				m_colorTransformIdentity : colorTransform;
		}
		return c;
	}

	private void LinkButton()
	{
		m_buttonHead = null;
		if (interactive && m_rootMovie.hasButton) {
			m_focusOnLink = false;
			m_rootMovie.LinkButton();
			if (m_focus != null && !m_focusOnLink) {
				m_focus.RollOut();
				m_focus = null;
			}
		}
	}

	public int ExecInternal(float tick)
	{
		if (m_rootMovie == null)
			return 0;
		bool execed = false;
		float currentProgress = m_progress;

		if (m_execDisabled && tweens == null) {
			if (!m_executedForExecDisabled) {
				++m_execCount;
				m_rootMovie.Exec();
				m_rootMovie.PostExec(true);
				m_executedForExecDisabled = true;
				execed = true;
			}
		} else {
			bool progressing = true;
			if (tick == 0) {
				m_progress = m_tick;
			} else if (tick < 0) {
				m_progress = m_tick;
				progressing = false;
			} else {
				if (m_time == 0) {
					m_time += (double)m_tick;
					m_progress += m_tick;
				} else {
					m_time += (double)tick;
					m_progress += tick;
				}
			}

			if (m_execHandlers != null) {
				foreach (var h in m_execHandlers)
					h(this);
			}

			int execLimit = m_execLimit;
			while (m_progress >= m_tick - m_roundOffTick) {
				if (--execLimit < 0) {
					m_progress = 0;
					break;
				}
				m_progress -= m_tick;
				++m_execCount;
				m_rootMovie.Exec();
				m_rootMovie.PostExec(progressing);
				execed = true;
				if (!m_frameSkip)
					break;
			}

			if (m_progress < m_roundOffTick)
				m_progress = 0;

			LinkButton();
		}

		if (isLWFAttached) {
			bool hasButton = m_rootMovie.ExecAttachedLWF(tick, currentProgress);
			if (hasButton)
				LinkButton();
		}

		m_needsUpdate = false;
		if (!m_fastForward) {
			if (execed || m_propertyDirty || m_needsUpdateForAttachLWF)
				m_needsUpdate = true;
		}

		if (!m_execDisabled) {
			if (tick < 0)
				m_progress = currentProgress;
		}

		return m_renderingCount;
	}

	public int Exec(float tick = 0,
		Matrix matrix = null, ColorTransform colorTransform = null)
	{
		bool needsToUpdate = false;
		if (matrix != null)
			needsToUpdate |= m_execMatrix.SetWithComparing(matrix);
		if (colorTransform != null)
			needsToUpdate |=
				m_execColorTransform.SetWithComparing(colorTransform);
		DateTime startTime = default(DateTime);
		if (m_parent == null) {
			m_fastForwardCurrent = m_fastForward;
			if (m_fastForwardCurrent) {
				tick = m_tick;
				startTime = DateTime.Now;
			}
		}

		int renderingCount = 0;
		for (;;) {
			renderingCount = ExecInternal(tick);
			needsToUpdate |= m_needsUpdate;
			if (needsToUpdate)
				Update(matrix, colorTransform);
			if (isLWFAttached)
				m_rootMovie.UpdateAttachedLWF();
			if (needsToUpdate)
				m_rootMovie.PostUpdate();
			if (m_fastForwardCurrent && m_fastForward && m_parent == null) {
				var diff = DateTime.Now - startTime;
				if (diff.TotalMilliseconds >= m_fastForwardTimeout)
					break;
			} else {
				break;
			}
		}

		return renderingCount;
	}

	public int ForceExec(
		Matrix matrix = null, ColorTransform colorTransform = null)
	{
		return Exec(0, matrix, colorTransform);
	}

	public int ForceExecWithoutProgress(
		Matrix matrix = null, ColorTransform colorTransform = null)
	{
		return Exec(-1, matrix, colorTransform);
	}

	public void Update(
		Matrix matrix = null, ColorTransform colorTransform = null)
	{
		++m_updateCount;
		Matrix m = CalcMatrix(matrix);
		ColorTransform c = CalcColorTransform(colorTransform);
		m_renderingIndex = 0;
		m_renderingIndexOffsetted = 0;
		m_rootMovie.Update(m, c);
		m_renderingCount = m_renderingIndex;
		m_propertyDirty = false;
		m_needsUpdateForAttachLWF = false;
	}

	public int Render(int rIndex = 0,
		int rCount = 0, int rOffset = Int32.MinValue)
	{
		if (m_rootMovie == null || m_fastForwardCurrent)
			return 0;
		if (rCount > 0)
			m_renderingCount = rCount;
		m_renderingIndex = rIndex;
		m_renderingIndexOffsetted = rIndex;
		if (m_property.hasRenderingOffset) {
			RenderOffset();
			rOffset = m_property.renderingOffset;
		}
		m_rendererFactory.BeginRender(this);
		m_rootMovie.Render(m_attachVisible, rOffset);
		m_rendererFactory.EndRender(this);
		return m_renderingIndex - rIndex;
	}

#if UNITY_EDITOR
	public void RenderNow()
	{
		m_rootMovie.RenderNow();
	}
#endif

	public int Inspect(Inspector inspector, int hierarchy = 0,
		int inspectDepth = 0, int rIndex = 0, int rCount = 0,
		int rOffset = Int32.MinValue)
	{
		if (rCount > 0)
			m_renderingCount = rCount;
		m_renderingIndex = rIndex;
		m_renderingIndexOffsetted = rIndex;
		if (m_property.hasRenderingOffset) {
			RenderOffset();
			rOffset = m_property.renderingOffset;
		}

		m_rootMovie.Inspect(inspector, hierarchy, inspectDepth, rOffset);
		return m_renderingIndex - rIndex;
	}

	public void Destroy()
	{
		m_rootMovie.Destroy();
#if LWF_USE_LUA
		DestroyLua();
#endif
		if (m_rendererFactory != null) {
			m_rendererFactory.Destruct();
			m_rendererFactory = null;
		}

		m_alive = false;

		if (lwfUnloader != null)
			lwfUnloader();
	}

	public int GetIObjectOffset()
	{
		return ++m_iObjectOffset;
	}

	public Movie SearchMovieInstance(int stringId)
	{
		return SearchMovieInstanceByInstanceId(SearchInstanceId(stringId));
	}

	public Movie SearchMovieInstance(string instanceName)
	{
		if (instanceName.Contains(".")) {
			string[] names = instanceName.Split(new Char[]{'.'});
			if (names[0] != m_data.strings[m_rootMovieStringId])
				return null;

			Movie m = m_rootMovie;
			for (int i = 1; i < names.Length; ++i) {
				m = m.SearchMovieInstance(names[i], false);
				if (m == null)
					return null;
			}

			return m;
		}

		int stringId = GetStringId(instanceName);
		if (stringId == -1)
			return rootMovie.SearchMovieInstance(instanceName, true);

		return SearchMovieInstance(stringId);
	}

	public Movie this[string instanceName]
	{
		get {return SearchMovieInstance(instanceName);}
	}

	public Movie SearchMovieInstanceByInstanceId(int instId)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return null;
		IObject obj = m_instances[instId];
		while (obj != null) {
			if (obj.IsMovie())
				return (Movie)obj;
			obj = obj.nextInstance;
		}
		return null;
	}

	public Button SearchButtonInstance(int stringId)
	{
		return SearchButtonInstanceByInstanceId(SearchInstanceId(stringId));
	}

	public Button SearchButtonInstance(string instanceName)
	{
		if (instanceName.Contains(".")) {
			string[] names = instanceName.Split(new Char[]{'.'});
			if (names[0] != m_data.strings[m_rootMovieStringId])
				return null;

			Movie m = m_rootMovie;
			for (int i = 1; i < names.Length; ++i) {
				if (i == names.Length - 1) {
					return m.SearchButtonInstance(names[i], false);
				} else {
					m = m.SearchMovieInstance(names[i], false);
					if (m == null)
						return null;
				}
			}

			return null;
		}

		int stringId = GetStringId(instanceName);
		if (stringId == -1)
			return rootMovie.SearchButtonInstance(instanceName, true);

		return SearchButtonInstance(stringId);
	}

	public Button SearchButtonInstanceByInstanceId(int instId)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return null;
		IObject obj = m_instances[instId];
		while (obj != null) {
			if (obj.IsButton())
				return (Button)obj;
			obj = obj.nextInstance;
		}
		return null;
	}

	public IObject GetInstance(int instId)
	{
		return m_instances[instId];
	}

	public void SetInstance(int instId, IObject instance)
	{
		m_instances[instId] = instance;
	}

	public ProgramObjectConstructor GetProgramObjectConstructor(
		string programObjectName)
	{
		return GetProgramObjectConstructor(
			SearchProgramObjectId(programObjectName));
	}

	public ProgramObjectConstructor GetProgramObjectConstructor(
		int programObjectId)
	{
		if (programObjectId < 0 ||
				programObjectId >= m_data.programObjects.Length)
			return null;
		return m_programObjectConstructors[programObjectId];
	}

	public void SetProgramObjectConstructor(string programObjectName,
		ProgramObjectConstructor programObjectConstructor)
	{
		SetProgramObjectConstructor(
			SearchProgramObjectId(programObjectName), programObjectConstructor);
	}

	public void SetProgramObjectConstructor(int programObjectId,
		ProgramObjectConstructor programObjectConstructor)
	{
		if (programObjectId < 0 ||
				programObjectId >= m_data.programObjects.Length)
			return;
		m_programObjectConstructors[programObjectId] = programObjectConstructor;
	}

	public void ExecMovieCommand()
	{
		if (m_movieCommands.Count == 0)
			return;

		List<List<string>> deletes = new List<List<string>>();
		foreach (KeyValuePair<List<string>, MovieCommand> kvp
				in m_movieCommands) {
			bool available = true;
			Movie movie = m_rootMovie;
			foreach (string name in kvp.Key) {
				movie = movie.SearchMovieInstance(name);
				if (movie == null) {
					available = false;
					break;
				}
			}
			if (available) {
				kvp.Value(movie);
				deletes.Add(kvp.Key);
			}
		}
		foreach (List<string> key in deletes)
			m_movieCommands.Remove(key);
	}

	public void SetMovieCommand(string[] instanceNames, MovieCommand cmd)
	{
		List<string> names = new List<string>();
		foreach (string name in instanceNames)
			names.Add(name);
		m_movieCommands.Add(names, cmd);
		ExecMovieCommand();
	}

	public Movie SearchAttachedMovie(string attachName)
	{
		return m_rootMovie.SearchAttachedMovie(attachName);
	}

	public LWF SearchAttachedLWF(string attachName)
	{
		return m_rootMovie.SearchAttachedLWF(attachName);
	}

	public bool AddAllowButton(string buttonName)
	{
		int instId = SearchInstanceId(GetStringId(buttonName));
		if (instId < 0)
			return false;

		if (m_allowButtonList == null)
			m_allowButtonList = new AllowButtonList();
		m_allowButtonList[instId] = true;
		return true;
	}

	public bool RemoveAllowButton(string buttonName)
	{
		if (m_allowButtonList == null)
			return false;

		int instId = SearchInstanceId(GetStringId(buttonName));
		if (instId < 0)
			return false;

		return m_allowButtonList.Remove(instId);
	}

	public void ClearAllowButton()
	{
		m_allowButtonList = null;
	}

	public bool AddDenyButton(string buttonName)
	{
		int instId = SearchInstanceId(GetStringId(buttonName));
		if (instId < 0)
			return false;

		if (m_denyButtonList == null)
			m_denyButtonList = new DenyButtonList();
		m_denyButtonList[instId] = true;
		return true;
	}

	public void DenyAllButtons()
	{
		if (m_denyButtonList == null)
			m_denyButtonList = new DenyButtonList();
		for (int instId = 0; instId < m_instances.Length; ++instId)
			m_denyButtonList[instId] = true;
	}

	public bool RemoveDenyButton(string buttonName)
	{
		if (m_denyButtonList == null)
			return false;

		int instId = SearchInstanceId(GetStringId(buttonName));
		if (instId < 0)
			return false;

		return m_denyButtonList.Remove(instId);
	}

	public void ClearDenyButton()
	{
		m_denyButtonList = null;
	}

	public void DisableExec()
	{
		m_execDisabled = true;
		m_executedForExecDisabled = false;
	}

	public void EnableExec()
	{
		m_execDisabled = false;
	}

	public void SetPropertyDirty()
	{
		m_propertyDirty = true;
		if (m_parent != null)
			m_parent.lwf.SetPropertyDirty();
	}

	public LWF GetParent()
	{
		if (m_parent == null)
			return null;

		LWF lwfParent = m_parent.lwf;
		for (;;) {
			if (lwfParent == null || lwfParent.m_parent == null)
				return lwfParent;
			lwfParent = lwfParent.m_parent.lwf;
		}
	}

	public void SetInteractive()
	{
		interactive = true;
		if (m_parent != null)
			m_parent.lwf.SetInteractive();
	}

	public void SetFrameSkip(bool frameSkip)
	{
		m_frameSkip = frameSkip;
		m_progress = 0;
		if (m_parent != null)
			m_parent.lwf.SetFrameSkip(frameSkip);
	}

	public void SetLWFAttached()
	{
		isLWFAttached = true;
		m_needsUpdateForAttachLWF = true;
		if (m_parent != null)
			m_parent.lwf.SetLWFAttached();
	}

	public void SetFastForwardTimeout(int fastForwardTimeout)
	{
		m_fastForwardTimeout = fastForwardTimeout;
	}

	public void SetFastForward(bool fastForward)
	{
		m_fastForward = fastForward;
		if (m_parent != null)
			m_parent.lwf.SetFastForward(fastForward);
	}

	public void AddExecHandler(ExecHandler execHandler)
	{
		if (m_execHandlers == null)
			m_execHandlers = new ExecHandlerList();
		m_execHandlers.Add(execHandler);
	}

	public void RemoveExecHandler(ExecHandler execHandler)
	{
		if (m_execHandlers == null)
			return;
		m_execHandlers.RemoveAll(h => h == execHandler);
	}

	public void ClearExecHandler()
	{
		m_execHandlers = null;
	}

	public void SetExecHandler(ExecHandler execHandler)
	{
		ClearExecHandler();
		AddExecHandler(execHandler);
	}

	public void SetText(string textName, string text)
	{
		TextDictionaryItem item;
		if (!m_textDictionary.TryGetValue(textName, out item)) {
			m_textDictionary[textName] = new TextDictionaryItem(text);
		} else {
			if (item.renderer != null)
				item.renderer.SetText(text);
			item.text = text;
		}
	}

	public string GetText(string textName)
	{
		TextDictionaryItem item;
		if (m_textDictionary.TryGetValue(textName, out item))
			return item.text;
		return null;
	}

	public void SetTextRenderer(string fullPath,
		string textName, string text, TextRenderer textRenderer)
	{
		bool setText = false;
		string fullName = fullPath + "." + textName;
		TextDictionaryItem item;
		if (m_textDictionary.TryGetValue(fullName, out item)) {
			item.renderer = textRenderer;
			if (!String.IsNullOrEmpty(item.text)) {
				textRenderer.SetText(item.text);
				setText = true;
			}
		} else {
			m_textDictionary[fullName] =
				new TextDictionaryItem(text, textRenderer);
		}

		if (m_textDictionary.TryGetValue(textName, out item)) {
			item.renderer = textRenderer;
			if (!setText && !String.IsNullOrEmpty(item.text)) {
				textRenderer.SetText(item.text);
				setText = true;
			}
		} else {
			m_textDictionary[textName] =
				new TextDictionaryItem(text, textRenderer);
		}

		if (!setText)
			textRenderer.SetText(text);
	}

	public void ClearTextRenderer(string textName)
	{
		TextDictionaryItem item;
		if (!m_textDictionary.TryGetValue(textName, out item))
			item.renderer = null;
	}

	public static void SetTextureLoadHandler(TextureLoadHandler h)
	{
		m_textureLoadHandler = h;
	}

	public static TextureLoadHandler GetTextureLoadHandler()
	{
		return m_textureLoadHandler;
	}
}

}	// namespace LWF
