using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionRandomJoke : CinematicAction
{
    public override string actionName
    {
        get { return "random_joke"; }
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        List<string> setups = LocalizedText.GetKeysInList("[JOKE_SETUP]");
        List<string> punchlines = LocalizedText.GetKeysInList("[JOKE_PUNCHLINE]");

        int jokeNum = Random.Range(0, setups.Count);
        string setupKey = setups[jokeNum];
        string punchlineKey = punchlines[jokeNum];
        string setup = LocalizedText.Get(setupKey);
        string punchline = LocalizedText.Get(punchlineKey);

        CinematicActionTypewriter setupAction = new CinematicActionTypewriter();
        Dictionary<string, string> setupParams = new Dictionary<string, string>();
        setupParams.Add("text", setup);
        setupParams.Add("keep_open", "true");
        setupAction.SetParameters(setupParams);
        yield return player.PlayAction(setupAction);

        CinematicActionTypewriter punchlineAction = new CinematicActionTypewriter();
        Dictionary<string, string> punchlineParams = new Dictionary<string, string>();
        punchlineParams.Add("text", punchline);
        punchlineAction.SetParameters(punchlineParams);
        yield return player.PlayAction(punchlineAction);

        yield break;
    }
}
