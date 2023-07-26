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

    }
}
