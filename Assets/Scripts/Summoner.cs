using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;
using VectorExtensions;
using ArrayExtensions;

public class Summoner : MonoBehaviour
{
    public GameObject[] summonedEntities;
    public int numEnemiesToSummon;
    public int maxRadius;
    public int minRadius;

    public bool isSummoning { get; private set; }

    private CollisionMap mCollisionMap;
    public float castSpeed = 4f;

    private void Start()
    {
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
    }

    public void CastSummon()
    {
        StartCoroutine(CastSummonCoroutine());
    }

    public IEnumerator CastSummonCoroutine()
    {
        // todo bdsowers - UGH
        SimpleMovement root = GetComponentInParent<SimpleMovement>();
        if (root != null && !DancePartyQuirk.quirkEnabled)
        {
            root.GetComponentInChildren<Animator>().Play("Spell");
        }

        isSummoning = true;

        List<Vector2Int> summonLocations = LockDownSummonLocations();

        yield return new WaitForSeconds(0.45f);

        Summon(summonLocations);

        yield return new WaitForSeconds(2f);

        isSummoning = false;

        yield break;
    }

    private List<Vector2Int> LockDownSummonLocations()
    {
        Vector2Int pos = transform.position.AsVector2IntUsingXZ();
        pos.y = -pos.y;

        List<Vector2Int> walkablePositions = WalkablePositionsInRange(pos.x, pos.y);
        List<Vector2Int> summonLocations = new List<Vector2Int>();
        for (int i = 0; i < numEnemiesToSummon; ++i)
        {
            if (walkablePositions.Count == 0)
                continue;

            Vector2Int randomPos = walkablePositions.Sample();
            summonLocations.Add(randomPos);
            walkablePositions.Remove(randomPos);
            mCollisionMap.MarkSpace(randomPos.x, randomPos.y, 4);

            Vector3 summonWorldPos = new Vector3(randomPos.x, 0.5f, -randomPos.y);
            GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX3_MagicAura_B_Runic");
            vfx.GetComponentInChildren<ParticleSystem>().playbackSpeed = 2.5f;
            vfx.transform.position = summonWorldPos + Vector3.up * 0.1f;
            vfx.transform.localScale = Vector3.one * 0.5f;
            vfx.AddComponent<DestroyAfterTimeElapsed>().time = 2f;
        }

        return summonLocations;
    }

    private void Summon(List<Vector2Int> locations)
    {
        for (int i = 0; i < locations.Count; ++i)
        {
            SummonEnemy(locations[i]);
        }
    }

    private void SummonEnemy(Vector2Int mapPos)
    {
        string enemy = summonedEntities.Sample().name;

        GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName(enemy));
        Vector2Int pos2 = mapPos;
        Vector3 pos = new Vector3(pos2.x, 0.5f, -pos2.y);
        newEnemy.transform.position = pos;
    }

    private List<Vector2Int> WalkablePositionsInRange(int originX, int originY)
    {
        List<Vector2Int> walkablePositions = new List<Vector2Int>();

        for (int xOffset = -maxRadius; xOffset <= maxRadius; ++xOffset)
        {
            for (int yOffset = -maxRadius; yOffset <= maxRadius; ++yOffset)
            {
                int orthogonalDistance = Mathf.Abs(xOffset) + Mathf.Abs(yOffset);
                if (orthogonalDistance == 0 || orthogonalDistance < minRadius || orthogonalDistance > maxRadius)
                    continue;

                int x = originX + xOffset;
                int y = originY + yOffset;

                if (x < 0 || y < 0 || x >= mCollisionMap.width || y >= mCollisionMap.height)
                    continue;

                if (mCollisionMap.SpaceMarking(x, y) == 0)
                {
                    walkablePositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return walkablePositions;
    }
}
