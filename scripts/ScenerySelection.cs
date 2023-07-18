using Godot;
using System;

public class ScenerySelection : CanvasLayer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    TextureButton[] startMatchButtons=new TextureButton[3];

    public override void _Ready()
    {
        var arr=new Godot.Collections.Array();
        arr=GetTree().GetNodesInGroup("BotonesEmpezarPartida");

        for(int i=0;i<startMatchButtons.Length;i++)
		{
			startMatchButtons[i]=(TextureButton)arr[i];
            startMatchButtons[i].Connect("pressed", this, nameof(OpenInventorySelection), new Godot.Collections.Array{i});
		}
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	public static CanvasLayer GetScenerySelection()
	{
		PackedScene scenerySelection=(PackedScene)ResourceLoader.Load("res://scenes/ScenerySelection.tscn");
		return (CanvasLayer)scenerySelection.Instance();
	}

    private void _on_Close_pressed()
    {
        QueueFree();
    }

    private void OpenInventorySelection(byte scenery)
    {
        //PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/Escenario"+scenery+".tscn");
        //GetTree().ChangeScene("res://scenes/Escenario"+scenery+".tscn");
        InventorySelection inventorySelection=InventorySelection.GetInventorySelection(scenery);
        AddChild(inventorySelection);     
    }
}
