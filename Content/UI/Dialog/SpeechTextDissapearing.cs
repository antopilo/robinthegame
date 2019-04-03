using Godot;
using System;

public class SpeechTextDissapearing : Control
{
    public string text = "";
    private Label Label;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        

        var tween = (Tween)GetNode("Tween");
        Label = (Label)GetNode("Label");
        Label.Text = text;
        Label.RectPosition -= new Vector2(Label.RectSize.x / 2, 0);

        // Fadeout
        tween.InterpolateProperty(this, "rect_position", RectPosition, RectPosition + new Vector2(0, -25),
            5, Tween.TransitionType.Expo, Tween.EaseType.Out);
        tween.InterpolateProperty(this, "modulate", new Color(1,1,1,1), new Color(1,1,1,0),
            text.Length - (text.Length / 1.25f), Tween.TransitionType.Linear, Tween.EaseType.In);
        tween.Start();
    }
}
