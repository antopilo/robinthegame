using Godot;
using System;

public class Key : Node2D
{
    Player Player;
    Sprite Sprite;
    Tween T;

    const int DETECTION_RANGE = 20;
    float DistanceFromPlayer;
    bool Grabbed = false;
    bool Used = false;
    Node2D Target;
    public int Index;

    public override void _Ready()
    {
        Player = GetNode("../../../Player") as Player;
        Sprite = GetNode("sprKey") as Sprite;
        T = GetNode("Tween") as Tween;
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
    }

    public void Kill()
    {
        T.StopAll();
        Sprite.Visible = false;
        Player.Following.RemoveAt(Player.Following.Count - 1);
    }

    public void MoveTo(Node2D pTarget)
    {
        Vector2 Destination = pTarget.Position;
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
            Kill();
        }
    }
}

