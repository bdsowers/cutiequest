using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public Image displayTemplate;

    private List<GameObject> mItems = new List<GameObject>();

    private List<Quirk> mQuirkList = new List<Quirk>();
    private List<Item> mItemList = new List<Item>();

    private void Start()
    {
        Refresh();
    }

    private void Clear()
    {
        for (int i = 0; i < mItems.Count; ++i)
        {
            Destroy(mItems[i]);
        }

        mItems.Clear();
    }

    public void Refresh()
    {
        Clear();

        mQuirkList.Clear();
        mItemList.Clear();

        // Quirks first - both player & follower
        if (Game.instance.avatar != null)
        {
            Game.instance.avatar.GetComponentsInChildren<Quirk>(mQuirkList);
            for (int i = 0; i < mQuirkList.Count; ++i)
            {
                Sprite sprite = mQuirkList[i].icon;

                AddToInventory(sprite);
            }
        }

        mQuirkList.Clear();
        if (Game.instance.followerData != null && Game.instance.avatar.follower != null)
        {
            Game.instance.avatar.follower.GetComponentsInChildren<Quirk>(mQuirkList);
            for (int i = 0; i < mQuirkList.Count; ++i)
            {
                Sprite sprite = mQuirkList[i].icon;

                AddToInventory(sprite);
            }
        }

        // And now items!
        Game.instance.playerStats.GetComponentsInChildren<Item>(mItemList);
        for (int i = 0; i < mItemList.Count; ++i)
        {
            SpriteRenderer sr = mItemList[i].GetComponentInChildren<SpriteRenderer>();
            Sprite sprite = sr.sprite;

            AddToInventory(sprite);
        }
    }

    void AddToInventory(Sprite sprite)
    {
        Image newDisplay = GameObject.Instantiate(displayTemplate, transform);
        newDisplay.gameObject.SetActive(true);
        newDisplay.sprite = sprite;
        mItems.Add(newDisplay.gameObject);
    }
}
