using Godot;
using System;
using System.IO;
using Newtonsoft.Json;

public class Root : Control
{
    public static LevelInfo LevelInfo;
    public Settings settings = new Settings();
    public static Console Console;
    public static DeathCount DeathCount;
    public static GameController GameController;
    public static Player Player;
    public static Weapon Weapon;
    public static Dialog Dialog;
    public static SceneTransition SceneTransition;
    public static SceneSwitcher SceneSwitcher;
    public static Viewport Viewport;
    public static ViewportContainer GameContainer;

    public override void _Ready()
    {
        // Get node
        Console = GetNode("UI/Console") as Console;
        SceneTransition = GetNode("UI") as SceneTransition;
        Viewport = GetNode("Game/Viewport") as Viewport;
        LevelInfo = (LevelInfo)GetNode("UI/LevelInfo");
        SceneSwitcher = GetNode("/root/SceneSwitcher") as SceneSwitcher;
        GameController = GetNode("Game/Viewport/World") as GameController;
        Player = GameController.GetNode("Player") as Player;
        Weapon = Player.GetNode("Weapon") as Weapon;
        GameContainer = GetNode("Game") as ViewportContainer;
        Dialog = (Dialog)GetNode("UI/Dialog");
        DeathCount = (DeathCount)GetNode("UI/DeathCount");

        LoadSettings();
        ApplySettings();
    }

    public override void  _Process(float delta){
        Console = GetNode("UI/Console") as Console;
        Viewport = GetNode("Game/Viewport") as Viewport;
        LevelInfo = (LevelInfo)GetNode("UI/LevelInfo");
        DeathCount = (DeathCount)GetNode("UI/DeathCount");
        Dialog = (Dialog)GetNode("UI/Dialog");
        GameController = GetNode("Game/Viewport").GetChild(0) as GameController;
        Player = GameController.GetNode("Player") as Player;

        Weapon = Player.GetNode("Weapon") as Weapon;
        GameContainer = GetNode("Game") as ViewportContainer;
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


