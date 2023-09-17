using Godot;
using System;

public partial class AnimationPlayer : Godot.AnimationPlayer
{
	[Export]
	string functionName = "PLAY_ANIMATION";
	//[Export]
	//Godot.Collections.Array<string> audioClips = new Godot.Collections.Array<string> { };
	[Export]
	InkWriter writer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		writer.AddSearchableFunction (functionName);
		writer.m_textFunctionFoundEvent.onEvent += OnFunctionEvent;
	}

	void OnFunctionEvent (InkDialogueLine line, InkTextVariable variable) {
		//GD.Print ("Got an event! For variable " + variable.variableName);
		if (variable.variableName == functionName) {
			if (variable.VariableArguments.Count > 0) {
					Play(variable.VariableArguments[0]);
			}
		}
	}
}
