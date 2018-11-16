using Godot;
using System;

public class Spike : Node2D
{	
	private void _on_oSpike_body_entered(PhysicsBody2D body)
	{
    	if(body is Player)
			((Player)body).Spawn(true);
		
	}
}



