using Godot;
using System;

public class AffirmationScreen : CanvasLayer
{
	TextureButton[] Arr;
	static int Action;
	static string Text;

	public override void _Ready()
	{
		GetNode<Label>("CenterContainer/Label").Text=Text;
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
	
	public static CanvasLayer GetAffirmationScreen(int Accion, string Texto)
	{
		PackedScene affirmationScreen=(PackedScene)ResourceLoader.Load("res://scenes/AffirmationScreen.tscn");
		Action=Accion;
		Text=Texto;
		return (CanvasLayer)affirmationScreen.Instance();
	}


	private void _on_AcceptBTN_pressed()
	{
		switch(Action)
		{
			case 1: //"Reiniciar"
				GetTree().Paused=false;
				GetTree().ReloadCurrentScene();
				break;
			case 2: //Salir al menu
				GetTree().Paused=false;
				GetTree().ChangeScene("res://scenes/MainMenu.tscn");
				break;
			case 3: //Salir del juego
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




