using Godot;
using System;

public class Play : Button
{
    private void _on_Play_pressed()
    {
        GD.Print("Play");
        this.Pressed = false;
        ((AnimationPlayer)GetNode("../../../../Transition/AnimationPlayer")).Play("FadeIn");
    }
}



