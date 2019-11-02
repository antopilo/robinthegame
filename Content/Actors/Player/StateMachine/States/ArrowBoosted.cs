using System;
using Godot;

public class ArrowBoosted : IState
{
    string IState.StateName => "ArrowBoosted";

    private Vector2 m_velocity;

    private const float BOOSTTIME_DURATION = 0.4f;
    private float BoostTime = BOOSTTIME_DURATION;

    void IState.Enter(ref Player host)
    {
        host.Jump(1.5f);
        m_velocity = host.Velocity;

        host.Velocity = m_velocity;
        host.RunDust.Emitting = true;
        
    }

    void IState.Exit(ref Player host)
    {
        host.Velocity = m_velocity;
        host.RunDust.Emitting = false;
        BoostTime = BOOSTTIME_DURATION;
    }

    void IState.Update(ref Player host, float delta)
    {
        ApplyGravity();

        host.Sprite.Play("Spinning");

        if(BoostTime > 0f)
            BoostTime -= delta;
        else
            host.StateMachine.SetState("Air");

        // Move the player.
        host.MoveAndSlide(m_velocity, new Vector2(0, -1));
    }

    private void ApplyGravity()
    {
        m_velocity.y += Air.GRAVITY;
    }

    private void UpdateSprite(ref Player host)
    {
        if (m_velocity.y >= 0)
            host.Sprite.Play("Falling");
        else
            host.Sprite.Play("Jump");
            
    }
}

