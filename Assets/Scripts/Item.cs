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

    public bool equipped { get; private set; }

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

            // Shop prices decrease when luck is high.
            int luck = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Luck, Game.instance.avatar.gameObject);
            progressSpecificCost -= Random.Range(0, luck / 2);
            progressSpecificCost = Mathf.Max(0, progressSpecificCost);

            if (Game.instance.quirkRegistry.IsQuirkActive<ExpensiveTastesQuirk>())
            {
                progressSpecificCost += ExpensiveTastesQuirk.CostModification();
            }
        }

        return Mathf.Max(1, baseCost + progressSpecificCost);
    }

    public void Equip()
    {
        if (equipped)
            return;

        equipped = true;

        // Add this to the persistent object so it survives between dungeon levels
        transform.SetParent(Game.instance.playerStats.transform);

        // Turn off all renderers so it no longer displays
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = false;
        }

        // Disable price canvas if one is showing
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
        
        // Add this guy's sprite to the inventory UI
        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();

        ShowEquipMessage();

        // Find any activation plates tied to this item and disable them
        ActivationPlate[] plates = GameObject.FindObjectsOfType<ActivationPlate>();
        for (int i = 0; i < plates.Length; ++i)
        {
            if (plates[i].item == this)
            {
                plates[i].gameObject.SetActive(false);
            }
        }

        if (onEquip != null)
        {
            onEquip(this);
        }
    }

    private void ShowEquipMessage()
    {
        string message = LocalizedText.Get(description);

        NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, message, NumberPopupReason.Good, 0.25f);
    }
}
