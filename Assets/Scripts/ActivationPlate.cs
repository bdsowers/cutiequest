using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPlate : MonoBehaviour
{
    public string cinematicEvent;

    public Item item;
    public Shrine shrine;

    private bool mIsPlayerInside;

    // todo bdsowers - gets the job done but is a pretty ugly hack
    private static bool mPlayerInsideAny = false;
    private static bool mLateUpdateProcessed = false;

    // Update is called once per frame
    void Update()
    {
        if (!Game.instance.avatar.isAlive)
            return;
        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return;
        if (DialogManager.AnyDialogsOpen())
            return;

        mLateUpdateProcessed = false;
        mPlayerInsideAny = (mIsPlayerInside || mPlayerInsideAny);
        
        if (Game.instance.actionSet.Activate.WasPressed)
        {
            if (mIsPlayerInside)
            {
                if (!string.IsNullOrEmpty(cinematicEvent))
                {
                    Game.instance.cinematicDirector.PostCinematicEvent(cinematicEvent);
                }
                else if (item != null)
                {
                    // Equip the item
                    if (item.Cost() <= Game.instance.playerData.numCoins)
                    {
                        Game.instance.playerData.numCoins -= item.Cost();

                        NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 0.7f, item.Cost(), NumberPopupReason.RemoveCoins);

                        item.Equip();

                        Game.instance.soundManager.PlaySound("confirm_special");
                    }
                }
                else if (shrine != null)
                {
                    if (!shrine.activated)
                    {
                        shrine.Activate();
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (mLateUpdateProcessed)
            return;

        mLateUpdateProcessed = true;
        Game.instance.avatar.buttonPromptCanvas.SetActive(mPlayerInsideAny);
        mPlayerInsideAny = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // todo bdsowers - I've never thought this method of detecting if we're inside a trigger was safe...
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            mIsPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            mIsPlayerInside = false; 
        }
    }
}
