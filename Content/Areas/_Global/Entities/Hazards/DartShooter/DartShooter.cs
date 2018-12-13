using Godot;
using System;

public class DartShooter : Node2D
{
    PackedScene DartScene;
    Position2D ShootPoint;

    const float RATE_FIRE = 1;
    private float DeltaTime;
    private float Time;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        DartScene = ResourceLoader.Load("res://Content/Areas/_Global/Entities/Hazards/DartShooter/Dart/Dart.tscn") as PackedScene;
        ShootPoint = GetNode("ShootPoint") as Position2D;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (DeltaTime >= RATE_FIRE && (GetNode("../../..") as GameController).CurrentRoom == (GetNode("../..") as Level))
        {
            Shoot();
            DeltaTime = 0;
        }

        DeltaTime += delta;
    }

    private void Shoot()
    {     
        (GetNode("AnimatedSprite") as AnimatedSprite).Frame = 0;

        Dart newDart = DartScene.Instance() as Dart;
        newDart.Position = ShootPoint.Position;

        switch (RotationDegrees)
        {
            case 0:
                newDart.Direction = new Vector2(-1, 0);
                break;
            case 90:
                newDart.Direction = new Vector2(0, -1);
                break;
            case 180:
                newDart.Direction = new Vector2(1, 0);
                break;
            case 270:
                newDart.Direction = new Vector2(0, 1);
                break;
            default:
                GD.Print("WARNING: A DARTSHOOT HAS A WEIRD ANGLE IN LEVEL: " + GetNode("../..").Name);
                break;
        }

        AddChild(newDart);
    }
}
