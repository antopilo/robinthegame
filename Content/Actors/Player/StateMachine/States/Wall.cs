using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Wall : IState
{
    public string StateName => "Wall";

    private Vector2 m_velocity = new Vector2();
    private int m_wallDirection = 0;
    private float wallFallMult = 0;

    public void Enter(ref Player host)
    {
        m_velocity = new Vector2(0,0);
        m_wallDirection = host.WallDirection;

        if (m_velocity.y > 0)
        {
            m_velocity.y /= 1.1f;
            host.Sprite.Play("Wall");
            
        }
        else
        {
            host.Sprite.Play("falling");
        }
    }


    public void Update(ref Player host, float delta)
    {
        ApplyGravity();

        host.Sprite.Play("Wall");
        host.Sprite.FlipH = m_wallDirection != -1;

        if (Input.IsActionPressed("Jump"))
            WallJump(ref host);

        host.MoveAndSlide(m_velocity, new Vector2(0, -1));
    }
    

    private void ApplyGravity()
    {
        m_velocity.y += Air.GRAVITY / 4;
    }


    private void WallJump(ref Player host)
    {
        var JumpDirection = 0;

        host.IsWallJumping = true;
        //CurrentMaxSpeed = MAX_AIR_SPEED;
        (host.GetNode("Timers/DisableInput") as Timer).Start();

        var CollisionCount = host.GetSlideCount() - 1;
        if (CollisionCount > -1)
        {
            var Collision = host.GetSlideCollision(CollisionCount);

            if (Collision.Normal == new Vector2(1, 0))
            {
                JumpDirection = 1;
                host.Sprite.FlipH = false;
            }
            else if (Collision.Normal == new Vector2(-1, 0))
            {
                JumpDirection = -1;
                host.Sprite.FlipH = true;
            }
        }
        else if (!host.IsOnFloor())
        {
            var RayLeft = host.GetNode("Raycasts/Left") as RayCast2D;
            var RayRight = host.GetNode("Raycasts/Right") as RayCast2D;
            if (RayLeft.IsColliding())
            {
                JumpDirection = 1;
                host.Sprite.FlipH = false;
            }
            else if (RayRight.IsColliding())
            {
                JumpDirection = -1;
                host.Sprite.FlipH = true;
            }
        }

        // Start timer
        //InputDisableTimer = WALLJUMP_DISABLETIME;
        //m_velocity.x = WALL_JUMP_FORCE * JumpDirection;
        //m_velocity.y = -WALL_JUMP_HEIGHT;
    }


    public void Exit(ref Player host)
    {
        host.Velocity = m_velocity;
    }
}

