using Godot;
using System;

public class PauseButton : CanvasLayer
{
//	[Signal]
//	public delegate void BotonPausaPresionado();
	static float timer=(float)0.3;
	
	public override void _Ready()
	{
		
	}

	public static void SetTimer()
	{
		timer=(float)0.3;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(Input.IsActionPressed("Pause") && 0>timer && !GetTree().Paused)
		{
			//GetTree().Quit(); //Cerrar el juego
			_on_BotonPausa_pressed();
		}
		if(0<timer) timer-=delta;
	}
	
	
	private void _on_BotonPausa_pressed()
	{
		Hide();
		GetParent().AddChild(PauseMenu.GetPauseMenu());
	}
	
	private void _on_BotonPausa_mouse_entered()
	{
		Modify.ChangeScale(GetChild<TextureButton>(0), new Vector2((float)0.4, (float)0.4));
	}
	
	private void _on_BotonPausa_mouse_exited()
	{
		Modify.ChangeScale(GetChild<TextureButton>(0), new Vector2((float)0.3, (float)0.3));
	}

}







