using Godot;
using System;
using System.Collections.Generic;

public class Level : Node2D
{
	// IF the level is smaller than the screen reset the size to those values.
    const int MIN_WIDTH = 320; 
    const int MIN_HEIGHT = 180;
	
	// Tiles are 8x8 pixels
    const int TileSize = 8;

	
    public Vector2 LevelRect; // True size
    public Vector2 LevelSize;  // Size to fit in the screen. half a tile shorter on the Y axis.
    public Vector2 LevelPosition; // Position in the World

    private Node Entities;
    private Player Player;

    [Export] public float LevelZoom = 1;

    public Vector2 SpawnPosition = new Vector2(); // Current spawn of the player.
    List<Vector2> Spawns = new List<Vector2>(); // List of possible spawns.

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

    GameController World;
    public override void _Ready()
    {

        World = GetNode("..") as GameController;

        // Get Layers
        LayerSolid = GetNode("fg_tile") as TileMap;
        LayerBackground = GetNode("bg_tile") as TileMap;
        LayerEntities = GetNode("entities") as TileMap;
        LayerFgDecals = GetNode("fg_decals") as TileMap;
        LayerBgDecals = GetNode("bg_decals") as TileMap;

        // Preload entities Scenes
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
        Entities = GetNode("objects");

        LevelRect = LayerSolid.GetUsedRect().Size;
        LevelSize = new Vector2(LevelRect.x * TileSize, (LevelRect.y - 0.5f) * TileSize);
        
        LevelPosition = GlobalPosition;
		
        LoadEntities();
        ChooseSpawn();
        AutoTileBorders();
    }

    /// <summary>
    /// This methods if for diplay debug grid. See Console.cs for more info.
    /// </summary>
    public override void _Draw()
    {
        // If ShowGrid is true, Display rectangle on each tile of current level.
        if (World.ShowGrid && World.CurrentRoom == this)
        {
            // Tiles
            foreach (Vector2 item in LayerSolid.GetUsedCells())
                DrawRect(new Rect2(LayerSolid.MapToWorld(item), new Vector2(8, 8)), new Color(1, 0, 0), false);

            // Entities. need fix
            foreach (Node2D node2D in Entities.GetChildren())
            {
                //DrawRect((Rect2)node2D.Get("Box"), new Color(1, 0, 0), false);
            }
        }

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
            // If the tile is not autotile. Which should always be.
            if (LayerSolid.TileSet.TileGetTileMode(LayerSolid.GetCellv(Tile)) != TileSet.TileMode.AutoTile)
                continue;

            if (Tile.x == 0 || Tile.y == 0 || Tile.x == LevelRect.x - 1 || Tile.y == LevelRect.y)
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

                autoTiling.x = rnd.Next(4); // Generate random variation of the same tile.(there is 4 version)

                LayerSolid.SetCell((int)Tile.x, (int)Tile.y, LayerSolid.GetCellv(Tile),
                            false, false, false, autoTiling); // Update tile.
            }
        }
    }

    /// <summary>
    /// This loads all the entities of replace the fake tiles placeholder with real
    /// Objects. It calls Place entities to place them.
    /// </summary>
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

    /// <summary>
    /// This methods Place a single Entity and add rotation if necessary.
    /// AntoPilo: dont loose time on this.
    /// </summary>
    /// <param name="pPosition"></param>
    /// <param name="pName"></param>
    /// <param name="pEntity"></param>
    public void PlaceEntity(Vector2 pPosition, string pName, PackedScene pEntity)
    {
        Node2D NewEntity = (Node2D)pEntity.Instance();
        NewEntity.Name = pName;
        Entities.AddChild(NewEntity); // Add Entitiy to the world.

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

    /// <summary>
    /// After placing all of the spawns. This Decides which spawn is the closest to the player.
    /// It set SpawnPosition to the closest one.
    /// </summary>
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

    /// <summary>
    /// Before leaving the levels. all of the spawns must be turned off.
    /// This is called by GameController.
    /// </summary>
    public void ResetSpawns()
    {
        foreach (Node node in Entities.GetChildren())
            if (node.IsInGroup("spawn"))
                (node as Spawn).Active = false;
    }
}
