using Godot;

public class Dart : Node2D
{
    [Export]float Speed = 1;
    public Vector2 Direction;

    public override void _PhysicsProcess(float delta)
    {
        MoveLocalX(-Speed);
    }

    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if (body is Player && (body as Player).Velocity.y > 0 && body.GlobalPosition.y <= this.GlobalPosition.y)
            (body as Player).SuperJump();
        else if (body is Player)
            (body as Player).Spawn(true);

        Destroy();
    }

    private void _on_DeathTime_timeout()
    {
        QueueFree();
    }

    private void _on_Area2D_area_entered(Area2D area)
    {
        if (area.GetParent() is Dart)
            (area.GetParent() as Dart).Destroy();

        if (area.GetParent() is Spawn)
        {
            GD.Print("WARNING: A DART IS HITTING A SPAWN DIRECTLY IN LEVEL: " + GetNode("../../..").Name);
            return;
        }
        Destroy();
    }

    public void Destroy()
    {
        (GetNode("Sprite") as Sprite).Visible = false;
        (GetNode("Break") as AudioStreamPlayer).Play();
        SetPhysicsProcess(false);
        CallDeferred("SetDisabled, true", GetNode("Area2D/CollisionShape2D") as CollisionShape2D);
        (GetNode("DeathTime") as Timer).Start();
        (GetNode("DeathParticles") as Particles2D).Emitting = true;
        QueueFree();
    }
}
