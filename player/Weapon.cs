using Godot;

public class Weapon : Node2D
{
    PackedScene ArrowScene;
    Player Player;
    Timer Timer;
    Node2D Origin;

    public Arrow CurrentArrow;
    public bool ControllerMode = false;
    public bool CanShoot = true;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ArrowScene = (PackedScene)ResourceLoader.Load("res://player/arrow.tscn");
        Player = (Player)GetNode("..");
        Timer = (Timer)GetNode("Timer");
        Origin = ((Node2D)GetNode("Arrow_origin"));
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
