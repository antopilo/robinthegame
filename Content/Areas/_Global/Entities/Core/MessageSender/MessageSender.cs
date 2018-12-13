using Godot;
using System;

public class MessageSender : Node2D
{
    [Export(PropertyHint.MultilineText)] string Message = "Enter your message here";
    GameController World;
    Sprite Mark;

    bool triggered = false;
    bool CanInteract = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        World = GetNode("../../../") as GameController;
        Mark = GetNode("Mark") as Sprite;
    }

    public override void _Process(float delta)
    {
        if(CanInteract && Input.IsActionJustPressed("interact"))
            World.DialogController.ShowMessage(Message.ToUpper());
        Mark.Visible = CanInteract && triggered;
    }

    private void _on_TriggerZone_body_entered(PhysicsBody2D body)
    {
        if(body is Player && (body as Player).Alive)
        {
            if (!triggered)
            {
                World.DialogController.ShowMessage(Message.ToUpper());
                triggered = true;
            }
            CanInteract = true;
        }
    }
    private void _on_TriggerZone_body_exited(PhysicsBody2D body)
    {
		if((body is Player) && (body as Player).Alive)
        {
        	CanInteract = false;
        }
    }
}
