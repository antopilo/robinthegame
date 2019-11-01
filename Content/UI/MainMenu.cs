using Godot;
using Newtonsoft.Json;
using System;
using System.IO;

public class MainMenu : Control
{
    private PackedScene PlayScene;
    public Settings settings = new Settings();
    private AnimationPlayer AnimPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayScene = (PackedScene)ResourceLoader.Load("res://Content/_Core/Scenes/RenderScene.tscn");
        LoadSettings();
        ApplySettings();
    }

    public void _on_Play_pressed()
    {
        ((AnimationPlayer)GetNode("Transition/AnimationPlayer")).Play("FadeIn");
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
        
        Engine.TargetFps = settings.MaxFps;
    }
    private void _on_AnimationPlayer_animation_finished(String anim_name)
    {
        GetTree().ChangeScene("res://Content/_Core/Scenes/RenderScene.tscn");
    }
}

