using Godot;
using System;

public class Escenario3 : Escenario
{
    TileMap tileMap;
    const int edgeBreakablePlatformIndex=4;
    const int breakablePlatformIndex=3;

    private const int PlatformSize1 = 24;
    private const int PlatformX1 = 18;
    private const int PlatformY1 = -5;

    private const int PlatformSize2 = 15;
    private const int PlatformX2 = 23;
    private const int PlatformY2 = 3;

    private const int PlatformSize3 = 11;
    private const int PlatformX3 = 25;
    private const int PlatformY3 = 14;

    int[] turnsToBreak=new int[3];

    public override void _Ready()
    {
        leftLimit=-1400f;
        rightLimit=3000f;
        topLimit=-1200f;
        bottomLimit=1200f;
        astronautsCameraPosition=new Vector2(-617, -451);
        martiansCameraPosition=new Vector2(2237, -464);
        base._Ready();
        Globals.Gravity=(int)Constants.Gravities.MoonGravity;

        tileMap=GetNode<TileMap>("TileMap");


        Godot.Collections.Array breakablePlatforms=GetNode("BreakablePlatforms").GetChildren();
        for(int i=0;i<breakablePlatforms.Count;i++)
        {
            Area2D breakablePlatform=(Area2D)breakablePlatforms[i];
            breakablePlatform.Connect("body_entered", this, nameof(PlatformToBreak), new Godot.Collections.Array{i});
        }

    }

    private new void ChangeTurn()
    {
        base.ChangeTurn();

        for(int i=0;i<turnsToBreak.Length;i++)
        {
            if(turnsToBreak[i]>1)  //when it is going to break;
            {
                turnsToBreak[i]--;
            }

            if(turnsToBreak[i]<0) //when it's gonna build
            {
                BuildPlatform(i+1);
                turnsToBreak[i]=0; //when it's normal
            }

            if(turnsToBreak[i]==1) //when it breaks now
            {
                BreakPlatform(i+1);
                turnsToBreak[i]=-1;
            }
        }
    }

    public void PlatformToBreak(Node body, int platform)
    {
        if(body is TileMap || turnsToBreak[platform]!=0)
        {
            return;
        }

        turnsToBreak[platform]=3;
    }
    public void BreakPlatform(int platform)
    {
        int x=0;
        int y=0;

        switch(platform)
        {
            case 1:
                x=PlatformX1;
                y=PlatformY1;
            break;

            case 2:
                x=PlatformX2;
                y=PlatformY2;
            break;

            case 3:
                x=PlatformX3;
                y=PlatformY3;
            break;
        }

        int tileIndex=tileMap.GetCell(x, y);

        while(tileIndex!=-1)
        {
            tileMap.SetCell(x, y, -1); //-1 es un tile vacío
            x++;
            tileIndex=tileMap.GetCell(x, y);
        }

    }

    private void SetPlatformCells(int startX, int y, int size, int edgeTileIndex, int middleTileIndex)
    {
        tileMap.SetCell(startX, y, edgeTileIndex);
        for (int i = 1; i < size; i++)
        {
            tileMap.SetCell(startX + i, y, middleTileIndex);
        }
        tileMap.SetCell(startX + size, y, edgeTileIndex, true);
    }

    public void BuildPlatform(int platform)
    {
        switch(platform)
        {
            case 1:
                SetPlatformCells(PlatformX1, PlatformY1, PlatformSize1, edgeBreakablePlatformIndex, breakablePlatformIndex);
                break;
            case 2:
                SetPlatformCells(PlatformX2, PlatformY2, PlatformSize2, edgeBreakablePlatformIndex, breakablePlatformIndex);
                break;
            case 3:
                SetPlatformCells(PlatformX3, PlatformY3, PlatformSize3, edgeBreakablePlatformIndex, breakablePlatformIndex);
                break;
        }
    }


}
