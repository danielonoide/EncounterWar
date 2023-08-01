using Godot;
using System;

public class ScenerySelection : CanvasLayer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public enum LoadScenery 
    {
        Scenery1,
        Scenery2,
        Scenery3,
        Null
    }
    public static LoadScenery LoadGameData{get;set;}=LoadScenery.Null;
    TextureButton[] startMatchButtons=new TextureButton[3];
    TextureButton[] continueButtons=new TextureButton[3];

    public override void _Ready()
    {
        var startMatchButtonsArr=new Godot.Collections.Array();
        startMatchButtonsArr=GetTree().GetNodesInGroup("BotonesEmpezarPartida");

        for(int i=0;i<startMatchButtons.Length;i++)
		{
			startMatchButtons[i]=(TextureButton)startMatchButtonsArr[i];
            startMatchButtons[i].Connect("pressed", this, nameof(OpenInventorySelection), new Godot.Collections.Array{i});
		}

        Godot.Collections.Array continueButtonsArr=GetTree().GetNodesInGroup("ContinueButtons");
        for(int i=0;i<continueButtonsArr.Count;i++)
        {
            continueButtons[i]=(TextureButton)continueButtonsArr[i];
            continueButtons[i].Connect("pressed", this, nameof(OpenInventorySelectionLoading), new Godot.Collections.Array{i});
        }

        DisableContinueButtons();
    }

    private void DisableContinueButtons()
    {
        File saveGame=new();
        for(int i=0;i<Constants.SaveFileNames.Length; i++)
        {
            if(!saveGame.FileExists(Constants.SaveFileNames[i]))
            {
                continueButtons[i].Disabled=true;
            }
        }
    }

    private void OpenInventorySelectionLoading(byte scenery)
    {
        switch(scenery)
        {
            case 0:
                LoadGameData=LoadScenery.Scenery1;
                break;
            case 1:
                LoadGameData=LoadScenery.Scenery2;
                break;

            case 2:
                LoadGameData=LoadScenery.Scenery3;
                break;
        }

        GetTree().ChangeScene("res://scenes/Escenario"+(scenery+1)+".tscn");
    }


    private void OpenInventorySelection(byte scenery)
    {
        //PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/Escenario"+scenery+".tscn");
        //GetTree().ChangeScene("res://scenes/Escenario"+scenery+".tscn");
        InventorySelection inventorySelection=InventorySelection.GetInventorySelection(scenery);
        AddChild(inventorySelection);     
    }


    private void _on_Close_pressed()
    {
        QueueFree();
    }


    public static CanvasLayer GetScenerySelection()
	{
		PackedScene scenerySelection=(PackedScene)ResourceLoader.Load("res://scenes/UI/ScenerySelection.tscn");
		return (CanvasLayer)scenerySelection.Instance();
	}

}
