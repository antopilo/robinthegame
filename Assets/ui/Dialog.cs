using Godot;

public class Dialog : Control
{
    private RichTextLabel Text;
    private Timer Delay;
    private Tween T;
    private int Speed = 10;
    
    public override void _Ready()
    {
        base._Ready();

        Text = GetNode("label") as RichTextLabel;
        Delay = GetNode("Timer") as Timer;
        T = GetNode("Tween") as Tween;
    }

    /// <summary>
    /// Slowly reveal pMessage letter by letter in the dialog box.
    /// </summary>
    /// <param name="pMessage"></param>
    public void ShowMessage(string pMessage)
    {
        Text.VisibleCharacters = 0;
        T.StopAll();
        Text.Clear();

        Text.Text = pMessage;

        var AnimationLength = pMessage.Length / Speed; // Consistent speed per letter.

        T.InterpolateProperty(Text, "visible_characters", 0,
            pMessage.Length, AnimationLength, Tween.TransitionType.Linear, Tween.EaseType.Out);
        T.Start();
    }
}
