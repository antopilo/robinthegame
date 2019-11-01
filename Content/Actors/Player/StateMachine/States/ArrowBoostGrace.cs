using System;
using Godot;

public class ArrowBoostGrace : IState
{
    
    private const float GRACETIME_DURATION = 0.5f;

    public string StateName => "ArrowBoostGrace";
    private Vector2 m_Velocity = new Vector2();
    
    private float m_GraceTime = GRACETIME_DURATION;
    private bool m_Jumped = false;

    public void Enter(ref Player host)
    {
        m_Velocity = host.Velocity;
        m_Jumped = false;
        m_GraceTime= GRACETIME_DURATION;
        host.Sprite.Play("Crouch");
    }

    public void Update(ref Player host, float delta)
    {
        // If its still grace time then countdown.
        if(m_GraceTime > 0f)
        {
            m_GraceTime -= delta;
            
            host.Sprite.Play("Crouch");

            // If the player decided to use the boost.
            if(Input.IsActionJustPressed("Jump"))
                m_Jumped = true;
        }
        else // Did the player used the boost?
        {
            if(m_Jumped)
            {
                host.Arrow.Jiggle();
                host.StateMachine.SetState("ArrowBoosted");
            }
            else
            {
                host.Arrow.Reset();
                host.StateMachine.SetState("Idle");
            }
        }

    }


    public void Exit(ref Player host)
    {
        
    }
}
