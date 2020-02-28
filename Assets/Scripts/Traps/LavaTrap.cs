using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTrap : PlacedTrap
{
    public GameObject spikes;

    private static float mTimer = 0f;
    private static bool mInAnyLava;
    private static bool mLateUpdateProcessed;

    private void Start()
    {
    }

    private void Update()
    {
        mLateUpdateProcessed = false;

        Killable killable = KillableMap.instance.KillableAtWorldPosition(transform.position);
        if (killable != null && killable.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            mInAnyLava = true;

            mTimer += Time.deltaTime;
            if (mTimer > 0.75f)
            {
                if (!Game.instance.playerStats.IsItemEquipped<IronBoots>())
                {
                    killable.TakeDamage(gameObject, 5, DamageReason.Trap);
                }

                mTimer = 0f;
            }
        }
    }

    private void LateUpdate()
    {
        if (mLateUpdateProcessed)
            return;

        if (!mInAnyLava)
        {
            mTimer = 0f;
        }

        mInAnyLava = false;
        mLateUpdateProcessed = true;
    }

    public override bool CanSpawn(List<Vector2Int> region)
    {
        return true;
    }

    public override void Spawn(List<Vector2Int> region, LevelGenerator levelGenerator)
    {
        // Generate the trap, always leaving at least one space as a hole
        int hole = Random.Range(0, region.Count);
        for (int posIdx = 0; posIdx < region.Count; ++posIdx)
        {
            if (posIdx == hole)
                continue;

            Vector2Int pos = region[posIdx];

            if (levelGenerator.collisionMap.SpaceMarking(pos.x, pos.y) == 0)
            {
                GameObject trapObj = levelGenerator.PlaceMapPrefab("LavaTrap", pos.x, pos.y);
                LavaTrap razers = trapObj.GetComponent<LavaTrap>();
            }
        }
    }
}
