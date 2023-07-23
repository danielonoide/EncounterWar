using Godot;
using System;

public class Lanzaglobos : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    Sprite sprite;
    HSlider hSlider;
    Label label;

    Line2D line;

    Area2D collisionArea;
    float angle=0;
    float offset=23.5f;

    const float speed=500;

    const byte maxBaloons=3;
    public byte BalloonsExploded {get; set;}=0;

    byte balloonsLaunched=0;

    RectangleShape2D collisionShape;
    
    float lineWidth=49;

    bool selected=false;

    bool balloonExploded=false;

    Vector2 initialVelocity=Vector2.Zero;

    public override void _Ready()
    {
        EventManager.OnBalloonExploded+=OnBalloonExploded;

        sprite=GetNode<Sprite>("Sprite");   
        hSlider=GetNode<HSlider>("HSlider");
        label=hSlider.GetNode<Label>("Label");
        line=GetNode<Line2D>("Line2D");
        collisionArea=GetNode<Area2D>("Colisionador");


        line.Width=49;

        collisionShape = new RectangleShape2D();
        collisionShape.Extents = new Vector2(1, lineWidth / 2);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!selected) return;

        RemoveCollisions();


        CalculateInitialVelocity();
        // Lógica de la trayectoria
        Vector2 finalPos=UpdateTrajectory(delta);

        // Lógica de la colisión
        int points = line.GetPointCount();
        int j = 1;
        while (j < points - 10)
        {
			var longitud=line.GetPointPosition(j).DistanceTo(line.GetPointPosition(j+10));
            CreateCollisionShape((line.GetPointPosition(j) + line.GetPointPosition(j + 10)) / 2, new Vector2(longitud/2, lineWidth/2) ,line.GetPointPosition(j).DirectionTo(line.GetPointPosition(j + 10)).Angle());
            j += 10;
        }

        int difference = points - (points - j);
		var Longitud=line.GetPointPosition(difference).DistanceTo(finalPos);
        CreateCollisionShape((line.GetPointPosition(difference) + finalPos) / 2, new Vector2(Longitud/2, lineWidth/2) ,line.GetPointPosition(difference).DirectionTo(finalPos).Angle());

    }

    private void OnBalloonExploded(GloboConAgua balloon)
    {
        BalloonsExploded++;
        if(BalloonsExploded==3)
        {
            QueueFree();
            GetTree().CallGroup("Escenarios", "ChangeTurn");
        }
    }

    private void RemoveCollisions()
    {
        foreach (CollisionShape2D collision in collisionArea.GetChildren())
        {
            collision.QueueFree();
        }
    }

    private void LaunchBalloon()
    {
        Escenario escenario=GetTree().Root.GetNode<Escenario>("Escenario");
        GloboConAgua globoConAgua=GloboConAgua.GetWaterBalloon();

        globoConAgua.SetVelocity(initialVelocity);
        globoConAgua.Position=GlobalPosition;

        escenario.AddChild(globoConAgua);
    }

    void CalculateInitialVelocity()
    {

        float angleRadians=Mathf.Deg2Rad(angle);
        Vector2 direction=new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
        direction=direction.Normalized();

        initialVelocity=direction*speed;
    }

    private Vector2 UpdateTrajectory(float delta)
    {
        line.ClearPoints();
        line.AddPoint(Vector2.Zero);


        Vector2 velocity=initialVelocity;
        Vector2 newPos = initialVelocity/12;


        GD.Print("NEw pos: "+newPos);
        GD.Print("initial Velocity: "+velocity);

        for (int i = 0; i < 300; i++)
        {
            line.AddPoint(newPos);
            velocity.y += Globals.Gravity * delta;
            newPos += velocity * delta;

            float lineAngle = i > 0 ? line.GetPointPosition(i - 1).DirectionTo(line.GetPointPosition(i)).Angle() : angle;

            if (IsColliding(ToGlobal(newPos), lineAngle)) break;
        }

		return newPos;
    }

    private void CreateCollisionShape(Vector2 position, Vector2 extents, float angle)
    {
        CollisionShape2D collision = new CollisionShape2D();
        collision.Position = position;
        collision.Rotation = angle;
		RectangleShape2D rectangleShape2D=new();
		rectangleShape2D.Extents=extents;
        collision.Shape = rectangleShape2D;
		
        collisionArea.AddChild(collision);
    }


    private bool IsColliding(Vector2 position, float angle)
    {
        Physics2DShapeQueryParameters queryParameters = new Physics2DShapeQueryParameters();
        queryParameters.Transform = new Transform2D(angle, position);
        queryParameters.SetShape(collisionShape);
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;

        var queryResult = spaceState.IntersectShape(queryParameters);

        foreach (Godot.Collections.Dictionary result in queryResult)
        {
            if (result["collider"] is not KinematicBody2D)
            {
                return true;
            }
        }

        return false;
    }


    private void _on_HSlider_value_changed(float value)
    {
        angle=-value;
        sprite.RotationDegrees=angle+offset;
        label.Text=value.ToString();
    }

    private void _on_HSlider_gui_input(InputEvent @event)
    {
        if(@event is InputEventMouseButton mouseButton)
        {
            if(mouseButton.Pressed)
            {
                selected=true;
            }
            else 
            {
                selected=false;
                if(balloonsLaunched<3)
                {
                    LaunchBalloon();
                    balloonsLaunched++;
                }
            }
        }
    }

}
