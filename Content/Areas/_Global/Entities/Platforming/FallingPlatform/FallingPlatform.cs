using Godot;

public enum PlatformStates { Ready, Shaking, Down, Waiting, Up, CoolDown}

public class FallingPlatform : Node2D
{
    public Rect2 Box;
    // Movement
    [Export] float FALL_DISTANCE = 300;
    [Export] int FALL_TIME = 4;
    [Export] int READY_CD = 1;
    private Vector2 InitialPosition = new Vector2();
    private Color InitialColor;

    // Shake
    [Export] int SHAKE_AMPLITUDE = 25;
    [Export] int SHAKE_LENGTH = 20;
    [Export] float SHAKE_TIME = 0.5f;
    [Export] int SpawnDelay = 5;

    // Refs
    private Sprite Sprite;
    private StaticBody2D Platform;
    private Tween Tween;
    private Timer Timer;
    private Timer ReadyCd;
    private Timer ShakeTimer;
    private CollisionShape2D Collision;
    private Area2D DetectionZone;
    private RayCast2D Raycast;
    // Engine
    float DeltaTime = 0;

    PlatformStates State = PlatformStates.Ready;

    // Old States
    //private bool Available = true;
    //private bool Up = true;
    //private bool Shaking = false;
    //private bool Ready = true;

    public override void _Ready()
    {
        // Get Refs
        Sprite = GetNode("Platform/Sprite") as Sprite;
        Platform = GetNode("Platform") as StaticBody2D;
        Tween = GetNode("TweenElevator") as Tween;
        Timer = GetNode("RespawnCooldown") as Timer;
        ReadyCd = GetNode("ReadyCooldown") as Timer;
        ShakeTimer = GetNode("ShakeTimer") as Timer;
        Collision = GetNode("Platform/Collision") as CollisionShape2D;
        DetectionZone = GetNode("Area2D") as Area2D;
        //RayCast = GetNode("RayCast") as RayCast2D;
        // Get initial position and color so the platform can reset to those values.
        InitialColor = (this as CanvasItem).Modulate;
        InitialPosition = Platform.Position;

        Box = new Rect2(GlobalPosition, new Vector2(8, 8));
    }

    // Update every frame.
    public override void _PhysicsProcess(float delta)
    {
        if (State == PlatformStates.Shaking) // If shaking, do the shaking animation/effect.
        {
            Sprite.RotationDegrees = Mathf.Cos(DeltaTime * SHAKE_LENGTH) * SHAKE_AMPLITUDE;
            DeltaTime += delta;
        }
 
    }

    public void Fall()
    {
        State = PlatformStates.Down;
        Collision.Disabled = true;

        // Fall down
        Tween.InterpolateProperty(Platform, "position", InitialPosition, new Vector2(InitialPosition.x, FALL_DISTANCE),
                 0.8f, Tween.TransitionType.Expo, Tween.EaseType.In);
        Tween.InterpolateProperty(Platform, "modulate", InitialColor,
           new Color(1, 1, 1, 0), 2, Tween.TransitionType.Linear, Tween.EaseType.OutIn);

        Tween.Start();
        Timer.Start();
        
    }

    /// <summary>
    /// This is when something Collides with the platform (walks on it).
    /// </summary>
    /// <param name="body"></param>
    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if(State != PlatformStates.Ready || !(body is Player))
            return;

        var player = (body as Player);
        if(player.Velocity.y >= 0) // Activate shaking process and Timer.
        {
            State = PlatformStates.Shaking;
            ShakeTimer.Start();
        }   
    }

    /// <summary>
    /// Called when the platform needs to go back up.
    /// </summary>
    private void _on_RespawnCooldown_timeout()
    {
        // Slowly Tween back to original position and opacity.
        Tween.InterpolateProperty(Sprite, "scale", new Vector2(), new Vector2(1, 1), 2f, Tween.TransitionType.Elastic, Tween.EaseType.Out);

        Platform.Position = InitialPosition;
        Platform.Modulate = InitialColor;

        Tween.Start();
    }

    private void _on_TweenElevator_tween_completed(Godot.Object @object, NodePath key)
    {

        if (State == PlatformStates.Up)
        {
            ReadyCd.WaitTime = READY_CD;
            ReadyCd.Start();
            Collision.Disabled = false;
        }
        else if (State == PlatformStates.Down)
            State = PlatformStates.Up;
        Sprite.GlobalRotationDegrees = 0;
    }

    private void _on_ReadyCooldown_timeout()
    {
       
        State = PlatformStates.Ready;
        Collision.Disabled = false;
    }

    public void Reset()
    {
        if(State == PlatformStates.Ready)
            return;

        // Slowly Tween back to original position and opacity.
        Tween.RemoveAll();
        Tween.InterpolateProperty(Sprite, "scale", new Vector2(), new Vector2(1, 1), 2f, Tween.TransitionType.Elastic, Tween.EaseType.Out);
        Tween.Start();
        
        DetectionZone.CallDeferred("set_monitoring", true);
        State = PlatformStates.Ready;
        Sprite.RotationDegrees = 0;
        Collision.Disabled = false;
        Platform.Position = InitialPosition;
        Platform.Modulate = InitialColor;
    }

    /// <summary>
    /// When Shaking timer is done. FALL! AHHHHHHHHHHHHHHHHHHHHHHHHHh....
    /// </summary>
    private void _on_ShakeTimer_timeout()
        =>  Fall();
    
        

}