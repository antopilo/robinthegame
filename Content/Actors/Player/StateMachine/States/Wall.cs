using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum WallDirection
{
    LEFT = -1, NONE = 0, RIGHT = 1
}

class Wall : IState
{
    public string StateName => "Wall";

    private Vector2 m_velocity = new Vector2();
    private WallDirection m_wallDirection = 0;
    private float wallFallMult = 0;
    private const float MAX_FALL_SPEED = 30f;

    // Init.
    public void Enter(ref Player host)
    {
        m_wallDirection = host.WallDirection;
        m_velocity = host.Velocity;

        
    }


    // Update each frame.
    public void Update(ref Player host, float delta)
    {
        // Gravity is valid when on a wall.
        ApplyGravity();

        // If the player is sliding down a wall and he hits the floow.
        // Make him switch state to idle.

        if(Air.LandedOnArrow(ref host))
            host.StateMachine.SetState("ArrowBoostGrace");
            
        else if (host.IsOnFloor())
            host.StateMachine.SetState("Idle");

        UpdateInput(ref host);

        // Set the sprite and direction.
        if (m_velocity.y > 0)
            host.Sprite.Play("Wall");
        else
            host.Sprite.Play("Falling");

        if (m_wallDirection == WallDirection.LEFT)
            host.Sprite.FlipH = false;
        else
            host.Sprite.FlipH = true;

        // Walljump.
        if (Input.IsActionJustPressed("Jump"))
            host.StateMachine.SetState("Walljump");

        // Max fall speed
        // TODO: CHANGE MAGIC VALUE.
        m_velocity.y = Mathf.Clamp(m_velocity.y, -Mathf.Inf, MAX_FALL_SPEED);

        // Move the player.
        host.MoveAndSlide(m_velocity, new Vector2(0, -1));

        if (host.GetSlideCount() <= 0)
            host.StateMachine.SetState("Air");

    }
    

    private void UpdateInput(ref Player host)
    {
        if (Input.IsActionPressed("Left") && m_wallDirection == WallDirection.LEFT)
            host.StateMachine.SetState("Air");
        else if (Input.IsActionPressed("Right") && m_wallDirection == WallDirection.RIGHT)
            host.StateMachine.SetState("Air");
    }


    // Apply a force downward on the m_velocity.;
    private void ApplyGravity()
    {
        m_velocity.y += Air.GRAVITY ;
    }

    // Transfer velocity to the player. The next state might need it.
    public void Exit(ref Player host)
    {
        host.Velocity = m_velocity;
        host.WallDirection = 0;
    }
}

