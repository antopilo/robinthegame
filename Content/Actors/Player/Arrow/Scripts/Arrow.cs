using Godot;

public enum ArrowType { Normal, Magic, Guided, Teleport, Distraction}

public class Arrow : KinematicBody2D
{
    const float MAX_FUEL = 100f;
    const float DEADZONE = 0.25f;
    const float GRAVITY = 2;
    
    // Nodes
    public Weapon Weapon;
    public Sprite Sprite;
    private Tween T;
    private Tween T2;
    private float Fuel = 100f;
    private float FuelCost = 0.1f;
    private float TargetRotation = 0f;

    // Settings n States    
    public bool ControllerMode = false;
    private bool IsControlled = true;
    public bool Frozen = false;
    public bool MovingBack = false;
    private bool CanDash = true;
    public float Speed = 0.75f;
    private float Angle;
    private Vector2 Direction;
    private Vector2 LastDirection;
    private Vector2 FreezePosition;
    private Vector2 ColliderOffset;
    private Object Collider;
    public Player Player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "Arrow";
        T = GetNode("Tween") as Tween;
        T2 = GetNode("Tween2") as Tween;
        Sprite = (Sprite)GetNode("Sprite");
        Player = Root.Player;
        Weapon = Player.GetNode("Weapon") as Weapon;
        this.LastDirection = new Vector2(1, 0);
        Weapon.CanShoot = false ;
        Player.ArrowExist = true;
        Player.Arrow = this;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("fire") && CanDash)
            Ability();
        else if ( (Frozen && !MovingBack) && (@event.IsActionPressed("fire") || @event.IsActionPressed("right_click") ) )
            ReturnToPlayer();
    }


    public void Jiggle()
    {
        T2.StopAll();
        T2.InterpolateProperty(this, "rotation_degrees", TargetRotation - 45, TargetRotation, 0.8f,
            Tween.TransitionType.Elastic, Tween.EaseType.Out);
        T2.Start();
    }

    public void TiltDown()
    {
        T2.StopAll();
        T2.InterpolateProperty(this, "rotation_degrees", TargetRotation , TargetRotation - 15, 0.8f,
            Tween.TransitionType.Elastic, Tween.EaseType.Out);
        T2.Start();
    }

    public void Reset()
    {
        T2.StopAll();
        T2.InterpolateProperty(this, "rotation_degrees", this.RotationDegrees , TargetRotation, 0.2f,
            Tween.TransitionType.Linear, Tween.EaseType.Out);
        T2.Start();
    }


    public override void _PhysicsProcess(float delta)
    {
        if (MovingBack) // If the arrow is returning to the player. Return.
        {
            LookAt(Root.Player.GlobalPosition);
            return;
        }

        if(IsControlled && !Frozen)
        {
            // Either Controller mode or Mouse mode.
            if (ControllerMode)
            {
                JoyStickControl();
                GlobalRotation = Angle;
            }
            else
            {
                MouseControl();
            }

            if (Fuel <= 0)
                IsControlled = false;
            Fuel -= FuelCost;
        }
        if (Frozen)
        {
            FollowCollider();
        }

        CheckCollision();

        //if (Collider != null)
        //    FollowCollider();
    }

    #region Controls

    /// <summary>
    /// When using the mouse, the arrow follows the mouse position.
    /// This is an algorithm that is yet to be solved because I dont know what 1.33 is.
    /// It is just a random number that I guessed and it worked. We need to solve this asap
    /// because 1.33 is only valid for 1280x720 so it must be something in relation with
    /// screen resolution. Good luck.
    /// </summary>
    public void MouseControl()
    {
        Vector2 Mouse = GetGlobalMousePosition();
        Vector2 Center = Root.Player.Camera.GetCameraScreenCenter();
        Vector2 Offset = Center - (GetViewportRect().Size / 2f);
        float StretchFactor = OS.WindowSize.x / 320;

        LookAt((Mouse / StretchFactor) + Offset * ((OS.WindowSize.x - 320f) / OS.WindowSize.x));
    }

    public void JoyStickControl()
    {
        // Get angle of the joystick and transpose it to the arrow.
        Direction = new Vector2(Godot.Input.GetJoyAxis(0, (int)JoystickList.Axis2), Godot.Input.GetJoyAxis(0, (int)JoystickList.Axis3));
        Angle = Direction.Angle();

        // If the joystick is in neutral position. Angle = last angle of the Joystick.
        if (Mathf.Abs(Direction.x) < DEADZONE && Mathf.Abs(Direction.y) < DEADZONE)
            Angle = LastDirection.Angle();
        else
            LastDirection = Direction;
    }

    #endregion

    #region Physics
    /// <summary>
    /// This method is triggered when the arrow hit something. It then decide what angle
    /// should the arrow be depending on the angle of the surface. it Snaps at every 90 degrees.
    /// </summary>
    public void CheckCollision()
    {
        KinematicCollision2D Collision = MoveAndCollide(GlobalTransform.x * Speed, false, true);
        
        if (!Frozen && Collision != null)
        {
            Vector2 NormalFloor = new Vector2(0, 1);
            Vector2 NormalCeiling = new Vector2(0, -1);
            Vector2 NormalWallR = new Vector2(1, 0);
            Vector2 NormalWallL = new Vector2(-1, 0);
            Collider = Collision.GetCollider();

            if (Collision.Normal == NormalFloor || Collision.Normal == NormalCeiling)
                GlobalRotationDegrees = GlobalRotationDegrees > -180 && GlobalRotationDegrees < 0 ? -90 : 90;
            else if (Collision.Normal == NormalWallL || Collision.Normal == NormalWallR)
                GlobalRotationDegrees = (GlobalRotationDegrees > -90 && GlobalRotationDegrees < 90) ? 0 : 180;
            else
                GlobalRotation =  Mathf.Stepify(Collision.Normal.Angle(), 90);

            FreezeArrow();
        }
        // If the arrow leaves the level screen. return to player.
        else if (Position.x <= Root.GameController.CurrentRoom.LevelPosition.x ||
        Position.y <= Root.GameController.CurrentRoom.LevelPosition.y ||
        Position.x >= (Root.GameController.CurrentRoom.LevelPosition.x + Root.GameController.CurrentRoom.LevelSize.x) ||
        Position.y >= (Root.GameController.CurrentRoom.LevelPosition.y + Root.GameController.CurrentRoom.LevelSize.y))
            ReturnToPlayer();
        else if (Frozen)
            Position = FreezePosition;
    }

    /// <summary>
    /// Freezes the arrow and stops the particles.
    /// </summary>
    public void FreezeArrow()
    {
        FreezePosition = GlobalPosition;
        SetCollisionLayerBit(0, true);

        var collision = MoveAndCollide(new Vector2());
        if (collision != null && collision.GetCollider() is Player)
            ReturnToPlayer();


        
        Speed = 0;
        Frozen = true;
        CanDash = false;

        ColliderOffset = ((Collider as Node2D).GlobalPosition - GlobalPosition);

        (GetNode("Particles2D") as Particles2D).Emitting = false;
        TargetRotation = this.RotationDegrees;
        Jiggle();
    }

    /// <summary>
    /// Slowly Moves the arrow to the player using a Tween
    /// TODO: Make scaling scale from the distance between the player and the arrow(currentPos).
    /// </summary>
    public void ReturnToPlayer()
    {
        float Time = (Position - Root.Player.Position).Length() / 300; // This is to have a constant speed.

        T.FollowProperty(this, "global_position", GlobalPosition, Root.Player, "global_position", Time,
            Tween.TransitionType.Quint, Tween.EaseType.In);
        T.InterpolateProperty(this, "scale", Scale, new Vector2(), Time,
            Tween.TransitionType.Expo, Tween.EaseType.In);

        T.Start();

        Root.Player.ArrowExist = true;
        MovingBack = true;
        SetCollisionLayerBit(0, false);

        
    } 
    #endregion


    private void FollowCollider()
    {
        GlobalPosition = (Collider as Node2D).GlobalPosition - ColliderOffset;
        if (Collider is FallingBlock3x3 && (Collider as FallingBlock3x3).Triggered && (!(Collider as FallingBlock3x3).Frozen) && GlobalRotationDegrees == -90)
            ReturnToPlayer();
    }


    public virtual void Ability()
    {
        Dash();
    }


    public void Dash()
    {
        if (ControllerMode)
        {
            JoyStickControl();
            GlobalRotation = Angle;
        }
        else
        {
            MouseControl();
        }
            
        IsControlled = false;
        Speed = 3;
        CanDash = false;
    }

    /// <summary>
    /// Called when the arrSow is returned to the player
    /// when tween is done.
    /// </summary>
    /// <param name="object"></param>
    /// <param name="key"></param>
    private void _on_Tween_tween_completed(Object @object, NodePath key)
    {
        if(key == "rotation_degrees")
            return;

        Player.StateMachine.SetState("Idle");

        Weapon.CanShoot = true;
        CallDeferred("queue_free");
    }

    public void Destroy()
    {

        Weapon.CanShoot = true;
        CallDeferred("queue_free");
    }
}



