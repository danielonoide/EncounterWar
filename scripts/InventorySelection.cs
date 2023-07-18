using Godot;
using System;
using System.Collections.Generic;

public class InventorySelection : CanvasLayer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

/*     const byte StarsScenery1=18;
    const byte StarsScenery2=15;
    const byte StarsScenery3=12; */

    byte scenery;

    static readonly byte[] starsNumber=new byte[3]{18, 15, 12};

    protected static readonly byte[] toolPrices=new byte[9]
    {
        1,3,1,2,3,2,2,2,2
    };

    public static byte[] astronautsTools, martiansTools;

    /*
    tool numbers:
     1   "Globo con agua"
     2   "Globo con tinta"
     3   "Globo de hielo"
     4   "Globo de tiempo"
     5   "Globo teledirigido"
     6   "Lanzaglobos"
     7   "Teletransportador"
     8   "Plátano"
     9   "Imán"
    */

/*     static Dictionary<string, byte> toolPrices=new()
    {
        {"Globo con agua", 1},
        {"Globo con tinta", 3},
        {"Globo de hielo", 1},
        {"Globo de tiempo", 2},
        {"Globo teledirigido", 3},
        {"Lanzaglobos", 2},
        {"Teletransportador", 2},
        {"Plátano", 2},
        {"Imán", 2},
    };
     */
    //Dictionary<string, int> astronautTools, martianTools;

    Label astronautsLabel, martiansLabel;

    protected Label[] counters;

    TextureButton[] readyButtons;

    byte astronautsCounter, martiansCounter;

    public override void _Ready()
    {
        astronautsLabel=GetNode<Label>("AstronautsCounter");
        martiansLabel=GetNode<Label>("MartiansCounter");

        astronautsCounter=martiansCounter=starsNumber[scenery];
        astronautsLabel.Text=martiansLabel.Text=astronautsCounter.ToString();

        //reiniciar inventarios
        astronautsTools=new byte[9]{0,0,0,0,0,0,0,0,0};
        martiansTools=new byte[9]{0,0,0,0,0,0,0,0,0};

        ConfigureButtons();
        ConfigureCounters();
        ConfigureReadyButtons();
    }

    private void ConfigureReadyButtons()
    {
        Godot.Collections.Array readyButtonsGroup=GetTree().GetNodesInGroup("ReadyButtons");
        readyButtons=new TextureButton[2];
        for(int i=0;i<readyButtonsGroup.Count;i++)
        {
            TextureButton readyButton=(TextureButton)readyButtonsGroup[i];
            readyButtons[i]=readyButton;
            readyButton.Connect("pressed", this, nameof(ReadyButtonPressed), new Godot.Collections.Array{i});
        }
    }

    protected void ConfigureCounters()
    {
        var countersGroup=GetTree().GetNodesInGroup("Counters");
        counters=new Label[countersGroup.Count];
        for(int i=0;i<countersGroup.Count;i++)
        {
            counters[i]=(Label)countersGroup[i];
        }
    }

    protected virtual void ConfigureButtons()
    {
        Godot.Collections.Array addButtons=GetTree().GetNodesInGroup("AddButtons");
        Godot.Collections.Array subtractButtons=GetTree().GetNodesInGroup("SubtractButtons");

        for(int i=0;i<addButtons.Count;i++)
        {
            TextureButton addButton=(TextureButton)addButtons[i];
            TextureButton subtractButton=(TextureButton)subtractButtons[i];

            addButton.Connect("pressed", this, nameof(AddTool), new Godot.Collections.Array{i});
            subtractButton.Connect("pressed", this, nameof(SubtractTool), new Godot.Collections.Array{i});
        }
    }

    //signals
    protected virtual void AddTool(byte tool)
    {
        if(tool<9)
        {
            readyButtons[0].Disabled=false;
            if(astronautsCounter>=toolPrices[tool])
            {
                astronautsCounter-=toolPrices[tool];
                astronautsLabel.Text=astronautsCounter.ToString();
                astronautsTools[tool]+=1;
                counters[tool].Text=astronautsTools[tool].ToString();
            }
        }
        else
        {
            readyButtons[1].Disabled=false;
            if(martiansCounter>=toolPrices[tool-9])
            {
                martiansCounter-=toolPrices[tool-9];
                martiansLabel.Text=martiansCounter.ToString();
                martiansTools[tool-9]+=1;
                counters[tool].Text=martiansTools[tool-9].ToString();
            }
        }
    }

    protected virtual void SubtractTool(byte tool)
    {
        if(tool<9)
        {
            readyButtons[0].Disabled=false;
            if(astronautsTools[tool]>0)
            {
                astronautsCounter+=toolPrices[tool];
                astronautsLabel.Text=astronautsCounter.ToString();
                astronautsTools[tool]-=1;
                counters[tool].Text=astronautsTools[tool].ToString();
            }
        }
        else
        {
            readyButtons[1].Disabled=false;
            if(martiansTools[tool-9]>0)
            {
                martiansCounter+=toolPrices[tool-9];
                martiansLabel.Text=martiansCounter.ToString();
                martiansTools[tool-9]-=1;
                counters[tool].Text=martiansTools[tool-9].ToString();
            }
        }

    }

    private void ReadyButtonPressed(byte button)
    {
        readyButtons[button].Disabled=true;
        if(readyButtons[0].Disabled==true && readyButtons[1].Disabled==true)
        {
            StartMatch(scenery);
        }
    }

    private void StartMatch(byte scenery)
    {
        GetTree().ChangeScene("res://scenes/Escenario"+(scenery+1)+".tscn");

/*         Escenario escenario=Escenario.GetScenery((byte)(scenery+1), astronautsTools, martiansTools);
        GetTree().Root.AddChild(escenario);
        GetTree().Root.RemoveChild(GetParent().GetParent()); */
    }

    private void _on_Close_pressed()
    {
        QueueFree();
    }

	public static InventorySelection GetInventorySelection(byte _scenery)
	{
		PackedScene inventorySelection=(PackedScene)ResourceLoader.Load("res://scenes/InventorySelection.tscn");
        InventorySelection instance=(InventorySelection)inventorySelection.Instance();
        instance.scenery=_scenery;
		return instance;
	}

}
