using Godot;
using System;
using static Constants;

public class MatchEnding : CanvasLayer
{
    Label winnerTeam;

    WinningTeam winner=0;


    public override void _Ready()
    {
        winnerTeam=GetNode<Label>("CenterContainer/Winner");

        if(winner==WinningTeam.Draw)
        {
            winnerTeam.Text="Empate";
        }
        else winnerTeam.Text=winner==WinningTeam.Astronauts ?  "Han ganado los astronautas" : "Han ganado los marcianos"; 

        
    }
    private void _on_RestartBTN_pressed()
    {
        GetTree().ReloadCurrentScene();
    }

    private void _on_MenuBTN_pressed()
    {
        GetTree().ChangeScene(Constants.MainMenuPath);
    }

    public static MatchEnding GetMatchEnding(WinningTeam winner)
	{
		PackedScene scene=(PackedScene)ResourceLoader.Load("res://scenes/UI/MatchEnding.tscn");
        MatchEnding matchEnding=(MatchEnding)scene.Instance();
        matchEnding.winner=winner;

		return matchEnding;
	}

}
