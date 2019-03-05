using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// These piggy-back off of the CinematicAction system, but they're not necessarily meant to be played
/// actions - they're conditions that can evaluate to true/false
/// todo - the inheritance on this could definitely use some reevaluation. It's functional and was a quick
/// way to get things up and running, but it's definitely not intuitive or logica.
/// </summary>
public abstract class CinematicRequirement : CinematicAction
{
    public abstract bool Evaluate(CinematicDirector player);

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        // These are playable if we want them to be - if they evaluate to true, we'll play child actions
        if (Evaluate(player))
        {
            yield return PlayChildActions(player);
        }

        yield break;
    }
}
