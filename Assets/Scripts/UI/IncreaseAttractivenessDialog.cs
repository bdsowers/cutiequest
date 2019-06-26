using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseAttractivenessDialog : MonoBehaviour
{
    public Text costLabel;

    private int mCost;

    public void ShowDialog()
    {
        gameObject.SetActive(true);

        mCost = 3;
        costLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(mCost).ToString();
    }

    public void OnYesPressed()
    {
        if (Game.instance.playerData.numHearts >= mCost)
        {
            Game.instance.playerData.numHearts -= mCost;

            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 0.7f, mCost, NumberPopupReason.RemoveHearts);

            Follower currentFollower = GameObject.FindObjectOfType<Follower>();
            if (currentFollower != null)
                currentFollower.GetComponentInChildren<CharacterModel>().RemoveModel();

            Game.instance.companionBuilder.BuildCompanionSet();

            Game.instance.cinematicDirector.PostCinematicEvent("stylist_success");
        }
        else
        {
            Game.instance.cinematicDirector.PostCinematicEvent("stylist_fail");
        }

        gameObject.SetActive(false);
    }

    public void OnNoPressed()
    {
        gameObject.SetActive(false);
    }
}
