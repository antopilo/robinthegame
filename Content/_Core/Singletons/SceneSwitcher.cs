using Godot;
using System.Collections.Generic;

public class SceneSwitcher : Node
{
    public static GameController CurrentWorld;
    public static Dictionary<string, PackedScene> LoadedWorld = new Dictionary<string, PackedScene>();
    public static PackedScene LastWorld;

    public bool ChangingScene = false;

    private PackedScene QueuedWorld;
    private string QueuedWaypoint = "";

    public override void _Ready()
    {
        CurrentWorld = Root.GameController;
    }

    public override void _Process(float delta)
    {
        if(ChangingScene && Root.SceneTransition.SceneChangeReady)
        {
            ChangingScene = false;
            Root.SceneTransition.SceneChangeReady = false;

            // Delete old world.
            Root.Viewport.GetChild(0).QueueFree();

            // Add new World.
            var newWorldScene = (GameController)QueuedWorld.Instance();
            var viewport = Root.Viewport;
            viewport.AddChild(newWorldScene);

            // If a specified waypoint is passed. Teleport the player there.
            if (QueuedWaypoint != "" && newWorldScene.HasNode("Waypoint/" + QueuedWaypoint))
            {
                // Setting player at waypoint.
                Position2D waypoint = (Position2D)newWorldScene.GetNode("Waypoint/" + QueuedWaypoint);
                newWorldScene.Player.GlobalPosition = waypoint.GlobalPosition;

                // Check to see if the waypoint is located in a room.
                if(newWorldScene.FindRoom(waypoint.GlobalPosition) != null)
                {
                    var newRoom = newWorldScene.FindRoom(waypoint.GlobalPosition);
                    newWorldScene.SnapCamToRoom(newRoom);
                    newWorldScene.CurrentRoom = newRoom;
                }
            }
            else
            {
                if(newWorldScene.HasNode("Waypoint") && newWorldScene.GetNode("Waypoint").GetChildCount() >= 0)
                {
                    var destination = (newWorldScene.GetNode("Waypoint").GetChild(0) as Node2D).GlobalPosition;
                    (newWorldScene.GetNode("Player") as Player).GlobalPosition = destination;
                    if (newWorldScene.FindRoom(destination) != null)
                    {
                        var newRoom = newWorldScene.FindRoom(destination);
                        newWorldScene.SnapCamToRoom(newRoom);
                        newWorldScene.CurrentRoom = newRoom;
                    }
                }
                else
                {
                    newWorldScene.Spawn(false);
                } 
            }
        }
    }


    // Load a world a place the player in it to a specified Waypoint
    // NOTE: Waypoint are placed under "Waypoints/<NameofWaypoint>" along with levels into a world.
    public void ChangeWorld(PackedScene pWorld, string Waypoint)
    {
        if (pWorld is null)
            return;

        Root.Player.CanControl = false;
        QueuedWorld = pWorld;
        QueuedWaypoint = Waypoint;
        Root.SceneTransition.FadeIn();
        ChangingScene = true;
    }


    #region SavingWorld
    // Save the current world into its own resource.
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
    #endregion
}
