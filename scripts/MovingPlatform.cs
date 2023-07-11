using Godot;
using System;

public class MovingPlatform : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    float speed=100f;
    float start;
    float limit=323f;
    sbyte direction=1;
    Vector2 velocity=new Vector2(0,0);

    public override void _Ready()
    {
        start=Position.x;
    }

    public override void _PhysicsProcess(float delta)
    {
        velocity.x=direction*speed;
        MoveAndSlide(velocity);
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
