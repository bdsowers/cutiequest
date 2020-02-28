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
    public bool canHitBoss = true;

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
        if (parentItem != null) parentItem.onEquip -= OnEquip;
    }

    private void OnEnemyHitByMelee(Enemy enemy, int damage, DamageReason reason)
    {
        // Sanity check
        if (!parentItem.equipped)
            return;
        if (reason != DamageReason.Melee)
            return;
        if (!canHitBoss && enemy.isBoss)
            return;

        int val = Random.Range(0, 100);
        int luck = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Luck, Game.instance.avatar.gameObject);

        // Guarantee status effect if we're testing items
        if (Cheats.forceTestItemGeneration) val = 0;

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
