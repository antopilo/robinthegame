using Godot;
using System;

public class FallingBlock3x3 : KinematicBody2D
{
    const float GRAVITY = 4;

    private Area2D KillZone;
    private bool Frozen = false;

    private bool Triggered = false;
    private Vector2 Velocity = new Vector2();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        KillZone = GetNode("KillZone") as Area2D;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed("interact"))
            Triggered = true;

        if (!Triggered || Frozen)
            return;


        ApplyGravity(delta);

        if (MoveAndCollide(Velocity) != null && !(MoveAndCollide(Velocity).GetCollider() is Player))
        {
            
            Frozen = true;
        }


        MoveAndCollide(Velocity);
    }

    private void ApplyGravity(float delta)
    {
        Velocity.y += GRAVITY * delta;
    }



    private void _on_KillZone_body_entered(object body)
    {
        if (body is Player && (body as Player).State == States.Ground)
            (body as Player).Spawn(true);
    }

}
