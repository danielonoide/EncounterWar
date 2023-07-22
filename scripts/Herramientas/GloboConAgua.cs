using Godot;
using System;

public class GloboConAgua : Throwable
{

    Timer timer;

    Particles2D particles;
    Area2D explosion;

    Sprite sprite;

    AudioStreamPlayer2D soundEffect;

    public override float MaxSize { get => 49; }

    protected float baseSpeed=25000f; //20000

    public override void _Ready()
    {
        timer=GetNode<Timer>("Timer");
        particles=GetNode<Particles2D>("Particles2D");
        explosion=GetNode<Area2D>("Explosion");

        sprite=GetNode<Sprite>("Sprite");
        soundEffect=GetNode<AudioStreamPlayer2D>("SoundEffect");


        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    public override void _PhysicsProcess(float delta)
    {
        if(velocity!=new Vector2(0,0)) base._PhysicsProcess(delta);
    }

    protected void _on_Collision_body_entered(Node body)
    {

        if(body is KinematicBody2D)
        {
            return;
        }
        
        GetTree().CallGroup("Escenarios", "ChangeTurn");
        Explode();
    }

    protected void Explode()
    {
        particles.Emitting=true;
        explosion.Monitoring=true;
        velocity=new Vector2(0,0);
        sprite.Visible=false;
        soundEffect.Play();
        timer.Start();
    }

    private void _on_Explosion_body_entered(Node body)
    {
        //GD.Print(body);
        if(body is Throwable throwable)
        {
            float distance=throwable.GlobalPosition.DistanceTo(GlobalPosition);
            Vector2 direction=(throwable.GlobalPosition-GlobalPosition).Normalized();

            //float speed=baseSpeed*(1f/distance);
            float speed=(float)baseSpeed/distance;

            //float maxSpeed = 10000000f;
            //speed = Mathf.Clamp(speed, 0, maxSpeed);

            Vector2 force=direction*speed;
            //GD.Print(force);
            throwable.SetVelocity(force);
        }

    }

    private void _on_Timer_timeout()
    {
        QueueFree();
    }
    

}
