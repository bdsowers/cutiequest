using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMM.RDG
{
    [System.Serializable]
    public struct RandomDungeonScopeData
    {
        public int criticalPathMinRooms;
        public int criticalPathMaxRooms;

        public bool forbidExtraRooms;
        public bool forbidSpecialRooms;
    }

    [System.Serializable]
    public struct RandomDungeonGenerationData
    {
        public int randomSeed;
        public RandomDungeonScopeData[] scopeData;
        public int minSpecialRooms;
        public int maxSpecialRooms;
        public int numExtraRoomsPlacedBeforeSpecialRooms;
        public int numExtraRoomsPlacedAfterSpecialRooms;

        public bool enforceTwoTileHighWalls;
        public bool fillEmptyCellsWithWalls;
    }

    /// <summary>
    /// Class to generate a random dungeon based on RandomDungeonGenerationData.
    /// GenerateDungeon does all the heavy lifting and is the only thing calling code need use.
    /// </summary>
    public class RandomDungeonGenerator
    {
        [Flags]
        private enum RoomPlacementOptions
        {
            None = 0,
            AllowWallsToBecomeExits = 1,
            AllowMismatchScopes = 2,
        }

        private bool RoomPlacementOptionSet(RoomPlacementOptions options, RoomPlacementOptions testOption)
        {
            return (options & testOption) == testOption;
        }

        /// <summary>
        /// List all the offfsets to find orthogonally adjacent tiles.
        /// Saves us some code/performance in a few places.
        /// </summary>
        private static Vector2Int[] mOrthogonalAdjacent = new Vector2Int[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
        };

        /// <summary>
        /// So we don't need to have a big nebulous array and worry about bounds checks, we'll
        /// construct our map in a dictionary and support any positions, including negatives.
        /// When we're finished, we'll push that off to an appropriately sized array.
        /// </summary>
        private Dictionary<Vector2Int, RandomDungeonTileData> mRoomScratch = new Dictionary<Vector2Int, RandomDungeonTileData>();
        private List<Vector2Int> mAvailableExits = new List<Vector2Int>();
        private RoomSet mRoomSet;
        
        private List<Vector2Int> mPrimaryPathPositions = new List<Vector2Int>();

        private RoomData mPreviousRoomPlaced;
        private Vector2Int mPreviousMapExitUsed;

        private System.Random mRNG;

        private int mCurrentRoomId = 1;
        private int mCurrentTrapId = 1;
        private int mCurrentTileId = 1;

        private bool mMapInvalid = true;
        private bool mGenerationRequiredRetry = false;
        
        public int maxDistanceFromPrimaryPath { get; private set; }

        public bool generationRequiredRetry
        {
            get { return mGenerationRequiredRetry; }
        }

        public List<RandomDungeonTileData> allTiles
        {
            get { return new List<RandomDungeonTileData>(mRoomScratch.Values); }
        }

        public bool stopCloseExitsToNowhere { get; set; }

        public RandomDungeon GenerateDungeon(RoomSet roomSet, RandomDungeonGenerationData generationData)
        {
            mRNG = new System.Random(generationData.randomSeed);

            mRoomSet = roomSet;

            while (mMapInvalid)
            {
                PrepareNewMapGeneration();

                ConstructPrimaryPath(generationData.scopeData);
                
                CollectAvailableExits(ForbiddenScopesForExtraRooms(generationData.scopeData));
                AddExtraRooms(generationData.numExtraRoomsPlacedBeforeSpecialRooms);

                CollectAvailableExits(ForbiddenScopesForSpecialRooms(generationData.scopeData));
                AddSpecialRooms(generationData.minSpecialRooms, generationData.maxSpecialRooms);

                CollectAvailableExits(ForbiddenScopesForExtraRooms(generationData.scopeData));
                AddExtraRooms(generationData.numExtraRoomsPlacedAfterSpecialRooms);

                if (mMapInvalid)
                {
                    mGenerationRequiredRetry = true;
                    mCurrentTileId = 1;
                }
            }

            return GenerateRandomDungeonFromCurrentProgress(generationData);
        }

        /// <summary>
        /// Given the current progress of map generation, generates a random dungeon.
        /// This is only exposed as public for demonstration purposes when we want to show the
        /// dungeon being generated dynamically.
        /// </summary>
        /// <param name="generationData"></param>
        /// <returns></returns>
        public RandomDungeon GenerateRandomDungeonFromCurrentProgress(RandomDungeonGenerationData generationData, Dictionary<Vector2Int, RandomDungeonTileData> scratch = null)
        {
            RandomDungeon dungeon = GenerateDungeonObjectFromScratchPad(scratch != null ? scratch : mRoomScratch);
            CloseExitsToNowhere(dungeon);

            if (generationData.enforceTwoTileHighWalls)
            {
                EnforceWallHeight(dungeon);
            }

            if (generationData.fillEmptyCellsWithWalls)
            {
                FillEmptyCellsWithWalls(dungeon);
            }

            return dungeon;
        }

        private List<int> ForbiddenScopesForExtraRooms(RandomDungeonScopeData[] scopes)
        {
            List<int> forbiddenScopes = new List<int>();
            for (int i = 0; i < scopes.Length; ++i)
            {
                if ( scopes[i].forbidExtraRooms)
                {
                    forbiddenScopes.Add(i);
                }
            }
            return forbiddenScopes;
        }

        private List<int> ForbiddenScopesForSpecialRooms(RandomDungeonScopeData[] scopes)
        {
            List<int> forbiddenScopes = new List<int>();
            for (int i = 0; i < scopes.Length; ++i)
            {
                if (scopes[i].forbidSpecialRooms)
                {
                    forbiddenScopes.Add(i);
                }
            }
            return forbiddenScopes;
        }

        private void FillEmptyCellsWithWalls(RandomDungeon dungeon)
        {
            for (int x = 0; x < dungeon.width; ++x)
            {
                for (int y = 0; y < dungeon.height; ++y)
                {
                    if (dungeon.TileType(x,y) == RandomDungeonTileData.EMPTY_TILE)
                    {
                        dungeon.ChangeTileType(x, y, RandomDungeonTileData.WALL_TILE);
                    }
                }
            }
        }

        private void EnforceWallHeight(RandomDungeon dungeon)
        {
            // Add a bottom layer of walls
            for (int x = 0; x < dungeon.width; ++x)
            {
                for (int y = dungeon.height - 2; y >= 0; --y)
                {
                    if (dungeon.TileType(x,y) == RandomDungeonTileData.WALL_TILE && dungeon.TileType(x, y + 1) == RandomDungeonTileData.EMPTY_TILE &&
                        (y - 1 < 0 || dungeon.TileType(x, y - 1) != RandomDungeonTileData.WALL_TILE))
                    {
                        dungeon.ChangeTileType(x, y + 1, RandomDungeonTileData.WALL_TILE);
                    }
                }
            }

            for (int x = 0; x < dungeon.width; ++x)
            {
                for (int y = dungeon.height - 2; y >= 0; --y)
                {
                    if (dungeon.TileType(x,y) == RandomDungeonTileData.EMPTY_TILE &&
                        (y - 1 < 0 || dungeon.TileType(x, y - 1) == RandomDungeonTileData.WALL_TILE) &&
                        ((x - 1 >= 0 && dungeon.TileType(x - 1, y) == RandomDungeonTileData.WALL_TILE) || (x + 1 < dungeon.width && dungeon.TileType(x + 1, y) == RandomDungeonTileData.WALL_TILE)))
                    {
                        dungeon.ChangeTileType(x, y + 1, RandomDungeonTileData.WALL_TILE);
                    }
                }
            }

            for (int x = 0; x < dungeon.width; ++x)
            {
                for (int y = 0; y < dungeon.height; ++y)
                {
                    int tile = dungeon.TileType(x, y);
                    bool wallBelow = (y < dungeon.height - 1 && dungeon.TileType(x, y + 1) == RandomDungeonTileData.WALL_TILE);
                    
                    if (tile == RandomDungeonTileData.EMPTY_TILE)
                    {
                        if (wallBelow)
                        {
                            dungeon.ChangeTileType(x, y, RandomDungeonTileData.WALL_TILE);
                        }
                    }
                }
            }
        }

        private void PrepareNewMapGeneration()
        {
            mRoomScratch.Clear();
            mAvailableExits.Clear();
            mPrimaryPathPositions.Clear();
            mCurrentRoomId = 1;
            mCurrentTrapId = 1;
            maxDistanceFromPrimaryPath = 0;
            mPreviousRoomPlaced = null;
            mMapInvalid = false;
            mGenerationRequiredRetry = false;
        }

        /// <summary>
        /// Constructs the primary or critical path for the dungeon - this is the central path
        /// that any extra rooms branch off from.
        /// Primary paths only move forward - they never branch. 
        /// </summary>
        /// <param name="scopes"></param>
        private void ConstructPrimaryPath(RandomDungeonScopeData[] scopes)
        {
            // After choosing where a new room is going to go, close off all the paths in
            // the previous room to prevent branching.

            List<int> successfulRoomsPerScope = new List<int>();
            for (int i = 0; i < scopes.Length; ++i)
            {
                successfulRoomsPerScope.Add(0);
            }

            for (int scopeIdx = 0; scopeIdx < scopes.Length; ++scopeIdx)
            {
                RandomDungeonScopeData scope = scopes[scopeIdx];
                int roomsPerScope = mRNG.Next(scope.criticalPathMinRooms, scope.criticalPathMaxRooms);
                
                for (int i = 0; i < roomsPerScope; ++i)
                {
                    RoomPlacementOptions options = RoomPlacementOptions.None;
                    if (i == 0)
                    {
                        options = RoomPlacementOptions.AllowMismatchScopes;
                    }

                    if (TryPlaceRoom("primary", true, scopeIdx, options) != null)
                    {
                        successfulRoomsPerScope[scopeIdx]++;
                    }
                }
            }

            for (int i = 0; i < successfulRoomsPerScope.Count; ++i)
            {
                if (successfulRoomsPerScope[i] < scopes[i].criticalPathMinRooms)
                {
                    mMapInvalid = true;
                    return;
                }
            }
        }

        private void CollectAvailableExits(List<int> forbiddenScopes = null)
        {
            mAvailableExits.Clear();

            foreach(KeyValuePair<Vector2Int, RandomDungeonTileData> pair in mRoomScratch)
            {
                if (pair.Value.tileType == RandomDungeonTileData.EXIT_TILE)
                {
                    if (forbiddenScopes == null || !forbiddenScopes.Contains(mRoomScratch[pair.Key].scope))
                    {
                        mAvailableExits.Add(pair.Key);
                    }
                }
            }
        }

        // todo bdsowers - enforce minOptionalSpecailRooms or use it as more a suggestion to the RNG?
        // todo bdsowers - enforce that optional special rooms don't get reused
        private void AddSpecialRooms(int minOptionalSpecialRooms, int maxOptionalSpecialRooms)
        {
            // Try to add all the required special rooms
            // If for some reason one can't be added, return false, which will trigger a full map regeneration
            List<RoomData> allRequiredSpecial = mRoomSet.AllRoomsFromCategory("special_required");
            for (int i = 0; i < allRequiredSpecial.Count; ++i)
            {
                if (TryPlaceRoom("", false, -1, RoomPlacementOptions.AllowWallsToBecomeExits, allRequiredSpecial[i]) == null)
                {
                    mMapInvalid = true;
                    return;
                }
            }

            int optionalSpecialRooms = mRNG.Next(minOptionalSpecialRooms, maxOptionalSpecialRooms + 1);
            for (int i = 0; i < optionalSpecialRooms; ++i)
            {
                TryPlaceRoom("special_optional", false, -1, RoomPlacementOptions.AllowWallsToBecomeExits);
            }
        }

        private void AddExtraRooms(int numRooms)
        {
            for (int i = 0; i < numRooms; ++i)
            {
                TryPlaceRoom("extra", false, -1, RoomPlacementOptions.AllowWallsToBecomeExits);
            }
        }

        /// <summary>
        /// Repeatedly tries to place a room, choosing a random room from the room set and a random exit to place it off of.
        /// This will retry some maximum number of times and quit if it never finds a valid choice.
        /// </summary>
        /// <param name="roomCategory"></param>
        /// <param name="primaryPath"></param>
        /// <param name="scope"></param>
        /// <param name="roomPlacementOptions"></param>
        /// <param name="roomDataOverride"></param>
        /// <returns>The room data for the room that was placed, or null if no room could be placed.</returns>
        private RoomData TryPlaceRoom(string roomCategory, bool primaryPath, int scope, RoomPlacementOptions roomPlacementOptions = RoomPlacementOptions.None, RoomData roomDataOverride = null)
        {
            int maxAttempts = 200;
            while (maxAttempts > 0)
            {
                Vector2Int mapExit = ChooseMapExitForNextRoom();
                RoomData randomRoom = roomDataOverride != null ? roomDataOverride : mRoomSet.RandomRoomOfCategory(roomCategory, mPreviousRoomPlaced, mRNG);

                if (randomRoom == null)
                {
                    --maxAttempts;
                    continue;
                }

                Vector2Int roomExit = randomRoom.RandomExit(mRNG);

                if (scope == -1)
                {
                    scope = mRoomScratch[mapExit].scope;
                }

                if (!RoomHasInvalidCollision(randomRoom, mapExit, roomExit, scope, roomPlacementOptions))
                {
                    // If we're constructing a primary path, clear out any exits currently
                    // in our list. Placing a room will add new available exits to build off of.
                    // This also guards against the primary path looping in on itself.
                    if (primaryPath)
                    {
                        mAvailableExits.Clear();

                        mPrimaryPathPositions.Add(new Vector2Int(mapExit.x, mapExit.y));
                    }

                    mAvailableExits.Remove(mapExit);
                    PlaceRoom(randomRoom, mapExit, roomExit, scope, primaryPath);

                    // The probability of picking a room lowers each time it's picked
                    randomRoom.probability /= 2;

                    mPreviousRoomPlaced = randomRoom;
                    mPreviousMapExitUsed = mapExit;

                    return randomRoom;
                }

                --maxAttempts;
            }

            return null;
        }

        private Vector2Int ChooseMapExitForNextRoom()
        {
            // If there are no available exits, use (0,0)
            // This should only ever be possible for the very first room placement
            // Otherwise pick a random exit
            int exitIndex = -1;
            Vector2Int mapExit;
            
            if (mAvailableExits.Count == 0)
            {
                mapExit = new Vector2Int(0, 0);
            }
            else
            {
                exitIndex = mRNG.Next(0, mAvailableExits.Count);
                mapExit = mAvailableExits[exitIndex];
            }

            return mapExit;
        }

        /// <summary>
        /// Actually performs room placement in the scratchpad.
        /// </summary>
        /// <param name="roomData"></param>
        /// <param name="mapExit"></param>
        /// <param name="roomExit"></param>
        /// <param name="scope"></param>
        /// <param name="primaryPath"></param>
        private void PlaceRoom(RoomData roomData, Vector2Int mapExit, Vector2Int roomExit, int scope, bool primaryPath)
        {
            int maxDistance = 0;

            for (int x = 0; x < roomData.width; ++x)
            {
                for (int y = 0; y < roomData.height; ++y)
                {
                    int mapX = mapExit.x - roomExit.x + x;
                    int mapY = mapExit.y - roomExit.y + y;
                    Vector2Int mapPos = new Vector2Int(mapX, mapY);
                    char roomTile = roomData.Tile(x, y);

                    // Empty tiles shouldn't be represented in the dictionary
                    if (roomTile == RandomDungeonTileData.EMPTY_TILE)
                    {
                        continue;
                    }

                    // Create a new tile or edit a tile that already exists.
                    // This ensures we don't generate unnecessary garbage.
                    RandomDungeonTileData td = null;
                    if (!mRoomScratch.ContainsKey(mapPos))
                    {
                        td = new RandomDungeonTileData();
                        mRoomScratch.Add(mapPos, td);

                        float distance = Vector2Int.Distance(roomExit, new Vector2Int(x, y));
                        int distanceInt = Mathf.CeilToInt(distance);
                        maxDistance = Mathf.Max(maxDistance, distanceInt);

                        td.tileId = mCurrentTileId + distanceInt;

                        td.position = mapPos;
                    }
                    else
                    {
                        td = mRoomScratch[mapPos];
                        td.Clear();
                    }
                   
                    td.tileType = roomTile;
                    td.scope = scope;
                    td.room = mCurrentRoomId;
                    
                    if (primaryPath)
                    {
                        td.distanceFromPrimaryPath = 0;
                    }
                    else
                    {
                        // todo bdsowers - this is not perfect
                        // maybe it doesn't need to be perfect, we'll see
                        int mapExitDistance = mRoomScratch[mapExit].distanceFromPrimaryPath;
                        td.distanceFromPrimaryPath = mapExitDistance + 1;
                        maxDistanceFromPrimaryPath = Mathf.Max(maxDistanceFromPrimaryPath, td.distanceFromPrimaryPath);
                    }

                    // Trap
                    // Register it in the map proper as a walkable tile
                    // give a block of trap tiles the same identifier - they form one large trap
                    if (roomTile == RandomDungeonTileData.TRAP_TILE)
                    {
                        int neighborTrap = NeighborTrap(mapX, mapY);
                        int trapId = neighborTrap;
                        if (trapId == -1)
                        {
                            trapId = mCurrentTrapId;
                            mCurrentTrapId++;
                        }

                        td.tileType = RandomDungeonTileData.WALKABLE_TILE;
                        td.trap = trapId;
                    }
                    else if (roomTile == RandomDungeonTileData.POSSIBLE_CHEST_TILE)
                    {
                        td.tileType = RandomDungeonTileData.WALKABLE_TILE;
                        td.chest = 1;
                    }
                    else if (roomTile == RandomDungeonTileData.GUARANTEED_CHEST_TILE)
                    {
                        td.tileType = RandomDungeonTileData.WALKABLE_TILE;
                        td.chest = 2;
                    }

                    // Add the new exit to the list of available exits
                    if (roomTile == RandomDungeonTileData.EXIT_TILE)
                    {
                        mAvailableExits.Add(mapPos);
                    }
                }
            }

            ++mCurrentRoomId;
            mCurrentTileId += maxDistance + 1;
        }

        private int NeighborTrap(int x, int y)
        {
            for (int adjustedX = -1; adjustedX <= 1; ++adjustedX)
            {
                for (int adjustedY = -1; adjustedY <= 1; ++adjustedY)
                {
                    int mapX = x + adjustedX;
                    int mapY = y + adjustedY;
                    Vector2Int v = new Vector2Int(mapX, mapY);

                    RandomDungeonTileData td = null;
                    if (mRoomScratch.TryGetValue(v, out td))
                    {
                        if (td.trap != -1)
                            return td.trap;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Verifies whether room placement is possible.
        /// We go through every tile in the room and ensure that it's not overlapping something invalid
        /// (ie: if we try to place a wall that would overlap a floor tile, that's invalid).
        /// </summary>
        /// <param name="roomData"></param>
        /// <param name="mapExit"></param>
        /// <param name="roomExit"></param>
        /// <param name="scope"></param>
        /// <param name="roomPlacementOptions"></param>
        /// <returns></returns>
        private bool RoomHasInvalidCollision(RoomData roomData, Vector2Int mapExit, Vector2Int roomExit, int scope, RoomPlacementOptions roomPlacementOptions = RoomPlacementOptions.None)
        {
            bool fullOverlap = true;

            for (int roomX = 0; roomX < roomData.width; ++roomX)
            {
                for (int roomY = 0; roomY < roomData.height; ++roomY)
                {
                    int mapX = mapExit.x - roomExit.x + roomX;
                    int mapY = mapExit.y - roomExit.y + roomY;
                    Vector2Int mapPos = new Vector2Int(mapX, mapY);

                    int roomTile = roomData.Tile(roomX, roomY);
                    
                    if (!mRoomScratch.ContainsKey(mapPos) || roomTile == RandomDungeonTileData.EMPTY_TILE)
                    {
                        // Any kind of tile can be placed over an empty location
                        // However, at this point we do know that the rooms are not 100% overlapping
                        fullOverlap = false;
                        continue;
                    }
                    else
                    {
                        int mapTile = mRoomScratch[mapPos].tileType;
                        int mapTileScope = mRoomScratch[mapPos].scope;

                        if (scope != mapTileScope && !RoomPlacementOptionSet(roomPlacementOptions, RoomPlacementOptions.AllowMismatchScopes))
                        {
                            // We're trying to connect with a room in a different scope, which we can never do to ensure
                            // that a scope only has a single point of entry.
                            return true;
                        }
                        else if (RoomPlacementOptionSet(roomPlacementOptions, RoomPlacementOptions.AllowWallsToBecomeExits) && 
                            mapTile == RandomDungeonTileData.WALL_TILE && roomTile == RandomDungeonTileData.EXIT_TILE)
                        {
                            // If the options indicate that walls can become exits, support that.
                            continue;
                        }
                        else if (mapTile != roomTile)
                        {
                            // Only identical tiles can be placed over one another.
                            return true;
                        }
                    }
                }
            }

            // There's still the chance that this room is being placed completely over a previous room, which
            // we don't want to allow because it throws off our room counts.
            // We should be trying to place against at least one empty square

            return fullOverlap;
        }

        /// <summary>
        /// After map generation is finished, this will close off all the exits that don't lead to
        /// another room.
        /// </summary>
        /// <param name="dungeon"></param>
        private void CloseExitsToNowhere(RandomDungeon dungeon)
        {
            if (stopCloseExitsToNowhere)
                return;

            int exitPasses = 2; // should be equal the wall height
            for (int exitPass = 0; exitPass < exitPasses; ++exitPass)
            {
                for (int x = 0; x < dungeon.width; ++x)
                {
                    for (int y = 0; y < dungeon.height; ++y)
                    {
                        int tile = dungeon.TileType(x, y);
                        if (tile == RandomDungeonTileData.EXIT_TILE)
                        {
                            if (dungeon.NumWalkableNeighbors(x, y) <= 1)
                            {
                                dungeon.ChangeTileType(x, y, RandomDungeonTileData.WALL_TILE);
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < dungeon.width; ++x)
            {
                for (int y = 0; y < dungeon.height; ++y)
                {
                    int tile = dungeon.TileType(x, y);
                    if (tile == RandomDungeonTileData.EMPTY_TILE)
                    {
                        if (dungeon.NumWalkableNeighbors(x, y) >= 1)
                        {
                            dungeon.ChangeTileType(x, y, RandomDungeonTileData.WALL_TILE);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates the RandomDungeon object from the scratchpad, creating the final 2d array that
        /// represents the dungeon in the process.
        /// </summary>
        /// <returns></returns>
        private RandomDungeon GenerateDungeonObjectFromScratchPad(Dictionary<Vector2Int, RandomDungeonTileData> scratch)
        {
            int minX = int.MaxValue,
                minY = int.MaxValue,
                maxX = int.MinValue,
                maxY = int.MinValue;

            foreach(Vector2Int pos in scratch.Keys)
            {
                minX = Mathf.Min(minX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxX = Mathf.Max(maxX, pos.x);
                maxY = Mathf.Max(maxY, pos.y);
            }

            int padding = 2;
            int width = maxX - minX + 1 + padding * 2;
            int height = maxY - minY + 1 + padding * 2;
            RandomDungeon dungeon = new RandomDungeon(width, height);
           
            foreach(KeyValuePair<Vector2Int, RandomDungeonTileData> pair in scratch)
            {
                int x = pair.Key.x - minX + padding;
                int y = pair.Key.y - minY + padding;
                dungeon.SetData(x, y, pair.Value);
            }

            for (int i = 0; i < mPrimaryPathPositions.Count; ++i)
            {
                mPrimaryPathPositions[i] += (new Vector2Int(-minX, -minY) + new Vector2Int(padding, padding));
            }

            dungeon.primaryPathPositions = mPrimaryPathPositions;

            return dungeon;
        }
    }
}