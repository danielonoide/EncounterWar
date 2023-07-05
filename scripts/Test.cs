using Godot;
using System;

public class Test : Node2D
{
	Line2D Linea;
	RigidBody2D Cuerpo;
	public override void _Ready()
	{
		Linea=GetNode<Line2D>("Line2D");
		Cuerpo=GetNode<RigidBody2D>("Cuerpo");
		
		Linea.AddPoint(new Vector2(0,0));
		Vector2 direccion=new Vector2(100,-50).Normalized();
		float speed=100f;
		float Angle=direccion.Angle();
		for(float t=0;t<10;t+=0.01f)
		{
			float X=(speed*(float)Mathf.Cos(Angle)*t)+(0);
			float Y=(0.5f*9.8f*t*t)+(speed*(float)Mathf.Sin(Angle)*t)+(0);
			Vector2 NewPos=new Vector2(X,Y);
			Linea.AddPoint(NewPos);
		}
		
		Cuerpo.ApplyImpulse(new Vector2(0,0), direccion*speed*3.5f);
		
	}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _PhysicsProcess(float delta)
  {
	  
  }
}
