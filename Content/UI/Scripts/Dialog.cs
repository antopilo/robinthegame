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
    private float time = 0;

    public override void _Ready()
    {
        SkipLabel = GetNode("SkipLabel") as Label;
        Text = GetNode("HBoxContainer/Middle/Text") as RichTextLabel;
        Delay = GetNode("Timer") as Timer;
        T = GetNode("Tween") as Tween;
    }

    public override void _Process(float delta)
    {
        time += delta;
        if(time < 0.5)
            return;

        if(Frozen && !Ended && Input.IsActionJustPressed("Interact"))
        {
            T.StopAll();
            Text.VisibleCharacters = 999;
            Ended = true;
            SkipLabel.Visible = true;
        }
        else if(Frozen && Ended && Input.IsActionJustPressed("Interact"))
        {
            GetTree().Paused = false;
            Root.Player.CanInteract = true;
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
        time = 0;
        Root.Player.CanInteract = false;

        Opened = true;
        Frozen = true;
        Ended = false;
        Visible = true;
        GetTree().Paused = true;

        Text.Clear();

        Text.PercentVisible = 0f;
        Text.Text = pMessage;

        float AnimationLength = pMessage.Length / Speed + 0.25f; // Consistent speed per letter.
        GD.Print("text reveal length : " + AnimationLength);
        T.StopAll();
        T.InterpolateProperty(Text, "percent_visible", 0f,
            1f, AnimationLength, Tween.TransitionType.Linear, Tween.EaseType.Out);
        T.Start();
    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        GD.Print("Tween done!");
        if(!Ended)
        {
            Ended = true;
            SkipLabel.Visible = true;
        }
    }
}