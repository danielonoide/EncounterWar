using Godot;
using System;

public class Thrower : Area2D
{
	Vector2 StartPos=new Vector2(0,0);
	Vector2 EndPos=new Vector2(0,0);
	Vector2 Direction=new Vector2(0,0);
	float Speed=0, angle;
	bool Selected=false, Moving=false;
	public Jugador Ball;
	Line2D Linea;

    Vector2 Vel=new Vector2(0,0);
    Vector2 VelIni=new Vector2(0,0);

	PackedScene mapaScene = (PackedScene)ResourceLoader.Load("res://scenes/Escenario3.tscn");
	Node mapa;
	TileMap tileMap;	
	Area2D colisionador;

	CircleShape2D circleShape2D;


	public override void _Ready()
	{
		Linea=GetNode<Line2D>("Line2D");
		mapa = mapaScene.Instance();
		tileMap = mapa.GetNode<TileMap>("TileMap");
		colisionador=GetNode<Area2D>("Colisionador");

		circleShape2D=new();
		circleShape2D.Radius=10;
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
			for(int i=0;i<colisionador.GetChildCount();i++)
			{
				colisionador.GetChild(i).QueueFree();
			}
			//Trajectory
			Linea.ClearPoints();
            Linea.AddPoint(new Vector2(0,0));
            //GetNode<Sprite>("Sprite2").GlobalPosition=StartPos;
			//Linea.AddPoint(new Vector2(0,0));
			Direction=(StartPos-GetGlobalMousePosition()).Normalized();
			Speed=StartPos.DistanceTo(GetGlobalMousePosition())*2; //ajuste
			if(Speed>800) //delimitar
			{
				Speed=800;
			}

			angle=Direction.Angle();
			//Vector2 BallCenter = StartPos+new Vector2(0,-35);
            Vel=Direction*Speed;
            VelIni=Direction*Speed;
		    Vector2 NewPos=StartPos-Position;
			for(int i=0;i<300;i++)
			{
				Linea.AddPoint(NewPos);
                Vel.y+=Globals.Gravity*delta;
                NewPos+=Vel*delta;
/* 				if(!CheckIfFree(ToGlobal(NewPos)))
				{
					break;
				} */

				if(IsColliding(ToGlobal(NewPos))) break;

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
			
			//rectangle collision
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
				colisionador.AddChild(Collision);


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
			colisionador.AddChild(collision);

		}


/* 		var areas = area2D.GetOverlappingAreas();

		foreach(Area2D area in areas)
		{
			int closestPointIndex = -1;
			float closestDistance = float.MaxValue;
			CollisionShape2D collisionShape2D=area.GetNode<CollisionShape2D>("CollisionShape2D");
			Shape2D shape2D=collisionShape2D.Shape;
			var pos= shape2D.CollideAndGetContacts(area2D.Transform, area2D.GetChild<CollisionShape2D>(area2D.GetChildCount()-1).Shap, area.Transform);
				GD.Print(pos[0]);

			for (int i = 0; i < Linea.GetPointCount(); i++)
			{
				Vector2 point = Linea.GetPointPosition(i);
				float distance = area.Position.DistanceTo(ToGlobal(point));

				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestPointIndex = i;
				}
			}
			for(int i=Linea.GetPointCount()-1;i>closestPointIndex;i--)
			{
				Linea.RemovePoint(i);
			}
		}		 */

/* 		var bodies = area2D.GetOverlappingBodies();

		foreach(var body in bodies)
		{
			int closestPointIndex = -1;
			float closestDistance = float.MaxValue;
			StaticBody2D staticBody2D;
			if(body is StaticBody2D) staticBody2D=(StaticBody2D)body;
			else continue; 
			Vector2 bodyPos=staticBody2D.GetCollisionPoint();

			for (int i = 0; i < Linea.GetPointCount(); i++)
			{
				Vector2 point = Linea.GetPointPosition(i);
				float distance = staticBody2D.Position.DistanceTo(ToGlobal(point));

				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestPointIndex = i;
				}
			}
			for(int i=Linea.GetPointCount()-1;i>closestPointIndex;i--)
			{
				Linea.RemovePoint(i);
			}
		}		
 */
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

	private bool IsColliding(Vector2 position)
	{
		// Obtiene el estado del espacio físico
		//Physics2DDirectSpaceState spaceState =new();
		// Define el área de forma rectangular alrededor de la posición

		// Realiza una consulta para verificar las colisiones con los RigidBody2D
		Physics2DShapeQueryParameters queryParameters=new(); //los parámetros de la query
		queryParameters.Transform=new Transform2D(1, position); //la posición que se va a checar
		queryParameters.SetShape(circleShape2D); //la forma que va a tener el shape que va a colisionar
		Physics2DDirectSpaceState spaceState=GetWorld2d().DirectSpaceState; //objeto para hacer queries del espacio físico 2D

		var queryResult = spaceState.IntersectShape(queryParameters);

		//return queryResult.Count>0;
		// Verifica si se encontraron colisiones con los RigidBody2D
		foreach (Godot.Collections.Dictionary result in queryResult)
		{
			if (result["collider"] is not KinematicBody2D)
			{
				return true;
			}
		}

		return false;
	}

	private bool IsCollidingWithArea2D(Vector2 position)
	{
		// Crea un área de colisión en la posición dada
		Area2D collisionArea = new Area2D();
		collisionArea.Position = position;

		// Crea una forma de colisión para el área
		CollisionShape2D collisionShape = new CollisionShape2D();
		// Asigna la forma deseada (por ejemplo, un círculo)
		CircleShape2D circle=new(); 
		circle.Radius=1;
		collisionShape.Shape = circle;

		// Agrega la forma de colisión al área
		collisionArea.AddChild(collisionShape);

		// Obtiene una lista de las áreas superpuestas
		Godot.Collections.Array overlappingAreas = collisionArea.GetOverlappingAreas();

		// Verifica si se encontraron áreas superpuestas
		return overlappingAreas.Count > 0;
	}

	private void _on_Area2D_area_entered(Area2D area)
	{
		
		int closestPointIndex = -1;
		float closestDistance = float.MaxValue;

		for (int i = 0; i < Linea.GetPointCount(); i++)
		{
			Vector2 point = Linea.GetPointPosition(i);
			float distance = area.Position.DistanceTo(point);

			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestPointIndex = i;
			}
		}
		GD.Print(closestPointIndex);
		for(int i=closestPointIndex;i<Linea.GetPointCount();i++)
		{
			Linea.RemovePoint(i);
		}
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

	public static Thrower GetThrower()
	{
		PackedScene Lanzador=(PackedScene)ResourceLoader.Load("res://scenes/Thrower2.tscn");
		return (Thrower)Lanzador.Instance();
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
					StartPos+=new Vector2(0, -35);
					Selected=true;
				}
				else{
					Selected=false;
					Ball.setVelocidad(VelIni);
					GetTree().CallGroup("Escenarios", "ChangeTurn");
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



