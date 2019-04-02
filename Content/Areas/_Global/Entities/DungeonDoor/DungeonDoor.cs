using Godot;
using System;

public class DungeonDoor : Node2D
{
    [Export] public PackedScene DestinationWorld; // World to load
    [Export] public string Waypoint = ""; // Waypoint in world.

    private PackedScene World;
    private SceneSwitcher SceneSwitcher;

    public void Interact()
    {
        Root.Player.RemoveFromInteraction(this);
        Root.SceneSwitcher.ChangeWorld(DestinationWorld, Waypoint);
    }
}
