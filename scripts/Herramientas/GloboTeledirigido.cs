using Godot;
using System;
using System.Collections.Generic;

public class GloboTeledirigido : GloboConAgua
{
    readonly float speed=400;
    bool exploded=false;

    AudioStreamPlayer2D startingSound;

    public override void _Ready()
    {
        base._Ready();
        startingSound=GetNode<AudioStreamPlayer2D>("StartingSound");

    }

    public override void _PhysicsProcess(float delta)
    {
        if(!exploded) Movement(delta);
        
    }

    protected override void Movement(float delta)
    {
        velocity.x=Convert.ToInt32(Input.IsActionPressed("Right")) - Convert.ToInt32(Input.IsActionPressed("Left"));
		velocity.y=Convert.ToInt32(Input.IsActionPressed("Down")) - Convert.ToInt32(Input.IsActionPressed("Up"));
        

        if(Input.IsActionPressed("LeftClick"))
        {
            velocity=Position.DirectionTo(GetGlobalMousePosition()).Normalized();
        }


		velocity.x*=speed;
		velocity.y*=speed;


        //velocity=MoveAndSlide(velocity, Vector2.Up);

        var collisionInfo= MoveAndCollide(velocity*delta, testOnly:true);

        if(collisionInfo==null)
        {
            MoveAndSlide(velocity);
        }
    }

    protected new void _on_Collision_body_entered(Node body)
    {
        if(body is KinematicBody2D)
        {
            return;
        }
        startingSound.Stop();
        base._on_Collision_body_entered(body);
        GetTree().CallGroup("Escenarios", "ChangeTurn");
        exploded=true;
    }

    protected new void _on_Explosion_body_entered(Node body)
    {
        if(body is Jugador jugador)
        {
            float distance=jugador.GlobalPosition.DistanceTo(GlobalPosition);
            jugador.AddHumidity(GetHumidityPoints(distance));
            Push(jugador, distance);
            Escenario.AddStar(jugador.IsMartian, true);
        }
    }


}
