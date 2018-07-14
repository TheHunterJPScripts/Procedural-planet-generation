/**
        ColorHeight.cs
        Purpose: Contains ColorHeight class and 
        the class required to have a custom layout on the inspector.
        Require: No other files required.

        @author Mikel Jauregui
        @version 1.1.0 14/07/2018 
*/

using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class ColorHeight
{
    [Tooltip("Minimun height to set the color.")]
    public int layer;
    [Tooltip("Color of the mesh.")]
    public Color color;
}

[CustomPropertyDrawer(typeof(ColorHeight), true)]
public class ColorHeightDrawer : PropertyDrawer
{
    /// <summary>
    /// Custom GUI view of the ColorHeight class.
    /// </summary>
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