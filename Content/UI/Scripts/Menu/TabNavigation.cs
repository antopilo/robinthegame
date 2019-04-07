using Godot;
using System;

public class TabNavigation : TabContainer
{


    public override void _Input(InputEvent @event)
    {
        if (!((Utilities)GetNode("../../")).MenuOpened)
            return;

        if (@event.IsActionPressed("ui_nextpage"))
        {
            if(this.CurrentTab < this.GetChildCount())
                this.CurrentTab++;
            else
                this.CurrentTab = 0;
        }


        if (@event.IsActionPressed("ui_previouspage"))
        {
            if (this.CurrentTab > 0)
                this.CurrentTab--;
            else
                this.CurrentTab = this.GetChildCount();
        }   
    }
}
