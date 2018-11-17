using Godot;

public class FallingPlatform : Node2D
{
    public Rect2 Box;
    const float FALL_DISTANCE = 300;
    const int SHAKE_AMPLITUDE = 25;
    const int SHAKE_LENGTH = 20;

    private Sprite Sprite;
    private StaticBody2D Platform;
    private Tween Tween;
    private Timer Timer;
    private Timer ShakeTimer;
    private CollisionShape2D Collision;
    float DeltaTime = 0; // Total time of the engine;

    private Vector2 InitialPosition = new Vector2();
    private Color InitialColor;

    private bool Available = true;
    private bool Up = true;
    private bool Shaking = false;
    
    public override void _Ready()
    {
        Sprite = GetNode("Platform/Sprite") as Sprite;
        Platform = GetNode("Platform") as StaticBody2D;
        Tween = GetNode("TweenElevator") as Tween;
        Timer = GetNode("RespawnCooldown") as Timer;
        ShakeTimer = GetNode("ShakeTimer") as Timer;
        Collision = GetNode("Platform/Collision") as CollisionShape2D;

        // Get initial position and color so the platform can reset to those values.
        InitialColor = (this as CanvasItem).Modulate;
        InitialPosition = Platform.Position;

        Box = new Rect2(GlobalPosition, new Vector2(8, 8));
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (Shaking) // If shaking, do the shaking animation/effect.
        {
            Sprite.RotationDegrees = Mathf.Cos(DeltaTime * 20) * 25;
            DeltaTime += delta;
        }
    }

    public void Fall()
    {
        Collision.Disabled = true;

        Tween.InterpolateProperty(Platform, "position", InitialPosition, new Vector2(InitialPosition.x, FALL_DISTANCE),
                 0.8f, Tween.TransitionType.Expo, Tween.EaseType.In);

        Tween.InterpolateProperty(Platform, "modulate", InitialColor,
           new Color(1, 1, 1, 0), 2, Tween.TransitionType.Linear, Tween.EaseType.OutIn);

        Tween.Start();
        Timer.Start();
        
    }
    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if(Available && Up)
        {
            Shaking = true;
            Available = false;
            Up = false;

            ShakeTimer.Start();
        }
    }

    private void _on_RespawnCooldown_timeout()
    {
        Tween.InterpolateProperty(Platform, "position", new Vector2(InitialPosition.x, FALL_DISTANCE), 
            InitialPosition, 0.8f, Tween.TransitionType.Expo, Tween.EaseType.Out);

        Tween.InterpolateProperty(Platform, "modulate", new Color(1, 1, 1, 0),
            InitialColor, 2, Tween.TransitionType.Linear, Tween.EaseType.Out);

        Tween.Start();
    }

    private void _on_TweenElevator_tween_completed(Godot.Object @object, NodePath key)
    {
        if (Up)
        {
            Available = true;
            Collision.Disabled = false;
        }
        else
        {
            Up = true;
            Shaking = false;
        }

        Sprite.GlobalRotationDegrees = 0;
    }

    private void _on_ShakeTimer_timeout()
        =>  Fall();
}