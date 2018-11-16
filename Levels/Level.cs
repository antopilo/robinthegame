using Godot;
using System.Collections.Generic;

public class Level : Node2D
{
    const int MIN_WIDTH = 320;
    const int MIN_HEIGHT = 180;
    const int TileSize = 8;

    public Vector2 LevelRect;
    public Vector2 LevelSize;
    public Vector2 LevelPosition = new Vector2();
    private Node Entities;
    private Player Player;
    public float LevelZoom;

    public Vector2 SpawnPosition = new Vector2();
    List<Vector2> Spawns = new List<Vector2>();

    #region Entities
    private PackedScene FallingPlatform;
    private PackedScene Spike;
    private PackedScene JumpPad;
    private PackedScene Door;
    private PackedScene Key;
    private PackedScene Extender;
    private PackedScene Spawn;
    private PackedScene FallBlock;
    private PackedScene JumpThroughPlatform; 
    #endregion

    #region Layers
    TileMap LayerSolid;
    TileMap LayerBackground;
    TileMap LayerEntities;
    TileMap LayerFgDecals;
    TileMap LayerBgDecals;
    #endregion
    
    public override void _Ready()
    {
        // Get Layers
        LayerSolid = GetNode("fg_tile") as TileMap;
        LayerBackground = GetNode("bg_tile") as TileMap;
        LayerEntities = GetNode("entities") as TileMap;
        LayerFgDecals = GetNode("fg_decals") as TileMap;
        LayerBgDecals = GetNode("bg_decals") as TileMap;

        // Get Entities
        FallingPlatform = ResourceLoader.Load("res://Levels/level_assets/entities/falling_platforms/FallingPlatform3Wide.tscn") as PackedScene;
        Spike = ResourceLoader.Load("res://Levels/level_assets/entities/spikes/oSpike.tscn") as PackedScene;
        JumpPad = ResourceLoader.Load("res://Levels/level_assets/entities/jump_pad/oJumpPad.tscn") as PackedScene;
        Door = ResourceLoader.Load("res://Levels/level_assets/entities/door_and_key/oDoor.tscn") as PackedScene;
        Key = ResourceLoader.Load("res://Levels/level_assets/entities/door_and_key/oKey.tscn") as PackedScene;
        Extender = ResourceLoader.Load("res://Levels/level_assets/entities/extender/oExtender.tscn") as PackedScene;
        Spawn = ResourceLoader.Load("res://Levels/level_assets/entities/spawn/oSpawn.tscn") as PackedScene;
        FallBlock = ResourceLoader.Load("res://Levels/level_assets/entities/falling_platforms/FallingPlatform1.tscn") as PackedScene;
        JumpThroughPlatform = ResourceLoader.Load("res://Levels/level_assets/entities/jump_through_platform/fall_through_platform.tscn") as PackedScene;

        Player = GetNode("../Player") as Player;
        Entities = (Node)GetNode("objects");

        LevelRect = LayerSolid.GetUsedRect().Size;
        LevelSize = new Vector2(LevelRect.x * TileSize, (LevelRect.y - 0.5f) * TileSize);
        LevelPosition = GlobalPosition;
		LevelZoom = 1f;
		
        LoadEntities();
        ChooseSpawn();
    }

    public void LoadEntities()
    {
        foreach (Vector2 Tile in LayerEntities.GetUsedCells())
        {
            int Cell = LayerEntities.GetCell((int)Tile.x, (int)Tile.y);

            switch (Cell)
            {
                case 0:
                    PlaceEntity(Tile, "Door", Door);
                    break;
                case 1:
                    PlaceEntity(Tile, "Key", Key);
                    break;
                case 2:
                    PlaceEntity(Tile, "FallBlockSingle", FallBlock);
                    break;
                case 3:
                    PlaceEntity(Tile, "JumpThroughPlatform", JumpThroughPlatform);
                    break;
                case 4:
                    PlaceEntity(Tile, "Extender", Extender);
                    break;
                case 5:
                    PlaceEntity(Tile, "Spawn", Spawn);
                    break;
                case 6:
                    PlaceEntity(Tile, "JumpPad", JumpPad);
                    break;
                case 7:
                    PlaceEntity(Tile, "Spike", Spike);
                    break;
            }
        }
    }

    public void PlaceEntity(Vector2 pPosition, string pName, PackedScene pEntity)
    {
        Node2D NewEntity = (Node2D)pEntity.Instance();
        NewEntity.Name = pName;
        Entities.AddChild(NewEntity);

        int X = (int)pPosition.x;
        int Y = (int)pPosition.y;
        Vector2 NewPosition = new Vector2();
        bool Transposed = LayerEntities.IsCellTransposed(X, Y);

        // Handles the Rotation of the tile and Transpose it to the Ent.
        if(Transposed && !LayerEntities.IsCellXFlipped(X, Y))
        {
            NewPosition = new Vector2(X, Y + 1);
            (NewEntity).GlobalRotationDegrees= 270;
            ((Node2D)NewEntity).GlobalPosition = LayerEntities.MapToWorld(NewPosition) + LevelPosition;
        }
        else if(Transposed && LayerEntities.IsCellXFlipped(X, Y))
        {
            NewPosition = new Vector2(X + 1, Y);
            ((Node2D)NewEntity).GlobalRotationDegrees = 90;
            ((Node2D)NewEntity).GlobalPosition = LayerEntities.MapToWorld(NewPosition) + LevelPosition;
        }
        else if (!Transposed && LayerEntities.IsCellYFlipped(X, Y))
        {
            NewPosition = new Vector2(X + 1, Y + 1);
            ((Node2D)NewEntity).GlobalRotationDegrees = 180;
            ((Node2D)NewEntity).GlobalPosition = LayerEntities.MapToWorld(NewPosition) + LevelPosition;
        }
        else
        {
            ((Node2D)NewEntity).GlobalPosition = LayerEntities.MapToWorld(pPosition) + LevelPosition;
        }

        LayerEntities.SetCell(X, Y, -1); // Erase the Tile from the tilemap(it has been replaced with a real object).

        if (pName == "Spawn")
        {
            SpawnPosition = NewEntity.GlobalPosition + LevelPosition;
            Spawns.Add(SpawnPosition);
            ChooseSpawn();
        }
    }

    public void ChooseSpawn()
    {
        foreach (Node node in Entities.GetChildren())
        {
            if (node.IsInGroup("spawn"))
            {
                Spawn s = node as Spawn;
                s.Active = false;
                var distanceFromPlayer = Mathf.Abs((Player.GlobalPosition - s.GlobalPosition).Length());
                var currentFromPlayer = Mathf.Abs((Player.GlobalPosition - SpawnPosition).Length());

                if(distanceFromPlayer <= currentFromPlayer)
                {
                    SpawnPosition = s.GlobalPosition;
                    s.Active = true;
                }
            }
        }
    }

    public void ResetSpawns()
    {
        foreach (Node node in Entities.GetChildren())
            if (node.IsInGroup("spawn"))
                (node as Spawn).Active = false;
    }
}
