using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotInk;
public partial class CustomInkButtonController : InkWriter {
	[Export]
	Godot.Collections.Array<Button> taggedButtons = new Godot.Collections.Array<Button> { };

	protected Dictionary<string, Button> buttonDictionary = new Dictionary<string, Button> { };
	private List<Button> allButtons = new List<Button>{};

	public override void _Ready () {
		base._Ready ();
		AddSearchableFunction("BUTTONTAG");
		foreach (Button btn in taggedButtons) {
			buttonDictionary.Add (btn.Text, btn);
			btn.Visible = false;
			GD.Print("Added button " + btn + " with tag " + btn.Text + " to dictionary!");
		}
	}

	protected override Button SpawnOptionObject (string text, InkChoiceLine choice) {

		Button outButton = null;
		if (choice.choiceText.HasVariable("BUTTONTAG")) {
			buttonDictionary.TryGetValue (choice.choiceText.GetVariable("BUTTONTAG").VariableArguments[0], out outButton);
			if (outButton != null) {
				GD.Print("Found button " + outButton + " with tag " + choice.choiceText.GetVariable("BUTTONTAG").VariableArguments[0]);
				Button foundButton = (outButton as Godot.Node).Duplicate () as Button;
				foundButton.Text = text;
				foundButton.Visible = true;
				foundButton.Pressed += delegate {
					InvokeDialogueEvents (choice.choiceText);
					ClearAllOptions ();
					if (clearText) {
						ClearAllText ();
					};
					PlayChoice (choice.choice);
					GD.Print("Clicked special tagged button " + outButton);
				};
				outButton.GetParent().AddChild(foundButton);
				return foundButton;
			}
		}

		Button button = (optionButton as Godot.Node).Duplicate () as Button;
		button.Text = text;
		button.Visible = true;
		button.Pressed += delegate {
			InvokeDialogueEvents (choice.choiceText);
			ClearAllOptions ();
			if (clearText) {
				ClearAllText ();
			};
			PlayChoice (choice.choice);
		};
		containerButtons.AddChild (button);
		return button;
	}
	public override void ClearAllOptions () {
		//base.ClearAllOptions();
		foreach (Button btn in allButtons){
			btn.QueueFree();
		}
		allButtons.Clear();
	}
	public override async void DisplayText (InkDialogueLine[] dialogueLines, List<InkChoiceLine> gatherChoices) {
		for (int i = 0; i < dialogueLines.Length; i++) {
			InkDialogueLine currentLine = dialogueLines[i];
			InvokeDialogueEvents (currentLine);
			if (currentLine.displayText.Trim () != "") { // Do not spawn a text object if there is no displayable text to display
				SpawnTextObject (currentLine.displayText);
			}
			//m_dialogueShownEvent.Invoke (currentLine);
		}
		// Scroll down
		await Task.Delay (10);
		scrollContainer.ScrollVertical = (int) scrollContainer.GetVScrollBar ().MaxValue;
		if (gatherChoices != null) {
			if (gatherChoices.Count > 0) {
				foreach (InkChoiceLine choice in gatherChoices) {
					Button spawnedButton = SpawnOptionObject (choice.choiceText.displayText, choice);
					allButtons.Add(spawnedButton);
				}
			} else { // we only invoke the writer finished event if there really are no more choices
				//m_writerFinishedEvent.Invoke (this);
				coroutineRunning = false;
			}
		} else { // we only invoke the writer finished event if there really are no more choices
			//m_writerFinishedEvent.Invoke (this);
			coroutineRunning = false;
		};
	}
}