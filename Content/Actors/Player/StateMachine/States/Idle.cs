using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Idle : IState
{
    public string StateName { get; } = "Idle";
    public StateMachine StateMachine { get; set; }

    private const int SUPER_JUMP_FORCE = 220;
    private Vector2 m_velocity = new Vector2();


    public void Enter(ref Player host)
    {
        m_velocity = new Vector2(0, 0);
    }


    public void Update(ref Player host, float delta)
    {
        if(GetInputDirection(ref host).x != 0)
            host.StateMachine.SetState("Moving");

        // Check for jump
        if (Input.IsActionJustPressed("Jump"))
        {
            if (host.IsCrouching)
                SuperJump(ref host);
            else
                Jump(ref host);
        }

        host.MoveAndSlide(m_velocity, new Vector2(0, -1));

        if (!host.IsOnFloor())
            host.StateMachine.SetState("Air");
    }


    private Vector2 GetInputDirection(ref Player host)
    {
        int dirX = 0, dirY = 0;

        if (Input.IsActionPressed("Left"))
        {
            dirX = -1;
        }
        else if (Input.IsActionPressed("Right"))
        {
            dirX = 1;
        }
        else          
        {
            dirX = 0;
        }

        if (Input.IsActionPressed("Down"))
        {
            dirY = 1;

            host.IsCrouching = true;
            host.Sprite.Play("Crouch");
        }
        else
        {
            dirY = 0;
            host.IsCrouching = false;
            host.Sprite.Play("Idle");
        }

        var inputDirection = new Vector2(dirX, dirY);

        return inputDirection;
    }


    public void Jump(ref Player host)
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

        m_velocity.y = -Moving.JUMP_FORCE;
    }

    public void SuperJump(ref Player host)
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

        m_velocity.y = -SUPER_JUMP_FORCE;
    }

    public void Exit(ref Player host)
    {
        host.Velocity = m_velocity;
    }


}

