using Godot;
using System;

public class Inventory : InventorySelection
{
    Godot.Collections.Array selectButtons;
    Jugador player;
    //byte[] toolsAvailable;
    Label starsAvailable;
    public override void _Ready()
    {
        player=GetParent<Jugador>();
        ConfigureButtons();
        ConfigureCounters();
        InitializeCounters();
        //Godot.Collections.Array arr=GetTree().GetNodesInGroup("AddButtons");
        //GD.Print("counters: "+arr.Count);
        starsAvailable=GetNode<Label>("Stars/Label");
        if(player.isMartian)
        {
            starsAvailable.Text=Escenario.AstronautsStars.ToString();
        }
        else
        {
            starsAvailable.Text=Escenario.MartiansStars.ToString();
        }
    
    }

    private void InitializeCounters()
    {
        for(int i=0;i<player.ToolsAvailable.Length;i++)
        {
            //GD.Print(toolsAvailable[i]);
            //GD.Print(counters[i].GetParent().GetParent().GetParent().GetParent().Name);

            counters[i].Text=player.ToolsAvailable[i].ToString();
        }
    }

    protected override void ConfigureButtons()
    {
        base.ConfigureButtons();
        selectButtons=GetTree().GetNodesInGroup("SelectButtons");

        for(int i=0;i<selectButtons.Count;i++)
        {
            TextureButton selectButton=(TextureButton)selectButtons[i];
            selectButton.Connect("pressed", this, nameof(SelectTool), new Godot.Collections.Array{i});
        }
    }

    protected override void AddTool(byte tool)
    {
        if(player.isMartian)
        {
            if(Escenario.AstronautsStars>=toolPrices[tool])
            {
                Escenario.AstronautsStars-=toolPrices[tool];
                player.ToolsAvailable[tool]+=1;
                starsAvailable.Text=Escenario.AstronautsStars.ToString();
                counters[tool].Text=player.ToolsAvailable[tool].ToString();
            }
        }
        else
        {
            if(Escenario.MartiansStars>=toolPrices[tool])
            {
                Escenario.MartiansStars-=toolPrices[tool];
                player.ToolsAvailable[tool]+=1;
                starsAvailable.Text=Escenario.MartiansStars.ToString();
                counters[tool].Text=player.ToolsAvailable[tool].ToString();
            }
        }

    }

    protected override void SubtractTool(byte tool)
    {
        if(player.isMartian)
        {
            if(player.ToolsAvailable[tool]>0)
            {
                Escenario.AstronautsStars+=toolPrices[tool];
                player.ToolsAvailable[tool]-=1;
                starsAvailable.Text=Escenario.AstronautsStars.ToString();
                counters[tool].Text=player.ToolsAvailable[tool].ToString();
            }
        }
        else
        {
            if(player.ToolsAvailable[tool]>0)
            {
                Escenario.MartiansStars+=toolPrices[tool];
                player.ToolsAvailable[tool]-=1;
                starsAvailable.Text=Escenario.MartiansStars.ToString();
                counters[tool].Text=player.ToolsAvailable[tool].ToString();
            }
        }
    }

    private void SelectTool(byte tool)
    {
        if(player.ToolsAvailable[tool]==0)
        {
            return;
        }
        GetTree().CallGroup("Escenarios", "ChangeTurn");
        QueueFree();
    }

    private void _on_Move_pressed()
    {
        QueueFree();
    }

    private void _on_Close_pressed()
    {
        //Visible=false;
        QueueFree();
    }

    public static Inventory GetInventory()
	{
		PackedScene inventorySelection=(PackedScene)ResourceLoader.Load("res://scenes/Inventory.tscn");
        Inventory instance=(Inventory)inventorySelection.Instance();
        //instance.toolsAvailable=_toolsAvailable;
		return instance;
	}

}

