using Godot;

public class Door : Node2D
{
    public Rect2 Box;
    Player Player;
    CollisionShape2D Collision;
    public Sprite Sprite;

    const int DETECTION_RANGE = 45;
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
        Distance = Mathf.Abs(((GlobalPosition + new Vector2(12, 12)) - Player.GlobalPosition).Length());

        if(Distance <= DETECTION_RANGE && Player.Following.Count > 0 && !Opened)
        {
            if((Player.Following[Player.Following.Count - 1] as Key).Used == false)
            {
                Opened = true;
                (Player.Following[Player.Following.Count - 1] as Key).T.StopAll();
                (Player.Following[Player.Following.Count - 1] as Key).Used = true;
                (Player.Following[Player.Following.Count - 1] as Key).MoveTo(this);
                
            }
        }
    }

    public void Open()
    {
        Tween t = GetNode("Tween") as Tween;
        t.InterpolateProperty(GetNode("sprDoor"), "scale", new Vector2(1, 1), new Vector2(0, 0), 1f, Tween.TransitionType.Elastic, Tween.EaseType.InOut, 0);
        (GetNode("collision/box") as CollisionShape2D).Disabled = true; 
        t.Start();
    }

    private void _on_Tween_tween_completed(Object @object, NodePath key)
    {
        this.QueueFree();
    }
}
