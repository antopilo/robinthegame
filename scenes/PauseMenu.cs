using Godot;
using System;

public class PauseMenu : Control
{

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            GetTree().Paused = !GetTree().Paused;
            Visible = GetTree().Paused;
        }
    }

    private void Unpause()
    {
        GetTree().Paused = false;
        Visible = false;
    }

    private void Quit()
        => GetTree().Quit();

}

