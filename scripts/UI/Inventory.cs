using Godot;
using System;

public class Inventory : InventorySelection
{
    readonly string[] toolNames=new string[9]
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

    Label starsAvailable;

    public static bool Unopenable {get; set;} =false;

    Texture teleportTexture=GD.Load<Texture>("res://sprites/tools/card_teleport.png");

    public override void _Ready()
    {
        EventManager.OnTurnChanged+=OnTurnChanged;

        TextureButton moveButton=GetNode<TextureButton>("Move/Select");
        
        Unopenable=true;
        player=GetParent<Jugador>();
        bool disableMovement=player.Moved|player.Frozen;
        moveButton.Disabled=disableMovement;

        ConfigureButtons();
        ConfigureCounters();
        InitializeCounters();

        starsAvailable=GetNode<Label>("Stars/Label");
        if(player.IsMartian)
        {
            starsAvailable.Text=Escenario.MartiansStars.ToString();
        }
        else
        {
            starsAvailable.Text=Escenario.AstronautsStars.ToString();
        }

        if(player.ActiveTeleporter!=null)
        {
            TextureButton teleporterButton=(TextureButton)selectButtons[6];
            teleporterButton.TextureNormal=teleportTexture;
        }
    
    }

    private void OnTurnChanged(bool isMartianTurn)
    {
        SelectedPlayer=null;
        Unopenable=false;
    }

    private void InitializeCounters()
    {
        for(int i=0;i<player.ToolsAvailable.Length;i++)
        {
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
        int stars = player.IsMartian ? Escenario.MartiansStars : Escenario.AstronautsStars;

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
            int stars = player.IsMartian ? Escenario.MartiansStars : Escenario.AstronautsStars;
            stars += toolPrices[tool];
            player.ToolsAvailable[tool] -= 1;
            UpdateStarsAndLabel(stars);
            counters[tool].Text = player.ToolsAvailable[tool].ToString();
        }
    }

    private void UpdateStarsAndLabel(int stars)
    {
        if (player.IsMartian)
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
        SelectedPlayer=player;

        PackedScene toolToInvoke=GD.Load<PackedScene>("res://scenes/Herramientas/"+toolNames[tool]+".tscn");
        Node2D toolNode=(Node2D)toolToInvoke.Instance();

        Escenario escenario=GetTree().Root.GetNode<Escenario>("Escenario");

        if(toolNode is Teleporter && player.ActiveTeleporter!=null)
        {
            player.Teleport();
            CloseInventory();
            return;
        }

        if(player.ToolsAvailable[tool]==0)
        {
            return;
        }

        if(toolNode is GloboTeledirigido)
        {
            escenario.AddChild(toolNode);
            toolNode.Position=player.Position;
            toolNode.Position+=new Vector2(0,-100); //altura respecto al jugador
            ToolSelection(tool);
            return;
        }

        if(toolNode is Lanzaglobos)
        {
            player.AddChild(toolNode);
            ToolSelection(tool);
            return;
        }


        Throwable throwable=(Throwable)toolNode;
        Thrower lanzador=Thrower.GetThrower(throwable, throwable.MaxSize);


        //instanciar la herramienta
/*         if(throwable is Teleporter)
        {
            player.AddChild(throwable);
            GetTree().Root.AddChild(lanzador);
            ToolSelection(tool);
            return;
        } */

        if(throwable is Teleporter teleporter)
        {
            player.ActiveTeleporter=teleporter;
        }

        throwable.Position=player.Position;
        escenario.AddChild(throwable);

        //instanciar el lanzador
        escenario.AddChild(lanzador);
        ToolSelection(tool);
    }

    private void ToolSelection(byte tool)
    {
        player.ToolsAvailable[tool]-=1;
        player.Moved=false;
        //SelectedPlayer=null;
        QueueFree();
    }

    private void _on_Move_pressed()
    {
        Thrower lanzador=Thrower.GetThrower(player, player.MaxSize);
        GetTree().Root.AddChild(lanzador);
        player.Moved=true;
        SelectedPlayer=player;
        //CloseInventory();
        QueueFree();

    }

    private void _on_Close_pressed()
    {
        CloseInventory();
    }

    private void CloseInventory()
    {
        Unopenable=false;
        QueueFree();
    }

    public static Inventory GetInventory()
	{
		PackedScene inventorySelection=(PackedScene)ResourceLoader.Load("res://scenes/UI/Inventory.tscn");
        Inventory instance=(Inventory)inventorySelection.Instance();
		return instance;
	}

}

