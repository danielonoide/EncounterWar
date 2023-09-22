using Godot;
using System;

public class Teleporter : Throwable
{
    public override float MaxSize {get=>60; }

    //bool turnChanged=false;
    bool flag=true;
    General signalManager;
    AudioStreamPlayer soundEffect;
    
    public bool launched=false;


    public override void _Ready()
    {
        base._Ready();
        soundEffect=GetNode<AudioStreamPlayer>("SoundEffect");
		signalManager=GetNode<General>("/root/General");
        signalManager.Connect(nameof(General.OnPlayerDeath),this, nameof(OnPlayerDeath));

    }
    

    public override void _PhysicsProcess(float delta)
    {
        if(velocity!=Vector2.Zero) 
        {
            base._PhysicsProcess(delta);
            launched=true;
            //GetNode<CollisionShape2D>("CollisionShape2D").Disabled=false;
            //SetCollisionMaskBit(2,true);
        }
        if(IsOnFloor() && flag)
        {
            flag=false;
            soundEffect.Play();
            //GetNode<CollisionShape2D>("CollisionShape2D2").Disabled=false;
            //GetNode<CollisionShape2D>("CollisionShape2D").Disabled=true;
            //SetCollisionMaskBit(1,true);

        } 
    }



    private void OnPlayerDeath(Jugador player)
    {
/*         if(IsInstanceValid(this))
        {
            return;
        } */

        //GD.Print("Evento activado");
        if(player.ActiveTeleporter==this)
        {
            QueueFree();
        }
    }

    public override Godot.Collections.Dictionary<string,object> Save()
    {
        var save=base.Save();
        save.Add("launched", launched);
        return save;
    }

}
