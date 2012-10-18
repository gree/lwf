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

using EventHandler = Action<Movie, Button>;
using MovieEventHandler = Action<Movie>;
using ButtonEventHandler = Func<Button, bool>;
using ButtonKeyPressHandler = Func<Button, int, bool>;
using MovieCommand = System.Action<Movie>;
using MovieCommands = Dictionary<List<string>, System.Action<Movie>>;
using ProgramObjectConstructor = Func<ProgramObject, int, int, int, Renderer>;
using Condition = Format.ButtonCondition.Condition;
using MovieEventHandlersDictionary = Dictionary<string, MovieEventHandlers>;
using ButtonEventHandlersDictionary = Dictionary<string, ButtonEventHandlers>;
using DetachHandler = Action<LWF>;
using Inspector = System.Action<Object, int, int, int>;
using AllowButtonList = Dictionary<int, bool>;
using DenyButtonList = Dictionary<int, bool>;

public class MovieEventHandlers
{
	public MovieEventHandler load;
	public MovieEventHandler postLoad;
	public MovieEventHandler unload;
	public MovieEventHandler enterFrame;
	public MovieEventHandler update;
	public MovieEventHandler render;

	public MovieEventHandlers() {}
	public MovieEventHandlers(MovieEventHandler l,
		MovieEventHandler p, MovieEventHandler u,
		MovieEventHandler e, MovieEventHandler up, MovieEventHandler r)
	{
		load = l;
		postLoad = p;
		unload = u;
		enterFrame = e;
		update = up;
		render = r;
	}
}

public class ButtonEventHandlers
{
	public ButtonEventHandler load;
	public ButtonEventHandler unload;
	public ButtonEventHandler enterFrame;
	public ButtonEventHandler update;
	public ButtonEventHandler render;
	public ButtonEventHandler rollOver;
	public ButtonEventHandler rollOut;
	public ButtonEventHandler press;
	public ButtonEventHandler release;
	public ButtonKeyPressHandler keyPress;

	public ButtonEventHandlers() {}
	public ButtonEventHandlers(ButtonEventHandler l, ButtonEventHandler u,
		ButtonEventHandler e, ButtonEventHandler up, ButtonEventHandler r,
		ButtonEventHandler rOver, ButtonEventHandler rOut, ButtonEventHandler p,
		ButtonEventHandler rl, ButtonKeyPressHandler k)
	{
		load = l;
		unload = u;
		enterFrame = e;
		update = up;
		render = r;
		rollOver = rOver;
		rollOut = rOut;
		press = p;
		release = rl;
		keyPress = k;
	}
}

public partial class LWF
{
	private static int EXEC_LIMIT = 10;
	private static float ROUND_OFF_TICK_RATE = 0.05f;

	private Data m_data;
	private IRendererFactory m_rendererFactory;
	private int m_rootMovieStringId;
	private Property m_property;
	private Movie m_rootMovie;
	private IObject[] m_instances;
	private Button m_focus;
	private Button m_buttonHead;
	private EventHandler[] m_eventHandlers;
	private MovieEventHandlers[] m_movieEventHandlers;
	private ButtonEventHandlers[] m_buttonEventHandlers;
	private MovieCommands m_movieCommands;
	private ProgramObjectConstructor[] m_programObjectConstructors;
	private MovieEventHandlersDictionary m_movieEventHandlersByFullName;
	private ButtonEventHandlersDictionary m_buttonEventHandlersByFullName;
	private DetachHandler m_detachHandler;
	private AllowButtonList m_allowButtonList;
	private DenyButtonList m_denyButtonList;
	private int m_renderingIndex;
	private int m_renderingIndexOffsetted;
	private int m_renderingCount;
	private int m_depth;
	private uint m_execCount;
	private uint m_updateCount;
	private double m_time;
	private float m_progress;
	private float m_tick;
	private float m_roundOffTick;
	private float m_thisTick;
	private Movie m_parent;
	private string m_attachName;
	private bool m_attachVisible;
	private bool m_execDisabled;
	private bool m_executedForExecDisabled;
	private bool m_interceptByNotAllowOrDenyButtons;
	private bool m_intercepted;
	private bool m_propertyDirty;
	private float m_pointX;
	private float m_pointY;
	private bool m_pressing;
	private Matrix m_matrix;
	private ColorTransform m_colorTransform;

	public Data data {get {return m_data;}}
	public bool interactive {get; set;}
	public float scaleByStage {get; set;}
	public bool isExecDisabled {get {return m_execDisabled;}}
	public bool attachVisible {get {return m_attachVisible;}}
	public bool isPropertyDirty {get {return m_propertyDirty;}}
	public bool isLWFAttached {get; set;}
	public object privateData {get; set;}
	public IRendererFactory rendererFactory
		{get {return m_rendererFactory;}}
	public Property property {get {return m_property;}}
	public Movie rootMovie {get {return m_rootMovie;}}
	public Button focus {get {return m_focus;}}
	public Button buttonHead {
		get {return m_buttonHead;}
		set {m_buttonHead = value;}
	}
	public float pointX {get {return m_pointX;}}
	public float pointY {get {return m_pointY;}}
	public bool pressing {get {return m_pressing;}}
	public int renderingIndex {get {return m_renderingIndex;}}
	public int renderingIndexOffsetted {get {return m_renderingIndexOffsetted;}}
	public int renderingCount {get {return m_renderingCount;}}
	public float width {get {return m_data.header.width;}}
	public float height {get {return m_data.header.height;}}
	public double time {get {return m_time;}}
	public float tick {get {return m_tick;}}
	public float thisTick {get {return m_thisTick;}}
	public uint updateCount {get {return m_updateCount;}}
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

	public LWF(Data lwfData, IRendererFactory rendererFactory = null)
	{
		m_data = lwfData;

		interactive = lwfData.buttonConditions.Length > 0;
		m_tick = 1.0f / m_data.header.frameRate;
		m_roundOffTick = m_tick * ROUND_OFF_TICK_RATE;
		m_attachVisible = true;
		m_interceptByNotAllowOrDenyButtons = true;
		m_intercepted = false;
		scaleByStage = 1.0f;
		m_pointX = Single.MinValue;
		m_pointY = Single.MinValue;
		m_pressing = false;

		if (!interactive && m_data.frames.Length == 1)
			DisableExec();

		m_property = new Property(this);
		m_instances = new IObject[m_data.instanceNames.Length];
		m_eventHandlers = new EventHandler[m_data.events.Length];
		m_movieEventHandlers = new MovieEventHandlers[m_instances.Length];
		m_buttonEventHandlers = new ButtonEventHandlers[m_instances.Length];
		m_movieCommands = new MovieCommands();
		m_programObjectConstructors =
			new ProgramObjectConstructor[m_data.programObjects.Length];

		m_matrix = new Matrix();
		m_colorTransform = new ColorTransform();

		Init();

		SetRendererFactory(rendererFactory);
	}

	public void SetRendererFactory(IRendererFactory rendererFactory = null)
	{
		if (rendererFactory == null)
			rendererFactory = new NullRendererFactory();
		m_rendererFactory = rendererFactory;
		m_rendererFactory.Init(this);
	}

	public void SetFrameRate(int frameRate)
	{
		if (frameRate == 0)
			return;
		m_tick = 1.0f / frameRate;
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

	public void SetAttachVisible(bool visible)
	{
		m_attachVisible = visible;
	}

	public void ClearFocus(Button button)
	{
		if (m_focus == button)
			m_focus = null;
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
	}

	private Matrix CalcMatrix(Matrix matrix)
	{
		Matrix m;
		Property p = m_property;
		if (p.hasMatrix) {
			if (matrix != null) {
				m = m_matrix;
				Utility.CalcMatrix(m, matrix, p.matrix);
			} else {
				m = p.matrix;
			}
		} else {
			m = matrix;
		}
		return m;
	}

	private ColorTransform CalcColorTransform(ColorTransform colorTransform)
	{
		ColorTransform c;
		Property p = m_property;
		if (p.hasColorTransform) {
			if (colorTransform != null) {
				c = m_colorTransform;
				Utility.CalcColorTransform(c, colorTransform, p.colorTransform);
			} else {
				c = p.colorTransform;
			}
		} else {
			c = colorTransform;
		}
		return c;
	}

	public int Exec(float tick = 0,
		Matrix matrix = null, ColorTransform colorTransform = null)
	{
		bool execed = false;
		float currentProgress = m_progress;

		if (m_execDisabled) {
			if (!m_executedForExecDisabled) {
				m_rootMovie.execCount = ++m_execCount;
				m_rootMovie.Exec();
				m_rootMovie.PostExec(true);
				m_executedForExecDisabled = true;
				execed = true;
			}
		} else {
			bool progressing = true;
			m_thisTick = tick;
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

			int execLimit = EXEC_LIMIT;
			while (m_progress >= m_tick - m_roundOffTick) {
				if (--execLimit < 0) {
					m_progress = 0;
					break;
				}
				m_progress -= m_tick;
				m_rootMovie.execCount = ++m_execCount;
				m_rootMovie.Exec();
				m_rootMovie.PostExec(progressing);
				execed = true;
			}

			if (m_progress < m_roundOffTick)
				m_progress = 0;

			if (interactive) {
				m_buttonHead = null;
				m_rootMovie.LinkButton();
			}
		}

		if (execed || isLWFAttached ||
				isPropertyDirty || matrix != null || colorTransform != null)
			Update(matrix, colorTransform);

		if (!m_execDisabled) {
			if (tick < 0)
				m_progress = currentProgress;
		}

		return m_renderingCount;
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
		Matrix m = CalcMatrix(matrix);
		ColorTransform c = CalcColorTransform(colorTransform);
		m_renderingIndex = 0;
		m_renderingIndexOffsetted = 0;
		m_rootMovie.Update(m, c);
		m_renderingCount = m_renderingIndex;
		m_thisTick = 0;
		m_propertyDirty = false;
		m_updateCount++;
	}

	public int Render(int rIndex = 0,
		int rCount = 0, int rOffset = Int32.MinValue)
	{
		int renderingCountBackup = m_renderingCount;
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
		m_renderingCount = renderingCountBackup;
		return m_renderingCount;
	}

	public int Inspect(Inspector inspector, int hierarchy = 0,
		int inspectDepth = 0, int rIndex = 0, int rCount = 0,
		int rOffset = Int32.MinValue)
	{
		int renderingCountBackup = m_renderingCount;
		if (rCount > 0)
			m_renderingCount = rCount;
		m_renderingIndex = rIndex;
		m_renderingIndexOffsetted = rIndex;
		if (m_property.hasRenderingOffset) {
			RenderOffset();
			rOffset = m_property.renderingOffset;
		}

		m_rootMovie.Inspect(inspector, hierarchy, inspectDepth, rOffset);
		m_renderingCount = renderingCountBackup;
		return m_renderingCount;
	}

	public void Destroy()
	{
		m_rootMovie.Destroy();
	}

	public int GetInstanceNameStringId(int instId)
	{
		if (instId < 0 || instId >= m_data.instanceNames.Length)
			return -1;
		return m_data.instanceNames[instId].stringId;
	}

	public int GetStringId(string str)
	{
		int i;
		if (m_data.stringMap.TryGetValue(str, out i))
			return i;
		else
			return -1;
	}

	public int SearchInstanceId(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.instanceNameMap.TryGetValue(stringId, out i))
			return i;
		else
			return -1;
	}

	public int SearchFrame(Movie movie, string label)
	{
		return SearchFrame(movie, GetStringId(label));
	}

	public int SearchFrame(Movie movie, int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int frameNo;
		Dictionary<int, int> labelMap = m_data.labelMap[movie.objectId];
		if (labelMap.TryGetValue(stringId, out frameNo))
			return frameNo + 1;
		else
			return -1;
	}

	public Dictionary<int, int> GetMovieLabels(Movie movie)
	{
		if (movie == null)
			return null;
		return m_data.labelMap[movie.objectId];
	}

	public int SearchMovieLinkage(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.movieLinkageMap.TryGetValue(stringId, out i))
			return m_data.movieLinkages[i].movieId;
		else
			return -1;
	}

	public string GetMovieLinkageName(int movieId)
	{
		int i;
		if (m_data.movieLinkageNameMap.TryGetValue(movieId, out i))
			return m_data.strings[i];
		else
			return null;
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

		return SearchMovieInstance(GetStringId(instanceName));
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

	public int SearchEventId(string eventName)
	{
		return SearchEventId(GetStringId(eventName));
	}

	public int SearchEventId(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.eventMap.TryGetValue(stringId, out i))
			return i;
		else
			return -1;
	}

	public int SearchProgramObjectId(string programObjectName)
	{
		return SearchProgramObjectId(GetStringId(programObjectName));
	}

	public int SearchProgramObjectId(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.programObjectMap.TryGetValue(stringId, out i))
			return i;
		else
			return -1;
	}

	public IObject GetInstance(int instId)
	{
		return m_instances[instId];
	}

	public void SetInstance(int instId, IObject instance)
	{
		m_instances[instId] = instance;
	}

	public void SetEventHandler(string eventName, EventHandler eventHandler)
	{
		SetEventHandler(SearchEventId(eventName), eventHandler);
	}

	public void SetEventHandler(int eventId, EventHandler eventHandler)
	{
		if (eventId < 0 || eventId >= m_data.events.Length)
			return;
		m_eventHandlers[eventId] = eventHandler;
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

	public MovieEventHandlers GetMovieEventHandlers(Movie m)
	{
		if (m_movieEventHandlersByFullName != null) {
			string fullName = m.GetFullName();
			if (fullName != null) {
				MovieEventHandlers handlers;
				if (m_movieEventHandlersByFullName.TryGetValue(
						fullName, out handlers)) {
					return handlers;
				}
			}
		}

		int instId = m.instanceId;
		if (instId < 0 || instId >= m_instances.Length)
			return null;
		return m_movieEventHandlers[instId];
	}

	public void SetMovieEventHandler(string instanceName,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			SetMovieEventHandler(
				instId, load, postLoad, unload, enterFrame, update, render);
			return;
		}

		if (!instanceName.Contains("."))
			return;

		if (m_movieEventHandlersByFullName == null)
			m_movieEventHandlersByFullName = new MovieEventHandlersDictionary();

		if (load != null || postLoad != null || unload != null ||
				enterFrame != null || update != null || render != null) {
			m_movieEventHandlersByFullName[instanceName] =
				new MovieEventHandlers(
					load, postLoad, unload, enterFrame, update, render);
		} else {
			m_movieEventHandlersByFullName.Remove(instanceName);
		}
	}

	public void SetMovieEventHandler(int instId,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		MovieEventHandlers handlers;
		if (load != null || postLoad != null || unload != null ||
				enterFrame != null || update != null || render != null) {
			handlers = new MovieEventHandlers(
				load, postLoad, unload, enterFrame, update, render);
		} else {
			handlers = null;
		}
		m_movieEventHandlers[instId] = handlers;

		if (instId == m_rootMovie.instanceId)
			m_rootMovie.SetHandlers(handlers);
	}

	public ButtonEventHandlers GetButtonEventHandlers(Button b)
	{
		if (m_buttonEventHandlersByFullName != null) {
			string fullName = b.GetFullName();
			if (fullName != null) {
				ButtonEventHandlers handlers;
				if (m_buttonEventHandlersByFullName.TryGetValue(
						fullName, out handlers)) {
					return handlers;
				}
			}
		}

		int instId = b.instanceId;
		if (instId < 0 || instId >= m_instances.Length)
			return null;
		return m_buttonEventHandlers[instId];
	}

	public void SetButtonEventHandler(string instanceName,
		ButtonEventHandler press = null, ButtonEventHandler release = null,
		ButtonEventHandler rollOver = null, ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null, ButtonEventHandler load = null,
		ButtonEventHandler unload = null, ButtonEventHandler enterFrame = null,
		ButtonEventHandler update = null, ButtonEventHandler render = null)
	{
		int instId = SearchInstanceId(GetStringId(instanceName));
		if (instId >= 0) {
			SetButtonEventHandler(instId, press, release, rollOver, rollOut,
				keyPress, load, unload, enterFrame, update, render);
			return;
		}

		if (!instanceName.Contains("."))
			return;

		if (m_buttonEventHandlersByFullName == null)
			m_buttonEventHandlersByFullName =
				new ButtonEventHandlersDictionary();

		if (load != null ||
				unload != null || enterFrame != null || update != null ||
				render != null || rollOver != null || rollOut != null ||
				press != null || release != null || keyPress != null) {
			m_buttonEventHandlersByFullName[instanceName] =
				new ButtonEventHandlers(load, unload, enterFrame, update,
					render, rollOver, rollOut, press, release, keyPress);
		} else {
			m_buttonEventHandlersByFullName.Remove(instanceName);
		}
	}

	public void SetButtonEventHandler(int instId,
		ButtonEventHandler press = null, ButtonEventHandler release = null,
		ButtonEventHandler rollOver = null, ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null, ButtonEventHandler load = null,
		ButtonEventHandler unload = null, ButtonEventHandler enterFrame = null,
		ButtonEventHandler update = null, ButtonEventHandler render = null)
	{
		if (instId < 0 || instId >= m_instances.Length)
			return;

		ButtonEventHandlers handlers;
		if (load != null || unload != null || enterFrame != null ||
				update != null || render != null || rollOver != null ||
				rollOut != null || press != null || release != null ||
				keyPress != null) {
			handlers = new ButtonEventHandlers(load, unload, enterFrame, update,
				render, rollOver, rollOut, press, release, keyPress);
		} else {
			handlers = null;
		}
		m_buttonEventHandlers[instId] = handlers;
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
}

}	// namespace LWF
