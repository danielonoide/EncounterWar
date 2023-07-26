using Godot;
using System;

public class Platano : Throwable
{
    RectangleShape2D rectangleShape2D;
    bool dropped=false;
    AudioStreamPlayer restartSound;

    bool flag=true;
    public override void _Ready()
    {
        restartSound=GetNode<AudioStreamPlayer>("LaunchRestartSound");
        rectangleShape2D=new();
        rectangleShape2D.Extents=new Vector2(1,59);
    }

    public override void _Process(float delta)
    {
        if(!dropped)Position=GetGlobalMousePosition();
    }

    public override void _PhysicsProcess(float delta)
    {
        if(velocity!=Vector2.Zero) 
        {
            base._PhysicsProcess(delta);
            GetNode<Area2D>("Entro").Monitoring=true;
        }


        

/*         if(IsOnFloor() && flag)
        {
            //GetNode<CollisionShape2D>("CollisionShape2D").Disabled=true;

            velocity=Vector2.Zero;
            GetTree().CallGroup("Escenarios", "ChangeTurn");
            flag=false;
        } */
    }

    private bool CanDrop()
    {
        Vector2 position=Position;
        bool platformTouched=false;
        while(position.y<1200)
        {
            Physics2DShapeQueryParameters queryParameters = new Physics2DShapeQueryParameters();
            queryParameters.Transform = new Transform2D(1, position);

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


    private void _on_Entro_body_entered(Node body)
    {
        GD.Print(body);
        if(body is Jugador jugador)
        {
            jugador.HasToFall=true;
            QueueFree();
        }
        else
        {
            velocity=Vector2.Zero;
            GetTree().CallGroup("Escenarios", "ChangeTurn");
        }
    }


    private void _on_Entro_input_event(object viewport, object @event, int shape_idx)
    {
        if(@event is InputEventMouseButton mouseButton && !mouseButton.Pressed)
        {
            if(CanDrop())
            {
                dropped=true;
                SetVelocity(new Vector2(0,-1));
            }
            else
            {
                restartSound.Play();
            }
        }
    }

}
