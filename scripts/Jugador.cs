using Godot;
using System;


public class Jugador : KinematicBody2D
{
	public Vector2 velocidad=new Vector2(0,0);  //x y y son de tipo float
	int speed=500;
	int altura=180;
	float timer=(float)0.5;
	float TiempoAire=(float)0.15;
	//int y=0;
	//float t=(float)-0.1; //variable hecha para hacer retardos
	public bool isMartian=false;
	public static bool ThrowerGenerated=false;

	public byte[] ToolsAvailable {get; set;}

	AnimatedSprite animatedSprite;
	
	public void setVelocidad(Vector2 Vector)
	{
		this.velocidad=Vector;
	}
	
	public override void _Ready()  //se ejecuta cuando carga el nodo
	{ 
		ThrowerGenerated=false;
		animatedSprite=GetNode<AnimatedSprite>("AnimatedSprite");
	}
	
	public override void _Process(float delta) // se ejecuta cada frame, si el juego va a 60 fps, se ejecuta 60 veces por segundo
	{
		if(Input.IsActionPressed("Right"))
		{
			GetNode<Sprite>("Sprite").FlipH=true;
		}
		
		if(Input.IsActionPressed("Left"))
		{
			GetNode<Sprite>("Sprite").FlipH=false;
		}
	}
	
	public override void _PhysicsProcess(float delta) //se ejecuta 60 veces por segundo
	{
//		if(Input.IsActionPressed("Down") && 0>timer)
//		{
//			RigidBody2D Lanzable=Throwable.GetThrowable();
//			Lanzable.Position=Position+new Vector2(0,-100);
//			GetParent().AddChild(Lanzable);
//			if(GetNode<Sprite>("Sprite").FlipH)
//			{
//				Lanzable.ApplyImpulse(new Vector2(0,10), new Vector2(200,-200));
//			}
//			else{
//				Lanzable.ApplyImpulse(new Vector2(0,-10), new Vector2(-200,-200));				
//			}
//			timer=(float)0.5;
//		}
//		if(0<timer) timer-=delta;
		//MovimientoYGravedad(delta);
		
		velocidad=MoveAndSlide(velocidad, Vector2.Up);
		velocidad.y+=Globals.Gravity*delta;

		if(IsOnFloor()) 
		{
			velocidad.x=0;
			
			if(isMartian)
			{
				animatedSprite.Animation="martian_idle";
			}
			else
			{
				animatedSprite.Animation="astronaut_idle";
			}
		}
		else
		{
			if(isMartian)
			{
				animatedSprite.Animation="martian_jump";
			}
			else
			{
				animatedSprite.Animation="astronaut_jump";
			}
		}		
	}
	
	public void MovimientoYGravedad(float delta)
	{
		//Label label=GetParent().GetNode<CanvasLayer>("CanvasLayer").GetNode<Label>("Label");
		
		//int.Parse solo puede convertir cadenas
		//Convert.ToInt32 puede convertir variables boolenas también
		
		
		velocidad.x=Convert.ToInt32(Input.IsActionPressed("Right")) - Convert.ToInt32(Input.IsActionPressed("Left"));
		//velocidad.y=Convert.ToInt32(Input.IsActionPressed("Down")) - Convert.ToInt32(Input.IsActionPressed("Up"));
		
		velocidad.x*=speed;
		//velocidad.y*=speed;
		velocidad=MoveAndSlide(velocidad, Vector2.Up);  //tenemos que estipular la dirección de la fuerza normal como segundo argumento. retorna un vector2 que representa el movimiento que quedó después de ejecutar la función (la desaceleración)
		velocidad.y+=Globals.Gravity*delta;  //a=(vf-vi)/t
		
		if(IsOnFloor()){  //esta funcion es recomendable que se use despues de MoveAndSlide
			//velocidad.y=0; deja de ser necesario por que se usa el retorno de MoveAndSlide()
			speed=300;
			Salto(delta);
			TiempoAire=0.15f;
		}
		else{
			if(!(0>=TiempoAire)) 
			{
				Salto(delta);
			}
			if(0<TiempoAire) TiempoAire-=delta;
			//t=(float)-0.1;
			//y=0;
			speed=150;
		} 
		
		
		
		
		
		
		/*
		velocidad.x=0;
		velocidad.y=0;
		
		if(Input.IsActionPressed("Right")){
			velocidad.x+=speed;
		}
		
		if(Input.IsActionPressed("Left")){
			velocidad.x-=speed;
		}
		
		if(Input.IsActionPressed("Up")){
			velocidad.y-=speed;
		}
		
		if(Input.IsActionPressed("Down")){
			velocidad.y+=speed;
		}
		
		//Position+=velocidad; incorrecto porque no toma en cuenta las colisiones
		//MoveAndCollide(velocidad); cuando choca con algo se queda quieto, y el vector se tiene que multiplicar por delta para que se mueva suavemente
		MoveAndSlide(velocidad); //cuando choca con algo se puede delizar a los lados, la funcion se encarga de multiplicar por delta
		
		*/
	
	}
	
	public void Salto(float delta)
	{
		
		if(Input.IsActionPressed("Up")){
			velocidad.y= - (float)Math.Sqrt(2*Globals.Gravity*altura);  //vi=sqrt(2*g*h)
		}
		
		/*
		if(Input.IsActionPressed("Up") && -900<y && 0>t)
		{
			y-=(int)Math.Sqrt(gravedad*altura);
			t=(float)0.5;
		}
		else if(!Input.IsActionPressed("Up")){
			velocidad.y=y;
			y=0;
		}
		label.Text=Convert.ToString(-(y)); //escribir en una etiqueta
		if(0<t) t-=delta;
		*/
		
		
	}
	
		
	private void _on_Area2D_body_entered(object body)
	{
		//GD.Print("Entró");
		if(body is Line2D)
		{
			GD.Print("SIII");
		}
	}

	private void _on_Jugador_input_event(object viewport, object @event, int shape_idx)
	{
		if(Escenario.MartianTurn!=isMartian)
		{
			return;
		}

		if(@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex==(int)ButtonList.Left && !ThrowerGenerated && !MouseButtonEvent.Pressed)
			{
/* 				Thrower2 Lanzador=Thrower2.GetThrower();
				Lanzador.Ball=this;
				Lanzador.Position=Position;
				Lanzador.Position+=new Vector2(0,35);
				GetParent().AddChild(Lanzador);
				
				
				ThrowerGenerated=true; */
				//GetNode<Inventory>("Inventory").Visible=true;
				AddChild(Inventory.GetInventory());
			}
		}
	}
	
}









