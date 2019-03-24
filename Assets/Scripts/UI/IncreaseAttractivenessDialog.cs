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

        int level = Game.instance.playerData.attractiveness;
        int nextLevel = level + 1;
        mCost = Mathf.RoundToInt(Mathf.Pow(3, nextLevel));
        costLabel.text = mCost.ToString();
    }

    public void OnYesPressed()
    {
        if (Game.instance.playerData.numHearts >= mCost)
        {
            Game.instance.playerData.attractiveness += 1;
            Game.instance.playerData.numHearts -= mCost;

            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 0.7f, mCost, NumberPopupReason.RemoveHearts);

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
