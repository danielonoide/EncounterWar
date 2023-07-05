using Godot;
using System;


public class MainMenu : CanvasLayer
{
	Vector2 Movimiento=new Vector2(1,0);
	
	Vector2 OpEscalaSec=new Vector2((float)0.5,(float)0.5);
	Vector2 OpEscalaIni=new Vector2((float)0.3,(float)0.3);	
	Vector2 OpPosicionSec=new Vector2(2340,1132);
	Vector2 OpPosicionIni=new Vector2(1222,609);
	
	Vector2 StEscalaSec=new Vector2((float)1.2,(float)1.2);
	Vector2 StEscalaIni=new Vector2(1,1);	
	Vector2 StPosicionSec=new Vector2(443,370);
	Vector2 StPosicionIni=new Vector2(480,376);
	
	Vector2 ExEscalaIni=new Vector2((float)0.8,(float)0.8);	
	Vector2 ExPosicionSec=new Vector2(480,489);
	Vector2 ExPosicionIni=new Vector2(516,500);
	
	
	//public ParallaxLayer Bg;
	Camera2D Camara;
	TextureButton Start, Opciones, Exit;
	AudioStreamPlayer Musica;
	
	
	public override void _Ready()
	{
		//Bg=GetNode("ParallaxBackground").GetNode<ParallaxLayer>("Bg");
		Camara=GetNode<Camera2D>("Camera2D");
		Start=GetNode<TextureButton>("Start");
		Exit=GetNode<TextureButton>("Exit");		
		Opciones=GetNode<TextureButton>("Opciones");
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
	/*if(GetNode<Sprite>("Bg").Position.y==514) 
	{
		Movimiento.x=-1;
		Movimiento.y=-1;
	}
	if(GetNode<Sprite>("Bg").Position.y==222) 
	{
		Movimiento.x=1;
		Movimiento.y=1;
	}
	*/
	
	//Bg.Position+=Movimiento;
	Camara.Position+=Movimiento;
  }

	private void _on_Opciones_mouse_entered()
	{
		Modify.ChangeScale(Opciones, OpEscalaSec);
		//Modify.ChangePosition(Opciones, OpPosicionSec);
	}

	private void _on_Opciones_mouse_exited()
	{
		Modify.ChangeScale(Opciones, OpEscalaIni);
		//Modify.ChangePosition(Opciones, OpPosicionIni);
	}
	
	private void _on_Opciones_pressed()
	{
		AddChild(Settings.GetSettings());
		Opciones.Hide();
	}
	
	private void CloseSettings()
	{
		Modify.ChangeScale(Opciones, OpEscalaIni);
		Opciones.Show();
	}
	
	private void _on_Start_mouse_entered()
	{
		Modify.ChangeScale(Start, StEscalaSec);
		Modify.ChangePosition(Start, StPosicionSec);
	}
	
	private void _on_Start_mouse_exited()
	{
		Modify.ChangeScale(Start, StEscalaIni);
		Modify.ChangePosition(Start, StPosicionIni);
	}
	
	private void _on_Exit_mouse_entered()
	{
		Modify.ChangeScale(Exit, StEscalaIni);
		Modify.ChangePosition(Exit, ExPosicionSec);
	}
	
	private void _on_Exit_mouse_exited()
	{
		Modify.ChangeScale(Exit, ExEscalaIni);
		Modify.ChangePosition(Exit, ExPosicionIni);
	}
	
	private void _on_Start_pressed()
	{
		GetTree().ChangeScene("res://scenes/Escenario3.tscn");
	}

	private void _on_Exit_pressed()
	{
		AddChild(AffirmationScreen.GetAffirmationScreen(3, "  Exit game?"));
	}


}













