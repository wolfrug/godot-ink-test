using System;
using Godot;

public partial class Item : Node {
	[Export] string itemId;
	[Export] RichTextLabel itemTextLabel;
	[Export] string itemText = "Found Black Mirror!";
	[Export] TextureRect itemIconRect;
	[Export] Texture2D itemIcon;

	//private Tween tween;
	public virtual void Init () {
		itemTextLabel.Text = "[center]" + itemText + "[/center]";
		itemIconRect.Texture = itemIcon;
		GD.Print("Initializing item " + itemId + " with text " + itemText);
		//tween = itemIconRect.CreateTween ();
	}
	public string ID {
		get {
			return itemId;
		}
	}
	public void Display (bool display) {
		if (display) {
			Tween tween = itemIconRect.CreateTween ();
			tween.TweenProperty (itemIconRect, "modulate:a", 1f, 1.0f).SetTrans (Tween.TransitionType.Sine);
		} else {
			Tween tween = itemIconRect.CreateTween ();
			tween.TweenProperty (itemIconRect, "modulate:a", 0f, 1.0f).SetTrans (Tween.TransitionType.Sine);
		}
	}

}