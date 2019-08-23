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
        GameObject speakerContainer = player.objectMap.GetObjectByName("speaker");
        bool showSpeaker = (mSpeakerModel != "none");

        if (speakerContainer != null)
        {
            player.objectMap.GetObjectByName("speaker").SetActive(showSpeaker);
        }

        if (showSpeaker)
        {
            CharacterModel model = GameObject.Find("CharacterImageCapture").GetComponentInChildren<CharacterModel>();
            model.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            model.ChangeModel(mSpeakerModel);
        }

        yield break;
    }
}
