using Godot;
using System;
using System.IO;
using Newtonsoft.Json;

public class Root : Node2D
{
    public LevelInfo LevelInfo;
    public Settings settings = new Settings();
    public static Console Console;
    public GameController GameController;
    public static Player Player;
    public Weapon Weapon;
    public static ViewportContainer GameContainer;

    public override void _Ready()
    {
        // Get node
        Console = GetNode("UI/Console") as Console;
        GameController = GetNode("Game/Viewport/World") as GameController;
        Player = GameController.GetNode("Player") as Player;
        Weapon = Player.GetNode("Weapon") as Weapon;
        GameContainer = GetNode("Game") as ViewportContainer;
        LoadSettings();
        ApplySettings();
    }

    public void LoadSettings()
	{
        if (!System.IO.File.Exists("settings.json"))
            SaveSettings();

        StreamReader writer = new StreamReader("settings.json");
        
        string file = writer.ReadToEnd();
        writer.Close();

        Settings LoadedSettings = JsonConvert.DeserializeObject<Settings>(file);
        settings = LoadedSettings;
	}

    public void SaveSettings()
    {
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        StreamWriter file = new StreamWriter("settings.json");
        file.WriteLine(json);
        file.Close();
    }

    public void ApplySettings()
    {
        // Save first 
        SaveSettings();

        // Apply
        OS.WindowSize = settings.Resolution;
        OS.WindowFullscreen = settings.Fullscreen;
        OS.WindowBorderless = settings.Borderless;
        OS.VsyncEnabled = settings.Vsync;
        Weapon.ControllerMode = settings.Controller;
        Engine.TargetFps = settings.MaxFps;
    }


}


