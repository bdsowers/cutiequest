using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackCoffee : MonoBehaviour
{
    public Item parentItem;

    // Start is called before the first frame update
    void Start()
    {
        parentItem.onEquip += OnEquip;
    }

    private void OnEquip(Item item)
    {
    }

    private void OnDestroy()
    {
        if (parentItem != null) parentItem.onEquip -= OnEquip;
    }
}
