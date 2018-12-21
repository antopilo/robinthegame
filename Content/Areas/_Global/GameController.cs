using Godot;

public class GameController : Node2D
{
    [Export] string StartLevel = "0";

    public LevelInfo LevelInfo;
    public Root Root;
    public Player Player;
    public Level CurrentRoom;
    public Dialog DialogController;
    public DeathCount DeathCounter;

    public bool ShowGrid = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Root = GetNode("../../..") as Root;
        Player = GetNode("Player") as Player;
        CurrentRoom = GetNode(StartLevel) as Level;

        //UI
        LevelInfo = GetNode("../../../UI/LevelInfo") as LevelInfo;
        DialogController = GetNode("../../../UI/Dialog") as Dialog;
        DeathCounter = GetNode("../../../UI/DeathCount") as DeathCount;

        Spawn(false);
        MoveCamToRoom(CurrentRoom);
    }

    // Called Every frame.
    public override void _PhysicsProcess(float delta)
        => UpdateRoom();

    /// <summary>
    /// Loops through each level(Node that have the "level" tag), and determine in which room
    /// is the player located.
    /// </summary>
    private void UpdateRoom()
    {
        Level Room;

        // Loops through each Room in the world. They decide which one is the Player in.
        foreach (Node level in GetChildren())
        {
            if (level.IsInGroup("level"))
            {
                Room = (Level)level;

                float x = Player.Position.x;
                float y = Player.Position.y;
                float xMin = Room.LevelPosition.x;
                float yMin = Room.LevelPosition.y;
                float xMax = Room.LevelPosition.x + Room.LevelSize.x;
                float yMax = Room.LevelPosition.y + Room.LevelSize.y;

                // If the Player is inside the level.
                if ((x > xMin) && (y > yMin) && (x < xMax) && (y < yMax) && CurrentRoom != level)
                {
                    if (y <= CurrentRoom.LevelPosition.y)
                        Player.Jump();

                    Level oldLevel = CurrentRoom;
                    oldLevel.Unload();

                    CurrentRoom = Room;
                    CurrentRoom.Load();
                    MoveCamToRoom(CurrentRoom);
                    LevelInfo.UpdateInfo(CurrentRoom);
                    GD.Print("STATUS: PLAYER ENTERED ROOM : " + CurrentRoom.Name);
                }
            }
        }
    }

    private void MoveCamToRoom(Level pRoom)
    {
        // This function smoothly moves the camera to a specified pRoom.
        Player.SetPhysicsProcess(false);

        Tween T;

        // If there is no Tween node, then create one and use it.
        if (!HasNode("CameraAreaTween"))
        {
            T = new Tween();
            T.Name = "CameraAreaTween";
            AddChild(T);

            T.Connect("tween_completed", this, "_on_Tween_tween_completed");
        }
        else
        {
            T = (Tween)GetNode("CameraAreaTween");
        }

        T.RemoveAll();

        float NewLimitLeft = pRoom.GlobalPosition.x;
        float NewLimitRight = pRoom.GlobalPosition.x + pRoom.LevelSize.x;
        float NewLimitTop = pRoom.GlobalPosition.y;
        float NewLimitBottom = pRoom.GlobalPosition.y + pRoom.LevelSize.y;

        Vector2 NewCameraZoom = new Vector2(pRoom.LevelZoom, pRoom.LevelZoom); // Levels can have custom zoom.

        Camera2D Camera = Player.Camera;
        Vector2 CameraCenter = Camera.GetCameraScreenCenter();

        Camera.LimitRight = (int)(CameraCenter.x + GetViewport().Size.x / 2 * Camera.Zoom.x);
        Camera.LimitLeft = (int)(CameraCenter.x - GetViewport().Size.x / 2 * Camera.Zoom.x);
        Camera.LimitTop = (int)(CameraCenter.y - GetViewport().Size.y / 2 * Camera.Zoom.y);
        Camera.LimitBottom = (int)(CameraCenter.y + GetViewport().Size.y / 2 * Camera.Zoom.y);

        float Time = 0.4f;

        Tween.TransitionType Transition = Tween.TransitionType.Linear;
        Tween.EaseType Ease = Tween.EaseType.InOut;

        T.InterpolateProperty(Camera, "limit_right", Camera.LimitRight, NewLimitRight, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "limit_left", Camera.LimitLeft, NewLimitLeft, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "limit_top", Camera.LimitTop, NewLimitTop, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "limit_bottom", Camera.LimitBottom, NewLimitBottom, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "zoom", Camera.Zoom, NewCameraZoom, 0.6f, Transition, Ease);
        T.Start();

        if (Player.Arrow != null) // If an arrow was in the old room. Return it to the player.
            Player.Arrow.ReturnToPlayer();
    }

    /// <summary>
    /// The spawn is currently half done. For now Spawning the player means moving smoothly
    /// the player from his current position to the closest spawn in the current level.
    /// </summary>
    /// <param name="WithAnimation">If the transition animation is played or not.</param>
    public void Spawn(bool WithAnimation)
    {
        SceneTransition TransitionPlayer = (SceneTransition)GetNode("../../../UI");
        Player.Alive = false;
        DeathCounter.Deaths++;
        //CurrentRoom.Reload();

        if (WithAnimation)
            TransitionPlayer.Fade();

        if (CurrentRoom.SpawnPosition == new Vector2())
            CurrentRoom.ChooseSpawn();

        Player.GlobalPosition = CurrentRoom.SpawnPosition;
        Player.Alive = true;
    }

    public void _on_Tween_tween_completed(Godot.Object @object, KeyList @key)
    {
        Player.SetPhysicsProcess(true);
        Player.CanControl = true;
        Player.Alive = true;
    }

    public void ChangeRoom(Level pRoom)
    {
        CurrentRoom = pRoom;
        Spawn(false);
        MoveCamToRoom(CurrentRoom);
    }

    /// <summary>
    /// Checks if the player has pItem in his inventory.
    /// </summary>
    /// <param name="pItem">Specify an item</param>
    /// <returns></returns>
    public bool PlayerHas(Node2D pItem)
    {
        foreach (var item in Player.Following)
            if (item == pItem)
                return true;

        return false;
    }

}
