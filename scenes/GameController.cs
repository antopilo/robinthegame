using Godot;

public class GameController : Node2D
{
    [Export] int StartLevel = 0;
    public Player Player;
    public Level CurrentRoom;
    public bool ShowGrid = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetNode("Player") as Player;
        CurrentRoom = (Level)GetNode("0");
        Spawn(false);
        MoveCamToRoom(CurrentRoom);
    }

    // Called Every frame.
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        UpdateRoom();
    }

    private void UpdateRoom()
    {
        Level Room;
        // Loops through each Room in the world. They decide which one is the Player in.
        foreach (Node node in GetChildren())
        {
            if (node.IsInGroup("level"))
            {
                Room = (Level)node;
                float x = Player.Position.x;
                float y = Player.Position.y;
                float xMin = ((Level)node).LevelPosition.x;
                float yMin = ((Level)node).LevelPosition.y;
                float xMax = ((Level)node).LevelPosition.x + ((Level)node).LevelSize.x;
                float yMax = ((Level)node).LevelPosition.y + ((Level)node).LevelSize.y;

                // If the Player is inside the level.
                if ((x > xMin) && (y > yMin) && (x < xMax) && (y < yMax) && CurrentRoom != node)
                {
                    CurrentRoom.ResetSpawns();
                    CurrentRoom = (Level)node;
                    CurrentRoom.ChooseSpawn();
                    MoveCamToRoom(CurrentRoom);
                }
            }
        }
    }

    private void MoveCamToRoom(Level pRoom)
    {
        // This function smoothly moves the camera to a specified pRoom.
        Tween T;

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
        Vector2 NewCameraZoom = new Vector2(pRoom.LevelZoom, pRoom.LevelZoom);

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

        if (Player.ArrowExist)
            Player.Arrow.ReturnToPlayer();
    }

    public override void _Draw()
    {
        if(ShowGrid == true)
		{
			DrawSetTransform(new Vector2(), 0, new Vector2(8, 8)); // Tile size is 8.
            for (int x = 0; x < 1000; x++)
            {
                DrawLine(new Vector2(x, 0), new Vector2(x, 1000), new Color(1, 0, 0), 1.0f);
                DrawLine(new Vector2(0, x), new Vector2(1000, x), new Color(1, 0, 0), 1.0f);
            }
		}  
    } 

    public void Spawn(bool WithAnimation)
    {
        Tween T;
        SceneTransition TransitionPlayer = (SceneTransition)GetNode("../../../Overlay");

        if (WithAnimation)
            TransitionPlayer.Fade();

        if (!HasNode("TweenSpawn"))
        {
            T = new Tween();
            T.Name = "TweenSpawn";
            AddChild(T);
            T.PauseMode = Node.PauseModeEnum.Process;
            T.Connect("tween_completed", this, "_on_Tween_tween_completed");
        }
        else
        {
            T = (Tween)GetNode("TweenSpawn");
        }

        T.RemoveAll();

        Vector2 StartPosition = Player.Position;
        Vector2 EndPosition = CurrentRoom.SpawnPosition;
        T.InterpolateProperty(Player, "position", StartPosition, EndPosition, 0.4f, Tween.TransitionType.Expo, Tween.EaseType.Out);
        T.Start();

        Player.CollisionBox.Disabled = true;
        Player.CanControl = false;
        Player.Sprite.Play("idle");

        MoveCamToRoom(CurrentRoom);
    }

    public void _on_Tween_tween_completed(Godot.Object @object, KeyList @key)
    {
        GetTree().Paused = false;
        Player.CanControl = true;
        Player.CollisionBox.Disabled = false;
    }

    public void ChangeRoom(Level pRoom)
    {
        CurrentRoom = pRoom;
        Spawn(false);
        MoveCamToRoom(CurrentRoom);
    }
}