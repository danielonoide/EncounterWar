using Godot;
using System;

public class GloboTeledirigido : GloboConAgua
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    float speed=400;
    bool exploded=false;

    public override void _PhysicsProcess(float delta)
    {
        if(!exploded) Movement(delta);
    }

    protected override void Movement(float delta)
    {
        velocity.x=Convert.ToInt32(Input.IsActionPressed("Right")) - Convert.ToInt32(Input.IsActionPressed("Left"));
		velocity.y=Convert.ToInt32(Input.IsActionPressed("Down")) - Convert.ToInt32(Input.IsActionPressed("Up"));
		
		velocity.x*=speed;
		velocity.y*=speed;


        velocity=MoveAndSlide(velocity, Vector2.Up);
    }

    protected new void _on_Collision_body_entered(Node body)
    {
        if(body is KinematicBody2D)
        {
            return;
        }
        base._on_Collision_body_entered(body);
        exploded=true;
    }

}
