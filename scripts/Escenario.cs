using Godot;
using System;


public class Escenario : Node2D
{
	protected Camera2D Camara;
	protected Label zoomPercentage;
	protected float zoom=0.1f; //los saltos
	protected bool Pressed=false;
	protected float Timer=4;

	protected float maxZoom=0.5f;
	protected float minZoom=1.9f;

	protected float realMinZoom=1.95f;
	protected Vector2 cameraSize=new Vector2(1366, 768);
	protected float leftLimit=-2500f;  //a los límites de la cámara le restamos la mitad de su ancho
	protected float rightLimit=2500f;
	protected float topLimit=-1400f;
	protected float bottomLimit=1000f;
	AudioStreamPlayer music;

	bool martianTurn; 

	public override void _Ready()
	{
		//PauseButton.GetPauseButton().Connect("BotonPausaPresionado", this, nameof(BotonPausaPresionado)); //nombre de la señal, objetivo y funcion a ejecutar
		Camara=GetNode<Camera2D>("Camera2D");
		zoomPercentage=GetNode<Label>("HUD/Zoom/Label");
		music=GetNode<AudioStreamPlayer>("Music");
		var random=new Random();
		int currentTurn=random.Next(0,2);
		martianTurn=Convert.ToBoolean(currentTurn);
	}

	
	public override void _Process(float delta)
	{
		zoomPercentage.Text=(200-(int)(Camara.Zoom.x*100)).ToString()+"%";
		
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

		if(!music.Playing)
		{
			music.Play();
		}	

	}
	
	public override void _PhysicsProcess(float delta)
	{

	}
	

	
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventMouseButton evento)
		{
			if (evento.Pressed && evento.ButtonIndex==Constants.ButtonWheelDown) 
			{
				UnZoom();
			}
			if (evento.Pressed && evento.ButtonIndex==Constants.ButtonWheelUp)
			{
				Zoom();		
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
			if (Pressed)
			{
				Camara.SmoothingEnabled = false;
				Vector2 newPosition = Camara.Position - Movimiento.Relative * Camara.Zoom;
				float leftLimitZoom=leftLimit+( (cameraSize.x*Camara.Zoom.x)/2 );
				float rightLimitZoom=rightLimit-( (cameraSize.x*Camara.Zoom.x)/2 );
				float topLimitZoom=topLimit+( (cameraSize.y*Camara.Zoom.y)/2 );
				float bottomLimitZoom=bottomLimit-( (cameraSize.y*Camara.Zoom.y)/2 );

				// Verificar límites de la cámara
				newPosition.x = Mathf.Clamp(newPosition.x, leftLimitZoom, rightLimitZoom);
				newPosition.y = Mathf.Clamp(newPosition.y, topLimitZoom, bottomLimitZoom);

				Camara.Position=newPosition;
			}
			else
			{
				Camara.SmoothingEnabled = true;
			}
		}
		
	}
	
	protected void _on_Zoom_pressed()
	{
		Zoom();
	}


	protected void _on_UnZoom_pressed()
	{
		UnZoom();
	}

	
	
	protected void Reanudar()
	{
		//AddChild(PauseButton.GetPauseButton());
		GetNode<CanvasLayer>("HUD").Show();
	}

	void Zoom()
	{
		if(Camara.Zoom.x>maxZoom)
		{
			float newZoom=(float)Math.Round(Camara.Zoom.x-zoom,1);
			Camara.Zoom=new Vector2(newZoom, newZoom); 	
		}
	}

	void UnZoom()
	{
		if(Camara.Zoom.x<minZoom)
		{
			float newZoom=(float)Math.Round(Camara.Zoom.x+zoom,1);
			Camara.Zoom=new Vector2(newZoom, newZoom); 	
			return;
		}

		if(Camara.Zoom.x==minZoom)
		{
			Camara.Zoom=new Vector2(realMinZoom, realMinZoom); 	
		}
	}
	

}








