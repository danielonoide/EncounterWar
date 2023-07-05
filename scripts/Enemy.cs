using Godot;
using System;

public class Enemy : RigidBody2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(Input.IsActionPressed("Down"))
		{
			//ApplyImpulse(new Vector2(0,0), new Vector2(10,-50));
		}
		if(Input.IsActionPressed("Up"))
		{
			//ApplyImpulse(new Vector2(0,0), new Vector2(-10,0));			
		}
	}


}
