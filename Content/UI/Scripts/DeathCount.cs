using Godot;
using System;

public class DeathCount : Control
{
    private Label label;
    public int Deaths = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label = GetNode("HBoxContainer/Count") as Label;
    }

    public override void _Process(float delta)
    {
        label.Text = "x" + Deaths.ToString();
    }
}
