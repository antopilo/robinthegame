using Godot;
using System;

public class Npc : Node
{
    [Export] public string Name = "";
    [Export] public Image Face;
    [Export(PropertyHint.MultilineText)] public string Dialog;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
