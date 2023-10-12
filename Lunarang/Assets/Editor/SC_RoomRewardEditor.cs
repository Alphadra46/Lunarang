using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SC_RoomRewards))]
public class SC_RoomRewardEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {

        EditorGUILayout.Space(32);
        EditorGUILayout.LabelField("Room Reward Generator",GetTitleStyle());
        EditorGUILayout.Separator();

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        DrawSimulateRewards();
        DrawReset();
        DrawRewardChoices();
        
        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
    }

    void DrawSimulateRewards()
    {
        EditorGUILayout.Separator();
        
        SC_RoomRewards roomRewards = (SC_RoomRewards)target;
        if (GUILayout.Button("Simulate Rewards"))
        {
            roomRewards.SimulateReward();
        }
    }

    void DrawReset()
    {
        EditorGUILayout.Separator();
        
        SC_RoomRewards roomRewards = (SC_RoomRewards)target;
        if (GUILayout.Button("Reset All Loot Tables"))
        {
            roomRewards.ResetAllLootTables();
        }
    }
    
    void DrawRewardChoices()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.Space(32);
        
        SC_RoomRewards roomRewards = (SC_RoomRewards)target;
        if (GUILayout.Button("Skill 1"))
        {
            roomRewards.ChooseReward(0);
        }
        
        EditorGUILayout.Space(8);
        if (GUILayout.Button("Skill 2"))
        {
            roomRewards.ChooseReward(1);
        }
        
        EditorGUILayout.Space(8);
        if (GUILayout.Button("Skill 3"))
        {
            roomRewards.ChooseReward(2);
        }
    }
    
    GUIStyle GetTitleStyle() {
        var style = new GUIStyle(EditorStyles.label);
        style.fontSize = 20;
        style.alignment = TextAnchor.MiddleCenter;

        return style;
    }
}




