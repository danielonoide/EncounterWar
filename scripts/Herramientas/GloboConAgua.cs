using Godot;
using System;

public class GloboConAgua : Throwable
{

    Timer timer;

    Particles2D particles;
    Area2D explosion;

    Sprite sprite;

    AudioStreamPlayer2D soundEffect;
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

    private void _on_Collision_body_entered(Node body)
    {
        if(body is KinematicBody2D)
        {
            return;
        }

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
    }

    private void _on_Timer_timeout()
    {
        QueueFree();
    }

}
