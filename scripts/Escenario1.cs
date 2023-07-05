using Godot;
using System;

public class Escenario1 : Escenario
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    Jugador P;
    public override void _Ready()
    {
        base._Ready();
        P=GetNode<KinematicBody2D>("Jugador") as Jugador;
        var C=GetNode<CollisionShape2D>("CollisionShape2D");
        var dis=P.Position.DistanceTo(C.Position);
        GD.Print(dis);
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        base._Process(delta);
      
    }


    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        // P.velocidad+=new Vector2(100,0);
        if(Input.IsActionJustReleased("Up"))
        {
        }

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
    }

    private void _on_Zoom_pressed()
	{
		if(Camara.Zoom>new Vector2((float)0.4,(float)0.4))
		{
			Camara.Zoom-=Zoom;
		}
	}


	private void _on_UnZoom_pressed()
	{
		if(Camara.Zoom<new Vector2((float)1.8,(float)1.8))
		{
			Camara.Zoom+=Zoom;
		}
	}

    public void Reanudar()
    {
        base.Reanudar();
    }
}
