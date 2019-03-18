using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionChangeSpeaker : CinematicAction
{
    private string mSpeakerModel;

    public override string actionName
    {
        get { return "change_speaker"; }
    }

    public override string simpleParameterName
    {
        get { return "speaker_model"; }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        mSpeakerModel = dataProvider.GetStringData(mParameters, "speaker_model");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject.Find("CharacterImageCapture").GetComponentInChildren<CharacterModel>().ChangeModel(mSpeakerModel);
        yield break;
    }
}
