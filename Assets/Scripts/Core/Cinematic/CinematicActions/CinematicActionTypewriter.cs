using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionTypewriter : CinematicAction
{
    private string mText;
    private string mTarget;
    private bool mWaitForInteraction;
    private bool mKeepOpen = false;

    public override string actionName
    {
        get
        {
            return "typewriter";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "text";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mText = dataProvider.GetStringData(mParameters, "text");
        mText = PigLatinQuirk.ApplyQuirkIfPresent(LocalizedText.Get(mText));

        mTarget = dataProvider.GetStringData(mParameters, "target", "typewriter");
        mWaitForInteraction = dataProvider.GetBoolData(mParameters, "wait_for_interaction", true);
        mKeepOpen = dataProvider.GetBoolData(mParameters, "keep_open", false);

        shouldYield = mWaitForInteraction;
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        Typewriter target = player.objectMap.GetObjectByName(mTarget).GetComponent<Typewriter>();

        if (!mWaitForInteraction)
        {
            yield return target.ShowTextCoroutine(mText, 1f);
        }
        else
        {
            target.ShowText(mText, 1f);

            // Provide a little lag time
            yield return new WaitForSeconds(0.25f);

            bool keepWaiting = true;
            BasicActionSet actionSet = Game.instance.actionSet;
            while (keepWaiting)
            {
                if (target.isAnimating)
                {
                    // todo bdsowers - use InControl for this
                    if (actionSet.Activate.WasPressed || actionSet.Spell.WasPressed)
                    {
                        target.ForceFinish();
                    }
                }
                else
                {
                    if (actionSet.Activate.WasPressed || actionSet.Spell.WasPressed)
                    {
                        keepWaiting = false;

                        if (!mKeepOpen)
                        {
                            target.HideText();
                        }
                    }
                }

                yield return null;
            }

        }
        yield break;
    }
}
