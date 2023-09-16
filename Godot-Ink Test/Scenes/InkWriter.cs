using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotInk;

public partial class InkWriter : InkStoryData {
	[Export]
	protected string startKnot = "start";

	[Export] protected bool clearText = true;

	[Export]
	protected VBoxContainer containerText;
	[Export]
	protected VBoxContainer containerButtons;
	[Export]
	protected RichTextLabel textLabel;
	[Export]
	protected Button optionButton;

	protected bool coroutineRunning = false;
	protected string m_storyFlow = "default";

	protected ScrollContainer scrollContainer;

	public override void _Ready () {
		base._Ready ();
		scrollContainer = (containerText.GetParent() as ScrollContainer);
		StartStory ();
	}

	public virtual void StartStory () {
		ClearAllOptions ();
		ClearAllText ();
		PlayKnot (startKnot);
	}

	/*private void ContinueStory () {
		foreach (Node child in containerText.GetChildren ()) {
			child.QueueFree ();
		}
		foreach (Node child in containerButtons.GetChildren ()) {
			child.QueueFree ();
		}

		RichTextLabel content = (textLabel as Godot.Node).Duplicate () as RichTextLabel;
		content.Text = story.ContinueMaximally ();
		content.Visible = true;
		containerText.AddChild (content);

		foreach (InkChoice choice in story.CurrentChoices) {
			Button button = (optionButton as Godot.Node).Duplicate () as Button;
			button.Text = choice.Text;
			button.Visible = true;
			button.Pressed += delegate {
				story.ChooseChoiceIndex (choice.Index);
				ContinueStory ();
			};
			containerButtons.AddChild (button);
		}
	}*/

	protected virtual void SpawnTextObject (string text) {
		RichTextLabel content = (textLabel as Godot.Node).Duplicate () as RichTextLabel;
		content.Text = text;
		content.Visible = true;
		containerText.AddChild (content);
	}
	protected virtual Button SpawnOptionObject (string text, InkChoiceLine choice) {
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

	public virtual void ClearAllText () {
		foreach (Node child in containerText.GetChildren ()) {
			child.QueueFree ();
		}
	}
	public virtual void ClearAllOptions () {
		foreach (Node child in containerButtons.GetChildren ()) {
			child.QueueFree ();
		}
	}

	public virtual void PlayKnot (string knotName) { // play directly from a knot
		List<InkChoiceLine> gatherChoices = new List<InkChoiceLine> { };
		InkDialogueLine[] dialogueLines = CreateStringArrayKnot (knotName, gatherChoices, m_storyFlow);
		if (!coroutineRunning) {
			//StopCoroutine (m_displayCoroutine);
		} else {
			//m_writerStartedEvent.Invoke (this);
		}
		DisplayText (dialogueLines, gatherChoices);
	}
	public virtual void PlayChoice (InkChoice choice) { // play from a choice - mainly used internally
		List<InkChoiceLine> gatherChoices = new List<InkChoiceLine> { };
		InkDialogueLine[] dialogueLines = CreateStringArrayChoice (choice, gatherChoices, m_storyFlow);
		if (!coroutineRunning) {
			//StopCoroutine (m_displayCoroutine);
		} else {
			//m_writerStartedEvent.Invoke (this); // We invoke the started event when starting a whole new thing
		}
		DisplayText (dialogueLines, gatherChoices);
	}

	public virtual void PlayDialogueLines (InkDialogueLine[] targetLines) { // just provide the lines directly
		if (!coroutineRunning) {
			//StopCoroutine (m_displayCoroutine);
		} else {
			//m_writerStartedEvent.Invoke (this); // We invoke the started event when starting a whole new thing
		}
		DisplayText (targetLines, null);
	}

	// Where most of the magic happens: takes the line of dialogue + possible expected choices, displays them one by one by spawning text objects
	// And then displayes the options by spawning buttons
	public virtual async void DisplayText (InkDialogueLine[] dialogueLines, List<InkChoiceLine> gatherChoices) {
		for (int i = 0; i < dialogueLines.Length; i++) {
			InkDialogueLine currentLine = dialogueLines[i];
			InvokeDialogueEvents (currentLine);
			if (currentLine.displayText.Trim () != "") { // Do not spawn a text object if there is no displayable text to display
				SpawnTextObject (currentLine.displayText);
			}
			//m_dialogueShownEvent.Invoke (currentLine);
		}
		// Scroll down
		await Task.Delay(10);
		scrollContainer.ScrollVertical = (int)scrollContainer.GetVScrollBar().MaxValue;
		if (gatherChoices != null) {
			if (gatherChoices.Count > 0) {
				List < (InkChoiceLine, Button) > allButtons = new List < (InkChoiceLine, Button) > { };
				foreach (InkChoiceLine choice in gatherChoices) {
					SpawnOptionObject (choice.choiceText.displayText, choice);
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
	/*protected virtual void PressOptionButton (InkChoiceLine optionButton, Button selectedButton, List < (InkChoiceLine, Button) > allButtons) {
		// We only press one
		m_optionPressed = true;
		m_waitingOnOptionPress = false;
		InvokeDialogueEvents (optionButton.choiceText);
		PlayChoice (optionButton.choice);
		foreach ((InkChoiceLine, Button) set in allButtons) {
			set.Item2.gameObject.SetActive (false);
		}
	}*/

	protected virtual void InvokeDialogueEvents (InkDialogueLine currentLine) {
		if (currentLine.inkVariables.Count > 0) {
			foreach (InkTextVariable variable in currentLine.inkVariables) {
				//m_textFunctionFoundEvent.Invoke (currentLine, variable);
				InvokeFunctionEvent (currentLine, variable);
				GD.Print ("Invoked ink function: " + variable.variableName + "(" + string.Join ("\n", variable.VariableArguments) + ")");
			}
		}
		if (currentLine.inkTags.Count > 0) {
			foreach (string tag in currentLine.inkTags) {
				InvokeTagEvent (currentLine, tag);
				//m_inkTagFoundEvent.Invoke (currentLine, tag);
			}
		}
	}
}