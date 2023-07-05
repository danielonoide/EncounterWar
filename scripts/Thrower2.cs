using Godot;
using System;

public class Thrower2 : Area2D
{
	Vector2 StartPos=new Vector2(0,0);
	Vector2 EndPos=new Vector2(0,0);
	Vector2 Direction=new Vector2(0,0);
	float Speed=0, Angle;
	bool Selected=false, Moving=false;
	public Jugador Ball;
	Line2D Linea;

    Vector2 Vel=new Vector2(0,0);
    Vector2 VelIni=new Vector2(0,0);

	PackedScene mapaScene = (PackedScene)ResourceLoader.Load("res://scenes/Escenario3.tscn");
	Node mapa;
	TileMap tileMap;
	int gravedad=500;
	
	Area2D colisionador;
	public override void _Ready()
	{
		Linea=GetNode<Line2D>("Line2D");
		mapa = mapaScene.Instance();
		tileMap = mapa.GetNode<TileMap>("TileMap");
		colisionador=GetNode<Area2D>("Colisionador");
	}
	
	public override void _Process(float delta)
	{
		if(Selected)
		{
			Position=GetGlobalMousePosition();
		}
		else if(Speed!=0){    //improve
			QueueFree();
			Jugador.ThrowerGenerated=false;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if(Moving)
		{
			var Area2d= GetNode<Area2D>("Area2D");
			for(int i=0;i<Area2d.GetChildCount();i++)
			{
				Area2d.GetChild(i).QueueFree();
			}
			//Trajectory
			Linea.ClearPoints();
            Linea.AddPoint(new Vector2(0,0));
            GetNode<Sprite>("Sprite2").GlobalPosition=StartPos;
			//Linea.AddPoint(new Vector2(0,0));
			Direction=(StartPos-GetGlobalMousePosition()).Normalized();
			Speed=StartPos.DistanceTo(GetGlobalMousePosition())*2; //ajuste
			if(Speed>800) //delimitar
			{
				Speed=800;
			}

			Angle=Direction.Angle();
			Vector2 BallCenter = StartPos+new Vector2(0,-35);
            Vel=Direction*Speed;
            VelIni=Direction*Speed;
		    Vector2 NewPos=BallCenter-Position;
			for(int i=0;i<300;i++)
			{
				Linea.AddPoint(NewPos);
                Vel.y+=gravedad*delta;
                NewPos+=Vel*delta;
				if(!CheckIfFree(ToGlobal(NewPos)))
				{
					break;
				}
			}
			int Points=Linea.GetPointCount();
			//segment collision
/* 			for(int i=1;i<Points-10;i+=10)
			{
				CollisionShape2D Collision=new CollisionShape2D();
				SegmentShape2D Segment=new SegmentShape2D();
				Segment.A=Linea.GetPointPosition(i);
				Segment.B=Linea.GetPointPosition(i+10);
				Collision.Shape=Segment;

				GetNode<Area2D>("Area2D").AddChild(Collision);
			}

			CollisionShape2D collision=new CollisionShape2D();
			SegmentShape2D segment=new SegmentShape2D();
			segment.A=Linea.GetPointPosition(Points-1);
			segment.B=NewPos;
			collision.Shape=segment;

			GetNode<Area2D>("Area2D").AddChild(collision); */
			
			//rectange collision
			int j=1;
			while(j<Points-10)
			{
				CollisionShape2D Collision=new CollisionShape2D();
				RectangleShape2D Rect=new RectangleShape2D();
				Collision.Position=(Linea.GetPointPosition(j)+Linea.GetPointPosition(j+10))/2;
				Collision.Rotation=Linea.GetPointPosition(j).DirectionTo(Linea.GetPointPosition(j+10)).Angle();
				var Longitud=Linea.GetPointPosition(j).DistanceTo(Linea.GetPointPosition(j+10));
				Rect.Extents=new Vector2(Longitud/2,10);
				Collision.Shape=Rect;
				GetNode<Area2D>("Area2D").AddChild(Collision);

				j+=10;
			}
			int diferencia=Points-(Points-j);

			CollisionShape2D collision=new CollisionShape2D();
			RectangleShape2D rect=new RectangleShape2D();
			collision.Position=(Linea.GetPointPosition(diferencia)+NewPos)/2;
			collision.Rotation=Linea.GetPointPosition(diferencia).DirectionTo(NewPos).Angle();
			var longitud=Linea.GetPointPosition(diferencia).DistanceTo(NewPos);
			rect.Extents=new Vector2(longitud/2,10);
			collision.Shape=rect;
			GetNode<Area2D>("Area2D").AddChild(collision);
			


		}
	}

	public bool CheckIfFree(Vector2 posicion)
	{
		Vector2 cellPos = tileMap.WorldToMap(posicion); // Obtener la posición de celda correspondiente a la posición del mundo
		if (tileMap.GetCellv(cellPos) == -1) // Comprobar si hay un tile en la posición de celda
		{
			return true; // No hay un tile en la posición, se puede continuar
		}
		else
		{
			return false; // Hay un tile en la posición, no se puede continuar
		}

			//al mover un TileMap de la posición (0,0), es posible que la función WorldToMap no funcione correctamente, 
			//ya que esta función asume que el TileMap está en la posición (0,0) y realiza cálculos basados en esa suposición.
	}

/* 	public bool CheckIfFree(Vector2 posicion) //se lagea de a madres
	{
		Vector2 tileMapPos = tileMap.GlobalPosition; // Obtener la posición global del TileMap
		Vector2 localPos = posicion - tileMapPos; // Ajustar la posición para tener en cuenta la posición del TileMap
		Vector2 cellPos = tileMap.WorldToMap(localPos); // Obtener la posición de celda correspondiente a la posición local ajustada
		if (tileMap.GetCellv(cellPos) == -1) // Comprobar si hay un tile en la posición de celda
		{
			return true; // No hay un tile en la posición, se puede continuar
		}
		else
		{
			return false; // Hay un tile en la posición, no se puede continuar
		}
	} */



/* 	public bool CheckIfFree(Vector2 posicion)
	{
		var Params = new Physics2DShapeQueryParameters();
		Params.SetShape(new CircleShape2D() { Radius = 1.0f });
		Params.Transform = new Transform2D(posicion, new Vector2(1, 1), new Vector2(1,1));


		var spaceState=GetWorld2d().DirectSpaceState;
		var queryResults=spaceState.IntersectShape(Params);
		foreach(Godot.Collections.Dictionary result in queryResults)
		{
			GD.Print(result["collider"]);
			if(result["collider"] is TileMap)
			{
				return false;
			}
		}

		return true;

	} */

/* 	public bool CheckIfFree(Vector2 posicion)
	{
		colisionador.Position=posicion;
		var nodos=colisionador.GetOverlappingBodies();
		foreach(var nodo in nodos)
		{
			GD.Print(nodo);
			if(nodo is TileMap)
			{
				return false;
			}
		}

		return true;
	} */

	public static Thrower2 GetThrower()
	{
		PackedScene Lanzador=(PackedScene)ResourceLoader.Load("res://scenes/Thrower2.tscn");
		return (Thrower2)Lanzador.Instance();
	}

	private void _on_Area2D_body_entered(object body)
	{

		if(body is KinematicBody2D && body!=Ball)
		{
			GD.Print("Apagalo otto");
		}

	}

	private void _on_Area2D_body_exited(object body)
	{

		if(body is KinematicBody2D && body!=Ball)
		{
			GD.Print("Préndelo otto");
		}
		
	}
	
	private void _on_Thrower2_input_event(object viewport, object @event, int shape_idx)
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
					Ball.setVelocidad(VelIni);
				}
			}
		}
		
		if(@event is InputEventMouseMotion MouseMotionEvent && Selected)
		{
			Moving=true;
		
		}
		else{
			Moving=false;
		}
		
	}
}



