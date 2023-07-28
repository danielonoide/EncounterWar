using Godot;
using System;

public class MatchEnding : CanvasLayer
{
    Label winnerTeam;

    byte winner=0;


    public override void _Ready()
    {
        winnerTeam=GetNode<Label>("CenterContainer/Winner");

        if(winner==0)
        {
            winnerTeam.Text="Empate";
        }
        else winnerTeam.Text=winner==1 ?  "Han ganado los astronautas" : "Han ganado los marcianos"; 

        
    }
    private void _on_RestartBTN_pressed()
    {
        GetTree().ReloadCurrentScene();
    }

    private void _on_MenuBTN_pressed()
    {
        GetTree().ChangeScene(Constants.MainMenuPath);
    }

    public static MatchEnding GetMatchEnding(byte winner)
	{
		PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/UI/MatchEnding.tscn");
        MatchEnding matchEnding=(MatchEnding)scene.Instance();
        matchEnding.winner=winner;

		return matchEnding;
	}

}
