using Godot;
using System;

public class Inventory : CanvasLayer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}


public class InventoryItem
{
    public string ObjectType { get; }
    public int Quantity { get; set; }

    public InventoryItem(string objectType, int quantity)
    {
        ObjectType = objectType;
        Quantity = quantity;
    }
}