using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineGuillotine : Shrine
{
    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        if (WasAccepted())
        {
            int val = Random.Range(0, 2);
            if (val == 0)
            {
                NumberPopupGenerator.instance.GeneratePopup(gameObject, "All enemies destroyed", NumberPopupReason.Good);

                Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
                for (int i = 0; i < enemies.Length; ++i)
                {
                    Killable killable = enemies[i].GetComponent<Killable>();
                    killable.TakeDamage(gameObject, killable.health, DamageReason.ForceKill);
                }
            }
            else
            {
                NumberPopupGenerator.instance.GeneratePopup(gameObject, "The odds were not in your favor", NumberPopupReason.Bad);

                Game.instance.playerData.health = 1;
                Game.instance.avatar.GetComponent<Killable>().health = 1;
            }
        }
    }
}
