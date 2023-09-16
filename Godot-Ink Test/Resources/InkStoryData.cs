using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Godot;
using GodotInk;

[System.Serializable]
public class InkTextVariable {
    // This represents a text variable found (and removed) from a piece of ink text
    public string variableName; // the name of the variable, e.g. PLAYER
    private List<string> variableArguments = new List<string> { }; // any potential variable arguments, e.g. PLAYER(sad, left)
    public List<string> VariableArguments {
        get {
            return variableArguments;
        }
        set {
            variableArguments.Clear ();
            variableArguments.AddRange (value);
        }
    }
    public bool HasArgument (string argument) {
        return variableArguments.Contains (argument);
    }
    public InkTextVariable (string newVariableName = "", List<string> newVariableArguments = null) {
        variableName = newVariableName;
        if (newVariableArguments != null) {
            VariableArguments = newVariableArguments;
        }
    }
}

[System.Serializable]
public class InkDialogueLine {
    // This is a piece of data that contains all the information needed from a piece of ink dialogue - arrays of these are used to display longer dialogues
    public string text_unmodified; // the unmodified text
    public string displayText; // displayable text, with string variables removed
    public List<string> inkTags = new List<string> { }; // ink tags found in text
    public List<InkTextVariable> inkVariables = new List<InkTextVariable> { }; // ink variables found in text

    // Just some helpful functions
    public bool HasArgument (string argument) {
        foreach (InkTextVariable inkVariable in inkVariables) {
            if (inkVariable.HasArgument (argument)) {
                return true;
            }
        }
        return false;
    }
    public bool HasVariable (string variable) {
        foreach (InkTextVariable inkVariable in inkVariables) {
            if (inkVariable.variableName == variable) {
                return true;
            }
        }
        return false;
    }

    public bool HasVariableWithArgument (string variable, string argument) {
        if (!HasVariable (variable)) {
            return false;
        }
        foreach (InkTextVariable inkVariable in inkVariables) {
            if (inkVariable.variableName == variable) {
                if (inkVariable.HasArgument (argument)) {
                    return true;
                }
            }
        }
        return false;
    }
    public InkTextVariable GetVariable (string variableName) {
        if (!HasVariable (variableName)) {
            return null;
        }
        return inkVariables.Find ((x) => x.variableName == variableName);
    }
    public InkTextVariable GetVariableWithArgument (string variable, string argument) {
        foreach (InkTextVariable inkVariable in inkVariables) {
            if (inkVariable.variableName == variable) {
                if (inkVariable.HasArgument (argument)) {
                    return inkVariable;
                }
            }
        }
        return null;
    }
}

[System.Serializable]
public class InkChoiceLine {
    public InkChoice choice; // the choice itself
    public InkDialogueLine choiceText; // the ink dialogue line with commands, tags etc
}

[Signal]
public delegate void TextTagFoundEventHandler (InkDialogueLine line, string tag);

[Signal]
public delegate void TextFunctionFoundEventHandler (InkDialogueLine line, InkTextVariable variable);

public class RaiseTextFunctionEvent {
    public event TextFunctionFoundEventHandler onEvent;

    public virtual void Raise (InkDialogueLine line, InkTextVariable variable) {
        onEvent?.Invoke (line, variable);
    }
}
public class RaiseTextTagEvent {
    public event TextTagFoundEventHandler onEvent;

    public virtual void Raise (InkDialogueLine line, string tag) {
        onEvent?.Invoke (line, tag);
    }
}

public partial class InkStoryData : Node {

    public string m_ID = "default";
    [Export]
    public InkStory m_inkStory = null;

    public List<InkTextVariable> m_searchableTextVariables = new List<InkTextVariable> { }; // which text variables we are searching for & parsing
    [Export]
    public Godot.Collections.Array<string> searchableTextVariables = new Godot.Collections.Array<string> { }; // which text variables we are searching for & parsing
    public RaiseTextFunctionEvent m_textFunctionFoundEvent = new RaiseTextFunctionEvent ();
    public RaiseTextTagEvent m_inkTagFoundEvent = new RaiseTextTagEvent ();

    public InkStory InkStory {
        get {

            if (m_inkStory != null) {
                return m_inkStory;
            } else {
                //InitStory ();
                return m_inkStory;
            }
        }
        set {
            m_inkStory = value;
        }
    }

    /* public void SaveStory () {
         string json = InkStory.state.ToJson ();
         PlayerPrefs.SetString ("InkWriter_ExampleSave", json);
     }
     */

    public override void _Ready () { // inits a story by either loading it or starting a new one
        foreach (string variable in searchableTextVariables) {
            //GD.Print ("Attempting to add new searchable function: " + variable);
            AddSearchableFunction (variable);
        }
        /* if (m_inkStory == null) {
              InkStory = new Story (m_inkJsonAsset.text);
          };
           
          if (SavedStory) { // We load if we can
              string savedJson = PlayerPrefs.GetString ("InkWriter_ExampleSave");
              InkStory.state.LoadJson (savedJson);
              Debug.Log ("Loaded story state");
          } else { // If we can't, we start from beginning
              Debug.Log ("No saved story found - starting new story");
          }
          
          if (m_defaultTextVariables != null) {
              foreach (InkTextVariable el in m_defaultTextVariables.m_variables) {
                  AddSearchableFunction (el);
              }
          }
          */
    }

    /*  public void ClearSavedStory () {
            PlayerPrefs.DeleteKey ("InkWriter_ExampleSave");
        }

        public bool SavedStory {
            get {
                return PlayerPrefs.HasKey ("InkWriter_ExampleSave");
            }
        }
        public bool IsLoaded () {
            return InkStory != null;
        }
*/
    public void AddSearchableFunction (string newVariable) {
        if (m_searchableTextVariables.FindAll ((x) => x.variableName == newVariable).Count == 0) { // Only add if it hasn't been added already
            GD.Print ("Adding new searchable function: " + newVariable);
            m_searchableTextVariables.Add (new InkTextVariable (newVariable, null));
        };
    }

    // Creates a string array of all the strings in a specific knot, and optionally the choices at the end
    // Note: create the List<Choice> when calling this and the list will be automagically modified (we don't return a new list)
    public InkDialogueLine[] CreateStringArrayKnot (string targetKnot, List<InkChoiceLine> gatherChoices, string optionalFlow = "default") {
        if (optionalFlow != "default") {
            InkStory.SwitchFlow (optionalFlow);
        }
        InkStory.ChoosePathString (targetKnot);
        InkDialogueLine[] returnArray = CreateDialogueArray (optionalFlow);
        if (gatherChoices != null) {
            foreach (InkChoice choice in InkStory.CurrentChoices) {
                gatherChoices.Add (ParseInkChoice (choice));
                //Debug.Log ("Added end choice with name: " + choice.text);
            }
        }
        if (optionalFlow != "default") {
            InkStory.RemoveFlow (optionalFlow);
        }
        return returnArray;
    }

    // Creates a list of strings starting from a choice, and then optionally gathers the choices
    public InkDialogueLine[] CreateStringArrayChoice (InkChoice startChoice, List<InkChoiceLine> gatherChoices, string optionalFlow = "default") {
        if (optionalFlow != "default") {
            InkStory.SwitchFlow (optionalFlow);
        }
        if (InkStory.CurrentChoices.Contains (startChoice)) {
            InkStory.ChooseChoiceIndex (startChoice.Index);
        } else {
            //GD.Print ("Tried to choose a choice that is no longer among the Inkwriter's available choices!");
            InkStory.ChoosePath (startChoice.TargetPath);
        }
        InkDialogueLine[] returnArray = CreateDialogueArray (optionalFlow);
        if (gatherChoices != null) {
            foreach (InkChoice choice in InkStory.CurrentChoices) {
                gatherChoices.Add (ParseInkChoice (choice));
                //Debug.Log ("Added end choice with name: " + choice.text);
            }
        }
        if (optionalFlow != "default") {
            InkStory.RemoveFlow (optionalFlow);
        }
        return returnArray;
    }

    // where the magic happens - actually goes through the ink file and "continues" things
    InkDialogueLine[] CreateDialogueArray (string targetFlow = "default") {
        string returnText = "";
        List<InkDialogueLine> returnArray = new List<InkDialogueLine> { };
        while (InkStory.CanContinue) {
            returnText = InkStory.Continue ();
            InkDialogueLine newLine = ParseInkText (returnText);
            if (InkStory.CurrentTags.Count > 0) {
                newLine.inkTags.AddRange (new List<string> (InkStory.CurrentTags));
            };
            returnArray.Add (newLine);
        }

        return returnArray.ToArray ();
    }

    // this creates an actual ink dialogue line by parsing out any functions, gathering tags and creating a 'displayable' text line
    public InkDialogueLine ParseInkText (string inkContinueText) {
        // Parses ink text for any defined text tags and creates a proper dialogue line object
        InkDialogueLine returnLine = new InkDialogueLine ();

        // Creates the list of text tags for the regex
        string regexList = "";
        foreach (InkTextVariable textPiece in m_searchableTextVariables) {
            regexList += textPiece.variableName + "|";
        }
        if (m_searchableTextVariables.Count > 0) {
            regexList = regexList.Remove (regexList.Length  -  1,  1); // remove last |
        }
        string input = inkContinueText;
        string pattern = @"\b(?:(?:" + regexList + @")\((?:[^()]|(?<open>\()|(?<-open>\)))*(?(open)(?!))\))";
        string cleanedText = input;

        // Creates a list where the first entry is e.g. PLAYER, and subsequent entries are variables
        List<List<string>> substrings = new List<List<string>> ();
        foreach (Match match in Regex.Matches (input, pattern)) {
            // Clean the text from the match
            cleanedText = cleanedText.Replace (match.Value, "");

            string innerPattern = @"^([^(]*)\(([^)]*)\)$";
            Match innerMatch = Regex.Match (match.Value, innerPattern);
            // Group 0 = original match
            // Group 1 = variable name
            string newVariableName = innerMatch.Groups[1].Value;
            //Debug.Log ("Variable name: " + newVariable.variableName);
            // Group 2 = arguments
            List<string> newVariableArguments = new List<string> { };
            if (innerMatch.Groups.Count > 2) {
                foreach (string splitString in innerMatch.Groups[2].Value.Split (',')) {
                    newVariableArguments.Add (splitString.Trim ());
                    //Debug.Log ("Argument: " + newVariable.VariableArguments[newVariable.VariableArguments.Count - 1]);
                }
            }
            InkTextVariable newVariable = new InkTextVariable (newVariableName, newVariableArguments);
            GD.Print ("Found ink variable: " + newVariableName + "(" + string.Join ("\n", newVariableArguments) + ")");
            returnLine.inkVariables.Add (newVariable);
        }
        returnLine.text_unmodified = inkContinueText;
        returnLine.displayText = cleanedText;
        //Debug.Log ("Cleaned text: " + cleanedText);

        return returnLine;
    }

    public InkChoiceLine ParseInkChoice (InkChoice newChoice) {
        InkChoiceLine returnVal = new InkChoiceLine ();
        returnVal.choice = newChoice;
        returnVal.choiceText = ParseInkText (newChoice.Text);
        return returnVal;
    }

    public void InvokeFunctionEvent (InkDialogueLine currentLine, InkTextVariable variable) {
        GD.Print ("Invoked global ink function event: " + variable.variableName + "(" + string.Join ("\n", variable.VariableArguments) + ")");
        m_textFunctionFoundEvent.Raise (currentLine, variable);
    }
    public void InvokeTagEvent (InkDialogueLine currentLine, string tag) {
        GD.Print ("Invoked global tag event: " + tag);
        m_inkTagFoundEvent.Raise (currentLine, tag);
    }
}