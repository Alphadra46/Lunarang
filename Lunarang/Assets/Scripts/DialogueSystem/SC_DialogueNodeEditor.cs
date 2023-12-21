using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(BaseNode))]
public class SC_DialogueNodeEditor : NodeEditor
{

    private BaseNode baseNod;
    private static GUIStyle editorLabelStyle;

    public override void OnBodyGUI()
    {
        
        if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        EditorStyles.label.normal.textColor = Color.white;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;

    }
}
