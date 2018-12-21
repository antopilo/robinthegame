using Godot;

public class Weapon : Node2D
{
    private PackedScene ArrowScene;
    private Player Player;
    private Timer Timer;
    private Position2D Origin;

    public Arrow CurrentArrow;
    public bool ControllerMode = false;
    public bool CanShoot = true;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ArrowScene = ResourceLoader.Load("res://Content/Actors/Player/Arrow/arrow.tscn") as PackedScene;
        Player = GetParent() as Player;
        Timer = GetNode("../Timers/Timer") as Timer;
        Origin = GetNode("Arrow_origin") as Position2D;
    }

    public override void _Input(InputEvent @event)
	{
        if (CanShoot)
        {
            if (@event.IsActionPressed("fire"))
            {
                CanShoot = false;

                Timer.Start();
                ShootArrow();
            }
            if (@event.IsActionPressed("right_click"))
            {
                CanShoot = false;

                Timer.Start();
                ShootArrow();
                CurrentArrow.Dash();
            }
        }
    }

    public void ShootArrow()
    {
        Vector2 FinalPosition = Origin.GlobalPosition;

        CurrentArrow = (Arrow)ArrowScene.Instance();
		GetNode("../..").AddChild(CurrentArrow);
		
        CurrentArrow.GlobalPosition = FinalPosition;
        CurrentArrow.ControllerMode = ControllerMode;
        CurrentArrow.Name = "arrow";

        
    }
}
