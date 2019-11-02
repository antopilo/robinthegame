using System;
using Godot;

public class ArrowBoostGrace : IState
{
    
    private const float GRACETIME_DURATION = 0.33f;

    public string StateName => "ArrowBoostGrace";
    private Vector2 m_Velocity = new Vector2();
    private float m_GraceTime = GRACETIME_DURATION;

    public void Enter(ref Player host)
    {
        m_GraceTime= GRACETIME_DURATION;
        host.Sprite.Play("Crouch");
        host.Arrow.TiltDown();
    }

    public void Update(ref Player host, float delta)
    {
        // If its still grace time then countdown.
        if(m_GraceTime > 0f)
        {
            m_GraceTime -= delta;
            
            host.Sprite.Play("Crouch");
        }
        else // Did the player used the boost?
        {
            host.Arrow.Jiggle();
            host.StateMachine.SetState("ArrowBoosted");
        }
    }

    public void Exit(ref Player host)
    {
        host.Velocity = m_Velocity;
    }
}
