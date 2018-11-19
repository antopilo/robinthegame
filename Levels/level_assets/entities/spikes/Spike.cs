using Godot;

public class Spike : Node2D
{
    public Rect2 Box;
    
    public override void _Ready()
    {
        Box = new Rect2(GlobalPosition, new Vector2(8, 4));
    }
    private void _on_oSpike_body_entered(PhysicsBody2D body)
	{
        if (body is Player)
        {
            (body as Player).Spawn(true);
           
        }
	}
}



