using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;
using ArrayExtensions;

public class DifficultyBoost : MonoBehaviour
{
    private List<Quirk> mQuirks;

    private static bool mQuirksChosen;
    private static List<string> mChosenQuirks;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDestroy()
    {
    }

    public void ApplyQuirks()
    {
        // Don't pick the quirks until we're in the dungeon, otherwise there's a chance
        // the player will choose a match that collides.
        if (Game.instance.InDungeon())
        {
            PickQuirks();
        }

        if (mChosenQuirks == null)
            return;

        // Actually setup all the quirks
        for (int i = 0; i < mChosenQuirks.Count; ++i)
        {
            GameObject quirk = PrefabManager.instance.PrefabByName(mChosenQuirks[i]);
            Debug.Log("Applying " + quirk.name);

            GameObject newQuirk = GameObject.Instantiate(quirk.gameObject, Game.instance.avatar.transform);
            newQuirk.SetLayerRecursive(LayerMask.NameToLayer("Player"));
        }

        Game.instance.RefreshInventory();
    }

    void PickQuirks()
    {
        if (mQuirksChosen)
            return;

        mQuirksChosen = true;

        string[] unavailable = new string[]
        {
            "CowboyQuirk",
            "OldTimeyQuirk",
            "SketchyQuirk",
            "Legion",
        };

        int numQuirks = Random.Range(2, 4);

        List<Quirk> possibleQuirks = Game.instance.companionBuilder.QuirksInLevel(Game.instance.playerData.attractiveness, -1, Game.instance.playerData.scoutLevel, -1);
        possibleQuirks.RemoveAll(i => System.Array.IndexOf(unavailable, i.name) != -1);

        int currentQuirk = 0;
        while (currentQuirk < numQuirks && possibleQuirks.Count > 0)
        {
            Quirk randomQuirk = possibleQuirks.Sample();
            possibleQuirks.Remove(randomQuirk);

            if (!Game.instance.quirkRegistry.IsQuirkActive(randomQuirk))
            {
                mChosenQuirks.Add(randomQuirk.name);
                ++currentQuirk;
            }
        }
    }

    public static void Reset()
    {
        mQuirksChosen = false;
        mChosenQuirks = new List<string>();
    }
}
