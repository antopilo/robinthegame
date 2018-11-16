using Godot;
using System;

public class FallingPlatform : Node2D
{
    Sprite Sprite;
    StaticBody2D Platform;
    Tween Tween;
    Timer Timer;
    Timer ShakeTimer;
    CollisionShape2D Collision;

    Vector2 InitialPosition = new Vector2();
    Color InitialColor;
    float FallDistance = 300f;
    float DeltaTime = 0;
    bool Availaible = true;
    bool Up = true;
    bool Shaking = false;

    public override void _Ready()
    {
        Sprite = GetNode("Platform/Sprite") as Sprite;
        Platform = GetNode("Platform") as StaticBody2D;
        Tween = GetNode("TweenElevator") as Tween;
        Timer = GetNode("RespawnCooldown") as Timer;
        ShakeTimer = GetNode("ShakeTimer") as Timer;
        Collision = GetNode("Platform/Collision") as CollisionShape2D;
        InitialColor = (this as CanvasItem).Modulate;
        InitialPosition = Platform.Position;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (Shaking)
        {
            Sprite.RotationDegrees = Mathf.Cos(DeltaTime * 20) * 25;
            DeltaTime += delta;
        }
    }

    public void Fall()
    {
        Tween.InterpolateProperty(Platform, "position", InitialPosition,
                new Vector2(InitialPosition.x, FallDistance), 0.8f, Tween.TransitionType.Expo, Tween.EaseType.OutIn);
        Tween.InterpolateProperty(Platform, "modulate", InitialColor,
           new Color(1, 1, 1, 0), 2, Tween.TransitionType.Linear, Tween.EaseType.OutIn);
        Tween.Start();
        Timer.Start();
        Collision.Disabled = true;
    }
    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if(Availaible && Up)
        {
            Shaking = true;
            ShakeTimer.Start();
            Availaible = false;
            Up = false;
        }
    }

    private void _on_RespawnCooldown_timeout()
    {
        Tween.InterpolateProperty(Platform, "position", new Vector2(InitialPosition.x, FallDistance), 
            InitialPosition, 0.8f, Tween.TransitionType.Expo, Tween.EaseType.OutIn);
        Tween.InterpolateProperty(Platform, "modulate", new Color(1, 1, 1, 0),
            InitialColor, 2, Tween.TransitionType.Linear, Tween.EaseType.OutIn);
        Tween.Start();
    }

    private void _on_TweenElevator_tween_completed(Godot.Object @object, NodePath key)
    {
        if (Up)
        {
            Availaible = true;
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
    {
        Fall();
    }
}



