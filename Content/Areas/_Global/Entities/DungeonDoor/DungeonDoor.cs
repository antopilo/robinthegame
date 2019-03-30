using Godot;
using System;

public class DungeonDoor : Node2D
{
    [Export] public PackedScene DestinationWorld = ResourceLoader.Load("res://Content/Areas/Worlds/ForestWorld.tscn") as PackedScene;

    private PackedScene World;
    private SceneSwitcher SceneSwitcher;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void Interact() => Root.SceneSwitcher.ChangeWorld(DestinationWorld);
}
