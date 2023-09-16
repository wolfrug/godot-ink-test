using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotInk;

public enum ArgumentRequirements {
	ANY_OF = 0000, // any of the given arguments
	EXACTLY = 0001, // exactly these arguments
	NONE_OF = 0002, // none of these arguments (but others allowed)
	ALL = 0003, // as long as it's the right variable, it is ok
}

[System.Serializable]
public class InkFunctionEvent { // these are called when an ink function is found in the -currently displayed- string
	public string targetVariable;
	public string[] targetArguments;
	public ArgumentRequirements argumentRequirement;
	public TextFunctionFoundEventHandler onEvent;

	public InkFunctionEvent (string newTargetVariable, List<string> newtargetArguments, ArgumentRequirements newArgumentRequirement, TextFunctionFoundEventHandler newFunctionFoundEvent) {
		targetVariable = newTargetVariable;
		targetArguments = newtargetArguments.ToArray ();
		argumentRequirement = newArgumentRequirement;
		onEvent = newFunctionFoundEvent;
	}
	public InkFunctionEvent (string newTargetVariable, string[] newtargetArguments, ArgumentRequirements newArgumentRequirement, TextFunctionFoundEventHandler newFunctionFoundEvent) {
		targetVariable = newTargetVariable;
		targetArguments = newtargetArguments;
		argumentRequirement = newArgumentRequirement;
		onEvent = newFunctionFoundEvent;
	}

	public bool ArgumentMatch (List<string> arguments) {
		if (argumentRequirement == ArgumentRequirements.ANY_OF) {
			foreach (string trg in targetArguments) {
				if (arguments.Contains (trg)) {
					return true;
				}
			}
			return false;
		}
		if (argumentRequirement == ArgumentRequirements.EXACTLY) {
			if (targetArguments.Length != arguments.Count) {
				return false;
			}
			foreach (string trg in targetArguments) {
				if (!arguments.Contains (trg)) {
					return false;
				}
			}
			return true;
		}
		if (argumentRequirement == ArgumentRequirements.NONE_OF) {
			foreach (string trg in targetArguments) {
				if (arguments.Contains (trg)) {
					return false;
				}
			}
			return true;
		}
		if (argumentRequirement == ArgumentRequirements.ALL) {
			return true;
		}
		return false;
	}
}

[System.Serializable]
public class InkTagEvent { // this is called when a tag is found in the currently displayed string
	public string targetTag;
	public TextTagFoundEventHandler onEvent;
}
public partial class InkListener : Node {

	[Export]
	public InkStoryData m_listenTarget;
	public List<InkFunctionEvent> m_functionEvents = new List<InkFunctionEvent> { };
	public List<InkTagEvent> m_tagEvents = new List<InkTagEvent> { };

	[Export]
	public string targetFunction;
	[Export]
	public Godot.Collections.Array<string> targetArguments = new Godot.Collections.Array<string> { };

	private Dictionary<string, List<InkFunctionEvent>> m_inkFunctionDict = new Dictionary<string, List<InkFunctionEvent>> { };

	public override void _Ready () {

		InkFunctionEvent testEvent = new InkFunctionEvent (targetFunction, targetArguments.ToList(), ArgumentRequirements.EXACTLY, null);
		m_listenTarget.AddSearchableFunction(targetFunction);
		m_functionEvents.Add(testEvent);

		foreach (InkFunctionEvent evt in m_functionEvents) { // adds the ones added in the editor initially
			AddNewFunctionEvent (evt);
		}
		if (m_listenTarget == null) {
			GD.Print ("No story object assigned to listener! Please assign a story object to it in the editor.");
		}
		GD.Print ("Adding new listeners!");
		m_listenTarget.m_textFunctionFoundEvent.onEvent += OnFunctionEvent;
		m_listenTarget.m_inkTagFoundEvent.onEvent += OnTagEvent;
	}

	public virtual void FunctionEvent(InkDialogueLine dialogueLine, InkTextVariable variable){

	}
	public virtual void TagEvent(InkDialogueLine dialogueLine, string tag){

	}

	public virtual void AddNewFunctionEvent (InkFunctionEvent evt) {
		if (!m_functionEvents.Contains (evt)) {
			m_functionEvents.Add (evt);
		}
		if (m_inkFunctionDict.ContainsKey (evt.targetVariable)) {
			m_inkFunctionDict[evt.targetVariable].Add (evt);
		} else {
			m_inkFunctionDict.Add (evt.targetVariable, new List<InkFunctionEvent> { evt });
		}
	}

	public virtual void OnFunctionEvent (InkDialogueLine dialogueLine, InkTextVariable variable) {
		GD.Print ("Received function event: " + variable.variableName + "(" + string.Join ("\n", variable.VariableArguments) + ")");
		List<InkFunctionEvent> functionEvents = new List<InkFunctionEvent> { };
		m_inkFunctionDict.TryGetValue (variable.variableName, out functionEvents);
		if (functionEvents != null) {
			if (functionEvents.Count > 0) {
				foreach (InkFunctionEvent evt in functionEvents) {
					if (evt.ArgumentMatch (variable.VariableArguments)) {
						GD.Print ("Invoking function event " + variable.variableName + "(" + string.Join ("\n", variable.VariableArguments) + ")");
						evt.onEvent.Invoke (dialogueLine, variable);
						FunctionEvent(dialogueLine, variable);
					}
				}
			}
		}
	}
	public virtual void OnTagEvent (InkDialogueLine dialogueLine, string tag) {
		GD.Print ("Received tag event for tag " + tag);
		foreach (InkTagEvent evt in m_tagEvents) {
			if (evt.targetTag == tag) {
				GD.Print ("Invoking tag event " + tag);
				evt.onEvent.Invoke (dialogueLine, tag);
				TagEvent(dialogueLine, tag);
			}
		}
	}

}