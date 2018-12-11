using Godot;
using System;

public class PauseMenu : Control
{
    private Control OptionsMenu;
    public Root root;

    Vector2[] PossibleResolution = new Vector2[] { new Vector2(320, 180), new Vector2(640, 360),
                                                  new Vector2(1280,720), new Vector2(1600,900), new Vector2(1920,1080)};
    int CurrentResolution = 0;

    int[] PossibleFps = new int[] { 15, 30, 60, 120, 144, 240, -1 };
    int CurrentFps = 0;

    public override void _Ready()
    {
        OptionsMenu = GetNode("MarginContainer/HBoxContainer/Options") as Control;
        root = GetNode("../..") as Root;

        for (int i = 0; i < PossibleResolution.Length - 1; i++)
        {
            if (root.settings.Resolution == PossibleResolution[i])
                CurrentResolution = i;
        }

        for (int i = 0; i < PossibleFps.Length - 1; i++)
        {
            if (root.settings.MaxFps == PossibleFps[i])
                CurrentFps = i;
        }
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            GetTree().Paused = !GetTree().Paused;
            Visible = GetTree().Paused;
            OptionsMenu.Visible = false;
            (GetNode("MarginContainer/HBoxContainer/Main/Resume") as Button).GrabFocus();
        }
    }

    private void ToggleOption()
    {
        OptionsMenu.Visible = !OptionsMenu.Visible;

        if (OptionsMenu.Visible)
        {
            (GetNode("MarginContainer/HBoxContainer/Options/Resolution/Resolution") as Button).Text = PossibleResolution[CurrentResolution].x.ToString() + " x " + PossibleResolution[CurrentResolution].y.ToString();
            (GetNode("MarginContainer/HBoxContainer/Options/Vsync/Vsync") as Button).Text = root.settings.Vsync.ToString();
            (GetNode("MarginContainer/HBoxContainer/Options/Fullscreen/Fullscreen") as Button).Text = root.settings.Fullscreen.ToString();
            (GetNode("MarginContainer/HBoxContainer/Options/Controller/Controller") as Button).Text = root.settings.Controller.ToString();
            (GetNode("MarginContainer/HBoxContainer/Options/Fps/Fps") as Button).Text = PossibleFps[CurrentFps].ToString();
        }
    }


    private void Unpause()
    {
        GetTree().Paused = false;
        Visible = false;
    }

    private void Quit()
        => GetTree().Quit();

    // SETTINGS HERE
    
    public void _on_Resolution_pressed()
    {
        if (CurrentResolution < PossibleResolution.Length - 1)
            CurrentResolution += 1;
        else
            CurrentResolution = 0;

        root.settings.Resolution = PossibleResolution[CurrentResolution];
        (GetNode("MarginContainer/HBoxContainer/Options/Resolution/Resolution") as Button).Text = PossibleResolution[CurrentResolution].x.ToString() + " x " + PossibleResolution[CurrentResolution].y.ToString();
        GD.Print(root.settings.Resolution.ToString());
    }

    private void _on_Vsync_pressed()
    {
        root.settings.Vsync = !root.settings.Vsync;
        (GetNode("MarginContainer/HBoxContainer/Options/Vsync/Vsync") as Button).Text = root.settings.Vsync.ToString();
    }


    private void _on_Apply_pressed()
    {
        root.SaveSettings();
        root.ApplySettings();
    }

    private void _on_Fullscreen_pressed()
    {
        root.settings.Fullscreen = !root.settings.Fullscreen;
        (GetNode("MarginContainer/HBoxContainer/Options/Fullscreen/Fullscreen") as Button).Text = root.settings.Fullscreen.ToString();
    }
	
	private void _on_Controller_pressed()
    {
        root.settings.Controller = !root.settings.Controller;
        (GetNode("MarginContainer/HBoxContainer/Options/Controller/Controller") as Button).Text = root.settings.Controller.ToString();
    }

    private void _on_Fps_pressed()
    {
        if (CurrentFps < PossibleFps.Length - 1)
            CurrentFps += 1;
        else
            CurrentFps = 0;

        root.settings.MaxFps = PossibleFps[CurrentFps];
        (GetNode("MarginContainer/HBoxContainer/Options/Fps/Fps") as Button).Text = PossibleFps[CurrentFps].ToString();
        GD.Print(root.settings.MaxFps.ToString());
    }
}






