using System;
using Godot;

public class Stool : Node2D
{
    public Vector2 InteractionOffset = new Vector2(4, 4);
    
    public void Interact()
    {
        Root.Player.GlobalPosition = this.GlobalPosition + new Vector2(4, 0);
        Root.Player.StateMachine.SetState("Sit");
    }
}

