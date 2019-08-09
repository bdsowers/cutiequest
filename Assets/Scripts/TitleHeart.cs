using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleHeart : MonoBehaviour
{
    private float mDelay = 0f;
    private float mAnimationTimer = 0f;
    private float mStartRotation = 0f;

    private float mDisappearTimer = -1f;
    private float mOriginalScale;

    // Start is called before the first frame update
    void Start()
    {
        mDelay = Random.Range(0f, 20f);
        mStartRotation = 0f;
        mOriginalScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (mDisappearTimer >= 0f)
        {
            mDisappearTimer += Time.deltaTime * 3f;
            mDisappearTimer = Mathf.Min(1f, mDisappearTimer);
            transform.localScale = Vector3.Lerp(Vector3.one * mOriginalScale, Vector3.zero, mDisappearTimer);
        }
        else if (mDelay >= 0f)
        {
            mDelay -= Time.deltaTime;
            mAnimationTimer = 0f;
            mStartRotation = transform.rotation.eulerAngles.y;
        }
        else
        {
            mAnimationTimer += Time.deltaTime;

            return;

            if (mAnimationTimer > 1f)
            {
                mAnimationTimer = 1f;
                mDelay = Random.Range(0f, 20f);
            }

            transform.localRotation = Quaternion.Euler(0f, mStartRotation + 180f * mAnimationTimer, 0f);

            float scale = mAnimationTimer * 2f;
            if (mAnimationTimer > 0.5f)
                scale = (1f - mAnimationTimer) * 2f;

            //transform.localScale = Vector3.one * (3f + scale * 0.6f);
        }
        
    }

    public void MakeDisappear()
    {
        mDisappearTimer = 0.01f;
    }
}
