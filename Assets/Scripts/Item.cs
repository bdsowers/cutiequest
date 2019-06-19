using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public delegate void ItemEquipped(Item item);
    public event ItemEquipped onEquip;

    public string friendlyName;
    public string description;

    public int baseCost;
    private int progressSpecificCost = -1;

    private void Start()
    {
        // todo bdsowers - ew
        gameObject.AddComponent<RevealWhenAvatarIsClose>().allowScaleVariation = false;

        GameObject costCanvas = GameObject.Instantiate(PrefabManager.instance.PrefabByName("CostCanvas"), transform);
        Text[] labels = costCanvas.GetComponentsInChildren<Text>();
        for (int i = 0; i < labels.Length; ++i)
        {
            labels[i].text = BadAtMathQuirk.ApplyQuirkIfPresent(Cost()).ToString();
        }
    }

    public int Cost()
    {
        // todo bdsowers - factor luck into the cost

        if (progressSpecificCost < 0)
        {
            // Items get more expensive the further into the dungeon you go.
            // Plus there's always some variability.
            progressSpecificCost = Game.instance.currentDungeonFloor * 15 + Random.Range(-15, 50);

            if (ExpensiveTastesQuirk.isQuirkPresent)
            {
                progressSpecificCost += ExpensiveTastesQuirk.CostModification();
            }
        }

        return baseCost + progressSpecificCost;
    }

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

        GetComponentInChildren<Canvas>().gameObject.SetActive(false);

        // Add this guy's sprite to the inventory UI
        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();

        if (onEquip != null)
        {
            onEquip(this);
        }
    }
}
