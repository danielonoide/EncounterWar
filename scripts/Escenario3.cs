using Godot;
using System;

public class Escenario3 : Escenario
{
    public override void _Ready()
    {
        leftLimit=-1400f;
        rightLimit=3000f;
        topLimit=-1200f;
        bottomLimit=1200f;
        base._Ready();
        Globals.Gravity=(int)Constants.Gravities.MoonGravity;


    }
}
