using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ColorExtensions;

public class NumberPopup : MonoBehaviour
{
    public Text amountLabel;
    public Text amountShadowLabel;
    public Image icon;

    public Sprite heartImage;
    public Sprite coinImage;

    public void PlayPopup(int amount, NumberPopupReason reason)
    {
        PlayPopup(amount.ToString(), reason);
    }

    public void PlayPopup(string amountStr, NumberPopupReason reason)
    {
        if (reason == NumberPopupReason.RemoveCoins || reason == NumberPopupReason.RemoveHearts)
            amountStr = "-" + amountStr;

        amountLabel.text = amountStr;
        amountShadowLabel.text = amountStr;

        if (reason == NumberPopupReason.Bad)
        {
            amountLabel.color = Color.red;
            icon.gameObject.SetActive(false);
        }
        else if (reason == NumberPopupReason.Good)
        {
            amountLabel.color = Color.green;
            icon.gameObject.SetActive(false);
        }
        else if (reason == NumberPopupReason.RemoveCoins)
        {
            amountLabel.color = Color.yellow;
            icon.sprite = coinImage;
        }
        else if (reason == NumberPopupReason.RemoveHearts)
        {
            amountLabel.color = Color.red;
            icon.sprite = heartImage;
        }
        
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        float targetScale = 0.015f;

        transform.localScale = Vector3.zero;
        float time = 0f;
        while (time < 1.5f)
        {
            time += Time.deltaTime * 6f;
            transform.localScale = Vector3.one * time * targetScale;
            transform.localPosition += Vector3.up * Time.deltaTime * 2f;
            yield return null;
        }

        while (time > 1f)
        {
            time -= Time.deltaTime * 6f;
            transform.localScale = Vector3.one * time * targetScale;
            transform.localPosition += Vector3.up * Time.deltaTime * 2f;
            yield return null;
        }

        transform.localScale = Vector3.one * targetScale;

        time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition += Vector3.up * Time.deltaTime * 1.5f;

            icon.color = icon.color.ModifiedAlpha(-Time.deltaTime);
            amountLabel.color = amountLabel.color.ModifiedAlpha(-Time.deltaTime);
            amountShadowLabel.color = amountShadowLabel.color.ModifiedAlpha(-Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);

        yield break;
    }
}
