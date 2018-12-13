using Godot;
using System;
using System.Linq;

public class Console : Control
{
    private Root root;

    private Label FpsLabel;
    private LineEdit ConsoleInput;
    private GameController GameController;
    private Dialog DialogBox;
    private RichTextLabel ConsoleBox;
    private Player Player;
    private Weapon Weapon;

    private string LastCommand = "";
    private string[] Commands = { "QUIT", "SHOWGRID", "SPAWN", "CONTROLLER", "WINDOW",
                                  "FULLSCREEN", "VSYNC", "BORDERLESS", "SAY", "TP",
                                  "MOVE", "HELP" };
    
    public override void _Ready()
    {
        // Get Node references.
        root = GetNode("../..") as Root;

        FpsLabel = GetNode("Fps") as Label;
        ConsoleInput = GetNode("LineEdit") as LineEdit;
        GameController = GetNode("../../Game/Viewport/World") as GameController;
        DialogBox = GetNode("../Dialog") as Dialog;
        ConsoleBox = GetNode("ConsoleBox") as RichTextLabel;
        Player = GameController.GetNode("Player") as Player;
        Weapon = Player.GetNode("Weapon") as Weapon;
		Player.CanControl = true;
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
                Player.CanControl = false;
            }
            else // Hide
            {
                ConsoleInput.ReleaseFocus();
                ConsoleInput.Clear();
                Player.CanControl = true;
            }
        }

        // Press up for the previous command.
        if (@event.IsActionPressed("ui_up"))
            ConsoleInput.Text = LastCommand;

        if(@event.IsActionPressed("Reload"))
            GameController.CurrentRoom.Reload();

        if (@event.IsActionPressed("Respawn"))
            GameController.Spawn(true);

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
        var parameters = input.Skip(1).ToArray();

        LastCommand = new_text;
        ConsoleInput.Clear();

        switch (command)
        {
            case "QUIT":
                GetTree().Quit();
                break;

            case "SHOWGRID":
                GameController.ShowGrid = !GameController.ShowGrid;
                GameController.CurrentRoom.Update();
                break;

            case "SPAWN":
            case "RESPAWN":
                GameController.Spawn(true);
                break;

            // Toggle Controller mode ON or OFF
            case "CONTROLLER": 
                Toggle = !Weapon.ControllerMode;
                
                if(parameters.Length == 0)
                    Weapon.ControllerMode = !Weapon.ControllerMode;
                else if(parameters[0] == "1")
                    Weapon.ControllerMode = true;
                else if(parameters[0] == "0")
                    Weapon.ControllerMode = false;
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
                Level Level = GameController.GetNode(DestinationLevel) as Level;

                if(Level != null && Level.IsInGroup("level"))
                {
                   GameController.ChangeRoom(Level);
                    GameController.Spawn(true);
                }
                break;

            case "RELOAD":
                GameController.CurrentRoom.Reload();
                break;
            case "SETSPAWN":
                GameController.CurrentRoom.SpawnPosition = GameController.Player.Position;
               
                break;
            // Move the player X Y Tile.
            case "MOVE":

                if (parameters.Length == 0)
                    return;
                if (parameters.Length == 2)
                {
                    var y = parameters[1].ToFloat();
                    Player.MoveLocalY(y * 8);
                }
                else if (parameters.Length > 2)
                {
                    ConsoleBox.BbcodeText += "\n [color=red]The move Command must follow this pattern: move X Y[/color]";
                    return;
                }

                var x = parameters[0].ToFloat();

                Player.MoveLocalX(x * 8);
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

        ConsoleBox.BbcodeText += "\n " + new_text;
    }
}


