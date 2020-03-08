using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Though it looks different, a treasure chest can be considered a shrine - it activates once
/// and has a one-time effect just like shrines.
/// </summary>
public class Chest : Shrine
{
    public GameObject lid;

    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        StartCoroutine(PlayOpenSequence());

        yield break;
    }

    private IEnumerator PlayOpenSequence()
    {
        Game.instance.soundManager.PlaySound("chest");

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 6f;

            lid.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(-50f, 0f, 0f), time);
            yield return null;
        }

        GetComponent<DropsItems>().Drop();

        yield break;
    }
}
