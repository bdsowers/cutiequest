using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Item>().onEquip += OnEquip;
    }

    private void OnEquip(Item item)
    {
        RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
        for (int i = 0; i < revealers.Length; ++i)
        {
            revealers[i].Reveal();
        }
    }

    private void OnDestroy()
    {
        GetComponent<Item>().onEquip -= OnEquip;
    }
}
