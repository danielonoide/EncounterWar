using Godot;
using System;

public class Player : RigidBody2D
{
	bool Presionado;
	public static bool ThrowerGenerated=false;
	
	public override void _Ready()
	{
		ThrowerGenerated=false;
	}

	public override void _Process(float delta)
	{
		
	}
	
	private void _on_Player_input_event(object viewport, object @event, int shape_idx)
	{
		if(@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left && !ThrowerGenerated)
			{
//				Area2D Lanzador=Thrower.GetThrower(this);
//				Lanzador.Position=Position;
//				Lanzador.Position+=new Vector2(0,50);
//				GetParent().AddChild(Lanzador);
/* 				Thrower Lanzador=Thrower.GetThrower();
				Lanzador.Ball=this;
				Lanzador.Position=Position;
				Lanzador.Position+=new Vector2(0,35);
				GetParent().AddChild(Lanzador); */
				
				
				ThrowerGenerated=true;
			}
		}



//		if(@event is InputEventMouseButton MouseButtonEvent)
//		{
//			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left)
//			{
//				if(MouseButtonEvent.Pressed)
//				{
//					StartPos=GetGlobalMousePosition();
//					Presionado=true;
//				}
//				else{
//					Soltado();
//				}
//			}
//		}
	}


	
//	public override void _UnhandledInput(InputEvent @event)
//	{
//		if(@event is InputEventMouseButton MouseButtonEvent)
//		{
//			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left && !MouseButtonEvent.Pressed && Presionado==true)
//			{
//				Soltado();
//			}
//		}
//	}
//
//	void Soltado()
//	{
//		Presionado=false;
//		EndPos=GetGlobalMousePosition();
//		Direction=(StartPos-EndPos).Normalized();
//		Speed=(EndPos-StartPos).Length();
//		ApplyImpulse(new Vector2(0,0), Direction*Speed);
//	}
	
	
	
}





