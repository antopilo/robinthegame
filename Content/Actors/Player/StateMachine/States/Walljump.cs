using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Walljump : IState
{
    public string StateName => "Walljump";

    private const int WALL_JUMP_HEIGHT = 140;
    private const int WALL_JUMP_FORCE = 120;
    private const float WALLJUMP_DISABLETIME = 0.25f;

    private Vector2 m_velocity = new Vector2();
    private int m_direction = 0;

    // Timer
    private float InputDisableTimer = 0.25f;


    public void Enter(ref Player host)
    {
        m_velocity = host.Velocity;
        m_direction = host.WallDirection;

        // Start the walljump
        StartWalljump(ref host);
    }

    private void StartWalljump(ref Player host)
    {
        host.IsWallJumping = true;
        //CurrentMaxSpeed = MAX_AIR_SPEED;
        (host.GetNode("Timers/DisableInput") as Timer).Start();

        var CollisionCount = host.GetSlideCount() - 1;
        if (CollisionCount > -1)
        {
            var Collision = host.GetSlideCollision(CollisionCount);

            if (Collision.Normal == new Vector2(1, 0))
            {
                m_direction = 1;
                host.Sprite.FlipH = false;
            }
            else if (Collision.Normal == new Vector2(-1, 0))
            {
                m_direction = -1;
                host.Sprite.FlipH = true;
            }
        }
        else if (!host.IsOnFloor())
        {
            var RayLeft = host.GetNode("Raycasts/Left") as RayCast2D;
            var RayRight = host.GetNode("Raycasts/Right") as RayCast2D;
            if (RayLeft.IsColliding())
            {
                m_direction = 1;
                host.Sprite.FlipH = false;
            }
            else if (RayRight.IsColliding())
            {
                m_direction = -1;
                host.Sprite.FlipH = true;
            }
        }

        // Start timer
        InputDisableTimer = WALLJUMP_DISABLETIME;
        m_velocity.x = WALL_JUMP_FORCE * m_direction;
        m_velocity.y = -WALL_JUMP_HEIGHT;
    }

    public void Update(ref Player host, float delta)
    {
        // Apply gravity.
        ApplyGravity();

        // Update sprite
        UpdateSprite(ref host);

  
        // Move the player.
        host.MoveAndSlide(m_velocity, new Vector2(0, -1));

        // Update timer with real time.
        InputDisableTimer -= delta;

        // If the time is over, the player can now be controlled, 
        // while still in the air.
        if (InputDisableTimer <= 0)
            host.StateMachine.SetState("Air");
    }

    private void UpdateSprite(ref Player host)
    {
        if (m_velocity.y >= 0)
            host.Sprite.Play("Falling");
        else
            host.Sprite.Play("Jump");
    }


    private void ApplyGravity()
    {
        m_velocity.y += Air.GRAVITY;
    }
    

    public void Exit(ref Player host)
    {
        host.Velocity = m_velocity;
    }
}

