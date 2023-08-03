using Godot;
using System;

public class Escenario2 : Escenario
{
    Godot.Collections.Array movingPlatforms;

    public override void _Ready()
    {
        leftLimit=-2000f;
        rightLimit=2000f;
        topLimit=-2500f;
        bottomLimit=700f;
        movingPlatforms=GetTree().GetNodesInGroup("MovingPlatforms");
        astronautsCameraPosition=new Vector2(-941, -58);
        martiansCameraPosition=new Vector2(941, -58);
        base._Ready();
        Globals.Gravity=(int)Constants.Gravities.SpaceGravity;
    }

    public override void SaveGame()
    {
        Godot.Collections.Dictionary<string,object> saveData=SaveData();
        Godot.Collections.Array movingPlatformsData=new();
        foreach(MovingPlatform movingPlatform in movingPlatforms)
        {
            Godot.Collections.Dictionary<string, object> movingPlatformData = new() 
            {
                { "Position", movingPlatform.Position },
                {"direction", movingPlatform.direction}
            };

            movingPlatformsData.Add(movingPlatformData);
        }

        saveData.Add("MovingPlatforms", movingPlatformsData);

        SaveDictionary(saveData);
    }

    public override void LoadGame()
    {
        base.LoadGame();

        Godot.Collections.Dictionary<string,object> saveData=LoadDictionary();

        //cargar plataformas movedizas

		Godot.Collections.Array movingPlatformsData = (Godot.Collections.Array)saveData["MovingPlatforms"];


        for(int i=0;i<movingPlatforms.Count;i++)
        {
            var movingPlatformData=(Godot.Collections.Dictionary)movingPlatformsData[i];
            var movingPlatform=(MovingPlatform)movingPlatforms[i];
            movingPlatform.Position=StringToVector2((string)movingPlatformData["Position"]);
            movingPlatform.direction=Convert.ToSByte(movingPlatformData["direction"]);
        }
        
    }

}
