using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monocle : MonoBehaviour
{
    public Item parentItem;

    // Start is called before the first frame update
    void Start()
    {
        parentItem.onEquip += OnEquip;
    }

    private void OnEquip(Item item)
    {
        // Destroy any vision-based quirks
        Quirk[] allQuirks = GameObject.FindObjectsOfType<Quirk>();
        for (int i = 0; i < allQuirks.Length; ++i)
        {
            allQuirks[i].DestroyVolume();
        }
    }

    private void OnDestroy()
    {
        if (parentItem != null) parentItem.onEquip -= OnEquip;
    }
}
