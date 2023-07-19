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

        astronautsCameraPosition=new Vector2(-941, -58);
        martiansCameraPosition=new Vector2(941, -58);
        base._Ready();
        Globals.Gravity=(int)Constants.Gravities.SpaceGravity;
    }

}
