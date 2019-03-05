using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;

public class LevelGenerator : MonoBehaviour
{
    RandomDungeon mDungeon;
    RandomDungeonGenerator mDungeonGenerator;
    RoomSet mRoomset;

    private void Start()
    {
        mDungeonGenerator = new RandomDungeonGenerator();
        TextAsset roomSetData = Resources.Load<TextAsset>("RoomSets/roomset6");
        mRoomset = new RoomSet();
        mRoomset.LoadFromTextAsset(roomSetData);

        mDungeon = mDungeonGenerator.GenerateDungeon(mRoomset, DungeonGenerationData());

        GenerateEnvironmentFromDungeon(mDungeon);
        PlaceAvatar();
        PlaceEnemies();
    }

    private void GenerateEnvironmentFromDungeon(RandomDungeon dungeon)
    {
        for (int x = 0; x < dungeon.width; ++x)
        {
            for (int y = 0; y < dungeon.height; ++y)
            {
                if (dungeon.TileType(x, y) == RandomDungeonTileData.WALL_TILE)
                {
                    GameObject newWall = GameObject.Instantiate(PrefabManager.instance.PrefabByName("StandardWall"));
                    newWall.transform.SetParent(transform);
                    newWall.transform.position = new Vector3(x, 0f, -y);
                }
                else if (dungeon.TileType(x, y) == RandomDungeonTileData.WALKABLE_TILE ||
                    dungeon.TileType(x, y) == RandomDungeonTileData.EXIT_TILE)
                {
                    GameObject newFloor = GameObject.Instantiate(PrefabManager.instance.PrefabByName("Floor"));
                    newFloor.transform.SetParent(transform);
                    newFloor.transform.position = new Vector3(x, 0f, -y);
                }
            }
        }
    }

    private void PlaceAvatar()
    {
        GameObject avatar = GameObject.Find("Avatar");
        Vector2Int pos = mDungeon.primaryPathPositions[0];
        avatar.transform.position = new Vector3(pos.x, 0.5f, -pos.y);
        avatar.GetComponent<PlayerController>().follower.transform.position = avatar.transform.position;
    }

    private void PlaceEnemies()
    {
        List<Vector2Int> walkablePositions = mDungeon.WalkableTilePositions();

        int numEnemies = 20;
        for (int i = 0; i < numEnemies; ++i)
        {
            GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName("Enemy"));
            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            Vector3 pos = new Vector3(pos2.x, 0.5f, -pos2.y);
            newEnemy.transform.position = pos;
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
        scope2.criticalPathMinRooms = 6;
        scope2.criticalPathMaxRooms = 10;

        RandomDungeonScopeData scope3 = new RandomDungeonScopeData();
        scope3.criticalPathMinRooms = 3;
        scope3.criticalPathMaxRooms = 6;

        RandomDungeonScopeData[] scopes = new RandomDungeonScopeData[] { scope1, scope2, scope3 };
        data.scopeData = scopes;

        return data;
    }
}
