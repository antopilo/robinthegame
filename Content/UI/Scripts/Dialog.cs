using Godot;

public class Dialog : Control
{
    private Label SkipLabel;
    private RichTextLabel Text;
    private Timer Delay;
    private Tween T;
    private int Speed = 20;

    private bool Frozen = false;
    private bool Ended = false;
    public bool Opened = false;
    public override void _Ready()
    {
        SkipLabel = GetNode("SkipLabel") as Label;
        Text = GetNode("HBoxContainer/Middle/Text") as RichTextLabel;
        Delay = GetNode("Timer") as Timer;
        T = GetNode("Tween") as Tween;
    }

    public override void _Process(float delta)
    {
        if(Frozen && !Ended && Input.IsActionJustPressed("jump"))
        {
            T.StopAll();
            Text.VisibleCharacters = 999;
            Ended = true;
            SkipLabel.Visible = true;
        }
        else if(Frozen && Ended && Input.IsActionJustPressed("jump"))
        {
            GetTree().Paused = false;
            Frozen = false;
            Visible = false;
            SkipLabel.Visible = false;
            Opened = false;
        }
    }


    /// <summary>
    /// Slowly reveal pMessage letter by letter in the dialog box.
    /// </summary>
    /// <param name="pMessage"></param>
    public void ShowMessage(string pMessage)
    {
        Opened = true;
        Frozen = true;
        Ended = false;

        GetTree().Paused = true;
        Visible = true;

        Text.Clear();

        Text.VisibleCharacters = 0;
        Text.Text = pMessage;

        var AnimationLength = pMessage.Length / Speed; // Consistent speed per letter.

        T.StopAll();
        T.InterpolateProperty(Text, "visible_characters", 0,
            pMessage.Length, AnimationLength, Tween.TransitionType.Linear, Tween.EaseType.Out);
        T.Start();
    }
}
