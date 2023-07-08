using Godot;
using System;

public class Escenario2 : Escenario
{


    public override void _Ready()
    {
        leftLimit=-2000f;
        rightLimit=2000f;
        topLimit=-2500f;
        bottomLimit=700f;
        base._Ready();
    }

}
