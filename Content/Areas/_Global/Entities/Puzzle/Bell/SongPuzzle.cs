using Godot;
using System;
using System.Collections.Generic;

public class SongPuzzle : Node2D
{
    [Export] int[] GoodSong = new int[4];
    [Export] float[] NoteTiming = new float[4];

    public List<Bell> Bells = new List<Bell>();
    private List<int> CurrentSong;
    public bool Completed = false;

    // Playback
    private float PlaybackInitialDelay = 2f;
    private float DeltaTime = 0f;
    private float NextRing = 0f;
    private int PlaybackCurrent = 0;
    private bool PlaybackDone = false;

    // Debugging
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
        if(PlaybackDone)
            return;

        DeltaTime += delta;

        Update();

        if (Completed && DeltaTime >= NextRing){
            Playback();
            return;
        }

        if(Completed) return;
        ScanBell();
        ScanTune();
    }

    // Verify if the puzzle is possible.
    private void VerifyPossible()
    {
        foreach (int note in GoodSong)
        {
            if (note > Bells.Count - 1)
                GD.PrintErr("SONG NOTE POSSIBLe");
        }
    }

    // Check if a bell is being played.
    private void ScanBell()
    {
        for (int i = 0; i < Bells.Count; i++)
            if (Bells[i].IsPlaying())
                CurrentSong.Add(i);
    }

    // Puzzle condition.
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
        NextRing = DeltaTime + PlaybackInitialDelay;
    }

    private void Playback()
    {
        if(PlaybackCurrent == GoodSong.Length - 1)
        {
            Bells[GoodSong[PlaybackCurrent]].Interact();
            PlaybackDone = true;
            return;
        }
        Bells[GoodSong[PlaybackCurrent]].Interact();
        NextRing = DeltaTime + NoteTiming[PlaybackCurrent];
        PlaybackCurrent++;
    }

    public override void _Draw()
    {
       DebugColor = Completed ? Green : Red;
       DrawRect(new Rect2(0,0,8,8), DebugColor, true);
    }
}
