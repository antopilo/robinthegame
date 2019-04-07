using Godot;
using System;

public class ItemNotification : MarginContainer
{

    public float ShowTime = 2f;
    public string IconPath = "res://icon.png";
    public string ItemName = "ITEM_NAME";
    public int Amount = 0;

    public Label NotificationLabel;
    public TextureRect NotificationIcon;

    private Vector2 TargetPosition;
    private Tween Tween;
    private Timer Timer;
    private Control Offset;

    private bool Appeared = false;

    public override void _Ready()
    {
        Offset = (Control)GetNode("Offset");
        NotificationLabel = (Label)GetNode("Offset/Control/MarginContainer2/Label");
        NotificationIcon = (TextureRect)GetNode("Offset/Control/MarginContainer/TextureRect");
        Tween = (Tween)GetNode("Tween");
        Timer = (Timer)GetNode("Timer");
        Timer.WaitTime = ShowTime;

        // Load Icon
        NotificationLabel.Text = Amount + " " + ItemName + " " + "added.";
        NotificationIcon.Texture = ResourceLoader.Load(IconPath) as Texture;

        
        TargetPosition = Offset.RectPosition;
        Offset.RectPosition = Offset.RectPosition + new Vector2(Offset.RectSize.x, Offset.RectPosition.y);

        // Tween to position
        Tween.InterpolateProperty(Offset, "rect_position", Offset.RectPosition, TargetPosition, 0.5f,
            Tween.TransitionType.Elastic, Tween.EaseType.Out);
        Tween.Start();
    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        if (!Appeared)
        {
            Timer.Start();
        }
        else
            this.QueueFree();
    }

    private void _on_Timer_timeout()
    {
        Tween.InterpolateProperty(this, "modulate", new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 1f,
            Tween.TransitionType.Linear, Tween.EaseType.In);
        Tween.Start();
        Appeared = true;
    }
}






