using System;
using Godot;
using GodotInk;

public partial class TextUpdater : RichTextLabel
{
	[Export]
	string functionName = "TEXT_AMOUNT";
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
				Text = variable.VariableArguments[0];
			}
		}
	}
}
