using Godot;

public class JumpPad : Node2D
{
    // Node references
    AnimatedSprite Sprite;
    CollisionShape2D Hitbox;
    Area2D DetectionZone;

    private bool ArrowPresent = false;
    private float Angle;
    private Vector2 Direction = new Vector2();

    public Rect2 Box;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Hitbox = GetNode("StaticBody2D/CollisionShape2D") as CollisionShape2D;
        DetectionZone = GetNode("Area2D") as Area2D;
        Sprite = GetNode("AnimatedSprite") as AnimatedSprite;

        Box = new Rect2(GlobalPosition, new Vector2(8, 4));
    }

    /// <summary>
    /// Event for when Something enters in collision with this.
    /// </summary>
    /// <param name="body"></param>
    private void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        // If its an arrow.
        if (body is Arrow)
        {
            // Change animation
            Sprite.Frame = 4;
            Sprite.Stop();

            Hitbox.Disabled = false;
            ArrowPresent = true;
        }

        // If its a player and there is no arrow. JUMP!
        if (body is Player && !ArrowPresent)
        {
            // The Jumppad method takes a direction as param.
            // The direction is defined by the angle of the jumppad.
            switch (RotationDegrees)
            {
                case 0.0f:
                    Direction = new Vector2(0, -1);// ^
                    break;
                case 90.0f:
                    Direction = new Vector2(1, 0); // ->
                    break;
                case 180.0f:
                    Direction = new Vector2(0, 1); // v
                    break;
                case 270.0f:
                    Direction = new Vector2(-1, 0); // <-
                    break;
            }

            // Apply jump
            (body as Player).JumpPad(Direction);

            Sprite.Stop();
            Sprite.Frame = 0;
            Sprite.Play("bounce");

            (GetNode("SFX/Bounce") as AudioStreamPlayer).Play();
        }
    }


    private void _on_Area2D_body_exited(PhysicsBody2D body)
    {
        if (body is Arrow)
        {
            ArrowPresent = false;
            Sprite.Play("bounce");
            Hitbox.Disabled = true;
        }
    }
}



