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
    public static InventoryManager InventoryManager;
    public static Player Player;
    public static Weapon Weapon;
    public static Dialog Dialog;
    public static SceneTransition SceneTransition;
    public static SceneSwitcher SceneSwitcher;
    public static Viewport m_viewport;
    public static ViewportContainer GameContainer;
    public static Utilities Utilities;

    public static Viewport Viewport 
    {
        get 
        {
            return (Viewport)GameContainer.GetNode("Viewport");
        }
    }

    public override void _Ready()
    {
        // Singletons
        SceneSwitcher = GetNode("/root/SceneSwitcher") as SceneSwitcher;
        InventoryManager = GetNode("/root/InventoryManager") as InventoryManager;

        // Get node
        SceneTransition = GetNode("UI") as SceneTransition;
        Utilities = (Utilities)GetNode("UI/Utilities");
        Console = GetNode("UI/Console") as Console;
        LevelInfo = (LevelInfo)GetNode("UI/LevelInfo");
        Dialog = (Dialog)GetNode("UI/Dialog");
        DeathCount = (DeathCount)GetNode("UI/DeathCount");

        GameContainer = GetNode("Game") as ViewportContainer;
        m_viewport = GetNode("Game/Viewport") as Viewport;
        GameController = GetNode("Game/Viewport/World") as GameController;
        Player = GameController.GetNode("Player") as Player;
        Weapon = Player.GetNode("Weapon") as Weapon;
        
        LoadSettings();
        ApplySettings();
    }

    public override void  _Process(float delta)
    {
        // Singletons
        SceneSwitcher = GetNode("/root/SceneSwitcher") as SceneSwitcher;
        InventoryManager = GetNode("/root/InventoryManager") as InventoryManager;

        // Get node
        SceneTransition = GetNode("UI") as SceneTransition;
        Utilities = (Utilities)GetNode("UI/Utilities");
        Console = GetNode("UI/Console") as Console;
        LevelInfo = (LevelInfo)GetNode("UI/LevelInfo");
        Dialog = (Dialog)GetNode("UI/Dialog");
        DeathCount = (DeathCount)GetNode("UI/DeathCount");

        GameContainer = GetNode("Game") as ViewportContainer;
        m_viewport = GetNode("Game/Viewport") as Viewport;
        GameController = Viewport.GetChild(0) as GameController;
        Player = GameController.GetNode("Player") as Player;
        Weapon = Player.GetNode("Weapon") as Weapon;

        if (Input.IsActionJustPressed("Screenshot"))
            Screenshot();
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

    public static void Screenshot()
    {
        var img = Viewport.GetViewport().GetTexture().GetData();
        Root.Console.Visible = false;
        img.FlipY();
        var path = "Screenshot/" + OS.GetDate()["year"] + "-" + OS.GetDate()["month"] + "-" + OS.GetDate()["day"] +
            "_" + OS.GetTime()["hour"] + "-" + OS.GetTime()["minute"] + "-" + OS.GetTime()["second"] + ".png";
        img.Resize(1920, 1080, Image.Interpolation.Nearest);
        img.SavePng(path);
        Root.Dialog.ShowMessage("Screenshot saved at: " + path);
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


