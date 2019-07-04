using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
    public CollisionShape2D CollisionBox { get; private set; }
    public AnimatedSprite Sprite { get; private set; }
    public Camera Camera { get; private set; }
    public Arrow Arrow { get; set; }
    private Particles2D RunDust;

    public StateMachine StateMachine;
    public int WallDirection = 1;

    public Vector2 FeetPosition => GlobalPosition + new Vector2(0, 4);
    public Vector2 Inputv => new Vector2(InputDirectionX, InputDirectionY);

    public Weapon Weapon;
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
    public float GravityMult = 1f;
    public Vector2 Velocity = new Vector2();

    public bool CanControl = true;
    public int LastDirectionX = 0;
    private int InputDirectionX = 0;
    private int InputDirectionY = 0; // For later use maybe.

    public States State { get; set; }

    public bool Alive = true;
    public bool IsCrouching { get; set; } = false;
    public bool IsWallJumping { get; set; } = false;
    public bool WasOnGround { get; set; } = false;
    public bool CanWallJump { get; private set; } = false;
    public bool CanJump { get; private set; } = false;
    public bool ArrowExist { get; set; } = false;
    public bool IsCeilling { get; private set; } = false;
    public Particles2D RunDust1, WallJumpDust;

    private float DeltaTime = 0;

    //SFX
    [Export] float FootStepRate = 0f;
    float FootStepTimer = 0;

    public bool CanInteract = false;
    public List<Node2D> Following = new List<Node2D>(); // List of following entities.
    public List<Node2D> InteractableObject = new List<Node2D>(99); // List of objects rdy to interact close-by

    // Init.
    public override void _Ready()
    {
        Weapon = GetNode("Weapon") as Weapon;

        CollisionBox = (CollisionShape2D)GetNode("Collision");
        Sprite = (AnimatedSprite)GetNode("AnimatedSprite");
        Camera = (Camera)GetNode("Camera2D");
        RunDust = (Particles2D)GetNode("Particles/RunDust");
        WallJumpDust = (Particles2D)GetNode("Particles/WallJump");

        StateMachine = new StateMachine(this);

        StateMachine.AddState(new Idle());
        StateMachine.AddState(new Moving());
        StateMachine.AddState(new Wall());
        StateMachine.AddState(new Air());

        StateMachine.SetState("Moving");
    }

    // Jumping.
    public override void _Input(InputEvent e)
    {
        if (Root.Console.Visible)
            return;

        //if (e.IsActionPressed("jump") && CanControl)
        //{
            
        //    if (State == States.Ground || (DeltaTime <= NextJumpTime && CanJump))
        //    {
        //        CanJump = false;
        //        WasOnGround = true;

        //        // Jump sound.
        //        //(GetNode("SFX/Jump") as AudioStreamPlayer).Play(0);

        //        if (IsCrouching)
        //            SuperJump();
        //        else
        //            Jump();
        //    }
        //    else if(State == States.Ledge)
        //    {
        //        if(!Sprite.FlipH && InputDirectionX == 1)
        //        {
        //            GD.Print("LEDGE JUMP");
        //            EnterAirState();
        //            Jump(1);
        //        }
        //        else if(Sprite.FlipH && InputDirectionX == -1)
        //        {
        //            GD.Print("LEDGE JUMP");
        //            EnterAirState();
        //            Jump(1);
        //        }
        //        else
        //            WallJump();
        //    }
        //    else if (CanWallJump)
        //    {
        //        WasOnGround = false;
        //        WallJump();
        //        WallJumpDust.Emitting = true;
        //    }
        //}
          

        //if (e.IsActionReleased("jump") && Velocity.y < 0 && WasOnGround) // Tap jump
        //    Velocity.y /= 1.5f;
    }

    public void ResetInput()
    {
        Velocity = new Vector2();
        Sprite.Animation = "idle";
        InputDirectionX = 0;
        InputDirectionY = 0;
    }

    // Main loop.
    public override void _PhysicsProcess(float delta)
    {
        CollisionBox.Disabled = State == States.Ghost;
        Modulate = State == States.Ghost ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1); 
        GetReference();
        //GetInput();
        //UpdateVelocity();

        StateMachine.Update(delta);

        if(State != States.Ghost)
        {
            //UpdateState();
            //CanWalljump();
        }
            
        //SpeedLimits();
        Sounds(delta); // Handles player sounds
        Particles(); // Handles player particles

        //Velocity += GetFloorVelocity();
        
        //MoveAndSlide(Velocity);

        //ApplyGravity();
        GetArrow();
        GetInteractable();

        DeltaTime += delta;
        FootStepTimer += delta;
    }

    private void GetReference()
    {
        if(Camera is null) 
            Camera = GetNode("Camera2D") as Camera;
    }

    public void GhostMode()
    {
        CollisionBox.Disabled = true;
    }

    /// <summary>
    /// Update the direction of the player with the input.
    /// Must be called every frame.
    /// Also plays the animation.
    /// </summary>
    private void GetInput()
    {
        // Input disabling
        if (!CanControl || !Alive || Root.Console.Visible)
        {
            InputDirectionX = 0;
            InputDirectionY = 0;
            return;
        }

        // Horizontal Inputs
        if (Godot.Input.IsActionPressed("Left"))
        {
            if(State != States.Wall && State != States.Ledge)
            {
                if(Mathf.Abs(Velocity.x) < MAX_SPEED / 2)
                    Sprite.Play("Face");
                else
                    Sprite.Play("Running");
                
                Sprite.FlipH = true;
            }
            InputDirectionX = -1;
            LastDirectionX = -1;
        }
        else if (Godot.Input.IsActionPressed("Right"))
        {
            if(State != States.Wall && State != States.Ledge)
            {
               if(Mathf.Abs(Velocity.x) < MAX_SPEED / 2)
                    Sprite.Play("Face");
                else
                    Sprite.Play("Running");
                Sprite.FlipH = false;
            }
                
            InputDirectionX = 1;
            LastDirectionX = 1;
        }
        else
        {
            InputDirectionX = 0;

            if (State == States.Ground)
                Sprite.Play("idle");
        }

        // Looking up
        if (Godot.Input.IsActionPressed("Up"))
        {
            InputDirectionY = -1;
        }
        else if (Godot.Input.IsActionPressed("Down"))
        {
            InputDirectionY = 1;

            if (InputDirectionX == 0 && State == States.Ground)
            {
                IsCrouching = true;
                Sprite.Play("Crouch");
            }
                
        }
        else
        {
            InputDirectionY = 0;
            IsCrouching = false;
        }
    }

    private void Particles()
    {
        WallJumpDust.Emitting = IsWallJumping;
        RunDust.Emitting = InputDirectionX != 0 && State == States.Ground;
    }

    private void Sounds(float delta)
    {
        if (InputDirectionX != 0 && State == States.Ground && FootStepTimer >= 0.125)
        {
            (GetNode("SFX/Footstep") as AudioStreamPlayer).Play(0);
            FootStepTimer = 0;
        }
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
        var NormalLeft = new Vector2(1, 0);
        var NormalRight = new Vector2(-1, 0);
        var NormalCeiling = new Vector2(0, 1);

        if (Collision.Normal == NormalGround && State != States.Ground)
            EnterGroundState();
        if ((Collision.Normal == NormalLeft || Collision.Normal == NormalRight) && State != States.Ledge)
            EnterWallState(Collision.Normal == NormalLeft ? 1 : 0);
        if (CanWallJump && CanGrabLedge())
            EnterLedge();
        if (Collision.Normal == NormalCeiling)
            Velocity.y = -Velocity.y / 4;

    }

    public bool CanGrabLedge()
    {
        if(!CanControl)
            return false;

        var castRight = (RayCast2D)GetNode("Raycasts/LedgeRight");
        var point = castRight.GetCollisionPoint() - Root.GameController.CurrentRoom.LevelPosition;
        var tmPos = Root.GameController.CurrentRoom.LayerSolid.WorldToMap(point);
        var tile = Root.GameController.CurrentRoom.LayerSolid.GetCellv(tmPos - new Vector2(0, 1));
        if(castRight.IsColliding() && tile == -1)
            return true;
        return false;
    }

    public void EnterLedge()
    {
        if (Velocity.y < 0)
            return;
        Sprite.Play("Ledge");
        Velocity.y = 0;
        GravityMult = 0;
        Velocity.x = 0;
        State = States.Ledge;
        Sprite.FlipH = Sprite.FlipH;
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


    public void EnterWallState(int direction) // Wall State
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
            Sprite.FlipH = Convert.ToBoolean(direction);
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

        if (Velocity.y >= 0)
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
    {
        
        Velocity.x += InputDirectionX * ACCELERATION;
        if (State == States.Ghost)
            Velocity.y += InputDirectionY * ACCELERATION;
    }
    /// <summary>
    /// Apply Constant force on the player. if the player is on the ground. Stop the gravity.
    /// </summary>
    private void ApplyGravity()
    {
        if (State == States.Ground || State == States.Ghost)
            GravityMult = 0;
        else
            GravityMult = 1;


        var scale = 1 - ((Mathf.Abs(Velocity.y) / MAX_AIR_SPEED) / 4);
        Sprite.Scale = new Vector2(Mathf.Clamp(scale, 0.75f, 1), 1);

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

        if (Mathf.Abs(Velocity.y) > MAX_FALL_SPEED && State != States.Ghost)
            Velocity.y = MAX_FALL_SPEED * Mathf.Sign(Velocity.y);
        if (State == States.Ghost && Mathf.Abs(Velocity.y) > CurrentMaxSpeed)
            Velocity.y = MAX_SPEED * Mathf.Sign(Velocity.y);
        if (InputDirectionX == 0 && CanControl)
            Velocity.x -= DECELERATION * Mathf.Sign(Velocity.x);

        // Ghost mode
        if (State == States.Ghost && InputDirectionY == 0 && CanControl)
            Velocity.y -= DECELERATION * Mathf.Sign(Velocity.y);

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
    public void Jump(float modifier = 1f)
    {   var CollisionCount = GetSlideCount() - 1;
        KinematicCollision2D col = null;
        if (CollisionCount > -1)
        {
            col = GetSlideCollision(CollisionCount);
        }
        if(col != null && col.GetCollider() is Arrow)
            (col.GetCollider() as Arrow).Jiggle();
        CurrentMaxSpeed = MAX_SPEED;
        Velocity.y = -JUMP_FORCE * modifier;
    }

    /// <summary>
    /// Crouch jump
    /// </summary>
    public void SuperJump()
    {
        var CollisionCount = GetSlideCount() - 1;
        KinematicCollision2D col = null;
        if (CollisionCount > -1)
        {
            col = GetSlideCollision(CollisionCount);
        }
        if(col != null && col.GetCollider() is Arrow)
            (col.GetCollider() as Arrow).Jiggle();
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
        Velocity = Vector2.Zero;
        Camera.Shake(3f, 0.05f);
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

    #region Interaction

    // This gets the list of all objects with the 
    private void GetInteractable()
	{
        CanInteract = InteractableObject.Count > 0;
        // Ordering them
        if (CanInteract)
        {
            var closest = InteractableObject[0];
            var closestDistance = (closest.GlobalPosition - GlobalPosition).Length(); // Distance du joueur
            for (int i = 0; i < InteractableObject.Count; i++)
            {
                var currentDistance = (InteractableObject[i].GlobalPosition - this.GlobalPosition).Length();
                if (currentDistance < closestDistance) // Compare si il y en a un plus proche que le present
                    closest = InteractableObject[i] as Node2D;
            }

            // Swaping the closest with the first of the list
            var temp = InteractableObject[0];
            var idx = InteractableObject.IndexOf(closest);
            InteractableObject[0] = closest;
            InteractableObject[idx] = temp;
        }

        // Interacts with the object
        if(Input.IsActionJustPressed("Interact") && CanInteract)
		{
			if( (InteractableObject[0] as Node2D).HasMethod("Interact"))
            {
                try{
			        InteractableObject[0].Call("Interact");
                }catch{}
            }
		}
    } 

    public void RemoveFromInteraction(Node2D pObject){
        if(InteractableObject.Contains(pObject))
            InteractableObject.Remove(pObject);
    }

    // Detection of interactable objects
    private void _on_InteractionRange_area_entered(object area)
    {
        var parent = (area as Area2D).GetParent() as Node2D;
        if(parent.HasMethod("Interact")){
            if((parent.Get("CanInteract") as bool?) == false)
                return;
            InteractableObject.Insert(0, ((area as Area2D).GetParent() as Node2D));
        }
    }

    private void _on_InteractionRange_area_exited(object area)
    {
        var parent = (area as Area2D).GetParent() as Node2D;
        if(parent.HasMethod("Interact")){
            InteractableObject.Remove((area as Area2D).GetParent() as Node2D);
        }
    } 
    #endregion
    private void _on_DisableInput_timeout()
    {
        CanControl = true;
        IsWallJumping = false;
    }
}
