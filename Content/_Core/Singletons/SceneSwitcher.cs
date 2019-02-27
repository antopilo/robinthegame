using Godot;
using System;
using System.Collections.Generic;

public class SceneSwitcher : Node
{
    public static GameController CurrentWorld;
    public static Dictionary<string, PackedScene> LoadedWorld = new Dictionary<string, PackedScene>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CurrentWorld = Root.GameController;
    }

    public static void ChangeWorld()
    {
        //if(!LoadedWorld.ContainsKey(destination.GetName()))
        //{
        //    GameController newWorld = destination.Instance() as GameController;
        //    SaveWorld();
        //    CurrentWorld.QueueFree();
        //    Root.Viewport.AddChild(newWorld);
        //    CurrentWorld = newWorld;

        //    Root.Player = Root.GameController.GetNode("Player") as Player;
        //    Root.Weapon = Root.Player.GetNode("Weapon") as Weapon;
        //}
    }

    public static void SaveWorld()
    {
        SetOwners(CurrentWorld);

        // Create resource
        var newPackedScene = new PackedScene();
        newPackedScene.Pack(CurrentWorld);

        LoadedWorld.Add(CurrentWorld.Name, newPackedScene);
    }

    // This makes sure that everynode contained in the Currentlevel gets saved
    // in the packed scene level. See EnterDebugMode() for more.
    private static void SetOwners(Node pNode)
    {
        foreach(Node node in pNode.GetChildren() )
        {
            if(node is Player)
                continue;

            node.SetOwner(CurrentWorld);

            if(node is Entity)
                continue;

            if(node.GetChildren().Count > 0) 
                SetOwners(node); // Recursivity
        }       
    }
}
