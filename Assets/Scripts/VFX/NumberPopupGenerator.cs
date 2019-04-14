using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NumberPopupReason
{
    TakeDamage,
    Heal,
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
        mInstance = this;
    }

    public void GeneratePopup(Vector3 position, int amount, NumberPopupReason reason)
    {
        amount = BadAtMathQuirk.ApplyQuirkIfPresent(amount);

        GameObject newPopup = GameObject.Instantiate(PrefabManager.instance.PrefabByName("NumberPopup"));
        newPopup.transform.position = position;
        newPopup.GetComponent<NumberPopup>().PlayPopup(amount, reason);
    }

    public void GeneratePopup(Vector3 position, string text, NumberPopupReason reason)
    {
        GameObject newPopup = GameObject.Instantiate(PrefabManager.instance.PrefabByName("NumberPopup"));
        newPopup.transform.position = position;
        newPopup.GetComponent<NumberPopup>().PlayPopup(text, reason);
    }
}
