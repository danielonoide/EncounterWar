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
            starsAvailable.Text=Escenario.MartiansStars.ToString();
        }
        else
        {
            starsAvailable.Text=Escenario.AstronautsStars.ToString();
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
/* 
    private void UpdateStarsAndTool(byte tool, sbyte sum)
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

    } */

    protected override void AddTool(byte tool)
    {
        int stars = player.isMartian ? Escenario.MartiansStars : Escenario.AstronautsStars;

        if (stars >= toolPrices[tool])
        {
            stars -= toolPrices[tool];
            player.ToolsAvailable[tool] += 1;
            UpdateStarsAndLabel(stars);
            counters[tool].Text = player.ToolsAvailable[tool].ToString();
        }

    }

    protected override void SubtractTool(byte tool)
    {
        if (player.ToolsAvailable[tool] > 0)
        {
            int stars = player.isMartian ? Escenario.MartiansStars : Escenario.AstronautsStars;
            stars += toolPrices[tool];
            player.ToolsAvailable[tool] -= 1;
            UpdateStarsAndLabel(stars);
            counters[tool].Text = player.ToolsAvailable[tool].ToString();
        }
    }

    private void UpdateStarsAndLabel(int stars)
    {
        if (player.isMartian)
        {
            Escenario.MartiansStars = stars;
        }
        else
        {
            Escenario.AstronautsStars = stars;
        }

        starsAvailable.Text = stars.ToString();
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

