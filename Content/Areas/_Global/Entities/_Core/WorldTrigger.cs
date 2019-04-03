using Godot;
using System;

public class WorldTrigger : Area2D
{
    [Export] public string Destination = "";
    [Export] public string Waypoint = "";
    private string WorldPath = "res://Content/Areas/Worlds/";
    private void _on_WorldTrigger_body_entered(object body)
    {
        if (body is Player)
            Root.SceneSwitcher.ChangeWorld(ResourceLoader.Load(WorldPath + Destination + ".tscn") as PackedScene, Waypoint);
    }
}



