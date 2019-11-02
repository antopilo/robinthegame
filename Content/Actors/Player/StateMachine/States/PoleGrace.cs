using System;
using Godot;

public class PoleGrace : IState
{
    public string StateName => "PoleGrace";

    private const float SPIN_DURATION = 0.5f;
    private float m_SpinTime = SPIN_DURATION;

    private Vector2 m_Velocity = new Vector2();

    public void Enter(ref Player host)
    {
        m_SpinTime = SPIN_DURATION;
    }

    public void Exit(ref Player host)
    {
        
    }

    public void Update(ref Player host, float delta)
    {
        // During spin.
        if(m_SpinTime > 0f)
        {
            m_SpinTime -= delta;
            host.Sprite.Play("PoleSwing");
        }
        else // Spins is over!
        {
            // If the space button is being holded.
            if(Input.IsActionPressed("Jump"))
            {
                // Boost
                host.StateMachine.SetState("ArrowBoosted");
            }
            else
            {
                // Stand on it.
                host.Jump();
                
                host.StateMachine.SetState("Air");
            }
        }
    }
}

