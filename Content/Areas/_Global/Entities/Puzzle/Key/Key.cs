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
                InventoryManager.AddItem("Key", 1);
                Grabbed = true;

                Target = Player;
            }
        }
        else if(!Used && Grabbed)
        {
            T.InterpolateProperty(this, "global_position", GlobalPosition, Target.GlobalPosition, 0.3f, 
                Tween.TransitionType.Linear, Tween.EaseType.Out);
            T.InterpolateProperty(this, "scale", new Vector2(1,1), new Vector2(0, 0), 0.3f, 
                Tween.TransitionType.Linear, Tween.EaseType.Out);
            T.Start();
        }
    }

    public void Kill()
    {
        T.StopAll();
        Sprite.Visible = false;
        this.QueueFree();
    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        Kill();
    }
}

