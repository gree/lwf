using UnityEngine;
using UnityEditor;

using Align = BitmapFont.Renderer.Align;
using VerticalAlign = BitmapFont.Renderer.VerticalAlign;

[CustomEditor(typeof(UIBitmapFontLabel))]
public class UIBitmapFontLabelInspector : UIWidgetInspector
{
	protected UIBitmapFontLabel mLabel;

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
		mLabel = (UIBitmapFontLabel)target;

		GUI.skin.textArea.wordWrap = true;
		string text = string.IsNullOrEmpty(mLabel.text) ? "" : mLabel.text;
		text = EditorGUILayout.TextArea(
			text, GUI.skin.textArea, GUILayout.Height(100f));
		if (!text.Equals(mLabel.text)) {RegisterUndo(); mLabel.text = text;}

		GUILayout.BeginHorizontal();
		{
			LookLikeControls(55f);
			string fontName =
				string.IsNullOrEmpty(mLabel.fontName) ? "" : mLabel.fontName;
			fontName = EditorGUILayout.TextField(
				"Font Name", fontName, GUILayout.MinWidth(50f));
			if (!fontName.Equals(mLabel.fontName))
				{RegisterUndo(); mLabel.fontName = fontName;}

			LookLikeControls(55f);
			float size = EditorGUILayout.FloatField(
				"Font Size", mLabel.size, GUILayout.MinWidth(50f));
			if (size != mLabel.size) {RegisterUndo(); mLabel.size = size;}
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
			float spaceAdvance = EditorGUILayout.FloatField(
				"Space Advance", mLabel.spaceAdvance, GUILayout.Width(120f));
			if (spaceAdvance != mLabel.spaceAdvance)
				{RegisterUndo(); mLabel.spaceAdvance = spaceAdvance;}

			float tabSpacing = EditorGUILayout.FloatField(
				"Tab Spacing", mLabel.tabSpacing, GUILayout.Width(120f));
			if (tabSpacing != mLabel.tabSpacing)
				{RegisterUndo(); mLabel.tabSpacing = tabSpacing;}
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

	[MenuItem("NGUI/Create a Bitmap Font Label")]
	static public void AddLabel()
	{
		GameObject root = NGUIMenu.SelectedRoot();

		if (NGUIEditorTools.WillLosePrefab(root))
		{
			NGUIEditorTools.RegisterUndo("Add a Bitmap Font Label", root);

			GameObject obj = new GameObject("UIBitmapFontLabel");
			obj.layer = root.layer;
			obj.transform.parent = root.transform;

			UIBitmapFontLabel label = obj.AddComponent<UIBitmapFontLabel>();
			label.MakePixelPerfect();
			label.transform.localPosition = new Vector3(0, 0, -1);
			label.transform.localScale = new Vector3(1, 1, 1);
			label.fontName = "Font/font";
			label.text = "Bitmap Font Label";
			label.size = 16;
			label.spaceAdvance = 0.25f;
			label.lineSpacing = 1;
			label.tabSpacing = 4f;
			label.width = 500;
			label.height = 100;

			Selection.activeGameObject = obj;
		}
	}
}
