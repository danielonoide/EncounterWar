using Godot;
using System;
using System.Collections.Generic;

[Serializable]
public class PlatanoData
{
    public bool martianDropped;
    public Vector2 position;

    public PlatanoData(Platano banana)
    {
        martianDropped=banana.martianDropped;
        position=banana.Position;
    }
     
}
public class Platano : Throwable
{
    RectangleShape2D rectangleShape2D;
    bool dropped=false;
    AudioStreamPlayer restartSound;
    AudioStreamPlayer soundEffect;
    bool flag=true;
    public bool martianDropped=false;
    public override float MaxSize {get; }

    Area2D detectPlayers;


    List<Node> collidingBodies=new();

    public bool loaded=false;

    public override void _Ready()
    {
        restartSound=GetNode<AudioStreamPlayer>("LaunchRestartSound");
        soundEffect=GetNode<AudioStreamPlayer>("SoundEffect");
        detectPlayers=GetNode<Area2D>("DetectPlayers");

        rectangleShape2D=new();
        rectangleShape2D.Extents=new Vector2(1, 70);
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
            detectPlayers.Monitoring=true;
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

        return collidingBodies.Count==0;

    }


    private void _on_IsColliding_body_entered(Node body)
    {
        collidingBodies.Add(body);
    }

    private void _on_IsColliding_body_exited(Node body)
    {
        collidingBodies.Remove(body);
    }


    private void _on_DetectPlayers_body_entered(Node body)
    {
        if(body is Jugador jugador )
        {
            if(jugador.IsMartian!=martianDropped)
            {
                jugador.HasToFall=true;
                Escenario.AddStar(jugador.IsMartian,true);
                GetNode<Sprite>("Sprite").Visible=false;
                soundEffect.Play();
            }
        }
        else
        {
            velocity=Vector2.Zero;
            if(!loaded) GetTree().CallGroup("Escenarios", "ChangeTurn");
        }
    }

    private void _on_SoundEffect_finished()
    {
        QueueFree();
    }


    private void _on_DetectPlayers_input_event(object viewport, object @event, int shape_idx)
    {
        if(@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex==(int)ButtonList.Left
         && !mouseButton.Pressed && !dropped)
        {
            if(CanDrop())
            {
                dropped=true;
                martianDropped=Escenario.MartianTurn;
                SetVelocity(new Vector2(0,-1));
            }
            else
            {
                restartSound.Play();
            }
        }
    }

    public static Platano GetPlatano()
	{
		PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/Herramientas/Platano.tscn");
		Platano platano=scene.Instance<Platano>();

/*         platano.martianDropped=banana.martianDropped;
        platano.Position=banana.position;         */
        platano.dropped=true;
        platano._Ready();
        platano.detectPlayers.Monitoring=true;
        

		return platano;
	}

}
