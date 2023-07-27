using Godot;
using System;

public class Lanzaglobos : ProjectileLauncher
{
    Sprite sprite;
    HSlider hSlider;
    Label label;
    float offset=23.5f;
    const byte maxBaloons=3;
    public byte BalloonsExploded {get; set;}=0;

    byte balloonsLaunched=0;
    bool balloonExploded=false;

    protected override Vector2 StartingPoint { get => Vector2.Zero; }

    Escenario escenario;

    

    public override void _Ready()
    {
        base._Ready();
        EventManager.OnBalloonExploded+=OnBalloonExploded;
        escenario=GetTree().Root.GetNode<Escenario>("Escenario");


        sprite=GetNode<Sprite>("Sprite");   
        hSlider=GetNode<HSlider>("HSlider");
        label=hSlider.GetNode<Label>("Label");

        lineWidth=49;
        line.Width=lineWidth;


        speed=500;
    }


    private void OnBalloonExploded(GloboConAgua balloon)
    {
        BalloonsExploded++;
        if(BalloonsExploded==3)
        {
            QueueFree();
            balloon.LanzaglobosTerminado=true;
            GetTree().CallGroup("Escenarios", "ChangeTurn");
        }
    }

    private void LaunchBalloon()
    {
        GloboConAgua globoConAgua=GloboConAgua.GetWaterBalloon();

        globoConAgua.SetVelocity(initialVelocity);
        globoConAgua.Position=GlobalPosition;

        escenario.AddChild(globoConAgua);
    }

    protected override void CalculateInitialVelocity()
    {
        float degAngleRadians=Mathf.Deg2Rad(degAngle);
        direction=new Vector2(Mathf.Cos(degAngleRadians), Mathf.Sin(degAngleRadians));
        direction=direction.Normalized();

        initialVelocity=direction*speed;
    }
    /* 
        private Vector2 UpdateTrajectory(float delta)
        {
            line.ClearPoints();
            line.AddPoint(Vector2.Zero);


            Vector2 velocity=initialVelocity;
            Vector2 newPos = Vector2.Zero;  //starting point


            for (int i = 0; i < 300; i++)
            {
                line.AddPoint(newPos);
                velocity.y += Globals.Gravity * delta;
                newPos += velocity * delta;

                float lineAngle = i > 0 ? line.GetPointPosition(i - 1).DirectionTo(line.GetPointPosition(i)).Angle() : degAngle;

                if (IsColliding(ToGlobal(newPos), lineAngle)) break;
            }

            return newPos;
        }
     */

    protected override void RestartLaunch()
    {
        restartSound.Play();
    }

    private void _on_HSlider_value_changed(float value)
    {
        degAngle=-value;
        sprite.RotationDegrees=degAngle+offset;
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
                if(!CorrectAngle() || collidingBodies.Count>0)
                {
                    RestartLaunch();
                    return;
                }

                if(balloonsLaunched<3)
                {
                    LaunchBalloon();
                    balloonsLaunched++;
                }
            }
        }
    }

}
