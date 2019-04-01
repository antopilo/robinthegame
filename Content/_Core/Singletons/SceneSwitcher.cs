using Godot;
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

    // Load a world a place the player in it to a specified Waypoint
    // NOTE: Waypoint a placed under Waypoint/NameofWaypoint along with levels into a world.
    public void ChangeWorld(PackedScene pWorld, string Waypoint)
    {
        // Delete old world.
        Root.Viewport.GetChild(0).QueueFree();

        // Add new World.
        GameController newWorldScene = pWorld.Instance() as GameController;
        Root.Viewport.AddChild(newWorldScene);

        // Transition
        Root.SceneTransition.Fade();

        if (Waypoint != "" && newWorldScene.HasNode("Waypoint/" + Waypoint))
        {
           (newWorldScene.GetNode("Player") as Player).GlobalPosition = (newWorldScene.GetNode("Waypoint/" + Waypoint) as Position2D).GlobalPosition;
            newWorldScene.SnapCamToRoom(newWorldScene.FindRoom());
            newWorldScene.CurrentRoom = newWorldScene.FindRoom();
        }
    }


    // Save the current wolrd into its own resource.
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
