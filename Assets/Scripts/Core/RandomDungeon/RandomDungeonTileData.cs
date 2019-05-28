using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMM.RDG
{
    /// <summary>
    /// Class used to represent every tile in a RandomDungeonTileMap.
    /// Contains information about the tile type, scope, room identifier, etc.
    /// </summary>
    public class RandomDungeonTileData
    {
        public char tileType;
        public int scope;
        public int room;
        public int distanceFromPrimaryPath = 0;
        public int trap = -1;
        public int chest = -1;

        public int tileId = 0;
        public Vector2Int position;

        public const char EMPTY_TILE = '0';
        public const char WALKABLE_TILE = '1';
        public const char WALL_TILE = '2';
        public const char EXIT_TILE = '3';

        // These will not exist in the final map; they will be replaced with WALKABLE_TILE
        // and extra information will be tacked on the TileData
        public const char TRAP_TILE = '4';
        public const char POSSIBLE_CHEST_TILE = '5';
        public const char GUARANTEED_CHEST_TILE = '6';

        public RandomDungeonTileData()
        {

        }

        public RandomDungeonTileData(RandomDungeonTileData copy)
        {
            tileType = copy.tileType;
            scope = copy.scope;
            room = copy.room;
            trap = copy.trap;
            chest = copy.chest;
            distanceFromPrimaryPath = copy.distanceFromPrimaryPath;
        }

        public void Clear()
        {
            distanceFromPrimaryPath = 0;
            trap = -1;
            chest = -1;
        }
    }
}