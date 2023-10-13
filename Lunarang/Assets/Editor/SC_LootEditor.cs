using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Loot<>))]
public class SC_LootEditor : PropertyDrawer
{
    const int FIELD_SPACING = 8;
    const int LINE_SPACING = 8;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var overallWidth = position.width;
        var initialX = position.x;

        EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        position.height = (position.height - LINE_SPACING) / 2;

        position = DrawLabel(position, overallWidth, "Probability", 0.25f);
        position = DrawLabel(position, overallWidth, "Drop", 0.55f);

        position.y += GetPropertyHeight(property, label) / 2;
        position.x = initialX;

        position = DrawPropertyField(position, overallWidth, property.FindPropertyRelative("Probability"), 0.25f);
        position = DrawPropertyField(position, overallWidth, property.FindPropertyRelative("Drop"), 0.55f);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label) * 2 + LINE_SPACING;
    }

    protected Rect DrawPropertyField(Rect position, float overallWidth, SerializedProperty property, float widthPercentage) {
        float width = widthPercentage * overallWidth;
        position.width = width - FIELD_SPACING;
        EditorGUI.PropertyField(position, property, GUIContent.none);

        position.x += width;
        return position;
    }

    protected Rect DrawLabel(Rect position, float overallWidth, string label, float widthPercentage) {
        float width = widthPercentage * overallWidth;
        position.width = width - FIELD_SPACING;
        EditorGUI.LabelField(position, label);

        position.x += width;
        return position;
    }
}
