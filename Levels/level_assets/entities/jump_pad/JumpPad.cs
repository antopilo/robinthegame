using Godot;
using System;

public class JumpPad : Node2D
{
    AnimatedSprite Sprite;
    CollisionShape2D Hitbox;
    Area2D DetectionZone;

    private bool ArrowPresent = false;
    float Angle;
    Vector2 Direction = new Vector2();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Hitbox = GetNode("StaticBody2D/CollisionShape2D") as CollisionShape2D;
        DetectionZone = GetNode("Area2D") as Area2D;
        Sprite = GetNode("AnimatedSprite") as AnimatedSprite;
    }

    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if(body is Arrow)
        {
            Sprite.Frame = 4;
            Sprite.Stop();
            Hitbox.Disabled = false;
            ArrowPresent = true;
        }
        if(body is Player && !ArrowPresent)
        {
            switch (RotationDegrees)
            {
                case 0.0f:
                    Direction = new Vector2(0, -1);
                    break;
                case 90.0f:
                    Direction = new Vector2(1, 0);
                    break;
                case 180.0f:
                    Direction = new Vector2(0, 1);
                    break;
                case 270.0f:
                    Direction = new Vector2(-1, 0);
                    break;
            }
            (body as Player).JumpPad(Direction);
            Sprite.Stop();
            Sprite.Frame = 0;
            Sprite.Play("bounce");
        }
    }


    private void _on_Area2D_body_exited(PhysicsBody2D body)
    {
        if(body is Arrow)
        {
            ArrowPresent = false;
            Sprite.Play("bounce");
            Hitbox.Disabled = true;
        }
    }
}



