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

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using Factory = LWF.UnityRenderer.Factory;
using Type = LWF.Format.Object.Type;
using GType = LWF.Format.GraphicObject.Type;

public class LWFObjectInspector : EditorWindow
{
	public class ObjectContainer
	{
		public LWFObject lwfObject;
		public LWF.Object obj;
		public Dictionary<int, ObjectContainer> objects;
		public int hierarchy;
		public int depth;
		public int renderingIndex;
		public int renderingCount;
		public int renderingOffset;

		public ObjectContainer(
			LWFObject lo, LWF.Object o, int h, int d, int ri, int rc, int ro)
		{
			lwfObject = lo;
			obj = o;
			hierarchy = h;
			depth = d;
			renderingIndex = ri;
			renderingCount = rc;
			renderingOffset = ro;
			objects = new Dictionary<int, ObjectContainer>();
		}
	}

	public static Color buttonColor = new Color(1, 0, 0, 0.5f);
	public static Color focusButtonColor = new Color(0, 1, 0, 0.5f);
	LWFObjectButtonInspector buttonInspector;
	Dictionary<object, bool> visibilities;
	Dictionary<LWF.Object, ObjectContainer> objects;
	Vector2 scrollPos;

	[MenuItem("Window/LWFObject Inspector %#l")]
	static void Init()
	{
		EditorWindow window = GetWindow(typeof(LWFObjectInspector));
		window.Show();
	}

	static void SetButtonColor(Color c, Color fc)
	{
		buttonColor = c;
		focusButtonColor = fc;
	}

	public LWFObjectInspector()
	{
		visibilities = new Dictionary<object, bool>();
		objects = new Dictionary<LWF.Object, ObjectContainer>();
	}

	void DrawInfo(ObjectContainer container, LWF.Object obj)
	{
		LWF.Matrix m = obj.matrix;
		EditorGUILayout.LabelField("Matrix", string.Format(
			"(sx:{0}, sy:{1}, k0:{2}, k1:{3}, tx:{4}, ty:{5}) ri:{6} rc:{7}",
			m.scaleX, m.scaleY, m.skew0, m.skew1, m.translateX, m.translateY,
			container.renderingIndex, container.renderingCount));

		LWF.ColorTransform c = obj.colorTransform;
		EditorGUILayout.LabelField("ColorTransform",
			string.Format("multi:(r:{0},g:{1},b:{2},a:{3}) " +
				"add:(r:{4},g:{5},b:{6},a:{7})",
			c.multi.red, c.multi.green, c.multi.blue, c.multi.alpha,
			c.add.red, c.add.green, c.add.blue, c.add.alpha));
	}

	void DrawInspector(ObjectContainer container)
	{
		EditorGUI.indentLevel = container.hierarchy + 1;

		LWF.Object obj = container.obj;
		LWF.LWF lwf = obj.lwf;
		if (obj.type == Type.MOVIE) {
			LWF.Movie movie = (LWF.Movie)obj;

			string movieName = "Movie: " +
				(movie.name == null ? "(null)" : movie.name);
			if (!visibilities.ContainsKey(movie))
				visibilities[movie] = true;
			visibilities[movie] =
				EditorGUILayout.Foldout(visibilities[movie], movieName);

			if (!visibilities[movie])
				return;

			EditorGUI.indentLevel = container.hierarchy + 2;
			string fullName = movie.GetFullName();
			if (fullName == null)
				fullName = "(null)";
			EditorGUILayout.LabelField("Fullname:", fullName);
			EditorGUILayout.LabelField("Visible:", movie.visible.ToString());
			EditorGUILayout.LabelField("Playing:", movie.playing.ToString());
			EditorGUILayout.LabelField("Frame:", movie.currentFrame.ToString());
			DrawInfo(container, movie);

			// TODO
			EditorGUILayout.Space();

			foreach (KeyValuePair<int, ObjectContainer>
					kvp in container.objects) {
				DrawInspector(kvp.Value);
			}
		} else {
			EditorGUILayout.LabelField("Depth:", container.depth.ToString());
			EditorGUI.indentLevel = container.hierarchy + 2;

			switch (obj.type) {
			case Type.BUTTON:
				LWF.Button button = (LWF.Button)obj;
				string buttonName =
					(button.name == null ? "(null)" : button.name);
				string buttonFullName = button.GetFullName();
				if (buttonFullName == null)
					buttonFullName = "(null)";
				EditorGUILayout.LabelField("Button:", buttonName);

				EditorGUI.indentLevel = container.hierarchy + 3;
				EditorGUILayout.LabelField("Fullname:", buttonFullName);
				DrawInfo(container, obj);
				// TODO
				break;

			case Type.GRAPHIC:
				EditorGUILayout.LabelField("Graphic:", "");
				EditorGUI.indentLevel = container.hierarchy + 3;
				DrawInfo(container, obj);
				// TODO
				break;

			case Type.BITMAP:
				LWF.Bitmap bitmap = (LWF.Bitmap)obj;
				int tFId = lwf.data.bitmaps[bitmap.objectId].textureFragmentId;
				string textureName = (tFId == -1 ? "(null)" :
					lwf.data.textureFragments[tFId].filename);
				EditorGUILayout.LabelField("Bitmap:", textureName);
				EditorGUI.indentLevel = container.hierarchy + 3;
				DrawInfo(container, obj);
				// TODO
				break;

			case Type.BITMAPEX:
				LWF.BitmapEx bitmapEx = (LWF.BitmapEx)obj;
				int tFIdEx =
					lwf.data.bitmapExs[bitmapEx.objectId].textureFragmentId;
				string textureNameEx = (tFIdEx == -1 ? "(null)" :
					lwf.data.textureFragments[tFIdEx].filename);
				EditorGUILayout.LabelField("BitmapEx:", textureNameEx);
				EditorGUI.indentLevel = container.hierarchy + 3;
				DrawInfo(container, obj);
				// TODO
				break;

			case Type.TEXT:
				LWF.Text text = (LWF.Text)obj;
				int nameStringId = lwf.data.texts[text.objectId].nameStringId;
				string textName = nameStringId == -1 ?
					"" : lwf.data.strings[nameStringId];
				EditorGUILayout.LabelField("Text:", textName);
				EditorGUI.indentLevel = container.hierarchy + 3;
				DrawInfo(container, obj);
				// TODO
				break;

			case Type.PARTICLE:
				EditorGUILayout.LabelField("Particle:", "");
				EditorGUI.indentLevel = container.hierarchy + 3;
				DrawInfo(container, obj);
				// TODO
				break;

			case Type.PROGRAMOBJECT:
				LWF.ProgramObject pObject = (LWF.ProgramObject)obj;
				string pObjectName = lwf.data.strings[
					lwf.data.programObjects[pObject.objectId].stringId];
				EditorGUILayout.LabelField("ProgramObject:", pObjectName);
				EditorGUI.indentLevel = container.hierarchy + 3;
				DrawInfo(container, obj);
				// TODO
				break;
			}
		}
	}

	void OnDestroy()
	{
		if (buttonInspector != null) {
			GameObject.DestroyImmediate(buttonInspector.gameObject);
			buttonInspector = null;
		}
	}

	void OnInspectorUpdate()
	{
		Repaint();
	}

	void OnGUI()
	{
		LWFObject[] lwfObjects =
			FindObjectsOfType(typeof(LWFObject)) as LWFObject[];
		if (lwfObjects == null || lwfObjects.Length == 0) {
			if (buttonInspector != null)
				GameObject.DestroyImmediate(buttonInspector.gameObject);
			return;
		}

		if (buttonInspector == null) {
			GameObject obj = new GameObject("LWFObjectButtonInspector");
			obj.hideFlags = HideFlags.HideAndDontSave;
			buttonInspector = obj.AddComponent<LWFObjectButtonInspector>();
			buttonInspector.SetVisibilities(visibilities);
		}

		EditorGUILayout.BeginVertical();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		foreach (LWFObject lwfObject in lwfObjects) {
			EditorGUI.indentLevel = 0;
			LWF.LWF lwf = lwfObject.lwf;
			if (lwf == null)
				continue;

			string name = lwf.name;
			if (lwf.parent != null) {
				string parentName = lwf.parent.name;
				if (parentName == null)
					parentName = "(null)";
				string parentFullname = lwf.parent.GetFullName();
				if (parentFullname == null)
					parentFullname = "(null)";
				name += string.Format(" / attached:{0} on:{1} / {2}",
					lwf.attachName, parentName, parentFullname);
			}

			if (!visibilities.ContainsKey(lwfObject))
				visibilities[lwfObject] = true;
			visibilities[lwfObject] =
				EditorGUILayout.Foldout(visibilities[lwfObject], name);
			if (!visibilities[lwfObject])
				continue;

			lwfObject.lwf.Inspect((obj, hierarchy, depth, rOffset) => {
				ObjectContainer container =
					new ObjectContainer(lwfObject, obj, hierarchy, depth,
						obj.lwf.renderingIndex, obj.lwf.renderingCount,
						rOffset);
				objects[obj] = container;

				if (obj.parent != null)
					objects[obj.parent].objects[depth] = container;
			});

			DrawInspector(objects[lwfObject.lwf.rootMovie]);
		}

		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
}

public class LWFObjectButtonInspector : MonoBehaviour
{
	static Texture2D texture;
	Dictionary<object, bool> visibilities;
	Matrix4x4 matrix;
	Matrix4x4 renderMatrix;

	LWFObjectButtonInspector()
	{
		matrix = new Matrix4x4();
		renderMatrix = new Matrix4x4();
	}

	public void SetVisibilities(Dictionary<object, bool> v)
	{
		visibilities = v;
	}

	void DrawButton(LWF.Button button, Factory factory)
	{
		Matrix4x4 savedMatrix = GUI.matrix;
		Color savedColor = GUI.color;

		factory.ConvertMatrix(ref matrix, button.matrix, 0, button.height);
		Factory.MultiplyMatrix(ref renderMatrix,
			factory.gameObject.transform.localToWorldMatrix, matrix);
		Camera camera = factory.inputCamera;

		Matrix4x4 m = renderMatrix;
		Vector2 lt = GUIUtility.ScreenToGUIPoint(camera.WorldToScreenPoint(
			m.MultiplyPoint(new Vector3(0, button.height))));
		Vector2 rt = GUIUtility.ScreenToGUIPoint(camera.WorldToScreenPoint(
			m.MultiplyPoint(new Vector3(button.width, button.height))));
		Vector2 ld = GUIUtility.ScreenToGUIPoint(camera.WorldToScreenPoint(
			m.MultiplyPoint(new Vector3(0, 0))));

		float dx = rt.x - lt.x;
		float dy = rt.y - lt.y;
		float w = Mathf.Sqrt(dx * dx + dy * dy);
		dx = ld.x - lt.x;
		dy = ld.y - lt.y;
		float h = Mathf.Sqrt(dx * dx + dy * dy);
		float angle =
			Mathf.Atan2(rt.x - lt.x, rt.y - lt.y) * Mathf.Rad2Deg - 90;
		lt.y = Screen.height - lt.y;
		GUIUtility.RotateAroundPivot(angle, lt);

		GUI.color = button == button.lwf.focus ?
			LWFObjectInspector.focusButtonColor :
				LWFObjectInspector.buttonColor;
		GUI.DrawTexture(new Rect(lt.x, lt.y, w, h), texture);

		GUI.matrix = savedMatrix;
		GUI.color = savedColor;
	}

	void OnGUI()
	{
		LWFObject[] lwfObjects =
			FindObjectsOfType(typeof(LWFObject)) as LWFObject[];
		if (lwfObjects == null)
			return;

		if (!texture) {
			texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, Color.white);
			texture.Apply();
		}

		Matrix4x4 savedMatrix = GUI.matrix;
		Color savedColor = GUI.color;

		foreach (LWFObject lwfObject in lwfObjects) {
			bool visibility;
			if (visibilities.TryGetValue(
					lwfObject, out visibility) && !visibility)
				continue;

			LWF.LWF lwf = lwfObject.lwf;
			if (lwf == null)
				continue;

			lwf.Inspect((obj, hierarchy, depth, rOffset) => {
				if (obj.type != Type.BUTTON)
					return;

				for (LWF.Object o = obj.parent; o != null; o = o.parent) {
					LWF.Movie m = o as LWF.Movie;
					if (m != null && !m.visible)
						return;
					if (visibilities.TryGetValue(
							o, out visibility) && !visibility)
						return;
				}

				Factory factory = lwf.rendererFactory as Factory;
				if (factory == null)
					return;

				LWF.Button button = (LWF.Button)obj;
				DrawButton(button, factory);
			});
		}

		GUI.matrix = savedMatrix;
		GUI.color = savedColor;
	}
}
