using Godot;
using System;

public partial class QuitFunction : InkListener
{
	public override void FunctionEvent(InkDialogueLine dialogueLine, InkTextVariable variable){
		GetTree().Quit();
	}
}
