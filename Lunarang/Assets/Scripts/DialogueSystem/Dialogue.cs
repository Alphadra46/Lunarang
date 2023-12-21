using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Dialogue")]
[NodeTint("#08c764")]
[NodeWidth(300)]
public class Dialogue : BaseNode
{
	[Input(backingValue = ShowBackingValue.Never)] public BaseNode input;
	[Output(backingValue = ShowBackingValue.Never)] public BaseNode output;
	
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