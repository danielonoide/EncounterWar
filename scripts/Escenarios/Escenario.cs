using Godot;
using System;
using System.Collections.Generic;


public class Escenario : Node2D
{
	protected Camera2D camera;
	protected Label zoomPercentage;
	protected float zoom=0.1f; //los saltos
	protected bool rightClick=false;
	protected const float maxZoom=0.5f;
	protected const float minZoom=1.9f;

	protected const float realMinZoom=1.95f;
	protected Vector2 cameraSize=new Vector2(1366, 768);
	protected float leftLimit=-2500f;  //a los límites de la cámara le restamos la mitad de su ancho
	protected float rightLimit=2500f;
	protected float topLimit=-1400f;
	protected float bottomLimit=1000f;
	AudioStreamPlayer music;

	static bool martianTurn; 
	public static bool MartianTurn{get=>martianTurn;}

	public static int AstronautsStars { get; set; }
	public static int MartiansStars { get; set; }
	
	byte astronautsSpecialTurnsLeft=3;
	byte martiansSpecialTurnsLeft=3;

	TextureButton astronautsSpecial;
	TextureButton martiansSpecial;

	bool martiansInvisible=false;

	Timer messageTimer;
	Timer gameOverTimer;
	Label messageLabel;

	Texture astronautCursor=GD.Load<Texture>("res://sprites/cursors/spaceship3.png");
	Texture martianCursor=GD.Load<Texture>("res://sprites/cursors/alien_cursor4.png");

	Texture astronautTexture=GD.Load<Texture>("res://sprites/characters/astronaut_idle_single.png");
	Texture martianTexture=GD.Load<Texture>("res://sprites/characters/martian_idle.png");


	AudioStreamPlayer gameOverSound;
	AudioStreamPlayer turnChangeSound;

	AudioStreamPlayer deathSound;

	protected Vector2 astronautsCameraPosition=new Vector2(0,0);
	protected Vector2 martiansCameraPosition=new Vector2(0,0);


	Godot.Collections.Array martians, astronauts;

	Label martiansLabel, astronautsLabel;

	TextureRect ink;

	static bool playerDeathEventSuscribed=false;


	General signalManager;

	
	public override void _Ready()
	{
		//signals
		signalManager=GetNode<General>("/root/General");
		signalManager.Connect(nameof(General.OnPlayerDeath), this, nameof(OnPlayerDeath));


		//PauseButton.GetPauseButton().Connect("BotonPausaPresionado", this, nameof(BotonPausaPresionado)); //nombre de la señal, objetivo y funcion a ejecutar
		camera=GetNode<Camera2D>("Camera2D");
		zoomPercentage=GetNode<Label>("HUD/Zoom/Label");
		music=GetNode<AudioStreamPlayer>("Music");
		messageTimer=GetNode<Timer>("HUD/Messaging/Timer");
		messageLabel=GetNode<Label>("HUD/Messaging/CenterContainer/Message");

		//specials
		astronautsSpecial=GetNode<TextureButton>("HUD/TeamInfo/AstronautSpecial");
		martiansSpecial=GetNode<TextureButton>("HUD/TeamInfo/MartianSpecial");

		//game over
		gameOverTimer=GetNode<Timer>("GameOverTimer");

		//ink
		ink=GetNode<TextureRect>("HUD/Ink");


		//audio
		//matchSFX["GameStart"]=GetNode<AudioStreamPlayer>("MatchSFX/GameStart");
		gameOverSound=GetNode<AudioStreamPlayer>("MatchSFX/GameOver");
		turnChangeSound=GetNode<AudioStreamPlayer>("MatchSFX/TurnChange");
		deathSound=GetNode<AudioStreamPlayer>("DeathSound");

		//choose turn
		var random=new Random();
		int currentTurn=random.Next(0,2);
		martianTurn=Convert.ToBoolean(currentTurn);
		if(martianTurn)
		{
			ShowMessage("¡Empiezan los marcianos!");
			Input.SetCustomMouseCursor(martianCursor, Input.CursorShape.Arrow, new Vector2(0,0));
			camera.Position=martiansCameraPosition;
		}
		else
		{
			ShowMessage("¡Empiezan los astronautas!");
			Input.SetCustomMouseCursor(astronautCursor, Input.CursorShape.Arrow, new Vector2(3,0));
			camera.Position=astronautsCameraPosition;
		}

		//group astronauts and martians
		astronauts=GetNode("Astronauts").GetChildren();
		foreach(Jugador astronaut in astronauts)
		{
			astronaut.AddToGroup("Astronauts");
			astronaut.ToolsAvailable=new byte[9];
			Array.Copy(InventorySelection.AstronautsTools, astronaut.ToolsAvailable, 9);

			//astronaut.AddChild(Inventory.GetInventory(InventorySelection.astronautsTools));
		}

		martians=GetNode("Martians").GetChildren();
		foreach(Jugador martian in martians)
		{
			martian.AddToGroup("Martians");
			martian.IsMartian=true;
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
		AstronautsStars=0;
		MartiansStars=0;

		//team counters
		astronautsLabel=GetNode<Label>("HUD/TeamInfo/AstronautsCounter");
		martiansLabel=GetNode<Label>("HUD/TeamInfo/MartiansCounter");

		astronautsLabel.Text=astronauts.Count.ToString();
		martiansLabel.Text=martians.Count.ToString();


		//death zone
		Area2D deathZone = GetNode<Area2D>("DeathZone");

		SegmentShape2D[] segments = new SegmentShape2D[3];

		segments[0] = new SegmentShape2D()
		{
			A = new Vector2(leftLimit, topLimit),
			B = new Vector2(leftLimit, bottomLimit)
		};

/* 		segments[1] = new SegmentShape2D()
		{
			A = new Vector2(leftLimit, topLimit),
			B = new Vector2(rightLimit, topLimit)
		}; */

		segments[1] = new SegmentShape2D()
		{
			A = new Vector2(rightLimit, topLimit),
			B = new Vector2(rightLimit, bottomLimit)
		};

		segments[2] = new SegmentShape2D()
		{
			A = new Vector2(rightLimit, bottomLimit),
			B = new Vector2(leftLimit, bottomLimit)
		};

		for (int i = 0; i < segments.Length; i++)
		{
			deathZone.GetChild<CollisionShape2D>(i).Shape = segments[i];
		}

		//reiniciar inventario
		Inventory.SelectedPlayer=null;
		Inventory.Unopenable=false;

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
			if (evento.Pressed && evento.ButtonIndex==(int)ButtonList.WheelDown) 
			{
				UnZoom();
			}
			if (evento.Pressed && evento.ButtonIndex==(int)ButtonList.WheelUp)
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

	public void ChangeInkVisibility(bool visible)
	{
		ink.Visible=visible;
	}

	protected void ChangeTurn()
	{
		AstronautsStars++;
		MartiansStars++;

		ink.Visible=false;

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
		EventManager.NotifyTurnChanged(martianTurn);
		signalManager.EmitSignal(nameof(General.OnTurnChanged), martianTurn);
		turnChangeSound.Play();
		
		
		//Specials

		if(martiansInvisible)
		{
			MartianTurnVisible();
			martiansInvisible=false;
		}

		astronautsSpecialTurnsLeft = (byte)Math.Max(astronautsSpecialTurnsLeft - 1, 0);
		astronautsSpecial.Visible = astronautsSpecialTurnsLeft <= 0;

		martiansSpecialTurnsLeft = (byte)Math.Max(martiansSpecialTurnsLeft - 1, 0);
		martiansSpecial.Visible = martiansSpecialTurnsLeft <= 0;
		
	}


	private void _on_AstronautSpecial_pressed()
	{
		if(martianTurn) return;
		astronautsSpecialTurnsLeft=3;
		astronautsSpecial.Visible=false;
		AstronautsSpecial astronautShip=AstronautsSpecial.GetAstronautsSpecial();
		astronautShip.Position=new Vector2(GetGlobalMousePosition().x, topLimit);
		AddChild(astronautShip);


		//si el jugador se movió
		if(Inventory.SelectedPlayer!=null)
		{
			Inventory.SelectedPlayer.Moved=false;
		}
	}

	private void _on_MartianSpecial_pressed()
	{
		if(!martianTurn) return;

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


	public static void AddStar(bool isMartian, bool changedTurn)
    {
        if(!changedTurn) //significa que la herramienta no cambió el turno al ser lanzada
        {
            if(isMartian==MartianTurn)
            {
                return;
            }
            if(MartianTurn)
            {
                MartiansStars++;
                GD.Print("se agregó una estrella a los marcianos");
                return;
            }

            AstronautsStars++;
            GD.Print("se agregó una estrella a los astronautas");

            return;
        }



        if(isMartian!=Escenario.MartianTurn)
        {
            return;
        }

        if(!MartianTurn)
        {
            MartiansStars++;
            GD.Print("se agregó una estrella a los marcianos");
            return;
        }

        AstronautsStars++;
        GD.Print("se agregó una estrella a los astronautas");
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

	private void _on_DeathZone_body_entered(Node body)
	{
		//GD.Print("body death");

		if(body is Jugador jugador)
		{
			signalManager.EmitSignal(nameof(General.OnPlayerDeath),jugador);
		}

		if(body is Teleporter teleporter)
		{
			body.QueueFree();
			signalManager.EmitSignal(nameof(General.OnTeleporterRemoved), teleporter);
			//EventManager.NotifyTeleporterRemoved(teleporter);
		}

		if(body is Iman iman)
		{
			//EventManager.OnTurnChanged-=iman.OnTurnChanged;
			iman.QueueFree();
		}

	}

	private void _on_DeathZone_area_entered(Node area)
	{
		if(area.Name.Equals("BananaIsColliding")) return; //para que no cambie 2 turnos el platano

		Throwable throwable = area.GetParent() as Throwable;
		if (throwable == null) return;

		throwable.QueueFree();

		if (GetTree().HasGroup("Lanzaglobos"))
		{
			Lanzaglobos lanzaglobos = GetTree().GetNodesInGroup("Lanzaglobos")[0] as Lanzaglobos;
			lanzaglobos.BalloonsExploded++;
			if (lanzaglobos != null && lanzaglobos.BalloonsExploded == 3)
			{
				ChangeTurn();
				lanzaglobos.QueueFree();
			}
			return;

		}

		ChangeTurn();

	}

	private void OnPlayerDeath(Jugador jugador)
	{
		SubtractTeamNumber(1, jugador.IsMartian);
		if(Inventory.SelectedPlayer==jugador)
		{
			ChangeTurn();
		}

		deathSound.Play();
		jugador.QueueFree();


/* 		if(!jugador.IsQueuedForDeletion())
		{
			jugador.QueueFree();
		} */

	}

	private void SubtractTeamNumber(byte subtrahend, bool martian)
	{
		Label label=martian ? martiansLabel : astronautsLabel;
		int num=Convert.ToInt32(label.Text);
		num=num-subtrahend;
		label.Text=num.ToString();	

		if(num<=0 && gameOverTimer.TimeLeft<gameOverTimer.WaitTime)
		{
			gameOverTimer.Start();
		}

	}

	private void _on_GameOverTimer_timeout()
	{
		if(astronautsLabel.Text.Equals("0") && martiansLabel.Text.Equals("0"))
		{
			GameOver(Constants.WinningTeam.Draw);
			return;
		}

		GameOver(astronautsLabel.Text=="0" ? Constants.WinningTeam.Martians : 
		Constants.WinningTeam.Astronauts);
	}

    private void GameOver(Constants.WinningTeam winningTeam) 
    {
		gameOverSound.Play();
		GetNode<CanvasLayer>("HUD").Visible=false;
		MatchEnding matchEnding=MatchEnding.GetMatchEnding(winningTeam);
		AddChild(matchEnding);
    }
}








