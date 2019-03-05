using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;

public class CollisionMap : MonoBehaviour
{
    int[,] mMap;

    public void SetupWithDungeon(RandomDungeon dungeon)
    {
        mMap = new int[dungeon.width, dungeon.height];

        for (int x = 0; x < dungeon.width; ++x)
        {
            for (int y = 0; y < dungeon.height; ++y)
            {
                if (dungeon.TileType(x,y) == RandomDungeonTileData.WALL_TILE ||
                    dungeon.TileType(x,y) == RandomDungeonTileData.EMPTY_TILE)
                {
                    mMap[x, y] = 1;
                }
                else
                {
                    mMap[x, y] = 0;
                }
            }
        }
    }

    public void MarkSpace(int x, int y, int value)
    {
        mMap[x, y] = value;
    }

    public int SpaceMarking(int x, int y)
    {
        return mMap[x, y];
    }
}
