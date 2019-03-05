using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CinematicActionIntroAnimateTextOut : CinematicAction
{
    private string mTarget;
    private float mDelayIncrement;
    private float mSpeedMulitplier;

    public override string actionName
    {
        get
        {
            return "intro_animate_text_out";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "target";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTarget = dataProvider.GetStringData(mParameters, "target");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        TextMeshProUGUI text = player.objectMap.GetObjectByName(mTarget).GetComponent<TextMeshProUGUI>();

        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * player.playbackTimeScale;
            text.alpha = alpha;
            text.characterSpacing += Time.deltaTime * 6f * player.playbackTimeScale;
            yield return null;
        }

        text.alpha = 0f;

        yield break;
    }
}
