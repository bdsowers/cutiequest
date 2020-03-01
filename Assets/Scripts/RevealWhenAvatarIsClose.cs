using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealWhenAvatarIsClose : MonoBehaviour
{
    public bool allowScaleVariation = true;
    public bool revealDelay = true;

    private bool mRevealed = false;
    private bool mFullyRevealed = false;

    public bool fullyRevealed {  get { return mFullyRevealed; } }

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (mFullyRevealed)
            return;

        float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);
        if (!mRevealed && distance < NearsightedQuirk.ApplyQuirkIfPresent(6.5f))
        {
            StartUpdateReveal();
        }

        UpdateReveal();
    }

    // Reveal without garbage-intensive coroutines
    private float mRevealDelayTimer = 0f;
    private Vector3 mStartScale;
    private Vector3 mTargetScale;
    private float mRevealTimer = 0f;
    private float mOvershoot = 0.2f;
    private float mRevealSpeed = 3f;
    private int mRevealState = 0;
    void StartUpdateReveal()
    {
        mRevealed = true;

        mRevealDelayTimer = Random.Range(0f, 0.5f);
        if (!revealDelay)
        {
            mRevealDelayTimer = 0f;
        }

        mStartScale = Vector3.zero;
        mTargetScale = Vector3.one;
        if (allowScaleVariation)
        {
            mTargetScale.y = Random.Range(0.9f, 1.2f);
        }
    }

    void UpdateReveal()
    {
        if (mRevealed && !mFullyRevealed)
        {
            if (mRevealDelayTimer > 0f)
            {
                mRevealDelayTimer -= Time.deltaTime;
                return;
            }

            if (mRevealState == 0)
            {
                if (mRevealTimer < 1f + mOvershoot)
                {
                    mRevealTimer += Time.deltaTime * mRevealSpeed;
                    transform.localScale = mStartScale + (mTargetScale - mStartScale) * mRevealTimer;
                }
                else
                {
                    mRevealState = 1;
                }
            }
            else if (mRevealState == 1)
            {
                if (mRevealTimer > 1f)
                {
                    mRevealTimer -= Time.deltaTime * mRevealSpeed;
                    if (mRevealTimer < 1f) mRevealTimer = 1f;
                    transform.localScale = mStartScale + (mTargetScale - mStartScale) * mRevealTimer;
                }
                else
                {
                    transform.localScale = mTargetScale;
                    mFullyRevealed = true;
                    enabled = false;
                }
            }
        }
    }

    public void Reveal()
    {
        if (mRevealed)
            return;

        mRevealed = true;

        StartCoroutine(RevealAnimation());
    }

    private IEnumerator RevealAnimation()
    {
        float delay = Random.Range(0f, 0.5f);
        if (!revealDelay)
            delay = 0f;

        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        if (allowScaleVariation)
            targetScale.y = Random.Range(0.9f, 1.2f);

        float overshoot = 0.2f;
        float time = 0f;
        float speed = 3f;
        while (time < 1f + overshoot)
        {
            time += Time.deltaTime * speed;
            transform.localScale = startScale + (targetScale - startScale) * time;
            yield return null;
        }

        while (time > 1.0f)
        {
            time -= Time.deltaTime * speed;
            if (time < 1f) time = 1f;
            transform.localScale = startScale + (targetScale - startScale) * time;
            yield return null;
        }

        transform.localScale = targetScale;

        mFullyRevealed = true;

        enabled = false;

        yield break;
    }
}
