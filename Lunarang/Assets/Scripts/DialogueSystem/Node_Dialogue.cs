using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Dialogue")]
[NodeTint("#08c764")]
[NodeWidth(300)]
public class Node_Dialogue : Node_Base
{

	[Input] public int entry;
	[Output] public int exit;
	public string speakerName;
	[Multiline] public string dialogueLine;
	public Sprite sprite;

	/// <summary>
	/// Get all informations of the Node.
	/// </summary>
	/// <returns></returns>
	public override string GetString()
	{
		return "DialogueNode/"+ speakerName + "/" + dialogueLine;
	}
	
	/// <summary>
	/// Get current sprite.
	/// </summary>
	/// <returns></returns>
	public override Sprite GetSprite()
	{
		return sprite;
	}
	
}