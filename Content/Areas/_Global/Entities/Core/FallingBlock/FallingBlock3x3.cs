using Godot;
using System;

public class FallingBlock3x3 : KinematicBody2D
{
    private Vector2 Origin;
    [Export] Vector2 Dimension = new Vector2(3,3);

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
        MakeCollision();

        Origin = Position; // Get start position for reset.

        // References
        KillZone = GetNode("KillZone") as Area2D;
        RayCast = GetNode("DownRaycast") as RayCast2D;

    }

    public override void _PhysicsProcess(float delta)
    {
        Update();
        // locking on the X axis.
        Position = new Vector2(Origin.x, Position.y);
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
        MoveLocalY(Velocity.y);
    }

    public override void _Draw()
    {
        Color color;
        if (Frozen && Triggered)
            color = new Color(1, 0, 0);
        else if (!Frozen && Triggered)
            color = new Color(0, 1, 0);
        else
            color = new Color(0, 0 ,1);

        DrawRect(new Rect2(new Vector2(0, 0), new Vector2(Dimension.x * 8, Dimension.y * 8)), color, true);
    }

    // Make the collision in relation with the size of the falling block.
    private void MakeCollision()
    {
        float margin = this.GetSafeMargin();

        // Turning off tilemap collisions
        var tm = GetNode("TileMap") as TileMap;
        tm.SetCollisionLayerBit(0,false);
        tm.SetCollisionMaskBit(0,false);
        tm.ShowBehindParent = true; 

        // Adding the collision.
        var collisionShape = new CollisionShape2D()
        {
            Name = "Collision",
            Position = new Vector2((Dimension.x * 8) / 2f , (Dimension.y * 8 ) / 2) ,
            Shape = new RectangleShape2D()
            {
                Extents = new Vector2((Dimension.x * 8 - (margin * 2)) / 2 , ((Dimension.y * 8) - (GetSafeMargin() * 2)) / 2 )
            }
        };

        // Adding killzone under the block.
        var killZone = new Area2D()
        {
            Name = "KillZone",
            Position = new Vector2((Dimension.x * 8) / 2 , (Dimension.y * 8))
        };
        killZone.AddChild(new CollisionShape2D()
        {
            Shape = new RectangleShape2D()
            {
                Extents = new Vector2((Dimension.x * 8 - this.GetSafeMargin()) / 2, 1)
            }
        });

        killZone.Connect("body_entered", this, "_on_KillZone_body_entered");
        killZone.Connect("body_exited", this, "_on_KillZone_body_exited");

        AddChild(killZone);
        AddChild(collisionShape);
    }


    /// <summary>
    /// Check if the player passes under the block.
    /// </summary>
    private void CheckCast()
    {
        if (RayCast.IsColliding() && (RayCast.GetCollider() is Player) && (RayCast.GetCollider() as Player).Alive )
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
        Collision = MoveAndCollide(Velocity, true, true, true);

        if (Collision == null)
            return;
        
        if (Collision.GetCollider() is Player && (Collision.GetCollider() as Player).Alive)
        {
            Triggered = true;
        }

        // If it is a tile or a falling block that is frozen.
        if(Triggered && Collision.Normal == new Vector2(0, -1) && (Collision.GetCollider() is TileMap || 
            (Collision.GetCollider() is FallingBlock3x3 && (Collision.GetCollider() as FallingBlock3x3).Frozen)) )
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
        Frozen = Triggered = false;
        Velocity = new Vector2();
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


