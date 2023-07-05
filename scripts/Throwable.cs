using Godot;
using System;

public class Throwable : RigidBody2D
{
	public override void _Ready()
	{
		
	}
	
	public override void _Process(float delta)
	{
		
	}
	
	public static RigidBody2D GetThrowable()
	{
		PackedScene MenuPausa=(PackedScene)ResourceLoader.Load("res://scenes/Throwable.tscn");
		return (RigidBody2D)MenuPausa.Instance();
	}
	
	private void _on_Timer_timeout()
	{
		QueueFree();
	}

	
}


