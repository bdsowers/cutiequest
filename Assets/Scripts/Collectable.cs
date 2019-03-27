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
            if (currencyType == CurrencyType.Hearts)
            {
                Game.instance.playerData.numHearts++;
            }
            else
            {
                Game.instance.playerData.numCoins++;
            }

            Destroy(gameObject);
        }
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
            delay -= Time.deltaTime;
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
