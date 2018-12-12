using Godot;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
    public CollisionPolygon2D CollisionBox { get; private set; }
    public AnimatedSprite Sprite { get; private set; }
    public Camera2D Camera { get; private set; }
    public Arrow Arrow { get; set; }

    private const int GRAVITY = 4;
    private const int ACCELERATION = 5;
    private const int DECELERATION = 4;
    private const float JUMP_TIMING_TOLERANCE = 0.1f;
    private const int JUMP_FORCE = 190;
    private const int SUPER_JUMP_FORCE = 220;
    private const int WALL_JUMP_HEIGHT = 140;
    private const int WALL_JUMP_FORCE = 120;
    private const int JUMP_PAD_FORCE = 260;
    private const int MAX_SPEED = 90;
    private const int MAX_AIR_SPEED = 100;
    private const int MAX_FALL_SPEED = 300;

    private float NextJumpTime = 0f;
    private float CurrentMaxSpeed = MAX_SPEED;
    private float GravityMult = 1f;
    public Vector2 Velocity = new Vector2();

    public bool CanControl = true;
    public int LastDirectionX = 0;
    private int InputDirectionX = 0;
    private int InputDirectionY = 0; // For later use maybe.
    
    public States State { get; private set; }

    public bool Alive = true;
    public bool IsCrouching { get; private set; } = false;
    public bool IsWallJumping { get; private set; } = false;
    public bool WasOnGround { get; private set; } = false;
    public bool CanWallJump { get; private set; } = false;
    public bool CanJump { get; private set; } = false;
    public bool ArrowExist { get; set; } = false;
    public bool IsCeilling { get; private set; } = false;

    private float DeltaTime = 0;

    public List<Node2D> Following = new List<Node2D>(); // List of following entities.

    public override void _Ready()
    {
        base._Ready();

        CollisionBox = (CollisionPolygon2D)GetNode("Collision");
        Sprite = (AnimatedSprite)GetNode("AnimatedSprite");
        Camera = (Camera2D)GetNode("Camera2D");
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("jump") && CanControl)
            if (State == States.Ground || DeltaTime <= NextJumpTime && CanJump)
            {
                CanJump = false;
                WasOnGround = true;
                
                if (IsCrouching)
                    SuperJump();
                else
                    Jump();
            }
            else if (CanWallJump)
            {
                WasOnGround = false;
                WallJump();
            }

        if (e.IsActionReleased("jump") && Velocity.y < 0 && WasOnGround) // Tap jump
            Velocity.y /= 1.75f;
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput();
        UpdateVelocity();
        UpdateState();
        CanWalljump();
		SpeedLimits();
        Sounds();
        MoveAndSlide(Velocity);
        ApplyGravity();
        GetArrow();

        DeltaTime += delta;
    }


    /// <summary>
    /// Update the direction of the player with the input.
    /// Must be called every frame.
    /// Also plays the animation.
    /// </summary>
    private void GetInput()
    {
        if (!CanControl)
        {
            InputDirectionX = 0;
            InputDirectionY = 0;
            return;
        }

        if (Input.IsActionPressed("ui_left"))
        {
            Sprite.Play("Running");
            Sprite.FlipH = true;
            InputDirectionX = -1;
            LastDirectionX = -1;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            Sprite.Play("Running");
            Sprite.FlipH = false;
            InputDirectionX = 1;
            LastDirectionX = 1;
        }
        else
        {
            InputDirectionX = 0;

            if (State == States.Ground)
                Sprite.Play("idle");
        }

        if (Input.IsActionPressed("ui_up"))
            InputDirectionY = -1;
        else
            InputDirectionY = 0;

        if (Input.IsActionPressed("ui_down") && State == States.Ground)
        {
            IsCrouching = true;
            InputDirectionY = 1;

            if (InputDirectionX == 0)
                Sprite.Play("Crouch");
        }
        else
        {
            InputDirectionY = 0;
            IsCrouching = false;
        }
    }

    
    private void Sounds()
    {
        if (InputDirectionX != 0 && State == States.Ground)
            (GetNode("SFX/Footstep") as AudioStreamPlayer).Playing = true;
        else
            (GetNode("SFX/Footstep") as AudioStreamPlayer).Playing = false;

    }

    #region States
    /// <summary>
    /// Decide what state is the player in. See States.cs for all of the possible states.
    /// </summary>
    private void UpdateState()
    {
        var CollisionCount = GetSlideCount() - 1;

        if (CollisionCount <= -1)
        {
            EnterAirState();
            return;
        }

        var Collision = GetSlideCollision(CollisionCount);
        var NormalGround = new Vector2(0, -1);

        if (Collision.Normal == NormalGround && State != States.Ground)
            EnterGroundState();

        var NormalLeft = new Vector2(1, 0);
        var NormalRight = new Vector2(-1, 0);

        if (Collision.Normal == NormalLeft || Collision.Normal == NormalRight)
            EnterWallState();

        var Ceiling = new Vector2(0, 1);

        if (Collision.Normal == Ceiling)
            Velocity.y = 0;
    }

    public void EnterGroundState() // Ground state.
    {
        if (State == States.Air)
            Velocity.y = 3;

        State = States.Ground;
        CurrentMaxSpeed = MAX_SPEED;
        CanControl = true;
        CanJump = true;
        IsCeilling = false;
    }

    public void EnterWallState() // Wall State
    {
        State = States.Wall;
        GravityMult = 1;

        if (State == States.Ground)
            CanWallJump = true;

        IsCeilling = false;

        if (Velocity.y > 0)
        {
            Velocity.y /= 1.1f;
            Sprite.Play("Wall");
        }
        else
        {
            Sprite.Play("falling");
        }
    }

    public void EnterAirState() // Air State.
    {
        if (State != States.Air)
            NextJumpTime = DeltaTime + JUMP_TIMING_TOLERANCE;

        if (Velocity.y > 0)
            Sprite.Play("falling");
        else
            Sprite.Play("Jump");

        State = States.Air;
    }

    #endregion

    #region Physics
    /// <summary>
    /// Pretty much just adds the acceleration. 
    /// </summary>
    private void UpdateVelocity()
        => Velocity.x += InputDirectionX * ACCELERATION;
    /// <summary>
    /// Apply Constant force on the player. if the player is on the ground. Stop the gravity.
    /// </summary>
    private void ApplyGravity()
    {
        if (State == States.Ground)
            GravityMult = 0;
        else
            GravityMult = 1;

        Velocity.y += GRAVITY * GravityMult;
    }


    /// <summary>
    /// Define speed limits.
    /// </summary>
    private void SpeedLimits()
    {
        if (IsWallJumping && Mathf.Abs(Velocity.x) > MAX_AIR_SPEED)
            Velocity.x = MAX_AIR_SPEED * Mathf.Sign(Velocity.x);

        if (Mathf.Abs(Velocity.x) > CurrentMaxSpeed)
            Velocity.x = MAX_SPEED * Mathf.Sign(Velocity.x);

        if (Mathf.Abs(Velocity.y) > MAX_FALL_SPEED)
            Velocity.y = MAX_FALL_SPEED * Mathf.Sign(Velocity.y);

        if (InputDirectionX == 0 && CanControl)
            Velocity.x -= DECELERATION * Mathf.Sign(Velocity.x);

        if (Mathf.Abs(Velocity.x) < 10 && InputDirectionX == 0)
            Velocity.x = 0;
    }
    #endregion

    #region Abilities
    /// <summary>
    /// Decide if the player can Wall jump or not.
    /// </summary>
    private void CanWalljump()
    {
        var Left = (RayCast2D)GetNode("Raycasts/Left");
        var Right = (RayCast2D)GetNode("Raycasts/Right");

        CanWallJump = (Left.IsColliding() || Right.IsColliding()) || State == States.Wall;
    }


    /// <summary>
    /// Normal jump
    /// </summary>
    public void Jump()
    {
        CurrentMaxSpeed = MAX_SPEED;
        Velocity.y = -JUMP_FORCE;
    }

    /// <summary>
    /// Crouch jump
    /// </summary>
    public void SuperJump()
    {
        CurrentMaxSpeed = MAX_SPEED;
        Velocity.y = -SUPER_JUMP_FORCE;
    }


    /// <summary>
    /// Wall jump
    /// </summary>
    public void WallJump()
    {
        var JumpDirection = 0;

        IsWallJumping = true;
        CanControl = false;
        CurrentMaxSpeed = MAX_AIR_SPEED;
        (GetNode("Timers/DisableInput") as Timer).Start();

        var CollisionCount = GetSlideCount() - 1;

        if (CollisionCount > -1)
        {
            var Collision = GetSlideCollision(CollisionCount);

            if (Collision.Normal == new Vector2(1, 0))
            {
                JumpDirection = 1;
                Sprite.FlipH = false;
            }
            else if (Collision.Normal == new Vector2(-1, 0))
            {
                JumpDirection = -1;
                Sprite.FlipH = true;
            }
        }
        else if (State != States.Ground)
        {
            var RayLeft = GetNode("Raycasts/Left") as RayCast2D;
            var RayRight = GetNode("Raycasts/Right") as RayCast2D;

            if (RayLeft.IsColliding())
            {
                JumpDirection = 1;
                Sprite.FlipH = false;
            }
            else if (RayRight.IsColliding())
            {
                JumpDirection = -1;
                Sprite.FlipH = true;
            }
        }

        Velocity.x = WALL_JUMP_FORCE * JumpDirection;
        Velocity.y = -WALL_JUMP_HEIGHT;
    }


    /// <summary>
    /// Boost the player in a specified direction.
    /// TODO: Improve and define forces better.
    /// </summary>
    /// <param name="pDirection"></param>
    public void JumpPad(Vector2 pDirection)
    {
        if (pDirection.x != 0)
        {
            Velocity.x = Mathf.Sign(pDirection.x) * JUMP_PAD_FORCE * 5;
            Velocity.y = -JUMP_PAD_FORCE / 1.5f;

            IsWallJumping = true;
            CurrentMaxSpeed = MAX_AIR_SPEED;
            CanControl = false;
            (GetNode("Timers/DisableInput") as Timer).Start();

        }
        else if (pDirection.y != 0)
        {
            Velocity.y = Mathf.Sign(pDirection.y) * JUMP_PAD_FORCE;
        }

        WasOnGround = false;
    }

    public void Spawn(bool WithAnimation)
    {
        (GetParent() as GameController).Spawn(WithAnimation);
        Velocity = new Vector2();
    }


    /// <summary>
    /// Check every frame if there is an arrow and get it as a reference.
    /// </summary>
    public void GetArrow()
    {
        if (HasNode("arrow"))
        {
            ArrowExist = true;
            Arrow = GetNode("../arrow") as Arrow;
        }
        else
        {
            ArrowExist = false;
            Arrow = null;
        }
    } 
    #endregion

    private void _on_DisableInput_timeout()
    {
        CanControl = true;
        IsWallJumping = false;
    }
}




