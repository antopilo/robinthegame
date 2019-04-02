using Godot;
using System;

public class DungeonDoor : Node2D
{
    [Export] public string DestinationWorld; // World to load
    [Export] public string Waypoint = ""; // Waypoint in world.

    private string WorldPath = "res://Content/Areas/Worlds/";
    private PackedScene World;
    private SceneSwitcher SceneSwitcher;

    public void Interact()
    {
        Root.Player.RemoveFromInteraction(this);
        var scene = (PackedScene)ResourceLoader.Load(WorldPath + DestinationWorld + ".tscn");
        Root.SceneSwitcher.ChangeWorld(scene, Waypoint);
    }
}
