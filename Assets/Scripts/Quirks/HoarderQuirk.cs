using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;
using ArrayExtensions;
using VectorExtensions;

public class HoarderQuirk : Quirk
{
    public void SpawnDebris(RandomDungeon dungeon, CollisionMap collisionMap, Vector2 avatarStartPosition)
    {
        List<Vector2Int> positions = collisionMap.EmptyPositions();
        positions.RemoveAll((pos) => VectorHelper.OrthogonalDistance(pos, avatarStartPosition) < 3);

        int numDebris = Random.Range(positions.Count / 5, positions.Count / 4);
        for (int i = 0; i < numDebris; ++i)
        {
            if (positions.Count == 0)
                return;

            Vector2Int pos = positions.Sample();

            // todo bdsowers - ensure that this position is valid
            GameObject newDebris = PrefabManager.instance.InstantiatePrefabByName(PrefabManager.instance.debrisPrefabs.Sample().name);
            newDebris.transform.position = MapCoordinateHelper.MapToWorldCoords(pos);
            collisionMap.MarkSpace(pos.x, pos.y, newDebris.GetComponent<SimpleMovement>().uniqueCollisionIdentity);
            positions.Remove(pos);
        }
    }
}
