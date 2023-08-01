using Godot;
using System;

public class AffirmationScreen : CanvasLayer
{
	TextureButton[] Arr;
	Actions action=Actions.Quit;
	string text;
	public enum Actions
	{
		Restart,
		Menu,
		Quit,
		Null
	}

	public override void _Ready()
	{
		GetNode<Label>("CenterContainer/Label").Text=text;
		Arr=new TextureButton[2] {GetNode<TextureButton>("AcceptBTN"), GetNode<TextureButton>("DeclineBTN")};
		for(int i=0;i<2;i++)
		{
			Arr[i].Connect("mouse_entered",this ,nameof(MouseEntrance), new Godot.Collections.Array{i});
			Arr[i].Connect("mouse_exited",this ,nameof(MouseExit), new Godot.Collections.Array{i});			
		}
	}

	 public override void _Process(float delta)
	 {
	  
	 }
	
	public static AffirmationScreen GetAffirmationScreen(AffirmationScreen.Actions accion, string texto)
	{
		PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/UI/AffirmationScreen.tscn");
		AffirmationScreen affirmationScreen=scene.Instance<AffirmationScreen>();
		affirmationScreen.action=accion;
		affirmationScreen.text=texto;
		return affirmationScreen;
	}


	private void _on_AcceptBTN_pressed()
	{
		switch(action)
		{
			case Actions.Restart: //"Reiniciar"
				GetTree().Paused=false;
				GetTree().ReloadCurrentScene();
				break;
			case Actions.Menu: //Salir al menu
				GetTree().Paused=false;
				GetTree().ChangeScene(Constants.MainMenuPath);
				break;
			case Actions.Quit: //Salir del juego
				GetTree().Quit();
				break;
		}
		QueueFree();
	}
	
	private void _on_DeclineBTN_pressed()
	{
		QueueFree();
	}
	
	private void MouseEntrance(int Nodo)
	{
		Modify.ChangeScale(Arr[Nodo], new Vector2(1,1));		
	}
	
	private void MouseExit(int Nodo)
	{
		Modify.ChangeScale(Arr[Nodo], new Vector2((float)0.8, (float)0.8));		
	}
	
}




