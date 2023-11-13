using Godot;
using System;
using System.Collections.Generic;

public class GloboDeHielo : GloboConAgua
{


    private List<Jugador> frozenPlayers=new();
    bool flag=true;
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
            float distance=jugador.GlobalPosition.DistanceTo(GlobalPosition);
            jugador.AddHumidity(GetHumidityPoints(distance));
            AddStars(jugador.IsMartian, distance);
        }
    }

    private void OnTurnChanged(bool isMartianTurn) //there is a delay of 1 turn
    {
        if(flag)
        {
            flag=false;
            return;
        }

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

        if(frozenPlayers.Count==0)
        {
            //desuscribir si es que ya no hay jugadores congelados y no es la primera ejecución
            EventManager.OnTurnChanged-=OnTurnChanged; 
        }

    }

}
