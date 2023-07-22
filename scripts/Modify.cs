using Godot;
using System;



static class Constants
{
/* 	public const int ButtonWheelUp=4;
	public const int ButtonWheelDown=5;
 */
	public enum Gravities
	{
		MarsGravity=500, //óptimo 600 500
		SpaceGravity=125, //óptimo 500  125
		MoonGravity=250 //óptimo 700  250
	};
/* 	public const int MarsGravity=600; //óptimo 600
	public const int SpaceGravity=500; //óptimo 500
	public const int MoonGravity=700; //óptimo 700 */
}

static class Volume
{
	public static float[] Volumes=new float[]{1,1,1};
	//1 es general, 2 es musica y 3 es sfx
}

static class Globals
{
	private static int gravity;

	public static int Gravity
	{
		get 
		{
			return gravity;
		}
		set
		{
			if(value>0 && Enum.IsDefined(typeof(Constants.Gravities), value))
			{
				gravity=value;
			}
		}
	}
}

public class EventManager
{
	public delegate void TurnChangedEventHandler(bool isMartianTurn);
	public static event TurnChangedEventHandler OnTurnChanged;

	public static void NotifyTurnChanged(bool isMartianTurn)
	{
		OnTurnChanged?.Invoke(isMartianTurn);
	}

	public delegate void TeleporterRemovedEventHandler(Teleporter teleporter);
	public static event TeleporterRemovedEventHandler OnTeleporterRemoved;

	public static void NotifyTeleporterRemoved(Teleporter teleporter)
	{
		OnTeleporterRemoved?.Invoke(teleporter);
	}
}


public class Modify : Node2D
{

	public static Node2D GetGlobals()
	{
		PackedScene Globals=(PackedScene)ResourceLoader.Load("res://scenes/Globals.tscn");
		return (Node2D)Globals.Instance();
	}
	
	public static void ChangeScale(CanvasLayer Nodo, Vector2 Vector)
	{
		Nodo.Scale=Vector;
	}
	
	public static void ChangeScale(TextureButton Nodo, Vector2 Vector)
	{
		Nodo.RectScale=Vector;
	}
	
	public static void ChangePosition(TextureButton TextB, Vector2 Vector)
	{
		TextB.RectPosition=Vector;
	}
	
	
}

