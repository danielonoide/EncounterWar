using Godot;
using System;

public class AstronautsSpecial : Area2D
{

    RectangleShape2D rectangleShape2D;
    Line2D line;
    CollisionShape2D collisionShape2D;
    AudioStreamPlayer launchRestart;
    public override void _Ready()
    {
        Inventory.Unopenable=true;
        line=GetNode<Line2D>("Line2D");
        launchRestart=GetNode<AudioStreamPlayer>("LaunchRestartSound");
        collisionShape2D=GetNode<CollisionShape2D>("CollisionShape2D");

        rectangleShape2D = new()
        {
            Extents = new Vector2(16, 1)
        };
    }

    public override void _Process(float delta)
    {
        //GD.Print(canDrop);
        collisionShape2D.GlobalPosition=GetGlobalMousePosition();
        Position=new Vector2(GetGlobalMousePosition().x, Position.y);
    }


    public override void _PhysicsProcess(float delta)
    {
        DrawLine();
    }

    private void DrawLine()
    {
        line.ClearPoints();
        line.AddPoint(Vector2.Zero);
        line.AddPoint(ToLocal(LineEnding()));
    }

    private Vector2 LineEnding() 
	{
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
        Vector2 bottomLimit=new Vector2(Position.x, 1200);
		Godot.Collections.Dictionary queryResult=
		spaceState.IntersectRay(Position, bottomLimit, new Godot.Collections.Array{this}, 1);

        if(queryResult.Count==0) return bottomLimit;
        
        Vector2 result=(Vector2)queryResult["position"];
        result.y-=80; //nose porq

        return result;

	}


    private bool CanDrop()
    {
        Vector2 position=GlobalPosition;
        bool platformTouched=false;

        while(position.y<1200)
        {
            Physics2DShapeQueryParameters queryParameters = new Physics2DShapeQueryParameters();
            queryParameters.Transform = new Transform2D(0, position);

            queryParameters.SetShape(rectangleShape2D);
            Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;

            var queryResult = spaceState.IntersectShape(queryParameters);

            foreach (Godot.Collections.Dictionary result in queryResult)
            {
                if(result["collider"] is Jugador && !platformTouched)
                {
                    return false;
                }

                if (result["collider"] is not KinematicBody2D)
                {
                    platformTouched=true;
                }
            }

            position.y++;
        }

        return true;

    }


    private void _on_SpaceShip_input_event(object viewport, object @event, int shape_idx)
    {
        if(@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex==(int)ButtonList.Left & mouseButton.Pressed)
        {
            if(CanDrop())
            {
                GetTree().CallGroup("Escenarios", "ChangeTurn");
                LaunchBalloon();
                QueueFree();
            }
            else
            {
                launchRestart.Play();
            }
        }
    }

    private void LaunchBalloon()
    {
        GloboConAgua globoConAgua=GloboConAgua.GetSpecialWaterBalloon();

        globoConAgua.Position=GlobalPosition;
        globoConAgua.SetVelocity(new Vector2(0,1));

        Escenario escenario=GetTree().Root.GetNode<Escenario>("Escenario");
        escenario.AddChild(globoConAgua);
    }


    public static AstronautsSpecial GetAstronautsSpecial()
    {
        PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/AstronautsSpecial.tscn");
        AstronautsSpecial astronautsSpecial=(AstronautsSpecial)scene.Instance();
        return astronautsSpecial;
    }
}
