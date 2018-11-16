using Godot;
using System;

public class Dialog : Control
{
    private RichTextLabel Text;
    private Timer Delay;
    private Tween T;
    private int Speed = 10;
    
    public override void _Ready()
    {
        base._Ready();

        Text = (RichTextLabel)GetNode("label");
        Delay = (Timer)GetNode("Timer");
        T = (Tween)GetNode("Tween");
    }

    public void ShowMessage(string pMessage)
    {
        float Time = pMessage.Length / Speed;

        T.StopAll();
        Text.VisibleCharacters = 0;
        Text.Clear();
        Text.Text = pMessage;
        T.InterpolateProperty(Text, "visible_characters", 0,
            pMessage.Length, Time, Tween.TransitionType.Linear, Tween.EaseType.Out);
        T.Start();
    }
}
