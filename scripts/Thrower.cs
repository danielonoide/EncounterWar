using Godot;
using System;
using System.Collections.Generic;

public class Thrower : Area2D
{
	Vector2 startPos=new Vector2(0,0);
	Vector2 endPos=new Vector2(0,0);
	Vector2 direction=new Vector2(0,0);

	Vector2 offset=new Vector2(0, 35);
	float speed=0, angle;
	bool selected=false;// moving=false;
	Line2D line;

    Vector2 vel=new Vector2(0,0);
    Vector2 initialVel=new Vector2(0,0);

	Area2D colisionador;
	//CircleShape2D circleShape2D;
	RectangleShape2D rectangleShape2D;

	float lineWidth=10;

	public Throwable throwable;

	List<Node> collidingBodies;

	AudioStreamPlayer restartSound;



	public override void _Ready()
	{
		Position=throwable.GlobalPosition;
		Position+=offset;

		line=GetNode<Line2D>("Line2D");
		line.Width=lineWidth;

		restartSound=GetNode<AudioStreamPlayer>("LaunchRestartSound");
		colisionador=GetNode<Area2D>("Colisionador");
		collidingBodies=new();

		rectangleShape2D=new();
		rectangleShape2D.Extents=new Vector2(1, lineWidth/2);
		
	}
	
	public override void _Process(float delta)
	{
		if(selected)
		{
			Position=GetGlobalMousePosition();
			if(Input.IsActionJustReleased("LeftClick"))
			{
				MouseReleased();
			} 
		}

	}

	public override void _PhysicsProcess(float delta)
	{
		if(!selected) return;
		
		RemoveCollisions();
		//Trajectory
		line.ClearPoints();
		line.AddPoint(new Vector2(0,0));
		direction=(startPos-GetGlobalMousePosition()).Normalized();
		speed=startPos.DistanceTo(GetGlobalMousePosition())*2; //ajuste
		if(speed>800) //delimitar
		{
			speed=800;
		}

		angle=direction.Angle();
		vel=direction*speed;
		initialVel=direction*speed; //lo que le paso al Throwable
		Vector2 newPos=startPos-Position;
		for(int i=0;i<300;i++)
		{
			line.AddPoint(newPos);
			vel.y+=Globals.Gravity*delta;
			newPos+=vel*delta;

			float lineAngle=i>0 ? line.GetPointPosition(i-1).DirectionTo(line.GetPointPosition(i)).Angle() : angle;

			if(IsColliding(ToGlobal(newPos), lineAngle)) break;

		}
		int Points=line.GetPointCount();
		

		//rectangle collision
		int j=1;
		while(j<Points-10)
		{
			CollisionShape2D Collision=new CollisionShape2D();
			RectangleShape2D Rect=new RectangleShape2D();
			Collision.Position=(line.GetPointPosition(j)+line.GetPointPosition(j+10))/2;
			Collision.Rotation=line.GetPointPosition(j).DirectionTo(line.GetPointPosition(j+10)).Angle();
			var Longitud=line.GetPointPosition(j).DistanceTo(line.GetPointPosition(j+10));
			Rect.Extents=new Vector2(Longitud/2, lineWidth/2);
			Collision.Shape=Rect;
			colisionador.AddChild(Collision);


			j+=10;
		}
		int diferencia=Points-(Points-j);

		CollisionShape2D collision=new CollisionShape2D();
		RectangleShape2D rect=new RectangleShape2D();
		collision.Position=(line.GetPointPosition(diferencia)+newPos)/2;
		collision.Rotation=line.GetPointPosition(diferencia).DirectionTo(newPos).Angle();
		var longitud=line.GetPointPosition(diferencia).DistanceTo(newPos);
		rect.Extents=new Vector2(longitud/2,lineWidth/2);
		collision.Shape=rect;
		colisionador.AddChild(collision);


	}

	private void RemoveCollisions()
	{
		for(int i=0;i<colisionador.GetChildCount();i++)
		{
			colisionador.GetChild(i).QueueFree();
		}
	}



	private bool IsColliding(Vector2 position, float angle)
	{
		// Obtiene el estado del espacio físico
		//Physics2DDirectSpaceState spaceState =new();
		// Define el área de forma rectangular alrededor de la posición

		// Realiza una consulta para verificar las colisiones con los RigidBody2D
		Physics2DShapeQueryParameters queryParameters=new(); //los parámetros de la query
		queryParameters.Transform=new Transform2D(angle, position); //la posición que se va a checar

		queryParameters.SetShape(rectangleShape2D); //la forma que va a tener el shape que va a colisionar
		Physics2DDirectSpaceState spaceState=GetWorld2d().DirectSpaceState; //objeto para hacer queries del espacio físico 2D

		var queryResult = spaceState.IntersectShape(queryParameters);

		// Verifica si se encontraron colisiones con todo menos KinematicBody
		foreach (Godot.Collections.Dictionary result in queryResult)
		{
			if (result["collider"] is not KinematicBody2D)
			{
				return true;
			}
		}

		return false;
	}


	public static Thrower GetThrower(Throwable _throwable)
	{
		PackedScene lanzador=(PackedScene)ResourceLoader.Load("res://scenes/Thrower.tscn");
		Thrower thrower=(Thrower)lanzador.Instance();
		thrower.throwable=_throwable;

		return thrower;
	}

	public static Thrower GetThrower(Throwable _throwable, float _lineWidth)
	{
		PackedScene lanzador=(PackedScene)ResourceLoader.Load("res://scenes/Thrower.tscn");
		Thrower thrower=(Thrower)lanzador.Instance();
		thrower.throwable=_throwable;
		thrower.lineWidth=_lineWidth;

		return thrower;
	}

	private void _on_Area2D_body_entered(Node body)
	{
		if(body is KinematicBody2D && body!=throwable && body!=Inventory.SelectedPlayer)
		{
			//GD.Print("Apagalo otto");
			collidingBodies.Add(body);
		}

	}

	private void _on_Area2D_body_exited(Node body)
	{

		if(body is KinematicBody2D && body!=throwable)
		{
			//GD.Print("Préndelo otto");
			collidingBodies.Remove(body);
		}
		
	}

	private void RestartLaunch()
	{

		Position=throwable.GlobalPosition;
		Position+=offset;
		line.ClearPoints();
		RemoveCollisions();
		//moving=false;
		restartSound.Play();
	}

	private void MouseReleased()
	{
		selected=false;
		
		if(collidingBodies.Count>0)
		{
			RestartLaunch();
			return;
		}

		throwable.SetVelocity(initialVel);
		QueueFree();

		if(throwable is Jugador || throwable is GloboConAgua) 
		{
			return;
		}
		
		GetTree().CallGroup("Escenarios", "ChangeTurn");
	}
	
	private void _on_Thrower_input_event(object viewport, object @event, int shape_idx)
	{
		if(@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left)
			{
				if(MouseButtonEvent.Pressed)
				{
					startPos=GetGlobalMousePosition();
					startPos-=offset;
					selected=true;
				}
				else{
					MouseReleased();
				}
			}
		}
/* 		if(@event is InputEventMouseMotion MouseMotionEvent)
		{
			GD.Print("Se mueve");
			moving=selected;
		} */

		
	}
}



