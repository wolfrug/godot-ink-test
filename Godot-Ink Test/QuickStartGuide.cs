using Godot;
using GodotInk;

public partial class QuickStartGuide : VBoxContainer
{
    [Export]
    private InkStory story;
    [Export]
    private RichTextLabel exampleLabel;
     [Export]
    private Button exampleButton;

    public override void _Ready()
    {
        ContinueStory();
    }

    private void ContinueStory()
    {
        foreach (Node child in GetChildren())
            child.QueueFree();

        RichTextLabel content = (exampleLabel as Godot.Node).Duplicate() as RichTextLabel;
        content.Text = story.ContinueMaximally();
        content.Visible = true;
        AddChild(content);

        foreach (InkChoice choice in story.CurrentChoices)
        {
            Button button = (exampleButton as Godot.Node).Duplicate() as Button;
            button.Text = choice.Text;
            button.Visible = true;
            button.Pressed += delegate
            {
                story.ChooseChoiceIndex(choice.Index);
                ContinueStory();
            };
            AddChild(button);
        }
    }
}