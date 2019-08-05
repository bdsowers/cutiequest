using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineGlassCannon : Shrine
{
    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        if (WasAccepted())
        {
            CharacterStatModifier attackModifier = Game.instance.playerStats.gameObject.AddComponent<CharacterStatModifier>();
            attackModifier.SetRelativeModification(CharacterStatType.Strength, 5);

            CharacterStatModifier defenseModifier = Game.instance.playerStats.gameObject.AddComponent<CharacterStatModifier>();
            defenseModifier.SetRelativeModification(CharacterStatType.Defense, -5);

            int currentMaxHealth = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
            CharacterStatModifier maxHealthModifier = Game.instance.playerStats.gameObject.AddComponent<CharacterStatModifier>();
            maxHealthModifier.SetRelativeModification(CharacterStatType.MaxHealth, -(currentMaxHealth/2));

            currentMaxHealth = currentMaxHealth / 2;
            if (Game.instance.playerData.health > currentMaxHealth)
            {
                Game.instance.playerData.health = currentMaxHealth;
                Game.instance.avatar.GetComponent<Killable>().health = Game.instance.playerData.health;
            }

            Game.instance.playerData.MarkDirty();
        }

        yield break;
    }
}
