
using Godot;

public class GameController : Node2D
{
    public Level CurrentRoom;
    public bool ShowGrid = false;

    private Player m_player;
    private LevelInfo LevelInfo;

    private bool IsSpawning = false;
    private Tween Tween;

    [Export] private string StartLevel = "";
    [Export] private Color BackgroundColor;

    public Player Player
    {
        get
        {
            if (m_player == null)
                m_player = (Player)GetNode("Player");

            return m_player;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LevelInfo = (LevelInfo)GetNode("../../../UI/LevelInfo");
        m_player = (Player)GetNode("Player");
        Root.Player = Player;

        if (BackgroundColor != null)
            VisualServer.SetDefaultClearColor(BackgroundColor);

        SnapCamToRoom(GetNode(StartLevel) as Level);
        ChangeRoom(GetNode(StartLevel) as Level);
        CurrentRoom.ChooseSpawn();
        Player.GlobalPosition = CurrentRoom.SpawnPosition;
        Player.Camera.ResetSmoothing();
        Tween = new Tween();
        Tween.Name = "CameraAreaTween";
        AddChild(Tween);
        Tween.Connect("tween_completed", this, "_on_Tween_tween_completed");

        Tween T;
            if (!HasNode("Tween"))
            {
                T = new Tween();
                T.Name = "Tween";
                AddChild(T);
                
            }
            else
            {
                T = (Tween)GetNode("Tween");
            }
            T.RemoveAll();
            T.InterpolateProperty(Player, "scale", new Vector2(0, 0), new Vector2(1, 1), 0.25f,
                Tween.TransitionType.Expo, Tween.EaseType.In);
            T.Start();
    }

    // Called Every frame.
    public override void _PhysicsProcess(float delta)
    {
        if(IsSpawning && Root.SceneTransition.SceneChangeReady)
        {
            IsSpawning = false;
            Root.Player.Alive = true;
            Root.SceneTransition.SceneChangeReady = false;
            Root.SceneTransition.MoveToPlayer();
            Tween T;
            if (!HasNode("Tween"))
            {
                T = new Tween();
                T.Name = "Tween";
                AddChild(T);
            }
            else
            {
                T = (Tween)GetNode("Tween");
            }
            T.RemoveAll();
            T.InterpolateProperty(Player, "scale", new Vector2(0, 0), new Vector2(1, 1), 0.25f,
                Tween.TransitionType.Expo, Tween.EaseType.In);
            T.Start();
            CurrentRoom.Reload();
            Player.SetPhysicsProcess(true);
            
            Root.InputRecorder.SetProcess(true);
        }

        if (Root.Player.State != States.Ghost)
            UpdateRoom();
    }
       

    /// <summary>
    /// Loops through each level(Node that have the "level" tag), and determine in which room
    /// is the player located.
    /// </summary>
    private void UpdateRoom()
    {
        if (!Player.Alive)
            return;
        Level Room;

        // Kill the player if he can die from falling
        if (CurrentRoom.FallKill && Player.Position.y > CurrentRoom.LevelSize.y + CurrentRoom.LevelPosition.y)
            Root.Player.Spawn(true);

        // Loops through each Room in the world. They decide which one is the Player in.
        foreach (Node level in GetChildren())
        {
            if (level is Level)
            {
                Room = (Level)level;

                float x = Player.Position.x, y = Player.Position.y;
                float xMin = Room.LevelPosition.x, yMin = Room.LevelPosition.y;
                float xMax = xMin + Room.LevelSize.x, yMax = yMin + Room.LevelSize.y;

                // If the Player is inside the level.
                if ((x >= xMin) && (y >= yMin) && (x < xMax) && (y < yMax) && CurrentRoom != level)
                {   
                    // if the player enters a level from under, do a jump to gain a little bit of height.
                    if (Root.Player.Alive && y <= CurrentRoom.LevelPosition.y)
                        Root.Player.Jump();
                    ChangeRoom(Room);
                }
            }
        }
    }

    // Finds which room the player is in.
    public Level FindRoom(Vector2 pPosition)
    {
        Level Room;

        // Loops through each Room in the world. They decide which one is the Player in.
        foreach (Node level in GetChildren())
        {
            if (level is Level)
            {
                Room = (Level)level;
                float x = pPosition.x, y = pPosition.y;
                float xMin = Room.LevelPosition.x, yMin = Room.LevelPosition.y;
                float xMax = xMin + Room.LevelSize.x, yMax = yMin + Room.LevelSize.y;
                // If the Player is inside the level.
                if ((x >= xMin) && (y >= yMin) && (x < xMax) && (y < yMax))
                    return (Level)level;
            }
        }
        // Return if the player is not inside a level.
        return null;
    }

    // Instantly snap the camera to a specified room.
    public void SnapCamToRoom(Level pRoom)
    {
        Tween T;  // If there is no Tween node, then create one and use it.
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

        Camera2D Camera = Root.Player.Camera;
        Camera.LimitLeft = (int)pRoom.GlobalPosition.x;
        Camera.LimitRight = (int)pRoom.GlobalPosition.x + (int)pRoom.LevelSize.x;
        Camera.LimitTop = (int)pRoom.GlobalPosition.y;
        Camera.LimitBottom = (int)pRoom.GlobalPosition.y + (int)pRoom.LevelSize.y;

        Root.Player.CanControl = true;
        Root.Player.Alive = true;
        Root.Player.SetPhysicsProcess(true);
        if(Root.InputRecorder != null)
            Root.InputRecorder.SetProcess(true);
    }

    /// <summary>
    /// Tween the camera to a new Level. Setting the limits of the camera to the level size.
    /// </summary>
    /// <param name="pRoom">Target level.</param>
    private void MoveCamToRoom(Level pRoom)
    {
        // Pausing the player.
        Player.SetPhysicsProcess(false);
        if(Root.InputRecorder != null)
            Root.InputRecorder.SetProcess(false);
        Tween T;  // If there is no Tween node, then create one and use it.
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

        Vector2 NewCameraZoom = new Vector2(pRoom.LevelZoom, pRoom.LevelZoom); // Levels can have custom zoom.
        Camera2D Camera = Player.Camera;
        Vector2 CameraCenter = Camera.GetCameraScreenCenter();

        // Setting the limit of the camera to the size of the level.
        float NewLimitLeft = pRoom.GlobalPosition.x;
        float NewLimitRight = pRoom.GlobalPosition.x + pRoom.LevelSize.x;
        float NewLimitTop = pRoom.GlobalPosition.y;
        float NewLimitBottom = pRoom.GlobalPosition.y + pRoom.LevelSize.y;

        Camera.LimitRight = (int)(CameraCenter.x + GetViewport().Size.x / 2 * Camera.Zoom.x);
        Camera.LimitLeft = (int)(CameraCenter.x - GetViewport().Size.x / 2 * Camera.Zoom.x);
        Camera.LimitTop = (int)(CameraCenter.y - GetViewport().Size.y / 2 * Camera.Zoom.y);
        Camera.LimitBottom = (int)(CameraCenter.y + GetViewport().Size.y / 2 * Camera.Zoom.y);

        // Transition settings.
        float Time = 0.5f;
        Tween.TransitionType Transition = Tween.TransitionType.Expo;
        Tween.EaseType Ease = Tween.EaseType.Out;

        T.InterpolateProperty(Camera, "limit_right", Camera.LimitRight, NewLimitRight, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "limit_left", Camera.LimitLeft, NewLimitLeft, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "limit_top", Camera.LimitTop, NewLimitTop, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "limit_bottom", Camera.LimitBottom, NewLimitBottom, Time, Transition, Ease);
        T.InterpolateProperty(Camera, "zoom", Camera.Zoom, NewCameraZoom, 0.6f, Transition, Ease);
        T.Start();

        // If there is an arrow in the level return it to Robin.
        if (Player.Arrow != null)
            Player.Arrow.ReturnToPlayer();
    }


    /// <summary>
    /// The spawn is currently half done. For now Spawning the player means moving smoothly
    /// the player from his current position to the closest spawn in the current level.
    /// </summary>
    /// <param name="WithAnimation">If the transition animation is played or not.</param>
    public void Spawn(bool WithAnimation)
    {
        if (!Root.Player.Alive)
            return;

        Root.Player.Alive = false;
        Root.Player.CanControl = false;
        Root.DeathCount.Deaths++;
        
        if (WithAnimation)
        {
            Root.SceneTransition.FadeIn();

            Tween T;  // If there is no Tween node, then create one and use it.
            if (!HasNode("Tween"))
            {
                T = new Tween();
                T.Name = "Tween";
                AddChild(T);

            }
            else
            {
                T = (Tween)GetNode("Tween");
            }
            T.RemoveAll();
            T.InterpolateProperty(Player, "scale", new Vector2(1, 1), new Vector2(0, 0), 0.2f,
                Tween.TransitionType.Expo, Tween.EaseType.In, 0.4f);
            T.Start();
            Player.SetPhysicsProcess(false);
            Root.InputRecorder.SetProcess(false);
        }
            

        IsSpawning = true;

        if (CurrentRoom.SpawnPosition == new Vector2())
            CurrentRoom.ChooseSpawn();

        Root.SceneTransition.MoveToPlayer();
    }


    public void _on_Tween_tween_completed(Godot.Object @object, KeyList @key)
    {
        Root.Player.SetPhysicsProcess(true);
        
        Root.InputRecorder.SetProcess(true);
        Root.Player.CanControl = true;
        Root.Player.Alive = true;
    }


    public void ChangeRoom(Level pRoom)
    {
        if (pRoom is null)
            return;

        CurrentRoom = pRoom;
        CurrentRoom.ChooseSpawn();
        Player.Sprite.Playing = false;
        CurrentRoom.Reload();
        MoveCamToRoom(CurrentRoom);
        LevelInfo.UpdateInfo(CurrentRoom);
    }


    /// <summary>
    /// Checks if the player has pItem in his inventory.
    /// </summary>
    /// <param name="pItem">Specify an item</param>
    /// <returns></returns>
    public bool PlayerHas(Node2D pItem)
    {
        foreach (var item in Root.Player.Following)
            if (item == pItem)
                return true;
        return false;
    }
	
	
	private void _on_AnimationPlayer_animation_finished(string anim_name)
	{
        GD.Print("Transition finished");
        Root.Player.Alive = true;
        Root.Player.CanControl = true;
        Player.Sprite.Playing = true;
        
	}
}



