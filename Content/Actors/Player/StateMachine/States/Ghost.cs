using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Ghost : IState
{
    public string StateName { get; } = "Ghost";

    private Vector2 m_velocity = new Vector2();
    private Vector2 m_inputDirection = new Vector2();

    public void Enter(ref Player host)
    {
        host.CollisionBox.Disabled = true;
        host.Modulate = new Color(1, 1, 1, 0.5f);
    }

    private void GetInput()
    {
        if (Input.IsActionPressed("Up"))
            m_inputDirection.y = -1;
        else if (Input.IsActionPressed("Down"))
            m_inputDirection.y = 1;
        else
            m_inputDirection.y = 0;

        if (Input.IsActionPressed("Left"))
            m_inputDirection.x = -1;
        else if (Input.IsActionPressed("Right"))
            m_inputDirection.x = 1;
        else
            m_inputDirection.x = 0;

    }

    public void Exit(ref Player host)
    {
        host.CollisionBox.Disabled = false;
        host.Modulate = new Color(1, 1, 1, 1f);
    }

    public void Update(ref Player host, float delta)
    {
        GetInput();

        m_velocity.x += m_inputDirection.x * Moving.ACCELERATION;
        m_velocity.y += m_inputDirection.y * Moving.ACCELERATION;

        if (m_inputDirection.y == 0)
            m_velocity.y -= Moving.DECELERATION * Mathf.Sign(m_velocity.y);
        if(m_inputDirection.x == 0)
            m_velocity.x -= Moving.DECELERATION * Mathf.Sign(m_velocity.x);

        m_velocity.x = Mathf.Clamp(m_velocity.x, -200, 200);
        m_velocity.y = Mathf.Clamp(m_velocity.y, -200, 200);

        host.MoveAndSlide(m_velocity);
    }
}

