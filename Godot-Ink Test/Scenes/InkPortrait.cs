using System;
using Godot;
using GodotInk;

public partial class InkPortrait : TextureRect {
	[Export]
	string functionName = "PORTRAIT";
	[Export]
	Godot.Collections.Array<Texture2D> textures = new Godot.Collections.Array<Texture2D> { };
	[Export]
	InkWriter writer;
	[Export] Godot.AnimationPlayer player;

	private bool fadedOut = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		writer.AddSearchableFunction (functionName);
		writer.m_textFunctionFoundEvent.onEvent += OnFunctionEvent;
	}

	void OnFunctionEvent (InkDialogueLine line, InkTextVariable variable) {
		//GD.Print ("Got an event! For variable " + variable.variableName);
		if (variable.variableName == functionName) {
			if (variable.VariableArguments.Count > 0) {
				int index = int.Parse (variable.VariableArguments[0]);
				if (index >= 0 && index < textures.Count) {
					if (Texture != textures[index] || fadedOut) {
						Texture = textures[index];
						player.Play ("fade_in");
						fadedOut = false;
					}
				} else {
					player.Play ("fade_out");
					fadedOut = true;
				}
			} else {
				player.Play ("fade_out");
				fadedOut = true;
			}
		}
	}
}