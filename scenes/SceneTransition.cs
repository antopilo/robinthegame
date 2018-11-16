using Godot;
using System;

public class SceneTransition : CanvasLayer
{
    private AnimationPlayer AnimPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimPlayer = (AnimationPlayer)GetNode("Transition/AnimationPlayer");
    }

	public void Fade()
	{
		AnimPlayer.Play("FadeOut");
	}
}



