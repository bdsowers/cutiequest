using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMM.RDG
{
    public class RandomDungeon
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public List<Vector2Int> primaryPathPositions { get; set; }

        private RandomDungeonTileData[,] mTiles;

        private static Vector2Int[] mOrthogonalAdjacent = new Vector2Int[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
        };

        public RandomDungeon(int dungeonWidth, int dungeonHeight)
        {
            width = dungeonWidth;
            height = dungeonHeight;
            mTiles = new RandomDungeonTileData[width, height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    mTiles[x, y] = new RandomDungeonTileData();
                    mTiles[x, y].tileType = RandomDungeonTileData.EMPTY_TILE;
                    mTiles[x, y].scope = 0;
                }
            }
        }

        public char TileType(int x, int y)
        {
            return mTiles[x, y].tileType;
        }

        public char TileType(Vector2Int pos)
        {
            return mTiles[pos.x, pos.y].tileType;
        }

        public bool IsPositionInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
        }

        public int Scope(int x, int y)
        {
            return mTiles[x, y].scope;
        }

        public int Scope(Vector2Int pos)
        {
            return mTiles[pos.x, pos.y].scope;
        }

        public RandomDungeonTileData Data(int x, int y)
        {
            return mTiles[x, y];
        }

        public void SetData(int x, int y, RandomDungeonTileData data)
        {
            mTiles[x, y] = data;
        }

        public void ChangeTileType(int x, int y, char newTileType)
        {
            mTiles[x, y].tileType = newTileType;
        }

        public bool TilePositionInBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public int NumWalkableNeighbors(int x, int y)
        {
            int walkableNeighbors = 0;
            for (int i = 0; i < mOrthogonalAdjacent.Length; ++i)
            {
                int mapX = x + mOrthogonalAdjacent[i].x;
                int mapY = y + mOrthogonalAdjacent[i].y;

                if (mapX >= 0 && mapY >= 0 &&
                    mapX < width && mapY < height &&
                    (TileType(mapX, mapY) == RandomDungeonTileData.WALKABLE_TILE || TileType(mapX, mapY) == RandomDungeonTileData.EXIT_TILE))
                {
                    walkableNeighbors++;
                }
            }

            return walkableNeighbors;
        }

        /// <summary>
        /// Given a scope, returns all the tiles that separate this scope and another one.
        /// The given tiles are those belonging to scope1
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public List<Vector2Int> WalkableTilePositionsSeparatingScope(int scope1, int scope2)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if ((mTiles[x, y].tileType == RandomDungeonTileData.WALKABLE_TILE || mTiles[x, y].tileType == RandomDungeonTileData.EXIT_TILE) &&
                        mTiles[x, y].scope == scope1 &&
                        TileHasWalkableNeighborsOfScope(x, y, scope2))
                    {
                        positions.Add(new Vector2Int(x, y));
                    }
                }
            }

            return positions;
        }

        private bool TileHasWalkableNeighborsOfScope(int x, int y, int scope)
        {
            for (int i = 0; i < mOrthogonalAdjacent.Length; ++i)
            {
                int adjustedX = x + mOrthogonalAdjacent[i].x;
                int adjustedY = y + mOrthogonalAdjacent[i].y;
                if (TilePositionInBounds(adjustedX, adjustedY) &&
                    mTiles[adjustedX, adjustedY].scope == scope &&
                    (mTiles[adjustedX, adjustedY].tileType == RandomDungeonTileData.WALKABLE_TILE || mTiles[adjustedX, adjustedY].tileType == RandomDungeonTileData.EXIT_TILE))
                {
                    return true;
                }
            }

            return false;
        }

        public List<Vector2Int> WalkableTilePositions()
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if (TileType(x, y) == RandomDungeonTileData.WALKABLE_TILE)
                    {
                        positions.Add(new Vector2Int(x, y));
                    }
                }
            }
            return positions;
        }

        public List<Vector2Int> WalkableNeighbors(int x, int y, int range)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            for (int xOffset = -range; xOffset <= range; ++xOffset)
            {
                for (int yOffset = -range; yOffset <= range; ++yOffset)
                {
                    int adjustedX = x + xOffset;
                    int adjustedY = y + yOffset;
                    if (TilePositionInBounds(adjustedX, adjustedY) && mTiles[adjustedX, adjustedY].tileType == RandomDungeonTileData.WALKABLE_TILE)
                    {
                        neighbors.Add(new Vector2Int(adjustedX, adjustedY));
                    }
                }
            }
            return neighbors;
        }

        public Vector2Int PositionForSpecificTile(char tileType)
        {
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if (mTiles[x,y].tileType == tileType)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return new Vector2Int(-1, -1);
        }
    }
}
