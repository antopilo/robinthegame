using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

class JumpPadded : IState
{
    public string StateName { get; } = "JumpPad";

    public const int JUMP_PAD_FORCE = 260;

    private Vector2 m_velocity = new Vector2();
    private Vector2 m_direction = new Vector2();

    // Timer
    private float InputDisableTimer = 0f;
    private const float DISABLE_TIME = 0.25f;
    public void Enter(ref Player host)
    {
        m_velocity = host.Velocity;
        m_direction = host.JumpPadDirection;

        if (m_direction.x != 0)
        {
            InputDisableTimer = DISABLE_TIME;
            m_velocity.x = Mathf.Sign(m_direction.x) * JUMP_PAD_FORCE;
            m_velocity.y = -JUMP_PAD_FORCE / 1.5f;

        }
        else if (m_direction.y != 0)
        {
            m_velocity.y = Mathf.Sign(m_direction.y) * JUMP_PAD_FORCE;
        }

        host.WasOnGround = false;
    }

    public void Exit(ref Player host)
    {
        host.Velocity = m_velocity;
    }

    public void Update(ref Player host, float delta)
    {
        host.MoveAndSlide(m_velocity);

        InputDisableTimer -= delta;

        if (InputDisableTimer <= 0)
            host.StateMachine.SetState("Air");
    }

}

