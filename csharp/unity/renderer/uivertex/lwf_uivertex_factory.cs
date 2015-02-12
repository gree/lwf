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
using System.Collections.Generic;

using ResourceCache = LWF.UnityRenderer.ResourceCache;
using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;

namespace LWF {
namespace UIVertexRenderer {

public class UIVertexBuffer
{
	public UIVertex[] vertices;
	public int[] objects;
	public int index;
	public bool modified;
	public bool initialized;

	public void Alloc(int n)
	{
		vertices = new UIVertex[n * 4];
		objects = new int[n];
		index = 0;
		modified = true;
		initialized = true;
	}
}

public interface IMeshRenderer
{
	void UpdateMesh(UIVertexBuffer buffer);
}

public class UIVertexComponent : MonoBehaviour
{
	public int updateCount;
	public UnityEngine.CanvasRenderer canvasRenderer;
	public MaterialPropertyBlock property;
	public UIVertexBuffer buffer;
	public UnityEngine.Color additionalColor;
	public List<IMeshRenderer> renderers;
	public int rendererCount;
	public int rectangleCount;
	private int additionalColorId;

	public void Init(Factory factory)
	{
		renderers = new List<IMeshRenderer>();

		canvasRenderer = gameObject.AddComponent<UnityEngine.CanvasRenderer>();
		UpdateSortingLayerAndOrder(factory);
		UpdateLayer(factory);

		if (factory.useAdditionalColor) {
			additionalColor = UnityEngine.Color.clear;
			property = new MaterialPropertyBlock();
			additionalColorId = Shader.PropertyToID("_AdditionalColor");
		}

		buffer = new UIVertexBuffer();
	}

	public void UpdateSortingLayerAndOrder(Factory factory)
	{
		//canvasRenderer.sortingLayerName = factory.sortingLayerName;
		//canvasRenderer.sortingOrder = factory.sortingOrder;
	}

	public void UpdateLayer(Factory factory)
	{
		gameObject.layer = factory.gameObject.layer;
	}

	public void AddRenderer(IMeshRenderer renderer, int rc, int uc)
	{
		if (updateCount != uc) {
			updateCount = uc;
			rendererCount = 0;
			rectangleCount = 0;
		}

		int i = rendererCount++;
		if (i < renderers.Count)
			renderers[i] = renderer;
		else
			renderers.Add(renderer);

		rectangleCount += rc;
	}

	public void SetMaterial(Material material, UnityEngine.Color ac)
	{
		if (canvasRenderer.GetMaterial() != material)
			canvasRenderer.SetMaterial(material, null);
		additionalColor = ac;
		buffer.modified = true;
	}

	public void Disable()
	{
		updateCount = 0;
		rendererCount = 0;
		rectangleCount = 0;
		canvasRenderer.SetMaterial(null, null);
		gameObject.SetActive(false);
	}

	public void UpdateMesh()
	{
		gameObject.SetActive(true);

		if (buffer.objects == null || buffer.objects.Length != rectangleCount) {
			buffer.Alloc(rectangleCount);
		} else {
			buffer.index = 0;
		}

		for (int i = 0; i < rendererCount; ++i)
			renderers[i].UpdateMesh(buffer);

		canvasRenderer.SetVertices(buffer.vertices, rectangleCount * 4);

		if (buffer.modified) {
			buffer.modified = false;
		}

		if (property != null) {
			property.AddColor(additionalColorId, additionalColor);
			//meshRenderer.SetPropertyBlock(property);
		}
	}

	void OnDestroy()
	{
		canvasRenderer.SetMaterial(null, null);
		UnityEngine.CanvasRenderer.Destroy(canvasRenderer);
	}
}

public partial class Factory : UnityRenderer.Factory
{
	public int updateCount;
	private int meshComponentNo;
	private int usedMeshComponentNo;
	private List<UIVertexComponent> meshComponents;
	private UIVertexComponent currentMeshComponent;
	private Factory parent;

	public Factory(Data d, GameObject gObj,
			float zOff = 0, float zR = 1, int rQOff = 0,
			string sLayerName = null, int sOrder = 0, bool uAC = false,
			Camera renderCam = null, Camera inputCam = null,
			string texturePrfx = "", string fontPrfx = "",
			TextureLoader textureLdr = null,
			TextureUnloader textureUnldr = null,
      string shaderName = "LWF",
			bool attaching = false)
		: base(d, gObj, zOff, zR, rQOff, sLayerName, sOrder, uAC, renderCam,
			inputCam, texturePrfx, fontPrfx, textureLdr, textureUnldr, shaderName)
	{
		CreateBitmapContexts();
		CreateTextContexts();

		meshComponents = new List<UIVertexComponent>();
		if (!attaching)
			AddMeshComponent();
		usedMeshComponentNo = -1;

		updateCount = -1;
	}

	public override void Destruct()
	{
		foreach (UIVertexComponent meshComponent in meshComponents)
			GameObject.Destroy(meshComponent.gameObject);

		DestructBitmapContexts();
		DestructTextContexts();

		base.Destruct();
	}

	private UIVertexComponent AddMeshComponent()
	{
		GameObject gobj = new GameObject(
			"LWF/" + data.name + "/Mesh/" + meshComponents.Count);
		gobj.SetActive(false);
		gobj.transform.parent = gameObject.transform;
		gobj.transform.localPosition = Vector3.zero;
		gobj.transform.localScale = Vector3.one;
		gobj.transform.localRotation = Quaternion.identity;
		UIVertexComponent meshComponent =
			gobj.AddComponent<UIVertexComponent>();
		meshComponent.Init(this);
		meshComponents.Add(meshComponent);
		return meshComponent;
	}

	public override void BeginRender(LWF lwf)
	{
		base.BeginRender(lwf);

		parent = null;
		var lwfParent = lwf.GetParent();
		if (lwfParent != null)
			parent = lwfParent.rendererFactory as Factory;
		if (parent != null)
			return;

		updateCount = lwf.updateCount;
		meshComponentNo = -1;
		currentMeshComponent = null;
	}

	public void Render(IMeshRenderer renderer, int rectangleCount,
		Material material, UnityEngine.Color additionalColor)
	{
		if (parent != null) {
			parent.Render(renderer, rectangleCount, material, additionalColor);
			return;
		}

		if (currentMeshComponent == null) {
			meshComponentNo = 0;
			currentMeshComponent = meshComponents[meshComponentNo];
			currentMeshComponent.SetMaterial(material, additionalColor);
		} else {
			Material componentMaterial =
				currentMeshComponent.canvasRenderer.GetMaterial();
			if (componentMaterial != material ||
					(currentMeshComponent.property != null &&
					currentMeshComponent.additionalColor != additionalColor)) {
				int no = ++meshComponentNo;
				if (no >= meshComponents.Count)
					AddMeshComponent();
				currentMeshComponent = meshComponents[no];
				currentMeshComponent.SetMaterial(material, additionalColor);
			}
		}

		currentMeshComponent.AddRenderer(renderer, rectangleCount, updateCount);
	}

	public override void EndRender(LWF lwf)
	{
		base.EndRender(lwf);

		if (parent != null)
			return;

		if (currentMeshComponent == null) {
			for (int i = 0; i <= usedMeshComponentNo; ++i)
				meshComponents[i].Disable();
			usedMeshComponentNo = -1;
			return;
		}

		for (int i = 0; i <= meshComponentNo; ++i)
			meshComponents[i].UpdateMesh();

		for (int i = meshComponentNo + 1; i <= usedMeshComponentNo; ++i)
			meshComponents[i].Disable();
		usedMeshComponentNo = meshComponentNo;
	}

	public override void UpdateSortingLayerAndOrder()
	{
		foreach (UIVertexComponent meshComponent in meshComponents)
			meshComponent.UpdateSortingLayerAndOrder(this);
	}

	public override void UpdateLayer()
	{
		foreach (UIVertexComponent meshComponent in meshComponents)
			meshComponent.UpdateLayer(this);
	}

	public override Renderer ConstructBitmap(
		LWF lwf, int objectId, Bitmap bitmap)
	{
		return new BitmapRenderer(lwf, m_bitmapContexts[objectId]);
	}

	public override Renderer ConstructBitmapEx(
		LWF lwf, int objectId, BitmapEx bitmapEx)
	{
		return new BitmapRenderer(lwf, m_bitmapExContexts[objectId]);
	}

	public override TextRenderer ConstructText(LWF lwf, int objectId, Text text)
	{
		return new TextMeshRenderer(lwf, m_textContexts[objectId]);
	}
}

}	// namespace UIVertexRenderer
}	// namespace LWF
