using Godot;
using System;
using System.Collections.Generic;

public abstract class ProjectileLauncher : Area2D
{

    protected bool selected=false;
    protected float lineWidth=10;
    protected Vector2 direction = Vector2.Zero;
    protected Vector2 initialVelocity = Vector2.Zero;
    protected float speed=0;
    protected List<Node> collidingBodies = new List<Node>();
    protected Area2D collisionArea;
    protected Line2D line;
    protected RectangleShape2D collisionShape;
    protected AudioStreamPlayer restartSound;

	protected float degAngle=0;

    protected abstract Vector2 StartingPoint{get; }

    protected abstract void RestartLaunch();

    protected abstract void CalculateInitialVelocity();


    public override void _Ready()
    {
        line = GetNode<Line2D>("Line2D");
        line.Width = lineWidth;

        restartSound = GetNode<AudioStreamPlayer>("LaunchRestartSound");
        collisionArea = GetNode<Area2D>("Colisionador");
        collisionShape = new RectangleShape2D();
        collisionShape.Extents = new Vector2(1, lineWidth / 2);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!selected) return;

        RemoveCollisions();

        // Lógica de la trayectoria
        Vector2 finalPos=UpdateTrajectory(delta);

        // Lógica de la colisión
        int points = line.GetPointCount();
        int j = 1;
        while (j < points - 10)
        {
			var longitud=line.GetPointPosition(j).DistanceTo(line.GetPointPosition(j+10));
            CreateCollisionShape((line.GetPointPosition(j) + line.GetPointPosition(j + 10)) / 2, new Vector2(longitud/2, lineWidth/2) ,line.GetPointPosition(j).DirectionTo(line.GetPointPosition(j + 10)).Angle());
            j += 10;
        }

        int difference = points - (points - j);
		var Longitud=line.GetPointPosition(difference).DistanceTo(finalPos);
        CreateCollisionShape((line.GetPointPosition(difference) + finalPos) / 2, new Vector2(Longitud/2, lineWidth/2) ,line.GetPointPosition(difference).DirectionTo(finalPos).Angle());
    }

    protected bool IsColliding(Vector2 position, float angle)
    {
        Physics2DShapeQueryParameters queryParameters = new Physics2DShapeQueryParameters();
        queryParameters.Transform = new Transform2D(angle, position);


        queryParameters.SetShape(collisionShape);
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;

        var queryResult = spaceState.IntersectShape(queryParameters);

        foreach (Godot.Collections.Dictionary result in queryResult)
        {
            if (result["collider"] is not KinematicBody2D)
            {
                GD.Print(result["collider"]);
                return true;
            }
        }

        return false;
    }

    protected void RemoveCollisions()
    {
        foreach (CollisionShape2D collision in collisionArea.GetChildren())
        {
            collision.QueueFree();
        }
    }

    protected void CreateCollisionShape(Vector2 position, Vector2 extents, float angle)
    {
        CollisionShape2D collision = new CollisionShape2D();
        collision.Position = position;
        collision.Rotation = angle;
		RectangleShape2D rectangleShape2D=new();
		rectangleShape2D.Extents=extents;
        collision.Shape = rectangleShape2D;
		
        collisionArea.AddChild(collision);
    }

    protected Vector2 UpdateTrajectory(float delta)
    {
        line.ClearPoints();
        line.AddPoint(Vector2.Zero);

        CalculateInitialVelocity();

        Vector2 velocity=initialVelocity;
        Vector2 newPos = StartingPoint; //starting point


        for (int i = 0; i < 300; i++)
        {
            line.AddPoint(newPos);
            velocity.y += Globals.Gravity * delta;
            newPos += velocity * delta;

            float lineAngle = i > 0 ? line.GetPointPosition(i - 1).DirectionTo(line.GetPointPosition(i)).Angle() : Mathf.Deg2Rad(degAngle);

            if (IsColliding(ToGlobal(newPos), lineAngle)) break;
        }
/*         Vector2 endPoint=new(newPos);
        endPoint-=velocity*delta;
        endPoint+=new Vector2(0,10);
        line.AddPoint(endPoint); */

		return newPos;
    }


    protected bool CorrectAngle()
    {
        if((degAngle<=-85 && degAngle>=-90) || (degAngle>=12 && degAngle<=150))
        {
            return false;
        }

        return true;
    }

    protected void _on_Colisionador_body_entered(Node body)
    {
        if (body is KinematicBody2D && body != Inventory.SelectedPlayer || body is Teleporter) //&& body!=throwable estaba agregado 
        {
            collidingBodies.Add(body);
        }
    }

    protected void _on_Colisionador_body_exited(Node body)
    {
        if (body is KinematicBody2D || body is Teleporter)
        {
            collidingBodies.Remove(body);
        }
    }

}
