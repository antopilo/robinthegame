using Godot;
using System;
using System.Collections.Generic;

public class SceneSwitcher : Node
{
    public static GameController CurrentWorld;

    public static Dictionary<string, PackedScene> LoadedWorld = new Dictionary<string, PackedScene>();
    public static PackedScene LastWorld;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CurrentWorld = Root.GameController;
    }

    public void ChangeWorld(PackedScene pWorld)
    {
        GD.Print("Door activated!");
        Root.Viewport.GetChild(0).QueueFree();
        GameController newWorldScene = pWorld.Instance() as GameController;
        Root.Viewport.AddChild(newWorldScene);
        Root.SceneTransition.Fade();
        Root.Player.Camera.Position = new Vector2();

    }

    public void SaveWorld()
    {
        SetOwners(CurrentWorld);

        // Create resource
        var newPackedScene = new PackedScene();
        newPackedScene.Pack(CurrentWorld);

        LoadedWorld.Add(CurrentWorld.Name, newPackedScene);
    }

    // This makes sure that everynode contained in the Currentlevel gets saved
    // in the packed scene level. See EnterDebugMode() for more.
    private void SetOwners(Node pNode)
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
