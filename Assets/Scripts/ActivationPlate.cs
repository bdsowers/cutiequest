using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class ActivationPlate : MonoBehaviour
{
    public string cinematicEvent;

    public Item item;
    public Shrine shrine;

    private bool mIsPlayerInside;

    // todo bdsowers - gets the job done but is a pretty ugly hack
    private static bool mCanActivateAny = false;
    private static bool mLateUpdateProcessed = false;

    private GameObject mLink = null;
    private bool mHasLink = false;

    private Shrine mLinkedShrine;

    public Vector3 activationDirection;

    public GameObject link { get { return mLink; } }

    public bool CanBeActivated()
    {
        if (!mIsPlayerInside)
            return false;

        if ((Game.instance.avatar.direction - activationDirection).magnitude < 0.01f)
            return true;

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Game.instance.avatar.isAlive)
            return;
        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return;
        if (DialogManager.AnyDialogsOpen())
            return;

        if (mHasLink)
        {
            if (mLink == null)
            {
                gameObject.SetActive(false);
            }
            else if (mLinkedShrine != null && mLinkedShrine.activated)
            {
                gameObject.SetActive(false);
            }
            else if (item != null && item.equipped)
            {
                gameObject.SetActive(false);
            }

            if (!gameObject.activeSelf)
                return;
        }

        mLateUpdateProcessed = false;
        mCanActivateAny = (CanBeActivated() || mCanActivateAny);
        
        if (Game.instance.quirkRegistry.IsQuirkActive<KleptoQuirk>() && mIsPlayerInside && item != null)
        {
            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, "Your partner stole " + item.friendlyName, NumberPopupReason.Bad);

            item.Equip();

            Game.instance.soundManager.PlaySound("confirm_special");

            Game.instance.MakeShopKeeperEnemy();
        }

        if (Game.instance.actionSet.Activate.WasPressed)
        {
            if (CanBeActivated())
            {
                if (!string.IsNullOrEmpty(cinematicEvent))
                {
                    Game.instance.cinematicDirector.PostCinematicEvent(cinematicEvent);
                }
                else if (item != null)
                {
                    AttemptPurchaseItem();
                }
                else if (shrine != null)
                {
                    AttemptShrineActivation();
                }
            }
        }
    }

    private void AttemptShrineActivation()
    {
        if (!shrine.activated)
        {
            shrine.Activate();
        }
    }

    private void AttemptPurchaseItem()
    {
        // Equip the item
        if (item.Cost() <= Game.instance.playerData.numCoins)
        {
            Game.instance.playerData.numCoins -= item.Cost();

            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, item.Cost(), NumberPopupReason.RemoveCoins);

            item.Equip();

            Game.instance.soundManager.PlaySound("confirm_special");
        }
    }

    private void LateUpdate()
    {
        if (mLateUpdateProcessed)
            return;

        mLateUpdateProcessed = true;
        Game.instance.avatar.buttonPromptCanvas.SetActive(mCanActivateAny);
        mCanActivateAny = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mIsPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mIsPlayerInside = false; 
        }
    }

    public void LinkToEntity(GameObject link)
    {
        mLink = link;
        mHasLink = true;

        mLinkedShrine = link.GetComponent<Shrine>();
    }
}
