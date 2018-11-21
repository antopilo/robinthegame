using Godot;
using System;

public class Sign : Node
{
    private Player Player;
    private string Message = "Salut";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetNode("../../../") as Player;
    }


}
