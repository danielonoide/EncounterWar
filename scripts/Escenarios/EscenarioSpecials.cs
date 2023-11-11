using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


public partial class Escenario : Node2D
{
    private void _on_AstronautSpecial_pressed()
	{
		if(martianTurn) return;

		if(Inventory.Unopenable) return; //Si tiene una herramienta fuera

		//si el jugador se movió
		if(Inventory.SelectedPlayer!=null)
		{
			Inventory.SelectedPlayer.Moved=false;
			if(!Inventory.SelectedPlayer.IsOnFloor()) return;
		}

		astronautsSpecialActive=true;
		
		astronautsSpecialTurnsLeft=3;
		astronautsSpecial.Visible=false;
		AddAstronautsSpecial();

	}

	private void AddAstronautsSpecial()
	{
		AstronautsSpecial astronautShip=AstronautsSpecial.GetAstronautsSpecial();
		astronautShip.Position=new Vector2(GetGlobalMousePosition().x, topLimit);
		AddChild(astronautShip);
	}

	private void _on_MartianSpecial_pressed()
	{
		if(!martianTurn) return;

		if(Inventory.Unopenable) return; //Si tiene una herramienta fuera

		//si el jugador se movió
		if(Inventory.SelectedPlayer!=null)
		{
			Inventory.SelectedPlayer.Moved=false;
		}

		martiansSpecialTurnsLeft=3;
		martiansSpecial.Visible=false;
		MartianTurnInvisible();
		ChangeTurn();
		martiansInvisible=true;

	}

	private void MartianTurnInvisible()
	{
		foreach(Jugador martian in martians)
		{
			if(IsInstanceValid(martian))
			{
				martian.Visible=false;
			}
		}
	}

	private void MartianTurnVisible()
	{
		foreach(Jugador martian in martians)
		{
			if(IsInstanceValid(martian))
			{
				martian.Visible=true;
			}
		}
	}

}