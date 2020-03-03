using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneButton : MonoBehaviour
{
    public GameObject phoneInterface;
    public GameObject newMatchesIndicator;
    public Wiggle mWiggle;

    private void Start()
    {
        mWiggle = GetComponentInParent<Wiggle>();
    }

    public void OnPressed()
    {
        phoneInterface.gameObject.SetActive(true);
    }

    private void Update()
    {
        mWiggle.enabled = !QuestR.seenMatches;
        newMatchesIndicator.SetActive(!QuestR.seenMatches);

        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return;

        if (Game.instance.actionSet.ToggleMap.WasPressed && Game.instance.CanOpenUI())
        {
            if (Game.instance.dialogManager != null && !Game.instance.dialogManager.AnyDialogsOpen())
            {
                OnPressed();
            }
        }
    }
}
