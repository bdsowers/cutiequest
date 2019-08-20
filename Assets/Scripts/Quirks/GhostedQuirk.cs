using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;
using VectorExtensions;

public class GhostedQuirk : Quirk
{
    public void SpawnGhosts(RandomDungeon dungeon, CollisionMap collisionMap, Vector2Int avatarStartPosition)
    {
        List<Vector2Int> walkablePositions = collisionMap.EmptyPositions();
        walkablePositions.RemoveAll((pos) => VectorHelper.OrthogonalDistance(pos, avatarStartPosition) < 10);

        // Spawn a few cops in the level
        int numGhosts = Random.Range(10, 20);
        for (int i = 0; i < numGhosts; ++i)
        {
            if (walkablePositions.Count == 0)
                return;

            GameObject newGhost = GameObject.Instantiate(PrefabManager.instance.PrefabByName("Ghost"));
            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            walkablePositions.Remove(pos2);
            Vector3 pos = MapCoordinateHelper.MapToWorldCoords(pos2);
            newGhost.transform.position = pos;
        }
    }

    public override void Start()
    {
        base.Start();

        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
