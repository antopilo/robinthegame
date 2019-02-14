using Godot;

public class Key : Node2D
{
    const int DETECTION_RANGE = 20;

    // Refs
    public Rect2 Box;
    private Player Player;
    private Sprite Sprite;
    public Tween T;
    private Door door;
    
    private float DistanceFromPlayer;
    private bool Grabbed = false;
    public bool Used = false;

    public Node2D Target;
    public int Index;

    public override void _Ready()
    {
        Player = GetNode("../../../Player") as Player;
        Sprite = GetNode("sprKey") as Sprite;
        T = GetNode("Tween") as Tween;

        Box = new Rect2(GlobalPosition, new Vector2(8, 8));
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        DistanceFromPlayer = Mathf.Abs((Player.GlobalPosition - this.GlobalPosition).Length());
        
        if (!Grabbed)
        {
            if(DistanceFromPlayer <= DETECTION_RANGE)
            {
                Player.Following.Add(this);
                Index = Player.Following.IndexOf(this);
                Grabbed = true;

                if (Index == 0)
                    Target = Player;
                else
                    Target = Player.Following[Index - 1];
            }
        }
        else if(!Used && Grabbed)
        {
            T.FollowProperty(this, "global_position", GlobalPosition, Target, "global_position", 0.3f, Tween.TransitionType.Linear, Tween.EaseType.Out);
            T.Start();
        }
        if (Used == true && door.Sprite.Scale.x < 0.5)
            Kill();
    }

    public void Kill()
    {
        T.StopAll();
        Sprite.Visible = false;
        this.QueueFree();
    }

    public void MoveTo(Node2D pTarget)
    {
        door = pTarget as Door;
        Vector2 Destination = pTarget.GlobalPosition + new Vector2(12,12);
        T.StopAll();
        T.InterpolateProperty(this, "global_position", GlobalPosition, Destination, 0.3f, Tween.TransitionType.Expo, Tween.EaseType.InOut);
        T.Start();
    }

    public void ChangeTarget(Node2D pTarget)
    {
        T.StopAll();
        T.FollowProperty(this, "global_position", GlobalPosition, pTarget, "global_position", 0.3f, Tween.TransitionType.Linear, Tween.EaseType.Out);
        T.Start();
    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        if (Used)
        {
            Player.Following.Remove(this);
            door.Open();
            Used = true;
            //Kill();
        }
    }
}
