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

    public override void _Ready()
    {
        leftLimit=-1400f;
        rightLimit=3000f;
        topLimit=-1200f;
        bottomLimit=1200f;
        base._Ready();
        Globals.Gravity=(int)Constants.Gravities.MoonGravity;

        tileMap=GetNode<TileMap>("TileMap");


        Godot.Collections.Array breakablePlatforms=GetNode("BreakablePlatforms").GetChildren();
        foreach(Area2D platform in breakablePlatforms)
        {
            platform.Connect("body_entered", this, nameof(BreakPlatform), new Godot.Collections.Array{platform.Position});
        }

    }

    public void BreakPlatform(Node body, Vector2 startPosition)
    {
        if(body is TileMap) return;

        Vector2 position=tileMap.WorldToMap(startPosition);
        int x=(int)position.x;
        int y=(int)position.y;
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
