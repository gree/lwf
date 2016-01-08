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

using UnityEngine;
using System;
using System.IO;

using ResourceCache = LWF.UnityRenderer.ResourceCache;
using LWFDataLoader = System.Func<string, byte[]>;
using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;
using LWFDataCallback = System.Func<LWF.Data, bool>;
using LWFCallback = System.Action<LWFObject>;
using LWFCallbacks = System.Collections.Generic.List<System.Action<LWFObject>>;

using EventHandler = System.Action<LWF.Movie, LWF.Button>;
using EventResultHandler = System.Action<int>;
using MovieEventHandler = System.Action<LWF.Movie>;
using ButtonEventHandler = System.Action<LWF.Button>;
using ButtonKeyPressHandler = System.Action<LWF.Button, int>;
using MovieCommand = System.Action<LWF.Movie>;
using ProgramObjectConstructor =
	System.Func<LWF.ProgramObject, int, int, int, LWF.Renderer>;
using LWFObjectAttachHandler = System.Action<LWFObject>;
using LWFObjectDetachHandler = System.Func<LWFObject, bool>;

#if UNITY_EDITOR
using UnityEditor;
#endif

class HandlerWrapper
{
	public int id;
}

public class LWFObject : MonoBehaviour
{
	protected enum RendererType {
		CombinedMeshRenderer,
		DrawMeshRenderer,
		UIVertexRenderer,
	};
	public LWF.LWF lwf;
	public LWF.UnityRenderer.Factory factory;
	[HideInInspector] [NonSerialized] public bool isAlive;

	public string sortingLayerName {
		get {return mSortingLayerName;}
		set {
			if (string.Compare(mSortingLayerName, value) != 0) {
				mSortingLayerName = value;
				mDirty = true;
			}
		}
	}

	public int sortingOrder {
		get {return mSortingOrder;}
		set {
			if (mSortingOrder != value) {
				mSortingOrder = value;
				mDirty = true;
			}
		}
	}

	protected bool callUpdate;
	protected RendererType rendererType;
	protected bool executed;
	protected LWFCallbacks lwfLoadCallbacks;
	protected LWFCallbacks lwfDestroyCallbacks;
	protected int activateCount = 1;
	protected int resumeCount = 1;
	[HideInInspector] [SerializeField] private string mSortingLayerName;
	[HideInInspector] [SerializeField] private int mSortingOrder;
	private bool mDirty;
	private int mLayer;

	public LWFObject()
	{
		rendererType = RendererType.CombinedMeshRenderer;
		isAlive = true;
		lwfLoadCallbacks = new LWFCallbacks();
		lwfDestroyCallbacks = new LWFCallbacks();
	}

	public virtual void OnDestroy()
	{
		isAlive = false;

		foreach (var c in lwfDestroyCallbacks)
			c(this);
		lwfDestroyCallbacks = null;

		if (lwf != null) {
			lwf.Destroy();
			lwf = null;
		}
	}

	public void UseCombinedMeshRenderer()
	{
		rendererType = RendererType.CombinedMeshRenderer;
	}

	public void UseDrawMeshRenderer()
	{
		rendererType = RendererType.DrawMeshRenderer;
	}

	public void UseUIVertexRenderer()
	{
		rendererType = RendererType.UIVertexRenderer;
	}

	public void SetAutoUpdate(bool autoUpdate)
	{
		callUpdate = autoUpdate;
	}

	public virtual bool Load(string path,
		string texturePrefix = null, string fontPrefix = "",
		float zOffset = 0, float zRate = 1, int renderQueueOffset = 0,
		Camera renderCamera = null, Camera inputCamera = null,
		bool autoUpdate = true, bool useAdditionalColor = false,
		LWFDataCallback lwfDataCallback = null,
		LWFCallback lwfLoadCallback = null,
		LWFCallback lwfDestroyCallback = null,
		LWFDataLoader lwfDataLoader = null,
		TextureLoader textureLoader = null,
		TextureUnloader textureUnloader = null,
		string shaderName = "LWF"
#if LWF_USE_LUA
		, object luaState = null
#endif
		)
	{
		callUpdate = autoUpdate;
		if (inputCamera == null)
			inputCamera = Camera.main;

		if (texturePrefix == null)
			texturePrefix = Path.GetDirectoryName(path) + "/";
		if (lwfLoadCallback != null)
			lwfLoadCallbacks.Add(lwfLoadCallback);
		if (lwfDestroyCallback != null)
			lwfDestroyCallbacks.Add(lwfDestroyCallback);

		ResourceCache cache = ResourceCache.SharedInstance();
		LWF.Data data = cache.LoadLWFData(path, lwfDataLoader);
		if (data == null || !data.Check())
			return false;

		if (lwfDataCallback != null && !lwfDataCallback(data))
			return false;

		RendererType rt = rendererType;
#if UNITY_EDITOR
		if (!Application.isPlaying && rt == RendererType.CombinedMeshRenderer)
			rt = RendererType.DrawMeshRenderer;
#endif
		if (rt == RendererType.CombinedMeshRenderer) {
			factory = new LWF.CombinedMeshRenderer.Factory(
				data, gameObject, zOffset, zRate, renderQueueOffset,
				mSortingLayerName, mSortingOrder, useAdditionalColor,
				renderCamera, inputCamera, texturePrefix, fontPrefix,
				textureLoader, textureUnloader, shaderName);
		} else if (rt == RendererType.DrawMeshRenderer) {
			factory = new LWF.DrawMeshRenderer.Factory(
				data, gameObject, zOffset, zRate, renderQueueOffset,
				mSortingLayerName, mSortingOrder, useAdditionalColor,
				renderCamera, inputCamera, texturePrefix, fontPrefix,
				textureLoader, textureUnloader, shaderName);
		} else /*if (rt == RendererType.UIVertexRenderer)*/ {
			factory = new LWF.UIVertexRenderer.Factory(
				data, gameObject, zOffset, zRate, renderQueueOffset,
				mSortingLayerName, mSortingOrder, useAdditionalColor,
				renderCamera, inputCamera, texturePrefix, fontPrefix,
				textureLoader, textureUnloader, shaderName);
		}

#if LWF_USE_LUA
		lwf = new LWF.LWF(data, factory, luaState);
#else
		lwf = new LWF.LWF(data, factory);
#endif

		lwf.lwfLoader = (childPath, childTexturePrefix) => {
			LWF.Data childData = cache.LoadLWFData(childPath, lwfDataLoader);
			if (childData == null || !childData.Check())
				return null;

			if (lwfDataCallback != null && !lwfDataCallback(childData))
				return null;

			if (childTexturePrefix == null)
				childTexturePrefix = Path.GetDirectoryName(childPath) + "/";

			LWF.UnityRenderer.Factory f;
			if (rt == RendererType.CombinedMeshRenderer) {
				f = new LWF.CombinedMeshRenderer.Factory(
					childData, gameObject, factory.zOffset, factory.zRate,
					factory.renderQueueOffset, mSortingLayerName, mSortingOrder,
					factory.useAdditionalColor, factory.renderCamera,
					factory.inputCamera, childTexturePrefix, factory.fontPrefix,
					factory.textureLoader, factory.textureUnloader, shaderName, true);
			} else if (rt == RendererType.DrawMeshRenderer) {
				f = new LWF.DrawMeshRenderer.Factory(
					childData, gameObject, factory.zOffset, factory.zRate,
					factory.renderQueueOffset, mSortingLayerName, mSortingOrder,
					factory.useAdditionalColor, factory.renderCamera,
					factory.inputCamera, childTexturePrefix, factory.fontPrefix,
					factory.textureLoader, factory.textureUnloader, shaderName);
			} else /*if (rt == RendererType.UIVertexRenderer)*/ {
				f = new LWF.UIVertexRenderer.Factory(
					childData, gameObject, factory.zOffset, factory.zRate,
					factory.renderQueueOffset, mSortingLayerName, mSortingOrder,
					factory.useAdditionalColor, factory.renderCamera,
					factory.inputCamera, childTexturePrefix, factory.fontPrefix,
					factory.textureLoader, factory.textureUnloader, shaderName);
			}

#if LWF_USE_LUA
			LWF.LWF child = new LWF.LWF(childData, f, lwf.luaState);
#else
			LWF.LWF child = new LWF.LWF(childData, f);
#endif
			child.lwfLoader = lwf.lwfLoader;
			child.lwfUnloader = () => {
				ResourceCache.SharedInstance().UnloadLWFData(childPath);
			};
			return child;
		};
		lwf.lwfUnloader = () => {
			ResourceCache.SharedInstance().UnloadLWFData(path);
		};

		OnLoad();

		return true;
	}

	public void AddLoadCallback(LWFCallback lwfLoadCallback)
	{
		if (lwf != null)
			lwfLoadCallback(this);
		else
			lwfLoadCallbacks.Add(lwfLoadCallback);
	}

	public void AddDestroyCallback(LWFCallback lwfDestroyCallback)
	{
		lwfDestroyCallbacks.Add(lwfDestroyCallback);
	}

	public virtual void OnLoad()
	{
		foreach (var c in lwfLoadCallbacks)
			c(this);
		lwfLoadCallbacks = null;
	}

	public string GetRendererName()
	{
		return factory == null ? null : factory.GetType().FullName;
	}

	public virtual void FitForHeight(int stageHeight)
	{
		AddLoadCallback((o) => lwf.FitForHeight(stageHeight));
	}

	public virtual void FitForWidth(int stageWidth)
	{
		AddLoadCallback((o) => lwf.FitForWidth(stageWidth));
	}

	public virtual void ScaleForHeight(int stageHeight)
	{
		AddLoadCallback((o) => lwf.ScaleForHeight(stageHeight));
	}

	public virtual void ScaleForWidth(int stageWidth)
	{
		AddLoadCallback((o) => lwf.ScaleForWidth(stageWidth));
	}

	public virtual void UpdateLWF(float tick, int pointX = Int32.MinValue,
		int pointY = Int32.MinValue, bool press = false, bool release = false)
	{
		if (lwf == null)
			return;

		if (lwf.interactive) {
			if (pointX != Int32.MinValue && pointY != Int32.MinValue) {
				lwf.InputPoint(pointX, pointY);
				if (lwf == null)
					return;
			}
			if (press) {
				lwf.InputPress();
				if (lwf == null)
					return;
			}
			if (release)
				lwf.InputRelease();
		}

		if (lwf == null)
			return;

		if (!executed || resumeCount > 0) {
			if (tick == 0)
				tick = -1;
			lwf.Exec(tick);
			executed = true;
		}

		if (lwf == null)
			return;

		if (mDirty) {
			factory.sortingLayerName = mSortingLayerName;
			factory.sortingOrder = mSortingOrder;
			factory.UpdateSortingLayerAndOrder();
			mDirty = false;
		}

		int layer = gameObject.layer;
		if (mLayer != layer) {
			factory.UpdateLayer();
			mLayer = layer;
		}

		lwf.Render();
	}

	public virtual void Update()
	{
		if (!callUpdate || lwf == null)
			return;

		int pointX = Int32.MinValue;
		int pointY = Int32.MinValue;
		bool press = false;
		bool release = false;

		if (lwf.interactive && factory.inputCamera != null) {
			bool down = Input.GetButton("Fire1");
			press = Input.GetButtonDown("Fire1");
			release = Input.GetButtonUp("Fire1");
			if (down) {
				Vector3 screenPos = Input.mousePosition;
				Vector3 worldPos =
					factory.inputCamera.ScreenToWorldPoint(screenPos);
				Matrix4x4 matrix = gameObject.transform.worldToLocalMatrix;
				Vector3 pos = matrix.MultiplyPoint(worldPos);
				pointX = (int)pos.x;
				pointY = -(int)pos.y;
			}
		}

		UpdateLWF(Time.deltaTime, pointX, pointY, press, release);
	}

	public void SetText(
		string instanceName, string textName, string text)
	{
		AddLoadCallback((o) =>
			lwf.SetText(instanceName + "." + textName, text));
	}

	public void SetText(string textName, string text)
	{
		AddLoadCallback((o) => lwf.SetText(textName, text));
	}

	public string GetText(string instanceName, string textName)
	{
		if (lwf == null)
			return null;
		return lwf.GetText(instanceName + "." + textName);
	}

	public string GetText(string textName)
	{
		if (lwf == null)
			return null;
		return lwf.GetText(textName);
	}

	public Vector3 WorldToLWFPoint(Vector3 worldPoint)
	{
		return factory.WorldToLWFPoint(lwf, worldPoint);
	}

	public Vector3 ScreenToLWFPoint(Vector3 screenPoint)
	{
		Camera camera = factory.inputCamera;
		Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);
		return WorldToLWFPoint(worldPoint);
	}

	public void AttachMovie(string instanceName, string linkageName,
		string attachName, int attachDepth = -1, bool reorder = false,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.AttachMovie(linkageName, attachName, attachDepth, reorder,
				load, postLoad, unload, enterFrame, update, render);
		});
	}

	public void AttachEmptyMovie(string instanceName,
		string attachName, int attachDepth = -1, bool reorder = false,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.AttachEmptyMovie(attachName, attachDepth, reorder,
				load, postLoad, unload, enterFrame, update, render);
		});
	}

	public void SwapAttachedMovieDepth(
		string instanceName, int depth0, int depth1)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.SwapAttachedMovieDepth(depth0, depth1);
		});
	}

	public void DetachMovie(string instanceName, string attachName)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.DetachMovie(attachName);
		});
	}

	public void DetachMovie(string instanceName, int attachDepth)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.DetachMovie(attachDepth);
		});
	}

	public void DetachFromParent(string instanceName)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.DetachFromParent();
		});
	}

	public void AttachLWF(string instanceName, string path, string attachName,
		int attachDepth = -1, bool reorder = false, string texturePrefix = null)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.AttachLWF(path, attachName, attachDepth, reorder, texturePrefix);
		});
	}

	public void SwapAttachedLWFDepth(
		string instanceName, int depth0, int depth1)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.SwapAttachedLWFDepth(depth0, depth1);
		});
	}

	public void DetachLWF(string instanceName, string attachName)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.DetachLWF(attachName);
		});
	}

	public void DetachLWF(string instanceName, LWFObject lwfObject)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.DetachLWF(lwfObject.lwf);
		});
	}

	public void DetachAllLWFs(string instanceName)
	{
		AddMovieLoadHandler(instanceName, (m) => {
			m.DetachAllLWFs();
		});
	}

	public virtual void Pause()
	{
		--resumeCount;
	}

	public virtual void Resume()
	{
		++resumeCount;
	}

	public virtual void Activate()
	{
		AddLoadCallback((o) => {
			++activateCount;
			if (activateCount == 1) {
				bool attachVisible = lwf.attachVisible;
				lwf.SetAttachVisible(true);
				lwf.Render();
				lwf.SetAttachVisible(attachVisible);
				gameObject.SetActive(true);
			}
		});
	}

	public virtual void Deactivate()
	{
		AddLoadCallback((o) => {
			--activateCount;
			if (activateCount == 0) {
				bool attachVisible = lwf.attachVisible;
				lwf.SetAttachVisible(false);
				lwf.Render();
				lwf.SetAttachVisible(attachVisible);
				gameObject.SetActive(false);
			}
		});
	}

	public static void SetLoader(LWFDataLoader lwfDataLoader = null,
		TextureLoader textureLoader = null,
		TextureUnloader textureUnloader = null)
	{
		ResourceCache cache = ResourceCache.SharedInstance();
		cache.SetLoader(lwfDataLoader, textureLoader, textureUnloader);
	}

	public void AddEventHandler(string eventName,
		EventHandler eventHandler, EventResultHandler resultHandler = null)
	{
		AddLoadCallback((o) => {
			int id = lwf.AddEventHandler(eventName, eventHandler);
			if (resultHandler != null)
				resultHandler(id);
		});
	}

	public void RemoveEventHandler(string eventName, int id)
	{
		AddLoadCallback((o) => {lwf.RemoveEventHandler(eventName, id);});
	}

	public void ClearEventHandler(string eventName)
	{
		AddLoadCallback((o) => {lwf.ClearEventHandler(eventName);});
	}

	public void SetEventHandler(string eventName,
		EventHandler eventHandler, EventResultHandler resultHandler = null)
	{
		AddLoadCallback((o) => {
			int id = lwf.SetEventHandler(eventName, eventHandler);
			if (resultHandler != null)
				resultHandler(id);
		});
	}

	public void SetProgramObjectConstructor(string programObjectName,
		ProgramObjectConstructor programObjectConstructor)
	{
		AddLoadCallback((o) => {lwf.SetProgramObjectConstructor(
			programObjectName, programObjectConstructor);});
	}

	public void AddMovieEventHandler(string instanceName,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null,
		EventResultHandler resultHandler = null)
	{
		AddLoadCallback((o) => {
			int id = lwf.AddMovieEventHandler(instanceName,
				load, postLoad, unload, enterFrame, update, render);
			if (resultHandler != null)
				resultHandler(id);
		});
	}

	public void RemoveMovieEventHandler(string instanceName, int id)
	{
		AddLoadCallback((o) =>
			{lwf.RemoveMovieEventHandler(instanceName, id);});
	}

	public void ClearMovieEventHandler(string instanceName)
	{
		AddLoadCallback((o) => {lwf.ClearMovieEventHandler(instanceName);});
	}

	public void ClearMovieEventHandler(string instanceName,
		LWF.MovieEventHandlers.Type type)
	{
		AddLoadCallback(
			(o) => {lwf.ClearMovieEventHandler(instanceName, type);});
	}

	public void SetMovieEventHandler(string instanceName,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null,
		EventResultHandler resultHandler = null)
	{
		AddLoadCallback((o) => {
			int id = lwf.SetMovieEventHandler(instanceName,
				load, postLoad, unload, enterFrame, update, render);
			if (resultHandler != null)
				resultHandler(id);
		});
	}

	public void AddButtonEventHandler(string instanceName,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null,
		EventResultHandler resultHandler = null)
	{
		AddLoadCallback((o) => {
			int id = lwf.AddButtonEventHandler(
				instanceName, load, unload, enterFrame, update, render,
					press, release, rollOver, rollOut, keyPress);
			if (resultHandler != null)
				resultHandler(id);
		});
	}

	public void RemoveButtonEventHandler(string instanceName, int id)
	{
		AddLoadCallback((o) =>
			{lwf.RemoveButtonEventHandler(instanceName, id);});
	}

	public void ClearButtonEventHandler(string instanceName)
	{
		AddLoadCallback((o) => {lwf.ClearButtonEventHandler(instanceName);});
	}

	public void ClearButtonEventHandler(string instanceName,
		LWF.ButtonEventHandlers.Type type)
	{
		AddLoadCallback(
			(o) => {lwf.ClearButtonEventHandler(instanceName, type);});
	}

	public void SetButtonEventHandler(string instanceName,
		ButtonEventHandler load = null, ButtonEventHandler unload = null,
		ButtonEventHandler enterFrame = null, ButtonEventHandler update = null,
		ButtonEventHandler render = null, ButtonEventHandler press = null,
		ButtonEventHandler release = null, ButtonEventHandler rollOver = null,
		ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null,
		EventResultHandler resultHandler = null)
	{
		AddLoadCallback((o) => {
			int id = lwf.SetButtonEventHandler(
				instanceName, load, unload, enterFrame, update, render,
					press, release, rollOver, rollOut, keyPress);
			if (resultHandler != null)
				resultHandler(id);
		});
	}

	public void Init()
	{
		AddLoadCallback((o) => {lwf.Init();});
	}

	public void SetMovieCommand(string[] instanceNames, MovieCommand cmd)
	{
		AddLoadCallback((o) => {lwf.SetMovieCommand(instanceNames, cmd);});
	}

	public void ClearProperty()
	{
		AddLoadCallback((o) => {lwf.property.Clear();});
	}

	public void Move(float x, float y)
	{
		AddLoadCallback((o) => {lwf.property.Move(x, y);});
	}

	public void MoveTo(float x, float y)
	{
		AddLoadCallback((o) => {lwf.property.MoveTo(x, y);});
	}

	public void Rotate(float degree)
	{
		AddLoadCallback((o) => {lwf.property.Rotate(degree);});
	}

	public void RotateTo(float degree)
	{
		AddLoadCallback((o) => {lwf.property.RotateTo(degree);});
	}

	public void Scale(float x, float y)
	{
		AddLoadCallback((o) => {lwf.property.Scale(x, y);});
	}

	public void ScaleTo(float x, float y)
	{
		AddLoadCallback((o) => {lwf.property.ScaleTo(x, y);});
	}

	public void SetMatrix(LWF.Matrix m)
	{
		AddLoadCallback((o) => {lwf.property.SetMatrix(m);});
	}

	public void SetAlpha(float alpha)
	{
		AddLoadCallback((o) => {lwf.property.SetAlpha(alpha);});
	}

	public void SetColorTransform(LWF.ColorTransform c)
	{
		AddLoadCallback((o) => {lwf.property.SetColorTransform(c);});
	}

	public void AddMovieLoadHandler(
		string instanceName, MovieEventHandler handler, bool immortal = false)
	{
		AddLoadCallback((o) => {
			HandlerWrapper w = new HandlerWrapper();
			MovieEventHandler h = (m) => {
				if (!immortal)
					lwf.RemoveMovieEventHandler(instanceName, w.id);
				handler(m);
			};

			LWF.Movie movie = lwf[instanceName];
			if (movie != null) {
				handler(movie);
				if (immortal)
					w.id = lwf.AddMovieEventHandler(instanceName, load:h);
			} else {
				w.id = lwf.AddMovieEventHandler(instanceName, load:h);
			}
		});
	}

	public void PlayMovie(string instanceName, bool immortal = false)
	{
		AddMovieLoadHandler(instanceName, (m) => {m.Play();}, immortal);
	}

	public void StopMovie(string instanceName, bool immortal = false)
	{
		AddMovieLoadHandler(instanceName, (m) => {m.Stop();}, immortal);
	}

	public void NextFrameMovie(string instanceName, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.NextFrame();}, immortal);
	}

	public void PrevFrameMovie(string instanceName, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.PrevFrame();}, immortal);
	}

	public void SetVisibleMovie(string instanceName,
		bool visible, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.SetVisible(visible);}, immortal);
	}

	public void GotoAndStopMovie(string instanceName,
		string label, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.GotoAndStop(label);}, immortal);
	}

	public void GotoAndStopMovie(string instanceName,
		int frameNo, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.GotoAndStop(frameNo);}, immortal);
	}

	public void GotoAndPlayMovie(string instanceName,
		string label, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.GotoAndPlay(label);}, immortal);
	}

	public void GotoAndPlayMovie(string instanceName,
		int frameNo, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.GotoAndPlay(frameNo);}, immortal);
	}

	public void MoveMovie(string instanceName,
		float vx, float vy, bool immortal = false)
	{
		AddMovieLoadHandler(instanceName, (m) => {m.Move(vx, vy);}, immortal);
	}

	public void MoveToMovie(string instanceName,
		float vx, float vy, bool immortal = false)
	{
		AddMovieLoadHandler(instanceName, (m) => {m.MoveTo(vx, vy);}, immortal);
	}

	public void RotateMovie(string instanceName,
		float degree, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.Rotate(degree);}, immortal);
	}

	public void RotateToMovie(string instanceName,
		float degree, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.RotateTo(degree);}, immortal);
	}

	public void ScaleMovie(string instanceName,
		float vx, float vy, bool immortal = false)
	{
		AddMovieLoadHandler(instanceName, (m) => {m.Scale(vx, vy);}, immortal);
	}

	public void ScaleToMovie(string instanceName,
		float vx, float vy, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.ScaleTo(vx, vy);}, immortal);
	}

	public void SetMatrixMovie(string instanceName,
		LWF.Matrix matrix, float sx = 1, float sy = 1, float r = 0,
		bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.SetMatrix(matrix, sx, sy, r);}, immortal);
	}

	public void SetAlphaMovie(string instanceName,
		float v, bool immortal = false)
	{
		AddMovieLoadHandler(instanceName, (m) => {m.SetAlpha(v);}, immortal);
	}

	public void SetColorTransformMovie(
		string instanceName, LWF.ColorTransform c, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.SetColorTransform(c);}, immortal);
	}

	public void SetRenderingOffsetMovie(string instanceName,
		int rOffset, bool immortal = false)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.SetRenderingOffset(rOffset);}, immortal);
	}

	public void RequestCalculateBoundsMovie(string instanceName,
		MovieEventHandler callback = null)
	{
		AddMovieLoadHandler(
			instanceName, (m) => {m.RequestCalculateBounds(callback);});
	}

#if UNITY_EDITOR
	public virtual void OnEnable()
	{
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	public virtual void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	public virtual void OnSceneGUI(SceneView sceneView)
	{
		if (lwf != null)
			lwf.RenderNow();
	}
#endif
}
