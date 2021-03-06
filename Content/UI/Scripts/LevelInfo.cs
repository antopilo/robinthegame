using Godot;
using System;

public class LevelInfo : Control
{
    private Tween Tween;
    private Control Container;
    private Control Scale;

    private Label LevelName;
    private Node Difficulty;
    private Control Skull1, Skull2, Skull3;
    public override void _Ready()
    {
        Tween = GetNode("Tween") as Tween;
        Container = GetNode("MarginContainer") as Control;
        Scale = GetNode("MarginContainer/Info") as Control;

        Difficulty = GetNode("MarginContainer/Info/VSplit/Difficulty") as Node;
        LevelName = GetNode("MarginContainer/Info/VSplit/LevelName") as Label;

        Skull1 = GetNode("MarginContainer/Info/VSplit/Difficulty/Skull1") as Control;
        Skull2 = GetNode("MarginContainer/Info/VSplit/Difficulty/Skull2") as Control;
        Skull3 = GetNode("MarginContainer/Info/VSplit/Difficulty/Skull3") as Control;

        (this as CanvasItem).Modulate = new Color(1, 1, 1, 0);
        LevelName.Text = "";
    }
    
    public void UpdateInfo(Level pLevel)
    {
        if (pLevel.LevelName == "")
            return;

        Tween.StopAll();
        LevelName.Text = pLevel.LevelName;
        switch (pLevel.LevelDifficulty)
        {
            case 1:
                Skull1.Visible = true;
                Skull2.Visible = false;
                Skull3.Visible = false;
                break;
            case 2:
                Skull1.Visible = true;
                Skull2.Visible = true;
                Skull3.Visible = false;
                break;
            case 3:
                Skull1.Visible = true;
                Skull2.Visible = true;
                Skull3.Visible = true;
                break;
            default:
                Skull1.Visible = false;
                Skull2.Visible = false;
                Skull3.Visible = false;
                break;
        }
        Tween.InterpolateProperty(this as CanvasItem, "modulate", new Color(1, 1, 1, 0), new Color(1, 1, 1, 1),
            2f, Tween.TransitionType.Quad, Tween.EaseType.InOut);
        Tween.Start();
    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        if((this as CanvasItem).Modulate == new Color(1, 1, 1, 1))
        {
            Tween.InterpolateProperty(this as CanvasItem, "modulate", new Color(1, 1, 1, 1), new Color(1, 1, 1, 0),
                2f, Tween.TransitionType.Quad, Tween.EaseType.InOut);
            Tween.Start();
        }
        
    }


}
