using Godot;
using System;

public class Inventory : InventorySelection
{
    Godot.Collections.Array selectButtons;
    byte[] toolsAvailable;
    public override void _Ready()
    {
        Visible=false;
        GD.Print("tools available: "+toolsAvailable.Length);
        ConfigureButtons();
        ConfigureCounters();
        InitializeCounters();
        Godot.Collections.Array arr=GetTree().GetNodesInGroup("AddButtonss");
        GD.Print("counters: "+arr.Count);
    }

    private void InitializeCounters()
    {
        for(int i=0;i<toolsAvailable.Length;i++)
        {
            //GD.Print(toolsAvailable[i]);
            //GD.Print(counters[i].GetParent().GetParent().GetParent().GetParent().Name);

            counters[i].Text=toolsAvailable[i].ToString();
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
        GD.Print(tool);
    }

    protected override void SubtractTool(byte tool)
    {
        GD.Print(tool);
    }

    private void SelectTool(byte tool)
    {
        GD.Print(tool);
    }

    private void _on_Move_pressed()
    {

    }

    private void _on_Close_pressed()
    {
        Visible=false;
    }

    public static Inventory GetInventory(byte[] _toolsAvailable)
	{
		PackedScene inventorySelection=(PackedScene)ResourceLoader.Load("res://scenes/Inventory.tscn");
        Inventory instance=(Inventory)inventorySelection.Instance();
        instance.toolsAvailable=_toolsAvailable;
		return instance;
	}

}

