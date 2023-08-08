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
    public static bool Open {get; set;} =false;


    Texture teleportTexture=GD.Load<Texture>("res://sprites/tools/cards/normal/card_teleport.png");
    Texture teleportTextureGold=GD.Load<Texture>("res://sprites/tools/cards/gold/card_teleport_gold.png");

    Escenario escenario;

    //static bool suscribed=false;

    public override void _Ready()
    {
/*         if(!suscribed)
        {
            EventManager.OnTurnChanged+=OnTurnChanged;
            suscribed=true;
        } */
        escenario=GetTree().Root.GetNode<Escenario>("Escenario");


        TextureButton moveButton=GetNode<TextureButton>("Move/Select");
        Open=true;
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
            teleporterButton.TextureHover=teleportTextureGold;
        }
    
    }

/*     private void OnTurnChanged(bool isMartianTurn)
    {
        SelectedPlayer=null;
        Unopenable=false;
    }
 */
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

        if(toolNode is Platano)
        {
            escenario.AddChild(toolNode);
            ToolSelection(tool);
            return;
        }


        Throwable throwable=(Throwable)toolNode;
        Thrower lanzador=Thrower.GetThrower(throwable, throwable.MaxSize);

        if(throwable is Teleporter teleporter)
        {
            player.ActiveTeleporter=teleporter;
        }

        throwable.Position=player.Position;
        escenario.AddChild(throwable);

        //instanciar el lanzador
        throwable.AddChild(lanzador);
        ToolSelection(tool);
    }

    private void ToolSelection(byte tool)
    {
        player.ToolsAvailable[tool]-=1;
        player.Moved=false;
        //SelectedPlayer=null;
        Open=false;
        QueueFree();
    }

    private void _on_Move_pressed()
    {
        Thrower lanzador=Thrower.GetThrower(player, player.MaxSize);
        player.AddChild(lanzador);
        player.Moved=true;
        SelectedPlayer=player;
        //CloseInventory();
        Open=false;
        QueueFree();

    }

    private void _on_Close_pressed()
    {
        CloseInventory();
    }

    private void CloseInventory()
    {
        Open=false;
        Unopenable=false;
		GetTree().CallGroup("Escenarios", "ChangeInkVisibility", false);
        QueueFree();
    }

    public static Inventory GetInventory()
	{
		PackedScene inventorySelection=(PackedScene)ResourceLoader.Load("res://scenes/UI/Inventory.tscn");
        Inventory instance=(Inventory)inventorySelection.Instance();
		return instance;
	}

}

