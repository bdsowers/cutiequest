using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wiggle : MonoBehaviour
{
    private float mTweenInterval;

    // Start is called before the first frame update
    void Start()
    {
        mTweenInterval = Random.Range(1f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        mTweenInterval -= Time.deltaTime;
        if (mTweenInterval <= 0f)
        {
            mTweenInterval = Random.Range(1f, 4f);
            PlayWiggle();
        }
    }

    void PlayWiggle()
    {
        transform.DOShakeRotation(0.5f, new Vector3(0f, 0f, 30f));
    }
}
