using System;
using Godot;
using GodotInk;

public partial class EnergySlider : HSlider {
	[Export]
	string functionName = "SLIDER_ENERGY";
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
				int amount = int.Parse (variable.VariableArguments[0]);
				Value = amount;
			}
		}
	}
}