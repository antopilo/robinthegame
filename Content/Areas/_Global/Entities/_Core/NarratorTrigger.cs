using Godot;
using System;

public class NarratorTrigger : Area2D
{
    [Export] private string[] Message;
    [Export] private bool ShowOnce = true;
    private bool Showed = false;

    private void _on_NarratorTrigger_body_entered(object body)
    {
        if(ShowOnce && Showed)
            return;

        if(body is Player)
        {
            if(ShowOnce)
                Showed = true;

            //Root.Dialog.ShowMessage(Message);
        }
    }
}



