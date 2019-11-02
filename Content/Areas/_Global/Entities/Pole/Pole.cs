using System;
using Godot;

public class Pole : Node2D
{

    public override void _Process(float delta)
    {

    }

    private void _on_Area2D_body_entered(object body)
    {
        if(body is Player)
        {
            var host = (Player)body;

            if(host.StateMachine.CurrentState.StateName != "PoleGrace" && host.Velocity.y < 0)
            {
                Console.Log("Pole");
                host.StateMachine.SetState("PoleGrace");
                host.GlobalPosition = new Vector2(host.GlobalPosition.x, this.GlobalPosition.y + 4f);
            }
        }
    }

}



