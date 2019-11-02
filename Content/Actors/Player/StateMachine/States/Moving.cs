using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Moving : IState
{
    public string StateName { get; } = "Moving";


    public const int JUMP_FORCE = 190;
    private const int MAX_SPEED = 90;
    public const int ACCELERATION = 8;
    public const int DECELERATION = 4;

    private Vector2 InputDirection = new Vector2();
    private Vector2 m_velocity = new Vector2();


    public void Enter(ref Player host)
    {
        m_velocity = new Vector2(host.Velocity.x, host.Velocity.y);
        m_velocity.y = 3;
    }

    
    public void Update(ref Player host, float delta)
    {
        // Gets the direction of the inputs.
        // Also update sprites with direction.
        GetInputDirection(ref host);

        // Jumping
        if (Input.IsActionJustPressed("Jump"))
            Jump(ref host);

        // If pressing something, accelerate.
        m_velocity.x += InputDirection.x * ACCELERATION;

        // Implements a max speed.
        if (Mathf.Abs(m_velocity.x) > MAX_SPEED)
            m_velocity.x = MAX_SPEED * Mathf.Sign(m_velocity.x);

        // If not pressing anything, decelerate.
        if (InputDirection.x == 0)
            m_velocity.x -= DECELERATION * Mathf.Sign(m_velocity.x);

        if((LeftWallCheck(ref host)  & InputDirection.x == -1) ||
           (RightWallCheck(ref host) & InputDirection.x ==  1))
            m_velocity.x = 0;

        // Snap the value to 0 if hes is still moving a little bit.
        // Make sure that the player actually stops. 
        if (Mathf.Abs(m_velocity.x) < 10 && InputDirection.x == 0)
            m_velocity.x = 0;

        // If the player is idle...
        if (m_velocity.x == 0 && InputDirection == new Vector2())
            host.StateMachine.SetState("Idle");

        // Sets velocity for state transition.
        host.Velocity = m_velocity;


        DebugPrinter.AddDebugItem("Velocity", m_velocity.ToString());

        // If is on the ground, change state.
        if (!host.IsOnFloor())
        {
            host.StateMachine.SetState("Air");
        }
        // Move the player.
        host.MoveAndSlide(m_velocity, new Vector2(0, -1));

        host.RunDust.Emitting = InputDirection.x != 0;

        
    }

    private bool LeftWallCheck(ref Player host)
    {
        return ((RayCast2D)host.GetNode("Raycasts/Left")).IsColliding();
    }

    private bool RightWallCheck(ref Player host)
    {
        return ((RayCast2D)host.GetNode("Raycasts/Right")).IsColliding();
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

        InputDirection = new Vector2(inputX, inputY);
    }

    private void Jump(ref Player host)
    {
        var CollisionCount = host.GetSlideCount() - 1;
        KinematicCollision2D col = null;
        if (CollisionCount > -1)
        {
            col = host.GetSlideCollision(CollisionCount);
        }
        if (col != null && col.GetCollider() is Arrow)
            (col.GetCollider() as Arrow).Jiggle();

        host.WasOnGround = true;

        m_velocity.y = -JUMP_FORCE;
    }

    public void Exit(ref Player host)
    {
        host.RunDust.Emitting = false;
    }

    public bool CheckWalls(ref Player host)
    {
        var CollisionCount = host.GetSlideCount() - 1;
        KinematicCollision2D col = null;
        if (CollisionCount > -1)
        {
            for(int i = 0; i < CollisionCount; i++)
            {
                col = host.GetSlideCollision(i);

                // hit a wall
                if(col.Normal == new Vector2(1, 0) || col.Normal == new Vector2(-1, 0))
                    return true;
            }

        }
        return false;
    }
}

