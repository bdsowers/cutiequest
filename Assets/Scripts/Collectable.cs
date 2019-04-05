using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrencyType
{
    None,
    Hearts,
    Coins,
}

// todo bdsowers - rewrite to support any collectable
public class Collectable : MonoBehaviour
{
    public CurrencyType currencyType;

    private bool mIsAnimating;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectIfPossible(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CollectIfPossible(other);
    }

    private void CollectIfPossible(Collider collider)
    {
        if (mIsAnimating)
            return;

        if (collider.GetComponentInParent<PlayerController>() != null)
        {
            StartCoroutine(CollectCoroutine());
        }
    }

    private IEnumerator CollectCoroutine()
    {
        mIsAnimating = true;
        Vector3 startPosition = transform.position;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 5f;
            Vector3 position = Vector3.Lerp(startPosition, Game.instance.avatar.modelContainer.transform.position, time);
            transform.position = position;
            yield return null;
        }

        mIsAnimating = false;

        if (currencyType == CurrencyType.Hearts)
        {
            Game.instance.playerData.numHearts++;
        }
        else
        {
            Game.instance.playerData.numCoins++;
        }

        GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX3_Hit_Misc_D");
        vfx.transform.position = transform.position + Vector3.up * 0.4f;
        vfx.AddComponent<DestroyAfterTimeElapsed>().time = 1f;

        Destroy(gameObject);

        yield break;
    }

    public void PlayDropAnimation(Vector3 sourcePosition, Vector3 endPosition, bool shouldDelay)
    {
        StartCoroutine(PlayDropAnimation(gameObject, sourcePosition, endPosition, shouldDelay));
    }

    private IEnumerator PlayDropAnimation(GameObject obj, Vector3 sourcePosition, Vector3 endPosition, bool shouldDelay)
    {
        mIsAnimating = true;

        obj.transform.localScale = Vector3.zero;

        float startY = obj.transform.GetChild(0).transform.localPosition.y;
        float heightDeviation = Random.Range(0.2f, 0.5f);

        float delay = Random.Range(0.1f, 0.2f);
        if (!shouldDelay)
            delay = 0f;

        while (delay > 0)
        {
            delay -= Time.deltaTime * 0.75f;
            yield return null;
        }
        
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 4f;

            float y = startY;
            if (time < 0.5f)
            {
                y = startY + time * heightDeviation;
            }
            else
            {
                y = startY + (1 - time) * heightDeviation;
            }

            float scale = Mathf.Min(1f, time * 3f);
            obj.transform.localScale = Vector3.one * scale;
            obj.transform.GetChild(0).transform.localPosition = Vector3.up * y;
            obj.transform.position = Vector3.Lerp(sourcePosition, endPosition, time);

            yield return null;
        }

        obj.transform.localScale = Vector3.one;
        obj.transform.position = endPosition;
        obj.transform.GetChild(0).transform.localPosition = Vector3.up * startY;

        mIsAnimating = false;

        yield break;
    }
}
