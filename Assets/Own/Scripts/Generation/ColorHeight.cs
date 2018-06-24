using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class ColorHeight
{
    public int layer;
    public Color color;
}

[CustomPropertyDrawer(typeof(ColorHeight), true)]
public class ColorHeightDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        contentPosition.width *= 0.75f;
        EditorGUI.indentLevel = 0;
        contentPosition.width /= 2f;
        EditorGUIUtility.labelWidth = 45f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("layer"), new GUIContent("Layer"));
        contentPosition.x += contentPosition.width + 10;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("color"), new GUIContent("Color"));
        EditorGUI.EndProperty();
    }
}