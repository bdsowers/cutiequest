using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : SingleUseItem
{
    protected override void OnUse()
    {
        RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
        for (int i = 0; i < revealers.Length; ++i)
        {
            revealers[i].Reveal();
        }
    }
}
