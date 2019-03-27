using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    public string preActivationMessage;

    private bool mActivated;

    public bool activated
    {
        get { return mActivated; }
    }

    public void Activate()
    {
        if (mActivated)
            return;

        mActivated = true;

        StartCoroutine(ActivationCoroutine());
    }

    protected bool WasAccepted()
    {
        return Game.instance.cinematicDirector.dataProvider.GetStringData(null, "dialog_choice") == "yes";
    }

    protected virtual IEnumerator ActivationCoroutine()
    {
        if (!string.IsNullOrEmpty(preActivationMessage))
        {
            Game.instance.cinematicDataProvider.SetData("shrine_message", preActivationMessage);
            Game.instance.cinematicDirector.PostCinematicEvent("activate_shrine");

            while (Game.instance.cinematicDirector.IsCinematicPlaying())
            {
                yield return null;
            }
        }

        yield break;
    }
}
