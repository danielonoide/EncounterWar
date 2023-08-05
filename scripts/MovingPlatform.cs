using Godot;
using System;

public class MovingPlatform : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public static float Speed{get=>speed;}
    static readonly float speed=150f;
    const float start=-444f;
    readonly float limit=323f;
    public sbyte direction=1;
    Vector2 velocity=new(0,0);
    Area2D area2D;

    public override void _Ready()
    {
        area2D=GetNode<Area2D>("Area2D");
    }

    public override void _PhysicsProcess(float delta)
    {
        velocity.x=direction*speed;
        velocity.y=0;
        var collision = MoveAndCollide(velocity*delta); 
        //movimiento constante
        if(collision is not null) //no detecta encima (me sirve)
        {
            var collidingObject= collision.Collider;
            if(collidingObject is Throwable throwable)
            {
                throwable.SetVelocity(velocity);
            }
        }


        if(Position.x>=limit)
        {
            direction=-1;
        }

        if(Position.x<=start)
        {
            direction=1;
        }


        var bodies = area2D.GetOverlappingBodies();
        foreach(var body in bodies)
        {
            if(body is Jugador player)
            {
                player.OnMovingPlatform=direction;
            }
        }

    } 


    private void _on_Area2D_body_exited(Node body)
    {
        if(body is Jugador player)
        {
            player.OnMovingPlatform=null;
        }
    }


}
