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
        Destroy(gameObject);

        yield break;
    }

    private void PlayEffect()
    {
        GameObject newEffect = GameObject.Instantiate(PrefabManager.instance.PrefabByName(effect));
        newEffect.transform.position = transform.position;
    }
}
