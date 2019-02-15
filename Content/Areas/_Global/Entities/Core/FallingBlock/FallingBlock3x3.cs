using Godot;
using System;

public class FallingBlock3x3 : KinematicBody2D
{
    private Vector2 Origin;

    // Physics
    const float GRAVITY = 4;
    private Vector2 Velocity = new Vector2();

    private bool Frozen = false;
    private bool Triggered = false;
    private bool CollidingWithPlayer = false;

    // References
    private Area2D KillZone;
    private RayCast2D RayCast;
    private KinematicCollision2D Collision;
    private Player Player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("yoyoyo " + Origin);
        Origin = Position; // Get start position for reset.

        // References
        KillZone = GetNode("KillZone") as Area2D;
        RayCast = GetNode("DownRaycast") as RayCast2D;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Frozen)
            return;

        // Check for player ref.
        if (Player != null)
            CheckPlayer();

        CheckCast(); // Check under
        CheckTouch(); // Check for contact

        if (!Triggered || Frozen) 
            return;
        
        ApplyGravity(delta);
    }

    /// <summary>
    /// Check if the player passes under the block.
    /// </summary>
    private void CheckCast()
    {
        if (RayCast.IsColliding() && (RayCast.GetCollider() is Player))
        {
            Triggered = true;
        }
    }

    /// <summary>
    /// Check if the player touches the block.
    /// </summary>
    private void CheckTouch()
    {
        // Get collision
        Collision = MoveAndCollide(Velocity);

        if (Collision == null)
            return;
        
        if (Collision.GetCollider() is Player)
        {
            Triggered = true;
        }
        else if(Collision.Normal == new Vector2(0, -1) && !(Collision.GetCollider() is Spike))
        {
            Frozen = true;
            (Root.Player.Camera as Camera).Shake(2f, 0.05f);
        }
    }


    private void ApplyGravity(float delta)
    {
        Velocity.y += GRAVITY * delta;
    }

    private void CheckPlayer()
    {
        Player.Spawn(true);
    }


    // Resetting
    public void Reset()
    {
        GD.Print("Resetted falling block at : " + Origin);
        Frozen = Triggered = false;
        Velocity = new Vector2();
        GD.Print(Origin);
        Position = Origin;
        Player = null;
    }

    #region Signals
    // When something enters the killzone
    private void _on_KillZone_body_entered(object body)
    {
        if (body is Player)
        {
            Player = body as Player;
            CollidingWithPlayer = true;
        }
    }

    // When something leaves the killzone.
    private void _on_KillZone_body_exited(object body)
    {
        if (body is Player)
        {
            CollidingWithPlayer = false;
        }
    } 
    #endregion
}


