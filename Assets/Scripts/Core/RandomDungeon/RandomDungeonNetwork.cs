using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMM.RDG
{
    /// <summary>
    /// Evaluates a random dungeon and determines the networking of rooms which can be useful
    /// for after-the-fact addition of details (ie: adding things to dead ends).
    /// It's not a particularly fast algorithm atm...
    /// </summary>
    public class RandomDungeonNetwork
    {
        public class RandomDungeonNetworkNode
        {
            private List<int> mConnections = new List<int>();
            private List<Vector2Int> mEmptyPositions = new List<Vector2Int>();

            public int roomId { get; set; }
            public int distanceFromPrimaryPath { get; set; }
            public List<int> connections {  get { return mConnections; } }
            public List<Vector2Int> emptyPositions {  get { return mEmptyPositions; } }

            public bool ConnectionExists(int roomId)
            {
                return mConnections.Contains(roomId);
            }

            public void AddConnection(int roomId)
            {
                mConnections.Add(roomId);
            }

            public void RegisterEmptyPosition(int x, int y)
            {
                mEmptyPositions.Add(new Vector2Int(x, y));
            }
        }

        private Dictionary<int, RandomDungeonNetworkNode> mNodeMap = new Dictionary<int, RandomDungeonNetworkNode>();
        
        public List<RandomDungeonNetworkNode> DeadEnds()
        {
            List<RandomDungeonNetworkNode> deadEnds = new List<RandomDungeonNetworkNode>();
            foreach(KeyValuePair<int, RandomDungeonNetworkNode> entry in mNodeMap)
            {
                if (entry.Value.connections.Count == 1)
                {
                    deadEnds.Add(entry.Value);
                }
            }

            return deadEnds;
        }

        public void EvaluateDungeon(RandomDungeon dungeon)
        {
            int[] orthogonalOffsets = new int[] { 0, 1, 1, 0, 0, -1, -1, 0 };

            for (int x = 0; x < dungeon.width; ++x)
            {
                for (int y = 0; y < dungeon.height; ++y)
                {
                    // Only evaluate walkable tiles
                    if (dungeon.TileType(x, y) != RandomDungeonTileData.WALKABLE_TILE && dungeon.TileType(x, y) != RandomDungeonTileData.EXIT_TILE)
                        continue;

                    RandomDungeonTileData data = dungeon.Data(x, y);

                    // If the room isn't already part of the set, add it
                    RandomDungeonNetworkNode node = null;
                    if (!mNodeMap.TryGetValue(data.room, out node))
                    {
                        node = new RandomDungeonNetworkNode();
                        node.roomId = data.room;
                        node.distanceFromPrimaryPath = data.distanceFromPrimaryPath;

                        mNodeMap.Add(data.room, node);
                    }

                    // Distance from primary path should be the closest tile that we can get into for this room.
                    node.distanceFromPrimaryPath = Mathf.Min(data.distanceFromPrimaryPath, node.distanceFromPrimaryPath);

                    // Evaluate connections (orthogonally, only considering walkable / exit tiles)
                    for (int i = 0; i < orthogonalOffsets.Length; i += 2)
                    {
                        int neighborX = x + orthogonalOffsets[i];
                        int neighborY = y + orthogonalOffsets[i + 1];

                        if (dungeon.IsPositionInBounds(neighborX, neighborY))
                        {
                            RandomDungeonTileData neighborData = dungeon.Data(neighborX, neighborY);
                            bool isWalkable = (neighborData.tileType == RandomDungeonTileData.WALKABLE_TILE || neighborData.tileType == RandomDungeonTileData.EXIT_TILE);

                            if (isWalkable && neighborData.room != node.roomId && !node.ConnectionExists(neighborData.room))
                            {
                                node.AddConnection(neighborData.room);
                            }
                        }
                    }

                    // Keep track of all walkable tiles in this room so we know where we can spawn stuff.
                    // todo bdsowers - change the name of this
                    // By design it only keeps track of tiles that aren't next to a wall, but that
                    // isn't conveyed in the name.
                    // todo bdsowers - move a 'num surrounding walls' into map generation, as that can
                    // be useful for more than just this.
                    bool hasUnwalkableNeighbor = false;
                    for (int offsetX = -1; offsetX <= 1; ++offsetX)
                    {
                        for (int offsetY = -1; offsetY <= 1; ++offsetY)
                        {
                            int testX = x + offsetX;
                            int testY = y + offsetY;
                            if (!dungeon.IsPositionInBounds(testX, testY))
                            {
                                hasUnwalkableNeighbor = true;
                                continue;
                            }

                            char tileType = dungeon.TileType(testX, testY);
                            if (tileType != RandomDungeonTileData.WALKABLE_TILE &&
                                tileType != RandomDungeonTileData.EXIT_TILE)
                            {
                                hasUnwalkableNeighbor = true;
                                continue;
                            }
                        }
                    }

                    if (!hasUnwalkableNeighbor)
                    {
                        node.RegisterEmptyPosition(x, y);
                    }
                }
            }
        }
    }
}
