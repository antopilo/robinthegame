using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Air : IState
{
    public string StateName => "Air";

    public const int GRAVITY = 4;
    private const int MAX_SPEED = 90;
    private const int MAX_AIR_SPEED = 100;
    private const int ACCELERATION = 8;
    private const int DECELERATION = 4;

    private Vector2 InputDirection = new Vector2();
    private Vector2 m_velocity;
    private bool wasJumping = false;

    private float DeltaTime = 0;


    public void Enter(ref Player host)
    {
        m_velocity = host.Velocity;

        if (host.WasOnGround)
            wasJumping = true;
    }


    public void Update(ref Player host, float delta)
    {
        // Gets the direction of the inputs.
        // Also update sprites with direction.
        GetInputDirection(ref host);

        // If pressing something, accelerate.
        m_velocity.x += InputDirection.x * ACCELERATION;

        // Implements a max speed.
        if (Mathf.Abs(m_velocity.x) > MAX_SPEED)
            m_velocity.x = MAX_SPEED * Mathf.Sign(m_velocity.x);

        // If not pressing anything, decelerate.
        if (InputDirection.x == 0)
            m_velocity.x -= DECELERATION * Mathf.Sign(m_velocity.x);

        // Snap the value to 0 if hes is still moving a little bit.
        // Make sure that the player actually stops. 
        if (Mathf.Abs(m_velocity.x) < 10 && InputDirection.x == 0)
            m_velocity.x = 0;
        
        CheckCollisions(ref host);

        UpdateSprite(ref host);

        // Sets velocity for state transition.
        host.Velocity = m_velocity;

        // Move the player
        host.MoveAndSlide(m_velocity, new Vector2(0, -1));

        // If is on the ground, change state.
        if (host.IsOnFloor())
            host.StateMachine.SetState("Moving");

        ApplyGravity(ref host);
    }


    private void CheckCollisions(ref Player host)
    {
        var CollisionCount = host.GetSlideCount() - 1;
        if (CollisionCount <= -1)
            return;

        var Collision = host.GetSlideCollision(CollisionCount);
        var NormalGround = new Vector2(0, -1);
        var NormalLeft = new Vector2(1, 0);
        var NormalRight = new Vector2(-1, 0);
        var NormalCeiling = new Vector2(0, 1);

        if ((Collision.Normal == NormalLeft || Collision.Normal == NormalRight))
            host.StateMachine.SetState("Wall");
            

        // TODO: Add ledge grabbing, SEE Player.cs Line: 302.

        if (Collision.Normal == NormalCeiling)
            m_velocity.y = -m_velocity.y / 4;
    }


    private void GetInputDirection(ref Player host)
    {
        int inputX, inputY = 0;

        if (Input.IsActionPressed("Left"))  // Going left.
        {
            inputX = -1;
            host.LastDirectionX = -1;

            host.Sprite.Play("Running");
            host.Sprite.FlipH = true;
        }
        else if (Input.IsActionPressed("Right")) // Going right
        {
            inputX = 1;
            host.LastDirectionX = 1;

            host.Sprite.Play("Running");
            host.Sprite.FlipH = false;
        }
        else // Not moving...
        {
            inputX = 0;

            // Flip sprite towards last direction.
            if (host.LastDirectionX == 1)
                host.Sprite.FlipH = false;
            else
                host.Sprite.FlipH = true;

            // TODO: Make player run slower until it reach 0 velocity.
            //       Must change animation speed gradually.

            host.Sprite.Play("Idle");
        }

        if(Input.IsActionJustPressed("Jump") && CanWalljump(ref host))
        {
            host.StateMachine.SetState("Walljump");
        }

        if (Input.IsActionJustReleased("Jump") && m_velocity.y < 0 && host.WasOnGround)
        {
            if (wasJumping)
            {
                wasJumping = true;
                m_velocity.y /= 1.5f;
            }
        }
           

        InputDirection = new Vector2(inputX, inputY);
    }

    private bool CanWalljump(ref Player host)
    {
        var Left = (RayCast2D)host.GetNode("Raycasts/Left");
        var Right = (RayCast2D)host.GetNode("Raycasts/Right");
        return (Left.IsColliding() || Right.IsColliding());
    }


    private void WallJump(ref Player host)
    {
        var JumpDirection = 0;

       
    }


    private void ApplyGravity(ref Player host)
    {
        var scale = 1 - ((Mathf.Abs(m_velocity.y) / MAX_AIR_SPEED) / 4);
       // host.Sprite.Scale = new Vector2(Mathf.Clamp(scale, 0.75f, 1), 1);

        m_velocity.y += GRAVITY;
    }

    private void UpdateSprite(ref Player host)
    {
        if (m_velocity.y >= 0)
            host.Sprite.Play("Falling");
        else
            host.Sprite.Play("Jump");
    }

    public void Exit(ref Player host)
    {
       
        var CollisionCount = host.GetSlideCount() - 1;
        if (CollisionCount <= -1)
            return;

        var Collision = host.GetSlideCollision(CollisionCount);
        var NormalGround = new Vector2(0, -1);
        var NormalLeft = new Vector2(1, 0);
        var NormalRight = new Vector2(-1, 0);
        var NormalCeiling = new Vector2(0, 1);

        host.WallDirection = Collision.Normal == NormalLeft ? 1 : 0;
    }
}

