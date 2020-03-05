using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;
using VectorExtensions;
using ArrayExtensions;

public class Summoner : CharacterComponentBase
{
    public GameObject[] summonedEntities;
    public int numEnemiesToSummon;
    public int maxRadius;
    public int minRadius;

    public bool isSummoning { get; private set; }

    private CollisionMap mCollisionMap;
    public float castSpeed = 4f;

    private int mNumLivingSummonedEnemies;

    private List<GameObject> mSummonedEntities = new List<GameObject>();

    private void Start()
    {
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
    }

    public int LivingSummonedEntities()
    {
        int sum = 0;
        for (int i = 0; i < mSummonedEntities.Count; ++i)
        {
            if (mSummonedEntities[i] != null)
            {
                sum++;
            }
        }
        return sum;
    }

    public void CastSummon()
    {
        StartCoroutine(CastSummonCoroutine());
    }

    public IEnumerator CastSummonCoroutine()
    {
        if (characterRoot != null && !Game.instance.quirkRegistry.IsQuirkActive<DancePartyQuirk>())
        {
            commonComponents.animator.Play("Spell");
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
        Vector2Int pos = MapCoordinateHelper.WorldToMapCoords(transform.position);

        List<Vector2Int> walkablePositions = WalkablePositionsInRange(pos.x, pos.y);
        List<Vector2Int> summonLocations = new List<Vector2Int>();
        for (int i = 0; i < numEnemiesToSummon; ++i)
        {
            if (walkablePositions.Count == 0)
                continue;

            Vector2Int randomPos = walkablePositions.Sample();
            summonLocations.Add(randomPos);
            walkablePositions.Remove(randomPos);

            Vector3 summonWorldPos = MapCoordinateHelper.MapToWorldCoords(randomPos);
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
        // Clear out the list of entities so it doesn't grow unbounded
        ClearDeadSummonedEntities();

        for (int i = 0; i < locations.Count; ++i)
        {
            SummonEnemy(locations[i]);
        }
    }

    private void SummonEnemy(Vector2Int mapPos)
    {
        // Only summon here if this position is still empty
        if (mCollisionMap.SpaceMarking(mapPos.x, mapPos.y) != 0)
            return;

        string enemy = summonedEntities.Sample().name;

        GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName(enemy), Game.instance.levelGenerator.transform);
        Vector2Int pos2 = mapPos;
        Vector3 pos = MapCoordinateHelper.MapToWorldCoords(pos2);
        newEnemy.transform.position = pos;
        mCollisionMap.MarkSpace(mapPos.x, mapPos.y, newEnemy.GetComponent<SimpleMovement>().uniqueCollisionIdentity);

        mSummonedEntities.Add(newEnemy);
    }

    private void ClearDeadSummonedEntities()
    {
        mSummonedEntities.RemoveAll(i => i == null);
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
