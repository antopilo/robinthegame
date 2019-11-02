using Godot;

public class Extender : Node2D
{
    private AnimatedSprite Spr;
	private Tween T;
	private StaticBody2D Collision;
    public Rect2 Box;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Spr = GetNode("AnimatedSprite") as AnimatedSprite;
		T = GetNode("Tween") as Tween;
		Collision = GetNode("collision") as StaticBody2D;

        Box = new Rect2(GlobalPosition, new Vector2(16, 8));
    }

	private void _on_Area2D_body_entered(PhysicsBody2D body)
	{
        if (body is Arrow)
            Play("extend");
	}

	private void _on_Area2D_body_exited(PhysicsBody2D body)
	{
        if (body is Arrow)
            Play("retract");
	}
	
	public void Play(string pAnim)
	{
        Vector2 Destination;
        if (pAnim == "extend")
            Destination = new Vector2(8f, -4f);
        else
            Destination = new Vector2(8f, 12f);
        T.InterpolateProperty(Collision, "position", Collision.Position, Destination, 0.5f,
                    Tween.TransitionType.Linear, Tween.EaseType.InOut);
        T.Start();
        Spr.Animation = pAnim;
        Spr.Play();
    }
}
