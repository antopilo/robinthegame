using Godot;
using System;

public class BouncingBall : Node
{
    // Not done yet.
    Sprite Sprite;
    Rect2 Box;

    public override void _Ready()
    {
        Sprite = GetNode("Sprite") as Sprite;
    }

    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if (body is Player)
            return;

    }
}



