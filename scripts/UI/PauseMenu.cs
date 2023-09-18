using Godot;
using System;


public class PauseMenu : Node2D
{
	float timer=(float)0.3;
    readonly Vector2 scaling=new(0.2f, 0.2f);

    public override void _Ready()
	{
		GetTree().Paused=true;
		
		
		//Asignar valores a los arreglos
		var buttons =GetTree().GetNodesInGroup("Botones");
		
		
		//Conectar los eventos
		foreach(TextureButton button in buttons)
		{
			button.Connect("mouse_entered", this, nameof(MouseEntrance), new Godot.Collections.Array{button});
			button.Connect("mouse_exited", this,  nameof(MouseExit), new Godot.Collections.Array{button});
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
		PackedScene MenuPausa=(PackedScene)ResourceLoader.Load("res://scenes/UI/PauseMenu.tscn");
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
		AddChild(AffirmationScreen.GetAffirmationScreen(AffirmationScreen.Actions.Restart, "¿Reiniciar partida?"));		
	}
	
	private void _on_Menu_pressed()
	{
		AddChild(AffirmationScreen.GetAffirmationScreen(AffirmationScreen.Actions.Menu, "¿Salir al menú de inicio?"));
	}

	
	private void _on_Opciones_pressed()
	{
		AddChild(Settings.GetSettings());
	}
		
	private void MouseEntrance(TextureButton button)
	{
		button.RectScale+=scaling;
	}
	
	private void MouseExit(TextureButton button)
	{
		button.RectScale-=scaling;
	}
	

}










