using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO bdsowers - this really only supports flags and not more complicated expressions
// Use CinematicRequirementCompare for more complicated expressions, but even it has its limits.
// Revisit this if that ever becomes desired, though I'm trying to keep this langauge relatively simple.
public class CinematicActionIf : CinematicAction
{
    private bool mConditionIsTrue;

    public override string actionName
    {
        get
        {
            return "flag_set";
        }
    }

    public override string[] aliases
    {
        get
        {
            return new string[] { "flag_unset" };
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "condition";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mConditionIsTrue = dataProvider.GetBoolData(mParameters, "condition");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        if ((mConditionIsTrue && alias == "flag_set") || (!mConditionIsTrue && alias == "flag_unset"))
        {
            yield return PlayChildActions(player);
        }

        yield break;
    }
}
