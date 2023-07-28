using Godot;
using System;
using System.Collections.Generic;

public class GloboConTinta : GloboConAgua
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private List<Jugador> inkedPlayers=new();
    bool flag=true;

    public override void _Ready()
    {
        base._Ready();
        EventManager.OnTurnChanged+=OnTurnChanged;
        
    }

    protected new void _on_Explosion_body_entered(Node body)
    {
        base._on_Explosion_body_entered(body);
        if(body is Jugador jugador)
        {
            jugador.Inked=true;
            inkedPlayers.Add(jugador);
        }

    }

    private void OnTurnChanged(bool isMartianTurn)
    {
        if(flag)
        {
            flag=false;
            return;
        }
        List<Jugador> playersToRemove=new();

        foreach(var player in inkedPlayers)
        {
            if(player.IsMartian!=isMartianTurn)
            {
                player.Inked=false;
                playersToRemove.Add(player);
            }
        }

        foreach (var player in playersToRemove)
        {
            inkedPlayers.Remove(player);            
        }

        if(inkedPlayers.Count==0)
        {
            EventManager.OnTurnChanged-=OnTurnChanged; //desuscribir si ya no hay entintados
        }

    }
}
