using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;

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
        for (int x = 0; x < dungeon.width; ++x)
        {
            for (int y = 0; y < dungeon.height; ++y)
            {
                if (dungeon.TileType(x,y) == RandomDungeonTileData.EMPTY_TILE)
                {
                    mCollisionMap.MarkSpace(x, y, 1);
                }
                else if (dungeon.TileType(x, y) == RandomDungeonTileData.WALL_TILE)
                {
                    PlaceMapPrefab("StandardWall", x, y, 1);
                }
                else if (dungeon.TileType(x, y) == RandomDungeonTileData.WALKABLE_TILE ||
                    dungeon.TileType(x, y) == RandomDungeonTileData.EXIT_TILE)
                {
                    PlaceMapPrefab("Floor", x, y);
                }
                else if (dungeon.TileType(x,y) == SHOP_PEDESTAL)
                {
                    PlaceMapPrefab("StandardWall", x, y, 1);
                }
                else if (dungeon.TileType(x,y) == SHOP_KEEPER)
                {
                    PlaceMapPrefab("Floor", x, y);
                    PlaceMapPrefab("ShopKeep", x, y, 1, 0.5f);

                    GameObject activationPlate = PlaceMapPrefab("ActivationPlate", x, y + 1);
                    activationPlate.GetComponent<ActivationPlate>().cinematic = "Cinematics/shopkeeper";
                    activationPlate.GetComponent<ActivationPlate>().cinematicEvent = "shopkeep_talk";
                }
            }
        }
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

    private void PlaceEnemies()
    {
        List<Vector2Int> walkablePositions = mCollisionMap.EmptyPositions();

        int numEnemies = 20;
        for (int i = 0; i < numEnemies; ++i)
        {
            string enemy = Random.Range(0, 2) == 1 ? "Skeleton" : "Goblin";
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

    private RandomDungeonGenerationData DungeonGenerationData()
    {
        RandomDungeonGenerationData data = new RandomDungeonGenerationData();
        data.enforceTwoTileHighWalls = false;
        data.fillEmptyCellsWithWalls = false;
        data.randomSeed = Random.Range(int.MinValue, int.MaxValue);
        data.numExtraRoomsPlacedBeforeSpecialRooms = 10;
        data.numExtraRoomsPlacedAfterSpecialRooms = 20;

        RandomDungeonScopeData scope1 = new RandomDungeonScopeData();
        scope1.criticalPathMinRooms = 3;
        scope1.criticalPathMaxRooms = 6;

        RandomDungeonScopeData scope2 = new RandomDungeonScopeData();
        scope2.criticalPathMinRooms = 3;
        scope2.criticalPathMaxRooms = 6;

        RandomDungeonScopeData scope3 = new RandomDungeonScopeData();
        scope3.criticalPathMinRooms = 3;
        scope3.criticalPathMaxRooms = 6;

        RandomDungeonScopeData[] scopes = new RandomDungeonScopeData[] { scope1, scope2, scope3 };
        data.scopeData = scopes;

        return data;
    }
}
