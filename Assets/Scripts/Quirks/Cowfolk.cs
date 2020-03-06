using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Cowfolk : Quirk
{
    Sepia mSepia;
    PostProcessVolume mVolume;
    GameObject mTumbleweed;

    private float mTumbleweedTimer = 0f;
    private float mTumbleweedTime = 5f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // If the player has the monocle, vision-based quirks don't kick in
        if (Game.instance.playerStats.IsItemEquipped<Monocle>())
            return;

        mSepia = ScriptableObject.CreateInstance<Sepia>();
        mSepia.enabled.Override(true);

        mVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("VFXVolume"), 100f, mSepia);

        VFXVolumeCreated();

        mTumbleweed = Game.instance.cinematicDirector.objectMap.GetObjectByName("tumbleweed");
    }

    private void Update()
    {
        if (mTumbleweed == null) return;

        mTumbleweedTimer += Time.deltaTime;
        if (mTumbleweedTimer >= mTumbleweedTime)
        {
            mTumbleweedTimer = 0f;
            mTumbleweedTime = Random.Range(5f, 30f);

            StartCoroutine(PlayTumbleweedAnimation());
        }
    }

    private IEnumerator PlayTumbleweedAnimation()
    {
        mTumbleweed.SetActive(true);
        mTumbleweed.transform.localScale = Vector3.one * Random.Range(1f, 3f);

        float y = Random.Range(-Screen.height / 2, Screen.height / 2);
        Vector3 startPosition = new Vector3(-Screen.width / 2 - 300f, y, 0);
        Vector3 endPosition = new Vector3(Screen.width / 2 + 300f, y + Random.Range(-10f, 10f), 0f);
        mTumbleweed.transform.localPosition = startPosition;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 0.25f;
            mTumbleweed.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        mTumbleweed.SetActive(false);

        yield break;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        DestroyVolume();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        DestroyVolume();
    }

    public override void DestroyVolume()
    {
        if (mVolume == null)
            return;

        RuntimeUtilities.DestroyVolume(mVolume, true, true);
        mVolume = null;

        VFXVolumeDestroyed();
    }
}
