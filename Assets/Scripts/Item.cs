using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public delegate void ItemEquipped(Item item);
    public event ItemEquipped onEquip;

    public string friendlyName;
    public string description;
    public int baseCost;

    public void Equip()
    {
        // Add this to the persistent object so it survives between dungeon levels
        transform.SetParent(Game.instance.playerStats.transform);

        // Turn off all renderers so it no longer displays
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = false;
        }

        // Add this guy's sprite to the inventory UI
        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();

        if (onEquip != null)
        {
            onEquip(this);
        }
    }
}
