using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : SingleUseItem
{
    public int amount;

    protected override void OnUse()
    {
        Game.instance.playerData.health += amount;
        Game.instance.playerData.health = Mathf.Min(Game.instance.playerData.health, Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject));

        NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 1f, amount, NumberPopupReason.Heal);
    }
}
