using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SC_PlayerStats))]
public class SC_PlayerStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SC_PlayerStats playerStats = (SC_PlayerStats)target;
        
        EditorGUILayout.Space(32);
        EditorGUILayout.LabelField("Player Stats",GetTitleStyle());
        EditorGUILayout.Separator();
        
        EditorGUI.BeginChangeCheck();
        
        base.OnInspectorGUI();
        DrawPlayerStats(playerStats.currentHealth,"Health");
        
        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
    }

    void DrawPlayerStats(float value, string text)
    {
        Rect r = EditorGUILayout.BeginVertical();
        EditorGUI.ProgressBar(r,value,text);
        GUILayout.Space(18);
        EditorGUILayout.EndVertical();
        GUILayout.Space(2);
    }
    
    GUIStyle GetTitleStyle() {
        var style = new GUIStyle(EditorStyles.label);
        style.fontSize = 20;
        style.alignment = TextAnchor.MiddleCenter;

        return style;
    }
    
    
}
