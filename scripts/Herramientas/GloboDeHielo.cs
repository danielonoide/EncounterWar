using Godot;
using System;
using System.Collections.Generic;

public class GloboDeHielo : GloboConAgua
{


    private List<Jugador> frozenPlayers=new();
    public override void _Ready()
    {
        base._Ready();
        EventManager.OnTurnChanged+=OnTurnChanged;
        
    }

    protected new void _on_Explosion_body_entered(Node body)
    {
        if(body is Jugador jugador)
        {
            jugador.Frozen=true;
            frozenPlayers.Add(jugador);
        }
    }

    private void OnTurnChanged(bool isMartianTurn) //there is a delay of 1 turn :) useful
    {
        List<Jugador> playersToRemove=new();

        foreach(var player in frozenPlayers)
        {
            if(player.IsMartian!=isMartianTurn)
            {
                player.Frozen=false;
                playersToRemove.Add(player);
            }
        }
        foreach (var player in playersToRemove)
        {
            frozenPlayers.Remove(player);            
        }

        //frozenPlayers.Clear();

/*         GD.Print(frozenPlayers.Count);
        List<Jugador> playersToRemove=new();
        List<Jugador> subtractTurnsLeft=new();

        foreach(var keyValuePair in frozenPlayers)
        {
            if(keyValuePair.Value==0) 
            {
                //keyValuePair.Key.Moved=false;
                playersToRemove.Add(keyValuePair.Key);
            }
            else
            {
                //frozenPlayers[keyValuePair.Key]-=1;
                subtractTurnsLeft.Add(keyValuePair.Key);
            }
            GD.Print(keyValuePair.Value);

        }
        

        foreach(var player in playersToRemove)
        {
            player.Moved=false;
            frozenPlayers.Remove(player);
        }

        foreach(var player in subtractTurnsLeft)
        {
            frozenPlayers[player]-=1;
        } */

    }

}
