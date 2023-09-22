using Godot;
using System;

public class ActionCanceller : CanvasLayer
{

    byte tool;
    bool cancelMovement=false;

    public byte Tool
    {
        get=>tool;

        set
        {
            if(value>0 && value<=9)
            {
                tool=value;
            }
        }
    }

    General signalManager;

    public override void _Ready()
    {
        signalManager=GetNode<General>("/root/General");
        signalManager.Connect(nameof(General.OnTurnChanged), this, nameof(OnTurnChanged));
        signalManager.Connect(nameof(General.OnThrowableLaunched), this, nameof(OnThrowableLaunched));
        
    }

    public void Cancel()
    {
        if(cancelMovement)
        {
            CancelMovement();
        }
        else
        {
            CancelTool();
        }
    }


    public void CancelTool()
    {
        Inventory.Unopenable=false;
        
        //restar herramienta
        Inventory.SelectedPlayer.ToolsAvailable[tool]++;

        if(!Inventory.SelectedPlayer.Moved)
        {
            Inventory.SelectedPlayer=null;
        }

       DeleteTools();
       QueueFree();
    }

    public void CancelMovement()
    {
        Inventory.Unopenable=false;
        Inventory.SelectedPlayer.Moved=false;
        Inventory.SelectedPlayer=null;

        DeleteThrower();
        QueueFree();
    }

    private void DeleteThrower()
    {
        var thrower=GetTree().GetNodesInGroup("Thrower");
        foreach(Thrower th in thrower)
        {
            th.QueueFree();
        }

    }

    private void DeleteTools()
    {
        var throwables=GetTree().GetNodesInGroup("Throwable");

        foreach(Throwable throwable in throwables)
        {
            if ((throwable is Platano platano && platano.dropped) ||
                (throwable is Iman iman && iman.launched))
            {
                continue;
            }

            if(throwable is GloboTeledirigido globoTeledirigido)
            {
                signalManager.EmitSignal(nameof(General.OnRemoteBalloonRemoved), globoTeledirigido);
            }

            if(throwable is Teleporter teleporter)
            {
                signalManager.EmitSignal(nameof(General.OnTeleporterRemoved), teleporter);
            }


            throwable.QueueFree();
        }
    }

    private void OnTurnChanged(bool martianTurn)
    {
        QueueFree();
    }

    private void OnThrowableLaunched(Throwable throwable)
    {
        QueueFree();
    }

    
    public override void _Input(InputEvent @event)
    {
        //GD.Print(@event);
        if(@event is InputEventKey inputEventKey && inputEventKey.Scancode==(int)KeyList.Q && inputEventKey.Pressed)
        {
            Cancel();
        }
    }


    public static ActionCanceller GetToolCanceller(byte _tool)
    {
        PackedScene packedScene=GD.Load<PackedScene>("res://scenes/ToolCanceller.tscn");
        ActionCanceller actionCanceller=packedScene.Instance<ActionCanceller>();
        actionCanceller.Tool=_tool;

        return actionCanceller;
    }

    public static ActionCanceller GetMovementCanceller()
    {
        PackedScene packedScene=GD.Load<PackedScene>("res://scenes/ToolCanceller.tscn");
        ActionCanceller actionCanceller=packedScene.Instance<ActionCanceller>();
        actionCanceller.cancelMovement=true;

        return actionCanceller;
    }

}
