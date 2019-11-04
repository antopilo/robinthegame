using Godot;
using System.Collections.Generic;

public class Weapon : Node2D
{
    private PackedScene BlueArrowScene, PurpleArrowScene, HookArrowScene;
    private Player Player;
    private Timer Timer;
    private Position2D Origin;

    public Arrow CurrentArrow;
    public bool ControllerMode = false;
    public bool CanShoot = true;

    private List<PackedScene> availableArrows = new List<PackedScene>();
    private int SelectedArrow = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BlueArrowScene = ResourceLoader.Load("res://Content/Actors/Player/Arrow/arrow.tscn") as PackedScene;
        PurpleArrowScene = ResourceLoader.Load("res://Content/Actors/Player/Arrow/ArrowPurple.tscn") as PackedScene;
        HookArrowScene = ResourceLoader.Load("res://Content/Actors/Player/Arrow/ArrowHook.tscn") as PackedScene;
        Player = GetParent() as Player;
        Timer = GetNode("../Timers/Timer") as Timer;
        Origin = GetNode("Arrow_origin") as Position2D;

        availableArrows.Add(BlueArrowScene);
        availableArrows.Add(PurpleArrowScene);
        availableArrows.Add(HookArrowScene);
    }

    public override void _Input(InputEvent @event)
	{
        if (CanShoot)
        {
            //if (@event.IsActionPressed("fire"))
            //{
                
            //    ShootArrow();
            //}
            if (@event.IsActionPressed("right_click"))
            {
                CanShoot = false;

                Timer.Start();
                ShootArrow();
                //CurrentArrow.Dash();
            }

            if (@event.IsActionPressed("NextArrow"))
            {
                if (SelectedArrow < availableArrows.Count - 1)
                    SelectedArrow++;
                else
                    SelectedArrow = 0;
                Root.Dialog.ShowMessage("SelectedArrow: " + SelectedArrow, 2f);
            }
            else if (@event.IsActionPressed("PreviousArrow"))
            {
                if (SelectedArrow > 0)
                    SelectedArrow--;
                else
                    SelectedArrow = availableArrows.Count - 1;
                Root.Dialog.ShowMessage("SelectedArrow: " + SelectedArrow, 2f);
            }
        }
        else
        {
            if (@event.IsActionPressed("NextArrow"))
            {
                CurrentArrow.ReturnToPlayer();
                if (SelectedArrow < availableArrows.Count - 1)
                    SelectedArrow++;
                else
                    SelectedArrow = 0;
                Root.Dialog.ShowMessage("SelectedArrow: " + SelectedArrow, 2f);
            }
            else if (@event.IsActionPressed("PreviousArrow"))
            {
                CurrentArrow.ReturnToPlayer();
                if (SelectedArrow > 0)
                    SelectedArrow--;
                else
                    SelectedArrow = availableArrows.Count - 1;
                Root.Dialog.ShowMessage("SelectedArrow: " + SelectedArrow, 2f);
            }
        }
        
    }

    public void ShootArrow()
    {
        CanShoot = false;

        Timer.Start();
        Vector2 FinalPosition = Origin.GlobalPosition;

        var arrow = availableArrows[SelectedArrow].Instance() as Arrow;
        CurrentArrow = arrow;

		GetNode("../..").AddChild(arrow);

        arrow.GlobalPosition = FinalPosition;
        arrow.ControllerMode = ControllerMode;
        arrow.Name = "arrow";
    }

    public void DestroyArrow()
    {
        if (CurrentArrow is null)
            return;

        CurrentArrow.QueueFree();
    }
}
