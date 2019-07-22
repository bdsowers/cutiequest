using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;

public class CollisionMap : MonoBehaviour
{
    int[,] mMap;

    private Dictionary<int, Vector2Int> mPlacement = new Dictionary<int, Vector2Int>();

    public int width
    {
        get
        {
            return mMap.GetLength(0);
        }
    }

    public int height
    {
        get
        {
            return mMap.GetLength(1);
        }
    }

    public void SetupWithSize(int width, int height)
    {
        mMap = new int[width, height];
    }

    public void SetupWithDungeon(RandomDungeon dungeon)
    {
        mMap = new int[dungeon.width, dungeon.height];

        for (int x = 0; x < dungeon.width; ++x)
        {
            for (int y = 0; y < dungeon.height; ++y)
            {
                if (dungeon.TileType(x,y) == RandomDungeonTileData.WALL_TILE)
                {
                    mMap[x, y] = 1;
                }
                else if (dungeon.TileType(x,y) == RandomDungeonTileData.EMPTY_TILE)
                {
                    mMap[x, y] = -1;
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
        if (mMap[x, y] != 0)
            Debug.LogError("Trying to mark a space that's already marked");

        mMap[x, y] = value;

        if (mPlacement.ContainsKey(value))
        {
            mPlacement[value] = new Vector2Int(x, y);
        }
        else
        {
            mPlacement.Add(value, new Vector2Int(x, y));
        }
    }

    public void RemoveMarking(int value)
    {
        Vector2Int position;
        if (mPlacement.TryGetValue(value, out position))
        {
            if (mMap[position.x, position.y] != value)
            {
                Debug.LogError("Collision map has gotten out of whack");
            }

            mMap[position.x, position.y] = 0;
            mPlacement.Remove(value);
        }
        else
        {
            Debug.LogError("Trying to remove a marking that doesn't exist in the map: " + value);
        }
    }

    public int SpaceMarking(int x, int y)
    {
        return mMap[x, y];
    }

    public List<Vector2Int> EmptyPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < mMap.GetLength(0); ++x)
        {
            for (int y = 0; y < mMap.GetLength(1); ++y)
            {
                if (mMap[x,y] == 0)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }
        return positions;
    }

    public List<Vector2Int> EmptyPositionsNearPosition(Vector2Int sourcePosition, int range)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int xOffset = -range; xOffset <= range; ++xOffset)
        {
            for (int yOffset = -range; yOffset <= range; ++yOffset)
            {
                int x = sourcePosition.x + xOffset;
                int y = sourcePosition.y + yOffset;

                if (x >= 0 && y >= 0 && x < width && y < height && mMap[x,y] == 0)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }

        return positions;
    }

    public List<Vector2Int> EmptyOffsetsNearPosition(Vector2Int sourcePosition, int range)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int xOffset = -range; xOffset <= range; ++xOffset)
        {
            for (int yOffset = -range; yOffset <= range; ++yOffset)
            {
                int x = sourcePosition.x + xOffset;
                int y = sourcePosition.y + yOffset;

                if (x >= 0 && y >= 0 && x < width && y < height && mMap[x, y] == 0)
                {
                    positions.Add(new Vector2Int(xOffset, yOffset));
                }
            }
        }

        return positions;
    }
}
