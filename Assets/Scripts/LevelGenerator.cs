using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;
using ArrayExtensions;

public class LevelGenerator : MonoBehaviour
{
    private const int SHOP_PEDESTAL = 7;
    private const int SHOP_KEEPER = 8;

    RandomDungeon mDungeon;
    RandomDungeonGenerator mDungeonGenerator;
    RoomSet mRoomset;
    CollisionMap mCollisionMap;

    public RandomDungeon dungeon {  get { return mDungeon; } }

    private void Start()
    {
        mDungeonGenerator = new RandomDungeonGenerator();
        TextAsset roomSetData = Resources.Load<TextAsset>("RoomSets/roomset6");
        mRoomset = new RoomSet();
        mRoomset.LoadFromTextAsset(roomSetData);

        mDungeon = mDungeonGenerator.GenerateDungeon(mRoomset, DungeonGenerationData());

        mCollisionMap = GetComponent<CollisionMap>();
        mCollisionMap.SetupWithSize(mDungeon.width, mDungeon.height);
        GenerateEnvironmentFromDungeon(mDungeon);

        PlaceAvatar();
        PlaceEnemies();
        PlaceHearts();
        PlaceExit();
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

    private void GenerateEnvironmentFromDungeon(RandomDungeon dungeon)
    {
        DungeonBiomeData biomeData = Game.instance.currentDungeonData.biomeData;

        for (int x = 0; x < dungeon.width; ++x)
        {
            for (int y = 0; y < dungeon.height; ++y)
            {
                RandomDungeonTileData tileData = dungeon.Data(x, y);

                if (dungeon.TileType(x,y) == RandomDungeonTileData.EMPTY_TILE)
                {
                    mCollisionMap.MarkSpace(x, y, 1);
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

                    GameObject activationPlate = PlaceMapPrefab("ActivationPlate", x, y + 1);
                    activationPlate.GetComponent<ActivationPlate>().cinematicEvent = "shopkeep_talk";
                }


                if (tileData.chest == 2)
                {
                    PlaceMapPrefab("Chest", x, y, 1);
                    PlaceMapPrefab("ActivationPlate", x, y + 1);
                }
                else if (tileData.chest == 1)
                {
                    bool generateShrine = (Random.Range(0, 100) < 25);
                    if (generateShrine)
                    {
                        Debug.Log("A special room has spawned including a shrine.");
                        PlaceMapPrefab(PrefabManager.instance.shrinePrefabs.Sample().name, x, y, 1);
                        PlaceMapPrefab("ActivationPlate", x, y + 1);
                    }
                    else
                    {
                        Debug.Log("A special room has spawned including a chest.");
                        PlaceMapPrefab("Chest", x, y, 1);
                        PlaceMapPrefab("ActivationPlate", x, y + 1);
                    }
                }
            }
        }
    }

    private string RandomItem()
    {
        return PrefabManager.instance.itemPrefabs.Sample().name;
    }

    private Vector2Int FindEmptyNearbyPosition(Vector2Int sourcePos)
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

        mCollisionMap.MarkSpace(pos.x, pos.y, avatar.GetComponent<SimpleMovement>().collisionIdentity);
        avatar.transform.position = new Vector3(pos.x, 0.5f, -pos.y);

        // Also place any followers/pets adjacent to the player
        Follower follower = avatar.GetComponent<PlayerController>().follower;
        string followerId = Game.instance.playerData.followerUid;
        if (string.IsNullOrEmpty(followerId))
            followerId = "1";
        string followerModel = Game.instance.characterDataList.CharacterWithUID(followerId).model;
        follower.GetComponentInChildren<CharacterModel>().ChangeModel(followerModel);

        pos = FindEmptyNearbyPosition(pos);
        follower.transform.position = new Vector3(pos.x, 0.5f, -pos.y);
    }

    private void PlaceExit()
    {
        Vector2Int pos = mDungeon.primaryPathPositions[mDungeon.primaryPathPositions.Count - 1];
        pos = FindEmptyNearbyPosition(pos);

        GameObject exit = GameObject.Instantiate(PrefabManager.instance.PrefabByName("Exit"));
        exit.transform.position = new Vector3(pos.x, 0.4f, -pos.y);
    }

    private string ChooseEnemy(DungeonFloorData floorData)
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
        DungeonFloorData data = CurrentDungeonFloorData();
        int numEnemies = Random.Range(data.enemyData.minEnemies, data.enemyData.maxEnemies);

        for (int i = 0; i < numEnemies; ++i)
        {
            string enemy = ChooseEnemy(data);
            GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName(enemy));
            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            walkablePositions.Remove(pos2);
            Vector3 pos = new Vector3(pos2.x, 0.5f, -pos2.y);
            newEnemy.transform.position = pos;
            mCollisionMap.MarkSpace(pos2.x, pos2.y, newEnemy.GetComponent<SimpleMovement>().collisionIdentity);
        }
    }

    private void PlaceHearts()
    {
        List<Vector2Int> walkablePositions = mCollisionMap.EmptyPositions();

        int numHearts = 5;
        for (int i = 0; i < numHearts; ++i)
        {
            GameObject newHeart = GameObject.Instantiate(PrefabManager.instance.PrefabByName("CollectableHeart"));
            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            walkablePositions.Remove(pos2);
            Vector3 pos = new Vector3(pos2.x, 0.5f, -pos2.y);
            newHeart.transform.position = pos;
            
            // Don't mark these on the collision map - entities can walk through them freely
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
}
