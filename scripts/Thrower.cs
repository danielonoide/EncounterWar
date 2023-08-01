using Godot;
using System;
using System.Collections.Generic;

public class Thrower : ProjectileLauncher
{
    // Configuración
    private Vector2 offset = new Vector2(0, 35);
    private float maxSpeed = 800;

    // Lógica del lanzador
    private Vector2 startPos = Vector2.Zero;

    // Referencias
    public Throwable throwable;

    protected override Vector2 StartingPoint { get => startPos-Position; }

    public override void _Ready()
    {
        base._Ready();
        Position = throwable.GlobalPosition;
        Position += offset;
    }

    public override void _Process(float delta)
    {
        if (selected)
        {
            Position = GetGlobalMousePosition();
			
			if (Input.IsActionJustReleased("LeftClick"))
            {
				if(!CorrectAngle() && throwable is not Jugador)
				{
					RestartLaunch();
					return;
				}
                MouseReleased();
            }
        }
    }


    protected override void CalculateInitialVelocity()
    {
        direction = (startPos - GetGlobalMousePosition()).Normalized();
        speed = Mathf.Clamp(startPos.DistanceTo(GetGlobalMousePosition()) * 2, 0, maxSpeed);  //*2 es un ajuste

        float angle = direction.Angle();
		degAngle=Mathf.Rad2Deg(angle);
        Vector2 velocity = direction * speed;
        initialVelocity = direction * speed;
    }

    private void MouseReleased()
    {
        selected = false;

        if (collidingBodies.Count > 0 || !canThrow)
        {
            RestartLaunch();
            return;
        }

        throwable.SetVelocity(initialVelocity);
        QueueFree();

        if (throwable is Jugador)
        {
			Inventory.Unopenable=false;
            return;
        }

		//Inventory.Unopenable=false;


        GetTree().CallGroup("Escenarios", "ChangeTurn");
    }

    protected override void RestartLaunch()
    {
		selected=false;
        Position = throwable.GlobalPosition;
        Position += offset;
        line.ClearPoints();
        RemoveCollisions();
        restartSound.Play();
    }

/*     protected new void _on_Colisionador_body_entered(Node body)
    {
        base._on_Colisionador_body_entered(body);
        if(throwable is Jugador && body is Teleporter)
        {
            collidingBodies.Add(body);
        }
    } */


/*     protected new void _on_Colisionador_body_exited(Node body)
    {
        base._on_Colisionador_body_entered(body);
        if(body is Teleporter)
        {
            collidingBodies.Remove(body);
        }
    } */

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
/*                 else
                {
                    MouseReleased();
                } */
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
