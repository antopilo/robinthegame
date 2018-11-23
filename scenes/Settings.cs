using Godot;
using System;

public class Settings
{
    public Vector2 Resolution { get; set; } = new Vector2(1280, 720);
    public bool Fullscreen { get; set; } = true;
    public bool Borderless { get; set; } = false;
    public bool Vsync { get; set; } = true;
    public bool Controller { get; set; } = false;
    public int MaxFps { get; set; } = 60;
}
