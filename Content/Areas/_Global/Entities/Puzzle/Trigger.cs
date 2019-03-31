using Godot;
using System;

public class Trigger : Node2D
{
    public bool Triggered = false;
    public bool CanTrigger = true;

    private AnimatedSprite AnimatedSprite;

    // private Color Green = new Color(0, 1, 0);
    // private Color Red = new Color(1, 0, 0);
    // private Color Blue = new Color(0, 0, 1);
    // private Color DebugColor = new Color(1, 0, 0);

    public override void _Ready()
    {
        AnimatedSprite = GetNode("AnimatedSprite") as AnimatedSprite;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        AnimatedSprite.Animation = Triggered ? "Down" : "Up";
        // Debug only: Update();
    }

    // public override void _Draw()
    // {
    //    DebugColor = Triggered ? Green : Blue;
    //    DrawRect(new Rect2(0,0,8,8), DebugColor, false);
    // }

    public void Interact()
    {
        Triggered = true;
        (GetNode("InteractionZone") as Node2D).QueueFree();
    }
}
