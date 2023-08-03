using Godot;
using System;

public class MovingPlatform : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    float speed=150f;
    const float start=-444f;
    float limit=323f;
    public sbyte direction=1;
    Vector2 velocity=new Vector2(0,0);

/*     public override void _Ready()
    {
        start=Position.x;
    } */

    public override void _PhysicsProcess(float delta)
    {
        velocity.x=direction*speed;
        velocity.y=0;
        var collision = MoveAndCollide(velocity*delta);
        //movimiento constante
        if(collision is not null)
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
    } 


}
