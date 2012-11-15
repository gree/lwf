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
using System.Collections;
using System.Collections.Generic;

using ResourceCache = LWF.UnityRenderer.ResourceCache;
using LWFDataLoader = System.Func<string, byte[]>;
using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;
using LWFDataCallback = System.Func<LWF.Data, bool>;
using LWFCallback = System.Action<LWFPlayer>;
using LWFCallbacks = System.Collections.Generic.List<System.Action<LWFPlayer>>;

public class LWFPlayer : MonoBehaviour
{
	public LWF.LWF lwf;
	public LWF.CombinedMeshRenderer.Factory factory;
	public string lwfName;
	public Mesh[] meshes;
	public int frameNo;
	public bool isAlive;
	protected WaitForSeconds waitForSeconds;
	protected LWFCallbacks lwfLoadCallbacks;
	protected LWFCallbacks lwfDestroyCallbacks;
	protected int activateCount = 1;
	protected int resumeCount = 0;

	public LWFPlayer()
	{
		isAlive = true;
		lwfLoadCallbacks = new LWFCallbacks();
		lwfDestroyCallbacks = new LWFCallbacks();
	}

	public virtual void OnDestroy()
	{
		isAlive = false;

		if (lwfName == null)
			return;

		lwfDestroyCallbacks.ForEach(c => c(this));
		lwfDestroyCallbacks = null;

		if (lwf != null) {
			lwf.Destroy();
			lwf = null;
		}

		if (factory != null) {
			factory.Destruct();
			factory = null;
		}

		if (meshes != null) {
			meshes = null;
			ResourceCache.SharedInstance().DeleteRenderedMesh(lwfName);
		}

		ResourceCache.SharedInstance().UnloadLWFData(lwfName);
	}

	public virtual bool Load(string path,
		string texturePrefix = "", string fontPrefix = "",
		float zOffset = 0, float zRate = 1, int renderQueueOffset = 0,
		int cachingFrames = 0, Camera camera = null, bool autoPlay = true,
		LWFDataCallback lwfDataCallback = null,
		LWFCallback lwfLoadCallback = null,
		LWFCallback lwfDestroyCallback = null,
		LWFDataLoader lwfDataLoader = null,
		TextureLoader textureLoader = null,
		TextureUnloader textureUnloader = null)
	{
		lwfName = path;
		if (camera == null)
			camera = Camera.main;

		if (lwfLoadCallback != null)
			lwfLoadCallbacks.Add(lwfLoadCallback);
		if (lwfDestroyCallback != null)
			lwfDestroyCallbacks.Add(lwfDestroyCallback);

		LWF.Data data =
			ResourceCache.SharedInstance().LoadLWFData(lwfName, lwfDataLoader);
		if (data == null || !data.Check())
			return false;

		if (lwfDataCallback != null && !lwfDataCallback(data))
			return false;

		factory = new LWF.CombinedMeshRenderer.Factory(
			data, gameObject, zOffset, zRate, renderQueueOffset, camera,
			texturePrefix, fontPrefix, textureLoader, textureUnloader);

		lwf = new LWF.LWF(data, factory);

		if (cachingFrames == 0) {
			foreach (LWF.Format.Movie m in data.movies) {
				if (cachingFrames < m.frames)
					cachingFrames = m.frames;
			}
			if (cachingFrames == 0)
				cachingFrames = 1;
		}

		meshes = ResourceCache.SharedInstance().AddRenderedMesh(
			lwfName, cachingFrames);
		frameNo = 0;

		OnLoad();

		if (autoPlay)
			Play();

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
		lwfLoadCallbacks.ForEach(c => c(this));
		lwfLoadCallbacks = null;
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

	public virtual void Play()
	{
		++resumeCount;
		if (resumeCount == 1) {
			waitForSeconds = new WaitForSeconds(lwf.tick);
			StartCoroutine("UpdateLWF");
		}
	}

	public virtual void Stop()
	{
		--resumeCount;
		if (resumeCount == 0)
			StopCoroutine("UpdateLWF");
	}

	public virtual void Activate()
	{
		++activateCount;
		if (activateCount == 1)
#if UNITY_3_5
			gameObject.active = true;
#else
			gameObject.SetActive(true);
#endif
	}

	public virtual void Deactivate()
	{
		--activateCount;
		if (activateCount == 0)
#if UNITY_3_5
			gameObject.active = false;
#else
			gameObject.SetActive(false);
#endif
	}

	protected virtual IEnumerator UpdateLWF()
	{
		for (;;) {
			if (lwf != null) {
				Mesh mesh = meshes[frameNo];
				if (mesh == null) {
					Mesh orgMesh = factory.mesh;
					mesh = new Mesh();
					meshes[frameNo] = mesh;
					factory.mesh = mesh;
					factory.buffer.objects = null;
					lwf.ForceExec();
					lwf.Render();
					factory.mesh = orgMesh;
				}
				factory.meshFilter.sharedMesh = mesh;

				if (++frameNo >= meshes.Length)
					frameNo = 0;
			}

			yield return waitForSeconds;
		}
	}
}
