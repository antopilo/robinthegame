using System;

public class DebugViewer : Godot.RichTextLabel
{

    public override void _Process(float delta)
    {
        this.Clear();

        this.Text = DebugPrinter.GetDebugContent();
    }
}

