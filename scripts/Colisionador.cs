using Godot;
using System;

public class Colisionador : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    CollisionShape2D collider;
    public override void _Ready()
    {
        collider=GetNode<CollisionShape2D>("CollisionShape2D");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }


    public bool CheckIfFree(Vector2 posicion)
    {
        
        Vector2 from = Position;
        Vector2 to = posicion;

        // Obtiene el espacio de colisión directo del área
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;

        // Crea un RaycastResult para almacenar el resultado de la colisión
        Godot.Collections.Dictionary result = spaceState.IntersectRay(from, to, new Godot.Collections.Array { collider }, 1);

        if (result.Count > 0)
        {
            // Hay una colisión en la línea entre 'from' y 'to'
            return false;
        }
        else
        {
            // No hay colisión
            return true;
        }
        
    }

}
