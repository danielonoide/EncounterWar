using Godot;
using System;


public class Thrower : Area2D
{
	Vector2 startPos=new Vector2(0,0);
	Vector2 endPos=new Vector2(0,0);
	Vector2 direction=new Vector2(0,0);

	Vector2 offset=new Vector2(0, 35);
	float speed=0, angle;
	bool selected=false, Moving=false;
	Line2D line;

    Vector2 vel=new Vector2(0,0);
    Vector2 initialVel=new Vector2(0,0);

	Area2D colisionador;
	//CircleShape2D circleShape2D;
	RectangleShape2D rectangleShape2D;

	float lineWidth=10;

	public Throwable throwable;


	public override void _Ready()
	{
		Position=ToGlobal(throwable.Position);
		Position+=offset;
		line=GetNode<Line2D>("Line2D");
		//mapa = mapaScene.Instance();
		//tileMap = mapa.GetNode<TileMap>("TileMap");
		colisionador=GetNode<Area2D>("Colisionador");

/* 		circleShape2D=new();
		circleShape2D.Radius=10; */
		rectangleShape2D=new();
		rectangleShape2D.Extents=new Vector2(1, lineWidth/2);
		
	}
	
	public override void _Process(float delta)
	{
		if(selected)
		{
			Position=GetGlobalMousePosition();
		}
		else if(speed!=0){    //improve
			QueueFree();
			//Jugador.ThrowerGenerated=false;
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
			line.ClearPoints();
            line.AddPoint(new Vector2(0,0));
            //GetNode<Sprite>("Sprite2").GlobalPosition=startPos;
			//line.AddPoint(new Vector2(0,0));
			direction=(startPos-GetGlobalMousePosition()).Normalized();
			speed=startPos.DistanceTo(GetGlobalMousePosition())*2; //ajuste
			if(speed>800) //delimitar
			{
				speed=800;
			}

			angle=direction.Angle();
			//Vector2 BallCenter = startPos+new Vector2(0,-35);
			line.Width=lineWidth;
            vel=direction*speed;
            initialVel=direction*speed; //lo que le paso al Throwable
		    Vector2 newPos=startPos-Position;
			for(int i=0;i<300;i++)
			{
				line.AddPoint(newPos);
                vel.y+=Globals.Gravity*delta;
                newPos+=vel*delta;
/* 				if(!CheckIfFree(ToGlobal(newPos)))
				{
					break;
				} */

				float lineAngle=i>0 ? line.GetPointPosition(i-1).DirectionTo(line.GetPointPosition(i)).Angle() : angle;

				if(IsColliding(ToGlobal(newPos), lineAngle)) break;

			}
			int Points=line.GetPointCount();
			
			//segment collision

/* 			for(int i=1;i<Points-10;i+=10)
			{
				CollisionShape2D Collision=new CollisionShape2D();
				SegmentShape2D Segment=new SegmentShape2D();
				Segment.A=line.GetPointPosition(i);
				Segment.B=line.GetPointPosition(i+10);
				Collision.Shape=Segment;

				GetNode<Area2D>("Area2D").AddChild(Collision);
			}

			CollisionShape2D collision=new CollisionShape2D();
			SegmentShape2D segment=new SegmentShape2D();
			segment.A=line.GetPointPosition(Points-1);
			segment.B=newPos;
			collision.Shape=segment;

			GetNode<Area2D>("Area2D").AddChild(collision); */
			
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


/* 		var areas = area2D.GetOverlappingAreas();

		foreach(Area2D area in areas)
		{
			int closestPointIndex = -1;
			float closestDistance = float.MaxValue;
			CollisionShape2D collisionShape2D=area.GetNode<CollisionShape2D>("CollisionShape2D");
			Shape2D shape2D=collisionShape2D.Shape;
			var pos= shape2D.CollideAndGetContacts(area2D.Transform, area2D.GetChild<CollisionShape2D>(area2D.GetChildCount()-1).Shap, area.Transform);
				GD.Print(pos[0]);

			for (int i = 0; i < line.GetPointCount(); i++)
			{
				Vector2 point = line.GetPointPosition(i);
				float distance = area.Position.DistanceTo(ToGlobal(point));

				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestPointIndex = i;
				}
			}
			for(int i=line.GetPointCount()-1;i>closestPointIndex;i--)
			{
				line.RemovePoint(i);
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

			for (int i = 0; i < line.GetPointCount(); i++)
			{
				Vector2 point = line.GetPointPosition(i);
				float distance = staticBody2D.Position.DistanceTo(ToGlobal(point));

				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestPointIndex = i;
				}
			}
			for(int i=line.GetPointCount()-1;i>closestPointIndex;i--)
			{
				line.RemovePoint(i);
			}
		}		
 */
	}

/* 	public bool CheckIfFree(Vector2 posicion)  //si jaló
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
	} */

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

		for (int i = 0; i < line.GetPointCount(); i++)
		{
			Vector2 point = line.GetPointPosition(i);
			float distance = area.Position.DistanceTo(point);

			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestPointIndex = i;
			}
		}
		GD.Print(closestPointIndex);
		for(int i=closestPointIndex;i<line.GetPointCount();i++)
		{
			line.RemovePoint(i);
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

	private void _on_Area2D_body_entered(object body)
	{

		if(body is KinematicBody2D && body!=throwable)
		{
			GD.Print("Apagalo otto");
		}

	}

	private void _on_Area2D_body_exited(object body)
	{

		if(body is KinematicBody2D && body!=throwable)
		{
			GD.Print("Préndelo otto");
		}
		
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
					selected=false;
					//Ball.setvelocidad(initialVel);
					throwable.SetVelocity(initialVel);
					if(throwable is Jugador) return;

					GetTree().CallGroup("Escenarios", "ChangeTurn");
				}
			}
		}
		
		if(@event is InputEventMouseMotion MouseMotionEvent && selected)
		{

			Moving=true;
		
		}
		else{
			Moving=false;
		}
		
	}
}



