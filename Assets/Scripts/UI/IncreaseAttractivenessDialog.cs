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

            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, mCost, NumberPopupReason.RemoveHearts);

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
            List<GameObject> itemsToIgnore = DungeonOnlyItems();

            GameObject newItem = GameObject.Instantiate(PrefabManager.instance.itemPrefabs.Sample(itemsToIgnore));
            newItem.GetComponentInChildren<Item>().Equip();
        }
    }

    private List<GameObject> DungeonOnlyItems()
    {
        List<GameObject> items = new List<GameObject>();

        for (int i = 0; i < PrefabManager.instance.itemPrefabs.Length; ++i)
        {
            GameObject prefab = PrefabManager.instance.itemPrefabs[i];
            Item item = prefab.GetComponent<Item>();
            if (item != null && item.dungeonOnly)
            {
                items.Add(prefab);
            }
        }

        return items;
    }

    public void OnNoPressed()
    {
        Close();
    }
}
