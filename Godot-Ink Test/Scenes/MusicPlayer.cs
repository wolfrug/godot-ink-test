using Godot;
using GodotInk;
using System;

public partial class MusicPlayer : AudioStreamPlayer
{
	
	[Export]
	string functionName = "PLAY_MUSIC";
	[Export]
	Godot.Collections.Array<AudioStream> audioClips = new Godot.Collections.Array<AudioStream> { }; 
	[Export]
	InkWriter writer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		writer.AddSearchableFunction(functionName);
		writer.m_textFunctionFoundEvent.onEvent += OnFunctionEvent;
	}

	void OnFunctionEvent(InkDialogueLine line, InkTextVariable variable){
		GD.Print("Got an event! For variable " + variable.variableName);
		if (variable.variableName == functionName){
			if (variable.VariableArguments.Count > 0){
				int index = int.Parse(variable.VariableArguments[0]);
				if (index>=0 && index < audioClips.Count){
					Stream = audioClips[index];
					Play(0);
				}
			}
		}
	}
}
