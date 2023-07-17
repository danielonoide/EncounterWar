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

    static byte[] starsNumber=new byte[3]{18, 15, 12};

    static Dictionary<string, byte> toolPrices=new()
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
    
    Dictionary<string, int> astronautTools, martianTools;

    Label astronautsLabel, martiansLabel;

    Label[] counters;

    byte astronautsCounter, martiansCounter;

    public override void _Ready()
    {
        astronautTools=new();
        martianTools=new();

        astronautsLabel=GetNode<Label>("AstronautsCounter");
        martiansLabel=GetNode<Label>("MartiansCounter");

        astronautsCounter=martiansCounter=starsNumber[scenery];
        astronautsLabel.Text=martiansLabel.Text=astronautsCounter.ToString();

        Godot.Collections.Array addButtons=GetTree().GetNodesInGroup("AddButtons");
        Godot.Collections.Array subtractButtons=GetTree().GetNodesInGroup("SubtractButtons");

        for(int i=0;i<addButtons.Count;i++)
        {
            TextureButton addButton=(TextureButton)addButtons[i];
            TextureButton subtractButton=(TextureButton)subtractButtons[i];

            Label toolLabel=addButton.GetParent().GetNode<Label>("Label");
            if(!astronautTools.ContainsKey(toolLabel.Text))
            {
                astronautTools.Add(toolLabel.Text, 0);
            }

            if(!martianTools.ContainsKey(toolLabel.Text))
            {
                martianTools.Add(toolLabel.Text, 0);
            }

            string team=addButton.GetParent().GetParent().GetParent().Name;
            addButton.Connect("pressed", this, nameof(AddTool), new Godot.Collections.Array{toolLabel.Text, team});
            subtractButton.Connect("pressed", this, nameof(SubtractTool), new Godot.Collections.Array{toolLabel.Text, team});
        }

        var countersGroup=GetTree().GetNodesInGroup("Counters");
        counters=new Label[9];
        for(int i=0;i<countersGroup.Count;i++)
        {
            counters[i]=(Label)countersGroup[i];
        }

    }

    public override void _Process(float delta)
    {
        astronautsLabel.Text=astronautsCounter.ToString();
        martiansLabel.Text=martiansCounter.ToString();
    }

    //signals
    private void AddTool(string tool, string team)
    {
        if(team.Equals("Astronauts"))
        {
            if(astronautsCounter>=toolPrices[tool])
            {
                astronautsCounter-=toolPrices[tool];
                astronautTools[tool]+=1;
            }
        }
        else
        {
            if(martiansCounter>=toolPrices[tool])
            {
                martiansCounter-=toolPrices[tool];
                martianTools[tool]+=1;
            }
        }
    }

    private void SubtractTool(string tool, string team)
    {
        if(team.Equals("Astronauts"))
        {
            if(astronautTools[tool]>0)
            {
                astronautsCounter+=toolPrices[tool];
                astronautTools[tool]-=1;
            }
        }
        else
        {
            if(martianTools[tool]>0)
            {
                martiansCounter+=toolPrices[tool];
                martianTools[tool]-=1;
            }
        }
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
