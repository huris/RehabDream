using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[Flags]
public enum EditorListOption {
	None = 0,
	ListSize = 1,
	ListLabel = 2,
	Default = ListSize | ListLabel
}

public class WMG_E_Util : Editor {
	// Function to ovveride and display custom object array in inspector
	public void ArrayGUI(string label, string name, EditorListOption options = EditorListOption.Default) {
		bool showListLabel = (options & EditorListOption.ListLabel) != 0;
		bool showListSize = (options & EditorListOption.ListSize) != 0;
		SerializedProperty list = serializedObject.FindProperty(name);
		
		if (showListLabel) {
			EditorGUILayout.PropertyField(list, new GUIContent(label));
			EditorGUI.indentLevel += 1;
		}
		if (!showListLabel || list.isExpanded) {
			if (showListSize) {
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
			}
			for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
			}
		}
		if (showListLabel) {
			EditorGUI.indentLevel -= 1;
		}
	}
}
