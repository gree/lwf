using UnityEngine;
using UnityEditor;

using Style = LWF.UnityRenderer.ISystemFontRenderer.Style;
using Align = LWF.UnityRenderer.ISystemFontRenderer.Align;
using VerticalAlign = LWF.UnityRenderer.ISystemFontRenderer.VerticalAlign;

[CustomEditor(typeof(UISystemFontLabel))]
public class UISystemFontLabelInspector : UIWidgetInspector
{
	protected UISystemFontLabel mLabel;

	void LookLikeControls(float labelWidth)
	{
		EditorGUIUtility.LookLikeControls(labelWidth);
	}

	void RegisterUndo()
	{
		NGUIEditorTools.RegisterUndo("Label Change", mLabel);
	}

	protected override bool OnDrawProperties()
	{
		mLabel = (UISystemFontLabel)target;

		GUI.skin.textArea.wordWrap = true;
		string text = string.IsNullOrEmpty(mLabel.text) ? "" : mLabel.text;
		text = EditorGUILayout.TextArea(
			text, GUI.skin.textArea, GUILayout.Height(100f));
		if (!text.Equals(mLabel.text)) {RegisterUndo(); mLabel.text = text;}

		GUILayout.BeginHorizontal();
		{
			LookLikeControls(55f);
			float size = EditorGUILayout.FloatField(
				"Font Size", mLabel.size, GUILayout.MinWidth(50f));
			if (size != mLabel.size) {RegisterUndo(); mLabel.size = size;}

			LookLikeControls(30f);
			Style style = (Style)EditorGUILayout.EnumPopup(
				"Style", mLabel.style, GUILayout.MinWidth(50f));
			if (style != mLabel.style)
				{RegisterUndo(); mLabel.style = style;}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			LookLikeControls(40f);
			float width = EditorGUILayout.FloatField(
				"Width", mLabel.width, GUILayout.MinWidth(50f));
			if (width != mLabel.width) {RegisterUndo(); mLabel.width = width;}

			float height = EditorGUILayout.FloatField(
				"Height", mLabel.height, GUILayout.MinWidth(50f));
			if (height != mLabel.height)
				{RegisterUndo(); mLabel.height = height;}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			LookLikeControls(35f);
			Align align = (Align)EditorGUILayout.EnumPopup(
				"Align", mLabel.align, GUILayout.MinWidth(50f));
			if (align != mLabel.align)
				{RegisterUndo(); mLabel.align = align;}

			LookLikeControls(75f);
			VerticalAlign verticalAlign =
				(VerticalAlign)EditorGUILayout.EnumPopup(
				"VerticalAlign", mLabel.verticalAlign, GUILayout.MinWidth(50f));
			if (verticalAlign != mLabel.verticalAlign)
				{RegisterUndo(); mLabel.verticalAlign = verticalAlign;}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			LookLikeControls(80f);
			float lineSpacing = EditorGUILayout.FloatField(
				"Line Spacing", mLabel.lineSpacing, GUILayout.Width(120f));
			if (lineSpacing != mLabel.lineSpacing)
				{RegisterUndo(); mLabel.lineSpacing = lineSpacing;}

			float letterSpacing = EditorGUILayout.FloatField(
				"Letter Spacing", mLabel.letterSpacing, GUILayout.Width(120f));
			if (letterSpacing != mLabel.letterSpacing)
				{RegisterUndo(); mLabel.letterSpacing = letterSpacing;}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			float leftMargin = EditorGUILayout.FloatField(
				"Left Margin", mLabel.leftMargin, GUILayout.Width(120f));
			if (leftMargin != mLabel.leftMargin)
				{RegisterUndo(); mLabel.leftMargin = leftMargin;}

			float rightMargin = EditorGUILayout.FloatField(
				"Right Margin", mLabel.rightMargin, GUILayout.Width(120f));
			if (rightMargin != mLabel.rightMargin)
				{RegisterUndo(); mLabel.rightMargin = rightMargin;}
		}
		GUILayout.EndHorizontal();

		return true;
	}

	[MenuItem("NGUI/Create a System Font Label")]
	static public void AddLabel()
	{
		GameObject root = NGUIMenu.SelectedRoot();

		if (NGUIEditorTools.WillLosePrefab(root))
		{
			NGUIEditorTools.RegisterUndo("Add a System Font Label", root);

			GameObject obj = new GameObject("UISystemFontLabel");
			obj.layer = root.layer;
			obj.transform.parent = root.transform;

			UISystemFontLabel label = obj.AddComponent<UISystemFontLabel>();
			label.MakePixelPerfect();
			label.transform.localPosition = new Vector3(0, 0, -1);
			label.transform.localScale = new Vector3(1, 1, 1);
			label.text = "System Font Label";
			label.size = 16;
			label.lineSpacing = 1;
			label.width = 500;
			label.height = 100;

			Selection.activeGameObject = obj;
		}
	}
}
