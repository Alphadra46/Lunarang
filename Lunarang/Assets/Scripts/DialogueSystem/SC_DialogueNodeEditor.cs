using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(Node_Base))]
public class SC_DialogueNodeEditor : NodeEditor
{

    private Node_Base baseNode;
    private static GUIStyle editorLabelStyle;

    public override void OnBodyGUI()
    {
        
        if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        EditorStyles.label.normal.textColor = Color.white;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;

    }
}
