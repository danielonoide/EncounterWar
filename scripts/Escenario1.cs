using Godot;
using System;

public class Escenario1 : Escenario
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    bool dayTime=true;
    TextureRect nightBackground;
    CanvasModulate lightning;
    public override void _Ready()
    {
        base._Ready();
        nightBackground=GetNode<TextureRect>("ParallaxBackground/ParallaxLayer/NightBg");
        lightning=GetNode<CanvasModulate>("CanvasModulate");
    }

    private void _on_Timer_timeout()
    {
        if(dayTime)
        {
            nightBackground.Visible=true;
            lightning.Visible=true;
            dayTime=false;
        }
        else
        {
            nightBackground.Visible=false;
            lightning.Visible=false;
            dayTime=true;
        }
    }



}
