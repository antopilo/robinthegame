using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Console : Control
{
    private Root root;

    private Label FpsLabel;
    private LineEdit ConsoleInput;
    private string WorldPath = "res://Content/Areas/Worlds/";
    private Dialog DialogBox;
    private static RichTextLabel ConsoleBox;


    private string LastCommand = "";
    private string[] Commands = { "QUIT", "SHOWGRID", "SPAWN | RESPAWN", "CONTROLLER", "WINDOW",
                                  "FULLSCREEN", "VSYNC", "BORDERLESS", "SAY", "TP",
                                  "MOVE", "HELP", "SHAKE", "LOAD", "LW", "GHOST" , "RELOAD" , "HELP", "CLEAR" };
    private List<string> cache = new List<string>();

    public override void _Ready()
    {
        // Get Node references.
        root = GetNode("../..") as Root;

        FpsLabel = GetNode("Fps") as Label;
        ConsoleInput = GetNode("LineEdit") as LineEdit;
        DialogBox = GetNode("../Dialog") as Dialog;
        ConsoleBox = GetNode("ConsoleBox") as RichTextLabel;
    }

    public override void _Input(InputEvent @event)
    {
        // Toggle to Show or Hide the console.
        if (@event.IsActionReleased("console"))
        {
            Visible = !Visible;

            if (Visible) // Show
            {
                ConsoleInput.GrabFocus();
                ConsoleInput.Clear();
                Root.Player.CanControl = false;
            }
            else // Hide
            {
                ConsoleInput.ReleaseFocus();
                ConsoleInput.Clear();
                Root.Player.CanControl = true;
            }
        }

        // Press up for the previous command.
        if (@event.IsActionPressed("ui_up"))
            ConsoleInput.Text = LastCommand;

        if (@event.IsActionPressed("Respawn") && !Visible)
            Root.GameController.Spawn(true);

    }

    public override void _Process(float delta)
    {
        FpsLabel.Text = Engine.GetFramesPerSecond().ToString();
    }

    // Event for when entering a new command.
    private void _on_LineEdit_text_entered(String new_text)
    {
        bool Toggle;

        // Parse the command.
        var input = new_text.ToUpper().Split(" ");
        var command = input[0];
        var parameters = new_text.Split(" ").Skip(1).ToArray();

        LastCommand = new_text;
        ConsoleInput.Clear();
        ConsoleBox.BbcodeText += "\n " + new_text + "\n";
        switch (command)
        {
            case "QUIT":
                GetTree().Quit();
                break;
            case "SHOWGRID":
                Root.GameController.ShowGrid = !Root.GameController.ShowGrid;
                Root.Dialog.ShowMessage("GRID: " + Root.GameController.ShowGrid, 2f);
                Root.GameController.CurrentRoom.Update();
                break;
            case "LW":
                if(parameters.Length == 0)
                {
                    List<string> worlds = new List<string>();
                    Directory folder = new Directory();
                    folder.Open(WorldPath);
                    folder.ListDirBegin();
                    while (true)
                    {
                        var file = folder.GetNext();
                        if (file == "")
                            break;
                        else
                            worlds.Add(file);
                    }

                    cache.Clear();
                    ConsoleBox.BbcodeText += "List of available worlds: \n";
                    int idx = 0;
                    foreach (var world in worlds)
                    {
                        if (world == "." || world == "..")
                            continue;
                        ConsoleBox.BbcodeText += idx + ". " + world + "\n";
                        cache.Add(world);
                        idx++;
                    }
                }
                else
                {
                    var scene = ResourceLoader.Load(WorldPath + cache[int.Parse(parameters[0].ToString())]) as PackedScene;
                    if (scene == null)
                    {
                        ConsoleBox.BbcodeText += "[color=red] ERROR: " + cache[int.Parse(parameters[0].ToString())] + " couldn't be loaded.";
                        return;
                    }
                    Root.SceneSwitcher.ChangeWorld(scene, "");
                }
                
                break;
            case "LOAD":
                if (parameters.Length == 0)
                {
                    ConsoleBox.BbcodeText += "[color=red] Syntax: load [worldname] [waypoint]";
                    return;
                }
                var worldscene = ResourceLoader.Load(WorldPath + parameters[0] + ".tscn") as PackedScene;
                if(worldscene == null)
                {
                    ConsoleBox.BbcodeText += "[color=red] ERROR: " + parameters[0] + " couldn't be loaded.";
                    return;
                }
                if (parameters.Length == 1)
                    Root.SceneSwitcher.ChangeWorld(worldscene , "");
                else
                    Root.SceneSwitcher.ChangeWorld(worldscene, parameters[1]);

                Visible = false;
                ConsoleInput.ReleaseFocus();
                break;
            case "SC":
            case "SCREENSHOT":
                Root.Screenshot();
                break;
            case "LEVELS":
                cache.Clear();
                ConsoleBox.BbcodeText += "List of available worlds: \n";
                int i = 0;
                foreach (var level in Root.GameController.GetChildren())
                {
                    if (!(level is Level))
                        continue;
                    ConsoleBox.BbcodeText += i + ". " + (level as Level).Name + "\n";
                    i++;
                }
                break;
            case "WP":
            case "WAYPOINT":
                if(parameters.Length == 0)
                {
                    ConsoleBox.BbcodeText += "Here are all the waypoints found: \n";
                    var num = 0;
                    foreach (Node2D wp in Root.GameController.GetNode("Waypoint").GetChildren())
                    {
                        ConsoleBox.BbcodeText += num + ". " + wp.Name + "\n";
                    }
                    break;
                }
                else
                {
                    if(!Root.GameController.HasNode("Waypoint/" + parameters[0]))
                    {
                        ConsoleBox.BbcodeText += "ERROR: Can't find waypoint " + parameters[0];
                        break;
                    }
                    Root.Player.GlobalPosition = ((Position2D)Root.GameController.GetNode("Waypoint/" + parameters[0])).GlobalPosition;
                }
                break;
            case "SPAWN":
            case "RESPAWN":
                Root.Dialog.ShowMessage("Respawned", 2f);
                Root.GameController.Spawn(true);
                break;
            case "GHOST":
                if(Root.Player.State == States.Ghost)
                {
                    Root.Player.State = States.Air;
                    Root.GameController.ChangeRoom(Root.GameController.FindRoom(Root.Player.GlobalPosition));
                    Root.Player.Camera.GlobalPosition = Root.Player.GlobalPosition;
                    Root.Dialog.ShowMessage("Ghost mode deactivated", 2f);
                    Root.Player.Alive = true;
                } 
                else
                {
                    Root.Dialog.ShowMessage("Ghost mode activated", 2f);
                    Root.Player.Camera.GlobalPosition = Root.Player.GlobalPosition;
                    Root.Player.State = States.Ghost;
                }
                    
                break;
            // Toggle Controller mode ON or OFF
            case "CONTROLLER": 
                if(parameters.Length == 0)
                    Root.Weapon.ControllerMode = !Root.Weapon.ControllerMode;
                else if(parameters[0] == "1")
                    Root.Weapon.ControllerMode = true;
                else if(parameters[0] == "0")
                    Root.Weapon.ControllerMode = false;
                break;
            // Set the Window Size.
            case "WINDOW":
                if (parameters.Length == 0)
                {
                    root.settings.Resolution = new Vector2(320, 180);
                    root.ApplySettings();
                    break;
                }
                    
                switch (parameters[0])
                {
                    case "0":
                    case "1":
                        root.settings.Resolution = new Vector2(320, 180);
                        break;
                    case "2":
                        root.settings.Resolution = new Vector2(640, 360);
                        break;
                    case "3":
                        root.settings.Resolution = new Vector2(1280, 720);
                        break;
                    case "4":
                        root.settings.Resolution = new Vector2(1920, 900);
                        OS.WindowPosition = new Vector2();
                        break;
					case "5":
                        root.settings.Resolution = new Vector2(1920, 1080);
                        OS.WindowPosition = new Vector2();
                        break;
                }
                root.ApplySettings();
                break;
            case "CLEAR":
                ConsoleBox.BbcodeText = "";
                break;
            // Fullscreen
            case "FULLSCREEN":
                if (parameters.Length == 0)
                    root.settings.Fullscreen = !root.settings.Fullscreen;

                if (parameters[0] == "1")
                    root.settings.Fullscreen = true;
                else
                    root.settings.Fullscreen = false;

                root.ApplySettings();
                break;
            case "SHAKE":
                if (parameters.Length == 0)
                    (Root.Player.Camera as Camera).Shake(1f, 1f);
                try
                {
                    if (parameters.Length == 1)
                        (Root.Player.Camera as Camera).Shake(float.Parse(parameters[0]), 1f);
                    else if (parameters.Length == 2)
                        (Root.Player.Camera as Camera).Shake(float.Parse(parameters[0]), float.Parse(parameters[1]));
                                   }
                catch
                {
                    ConsoleBox.BbcodeText += "\n [color=red]The shake command must have 1 or 2 parameters. \n Shake [Amount] [Duration]";
                }
                break;

            // Toggle Vsync usage.
            case "VSYNC":
                if (parameters[0] == "0")
                    root.settings.Vsync = false;
                else
                    root.settings.Vsync = true;
                root.ApplySettings();
                break;
            case "MAXFPS":
                if (parameters.Length == 0)
                    ConsoleBox.BbcodeText += "\n [color=red]Must specify a value.[/color]";

                root.settings.MaxFps = parameters[0].ToInt();
                root.ApplySettings();
                Root.Dialog.ShowMessage("Max fps set to " + root.settings.MaxFps, 2f);
                break;
            
            // Make the borderless
            case "BORDERLESS":
                if(parameters.Length == 0)
                {
                    root.settings.Borderless = !root.settings.Borderless;
                    OS.WindowPosition = new Vector2(0, 0);
                }
                else if (parameters[0] == "1")
                {
                    root.settings.Borderless = true;
                    root.settings.Fullscreen = false;
                    root.settings.Resolution = OS.GetScreenSize();
                    OS.WindowPosition = new Vector2(0, 0);
                }
                else
                {
                    root.settings.Borderless = false;
                    OS.WindowPosition = new Vector2(0, 0);
                }

                root.ApplySettings();
                break;

            // Say command
            case "SAY":
                DialogBox.ShowMessage(new_text.Right(4));
                break;

            // Teleport the player to a specified room.
            case "TP":
                if (parameters.Length <= 0)
                    return;

                string DestinationLevel = parameters[0].ToLower();
                Level Level = Root.GameController.GetNode(DestinationLevel) as Level;
                if(Level != null && Level is Level)
                {
                    Root.GameController.ChangeRoom(Level);
                    Root.Dialog.ShowMessage("Teleported to " + Level.Name, 2f);
                    Root.Player.GlobalPosition = Level.GlobalPosition;
                    Level.ChooseSpawn();
                    Root.GameController.Spawn(false);
                }
                break;

            case "RELOAD":
                Root.GameController.CurrentRoom.Reload();
                Root.Dialog.ShowMessage("Level reloaded", 2f);
                break;
            case "SETSPAWN":
                Root.GameController.CurrentRoom.SpawnPosition = Root.Player.Position;
                Root.Dialog.ShowMessage("Spawn set at " + Root.GameController.CurrentRoom.SpawnPosition, 2f);
                break;
            // Move the player X Y Tile.
            case "MOVE":
                if (parameters.Length == 0)
                    return;
                if (parameters.Length == 2)
                {
                    var y = parameters[1].ToFloat();
                    Root.Player.MoveLocalY(y * 8);
                    DialogBox.ShowMessage("Moved player Y: " + y + " tiles ");
                }
                else if (parameters.Length > 2)
                {
                    ConsoleBox.BbcodeText += "\n [color=red]The move Command must follow this pattern: move X Y[/color]";
                    return;
                }

                var x = parameters[0].ToFloat();

                Root.Player.MoveLocalX(x * 8);
                DialogBox.ShowMessage("Moved player X: " + x + " tiles ");
                break;
            case "ADDITEM":
                InventoryManager.AddItem("Coal", 1);
                break;
            case "HELP":
                ConsoleBox.BbcodeText += "\n [color=red]Here is the list of commands that are available: [/color]";

                foreach (string cmd in Commands)
                    ConsoleBox.BbcodeText += "\n " + "[color=red]" + cmd + "[/color]";
                break;

            default:
                ConsoleBox.BbcodeText += "\n " + "[color=red]Unknown Command. Type : help for the list of commands.[/color]";
                return;
        }

        
    }

    public static void Log(string message)
    {
        ConsoleBox.BbcodeText += message + "\n";
    }
}


