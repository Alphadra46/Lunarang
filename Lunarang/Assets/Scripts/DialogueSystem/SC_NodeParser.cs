using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class SC_NodeParser : MonoBehaviour
{
    
    public SC_DialogueGraph graph;
    private Coroutine _parser;
    public TextMeshProUGUI speaker;
    public TextMeshProUGUI dialogue;
    public Image speakerImage;

    /// <summary>
    /// Find start node and execute it.
    /// </summary>
    private void Start()
    {
        foreach (var b in graph.nodes.Cast<Node_Base>().Where(b => b.GetString() == "Start"))
        {
            graph.current = b;
            break;
        }

        _parser = StartCoroutine(ParseNode());  
    }

    /// <summary>
    /// Decortifies and analyzes the node, sending it to the UI.
    /// </summary>
    private IEnumerator ParseNode()
    {
        var b = graph.current;
        var data = b.GetString();
        var dataParts = data.Split('/');

        switch (dataParts[0])
        {
            case "DialogueNode":
                speaker.text = dataParts[1];
                dialogue.text = dataParts[2];
                
                if(b.GetSprite() == null) speakerImage.gameObject.SetActive(false);
                else speakerImage.sprite = b.GetSprite();
                
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                NextNode("exit");
                break;
            case "Start":
                NextNode("exit");
                break;
        }
        
    }

    /// <summary>
    /// Select the next node to execute.
    /// </summary>
    /// <param name="fieldName">Name of the parameter linked</param>
    public void NextNode(string fieldName)
    {
        
        if (_parser != null)
        {
            StopCoroutine(_parser);
            _parser = null;
        }

        foreach (var p in graph.current.Ports)
        {
            if (p.fieldName != fieldName) continue;
            
            graph.current = p.Connection.node as Node_Base;
            break;
        }

        _parser = StartCoroutine(ParseNode());
    }
    
}
