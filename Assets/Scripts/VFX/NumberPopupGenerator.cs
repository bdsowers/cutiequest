﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NumberPopupReason
{
    Bad,
    Good,
    RemoveCoins,
    RemoveHearts,
}

public class NumberPopupGenerator : MonoBehaviour
{
    private static NumberPopupGenerator mInstance;

    public static NumberPopupGenerator instance
    {
        get { return mInstance; }
    }

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
    }

    public void GeneratePopup(Vector3 position, int amount, NumberPopupReason reason, float delay = 0f)
    {
        amount = BadAtMathQuirk.ApplyQuirkIfPresent(amount);

        StartCoroutine(mInstance.GeneratePopupEnumerator(position, amount.ToString(), reason, delay));
    }

    public void GeneratePopup(GameObject entity, int amount, NumberPopupReason reason, float delay = 0f)
    {
        amount = BadAtMathQuirk.ApplyQuirkIfPresent(amount);

        Vector3 position = entity.transform.position + Vector3.up * 0.7f;
        StartCoroutine(mInstance.GeneratePopupEnumerator(position, amount.ToString(), reason, delay));
    }

    public void GeneratePopup(Vector3 position, string text, NumberPopupReason reason, float delay = 0f)
    {
        StartCoroutine(mInstance.GeneratePopupEnumerator(position, text, reason, delay));
    }

    public void GeneratePopup(GameObject entity, string text, NumberPopupReason reason, float delay = 0f)
    {
        Vector3 position = entity.transform.position + Vector3.up * 0.7f;
        StartCoroutine(mInstance.GeneratePopupEnumerator(position, text, reason, delay));
    }

    private IEnumerator GeneratePopupEnumerator(Vector3 position, string text, NumberPopupReason reason, float delay)
    {
        if (delay > 0.01f)
            yield return new WaitForSeconds(delay);

        GameObject newPopup = GameObject.Instantiate(PrefabManager.instance.PrefabByName("NumberPopup"));
        newPopup.transform.position = position;
        newPopup.GetComponent<NumberPopup>().PlayPopup(text, reason);

        yield break;
    }
}
