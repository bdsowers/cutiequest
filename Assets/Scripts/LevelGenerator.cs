using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;
using ArrayExtensions;
using VectorExtensions;

public class LevelGenerator : MonoBehaviour
{
    private const char SHOP_PEDESTAL = '7';
    private const char SHOP_KEEPER = '8';
    private const char PRESET_ENEMY = '9';
    private const char AVATAR_POSITION = 'p';

    private const int WALKABLEMAP_DONT_MARK = -1;
    private const int WALKABLEMAP_USE_PREFAB_MARK = -2;
    private const int WALKABLEMAP_STATIC_MARK = 1;

    RandomDungeon mDungeon;
    RandomDungeonGenerator mDungeonGenerator;
    RoomSet mRoomset;
    CollisionMap mCollisionMap;
    KillableMap mKillableMap;

    public RandomDungeon dungeon {  get { return mDungeon; } }
    public CollisionMap collisionMap {  get { return mCollisionMap; } }

    private Vector2Int mAvatarStartPosition;
    private int mPresetEnemyCounter = 0;

    private bool mBombChestPlaced = false;
    private bool mNPCPlaced = false;
    private bool mShrinePlaced = false;

    private List<Item> mPreviouslyUsedItems = new List<Item>();

    private IEnumerator Start()
    {
        mDungeonGenerator = new RandomDungeonGenerator();

        TextAsset roomSetData = Resources.Load<TextAsset>("RoomSets/" + CurrentDungeonFloorData().roomSet);
        mRoomset = new RoomSet();
        mRoomset.LoadFromTextAsset(roomSetData);

        mDungeon = mDungeonGenerator.GenerateDungeon(mRoomset, DungeonGenerationData());

        mKillableMap = GetComponent<KillableMap>();
        mKillableMap.SetupWithDungeon(mDungeon);

        mCollisionMap = GetComponent<CollisionMap>();
        mCollisionMap.SetupWithSize(mDungeon.width, mDungeon.height);
        GenerateEnvironmentFromDungeon(mDungeon);

        PlaceAvatar();

        // Provide some time after the avatar is generated for any of its setup (ie: quirk setup)
        // to impact dungeon generation
        yield return null;

        // Special handling for the 'difficulty boost' effect
        DifficultyBoost db = GameObject.FindObjectOfType<DifficultyBoost>();
        if (db != null)
        {
            db.ApplyQuirks();
            yield return null;
        }

        yield return null;

        if (!IsPresetRoom())
        {
            PlaceDeadEndInterests();
        }

        PlaceTraps();
        PlaceEnemies();

        if (!IsPresetRoom())
        {

            PlaceHearts();
            PlaceExit();
        }

        QuirkSpecificSpawns();

        Game.instance.soundManager.PlayRandomMusicInCategory("DungeonMusic");

        // todo bdsowers - investigate this a bit further maybe?
        // Force a 'dirty' update for the player to try & fix a ui bug
        Game.instance.playerData.MarkDirty();

        yield break;
    }

    private void PlaceDeadEndInterests()
    {
        RandomDungeonNetwork network = new RandomDungeonNetwork();
        network.EvaluateDungeon(dungeon);
        List<RandomDungeonNetwork.RandomDungeonNetworkNode> deadEnds = network.DeadEnds();
        deadEnds.Sort((i1, i2) => i2.distanceFromPrimaryPath.CompareTo(i1.distanceFromPrimaryPath));

        // Remove dead ends that are too close to each other from consideration (based off room Id, which isn't a perfect metric but is OK)
        int position = 1;
        while (position < deadEnds.Count)
        {
            if (deadEnds[position].emptyPositions.Count == 0)
            {
                deadEnds.RemoveAt(position);
            }
            else if (Vector2.Distance(mAvatarStartPosition, deadEnds[position].emptyPositions.Sample()) < 8)
            {
                deadEnds.RemoveAt(position);
            }
            else if (Mathf.Abs(deadEnds[position].roomId - deadEnds[position-1].roomId) < 2)
            {
                deadEnds.RemoveAt(position);
            }
            else
            {
                ++position;
            }
        }

        int max = Mathf.Min(Random.Range(2,4), deadEnds.Count);

        for (int i = 0; i < max; ++i)
        {
            RandomDungeonNetwork.RandomDungeonNetworkNode deadEnd = deadEnds.Sample();
            Vector2Int pos = deadEnd.emptyPositions.Sample();

            PlacePointOfInterest(pos);

            deadEnds.Remove(deadEnd);
        }
    }

    private void PlacePointOfInterest(Vector2Int pos)
    {
        int value = Random.Range(0, 100);
        int prob = 33;

        if (PriorityNPCUnfound())
        {
            // Probably starts pretty high, but as the dungeon floor increases, gets even higher
            prob = 80 + Game.instance.currentDungeonFloor * 10;
        }

        if (value < prob && (!mShrinePlaced || !mNPCPlaced))
        {
            // Shrine or NPC
            value = Random.Range(0, 100);
            if (!mNPCPlaced && PriorityNPCUnfound() && AvailableNPCS().Count > 0)
            {
                PlaceNPC(pos);
            }
            else if (value < 50 && !mShrinePlaced)
            {
                PlaceShrine(pos);
            }
            else if (!mNPCPlaced && AvailableNPCS().Count > 0)
            {
                PlaceNPC(pos);
            }
        }
        else
        {
            PlaceChest(pos);
        }
    }

    private bool PriorityNPCUnfound()
    {
        List<string> highPriorityNPCFlags = new List<string>()
        {
            "punkypeter",
            "trainer",
        };

        foreach(string flag in highPriorityNPCFlags)
        {
            if (!Game.instance.playerData.IsFlagSet(flag))
            {
                return true;
            }
        }

        return false;
    }

    private List<string> AvailableNPCS()
    {
        Dictionary<string, string> priorityNPCMap = new Dictionary<string, string>()
        {
            {"trainer", "Trainer"},
            {"punkypeter", "PunkyPeter" },
        };

        Dictionary<string, string> npcMap = new Dictionary<string, string>()
        {
            {"beats", "Beats" },
            {"hotdogman", "HotDogMan" },
            {"stylist", "Stylist" },
            {"tourist", "Tourist" },
            {"bruiser", "Bruiser" }
        };

        List<string> npcs = new List<string>();

        // If there are any priority NPCs, only return them
        // They're more important gameplay-wise
        foreach (KeyValuePair<string, string> pair in priorityNPCMap)
        {
            if (!Game.instance.playerData.IsFlagSet(pair.Key))
            {
                npcs.Add(pair.Value);
                return npcs;
            }
        }

        foreach (KeyValuePair<string, string> pair in npcMap)
        {
            if (!Game.instance.playerData.IsFlagSet(pair.Key))
            {
                npcs.Add(pair.Value);
            }
        }

        return npcs;
    }

    private void PlaceNPC(Vector2Int pos, int cheatOverride = -1)
    {
        mNPCPlaced = true;

        List<string> npcs = AvailableNPCS();

        string npc = npcs.Sample();
        if (cheatOverride != -1)
            npc = npcs[cheatOverride];

        Debug.Log("Placed NPC " + npc);

        // todo bdsowers - use proper walkable map writing
        GameObject shrine = PlaceMapPrefab(npc, pos.x, pos.y, WALKABLEMAP_USE_PREFAB_MARK);
        PlaceSurroundingActivationPlates(pos.x, pos.y, "Character_" + npc, null, shrine);
    }

    private void PlaceShrine(Vector2Int pos, int cheatOverride = -1)
    {
        mShrinePlaced = true;

        string shrineName = PrefabManager.instance.shrinePrefabs.Sample().name;
        if (cheatOverride != -1)
            shrineName = PrefabManager.instance.shrinePrefabs[cheatOverride].name;

        GameObject shrine = PlaceMapPrefab(shrineName, pos.x, pos.y, WALKABLEMAP_STATIC_MARK);
        PlaceSurroundingActivationPlates(pos.x, pos.y, null, shrine.GetComponent<Shrine>(), shrine);
    }

    private void PlaceChest(Vector2Int pos)
    {
        int luck = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Luck, Game.instance.avatar.gameObject);
        int val = Random.Range(0, 100) + luck;

        string chestType = "Chest";
        if (val < 15 && !mBombChestPlaced)
        {
            mBombChestPlaced = true;
            chestType = "BombChest";
        }
        else if (val < 75)
        {
            chestType = "Chest";
        }
        else if (val < 90)
        {
            chestType = "SuperChest";
        }
        else
        {
            chestType = "MegaChest";
        }

        GameObject chest = PlaceMapPrefab(chestType, pos.x, pos.y, WALKABLEMAP_STATIC_MARK);
        PlaceSurroundingActivationPlates(pos.x, pos.y, null, chest.GetComponent<Shrine>(), chest);
    }

    private bool IsPresetRoom()
    {
        return CurrentDungeonFloorData().generationData.scopeData[0].criticalPathMaxRooms == 1;
    }

    public GameObject PlaceMapPrefab(string prefabName, int tileX, int tileY, int collisionMapMark = WALKABLEMAP_DONT_MARK, float yOffset = 0f)
    {
        GameObject newItem = GameObject.Instantiate(PrefabManager.instance.PrefabByName(prefabName), transform);
        newItem.transform.SetParent(transform);
        newItem.transform.position = new Vector3(tileX, yOffset, -tileY);

        if (collisionMapMark == WALKABLEMAP_USE_PREFAB_MARK)
        {
            int mark = newItem.GetComponent<SimpleMovement>().uniqueCollisionIdentity;
            mCollisionMap.MarkSpace(tileX, tileY, mark);
        }
        else if (collisionMapMark != WALKABLEMAP_DONT_MARK)
        {
            mCollisionMap.MarkSpace(tileX, tileY, collisionMapMark);
        }

        return newItem;
    }

    private void HandleCheatTile(RandomDungeonTileData tileData, int x, int y, DungeonBiomeData biomeData)
    {
        PlaceMapPrefab(biomeData.floorPrefabs.Sample(), x, y);

        // todo bdsowers - this code is a crime against god and deserves hellfire.
        int itemNum = tileData.tileType - 'A';
        if (itemNum < PrefabManager.instance.shrinePrefabs.Length)
        {
            PlaceShrine(new Vector2Int(x, y), itemNum);
        }
        else if (itemNum < PrefabManager.instance.shrinePrefabs.Length + AvailableNPCS().Count)
        {
            PlaceNPC(new Vector2Int(x, y), itemNum - PrefabManager.instance.shrinePrefabs.Length);
        }
    }

    private void GenerateEnvironmentFromDungeon(RandomDungeon dungeon)
    {
        DungeonBiomeData biomeData = Game.instance.currentDungeonData.biomeData;

        for (int x = 0; x < dungeon.width; ++x)
        {
            for (int y = 0; y < dungeon.height; ++y)
            {
                RandomDungeonTileData tileData = dungeon.Data(x, y);

                if (dungeon.TileType(x,y) >= 'A' && dungeon.TileType(x,y) <= 'Z')
                {
                    HandleCheatTile(tileData, x, y, biomeData);
                }
                else if (dungeon.TileType(x,y) == RandomDungeonTileData.EMPTY_TILE)
                {
                    mCollisionMap.MarkSpace(x, y, -1);
                }
                else if (dungeon.TileType(x, y) == RandomDungeonTileData.WALL_TILE)
                {
                    PlaceMapPrefab(biomeData.wallPrefabs.Sample(), x, y, WALKABLEMAP_STATIC_MARK);
                }
                else if (dungeon.TileType(x, y) == RandomDungeonTileData.WALKABLE_TILE ||
                    dungeon.TileType(x, y) == RandomDungeonTileData.EXIT_TILE ||
                    dungeon.TileType(x,y) == AVATAR_POSITION)
                {
                    PlaceMapPrefab(biomeData.floorPrefabs.Sample(), x, y);
                }
                else if (dungeon.TileType(x,y) == SHOP_PEDESTAL)
                {
                    PlaceMapPrefab(biomeData.floorPrefabs[0], x, y).GetComponent<RevealWhenAvatarIsClose>().allowScaleVariation = false; ;

                    PlaceMapPrefab(biomeData.shopPedestablPrefab, x, y, WALKABLEMAP_STATIC_MARK).GetComponent<RevealWhenAvatarIsClose>().allowScaleVariation = false;
                    GameObject buyableItem = PlaceMapPrefab(RandomItem(), x, y);
                    buyableItem.transform.localPosition += Vector3.up * 0.3f;

                    GameObject activationPlate = PlaceMapPrefab("ActivationPlate", x, y + 1);
                    PlaceSurroundingActivationPlates(x, y, null, null, null, buyableItem.GetComponent<Item>());
                }
                else if (dungeon.TileType(x,y) == SHOP_KEEPER)
                {
                    PlaceMapPrefab(biomeData.floorPrefabs[0], x, y);
                    GameObject shopKeeper = PlaceMapPrefab("ShopKeep", x, y, WALKABLEMAP_USE_PREFAB_MARK, 0.5f);

                    PlaceSurroundingActivationPlates(x, y, "shopkeep_talk", null, shopKeeper);

                    if (Game.instance.isShopKeeperEnemy)
                        Game.instance.MakeShopKeeperEnemy();
                }
                else if (dungeon.TileType(x,y) == PRESET_ENEMY)
                {
                    PlaceMapPrefab(biomeData.floorPrefabs[0], x, y);
                    PlaceEnemy(CurrentDungeonFloorData(), new Vector2Int(x, y));
                }


                if (tileData.chest == 2)
                {
                    PlaceChest(new Vector2Int(x, y));
                }
                else if (tileData.chest == 1)
                {
                    bool generateShrine = (Random.Range(0, 100) < 50);
                    if (generateShrine)
                    {
                        Debug.Log("A special room has spawned including a shrine.");
                        PlaceShrine(new Vector2Int(x, y));
                    }
                    else
                    {
                        Debug.Log("A special room has spawned including a chest.");
                        PlaceChest(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    private void PlaceSurroundingActivationPlates(int x, int y, string cinematicEvent = null, Shrine shrine = null, GameObject link = null, Item item = null)
    {
        int[] offsets = new int[] { 0, 1, 1, 0, -1, 0, 0, -1 };

        int offsetsToUse = offsets.Length;
        for (int i = 0; i < offsetsToUse; i += 2)
        {
            int offsetX = offsets[i];
            int offsetY = offsets[i + 1];

            GameObject plateObj = PlaceMapPrefab("ActivationPlate", x + offsetX, y + offsetY);
            ActivationPlate plate = plateObj.GetComponent<ActivationPlate>();
            plate.cinematicEvent = cinematicEvent;
            plate.shrine = shrine;
            plate.activationDirection = new Vector3(-offsetX, 0, offsetY);
            plate.item = item;

            if (link != null)
                plate.LinkToEntity(link);
        }
    }

    private string RandomItem()
    {
        List<Item> items = Game.instance.companionBuilder.ItemsInLevel(Game.instance.playerData.attractiveness, -1, Game.instance.playerData.scoutLevel, -1);
        Item item = items.Sample(mPreviouslyUsedItems);

        if (Cheats.forceTestItemGeneration) item = PrefabManager.instance.itemPrefabs[PrefabManager.instance.itemPrefabs.Length - 1].GetComponent<Item>();

        mPreviouslyUsedItems.Add(item);
        return item.name;
    }

    private bool IsPositionEmpty(Vector2Int pos)
    {
        int tileType = mDungeon.TileType(pos);
        bool possiblyEmpty = (tileType == RandomDungeonTileData.WALKABLE_TILE ||
            tileType == RandomDungeonTileData.EXIT_TILE ||
            tileType == AVATAR_POSITION);

        if (!possiblyEmpty)
            return false;

        return mCollisionMap.SpaceMarking(pos.x, pos.y) == 0;
    }

    public Vector2Int FindEmptyNearbyPosition(Vector2Int sourcePos)
    {
        if (!IsPositionEmpty(sourcePos))
        {
            for (int xOffset = -1; xOffset <= 1; ++xOffset)
            {
                for (int yOffset = 1; yOffset >= -1; --yOffset)
                {
                    Vector2Int pos = sourcePos + new Vector2Int(xOffset, yOffset);
                    if (IsPositionEmpty(pos) && Mathf.Abs(xOffset) != Mathf.Abs(yOffset))
                    {
                        return pos;
                    }
                }
            }
        }

        return sourcePos;
    }

    private void PlaceAvatar()
    {
        GameObject avatar = GameObject.Find("Avatar");
        avatar.transform.SetParent(transform);

        Vector2Int pos = mDungeon.primaryPathPositions[0];
        Vector2Int guaranteedPos = mDungeon.PositionForSpecificTile('p');
        if (guaranteedPos.x != -1 && guaranteedPos.y != -1)
            pos = guaranteedPos;

        pos = FindEmptyNearbyPosition(pos);
        mAvatarStartPosition = pos;

        mCollisionMap.MarkSpace(pos.x, pos.y, avatar.GetComponent<SimpleMovement>().uniqueCollisionIdentity);
        avatar.transform.position = MapCoordinateHelper.MapToWorldCoords(pos);

        // Also place any followers/pets adjacent to the player
        Follower follower = avatar.GetComponent<PlayerController>().follower;
        string followerId = Game.instance.playerData.followerUid;
        if (string.IsNullOrEmpty(followerId))
            followerId = "1";
        CharacterData followerData = Game.instance.followerData;
        follower.GetComponentInChildren<CharacterModel>().ChangeModel(followerData);

        pos = FindEmptyNearbyPosition(pos);
        follower.transform.position = MapCoordinateHelper.MapToWorldCoords(pos);

        avatar.GetComponent<Killable>().allowZeroDamage = (CurrentDungeonFloorData().roomSet == "introdungeon");

        avatar.GetComponent<PlayerController>().PlaceFollowerInCorrectPosition();
    }

    private void PlaceExit()
    {
        Vector2Int pos = mDungeon.primaryPathPositions[mDungeon.primaryPathPositions.Count - 1];
        pos = FindEmptyNearbyPosition(pos);

        GameObject exit = GameObject.Instantiate(PrefabManager.instance.PrefabByName("Exit"), transform);
        exit.transform.position = MapCoordinateHelper.MapToWorldCoords(pos, 0.4f);
    }

    private string ChooseEnemy(DungeonFloorData floorData)
    {
        if (IsPresetRoom())
        {
            return ChoosePresetEnemy(floorData);
        }
        else
        {
            return ChooseRandomEnemy(floorData);
        }
    }

    private string ChoosePresetEnemy(DungeonFloorData floorData)
    {
        ++mPresetEnemyCounter;
        if (mPresetEnemyCounter == 1)
        {
            return floorData.enemyData.commonEnemy.name;
        }
        else
        {
            return floorData.enemyData.uncommonEnemy.name;
        }
    }

    private string ChooseRandomEnemy(DungeonFloorData floorData)
    {
        DungeonData dungeonData = Game.instance.currentDungeonData;
        DungeonEnemyData enemyData = floorData.enemyData;

        // About 20% of enemies are from previous levels when possible
        bool useBackfill = Random.Range(0, 100) < 20;
        if (useBackfill && dungeonData.backfillEnemies != null && dungeonData.backfillEnemies.Count > 0)
        {
            return dungeonData.backfillEnemies.Sample().name;
        }

        int randomNum = Random.Range(0, 100);
        if (randomNum < 50)
        {
            return enemyData.commonEnemy.name;
        }
        else if (randomNum < 75)
        {
            return enemyData.uncommonEnemy.name;
        }
        else if (randomNum < 95)
        {
            return enemyData.rareEnemy.name;
        }
        else
        {
            return enemyData.scaryEnemy.name;
        }
    }

    private void PlaceEnemies()
    {
        List<Vector2Int> walkablePositions = mCollisionMap.EmptyPositions();

        // Remove any walkable positions too close to the avatar
       walkablePositions.RemoveAll((pos) => VectorHelper.OrthogonalDistance(pos, mAvatarStartPosition) < 10);

        DungeonFloorData data = CurrentDungeonFloorData();
        int numEnemies = Random.Range(data.enemyData.minEnemies, data.enemyData.maxEnemies);
        numEnemies = WantedQuirk.ApplyQuirkIfPresent(numEnemies);

        for (int i = 0; i < numEnemies; ++i)
        {
            if (walkablePositions.Count == 0)
                return;


            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            walkablePositions.Remove(pos2);
            PlaceEnemy(data, pos2);
        }
    }

    private void PlaceEnemy(DungeonFloorData data, Vector2Int pos2)
    {
        string enemy = ChooseEnemy(data);
        GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName(enemy), transform);
        Vector3 pos = MapCoordinateHelper.MapToWorldCoords(pos2);
        newEnemy.transform.position = pos;
        mCollisionMap.MarkSpace(pos2.x, pos2.y, newEnemy.GetComponent<SimpleMovement>().uniqueCollisionIdentity);
    }

    private void PlaceHearts()
    {
        List<Vector2Int> walkablePositions = mCollisionMap.EmptyPositions();

        int numHearts = 5;
        for (int i = 0; i < numHearts; ++i)
        {
            string prefab = "CollectableHeart";
            if (Game.instance.quirkRegistry.IsQuirkActive<GoldDiggerQuirk>())
            {
                prefab = "CollectableCoin";
            }

            GameObject newHeart = GameObject.Instantiate(PrefabManager.instance.PrefabByName(prefab), transform);
            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            walkablePositions.Remove(pos2);
            Vector3 pos = MapCoordinateHelper.MapToWorldCoords(pos2);
            newHeart.transform.position = pos;

            // Don't mark these on the collision map - entities can walk through them freely
        }
    }

    private int MaxTrapsToConsider(DungeonBiomeData biomeData)
    {
        return Mathf.Min(biomeData.trapPrefabs.Count, Game.instance.currentDungeonFloor);
    }

    private void PlaceTraps()
    {
        DungeonBiomeData biomeData = Game.instance.currentDungeonData.biomeData;

        // Collect regions with shared trap IDs
        Dictionary<int, List<Vector2Int>> regions = new Dictionary<int, List<Vector2Int>>();
        for (int x = 0; x < mDungeon.width; ++x)
        {
            for (int y = 0;  y < mDungeon.height; ++y)
            {
                RandomDungeonTileData td = mDungeon.Data(x, y);
                if (td.trap >= 0)
                {
                    List<Vector2Int> positions = null;
                    bool exists = regions.TryGetValue(td.trap, out positions);
                    if (!exists)
                    {
                        positions = new List<Vector2Int>();
                        regions.Add(td.trap, positions);
                    }

                    positions.Add(new Vector2Int(x, y));
                }
            }
        }

        // Try to place the traps
        // Traps may have requirements - ie: they may require certain region sizes
        // They may also conditionally take up 1 or all spaces of a region
        // todo bdsowers - break traps apart into traps & spawners so that traps
        // aren't responsible for spawning themselves, which is awkward...

        // Now generate traps, filling the respective regions (where still possible)

        // Find all the trap generators
        List<PlacedTrap> trapPlacerPrefabs = new List<PlacedTrap>();
        int maxTrapPrefabsToConsider = MaxTrapsToConsider(biomeData);
        for (int i = 0; i < maxTrapPrefabsToConsider; ++i)
        {
            string prefabName = biomeData.trapPrefabs[i];

            GameObject prefab = PrefabManager.instance.PrefabByName(prefabName);
            PlacedTrap trapPlacer = prefab.GetComponent<PlacedTrap>();
            if (trapPlacer != null)
            {
                trapPlacerPrefabs.Add(prefab.GetComponent<PlacedTrap>());
            }
            else
            {
                Debug.LogError("Unplaceable trap in biome: " + prefabName);
            }
        }

        List<PlacedTrap> potentialTraps = new List<PlacedTrap>();

        foreach(KeyValuePair<int, List<Vector2Int>> pair in regions)
        {
            int prob = 15;
            if (Game.instance.quirkRegistry.IsQuirkActive<TrapQueenQuirk>())
                prob = 75;

            if (Random.Range(0, 100) > prob)
                continue;

            potentialTraps.Clear();
            potentialTraps.AddRange(trapPlacerPrefabs);

            bool trapPlaced = false;
            while (!trapPlaced && potentialTraps.Count > 0)
            {
                PlacedTrap trap = potentialTraps.Sample();
                if (trap.CanSpawn(pair.Value))
                {
                    trap.Spawn(pair.Value, this);
                    trapPlaced = true;
                }
                else
                {
                    potentialTraps.Remove(trap);
                }
            }
        }
    }

    private DungeonFloorData CurrentDungeonFloorData()
    {
        DungeonData dungeonData = Game.instance.currentDungeonData;
        int floorIndex = Game.instance.currentDungeonFloor - 1;
        floorIndex = Mathf.Min(floorIndex, dungeonData.floorData.Length - 1);
        return dungeonData.floorData[floorIndex];
    }

    private RandomDungeonGenerationData DungeonGenerationData()
    {
        DungeonFloorData floorData = CurrentDungeonFloorData();
        RandomDungeonGenerationData data = floorData.generationData;
        data.randomSeed = Random.Range(int.MinValue, int.MaxValue);
        return data;
    }

    private void QuirkSpecificSpawns()
    {
        PartnerInCrimeQuirk picq = GameObject.FindObjectOfType<PartnerInCrimeQuirk>();
        if (picq != null)
        {
            picq.SpawnCops(mDungeon, mCollisionMap, mAvatarStartPosition);
        }

        HoarderQuirk hoarder = GameObject.FindObjectOfType<HoarderQuirk>();
        if (hoarder != null)
        {
            hoarder.SpawnDebris(mDungeon, mCollisionMap, mAvatarStartPosition);
        }

        GhostedQuirk ghosted = GameObject.FindObjectOfType<GhostedQuirk>();
        if (ghosted != null)
        {
            ghosted.SpawnGhosts(mDungeon, mCollisionMap, mAvatarStartPosition);
        }
    }
}
