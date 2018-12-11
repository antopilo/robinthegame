using Godot;

public class Spike : Node2D
{
    private void _on_oSpike_body_entered(PhysicsBody2D body)
	{
        float Rotation = this.RotationDegrees;

        if(body is Player && (body as Player).Alive)
        {
            switch (Rotation)
            {
                case 0:
                    if ((body as Player).Velocity.y >= -1)
                    {
                        (body as Player).Alive = false;
                        (body as Player).Spawn(true);
                    }
                    break;
                case 90:
                    if ((body as Player).Velocity.x <= 0)
                    {
                        (body as Player).Alive = false;
                        (body as Player).Spawn(true);
                    }
                    break;
                case 180:
                    if ((body as Player).Velocity.y <= 0)
                    {
                        (body as Player).Alive = false;
                        (body as Player).Spawn(true);
                    }
                    break;
                case 270:
                    if ((body as Player).Velocity.x >= 0)
                    {
                        (body as Player).Alive = false;
                        (body as Player).Spawn(true);
                    }
                    break;
            }
        }
        
        GD.Print("P: " + body.GlobalPosition.y + "S: " + this.GlobalPosition.y);
	}
}



