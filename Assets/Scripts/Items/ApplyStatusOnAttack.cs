using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO bdsowers - this is hard-wired for use from the player, but that shouldn't
// be necessary.
public class ApplyStatusOnAttack : MonoBehaviour
{
    public Item parentItem;
    public float chance;
    public MonoBehaviour statusComponentTemplate;

    private void Start()
    {
        parentItem.onEquip += OnEquip;
    }

    private void OnEquip(Item item)
    {
        Game.instance.centralEvents.onEnemyHit += OnEnemyHitByMelee;
    }

    private void OnDestroy()
    {
        Game.instance.centralEvents.onEnemyHit -= OnEnemyHitByMelee;
    }

    private void OnEnemyHitByMelee(Enemy enemy, int damage, DamageReason reason)
    {
        // Sanity check
        if (!parentItem.equipped)
            return;
        if (reason != DamageReason.Melee)
            return;

        int val = Random.Range(0, 100);
        int luck = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Luck, Game.instance.avatar.gameObject);

        if (val - luck / 4 < chance)
        {
            ApplyStatus(enemy);
        }
    }

    void ApplyStatus(Enemy target)
    {
        if (!target.GetComponent(statusComponentTemplate.GetType()))
        {
            Component newComponent = target.gameObject.AddComponent(statusComponentTemplate.GetType());
            MonoBehaviour actual = (MonoBehaviour)newComponent;
            actual.enabled = true;
        }
    }
}
