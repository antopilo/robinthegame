using Godot;

public class Spawn : Node2D
{
    Level Room;
    public bool Active = false;
    Particles2D Flames;
    Particles2D Smoke;

    public override void _Ready()
    {
        base._Ready();
        Room = (Level)GetNode("../../");
        Flames = (Particles2D)GetNode("fire/flame");
        Smoke = (Particles2D)GetNode("fire/smoke");
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Flames.Emitting = Active;
        Smoke.Emitting = Active;
    }

    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if (body.IsInGroup("Player"))
        {
            Active = true;
            Room.ChooseSpawn();
        }
    }
}


