using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorExtensions;

public class HitFlash : MonoBehaviour
{
    private bool mIsFlashing;

    public void Flash()
    {
        if (mIsFlashing)
            return;

        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        mIsFlashing = true;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 4f;
            float alpha = time * 2f;
            if (time > 0.5f)
                alpha = (1f - time) * 2f;

            alpha *= alpha;

            canvasGroup.alpha = alpha;
            yield return null;
        }

        mIsFlashing = false;

        yield break;
    }
}
