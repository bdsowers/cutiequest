using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;
using VectorExtensions;

public class PartnerInCrimeQuirk : Quirk
{
    public void SpawnCops(RandomDungeon dungeon, CollisionMap collisionMap, Vector2 avatarStartPosition)
    {
        List<Vector2Int> walkablePositions = collisionMap.EmptyPositions();
        walkablePositions.RemoveAll((pos) => VectorHelper.OrthogonalDistance(pos, avatarStartPosition) < 10);

        // Spawn a few cops in the level
        int numCops = Random.Range(3, 6);
        for (int i = 0; i < numCops; ++i)
        {
            if (walkablePositions.Count == 0)
                return;

            string enemy = (Random.Range(0, 2) == 0 ? "CopRanged" : "CopMelee");
            GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName(enemy));
            Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
            walkablePositions.Remove(pos2);
            Vector3 pos = MapCoordinateHelper.MapToWorldCoords(pos2);
            newEnemy.transform.position = pos;
            collisionMap.MarkSpace(pos2.x, pos2.y, newEnemy.GetComponent<SimpleMovement>().uniqueCollisionIdentity);
        }
    }
}
