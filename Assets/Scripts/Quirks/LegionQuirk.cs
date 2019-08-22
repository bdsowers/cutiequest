using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using GameObjectExtensions;

public class LegionQuirk : Quirk
{
    // Start is called before the first frame update
    public override void Start()
    {
        // Pick 3 quirks in the player's level and attach them
        List<Quirk> quirks = Game.instance.companionBuilder.QuirksInLevel(Game.instance.playerData.attractiveness);
        quirks.RemoveAll(i => i.name == "LegionQuirk");

        for (int i = 0; i < 3; ++i)
        {
            if (quirks.Count == 0)
                continue;

            Quirk randomQuirk = quirks.Sample();

            GameObject newQuirk = GameObject.Instantiate(randomQuirk.gameObject, transform);
            newQuirk.SetLayerRecursive(LayerMask.NameToLayer("Player"));

            quirks.Remove(randomQuirk);
            RemoveMutuallyExclusiveQuirks(randomQuirk, quirks);
        }
    }

    public void RemoveMutuallyExclusiveQuirks(Quirk newQuirk, List<Quirk> quirks)
    {
        string[] mutuallyExclusiveQuirks = new string[]
        {
            "CowboyQuirk",
            "OldTimeyQuirk",
            "SketchyQuirk",
        };

        if (System.Array.IndexOf(mutuallyExclusiveQuirks, newQuirk.name) != -1)
        {
            quirks.RemoveAll(i => System.Array.IndexOf(mutuallyExclusiveQuirks, i.name) != -1);
        }
    }
}
