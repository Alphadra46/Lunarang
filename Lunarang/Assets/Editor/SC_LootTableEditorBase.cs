using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SC_LootTableEditorBase<T> : UnityEditor.Editor where T :Object
{
    public override void OnInspectorGUI()
    {
        SC_LootTable<T> lootTable = (SC_LootTable<T>)target;
        
        EditorGUILayout.Space(32);
        EditorGUILayout.LabelField(lootTable.name, GetTitleStyle());
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Loot Table", GetSecondaryTitleStyle());
        EditorGUILayout.Space(32);
        
        EditorGUI.BeginChangeCheck();
        
        base.OnInspectorGUI();
        DrawDropChances();
        DrawSimulateSection();

        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
    }

    void DrawDropChances()
    {
        EditorGUILayout.Space(64);
        
        EditorGUILayout.LabelField("Drop Chances", GetTitleStyle());

        SC_LootTable<T> lootTable = (SC_LootTable<T>)target;
        
        lootTable.lootTable.ForEach(dropConfig =>
        {
            var chance = lootTable.GetChanceFor(dropConfig);
            DrawChanceEntry(chance, dropConfig.ToString() + " - " + Mathf.Floor(chance * 1000)/10+"%");
        });
    }

    void DrawSimulateSection()
    {
        EditorGUILayout.Separator();

        SC_LootTable<T> lootTable = (SC_LootTable<T>)target;
        if (GUILayout.Button("Simulate Drop"))
        {
            lootTable.SimulateDrop();
        }
    }
    
    GUIStyle GetTitleStyle() {
        var style = new GUIStyle(EditorStyles.label);
        style.fontSize = 20;
        style.alignment = TextAnchor.MiddleCenter;

        return style;
    }
    
    GUIStyle GetSecondaryTitleStyle() {
        var style = new GUIStyle(EditorStyles.label);
        style.fontSize = 16;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Italic;

        return style;
    }
    
    void DrawChanceEntry(float value, string text) {
        Rect r = EditorGUILayout.BeginVertical();
        EditorGUI.ProgressBar(r, value, text);
        GUILayout.Space(18);
        EditorGUILayout.EndVertical();
        GUILayout.Space(2);
    }
}
