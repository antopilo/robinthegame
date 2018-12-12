using Godot;
using System;
using System.IO;
using Newtonsoft.Json;

public class Root : Node2D
{
    public LevelInfo LevelInfo;
    public Settings settings = new Settings();
    public GameController GameController;
    public Player Player;
    public Weapon Weapon;

    public override void _Ready()
    {
        // Get node
        GameController = GetNode("Game/Viewport/World") as GameController;
        Player = GameController.GetNode("Player") as Player;
        Weapon = Player.GetNode("Weapon") as Weapon;
        

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


