using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


public partial class Escenario : Node2D
{
    public void SetCamera(GloboTeledirigido globoTeledirigido)
	{
		RemoveChild(camera);
		camera.Position=Vector2.Zero;
		globoTeledirigido.AddChild(camera);
	}

	private void OnRemoteBalloonRemoved(GloboTeledirigido globoTeledirigido)
	{
		Vector2 prevPos=globoTeledirigido.GlobalPosition;
		globoTeledirigido.RemoveChild(camera);
		camera.GlobalPosition=prevPos;
		AddChild(camera);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventMouseButton evento)
		{
			if (evento.Pressed && evento.ButtonIndex==(int)ButtonList.WheelDown) 
			{
				UnZoom();
			}
			if (evento.Pressed && evento.ButtonIndex==(int)ButtonList.WheelUp)
			{
				Zoom();		
			}	
			
			if(evento.ButtonIndex==2)
			{
				if(evento.Pressed)
				{
					rightClick=true;
				}
				else
				{
					rightClick=false;
				}
			}
		}
		
		if(@event is InputEventMouseMotion Movimiento)
		{
			if (rightClick)
			{
				camera.SmoothingEnabled = false;
				Vector2 newPosition = camera.Position - Movimiento.Relative * camera.Zoom;
				float leftLimitZoom=leftLimit+( (cameraSize.x*camera.Zoom.x)/2 );
				float rightLimitZoom=rightLimit-( (cameraSize.x*camera.Zoom.x)/2 );
				float topLimitZoom=topLimit+( (cameraSize.y*camera.Zoom.y)/2 );
				float bottomLimitZoom=bottomLimit-( (cameraSize.y*camera.Zoom.y)/2 );

				// Verificar límites de la cámara
				newPosition.x = Mathf.Clamp(newPosition.x, leftLimitZoom, rightLimitZoom);
				newPosition.y = Mathf.Clamp(newPosition.y, topLimitZoom, bottomLimitZoom);

				camera.Position=newPosition;
			}
			else
			{
				camera.SmoothingEnabled = true;
			}
		}
		
	}
	
	protected void _on_Zoom_pressed()
	{
		Zoom();
	}


	protected void _on_UnZoom_pressed()
	{
		UnZoom();
	}

	void Zoom()
	{
		if(camera.Zoom.x>maxZoom)
		{
			float newZoom=(float)Math.Round(camera.Zoom.x-zoom,1);
			camera.Zoom=new Vector2(newZoom, newZoom); 	
		}
	}

	void UnZoom()
	{
		if(camera.Zoom.x<minZoom)
		{
			float newZoom=(float)Math.Round(camera.Zoom.x+zoom,1);
			camera.Zoom=new Vector2(newZoom, newZoom); 	
			return;
		}

		if(camera.Zoom.x==minZoom)
		{
			camera.Zoom=new Vector2(realMinZoom, realMinZoom); 	
		}
	}
}