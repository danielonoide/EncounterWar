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

	public bool HasToFall {get; set;} =false;

	public bool falling=false;





	public Teleporter ActiveTeleporter { get; set; }=null;

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

	public override void _Ready()  //se ejecuta cuando carga el nodo
	{ 
		EventManager.OnTeleporterRemoved+=OnTeleporterRemoved;
		animatedSprite=GetNode<AnimatedSprite>("AnimatedSprite");
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

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		UpdateAnimation();
		if(HasToFall && IsOnFloor())
		{
			falling=true;
			HasToFall=false;
		}

		if(falling)
		{
			//GD.Print(falling);
			velocity.x=100;
			if(!IsOnFloor())
			{
				falling=false;
			}
		}

	}

	public void Teleport()
	{
		//GD.Print(GlobalPosition);
		//GD.Print(ActiveTeleporter.Position);

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

				if(!IsOnFloor())
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

/*     private void Fall()
    {
		while(IsOnFloor())
		{
			velocity.x=10;
		}
    } */
}









