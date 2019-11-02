using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
	public CollisionShape2D CollisionBox { get; private set; }
	public AnimatedSprite Sprite { get; private set; }
	public Camera Camera { get; private set; }
	public Arrow Arrow { get; set; }
	public Particles2D RunDust;

	public float InputStrength = 1.0f;

	public StateMachine StateMachine;
	public WallDirection WallDirection = 0;

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
	public const int JUMP_PAD_FORCE = 260;
	private const int MAX_SPEED = 90;
	private const int MAX_AIR_SPEED = 100;
	private const int MAX_FALL_SPEED = 300;

	public Vector2 JumpPadDirection = new Vector2();


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
		StateMachine.AddState(new Walljump());
		StateMachine.AddState(new JumpPadded());
		StateMachine.AddState(new Air());
		StateMachine.AddState(new Ghost());
		StateMachine.AddState(new Sit());
		StateMachine.AddState(new ArrowBoostGrace());
		StateMachine.AddState(new ArrowBoosted());
		StateMachine.AddState(new PoleGrace());
		// StateMachine.AddState(new PoleBoost());

		StateMachine.SetState("Moving");
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
		GetReference();

		StateMachine.Update(delta);

		Sounds(delta); // Handles player sounds
		Particles(); // Handles player particles
		GetArrow();
		GetInteractable();

		DebugPrinter.AddDebugItem("State", StateMachine.CurrentState.StateName);
		DebugPrinter.AddDebugItem("Position", new Vector2((int)Position.x,(int)Position.y).ToString());
		DeltaTime += delta;
		FootStepTimer += delta;
	}

	private void GetReference()
	{
		if(Camera is null) 
			Camera = GetNode("Camera2D") as Camera;
	}


	private void Particles()
	{
		WallJumpDust.Emitting = IsWallJumping;
		
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
	#endregion


	/// <summary>
	/// Normal jump
	/// </summary>
	public void Jump(float modifier = 1f)
	{   var CollisionCount = GetSlideCount() - 1;
		KinematicCollision2D col;   

		if (CollisionCount > -1)
		{
			col = GetSlideCollision(CollisionCount);

            if (col.Collider is Arrow)
                ((Arrow)col.Collider).Jiggle();
        }
		

		CurrentMaxSpeed = MAX_SPEED;
		Velocity.y = -JUMP_FORCE * modifier;
	}

	/// <summary>
	/// Crouch jump
	/// </summary>
	public void SuperJump()
	{
        // Check if there is a colision
		var CollisionCount = GetSlideCount() - 1;

		KinematicCollision2D col;

		if (CollisionCount > -1)
        {
            col = GetSlideCollision(CollisionCount);

            if (col.Collider is Arrow)
                ((Arrow)col.Collider).Jiggle();
        }

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
		this.JumpPadDirection = pDirection;
		StateMachine.SetState("JumpPad");
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
		if (GetParent().HasNode("arrow"))
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

	public void AddToInteraction(Node2D pObject)
	{
		if (InteractableObject.Contains(pObject))
			return;
		InteractableObject.Add(pObject);
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
            InteractableObject.Insert(0, parent);
        }
    }

    private void _on_InteractionRange_area_exited(object area)
    {
        var parent = (area as Area2D).GetParent() as Node2D;
        if(parent.HasMethod("Interact"))
            InteractableObject.Remove(parent);
    } 

	// Detection of interactable o
	#endregion

	private void _on_DisableInput_timeout()
    {
        CanControl = true;
        IsWallJumping = false;
    }
	
}
