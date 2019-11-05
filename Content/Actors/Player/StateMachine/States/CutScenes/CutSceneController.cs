using Coroutines;
using Godot;

public enum Keys
{
    LEFT, RIGHT, DOWN, UP, JUMP
}

public class CutSceneController : IState
{
    public virtual string StateName => throw new System.NotImplementedException();

    public CoroutineHandle Coroutine;
    public CoroutineRunner Runner = new CoroutineRunner();

    public const int JUMP_FORCE = 190;
    private const int MAX_SPEED = 90;
    public const int ACCELERATION = 8;
    public const int DECELERATION = 4;

    private Vector2 InputDirection = new Vector2();
    private bool Jumping = false;

    private Vector2 m_velocity = new Vector2();

    public void Press(Keys key)
    {
        switch(key)
        {
            case Keys.LEFT:
                InputDirection.x = -1;
                break;
            case Keys.RIGHT:
                InputDirection.x = 1;
                break;
            case Keys.DOWN:
                InputDirection.y = -1;
                break;
            case Keys.UP:
                InputDirection.y = 1;
                break;
        }
    }

    public void Release(Keys key)
    {
        switch(key)
        {
            case Keys.LEFT:
                if(InputDirection.x == -1)
                    InputDirection.x = 0;
                break;
            case Keys.RIGHT:
                if(InputDirection.x == 1)
                    InputDirection.x = 0;
                break;
            case Keys.DOWN:
                if(InputDirection.y == -1)
                    InputDirection.y = 0;
                break;
            case Keys.UP:
                if(InputDirection.y == 1)
                    InputDirection.y = 0;
                break;
        }
    }


    public virtual void Enter(ref Player host)
    {
        if(host.Arrow != null)
            host.Arrow.ReturnToPlayer();
        Runner.StopAll();
    }

    public virtual void Exit(ref Player host)
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update(ref Player host, float delta)
    {
        Runner.Update(delta);

         // If pressing something, accelerate.
        m_velocity.x += InputDirection.x * ACCELERATION;

        // Implements a max speed.
        if (Mathf.Abs(m_velocity.x) > MAX_SPEED)
            m_velocity.x = MAX_SPEED * Mathf.Sign(m_velocity.x);

        // If not pressing anything, decelerate.
        if (InputDirection.x == 0)
            m_velocity.x -= DECELERATION * Mathf.Sign(m_velocity.x);

        if((LeftWallCheck(ref host)  & InputDirection.x == -1) ||
           (RightWallCheck(ref host) & InputDirection.x ==  1))
            m_velocity.x = 0;

        // Make sure that the player actually stops. 
        if (Mathf.Abs(m_velocity.x) < 10 && InputDirection.x == 0)
            m_velocity.x = 0;

        // If the player is idle...
        if (m_velocity.x == 0 && InputDirection == new Vector2())
            host.StateMachine.SetState("Idle");

        // Sets velocity for state transition.
        host.Velocity = m_velocity;
        
        DebugPrinter.AddDebugItem("Velocity", m_velocity.ToString());
        host.MoveAndSlide(m_velocity, new Vector2(0, -1));
        
        UpdateSprite(ref host);
        if(!host.IsOnFloor())
            ApplyGravity(ref host);
        else
            m_velocity.y = 8;

    }

    public void ApplyGravity(ref Player host)
    {
        var scale = 1 - ((Mathf.Abs(m_velocity.y) / Air.MAX_AIR_SPEED) / 4);
       // host.Sprite.Scale = new Vector2(Mathf.Clamp(scale, 0.75f, 1), 1);

        m_velocity.y += Air.GRAVITY;
    }
    private bool LeftWallCheck(ref Player host)
    {
        return ((RayCast2D)host.GetNode("Raycasts/Left")).IsColliding();
    }

    private bool RightWallCheck(ref Player host)
    {
        return ((RayCast2D)host.GetNode("Raycasts/Right")).IsColliding();
    }

    public void Jump()
    {
        var CollisionCount = Root.Player.GetSlideCount() - 1;
        KinematicCollision2D col = null;
        if (CollisionCount > -1)
        {
            col = Root.Player.GetSlideCollision(CollisionCount);
        }
        if (col != null && col.Collider is Arrow)
            (col.Collider as Arrow).Jiggle();

        Root.Player.WasOnGround = true;

        m_velocity.y = -JUMP_FORCE;
    }

    public bool CheckWalls(ref Player host)
    {
        var CollisionCount = host.GetSlideCount() - 1;
        KinematicCollision2D col = null;
        if (CollisionCount > -1)
        {
            for(int i = 0; i < CollisionCount; i++)
            {
                col = host.GetSlideCollision(i);

                // hit a wall
                if(col.Normal == new Vector2(1, 0) || col.Normal == new Vector2(-1, 0))
                    return true;
            }
        }
        return false;
    }
    
    public void UpdateSprite(ref Player host)
    {
        if(host.IsOnFloor())
        {
            if(InputDirection.x == -1 )
            {
                host.Sprite.Play("Running");
                host.Sprite.FlipH = true;
            }
            else if(InputDirection.x == 1 )
            {
                host.Sprite.Play("Running");
                host.Sprite.FlipH = false;
            }
            else
                host.Sprite.Play("Idle");
        }
        else // Air
        {
            if(InputDirection.x == -1 )
                host.Sprite.FlipH = true;
            else if(InputDirection.x == 1 )
                host.Sprite.FlipH = false;

            if (m_velocity.y >= 0)
                host.Sprite.Play("Falling");
            else
                host.Sprite.Play("Jump");
            
        }

    }
}

