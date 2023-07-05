using Godot;
using System;


public class Escenario : Node2D
{
	protected Camera2D Camara;
	protected Label Etiqueta;
	protected Vector2 Zoom=new Vector2((float)0.2,(float)0.2);
	protected bool Pressed=false;
	protected float Timer=4;
	
	public override void _Ready()
	{
		//PauseButton.GetPauseButton().Connect("BotonPausaPresionado", this, nameof(BotonPausaPresionado)); //nombre de la señal, objetivo y funcion a ejecutar
		Camara=GetNode<Camera2D>("Camera2D");
		Etiqueta=GetNode("HUD").GetNode<Label>("Label");
	}


	
	public override void _Process(float delta)
	{
		Etiqueta.Text=(200-(int)(Camara.Zoom.x*100)).ToString()+"%";
		
		if(Input.IsActionPressed("Right") && 0>Timer)
		{
			Camara.Position=new Vector2(300,0);
			Timer=4;
		}
		if(Input.IsActionPressed("Left") && 0>Timer)
		{
			Camara.Position=new Vector2(-300,0);
			Timer=4;
		}
		if(0<Timer) Timer-=delta;
	}
	
	public override void _PhysicsProcess(float delta)
	{

	}
	

	
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventMouseButton evento)
		{
			if (evento.Pressed && evento.ButtonIndex==Constants.ButtonWheelDown && Camara.Zoom<new Vector2((float)1.8,(float)1.8)) 
			{
				Camara.Zoom+=Zoom; 
			}
			if (evento.Pressed && evento.ButtonIndex==Constants.ButtonWheelUp && Camara.Zoom>new Vector2((float)0.4,(float)0.4))
			{
				Camara.Zoom-=Zoom;				
			}	
			
			if(evento.ButtonIndex==2)
			{
				if(evento.Pressed)
				{
					Pressed=true;
				}
				else
				{
					Pressed=false;
				}
			}
		}
		
		if(@event is InputEventMouseMotion Movimiento)
		{
			if(Pressed)
			{
				Camara.SmoothingEnabled=false;
				Camara.Position-=Movimiento.Relative;
			}
			else Camara.SmoothingEnabled=true;
		}
		

	}
	
	private void _on_Zoom_pressed()
	{
		if(Camara.Zoom>new Vector2((float)0.4,(float)0.4))
		{
			Camara.Zoom-=Zoom;
		}
	}


	private void _on_UnZoom_pressed()
	{
		if(Camara.Zoom<new Vector2((float)1.8,(float)1.8))
		{
			Camara.Zoom+=Zoom;
		}
	}

	
	
	public void Reanudar()
	{
		//AddChild(PauseButton.GetPauseButton());
		GetNode<CanvasLayer>("HUD").Show();
	}
	

}








