using Godot;
using System;
using System.Collections.Generic;

/* [Serializable]
public class ImanData
{
    public bool martianLaunched;
    public Vector2 position;

    public ImanData(Iman magnet)
    {
        martianLaunched=magnet.martianLaunched;
        position=magnet.Position;
    }
} */
public class Iman : Throwable
{
    public override float MaxSize {get=>50;}
    public bool launched=false;
    bool flag=true;

    public bool martianLaunched=false;

    public int turns=10;

    List<Jugador> playersInMagnet;

    public Area2D playerDetector;

    AudioStreamPlayer2D landingSound;
    General signalManager;

    //public bool detectPlayers=false;

    
    public override void _Ready()
    {
        //EventManager.OnTurnChanged+=OnTurnChanged;
        signalManager=GetNode<General>("/root/General");
        signalManager.Connect(nameof(General.OnTurnChanged), this, nameof(OnTurnChanged));

        martianLaunched=Escenario.MartianTurn;
        landingSound=GetNode<AudioStreamPlayer2D>("LandingSound");
        playerDetector=GetNode<Area2D>("PlayerDetector");

        playersInMagnet=new();

    }

    public override void _PhysicsProcess(float delta)
    {
        if(velocity!=Vector2.Zero)
        {
             base._PhysicsProcess(delta);
             playerDetector.Monitoring=true;
             //detectPlayers=true;
             launched=true;

        }
/*         if(IsOnFloor() && flag)
        {
            flag=false;
            //playerDetector.Monitoring=true;
            landingSound.Play();
            //GetTree().CallGroup("Escenarios", "ChangeTurn");
        } */
    }


    public void OnTurnChanged(bool isMartianTurn)
    {
        if(playersInMagnet.Count==0)
        {
            return;
        }

        if(turns>0)
        {
            turns--;
            return;
        }

        foreach(var player in playersInMagnet)
        {
            player.ActiveMagnet=null;
        }

        playersInMagnet.Clear();


        //EventManager.OnTurnChanged -= OnTurnChanged; //que ya no se ejecute si el objeto se elimina
        QueueFree();
    }


    private void _on_PlayerDetector_body_entered(Node body)
    {
        if(body is Jugador jugador && jugador.IsMartian!=martianLaunched)
        {
            //jugador.Position=Position;
            jugador.ActiveMagnet=this;
            if(playersInMagnet.Count==0) turns=jugador.Moved ? 2 : 0;
            playersInMagnet.Add(jugador);
            GetTree().CallGroup("Escenarios", "AddStar", jugador.IsMartian, true);
        }
    }

    private void _on_GroundDetector_body_entered(Node body)
    {
        if(body is Jugador || body is MovingPlatform) return;
        landingSound.Play();
        velocity=Vector2.Zero;
    }

     public override Godot.Collections.Dictionary<string,object> Save()
    {
        var save=base.Save();
        save.Add("martianLaunched", martianLaunched);
        save.Add("turns", turns);
        save.Add("launched", launched);

        return save;
    }

     public static Iman GetIman()
	{
		PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/Herramientas/Iman.tscn");
		Iman iman=scene.Instance<Iman>();

        //iman._Ready();
        //iman.playerDetector.Monitoring=true;
/*         iman.martianLaunched=magnet.martianLaunched;
        iman.Position=magnet.position; */
        //iman.turns=magnet.turns;

		return iman;
	}


}
