using Godot;
using System;

public class Chest : Node2D
{
    public bool Opened = false;
    private AnimatedSprite AnimatedSprite;
    private AudioStreamPlayer AudioPlayer;
    private Light2D Light;

    public bool CanInteract = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
        AudioPlayer = (AudioStreamPlayer)GetNode("AudioStreamPlayer");
        Light = (Light2D)GetNode("Light");
    }

    public void Interact()
    {
        Light.Enabled = true;
        CanInteract = false;
        AnimatedSprite.Animation = "Opening";
        Root.Player.RemoveFromInteraction(this);
        AudioPlayer.Play(0);
    }
}
