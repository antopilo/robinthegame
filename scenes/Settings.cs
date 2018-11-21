using Godot;
using System;

public class Settings
{
    public Vector2 Resolution { get; set; }
    public bool Fullscreen { get; set; }
    public bool Borderless { get; set; }
    public bool Vsync { get; set; }
    public bool Controller { get; set; }
    public int MaxFps { get; set; }
}
