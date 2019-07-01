using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineBomb : Shrine
{
    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        if (WasAccepted())
        {

        }
    }
}
