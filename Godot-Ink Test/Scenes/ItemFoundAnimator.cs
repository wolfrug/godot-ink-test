using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotInk;

public partial class ItemFoundAnimator : CanvasLayer {
	[Export]
	string functionName = "ITEM_FOUND";
	[Export]
	Godot.Collections.Array<Item> Items = new Godot.Collections.Array<Item> { };
	[Export]
	InkWriter writer;

	private Dictionary<string, Item> itemsDict = new Dictionary<string, Item> { };
	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {
		writer.AddSearchableFunction (functionName);
		writer.m_textFunctionFoundEvent.onEvent += OnFunctionEvent;
		foreach (Item item in Items) {
			Item duplicate = (item as Godot.Node).Duplicate () as Item;
			AddChild(duplicate);
			itemsDict.Add (duplicate.ID, duplicate);
			duplicate.Init();
			//item.Visible = false;
		}
	}

	void OnFunctionEvent (InkDialogueLine line, InkTextVariable variable) {
		if (variable.variableName == functionName) {
			GD.Print ("Item Found Animation for variable " + variable.variableName);
			if (variable.VariableArguments.Count > 0) {
				Item tryGetItem = null;
				itemsDict.TryGetValue (variable.VariableArguments[0], out tryGetItem);
				if (tryGetItem != null) {
					tryGetItem.Init();
					DisplayItem (tryGetItem);
					GD.Print ("Displaying item " + tryGetItem.ID);
				}
			}
		}
	}

	async void DisplayItem (Item target) {
		target.Display (true);
		await Task.Delay (3000);
		target.Display (false);
	}
}