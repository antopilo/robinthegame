using Godot;
using System;

public class DungeonDoor : Node2D
{
    [Export] public PackedScene DestinationWorld = ResourceLoader.Load("res://Content/Areas/Worlds/ForestWorld.tscn") as PackedScene;

    private PackedScene World;
    private SceneSwitcher SceneSwitcher;
    [Export] string Waypoint = "";

    public void Interact()
    {
        Root.SceneSwitcher.ChangeWorld(DestinationWorld, Waypoint);
    }
}
