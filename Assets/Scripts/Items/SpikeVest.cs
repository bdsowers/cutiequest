using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeVest : MonoBehaviour
{
    public Item parentItem;

    private void Start()
    {
        parentItem.onEquip += OnEquip;
    }

    private void OnEquip(Item item)
    {
        Game.instance.centralEvents.onPlayerHit += OnPlayerHit;
    }

    private void OnPlayerHit(GameObject damageSource, int damage, DamageReason damageReason)
    {
        if (damageReason == DamageReason.Melee && damageSource != null)
        {
            Killable killable = damageSource.GetComponent<Killable>();
            killable.TakeDamage(gameObject, 5, DamageReason.StatusEffect);
        }
    }

    private void OnDestroy()
    {
        Game.instance.centralEvents.onPlayerHit -= OnPlayerHit;
        if (parentItem != null) parentItem.onEquip -= OnEquip;
    }
}
