using Godot;
using System;

public class Lift : Node2D
{
    [Export] public Vector2 Direction = new Vector2(0, 1);
    [Export] public float Speed = 2f;
    [Export] public string RequiredItem = "Lever";

    public bool CanInteract = true;
    public Vector2 InteractionOffset = new Vector2();

    private KinematicBody2D Platform;
    private bool Activated = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Platform = (KinematicBody2D)GetNode("Platform");
    }


}
