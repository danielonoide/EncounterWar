using Godot;
using System;


public class Jugador : Throwable
{
	int speed=500;
	public bool IsMartian { get; set; } = false;

	public byte[] ToolsAvailable {get; set;}

	AnimatedSprite animatedSprite;

    public override float MaxSize { get => 30; }//95

	public bool Moved {get; set;} =false;

	public bool Frozen {get; set;} =false;

	public bool Inked {get; set;} =false;

	//public bool InMagnet {get; set;} =false;
	public bool HasToFall {get; set;} =false;

	public bool falling=false;

	int sideToFall;

	public byte HumidityPoints{get; set;}=0;

	TextureProgress humidityMeter;

	public Teleporter ActiveTeleporter { get; set; }=null;
	public Iman ActiveMagnet { get; set; }=null;

	AudioStreamPlayer teleportSound;

	//Texture ink=GD.Load<Texture>("res://sprites/tools/ink.png");


	public enum AnimationType
	{
		martian_idle,
		astronaut_idle,
		martian_jump,
		astronaut_jump,
		martian_frozen,
		astronaut_frozen
	}

	General signalManager;

	public override void _Ready()  //se ejecuta cuando carga el nodo
	{ 
		//EventManager.OnTeleporterRemoved+=OnTeleporterRemoved;
		//EventManager.OnPlayerDeath+=OnPlayerDeath;
		signalManager=GetNode<General>("/root/General");
		signalManager.Connect(nameof(General.OnTeleporterRemoved), this, nameof(OnTeleporterRemoved));
		signalManager.Connect(nameof(General.OnMagnetRemoved), this, nameof(OnMagnetRemoved));
		
		teleportSound=GetNode<AudioStreamPlayer>("TeleportSound");
		animatedSprite=GetNode<AnimatedSprite>("AnimatedSprite");
		humidityMeter=GetNode<TextureProgress>("TextureProgress");
	}
		
	private void UpdateAnimation()
	{
		if(Frozen)
		{
			animatedSprite.Animation= IsMartian ? AnimationType.martian_frozen.ToString() : AnimationType.astronaut_frozen.ToString();
			return;
		}

		if (IsOnFloor())
		{
			
			animatedSprite.Animation = IsMartian ? AnimationType.martian_idle.ToString() : AnimationType.astronaut_idle.ToString();	
		}
		else
		{
			animatedSprite.Animation = IsMartian ? AnimationType.martian_jump.ToString() : AnimationType.astronaut_jump.ToString();
		}
	}

	private void OnTeleporterRemoved(Teleporter teleporter)
	{
		if(teleporter==ActiveTeleporter)
		{
			ActiveTeleporter=null;
		}
	}

	private void OnMagnetRemoved(Iman magnet)
	{
		if(magnet==ActiveMagnet)
		{
			ActiveMagnet=null;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if(ActiveMagnet==null) 
		{
			base._PhysicsProcess(delta);
		}
		else
		{
			velocity=Vector2.Zero;
			Position=ActiveMagnet.Position;
		}
		UpdateAnimation();
		if(HasToFall && IsOnFloor())
		{
			falling=true;
			HasToFall=false;
			sideToFall=GetSideToFall();
		}

		if(falling)
		{
			//GD.Print(falling);
			velocity.x=sideToFall*100;
			if(!IsOnFloor())
			{
				falling=false;
			}
		}

	}

	private int GetSideToFall()
	{
		Vector2 leftSide=CollideSide(true);
		Vector2 rightSide=CollideSide(false);

		float leftDistance=Position.DistanceSquaredTo(leftSide);
		float rightDistance=Position.DistanceSquaredTo(rightSide);

		
		return leftDistance>rightDistance ? -1:1;
	}

	private Vector2 CollideSide(bool leftSide) 
	{
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
		int side=leftSide ? -1 : 1;
		Vector2 destination=Position+new Vector2(500*side, 0);
		Godot.Collections.Dictionary queryResult=
		spaceState.IntersectRay(Position, destination, new Godot.Collections.Array{this});
		
		return queryResult.Count>0 ? (Vector2)queryResult["position"] : destination;

	}


	public void AddHumidity(byte humidity)
	{
		HumidityPoints+=humidity;
		humidityMeter.Value=HumidityPoints;
		if(HumidityPoints>=15)
		{
			signalManager.EmitSignal(nameof(General.OnPlayerDeath), this);
		}
		GD.Print("puntos de humeda: "+HumidityPoints);
	}



	public void Teleport()
	{
		//GD.Print(GlobalPosition);
		//GD.Print(ActiveTeleporter.Position);
		teleportSound.Play();
		Position=ActiveTeleporter.Position;
		ActiveTeleporter.QueueFree();
		ActiveTeleporter=null;
	}

	
	private void _on_Jugador_input_event(object viewport, object @event, int shape_idx)
	{
		if(Escenario.MartianTurn!=IsMartian)
		{
			return;
		}

		if(@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left && !MouseButtonEvent.Pressed)
			{
				if(Inventory.Unopenable)
				{
					return;
				}

				if(Inventory.SelectedPlayer!=null && Inventory.SelectedPlayer!=this)
				{
					return;
				}

				if(!IsOnFloor() && ActiveMagnet==null)
				{
					return;
				}

				if(Inked)
				{
					GetTree().CallGroup("Escenarios", "ChangeInkVisibility", true);
				}

				AddChild(Inventory.GetInventory());
			}
		}
	}

}









