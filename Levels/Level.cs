using Godot;
using System;
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
        //AutoTileBorders();
    }

    /// <summary>
    /// This methods Change each tiles that are on the border of the level to autotile with the level next to it.
    /// This gives a better look and makes the levels feel more connected.
    /// I KNOW THIS IS VERY POOR CODE BUT IT WORKS. - thats for john 
    /// 
    ///     x   x   x
    ///     x   x   x
    ///     x   x   x 
    /// </summary>
    public void AutoTileBorders()
    {
        Random rnd = new Random();
        foreach (Vector2 Tile in LayerSolid.GetUsedCells())
        {
            if (Tile.y == 0 || Tile.x == 0 || Tile.x == LevelRect.x - 1 || Tile.y == LevelRect.y - 1)
            {
                bool right = false;
                bool left = false;
                bool bottom = false;
                bool top = false;

                // Check if there is a tile next to the current one. Checks all side.
                if (Tile.x != LevelRect.x - 1 && LayerSolid.GetCell((int)Tile.x + 1, (int)Tile.y) == -1)
                    right = true;
                if (Tile.x != 0 && LayerSolid.GetCell((int)Tile.x - 1, (int)Tile.y) == -1)
                    left = true;
                if (Tile.y != LevelRect.y - 1 && LayerSolid.GetCell((int)Tile.x, (int)Tile.y + 1) == -1)
                    bottom = true;
                if (Tile.y != 0 && LayerSolid.GetCell((int)Tile.x, (int)Tile.y - 1) == -1)
                    top = true;

                var autoTiling = new Vector2();

                if (!right && left && !bottom && top)       // Top left
                    autoTiling.y = 0;
                else if (!right && !left && !bottom && top) // Top
                    autoTiling.y = 1;
                else if (right && !left && !bottom && top)  // Top Right
                    autoTiling.y = 2;
                else if (!right && left && !bottom && !top) // Middle left
                    autoTiling.y = 3;
                else if (!right && !left && !bottom && !top)// Middle
                    autoTiling.y = 4;
                else if (right && !left && !bottom && !top) // Middle right
                    autoTiling.y = 5;
                else if (!right && left && bottom && !top)  // Bottom left
                    autoTiling.y = 6;
                else if (!right && !left && bottom && !top) // Bottom 
                    autoTiling.y = 7;
                else if (right && !left && bottom && !top)  // Bottom right
                    autoTiling.y = 8;

                int variation = rnd.Next(4); // Random variation of the tile.
                
                autoTiling.x = variation;

                LayerSolid.SetCell((int)Tile.x, (int)Tile.y, LayerSolid.GetCellv(Tile), 
                            false, false, false, autoTiling);
            }
        }
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
