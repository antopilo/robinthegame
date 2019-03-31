using Godot;

public class DoorPuzzle : Node2D
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
        Collision = GetNode("collision/box") as CollisionShape2D;
        Sprite = GetNode("sprDoor") as Sprite;

        Box = new Rect2(GlobalPosition, new Vector2(24, 24));
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
