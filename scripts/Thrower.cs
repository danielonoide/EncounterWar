using Godot;
using System;
using System.Collections.Generic;

public class Thrower : Area2D
{
    // Configuración
    private Vector2 offset = new Vector2(0, 35);
    private float lineWidth = 10;
    private float maxSpeed = 800;

    // Lógica del lanzador
    private Vector2 startPos = Vector2.Zero;
    private Vector2 direction = Vector2.Zero;
    private float speed = 0;
    private float angle = 0;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 initialVelocity = Vector2.Zero;
    private bool selected = false;

    // Referencias
    private Line2D line;
    private Area2D collisionArea;
    private RectangleShape2D collisionShape;
    private List<Node> collidingBodies = new List<Node>();
    private Throwable throwable;
    private AudioStreamPlayer restartSound;

    public override void _Ready()
    {
        Position = throwable.GlobalPosition;
        Position += offset;

        line = GetNode<Line2D>("Line2D");
        line.Width = lineWidth;

        restartSound = GetNode<AudioStreamPlayer>("LaunchRestartSound");
        collisionArea = GetNode<Area2D>("Colisionador");
        collisionShape = new RectangleShape2D();
        collisionShape.Extents = new Vector2(1, lineWidth / 2);
    }

    public override void _Process(float delta)
    {
        if (selected)
        {
            Position = GetGlobalMousePosition();
            if (Input.IsActionJustReleased("LeftClick"))
            {
                MouseReleased();
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!selected) return;

        RemoveCollisions();

        // Lógica de la trayectoria
        UpdateTrajectory(delta);

        // Lógica de la colisión
        int points = line.GetPointCount();
        int j = 1;
        while (j < points - 10)
        {
            CreateCollisionShape((line.GetPointPosition(j) + line.GetPointPosition(j + 10)) / 2, line.GetPointPosition(j).DirectionTo(line.GetPointPosition(j + 10)).Angle());
            j += 10;
        }

        int difference = points - (points - j);
        CreateCollisionShape((line.GetPointPosition(difference) + startPos) / 2, line.GetPointPosition(difference).DirectionTo(startPos).Angle());
    }

    private void UpdateTrajectory(float delta)
    {
        line.ClearPoints();
        line.AddPoint(Vector2.Zero);

        direction = (startPos - GetGlobalMousePosition()).Normalized();
        speed = Mathf.Clamp(startPos.DistanceTo(GetGlobalMousePosition()) * 2, 0, maxSpeed);

        angle = direction.Angle();
        velocity = direction * speed;
        initialVelocity = direction * speed;

        Vector2 newPos = startPos - Position;

        for (int i = 0; i < 300; i++)
        {
            line.AddPoint(newPos);
            velocity.y += Globals.Gravity * delta;
            newPos += velocity * delta;

            float lineAngle = i > 0 ? line.GetPointPosition(i - 1).DirectionTo(line.GetPointPosition(i)).Angle() : angle;

            if (IsColliding(ToGlobal(newPos), lineAngle)) break;
        }
    }

    private void CreateCollisionShape(Vector2 position, float angle)
    {
        CollisionShape2D collision = new CollisionShape2D();
        collision.Position = position;
        collision.Rotation = angle;
        collision.Shape = collisionShape;
        collisionArea.AddChild(collision);
    }

    private void RemoveCollisions()
    {
        foreach (CollisionShape2D collision in collisionArea.GetChildren())
        {
            collision.QueueFree();
        }
    }

    private bool IsColliding(Vector2 position, float angle)
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
                return true;
            }
        }

        return false;
    }

    private void MouseReleased()
    {
        selected = false;

        if (collidingBodies.Count > 0)
        {
            RestartLaunch();
            return;
        }

        throwable.SetVelocity(initialVelocity);
        QueueFree();

        if (throwable is Jugador || throwable is GloboConAgua)
        {
            return;
        }

        GetTree().CallGroup("Escenarios", "ChangeTurn");
    }

    private void _on_Area2D_body_entered(Node body)
    {
        if (body is KinematicBody2D && body != throwable && body != Inventory.SelectedPlayer)
        {
            collidingBodies.Add(body);
        }
    }

    private void _on_Area2D_body_exited(Node body)
    {
        if (body is KinematicBody2D && body != throwable)
        {
            collidingBodies.Remove(body);
        }
    }

    private void RestartLaunch()
    {
        Position = throwable.GlobalPosition;
        Position += offset;
        line.ClearPoints();
        RemoveCollisions();
        restartSound.Play();
    }

    private void _on_Thrower_input_event(object viewport, object @event, int shape_idx)
    {
        if (@event is InputEventMouseButton MouseButtonEvent)
        {
            if (MouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
            {
                if (MouseButtonEvent.Pressed)
                {
                    startPos = GetGlobalMousePosition();
                    startPos -= offset;
                    selected = true;
                }
                else
                {
                    MouseReleased();
                }
            }
        }
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

}
