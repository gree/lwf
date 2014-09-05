/*
 * Copyright (C) 2014 GREE, Inc.
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
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
 
[CanEditMultipleObjects()]
[CustomEditor(typeof(LWFObject), true)]
public class LWFObjectEditor : Editor
{
	private LWFObject mLWFObject;
	private int mSortingOrder;
	private string[] mSortingLayerNames;
	private int mPopupMenuIndex;

	void OnEnable()
	{
		mLWFObject = target as LWFObject;
		if (mLWFObject == null)
			return;

		var sortingLayerName = mLWFObject.sortingLayerName;
		if (string.IsNullOrEmpty(sortingLayerName))
			sortingLayerName = "Default";
		mSortingOrder = mLWFObject.sortingOrder;
		mSortingLayerNames = GetSortingLayerNames();
		for (int i = 0; i < mSortingLayerNames.Length; ++i) {
			if (string.Compare(mSortingLayerNames[i], sortingLayerName) == 0) {
				mPopupMenuIndex = i;
				break;
			}
		}
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (mLWFObject == null)
			return;
 
		serializedObject.Update();
		mPopupMenuIndex = EditorGUILayout.Popup(
			"Sorting Layer", mPopupMenuIndex, mSortingLayerNames);
		mSortingOrder = EditorGUILayout.IntField(
			"Order in Layer", mSortingOrder);
		var sortingLayerName = mSortingLayerNames[mPopupMenuIndex];
		if (string.Compare(name, "Default") == 0)
			sortingLayerName = null;
		mLWFObject.sortingLayerName = sortingLayerName;
		mLWFObject.sortingOrder = mSortingOrder;
		serializedObject.ApplyModifiedProperties();
	}
 
	public string[] GetSortingLayerNames()
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty =
			internalEditorUtilityType.GetProperty("sortingLayerNames",
				BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
}
