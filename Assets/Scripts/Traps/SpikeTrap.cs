using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : PlacedTrap
{
    public GameObject spikes;

    private float mTimer = 2f;
    private bool mExtended = false;
    private bool mAnimationPlaying = false;

    public float timeOffset = 0f;

    
    private void Start()
    {
        mTimer += timeOffset;
    }

    private void Update()
    {
        mTimer -= Time.deltaTime;
        if (mTimer < 0f)
        {
            mTimer = 2f;
            StartCoroutine(MoveSpikes());
        }
    }

    private IEnumerator MoveSpikes()
    {
        mAnimationPlaying = true;

        Vector3 currentPosition = spikes.transform.localPosition;
        Vector3 target = Vector3.up * 0.35f;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 6f;
            spikes.transform.localPosition = Vector3.Lerp(currentPosition, target, time);
            yield return null;
        }

        DealDamage();

        while (time > 0f)
        {
            time -= Time.deltaTime * 6f;
            spikes.transform.localPosition = Vector3.Lerp(currentPosition, target, time);
            yield return null;
        }

        spikes.transform.localPosition = Vector3.zero;

        mAnimationPlaying = false;

        yield break;
    }

    private void DealDamage()
    {
        Killable killable = KillableMap.instance.KillableAtWorldPosition(transform.position);
        if (killable != null && killable.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            killable.TakeDamage(15);
        }
    }

    public override bool CanSpawn(List<Vector2Int> region)
    {
        return true;
    }

    public override void Spawn(List<Vector2Int> region, LevelGenerator levelGenerator)
    {
        int spikeNum = 0;
        bool uniform = (Random.Range(0, 2) == 0);
        float direction = (Random.Range(0, 2) == 0 ? 1 : -1);

        // Generate the trap, making sure we're not generating out of turn
        for (int posIdx = 0; posIdx < region.Count; ++posIdx)
        {
            Vector2Int pos = region[posIdx];

            if (levelGenerator.collisionMap.SpaceMarking(pos.x, pos.y) == 0)
            {
                GameObject trapObj = levelGenerator.PlaceMapPrefab("SpikeTrap", pos.x, pos.y);
                SpikeTrap spikes = trapObj.GetComponent<SpikeTrap>();

                if (!uniform)
                    spikes.timeOffset = 0.25f * spikeNum * direction;

                ++spikeNum;
            }
        }
    }
}
