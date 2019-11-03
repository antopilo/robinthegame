using System;
using Godot;

public class Hook : Node2D
{


    
    private void _on_Area2D_body_entered(object body)
    {
        if(body is HookArrow)
        {
            HookArrow arrow = (HookArrow)body;

            arrow.Hook = this;
            arrow.DoHook();
        }
    }
}



