using Godot;

public class Door : Node2D
{
    public Rect2 Box;
    Player Player;
    CollisionShape2D Collision;
    Sprite Sprite;

    const int DETECTION_RANGE = 25;
    float Distance;
    bool Opened = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetNode("../../../Player") as Player;
        Collision = GetNode("collision/box") as CollisionShape2D;
        Sprite = GetNode("sprDoor") as Sprite;

        Box = new Rect2(GlobalPosition, new Vector2(24, 24));
    }

    public override void _PhysicsProcess(float delta)
    {
        Distance = Mathf.Abs((GlobalPosition - Player.GlobalPosition).Length());

        if(Distance <= DETECTION_RANGE && Player.Following.Count > 0)
        {
            Opened = true;
            Collision.Disabled = true;
            Sprite.Visible = false;
            if((Player.Following[Player.Following.Count - 1] as Key).Used = false)
            {
                (Player.Following[Player.Following.Count - 1] as Key).T.StopAll();
                (Player.Following[Player.Following.Count - 1] as Key).Used = true;
                (Player.Following[Player.Following.Count - 1] as Key).MoveTo(this);
            }
            
            //this.QueueFree();
        }
    }
}
