using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class EnemyTeleport : MonoBehaviour
{
    public float teleportCooldown = 5f;
    public int teleportMinDistance = 4;
    public int teleportMaxDistance = 6;
    private float teleportTimer;

    private void Start()
    {
        teleportTimer = Random.Range(0f, teleportCooldown);
    }

    private bool CanTeleport()
    {
        return teleportTimer <= 0f;
    }

    public bool ShouldTeleport()
    {
        return CanTeleport() && Vector3.Distance(Game.instance.avatar.transform.position, transform.position) < 3f;
    }

    public void Teleport()
    {
        teleportTimer = teleportCooldown;

        CollisionMap collisionMap = GameObject.FindObjectOfType<CollisionMap>();

        Vector2Int pos = MapCoordinateHelper.WorldToMapCoords(transform.position);

        List<Vector2Int> viablePositions = new List<Vector2Int>();
        for (int xOffset = -teleportMaxDistance; xOffset <= teleportMaxDistance; ++xOffset)
        {
            for (int yOffset = -teleportMaxDistance; yOffset <= teleportMaxDistance; ++yOffset)
            {
                int teleDist = Mathf.Abs(xOffset) + Mathf.Abs(yOffset);
                if (teleDist < teleportMinDistance || teleDist > teleportMaxDistance)
                {
                    continue;
                }

                int testX = pos.x + xOffset;
                int testY = pos.y + yOffset;
                if (testX >= 0 && testY >= 0 && testX < collisionMap.width && testY < collisionMap.height &&
                    collisionMap.SpaceMarking(testX, testY) == 0)
                {
                    viablePositions.Add(new Vector2Int(testX, testY));
                }
            }
        }

        if (viablePositions.Count == 0)
            return;


        Vector2Int targetPos = viablePositions.Sample();

        collisionMap.RemoveMarking(GetComponent<SimpleMovement>().uniqueCollisionIdentity);
        collisionMap.MarkSpace(targetPos.x, targetPos.y, GetComponent<SimpleMovement>().uniqueCollisionIdentity);

        Vector3 previousPosition = transform.position;
        transform.position = new Vector3(targetPos.x, 0, -targetPos.y);

        GameObject effect = PrefabManager.instance.InstantiatePrefabByName("CFX2_WWExplosion_C");
        effect.transform.position = previousPosition;
        effect.AddComponent<DestroyAfterTimeElapsed>().time = 2f;
        effect.transform.localScale = Vector3.one * 0.75f;

        GameObject effect2 = PrefabManager.instance.InstantiatePrefabByName("CFX2_WWExplosion_C");
        effect2.transform.position = transform.position;
        effect2.AddComponent<DestroyAfterTimeElapsed>().time = 2f;
        effect2.transform.localScale = Vector3.one * 0.75f;
    }

    private void Update()
    {
        if (teleportTimer > 0f)
            teleportTimer -= Time.deltaTime;
    }
}
