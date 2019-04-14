using Godot;
using System;

public class Lift : Node2D
{
    [Export] public Vector2 Direction = new Vector2(0, 1);
    [Export] public float Speed = 2f;
    [Export] public string RequiredItem = "Lever";

    public bool CanInteract = true;
    public Vector2 InteractionOffset = new Vector2();

    private KinematicBody2D Platform;
    private Tween Tween;
    private Area2D InteractionZone;

    private bool Fixed = false;
    private bool Up = true;
    private bool PlayerPresent = false;

    
    private float TweenSpeed = 0f;

    private Vector2 InitialPosition = new Vector2();
    private Vector2 Target = new Vector2();
    private Vector2 LastPosition = new Vector2();
    public override void _Ready()
    {
        Platform = (KinematicBody2D)GetNode("Platform");
        InteractionZone = (Area2D)GetNode("InteractionZone");

        if (!HasNode("Target"))
        {
            GD.Print("ERROR: No target found ");
            Console.Log("ERROR: No target found ");
            return;
        }

        InitialPosition = Platform.GlobalPosition;
        Target = ((Node2D)GetNode("Target")).GlobalPosition;

        Tween = (Tween)GetNode("Tween");
        TweenSpeed = (InitialPosition - Target).Length() / (1 / Speed);
    }

    public override void _Process(float delta)
    {
        InteractionZone.GlobalPosition = Platform.GlobalPosition;
        if (Tween.IsActive() && PlayerPresent)
        {
            Root.Player.CanControl = false;
            
            Root.Player.GlobalPosition -= LastPosition - Platform.GlobalPosition;
            Root.Player.State = States.Ground;
        }
        else if(PlayerPresent && !Tween.IsActive())
        {
            Root.Player.CanControl = true;
        }
        LastPosition = Platform.GlobalPosition;
    }


    public void Interact()
    {
        if (!Fixed )
        {
            if (InventoryManager.HasItem(RequiredItem))
            {
                InventoryManager.RemoveItem(RequiredItem, 1);
                Root.Dialog.ShowMessage("Lift repaired", 2f);
                Fixed = true;
            }
            else
            {
                Root.Dialog.ShowMessage("You need to repair it first and you are missing the components...", 2f);
                return;
            }
        }
        else
        {
            if(Up)
            {
                Tween.InterpolateProperty(Platform, "global_position", 
                    InitialPosition, Target, Speed,
                    Tween.TransitionType.Linear, Tween.EaseType.InOut);
                Tween.Start();
                InteractionOffset = Target;
                CanInteract = false;
                Up = false;
                Root.Player.RemoveFromInteraction(this);
            }
            else if(!Up)
            {
                Tween.InterpolateProperty(Platform, "global_position", 
                    Target, InitialPosition, Speed,
                    Tween.TransitionType.Linear, Tween.EaseType.InOut);
                Tween.Start();
                InteractionOffset = new Vector2();
                CanInteract = false;
                Up = true;
                Root.Player.RemoveFromInteraction(this);
            }
        }
    }
	

	private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        Tween.RemoveAll();
        CanInteract = true;
        Root.Player.CanControl = true;
    }


    private void _on_PlayerCheck_body_entered(object body)
    {
        if(body is Player)
        {
            PlayerPresent = true;
            var player = body as Player;
        }
    }


    private void _on_PlayerCheck_body_exited(object body)
    {
        if (body is Player)
        {
            PlayerPresent = false;
            var player = body as Player;
        }
    }
}






