using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PurchasePrompt : MonoBehaviour
{
    public Text title;
    public Text description;
    public Text cost;
    public Image image;

    public GameObject purchaseContainer;
    public GameObject notEnoughMoneyContainer;

    private Item mCurrentItem = null;

    public void ShowForItem(Item item)
    {
        if (item == mCurrentItem)
            return;

        mCurrentItem = item;

        gameObject.SetActive(true);

        title.text = LocalizedText.Get(item.friendlyName);
        description.text = LocalizedText.Get(item.description);
        cost.text = item.Cost().ToString();
        image.sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;

        transform.localScale = Vector3.zero;

        transform.DOKill();
        transform.DOScale(1f, 0.3f);

        bool enoughMoney = Game.instance.playerData.numCoins >= item.Cost();
        purchaseContainer.SetActive(enoughMoney);
        notEnoughMoneyContainer.SetActive(!enoughMoney);
    }

    public void Hide()
    {
        // It's already hidden
        if (!IsOpen())
            return;

        mCurrentItem = null;

        transform.DOKill();
        transform.DOScale(0f, 0.3f);
    }

    public bool IsOpen()
    {
        return mCurrentItem != null;
    }
}
