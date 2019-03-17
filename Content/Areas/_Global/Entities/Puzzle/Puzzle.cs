using Godot;
using System;
using System.Collections.Generic;

public class Puzzle : Node2D
{
    private Color Green = new Color(0, 1, 0);
    private Color Red = new Color(1, 0, 0);
    private Color Blue = new Color(0, 0, 1);
    private Color DebugColor = new Color(0, 0, 1);

    public List<Trigger> Triggers = new List<Trigger>();
    public bool Solved = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach(var trigger in GetChildren())
        {
            if(trigger is Trigger){
                Triggers.Add(trigger as Trigger);
            }
        }
    }
 
    public override void _PhysicsProcess(float delta)
    {

        if(Solved)
            return;
        
        for (int i = 0; i < Triggers.Count; i++)
        {
            if(!(Triggers[i] as Trigger).Triggered)
                return;
        }

        
        DebugColor = Green;
        Update();
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(0,0,8,8), DebugColor);
    }
}
