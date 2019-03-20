using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleUseItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Item>().onEquip += OnEquip;
    }

    private void OnEquip(Item item)
    {
        OnUse();
    }

    private void OnDestroy()
    {
        GetComponent<Item>().onEquip -= OnEquip;
    }

    protected abstract void OnUse();
}
