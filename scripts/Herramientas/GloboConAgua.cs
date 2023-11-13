using Godot;
using System;

public class GloboConAgua : Throwable
{
    public bool LanzaglobosTerminado{get; set;}=false;
    Timer timer;

    Particles2D particles;
    Area2D explosion;

    Sprite sprite;

    AudioStreamPlayer2D soundEffect;

    public override float MaxSize { get => 49; }

    protected float baseSpeed=25000f; //20000
    protected float explosionRadius=95;

    General signalManager;

    bool exploded=false;

    public override void _Ready()
    {
        signalManager=GetNode<General>("/root/General");
        timer=GetNode<Timer>("Timer");
        particles=GetNode<Particles2D>("Particles2D");
        explosion=GetNode<Area2D>("Explosion");

        sprite=GetNode<Sprite>("Sprite");
        soundEffect=GetNode<AudioStreamPlayer2D>("SoundEffect");
        
        SetExplosionRadius();
    }

    protected void SetExplosionRadius()
    {
        //radio de la explosión
        CircleShape2D explosionShape=GetNode<CollisionShape2D>("Explosion/BalloonCollision").Shape as CircleShape2D;
        explosionShape.Radius=explosionRadius;

        //velocidad inicial de la explosión
        ParticlesMaterial particlesMaterial=GetNode<Particles2D>("Particles2D").ProcessMaterial as ParticlesMaterial;
        particlesMaterial.InitialVelocity=explosionRadius+20;
    }

    public override void _PhysicsProcess(float delta)
    {
        if(velocity!=new Vector2(0,0)) base._PhysicsProcess(delta);
    }

    protected void _on_Collision_body_entered(Node body)
    {

        if(body is Jugador)
        {
            return;   
        }

        signalManager.EmitSignal(nameof(General.OnBalloonExploded), this);
        Explode();
    }

    protected void Explode()
    {
        exploded=true;
        particles.Emitting=true;
        explosion.Monitoring=true;
        velocity=new Vector2(0,0);
        sprite.Visible=false;
        soundEffect.Play();
        timer.Start();
    }

    protected void _on_Explosion_body_entered(Node body)
    {
        //GD.Print(body);
        if(body is Jugador jugador)
        {
            float distance=jugador.GlobalPosition.DistanceTo(GlobalPosition);
            Push(jugador, distance);
            jugador.AddHumidity(GetHumidityPoints(distance));
            GD.Print($"Distancia: {distance}");

            if(!GetTree().HasGroup("Lanzaglobos") && !LanzaglobosTerminado)
            {
                AddStars(jugador.IsMartian, distance);
            }
            else
            {
                GetTree().CallGroup("Escenarios", "AddStar", jugador.IsMartian, LanzaglobosTerminado);
            }
        }

    }

    protected byte GetHumidityPoints(float distance)
    {
        byte humidityPoints=0;
        if(distance<75)
        {
            humidityPoints=3;
        }
        if(distance>75 && distance<90)
        {
            humidityPoints=2;
        }
        if(distance>90)
        {
            humidityPoints=1;
        }
        GD.Print($"Puntos de humedad: {humidityPoints}");


        return humidityPoints;

    }


    protected void Push(Jugador player, float distance)
    {
        Vector2 direction=(player.GlobalPosition-GlobalPosition).Normalized();
        float speed=(float)baseSpeed/distance;
        Vector2 force=direction*speed;
        player.SetVelocity(force);
    }

    protected virtual void AddStars(bool isMartian, float distance)
    {
        if(isMartian!=Escenario.MartianTurn)
        {
            return;
        }

        GD.Print(distance);
        byte starsToAdd=0;
        if(distance<75)
        {
            starsToAdd=3;
        }
        if(distance>75 && distance<90)
        {
            starsToAdd=2;
        }
        if(distance>90)
        {
            starsToAdd=1;
        }

        if(!Escenario.MartianTurn) //cambia de turno antes de que llegue aquí
        {
            Escenario.MartiansStars+=starsToAdd;
            GD.Print("se agregaron: "+starsToAdd+" estrellas a los marcianos");
            GetTree().CallGroup("Escenarios", "DisplayAddedStars", starsToAdd, true);

            return;
        }

        Escenario.AstronautsStars+=starsToAdd;
        GD.Print("se agregaron: "+starsToAdd+" estrellas a los astronautas");

        GetTree().CallGroup("Escenarios", "DisplayAddedStars", starsToAdd, false);

    }

    private void _on_Timer_timeout()
    {
        QueueFree();
    }

    public override Godot.Collections.Dictionary<string, object> Save()
    {
        if(exploded)
        {
            return null;
        }

        return base.Save();

    }


    public static GloboConAgua GetWaterBalloon()
	{
		PackedScene lanzador=(PackedScene)ResourceLoader.Load("res://scenes/Herramientas/GloboConAgua.tscn");
		GloboConAgua globoConAgua=lanzador.Instance<GloboConAgua>();

		return globoConAgua;
	}

    public static GloboConAgua GetSpecialWaterBalloon()
	{
		PackedScene lanzador=(PackedScene)ResourceLoader.Load("res://scenes/Herramientas/GloboConAgua.tscn");
		GloboConAgua globoConAgua=lanzador.Instance<GloboConAgua>();
        globoConAgua.baseSpeed=globoConAgua.baseSpeed*3;
        globoConAgua.explosionRadius=globoConAgua.explosionRadius*3;

		return globoConAgua;
	}
    

}
