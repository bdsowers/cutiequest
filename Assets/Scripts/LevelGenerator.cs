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

    private void Start()
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
    }

    private void PlaceDeadEndInterests()
    {
        RandomDungeonNetwork network = new RandomDungeonNetwork();
        network.EvaluateDungeon(dungeon);
        List<RandomDungeonNetwork.RandomDungeonNetworkNode> deadEnds = network.DeadEnds();
        deadEnds.Sort((i1, i2) => i2.distanceFromPrimaryPath.CompareTo(i1.distanceFromPrimaryPath));

        // Remove dead ends that are too close to each other from consideration (based off room Id, which isn't a perfect metric but is OK)
        // todo bdsowers - also remove dead-ends that are too close to our guaranteed chests/shrines that
        // have already been placed.
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
        if (value < 33 && (!mShrinePlaced || !mNPCPlaced))
        {
            // Shrine or NPC
            value = Random.Range(0, 100);
            if (value < 50 && !mShrinePlaced)
            {
                PlaceShrine(pos);
            }
            else
            {
                PlaceNPC(pos);
            }
        }
        else
        {
            PlaceChest(pos);
        }
    }

    private void PlaceNPC(Vector2Int pos, int cheatOverride = -1)
    {
        mNPCPlaced = true;

        string[] npcs = new string[] { "Beats", "HotDogMan", "PunkyPeter" };
        string npc = npcs.Sample();
        if (cheatOverride != -1)
            npc = npcs[cheatOverride];

        GameObject shrine = PlaceMapPrefab(npc, pos.x, pos.y, 1);
        PlaceSurroundingActivationPlates(pos.x, pos.y, "Character_" + npc, null, false);
    }

    private void PlaceShrine(Vector2Int pos, int cheatOverride = -1)
    {
        mShrinePlaced = true;

        string shrineName = PrefabManager.instance.shrinePrefabs.Sample().name;
        if (cheatOverride != -1)
            shrineName = PrefabManager.instance.shrinePrefabs[cheatOverride].name;

        GameObject shrine = PlaceMapPrefab(shrineName, pos.x, pos.y, 1);
        PlaceSurroundingActivationPlates(pos.x, pos.y, null, shrine.GetComponent<Shrine>());
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

        GameObject chest = PlaceMapPrefab(chestType, pos.x, pos.y, 1);
        PlaceSurroundingActivationPlates(pos.x, pos.y, null, chest.GetComponent<Shrine>());
    }

    private bool IsPresetRoom()
    {
        return CurrentDungeonFloorData().generationData.scopeData[0].criticalPathMaxRooms == 1;
    }

    private GameObject PlaceMapPrefab(string prefabName, int tileX, int tileY, int collisionMapMark = -1, float yOffset = 0f)
    {
        GameObject newItem = GameObject.Instantiate(PrefabManager.instance.PrefabByName(prefabName));
        newItem.transform.SetParent(transform);
        newItem.transform.position = new Vector3(tileX, yOffset, -tileY);

        if (collisionMapMark != -1)
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
        else if (itemNum < PrefabManager.instance.shrinePrefabs.Length + 3)
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
                    PlaceMapPrefab(biomeData.wallPrefabs.Sample(), x, y, 1);
                }
                else if (dungeon.TileType(x, y) == RandomDungeonTileData.WALKABLE_TILE ||
                    dungeon.TileType(x, y) == RandomDungeonTileData.EXIT_TILE)
                {
                    PlaceMapPrefab(biomeData.floorPrefabs.Sample(), x, y);
                }
                else if (dungeon.TileType(x,y) == SHOP_PEDESTAL)
                {
                    PlaceMapPrefab(biomeData.floorPrefabs[0], x, y).GetComponent<RevealWhenAvatarIsClose>().allowScaleVariation = false; ;
                    
                    PlaceMapPrefab(biomeData.shopPedestablPrefab, x, y, 1).GetComponent<RevealWhenAvatarIsClose>().allowScaleVariation = false;
                    GameObject buyableItem = PlaceMapPrefab(RandomItem(), x, y);
                    buyableItem.transform.localPosition += Vector3.up * 0.3f;

                    GameObject activationPlate = PlaceMapPrefab("ActivationPlate", x, y + 1);
                    activationPlate.GetComponent<ActivationPlate>().item = buyableItem.GetComponent<Item>();
                }
                else if (dungeon.TileType(x,y) == SHOP_KEEPER)
                {
                    PlaceMapPrefab(biomeData.floorPrefabs[0], x, y);
                    PlaceMapPrefab("ShopKeep", x, y, 1, 0.5f);

                    PlaceSurroundingActivationPlates(x, y, "shopkeep_talk", null, false);
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

    private void PlaceSurroundingActivationPlates(int x, int y, string cinematicEvent = null, Shrine shrine = null, bool includeBehind = true)
    {
        int[] offsets = new int[] { 0, 1, 1, 0, -1, 0, 0, -1 };

        int offsetsToUse = offsets.Length;
        if (!includeBehind)
            offsetsToUse-=2;

        for (int i = 0; i < offsetsToUse; i += 2)
        {
            int offsetX = offsets[i];
            int offsetY = offsets[i + 1];

            GameObject plateObj = PlaceMapPrefab("ActivationPlate", x + offsetX, y + offsetY);
            ActivationPlate plate = plateObj.GetComponent<ActivationPlate>();
            plate.cinematicEvent = cinematicEvent;
            plate.shrine = shrine;
        }
    }

    private string RandomItem()
    {
        return PrefabManager.instance.itemPrefabs.Sample().name;
    }

    public Vector2Int FindEmptyNearbyPosition(Vector2Int sourcePos)
    {
        if (mDungeon.TileType(sourcePos) != RandomDungeonTileData.WALKABLE_TILE ||
            mCollisionMap.SpaceMarking(sourcePos.x, sourcePos.y) != 0)
        {
            for (int xOffset = -1; xOffset <= 1; ++xOffset)
            {
                for (int yOffset = 1; yOffset >= -1; --yOffset)
                {
                    Vector2Int pos = sourcePos + new Vector2Int(xOffset, yOffset);
                    if (mDungeon.TileType(pos) == RandomDungeonTileData.WALKABLE_TILE &&
                        mCollisionMap.SpaceMarking(pos.x, pos.y) == 0 &&
                        Mathf.Abs(xOffset) != Mathf.Abs(yOffset))
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
        Vector2Int pos = mDungeon.primaryPathPositions[0];
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

        GameObject exit = GameObject.Instantiate(PrefabManager.instance.PrefabByName("Exit"));
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
        
        DungeonEnemyData enemyData = floorData.enemyData;

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
        GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName(enemy));
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
            if (GoldDiggerQuirk.quirkEnabled)
            {
                prefab = "CollectableCoin";
            }

            GameObject newHeart = GameObject.Instantiate(PrefabManager.instance.PrefabByName(prefab));
            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            walkablePositions.Remove(pos2);
            Vector3 pos = MapCoordinateHelper.MapToWorldCoords(pos2);
            newHeart.transform.position = pos;
            
            // Don't mark these on the collision map - entities can walk through them freely
        }
    }

    private void PlaceTraps()
    {
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

        // Now generate traps, filling the respective regions (where still possible)
        foreach(KeyValuePair<int, List<Vector2Int>> pair in regions)
        {
            // todo bdsowers - trap density, my dude
            if (Random.Range(0, 100) > 15)
                continue;

            int spikeNum = 0;
            bool uniform = (Random.Range(0, 2) == 0);
            float direction = (Random.Range(0, 2) == 0 ? 1 : -1);

            // Generate the trap, making sure we're not generating out of turn
            for (int posIdx = 0; posIdx < pair.Value.Count; ++posIdx)
            {
                Vector2Int pos = pair.Value[posIdx];

                if (mCollisionMap.SpaceMarking(pos.x, pos.y) == 0)
                {
                    GameObject trapObj = PlaceMapPrefab("SpikeTrap", pos.x, pos.y);
                    SpikeTrap spikes = trapObj.GetComponent<SpikeTrap>();

                    if (!uniform)
                        spikes.timeOffset = 0.25f * spikeNum * direction;

                    ++spikeNum;
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
    }
}
