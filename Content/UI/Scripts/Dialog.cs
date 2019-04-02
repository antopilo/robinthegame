using Godot;

public class Dialog : Control
{
    private Label SkipLabel;
    private Label Text;
    private MarginContainer Container;
    private Timer Delay;
    private ColorRect Background;

    private Tween T;
    private int Speed = 20;

    private bool Frozen = false;
    private bool Ended = false;
    public bool Opened = false;
    private float time = 0;

    public override void _Ready()
    {
        Container = (MarginContainer)GetNode("Middle");
        SkipLabel = GetNode("SkipLabel") as Label;
        Text = GetNode("Middle/Text") as Label;
        Background = (ColorRect)GetNode("ColorRect");
        Delay = GetNode("Timer") as Timer;
        T = GetNode("Tween") as Tween;
    }

    public override void _Process(float delta)
    {
        RectSize = new Vector2(RectSize.x, Container.RectSize.y);
        time += delta;
        //if(time < 0.5)
        //    return;

        if(Frozen && !Ended && Input.IsActionJustPressed("Interact"))
        {
            T.RemoveAll();
            this.Modulate = new Color(1, 1, 1, 1);
            SkipLabel.Modulate = new Color(1, 1, 1, 1);
            Text.PercentVisible = 100;
            Ended = true;
        }
        else if(Frozen && Ended && Input.IsActionJustPressed("Interact"))
        {
            Root.Player.CanControl = true;
            Root.Player.CanInteract = true;
            Frozen = false;
            Visible = false;
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
        SkipLabel.Modulate = new Color(1, 1, 1, 0);
        Root.Player.CanInteract = false;
        
        Opened = true;
        Frozen = true;
        Ended = false;
        Visible = true;

        //GetTree().Paused = true;
        Root.Player.ResetInput();
        Root.Player.CanControl = false;
        Text.PercentVisible = 0f;
        Text.Text = pMessage;

        // Resizing
        Text.RectSize = new Vector2(Text.RectSize.x, Text.RectMinSize.y);
        RectSize = new Vector2(RectSize.x, RectMinSize.y);
        Background.RectSize = new Vector2(RectSize.x, RectSize.y);

        float AnimationLength = pMessage.Split(' ').Length * 0.18f; // Consistent speed per letter.

        T.StopAll();
        T.InterpolateProperty(this, "modulate", new Color(1,1,1,0),
            new Color(1,1,1,1), 0.25f, Tween.TransitionType.Linear, Tween.EaseType.Out);
        T.InterpolateProperty(Text, "percent_visible", 0f,
            1f, AnimationLength, Tween.TransitionType.Linear, Tween.EaseType.Out);
        T.Start();
    }

    

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        if(!Ended && @object == Text)
        {
            T.InterpolateProperty(SkipLabel, "modulate", new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.25f,
                Tween.TransitionType.Linear, Tween.EaseType.In);
            T.Start();
            Ended = true;
        
        }
    }
}