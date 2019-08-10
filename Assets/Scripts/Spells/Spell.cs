using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string friendlyName;
    public string description;

    public float cooldown;
    private float mCooldownTimer;

    public Sprite icon;

    public int requiredLevel = 1;

    public bool canActivate
    {
        get { return mCooldownTimer <= 0f; }
    }

    public float cooldownTimer
    {
        get { return mCooldownTimer; }
    }

    public virtual void Activate(GameObject caster)
    {
        int magic = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, Game.instance.avatar.gameObject);
        float actualCooldown = cooldown - (magic / 4f);
        actualCooldown = Mathf.Max(1f, actualCooldown);

        mCooldownTimer = actualCooldown;
    }

    private void Update()
    {
        if (mCooldownTimer >= 0)
        {
            mCooldownTimer -= Time.deltaTime;
        }
    }

    protected void PlayBoilerplateVFX()
    {
        GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX2_PickupDiamond2");
        vfx.transform.position = Game.instance.avatar.transform.position + Vector3.up * 0.5f;
        vfx.AddComponent<DestroyAfterTimeElapsed>().time = 2f;
    }
}
