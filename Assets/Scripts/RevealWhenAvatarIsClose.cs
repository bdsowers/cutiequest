using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealWhenAvatarIsClose : MonoBehaviour
{
    private bool mRevealed = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (mRevealed)
            return;

        float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);
        if (distance < 6.5f)
        {
            Reveal();
        }
    }

    void Reveal()
    {
        mRevealed = true;

        StartCoroutine(RevealAnimation());
    }

    private IEnumerator RevealAnimation()
    {
        float delay = Random.Range(0f, 0.5f);
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;
        targetScale.y = Random.Range(0.9f, 1.2f);
        float overshoot = 0.2f;
        float time = 0f;
        float speed = 3f;
        while (time < 1f + overshoot)
        {
            time += Time.deltaTime * speed;
            transform.localScale = startScale + (targetScale - startScale) * time;
            yield return null;
        }

        while (time > 1.0f)
        {
            time -= Time.deltaTime * speed;
            if (time < 1f) time = 1f;
            transform.localScale = startScale + (targetScale - startScale) * time;
            yield return null;
        }

        transform.localScale = targetScale;

        yield break;
    }
}
