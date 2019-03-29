using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicRequirementBlock : CinematicRequirement
{
    public override string actionName
    {
        get
        {
            return "requirements";
        }
    }

    public override bool Evaluate(CinematicDirector player)
    {
        for (int i = 0; i < mChildActions.Count; ++i)
        {
            CinematicRequirement requirement = (CinematicRequirement)mChildActions[i];

            requirement.InterpretParameters(player.dataProvider);

            if (!requirement.Evaluate(player))
                return false;
        }

        return true;
    }
}
