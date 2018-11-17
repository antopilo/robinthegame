using Godot;
using System;
using System.Linq;

public class Console : Control
{
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
        ConsoleInput = GetNode("LineEdit") as LineEdit;
        GameController = GetNode("../../game/Viewport/GameManager") as GameController;
        DialogBox = GetNode("../Dialog") as Dialog;
        ConsoleBox = GetNode("ConsoleBox") as RichTextLabel;
        Player = GameController.GetNode("Player") as Player;
        Weapon = Player.GetNode("Weapon") as Weapon;
		Player.CanControl = true;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

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
                switch (parameters[0])
                {
                    case "":
                    case "0":
                    case "1":
                        OS.WindowSize = new Vector2(320, 180);
                        break;
                    case "2":
                        OS.WindowSize = new Vector2(640, 360);
                        break;
                    case "3":
                        OS.WindowSize = new Vector2(1280, 720);
                        break;
                    case "4":
                        OS.WindowSize = new Vector2(1920, 900);
                        OS.WindowPosition = new Vector2();
                        break;
					case "5":
                        OS.WindowSize = new Vector2(1920, 1080);
                        OS.WindowPosition = new Vector2();
                        break;
                }
                break;

            // Fullscreen
            case "FULLSCREEN":
                if (parameters[0] == "1")
                    OS.WindowFullscreen = true;
                else
                    OS.WindowFullscreen = false;
                break;
            
            // Toggle Vsync usage.
            case "VSYNC":
                if (parameters[0] == "0")
                    OS.VsyncEnabled = false;
                else
                    OS.VsyncEnabled = true;
                break;
            
            // Make the borderless
            case "BORDERLESS":
                if (parameters[0] == "1")
                    OS.WindowBorderless = true;
                else
                    OS.WindowBorderless = false;
                break;

            // Say command
            case "SAY":
                DialogBox.ShowMessage(new_text.Right(4));
                break;

            // Teleport the player to a specified room.
            case "TP":
                string DestinationLevel = parameters[0];
                Level Level = GameController.GetNode(DestinationLevel) as Level;

                if(Level != null && Level.IsInGroup("level"))
                {
                    GameController.CurrentRoom = Level;
                    GameController.Spawn(true);
                }
                break;
            
            // Move the player X Y Tile.
            case "MOVE":
                

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


