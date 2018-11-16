using Godot;
using System;

public class Arrow : KinematicBody2D
{
    public Weapon Weapon;
    Player Player;
    GameController World;
    Tween T;

    const float MAX_FUEL = 100f;
    float Fuel = 100f;
    float FuelCost = 0.1f;

    const float DeadZoneSize = 0.25f;
    public bool ControllerMode = false;
    bool IsControlled = true;
    bool Frozen = false;
    bool MovingBack = false;
    bool CanDash = true;
    bool DeadZone = false;

    Vector2 NormalFloor = new Vector2(0, 1);
    Vector2 NormalCeiling = new Vector2(0, -1);
    Vector2 NormalWallR = new Vector2(1, 0);
    Vector2 NormalWallL = new Vector2(-1, 0);

    float Gravity = 2;
    float Speed = 0.5f;
    Vector2 Direction;
    float Angle;
    Vector2 LastDirection;
    Vector2 FreezePosition;
    Vector2 AimPosition;
   
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Weapon = GetNode("../Player/Weapon") as Weapon;
        Player = GetNode("../Player") as Player;
        World = GetNode("..") as GameController;
        T = GetNode("Tween") as Tween;

        LastDirection = new Vector2((float)Player.LastDirectionX, 0);
        Weapon.CanShoot = false ;
        Player.ArrowExist = true;
        Player.Arrow = this;

        this.Name = "Arrow";
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("fire") && CanDash)
            Dash();
        else if (@event.IsActionPressed("fire") || @event.IsActionPressed("right_click"))
            ReturnToPlayer();
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (MovingBack)
        {
            LookAt(Player.GlobalPosition);
            return;
        }

        if(IsControlled && !Frozen)
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

            if (Fuel <= 0)
                IsControlled = false;
            Fuel -= FuelCost;
        }
        ApplyGravity();
        CheckCollision();
    }

    public void MouseControl()
    {
        Vector2 Mouse = GetGlobalMousePosition();
        Vector2 Center = (Player.Camera).GetCameraScreenCenter();
        float StretchFactor = OS.WindowSize.x / 320;
        LookAt(Mouse / StretchFactor + (Center - (GetViewportRect().Size / 2f))/ 1.33f);
    }

    public void JoyStickControl()
    {
        Direction = new Vector2(Input.GetJoyAxis(0, (int)JoystickList.Axis2), Input.GetJoyAxis(0, (int)JoystickList.Axis3));
        Angle = Direction.Angle();

        if (Mathf.Abs(Direction.x) < DeadZoneSize && Mathf.Abs(Direction.y) < DeadZoneSize)
            Angle = LastDirection.Angle();
        else
            LastDirection = Direction;
    }

    public void CheckCollision()
    {
        KinematicCollision2D Collision = MoveAndCollide(GlobalTransform.x * Speed);

        if (!Frozen && Collision != null)
        {
            if (Collision.Normal == NormalFloor || Collision.Normal == NormalCeiling)
            {
                if (GlobalRotationDegrees > -180 && GlobalRotationDegrees < 0)
                    GlobalRotationDegrees = -90;
                else
                    GlobalRotationDegrees = 90;
            }
            else if (Collision.Normal == NormalWallL || Collision.Normal == NormalWallR)
            {
                if (GlobalRotationDegrees > -90 && GlobalRotationDegrees < 90)
                    GlobalRotationDegrees = 0;
                else
                    GlobalRotationDegrees = 180;
            }
            FreezeArrow();
        }
        else if (Position.x <= World.CurrentRoom.LevelPosition.x || 
                Position.y <= World.CurrentRoom.LevelPosition.y ||
                Position.x >= (World.CurrentRoom.LevelPosition.x + World.CurrentRoom.LevelSize.x) ||
                Position.y >= (World.CurrentRoom.LevelPosition.y + World.CurrentRoom.LevelSize.y))
        {
            ReturnToPlayer();
        }
        else if (Frozen)
            Position = FreezePosition;
    }

    public void FreezeArrow()
    {
        FreezePosition = GlobalPosition;
        SetCollisionLayerBit(0, true);
        Speed = 0;
        Frozen = true;
        CanDash = false;

        ((Particles2D)GetNode("Particles2D")).Emitting = false;
    }

    public void ReturnToPlayer()
    {
        float Time = (Position - Player.Position).Length() / 300f;

        T.FollowProperty(this, "global_position", GlobalPosition, Player, "global_position", Time,
            Tween.TransitionType.Quint, Tween.EaseType.In);
        T.InterpolateProperty(this, "scale", Scale, new Vector2(), Time,
            Tween.TransitionType.Expo, Tween.EaseType.In);

        T.Start();

        Player.ArrowExist = true;
        MovingBack = true;
        SetCollisionLayerBit(0, false);
    }

   public void ApplyGravity()
    {
        if(Fuel <= 0)
        {
            //TODO: Make Arrow fall down with gravity.
            LookAt(Player.Position);
        }
    }

    public void Dash()
    {
        if (ControllerMode)
        {
            JoyStickControl();
            GlobalRotation = Angle;
        }
        else
            MouseControl();

        IsControlled = false;
        Speed = 3;
        CanDash = false;
    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        
        this.QueueFree();
        Weapon.CanShoot = true;
    }
}



