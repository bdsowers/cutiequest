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

    public void GeneratePopup(Vector3 position, int amount, NumberPopupReason reason, float delay = 0f)
    {
        amount = BadAtMathQuirk.ApplyQuirkIfPresent(amount);

        GeneratePopupEnumerator(position, amount.ToString(), reason, delay);
    }

    public void GeneratePopup(Vector3 position, string text, NumberPopupReason reason, float delay = 0f)
    {
        GeneratePopupEnumerator(position, text, reason, delay);
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
