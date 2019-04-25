using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public Image displayTemplate;

    private List<GameObject> mItems = new List<GameObject>();

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

        Item[] items = Game.instance.playerStats.GetComponentsInChildren<Item>();
        for (int i = 0; i < items.Length; ++i)
        {
            SpriteRenderer sr = items[i].GetComponentInChildren<SpriteRenderer>();
            Sprite sprite = sr.sprite;

            Image newDisplay = GameObject.Instantiate(displayTemplate, transform);
            newDisplay.gameObject.SetActive(true);
            newDisplay.sprite = sprite;
            mItems.Add(newDisplay.gameObject);
        }
    }
}
