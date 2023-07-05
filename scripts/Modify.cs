using Godot;
using System;



static class Constants
{
	public const int ButtonWheelUp=4;
	public const int ButtonWheelDown=5;
}

static class Volume
{
	public static float[] Volumes=new float[]{1,1,1};
	//1 es general, 2 es musica y 3 es sfx
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

