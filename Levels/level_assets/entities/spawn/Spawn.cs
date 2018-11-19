using Godot;

public class Spawn : Node2D
{
    // State of the spawn.
    public bool Active = false;

    // Particles reference
    private Particles2D Flames;
    private Particles2D Smoke;
    private Level Level;

    // Box for debugging draw.
    public Rect2 Box;

    public override void _Ready()
    {
        // Get Node
        Flames = GetNode("fire/flame") as Particles2D;
        Smoke = GetNode("fire/smoke") as Particles2D;
        Level = GetNode("../..") as Level;

        // Set Debug box
        Box = new Rect2(Position, new Vector2(8, 8));
    }
    public override void _PhysicsProcess(float delta)
    {
        // If Activate, Emit particles( Fire and smoke).
        Flames.Emitting = Active;
        Smoke.Emitting = Active;
    }

    /// <summary>
    /// When the player Walks on a spawn. Activate the fire.
    /// </summary>
    /// <param name="body"></param>
    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if (body.IsInGroup("Player"))
        {
			Level.ResetSpawns();
            Active = true;
            Level.ChooseSpawn();
        }
    }
}


