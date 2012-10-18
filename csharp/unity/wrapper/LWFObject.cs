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

using ResourceCache = LWF.UnityRenderer.ResourceCache;
using LWFDataLoader = System.Func<string, byte[]>;
using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;
using LWFDataCallback = System.Func<LWF.Data, bool>;
using BitmapFontDataLoader = System.Func<string, byte[]>;
using BitmapFontTextureLoader = System.Func<string, UnityEngine.Texture2D>;
using LWFLoadCallback = System.Action<LWFObject>;
using LWFLoadCallbacks =
	System.Collections.Generic.List<System.Action<LWFObject>>;

using EventHandler = System.Action<LWF.Movie, LWF.Button>;
using MovieEventHandler = System.Action<LWF.Movie>;
using ButtonEventHandler = System.Func<LWF.Button, bool>;
using ButtonKeyPressHandler = System.Func<LWF.Button, int, bool>;
using MovieCommand = System.Action<LWF.Movie>;
using ProgramObjectConstructor =
	System.Func<LWF.ProgramObject, int, int, int, LWF.Renderer>;
using LWFObjectDetachHandler = System.Func<LWFObject, bool>;
using RendererFactoryConstructor =
	System.Func<RendererFactoryArguments, LWF.UnityRenderer.Factory>;

public class RendererFactoryArguments
{
	public LWF.Data data;
	public GameObject gameObject;
	public float zOffset;
	public float zRate;
	public int renderQueueOffset;
	public Camera camera;
	public string texturePrefix;
	public string fontPrefix;
	public TextureLoader textureLoader;
	public TextureUnloader textureUnloader;

	public RendererFactoryArguments(LWF.Data d, GameObject gObj, float zOff,
		float zR, int rQOff, Camera cam, string texturePrfx, string fontPrfx,
		TextureLoader textureLdr, TextureUnloader textureUnldr)
	{
		data = d;
		gameObject = gObj;
		zOffset = zOff;
		zRate = zR;
		renderQueueOffset = rQOff;
		camera = cam;
		texturePrefix = texturePrfx;
		fontPrefix = fontPrfx;
		textureLoader = textureLdr;
		textureUnloader = textureUnldr;
	}
}

public class LWFObject : MonoBehaviour
{
	public LWF.LWF lwf;
	public LWF.UnityRenderer.Factory factory;
	public string lwfName;
	public bool isAlive;
	protected RendererFactoryConstructor rendererFactoryConstructor;
	protected bool callUpdate;
	protected bool useCombinedMeshRenderer;
	protected LWFLoadCallbacks lwfLoadCallbacks;
	protected int activateCount = 1;
	protected int resumeCount = 1;

	public LWFObject()
	{
		useCombinedMeshRenderer = true;
		isAlive = true;
		lwfLoadCallbacks = new LWFLoadCallbacks();
	}

	public virtual void OnDestroy()
	{
		isAlive = false;

		if (lwfName == null)
			return;

		if (lwf != null) {
			lwf.Destroy();
			lwf = null;
		}

		if (factory != null) {
			factory.Destruct();
			factory = null;
		}

		ResourceCache.SharedInstance().UnloadLWFData(lwfName);
	}

	public void SetRendererFactoryConstructor(RendererFactoryConstructor c)
	{
		rendererFactoryConstructor = c;
	}

	public void UseCombinedMeshRenderer()
	{
		useCombinedMeshRenderer = true;
	}

	public void UseDrawMeshRenderer()
	{
		useCombinedMeshRenderer = false;
	}

	public void SetAutoUpdate(bool autoUpdate)
	{
		callUpdate = autoUpdate;
	}

	public virtual bool Load(string path,
		string texturePrefix = "", string fontPrefix = "",
		float zOffset = 0, float zRate = 1, int renderQueueOffset = 0,
		Camera camera = null, bool autoUpdate = true,
		LWFDataCallback lwfDataCallback = null,
		LWFLoadCallback lwfLoadCallback = null,
		LWFDataLoader lwfDataLoader = null,
		TextureLoader textureLoader = null,
		TextureUnloader textureUnloader = null)
	{
		lwfName = path;
		callUpdate = autoUpdate;
		if (camera == null)
			camera = Camera.main;

		if (lwfLoadCallback != null)
			lwfLoadCallbacks.Add(lwfLoadCallback);

		LWF.Data data =
			ResourceCache.SharedInstance().LoadLWFData(lwfName, lwfDataLoader);
		if (data == null || !data.Check())
			return false;

		if (lwfDataCallback != null && !lwfDataCallback(data))
			return false;

		if (rendererFactoryConstructor != null) {
			RendererFactoryArguments arg = new RendererFactoryArguments(
				data, gameObject, zOffset, zRate, renderQueueOffset, camera,
				texturePrefix, fontPrefix, textureLoader, textureUnloader);
			factory = rendererFactoryConstructor(arg);
		} else if (useCombinedMeshRenderer && data.textures.Length == 1) {
			factory = new LWF.CombinedMeshRenderer.Factory(
				data, gameObject, zOffset, zRate, renderQueueOffset, camera,
				texturePrefix, fontPrefix, textureLoader, textureUnloader);
		} else {
			factory = new LWF.DrawMeshRenderer.Factory(
				data, gameObject, zOffset, zRate, renderQueueOffset, camera,
				texturePrefix, fontPrefix, textureLoader, textureUnloader);
		}

		lwf = new LWF.LWF(data, factory);

		OnLoad();

		return true;
	}

	public void AddLoadCallback(LWFLoadCallback lwfLoadCallback)
	{
		if (lwf != null)
			lwfLoadCallback(this);
		else
			lwfLoadCallbacks.Add(lwfLoadCallback);
	}

	public virtual void OnLoad()
	{
		foreach (LWFLoadCallback lwfLoadCallback in lwfLoadCallbacks)
			lwfLoadCallback(this);
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
			if (press)
				lwf.InputPress();
			else if (release)
				lwf.InputRelease();
		}

		if (lwf == null)
			return;

		if (resumeCount > 0)
			lwf.Exec(tick);

		if (lwf == null)
			return;

		lwf.Render();
	}

	void Update()
	{
		if (!callUpdate || lwf == null)
			return;

		int pointX = Int32.MinValue;
		int pointY = Int32.MinValue;
		bool press = false;
		bool release = false;

		if (lwf.interactive) {
			bool down = Input.GetButton("Fire1");
			press = Input.GetButtonDown("Fire1");
			release = Input.GetButtonUp("Fire1");
			if (down) {
				Vector3 screenPos = Input.mousePosition;
				Vector3 worldPos = factory.camera.ScreenToWorldPoint(screenPos);
				Matrix4x4 matrix = gameObject.transform.worldToLocalMatrix;
				Vector3 pos = matrix.MultiplyPoint(worldPos);
				pointX = (int)pos.x;
				pointY = -(int)pos.y;
			}
		}

		UpdateLWF(Time.deltaTime, pointX, pointY, press, release);
	}

	public void UseTextWithMovie(string instanceName)
	{
		AddLoadCallback((o) => factory.UseTextWithMovie(instanceName));
	}

	public void SetText(
		string instanceName, string textName, string text)
	{
		AddLoadCallback((o) => factory.SetText(instanceName, textName, text));
	}

	public void SetText(string textName, string text)
	{
		AddLoadCallback((o) => factory.SetText(textName, text));
	}

	public string GetText(string instanceName, string textName)
	{
		if (lwf == null)
			return null;
		return factory.GetText(instanceName, textName);
	}

	public string GetText(string textName)
	{
		if (lwf == null)
			return null;
		return factory.GetText(textName);
	}

	public Vector3 WorldToLWFPoint(Vector3 worldPoint)
	{
		return factory.WorldToLWFPoint(lwf, worldPoint);
	}

	public Vector3 ScreenToLWFPoint(Vector3 screenPoint)
	{
		Camera camera = factory.camera;
		Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);
		return WorldToLWFPoint(worldPoint);
	}

	public void AttachLWF(LWF.Movie movie, LWFObject lwfObject,
		string attachName, int attachDepth = -1, bool reorder = false,
		LWFObjectDetachHandler detachHandler = null)
	{
		AddLoadCallback((o) => {
			lwfObject.AddLoadCallback((lo) => {
				movie.AttachLWF(lwfObject.lwf,
						attachName, attachDepth, reorder, (attachedLWF) => {
					if (detachHandler == null) {
						if (lwfObject.isAlive)
							Destroy(lwfObject.gameObject);
					} else {
						if (detachHandler(lwfObject) && lwfObject.isAlive)
							Destroy(lwfObject.gameObject);
					}
				});

				lwfObject.callUpdate = false;

				Transform transform = lwfObject.gameObject.transform;
				transform.parent = lwfObject.transform;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
			});
		});
	}

	public void DetachLWF(LWF.Movie movie, string attachName)
	{
		if (movie == null)
			return;

		movie.DetachLWF(attachName);
	}

	public void DetachLWF(LWF.Movie movie, LWFObject lwfObject)
	{
		if (movie == null)
			return;

		movie.DetachLWF(lwfObject.lwf);
	}

	public void DetachAllLWFs(LWF.Movie movie)
	{
		if (movie == null)
			return;

		movie.DetachAllLWFs();
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
				gameObject.active = true;
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
				gameObject.active = false;
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

	public static void SetBitmapFontLoader(
		BitmapFontDataLoader dataLoader = null,
		BitmapFontTextureLoader textureLoader = null)
	{
		BitmapFont.ResourceCache cache =
			BitmapFont.ResourceCache.SharedInstance();
		cache.SetLoader(dataLoader, textureLoader);
	}

	public void SetEventHandler(string eventName, EventHandler eventHandler)
	{
		AddLoadCallback((o) => {lwf.SetEventHandler(eventName, eventHandler);});
	}

	public void SetProgramObjectConstructor(string programObjectName,
		ProgramObjectConstructor programObjectConstructor)
	{
		AddLoadCallback((o) => {lwf.SetProgramObjectConstructor(
			programObjectName, programObjectConstructor);});
	}

	public void SetMovieEventHandler(string instanceName,
		MovieEventHandler load = null, MovieEventHandler postLoad = null,
		MovieEventHandler unload = null, MovieEventHandler enterFrame = null,
		MovieEventHandler update = null, MovieEventHandler render = null)
	{
		AddLoadCallback((o) => {lwf.SetMovieEventHandler(
			instanceName, load, unload, enterFrame, update, render);});
	}

	public void SetButtonEventHandler(string instanceName,
		ButtonEventHandler press = null, ButtonEventHandler release = null,
		ButtonEventHandler rollOver = null, ButtonEventHandler rollOut = null,
		ButtonKeyPressHandler keyPress = null, ButtonEventHandler load = null,
		ButtonEventHandler unload = null, ButtonEventHandler enterFrame = null,
		ButtonEventHandler update = null, ButtonEventHandler render = null)
	{
		AddLoadCallback((o) => {lwf.SetButtonEventHandler(
			instanceName, press, release, rollOver, rollOut,
				keyPress, load, unload, enterFrame, update, render);});
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

	public void Rotate(float radian)
	{
		AddLoadCallback((o) => {lwf.property.Rotate(radian);});
	}

	public void RotateTo(float radian)
	{
		AddLoadCallback((o) => {lwf.property.RotateTo(radian);});
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
}
