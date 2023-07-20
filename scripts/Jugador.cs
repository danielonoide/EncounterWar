using Godot;
using System;


public class Jugador : Throwable
{
	//public Vector2 velocidad=new Vector2(0,0);  //x y y son de tipo float
	int speed=500;
	int altura=180;
	float timer=(float)0.5;
	float TiempoAire=(float)0.15;
	//int y=0;
	//float t=(float)-0.1; //variable hecha para hacer retardos
	public bool isMartian=false;

	public byte[] ToolsAvailable {get; set;}

	AnimatedSprite animatedSprite;

    public override float MaxSize { get => 95; }

	public bool Moved {get; set;} =false;

	
	public override void _Ready()  //se ejecuta cuando carga el nodo
	{ 
		animatedSprite=GetNode<AnimatedSprite>("AnimatedSprite");
	}
		
	public override void _PhysicsProcess(float delta) //se ejecuta 60 veces por segundo
	{
		base._PhysicsProcess(delta);

		if(IsOnFloor()) 
		{
			
			if(isMartian)
			{
				animatedSprite.Animation="martian_idle";
			}
			else
			{
				animatedSprite.Animation="astronaut_idle";
			}
		}
		else
		{
			if(isMartian)
			{
				animatedSprite.Animation="martian_jump";
			}
			else
			{
				animatedSprite.Animation="astronaut_jump";
			}
		}		
	}
	
	private void _on_Jugador_input_event(object viewport, object @event, int shape_idx)
	{
		if(Escenario.MartianTurn!=isMartian)
		{
			return;
		}

		if(@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left && !MouseButtonEvent.Pressed)
			{
				if(Inventory.InventoryOpened)
				{
					return;
				}

				if(Inventory.SelectedPlayer!=null && Inventory.SelectedPlayer!=this)
				{
					return;
				}

				AddChild(Inventory.GetInventory());
			}
		}
	}
	
}









