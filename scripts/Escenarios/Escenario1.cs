using Godot;
using System;

public class Escenario1 : Escenario
{
    bool dayTime=true;
    TextureRect nightBackground;
    CanvasModulate lightning;
    public override void _Ready()
    {
        astronautsCameraPosition=new Vector2(-1420, -149);
        martiansCameraPosition=new Vector2(1420, -149);
        base._Ready();
        Globals.Gravity=(int)Constants.Gravities.MarsGravity;
        nightBackground=GetNode<TextureRect>("ParallaxBackground/ParallaxLayer/NightBg");
        lightning=GetNode<CanvasModulate>("CanvasModulate");
    }

    private void _on_DayTimer_timeout()
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
