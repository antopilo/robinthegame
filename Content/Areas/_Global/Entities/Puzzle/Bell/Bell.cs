using Godot;
using System;

public class Bell : Node2D
{
    private bool playing = false;
    private AnimationPlayer AnimPlayer;
    private Sprite BellSprite, Frame;
    private AudioStreamPlayer Audio;
    public bool CanInteract = true;
    private Timer InteractTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        BellSprite = GetNode("Bell") as Sprite;
        Frame = GetNode("Frame") as Sprite;
        Audio = GetNode("AudioStreamPlayer") as AudioStreamPlayer;
        InteractTimer = GetNode("Timer") as Timer;
    }

    public void Interact()
    {
        if(!CanInteract)
            return;
        (GetNode("InteractionZone/CollisionShape2D") as CollisionShape2D).Disabled = true;
        GD.Print("The bell has been rung!");
        Audio.Playing = true;
        AnimPlayer.Play("Ring");
        playing = true;
        CanInteract = false;
        InteractTimer.Start();
    }

    private void _on_Timer_timeout()
    {
        playing = false;
        CanInteract = true;
        GD.Print("The bell is ready!");
        (GetNode("InteractionZone/CollisionShape2D") as CollisionShape2D).Disabled = false;
    }
}



