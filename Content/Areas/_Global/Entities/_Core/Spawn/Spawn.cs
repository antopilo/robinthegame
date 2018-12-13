using Godot;

public class Spawn : Node2D
{


    // Particles reference
    private Particles2D Flames;
    private Particles2D Smoke;
    private Level Level;

    public override void _Ready()
    {
        // Get Node
        Flames = GetNode("fire/flame") as Particles2D;
        Smoke = GetNode("fire/smoke") as Particles2D;
        Level = GetNode("../..") as Level;
        
    }
    public override void _PhysicsProcess(float delta)
    {
        // If Activate, Emit particles( Fire and smoke).
        Flames.Emitting = Level.SpawnPosition == this.GlobalPosition;
        Smoke.Emitting = Level.SpawnPosition == this.GlobalPosition;
    }

    /// <summary>
    /// When the player Walks on a spawn. Activate the fire.
    /// </summary>
    /// <param name="body"></param>
    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if (Level.SpawnPosition == this.GlobalPosition)
            return;

        if (body.IsInGroup("Player") && (body as Player).Alive)
        {
            (GetNode("SFX/LightUp") as AudioStreamPlayer).Play();
            Level.ChangeSpawn(this);
        }
    }
}


