using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boombox : MonoBehaviour
{
    public Item parentItem;

    private float mDamageTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEquipped(Item item)
    {

    }

    private void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (parentItem.equipped)
        {
            if (Game.instance.avatar.IsDancing())
            {
                mDamageTimer += Time.deltaTime;
                if (mDamageTimer > 1f)
                {
                    mDamageTimer = 0f;
                    DamageAllKillablesNearPlayer();
                }
            }
            else
            {
                mDamageTimer = 0f;
            }
        }
    }

    void DamageAllKillablesNearPlayer()
    {
        if (KillableMap.instance == null) return;

        List<Killable> allKillables = KillableMap.instance.allKillables;
        for (int i = 0; i < allKillables.Count; ++i)
        {
            Killable killable = allKillables[i];
            if (killable.gameObject == Game.instance.avatar.gameObject)
                continue;

            float distance = Vector3.Distance(Game.instance.avatar.transform.position, killable.transform.position);
            if (distance < 4f)
            {
                killable.TakeDamage(gameObject, 1, DamageReason.Spell);
            }
        }
    }
}
