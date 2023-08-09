using Godot;
using System;
using System.Collections.Generic;

public class GloboTeledirigido : GloboConAgua
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    readonly float speed=400;
    bool exploded=false;

    float angle=0;
    List<Jugador> playersInRange = new();

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
        
/*         float movementAngle=Mathf.Rad2Deg(velocity.Angle());
        var player=GetClosestPlayer();
        angle=player is null ? 0 : Mathf.Rad2Deg((player.GlobalPosition-GlobalPosition).Angle());

        if(angle!=0 && movementAngle>=angle-50 && movementAngle<=angle+50)
        {
            Vector2 drawBack=(player.GlobalPosition.DirectionTo(GlobalPosition));
            drawBack*=20;
            velocity=drawBack;
            MoveAndSlide(velocity);
            return;
        } */

        if(Input.IsActionPressed("LeftClick"))
        {
            velocity=Position.DirectionTo(GetGlobalMousePosition()).Normalized();
        }

        float movementAngle=Mathf.Rad2Deg(velocity.Angle());
        CalculateAverageAngle();

        if(angle!=0 && movementAngle>=angle-50 && movementAngle<=angle+50)
        {
            return;
        }

		velocity.x*=speed;
		velocity.y*=speed;


        velocity=MoveAndSlide(velocity, Vector2.Up);
    }
    private void CalculateAverageAngle()
    {
        if (playersInRange.Count == 0)
        {
            angle = 0;
            return;
        }

        float totalAngle = 0;
        foreach (Jugador player in playersInRange)
        {
            totalAngle += player.GlobalPosition.AngleToPoint(GlobalPosition);
        }

        angle = Mathf.Rad2Deg(totalAngle / playersInRange.Count);
    }
    private Jugador GetClosestPlayer()
    {
        float closestDistance = float.MaxValue;
        Jugador closestPlayer = null;

        foreach (Jugador player in playersInRange)
        {
            float distance = GlobalPosition.DistanceSquaredTo(player.GlobalPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
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


    private void _on_Detector_body_entered(Node body)
    {
        if(body is Jugador player)
        {
            playersInRange.Add(player);
        }
    }

    private void _on_Detector_body_exited(Node body)
    {
        if(body is Jugador jugador)
        {
            playersInRange.Remove(jugador);
        }
    }


}
