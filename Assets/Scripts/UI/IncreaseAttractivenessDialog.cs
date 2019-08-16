using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArrayExtensions;

public class IncreaseAttractivenessDialog : Dialog
{
    public Text messageLabel;
    public Text costLabel;

    private int mCost;

    private string mSuccessEvent;
    private string mFailEvent;

    private string mEffect;

    public void ShowDialog(string message, int cost, string effect, string successEvent, string failEvent)
    {
        gameObject.SetActive(true);

        mCost = cost;
        costLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(mCost).ToString();

        messageLabel.text = PigLatinQuirk.ApplyQuirkIfPresent(message);

        mSuccessEvent = successEvent;
        mFailEvent = failEvent;
        mEffect = effect;
    }

    public void OnYesPressed()
    {
        if (Game.instance.playerData.numHearts >= mCost)
        {
            Game.instance.playerData.numHearts -= mCost;

            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 0.7f, mCost, NumberPopupReason.RemoveHearts);

            ApplyEffect();

            Game.instance.cinematicDirector.PostCinematicEvent(mSuccessEvent);

            Game.instance.soundManager.PlaySound("confirm_special");
        }
        else
        {
            Game.instance.cinematicDirector.PostCinematicEvent(mFailEvent);
        }

        Close();
    }

    private void ApplyEffect()
    {
        if (mEffect == "shuffle_matches")
        {
            Game.instance.companionBuilder.BuildCompanionSet();
            QuestR.seenMatches = false;
        }
        else if (mEffect == "random_item")
        {
            // todo bdsowers - add something to Item to make this list auto-update
            List<GameObject> itemsToIgnore = new List<GameObject>
            {
                PrefabManager.instance.PrefabByName("Map"),
                PrefabManager.instance.PrefabByName("Health Potion"),
                PrefabManager.instance.PrefabByName("Greater Health Potion"),
            };
            
            GameObject newItem = GameObject.Instantiate(PrefabManager.instance.itemPrefabs.Sample(itemsToIgnore));
            newItem.GetComponentInChildren<Item>().Equip();
        }
    }

    public void OnNoPressed()
    {
        Close();
    }
}
