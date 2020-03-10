using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirismStatusEffect : StatusEffect
{
    public int strength { get; set; }

    public override void OnAdded()
    {
        base.OnAdded();

        duration = 5f;

        Game.instance.centralEvents.onEnemyHit += OnEnemyHit;
    }

    private void OnEnemyHit(Enemy enemy, int damage, DamageReason damageReason)
    {
        float multiplier = 0.05f + (strength / 200f);
        multiplier = Mathf.Min(multiplier, 0.3f);

        int amount = Mathf.RoundToInt(damage * multiplier);
        amount = Mathf.Max(amount, 1);

        // Cap vampirism at 10 to prevent it getting too powerful & to fix an insta-kill bug
        amount = Mathf.Min(amount, 10);

        Game.instance.playerData.health += amount;
        Game.instance.playerData.health = Mathf.Min(Game.instance.playerData.health, Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject));
        Game.instance.avatar.GetComponent<Killable>().health = Game.instance.playerData.health;

        NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, amount, NumberPopupReason.Good);
    }

    public override void OnRemoved()
    {
        Game.instance.centralEvents.onEnemyHit -= OnEnemyHit;

        base.OnRemoved();
    }
}
