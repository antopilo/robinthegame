using Godot;
using System;
using System.Collections.Generic;

public class SongPuzzle : Node2D
{
    public bool Completed = false;

    [Export] private int[] GoodSong = new int[4];
    private List<Bell> Bells = new List<Bell>();
    private List<int> CurrentSong;

    // Debug
    private Color Green = new Color(0, 1, 0);
    private Color Red = new Color(1, 0, 0);
    private Color Blue = new Color(0, 0, 1);
    private Color DebugColor = new Color(1, 0, 0);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CurrentSong = new List<int>(99);

        foreach (var bell in GetChildren())
            Bells.Add(bell as Bell);

        VerifyPossible();
    }

    public override void _Process(float delta)
    {
        Update();

        if (Completed)
            return;

        ScanBell();
        ScanTune();
    }

    private void VerifyPossible()
    {
        foreach (int note in GoodSong)
        {
            if (note > Bells.Count - 1)
                GD.PrintErr("SONG NOT POSSIBLE");
        }
    }
    private void ScanBell()
    {
        for (int i = 0; i < Bells.Count; i++)
            if (Bells[i].IsPlaying())
                CurrentSong.Add(i);
    }

    private void ScanTune()
    {
        if (CurrentSong.Count == 0)
            return;

        int idx = 0;
        for (int i = 0; i < GoodSong.Length; i++)
        {
            if (i >= CurrentSong.Count)
                return;

            if(CurrentSong[i] != GoodSong[i])
            {
                CurrentSong.Clear();
                idx = 0;
                return;
            }
            idx++;
        }
        CompletePuzzle();
    }

    
    public void CompletePuzzle()
    {
        Completed = true;
    }

    public override void _Draw()
    {
       DebugColor = Completed ? Green : Red;
       DrawRect(new Rect2(0,0,8,8), DebugColor, true);
    }
}
