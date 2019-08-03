using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionRemoveNPC : CinematicAction
{
    private string mNPCName;
    
    public override string actionName
    {
        get
        {
            return "remove_npc";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "name";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mNPCName = dataProvider.GetStringData(mParameters, "name");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject character = CinematicId.FindObjectWithId(mNPCName);
        GameObject.Destroy(character.gameObject);

        yield break;
    }
}
