using Godot;
using System;

public class Teleporter : Throwable
{
    public override float MaxSize {get=>60; }

    //bool turnChanged=false;
    bool flag=true;

    public override void _PhysicsProcess(float delta)
    {
        if(velocity!=Vector2.Zero) 
        {
            base._PhysicsProcess(delta);
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled=false;
            SetCollisionMaskBit(1,true);
        }
        if(IsOnFloor() && flag)
        {
            flag=false;
            GetNode<CollisionShape2D>("CollisionShape2D2").Disabled=false;
        } 
    }

}
