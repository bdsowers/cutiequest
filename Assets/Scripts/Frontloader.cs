using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Frontloader : MonoBehaviour
{
    public GameObject logo;

    public IEnumerator Start()
    {
        ResolutionManager.SetupResolution();

        StartCoroutine(PlayLogoAnimation());

        yield return new WaitForSeconds(2.5f);

        Game.instance.transitionManager.TransitionToScreen("Title");
    }

    public IEnumerator PlayLogoAnimation()
    {
        logo.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.2f);

        logo.transform.DOScale(Vector3.one, 0.8f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            logo.transform.Rotate(0f, 0f, -360f * 3f * Time.deltaTime);
            yield return null;
        }

        logo.transform.rotation = Quaternion.identity;

        yield break;
    }
}
