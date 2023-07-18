using Godot;
using System;

public class ThrowerRigidBody : Area2D
{
	Vector2 StartPos=new Vector2(0,0);
	Vector2 EndPos=new Vector2(0,0);
	Vector2 Direction=new Vector2(0,0);
	float Speed=0, Angle;
	bool Selected=false, Moving=false;
	public RigidBody2D Ball;
	Line2D Linea;
	Sprite Arrow;
	
	
	public override void _Ready()
	{
		Linea=GetNode<Line2D>("Line2D");
		Arrow=GetNode<Sprite>("Arrow");
	}
	
	public override void _Process(float delta)
	{
		if(Selected)
		{
			Position=GetGlobalMousePosition();
		}
		else if(Speed!=0){    //improve
			QueueFree();
			Player.ThrowerGenerated=false;
		}
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(Moving)
		{
			//Trajectory
			Linea.ClearPoints();
			Linea.AddPoint(new Vector2(0,0));
			Direction=(StartPos-GetGlobalMousePosition()).Normalized();
			Speed=StartPos.DistanceTo(GetGlobalMousePosition());
			if(Speed>800) //delimitar
			{
				Speed=800;
			}

			Angle=Direction.Angle();
			Vector2 BallCenter = StartPos+new Vector2(0,-35);

			for(float t=0;t<10;t+=0.01f)
			{
				float X=(Speed*(float)Mathf.Cos(Angle)*t)+(BallCenter.x-Position.x);
				float Y=(0.5f*9.8f*t*t)+(Speed*(float)Mathf.Sin(Angle)*t)+(BallCenter.y-Position.y);
				Vector2 NewPos=new Vector2(X,Y);
				Linea.AddPoint(NewPos);
			}

			//Arrow
//			Linea.ClearPoints();
//			Linea.AddPoint(new Vector2(0,0));
//			Direction=(StartPos-Position).Normalized();
//			Speed=(Position-StartPos).Length();
//			Linea.AddPoint(Direction*Speed);
//			Arrow.LookAt(GetGlobalMousePosition());
//			Arrow.RotationDegrees-=90;
//			Arrow.Position=Direction*Speed;

			//Trajectory KinematicBody2D
//			var MaxPoints=300;
//			Linea.ClearPoints();
//			var Pos=new Vector2(0,0);
//			var Vel=Direction*Speed;
//			foreach(int i in MaxPoints)
//			{
//				Linea.AddPoint(Pos);
//
//			}
		}
	}
	
	public static Thrower GetThrower()
	{
		PackedScene Lanzador=(PackedScene)ResourceLoader.Load("res://scenes/Thrower.tscn");
		return (Thrower)Lanzador.Instance();
	}
	
	private void _on_Thrower_input_event(object viewport, object @event, int shape_idx)
	{
		if(@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left)
			{
				if(MouseButtonEvent.Pressed)
				{
					StartPos=GetGlobalMousePosition();
					Selected=true;
				}
				else{
					Selected=false;
					//Speed=(EndPos-StartPos).Length();
					//Position=StartPos+new Vector2(0,-35);
					Ball.ApplyImpulse(new Vector2(0,0), Direction*Speed);
					//Ball.LinearVelocity=Direction*Speed;
				}
			}
		}
		
		if(@event is InputEventMouseMotion MouseMotionEvent && Selected)
		{
			Moving=true;
			
			//flecha
//			Linea.ClearPoints();
//			Linea.AddPoint(new Vector2(0,0));
//			Vector2 direccion=new Vector2(0,0);
//			direccion=(StartPos-Position).Normalized();
//			float speed;
//			speed=(Position-StartPos).Length();
//			Linea.AddPoint(direccion*speed);
//			Arrow.LookAt(GetGlobalMousePosition());
//			Arrow.RotationDegrees-=90;
//			Arrow.Position=direccion*speed;


//prediccion
//			Linea.ClearPoints();
//			Linea.AddPoint(new Vector2(0,0));
//			Vector2 direccion=new Vector2(0,0);
//			direccion=(StartPos-Position).Normalized();
//			float speed=(Position-StartPos).Length();
//			float Angle=Mathf.Rad2Deg(direccion.Angle());
//			//speed /= 10;
//			for(float t=0;t<10;t+=0.01f)
//			{
//				float X=(speed*(float)Mathf.Cos(Mathf.Deg2Rad(Angle))*t)+(StartPos.x-Position.x);
//				float Y=(0.5f*9.8f*t*t)+(speed*(float)Mathf.Sin(Mathf.Deg2Rad(Angle))*t)+StartPos.y-Position.y;
//				Vector2 NewPos=new Vector2(X,Y);
//				Linea.AddPoint(NewPos);
//			}
	
		}
		else{
			Moving=false;
		}
		
	}
}



