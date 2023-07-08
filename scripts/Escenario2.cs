using Godot;
using System;

public class Escenario2 : Escenario
{
/* 	protected new float leftLimit=-2000f;  //a los límites de la cámara le restamos la mitad de su ancho
	protected new float rightLimit=2000f;
	protected new float topLimit=-2500f;
	protected new float bottomLimit=700f; */

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        leftLimit=-2000f;
        rightLimit=2000f;
        topLimit=-2500f;
        bottomLimit=700f;
        base._Ready();
    }

}
