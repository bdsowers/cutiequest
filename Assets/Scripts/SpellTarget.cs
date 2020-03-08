using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorExtensions;

public class SpellTarget : MonoBehaviour
{
    Renderer mRenderer;
    Material mMaterial;

    Color mBaseColor = new Color(253 / 255.0f, 255 / 255.0f, 0f, 1f);
    Color mDangerColor = new Color(255 / 255.0f, 0, 0f, 1f);
    Color mBlinkColor = new Color(1f, 1f, 1f, 1f);

    public float castTime { get; set; }
    public string effect { get; set; }
    public Vector3 effectOffset { get; set; }

    public int strength { get; set; }

    public bool hideEffectIfNoHit { get; set; }

    public string triggerSFX { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        mRenderer = GetComponentInChildren<Renderer>();
        mMaterial = new Material(mRenderer.material);
        mRenderer.material = mMaterial;

        StartCoroutine(Animation(castTime));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Animation(float totalSeconds)
    {
        float fadeSeconds = 0f;
        float blinkSeconds = 0f;

        if (totalSeconds < 1f)
        {
            fadeSeconds = totalSeconds * 0.65f;
            blinkSeconds = totalSeconds * 0.35f;
        }
        else
        {
            fadeSeconds = totalSeconds - 1f;
            blinkSeconds = 1f;
        }

        mMaterial.color = mBaseColor;

        transform.localScale = Vector3.zero;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 5f;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            yield return null;
        }

        time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime / fadeSeconds;
            mMaterial.color = ColorHelper.Lerp(mBaseColor, mDangerColor, time);
            yield return null;
        }

        time = 0f;
        float blinkTime = 0.2f;
        while (time < 1f + blinkTime)
        {
            time += Time.deltaTime / blinkSeconds;

            float t = Mathf.PingPong(time, blinkTime) / blinkTime;
            mMaterial.color = ColorHelper.Lerp(mDangerColor, mBlinkColor, t);
            yield return null;
        }

        StartCoroutine(DealDamageCoroutine());
        if (!hideEffectIfNoHit)
        {
            PlayEffect();
        }

        if (!string.IsNullOrEmpty(triggerSFX))
        {
            Game.instance.soundManager.PlaySound(triggerSFX);
        }

        yield return new WaitForSeconds(0.35f);

        Destroy(gameObject);

        yield break;
    }

    private void PlayEffect()
    {
        GameObject newEffect = GameObject.Instantiate(PrefabManager.instance.PrefabByName(effect));
        newEffect.transform.position = transform.position + effectOffset;

        RepetitiveDamageTrap trapSpell = newEffect.GetComponent<RepetitiveDamageTrap>();
        if (trapSpell != null)
        {
            trapSpell.strength = strength;
        }
    }

    // To prevent spells being erroneously 'missed' when an enemy is moving at just
    // the right time, apply damage over time for a short time window.
    // This only applies to enemies - the player can only be hit on a particular frame.
    private IEnumerator DealDamageCoroutine()
    {
        float time = 0.2f;
        bool canHitPlayer = true;

        while (time > 0f)
        {
            bool damaged = DealDamage(canHitPlayer);
            if (damaged)
                time = -1f;

            time -= Time.deltaTime;
            canHitPlayer = false;

            yield return null;
        }
    }

    private bool DealDamage(bool canHitPlayer)
    {
        // See if a targetable entity is in the same space as us
        // Targetable in this case = someone not currently on the same layer as us
        Killable targetKillable = KillableMap.instance.KillableAtWorldPosition(transform.position);
        if (targetKillable != null && targetKillable.gameObject.layer != gameObject.layer)
        {
            if (!canHitPlayer && targetKillable.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return false;
            }

            int defense = targetKillable.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);
            int damage = strength * 4 - defense * 2;

            targetKillable.TakeDamage(null, damage, DamageReason.Spell);

            if (hideEffectIfNoHit)
            {
                PlayEffect();
            }

            return true;
        }

        return false;
    }
}
