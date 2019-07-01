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
                NumberPopupGenerator.instance.GeneratePopup(transform.position + Vector3.up * 0.7f, "All enemies destroyed", NumberPopupReason.Heal);

                Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
                for (int i = 0; i < enemies.Length; ++i)
                {
                    Killable killable = enemies[i].GetComponent<Killable>();
                    killable.TakeDamage(killable.health);
                }
            }
            else
            {
                NumberPopupGenerator.instance.GeneratePopup(transform.position + Vector3.up * 0.7f, "The odds were not in your favor", NumberPopupReason.TakeDamage);

                Game.instance.playerData.health = 1;
                Game.instance.avatar.GetComponent<Killable>().health = 1;
            }
        }
    }
}
