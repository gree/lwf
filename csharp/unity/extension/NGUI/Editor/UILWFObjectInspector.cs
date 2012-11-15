using UnityEngine;
using UnityEditor;

using ScaleType = UILWFObject.ScaleType;

[CustomEditor(typeof(UILWFObject))]
public class UILWFObjectInspector : UIWidgetInspector
{
	protected UILWFObject mLWFObject;

	void LookLikeControls(float labelWidth)
	{
		EditorGUIUtility.LookLikeControls(labelWidth);
	}

	void RegisterUndo()
	{
		NGUIEditorTools.RegisterUndo("UILWFObject Change", mLWFObject);
	}

	protected override bool OnDrawProperties()
	{
		mLWFObject = (UILWFObject)target;

		LookLikeControls(130f);
		string path =
			string.IsNullOrEmpty(mLWFObject.path) ? "" : mLWFObject.path;
		path = EditorGUILayout.TextField("LWF Path in Resources", path);
		if (!path.Equals(mLWFObject.path))
			{RegisterUndo(); mLWFObject.path = path;}

		GUILayout.BeginHorizontal();
		{
			LookLikeControls(50f);
			float zOffset = EditorGUILayout.FloatField(
				"Z Offset", mLWFObject.zOffset, GUILayout.MinWidth(50f));
			if (zOffset != mLWFObject.zOffset)
				{RegisterUndo(); mLWFObject.zOffset = zOffset;}

			float zRate = EditorGUILayout.FloatField(
				"Z Rate", mLWFObject.zRate, GUILayout.MinWidth(50f));
			if (zRate != mLWFObject.zRate)
				{RegisterUndo(); mLWFObject.zRate = zRate;}

		}
		GUILayout.EndHorizontal();

		LookLikeControls(130f);
		int renderQueueOffset = EditorGUILayout.IntField(
			"RendererQueue Offset", mLWFObject.renderQueueOffset);
		if (renderQueueOffset != mLWFObject.renderQueueOffset)
			{RegisterUndo(); mLWFObject.renderQueueOffset = renderQueueOffset;}

		LookLikeControls(60f);
		ScaleType scaleType = (ScaleType)EditorGUILayout.EnumPopup(
			"Scale Type", mLWFObject.scaleType);
		if (scaleType != mLWFObject.scaleType)
			{RegisterUndo(); mLWFObject.scaleType = scaleType;}

		return true;
	}

	[MenuItem("NGUI/Create a LWFObject")]
	static public void AddLWFObject()
	{
		GameObject root = NGUIMenu.SelectedRoot();

		if (NGUIEditorTools.WillLosePrefab(root))
		{
			NGUIEditorTools.RegisterUndo("Add a LWFObject", root);

			GameObject obj = new GameObject("UILWFObject");
			obj.layer = root.layer;
			Transform transform = obj.transform;
			transform.parent = root.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;

			UILWFObject lwfObject = obj.AddComponent<UILWFObject>();
			lwfObject.zRate = 1;

			Selection.activeGameObject = obj;
		}
	}
}
