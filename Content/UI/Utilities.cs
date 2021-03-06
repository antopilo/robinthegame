using Godot;
using System;

public class Utilities : Control
{
    public bool MenuOpened = false;
    public Inventory Inventory;
    private Vector2 InitialPosition = new Vector2();
    private Tween Tween;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Inventory = (Inventory)GetNode("MarginContainer/TabContainer/Inventory");
        Tween = (Tween)GetNode("Tween");
        InitialPosition = this.RectPosition;
    }

    public override void _Input(InputEvent @event)
    {
        if (Tween.IsActive())
            return;
        if (@event.IsActionPressed("OpenUI") && !MenuOpened)
            OpenUI();
        else if (@event.IsActionPressed("OpenUI") && MenuOpened)
            CloseUI();
    }
    public void OpenUI()
    {
        if(MenuOpened)
            return;
        Tween.StopAll();
        Console.Log("UI opened");
        MenuOpened = true;
        Tween.InterpolateProperty(this, "rect_position", InitialPosition, new Vector2(0, 0), 0.5f,
            Tween.TransitionType.Expo, Tween.EaseType.Out);
        Tween.Start();

        GetTree().Paused = true;
    }

    public void CloseUI()
    {
        if(!MenuOpened)
            return;
        Tween.StopAll();    
        Console.Log("UI closed");
        MenuOpened = false;
        Tween.InterpolateProperty(this, "rect_position", new Vector2(0, 0), InitialPosition, 0.5f,
            Tween.TransitionType.Expo, Tween.EaseType.Out);
        Tween.Start();
        GetTree().Paused = false;
    }
}
