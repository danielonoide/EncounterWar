using Godot;
using System;

public class Inventory : InventorySelection
{
    string[] toolNames=new string[9]
    {
        "GloboConAgua",
        "GloboConTinta",
        "GloboDeHielo",
        "GloboDeTiempo",
        "GloboTeledirigido",
        "Lanzaglobos",
        "Teletransportador",
        "Platano",
        "Iman"
    };
    Godot.Collections.Array selectButtons;
    Jugador player;

	public static Jugador SelectedPlayer {get; set;}=null;

    //byte[] toolsAvailable;
    Label starsAvailable;

    public static bool InventoryOpened {get; set;} =false;

    //public static bool disableMoveButton {get; set;} = false;
    public override void _Ready()
    {
        TextureButton moveButton=GetNode<TextureButton>("Move/Select");
        
        InventoryOpened=true;
        player=GetParent<Jugador>();
        moveButton.Disabled=player.Moved;
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

        PackedScene toolToInvoke=GD.Load<PackedScene>("res://scenes/Herramientas/"+toolNames[tool]+".tscn");
        Throwable throwable=(Throwable)toolToInvoke.Instance();
        Thrower lanzador=Thrower.GetThrower(throwable, throwable.MaxSize);


        //instanciar la herramienta
        throwable.Position=player.Position;
        GetTree().Root.AddChild(throwable);

        //instanciar el lanzador
        GetTree().Root.AddChild(lanzador);

		//GetParent().GetNode("Throwable").AddChild(lanzador);
        
        //GetTree().CallGroup("Escenarios", "ChangeTurn");

        player.ToolsAvailable[tool]-=1;
        player.Moved=false;
        SelectedPlayer=null;
        CloseInventory();

    }

    private void _on_Move_pressed()
    {
        Thrower lanzador=Thrower.GetThrower(player, player.MaxSize);
        GetTree().Root.AddChild(lanzador);
        player.Moved=true;
        SelectedPlayer=player;
        CloseInventory();

    }

    private void _on_Close_pressed()
    {
        //Visible=false;
        CloseInventory();
    }

    private void CloseInventory()
    {
        InventoryOpened=false;
        QueueFree();
    }

    public static Inventory GetInventory()
	{
		PackedScene inventorySelection=(PackedScene)ResourceLoader.Load("res://scenes/UI/Inventory.tscn");
        Inventory instance=(Inventory)inventorySelection.Instance();
        //instance.toolsAvailable=_toolsAvailable;
		return instance;
	}

}

