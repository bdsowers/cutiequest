using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
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
}
