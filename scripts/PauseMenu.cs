using Godot;
using System;


public class PauseMenu : Node2D
{
	float timer=(float)0.3;
	
	AudioStreamPlayer Musica;
	Godot.Collections.Array Arr;
	TextureButton[] Arr2=new TextureButton[4];
	
	public override void _Ready()
	{
		GetTree().Paused=true;
		
		
		//Asignar valores a los arreglos
		
		Arr=new Godot.Collections.Array();
		Arr=GetTree().GetNodesInGroup("Botones");
		
		//Asignar al arreglo de TextureButton el arreglo Godot.Collections.Array
		for(int i=0;i<Arr2.Length;i++)
		{
			Arr2[i]=(TextureButton)Arr[i];
		}
		
		//Conectar los eventos
		for(int i=0;i<Arr2.Length;i++)
		{
			Arr2[i].Connect("mouse_entered", this, nameof(MouseEntrance), new Godot.Collections.Array{i});
			Arr2[i].Connect("mouse_exited", this,  nameof(MouseExit), new Godot.Collections.Array{i});
		}
		
		Arr2[3].Connect("pressed", this, nameof(SettingsPressed), new Godot.Collections.Array{Arr[3]});
		
		//Obtener Nodo de Musica
		Musica=GetNode<AudioStreamPlayer>("Music");
	}
	
	public override void _Process(float delta)
	{
		if(!Musica.Playing)
		{
			Musica.Play();
		}	
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(Input.IsActionPressed("Pause") && 0>timer && GetTree().Paused)
		{
			PauseButton.SetTimer();
			_on_Reanudar_pressed();
		}
		if(0<timer) timer-=delta;
	}
	
	public static Node2D GetPauseMenu()
	{
		PackedScene MenuPausa=(PackedScene)ResourceLoader.Load("res://scenes/PauseMenu.tscn");
		return (Node2D)MenuPausa.Instance();
	}

	private void _on_Reanudar_pressed()
	{		
		QueueFree();
		GetTree().CallGroup("Escenarios", "Reanudar");
		GetTree().Paused=false;
	}
	
	private void _on_Reiniciar_pressed()
	{
		AddChild(AffirmationScreen.GetAffirmationScreen(1, "¿Reiniciar partida?"));		
//		GetTree().Paused=false;
//		GetTree().ReloadCurrentScene();
	}
	
	private void _on_Menu_pressed()
	{
		AddChild(AffirmationScreen.GetAffirmationScreen(2, "¿Salir al menú de inicio?"));
//		GetTree().Paused=false;
//		GetTree().ChangeScene("res://scenes/MainMenu.tscn");
	}
	
	

	
	private static void SettingsPressed(Node Nodo)
	{
		Nodo.AddChild(Settings.GetSettings());
	}
		
	private void MouseEntrance(int Nodo)
	{
		Modify.ChangeScale(Arr2[Nodo], new Vector2((float)1.2, (float)1.2));
	}
	
	private void MouseExit(int Nodo)
	{
		Modify.ChangeScale(Arr2[Nodo], new Vector2(1, 1));
	}
	

}










