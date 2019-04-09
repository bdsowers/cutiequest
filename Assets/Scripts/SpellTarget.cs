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

    public int strength { get; set; }

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

        PlayEffect();
        yield return new WaitForSeconds(0.35f);
        DealDamage();

        Destroy(gameObject);

        yield break;
    }

    private void PlayEffect()
    {
        GameObject newEffect = GameObject.Instantiate(PrefabManager.instance.PrefabByName(effect));
        newEffect.transform.position = transform.position;
    }

    private void DealDamage()
    {
        // todo bdsowers - this is perhaps  a little too precise and misses some hits

        // See if a targetable entity is in the same space as us
        // Targetable in this case = someone not currently on the same layer as us
        Killable targetKillable = KillableMap.instance.KillableAtWorldPosition(transform.position);
        if (targetKillable != null && targetKillable.gameObject.layer != gameObject.layer)
        {
            int defense = targetKillable.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);
            int damage = strength * 4 - defense * 2;

            targetKillable.TakeDamage(damage);
        }
    }
}
