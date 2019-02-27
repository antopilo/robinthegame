using Godot;
using System;

public class DungeonDoor : Node2D
{
    [Export] public PackedScene DestinationWorld;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if(DestinationWorld == null)
            GD.PrintErr(this.Name + " Dungeon door without a destination world!");
    }

    public void Interact()
    {
        if(DestinationWorld == null)
            return;

        SceneSwitcher.ChangeWorld();
        GD.Print("Door activated!");
    }
}
