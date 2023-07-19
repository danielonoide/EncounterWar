using Godot;
using System;
using System.Collections.Generic;


public class Escenario : Node2D
{
	protected Camera2D camera;
	protected Label zoomPercentage;
	protected float zoom=0.1f; //los saltos
	protected bool rightClick=false;
	protected float maxZoom=0.5f;
	protected float minZoom=1.9f;

	protected float realMinZoom=1.95f;
	protected Vector2 cameraSize=new Vector2(1366, 768);
	protected float leftLimit=-2500f;  //a los límites de la cámara le restamos la mitad de su ancho
	protected float rightLimit=2500f;
	protected float topLimit=-1400f;
	protected float bottomLimit=1000f;
	AudioStreamPlayer music;

	static bool martianTurn; 
	public static bool MartianTurn
	{
		get
		{
			return martianTurn;
		}
	}

	static int astronautsStars=0;
	static int martiansStars=0;
	public static int AstronautsStars { get=>astronautsStars; set=>astronautsStars=value; }
	public static int MartiansStars { get=>martiansStars; set=>martiansStars=value; }

/* 	byte[] AstronautsInventory {get; set;}
	byte[] MartiansInventory {get; set;} */

	int turns=0;

	Timer messageTimer;
	Label messageLabel;

	Texture astronautCursor=GD.Load<Texture>("res://sprites/cursors/spaceship3.png");
	Texture martianCursor=GD.Load<Texture>("res://sprites/cursors/alien_cursor4.png");

	Texture astronautTexture=GD.Load<Texture>("res://sprites/characters/astronaut_idle_single.png");
	Texture martianTexture=GD.Load<Texture>("res://sprites/characters/martian_idle.png");

	Dictionary<string, AudioStreamPlayer> matchSFX;

	protected Vector2 astronautsCameraPosition=new Vector2(0,0);
	protected Vector2 martiansCameraPosition=new Vector2(0,0);

	
	public override void _Ready()
	{
		//PauseButton.GetPauseButton().Connect("BotonPausaPresionado", this, nameof(BotonPausaPresionado)); //nombre de la señal, objetivo y funcion a ejecutar
		camera=GetNode<Camera2D>("Camera2D");
		zoomPercentage=GetNode<Label>("HUD/Zoom/Label");
		music=GetNode<AudioStreamPlayer>("Music");
		messageTimer=GetNode<Timer>("HUD/Messaging/Timer");
		messageLabel=GetNode<Label>("HUD/Messaging/CenterContainer/Message");

		//audio
		matchSFX=new();
		//matchSFX["GameStart"]=GetNode<AudioStreamPlayer>("MatchSFX/GameStart");
		matchSFX["GameOver"]=GetNode<AudioStreamPlayer>("MatchSFX/GameOver");
		matchSFX["TurnChange"]=GetNode<AudioStreamPlayer>("MatchSFX/TurnChange");

		//choose turn
		var random=new Random();
		int currentTurn=random.Next(0,2);
		martianTurn=Convert.ToBoolean(currentTurn);
		if(martianTurn)
		{
			ShowMessage("¡Turno de los marcianos!");
			Input.SetCustomMouseCursor(martianCursor, Input.CursorShape.Arrow, new Vector2(0,0));
			camera.Position=martiansCameraPosition;
		}
		else
		{
			ShowMessage("¡Turno de los astronautas!");
			Input.SetCustomMouseCursor(astronautCursor, Input.CursorShape.Arrow, new Vector2(3,0));
			camera.Position=astronautsCameraPosition;
		}

		//group astronauts and martians
		Godot.Collections.Array astronauts=GetNode("Astronauts").GetChildren();
		foreach(Jugador astronaut in astronauts)
		{
			astronaut.AddToGroup("Astronauts");
			//aquí debo acceder a astronautsTools
			//astronaut.ToolsAvailable=InventorySelection.astronautsTools;
			astronaut.ToolsAvailable=new byte[9];
			Array.Copy(InventorySelection.AstronautsTools, astronaut.ToolsAvailable, 9);

			//astronaut.AddChild(Inventory.GetInventory(InventorySelection.astronautsTools));
		}

		Godot.Collections.Array martians=GetNode("Martians").GetChildren();
		foreach(Jugador martian in martians)
		{
			martian.AddToGroup("Martians");
			martian.isMartian=true;
			//change sprite
/* 			Sprite sprite=martian.GetNode<Sprite>("Sprite");
			sprite.Texture=martianTexture;
			sprite.Hframes=1;
			sprite.Vframes=1;
			sprite.Scale=new Vector2(0.369f, 0.366f); */

			//change animation
			AnimatedSprite animatedSprite=martian.GetNode<AnimatedSprite>("AnimatedSprite");
			animatedSprite.Animation="martian_idle";
			animatedSprite.Scale=new Vector2(0.369f, 0.366f);
			animatedSprite.Position=new Vector2(0, -3);

			//instanciar inventarios
			martian.ToolsAvailable=new byte[9];
			Array.Copy(InventorySelection.MartiansTools, martian.ToolsAvailable, 9);
			//martian.AddChild(Inventory.GetInventory(InventorySelection.martiansTools));

		}		

		//initialize stars
		astronautsStars=0;
		martiansStars=0;



/* 		GD.Print("astro");

		foreach(var i in InventorySelection.astronautsTools)
		{
			GD.Print(i);
		}
		GD.Print("marcianos");

		foreach(var i in InventorySelection.martiansTools)
		{
			GD.Print(i);
		} */

	}

	
	public override void _Process(float delta)
	{
		zoomPercentage.Text=(200-(int)(camera.Zoom.x*100)).ToString()+"%";

		//ShowFPS
		GetNode<Label>("HUD/FPS").Text=Engine.GetFramesPerSecond().ToString();
	}
	
	private void ShowMessage(string message)
	{
		messageTimer.Start();
		messageLabel.Text=message;
		messageLabel.Visible=true;
	}

	private void _on_Timer_timeout()
    {
		messageLabel.Visible=false;
	}

	private void _on_GameStartSound_finished()
	{
		music.Play();
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
					rightClick=true;
				}
				else
				{
					rightClick=false;
				}
			}
		}
		
		if(@event is InputEventMouseMotion Movimiento)
		{
			if (rightClick)
			{
				camera.SmoothingEnabled = false;
				Vector2 newPosition = camera.Position - Movimiento.Relative * camera.Zoom;
				float leftLimitZoom=leftLimit+( (cameraSize.x*camera.Zoom.x)/2 );
				float rightLimitZoom=rightLimit-( (cameraSize.x*camera.Zoom.x)/2 );
				float topLimitZoom=topLimit+( (cameraSize.y*camera.Zoom.y)/2 );
				float bottomLimitZoom=bottomLimit-( (cameraSize.y*camera.Zoom.y)/2 );

				// Verificar límites de la cámara
				newPosition.x = Mathf.Clamp(newPosition.x, leftLimitZoom, rightLimitZoom);
				newPosition.y = Mathf.Clamp(newPosition.y, topLimitZoom, bottomLimitZoom);

				camera.Position=newPosition;
			}
			else
			{
				camera.SmoothingEnabled = true;
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

	void Zoom()
	{
		if(camera.Zoom.x>maxZoom)
		{
			float newZoom=(float)Math.Round(camera.Zoom.x-zoom,1);
			camera.Zoom=new Vector2(newZoom, newZoom); 	
		}
	}

	void UnZoom()
	{
		if(camera.Zoom.x<minZoom)
		{
			float newZoom=(float)Math.Round(camera.Zoom.x+zoom,1);
			camera.Zoom=new Vector2(newZoom, newZoom); 	
			return;
		}

		if(camera.Zoom.x==minZoom)
		{
			camera.Zoom=new Vector2(realMinZoom, realMinZoom); 	
		}
	}

	private void Reanudar()
	{
		//AddChild(PauseButton.GetPauseButton());
		GetNode<CanvasLayer>("HUD").Show();
	}

	protected void ChangeTurn()
	{
		astronautsStars++;
		martiansStars++;

		if(!martianTurn)
		{
			ShowMessage("¡Turno de los marcianos!");
			Input.SetCustomMouseCursor(martianCursor, Input.CursorShape.Arrow, new Vector2(0,0));
		}
		else
		{
			ShowMessage("¡Turno de los astronautas!");
			Input.SetCustomMouseCursor(astronautCursor, Input.CursorShape.Arrow, new Vector2(3,0));
		}
		martianTurn=!martianTurn;
		matchSFX["TurnChange"].Play();
		turns++;
	}

/* 	public static Escenario GetScenery(byte scenery, byte[] _astronautsInventory, byte[] _martiansInventory)
	{
        PackedScene packedScene= (PackedScene)GD.Load("res://scenes/Escenario"+scenery+".tscn");
		Escenario escenarioInstance = (Escenario)packedScene.Instance();
		escenarioInstance.AstronautsInventory=_astronautsInventory;
		escenarioInstance.MartiansInventory=_martiansInventory;
		return escenarioInstance;
	} 
	 */

}








