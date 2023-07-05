using Godot;
using System;

public class Settings : CanvasLayer
{
	
	CheckButton Toggle;
	bool Toggled=true;
	Godot.Collections.Array Arr;
	HSlider[] Arr2=new HSlider[3];
	Label[] Etiquetas=new Label[3];
	
	public override void _Ready()
	{
		
		Arr=new Godot.Collections.Array();
		Arr=GetTree().GetNodesInGroup("HSlider");
		
		//Asignar al arreglo de TextureButton el arreglo Godot.Collections.Array
		for(int i=0;i<Arr2.Length;i++)
		{
			Arr2[i]=(HSlider)Arr[i];
		}
		
		//Conectar los eventos
		for(int i=0;i<Arr2.Length;i++)
		{
			Arr2[i].Connect("value_changed", this, nameof(HSliderValueChanged), new Godot.Collections.Array{i});
			Etiquetas[i]=Arr2[i].GetNode<Label>("Label");
			Etiquetas[i].Text=(Volume.Volumes[i]*100).ToString();
			Arr2[i].Value=Volume.Volumes[i]*100;
		}
		
		
		Toggle=GetNode("VBoxContainer").GetNode("HBoxContainer").GetNode<CheckButton>("CheckButton");
		Toggled=false;
		if(OS.WindowFullscreen)
		{
			Toggle.Pressed=true;
		}
		else
		{
			Toggle.Pressed=false;
		}
		Toggled=true;
		
	}
	
	public static CanvasLayer GetSettings()
	{
		PackedScene MenuAjustes=(PackedScene)ResourceLoader.Load("res://scenes/Settings.tscn");
		return (CanvasLayer)MenuAjustes.Instance();
	}
	
	private void _on_CheckButton_toggled(bool button_pressed)
	{
		if(Toggled) OS.WindowFullscreen=!OS.WindowFullscreen;
	}
	
	private void _on_Close_pressed()
	{
		QueueFree();
		GetTree().CallGroup("Menus", "CloseSettings");
	}
	
	private void HSliderValueChanged(float value, int Nodo)
	{
		AudioServer.SetBusVolumeDb(Nodo, GD.Linear2Db(value/100));
		Volume.Volumes[Nodo]=(float)value/100;
		Etiquetas[Nodo].Text= value.ToString();
	}


}




