using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CinematicActionIntroAnimateTextIn : CinematicAction
{
    private string mTarget;
    private float mDelayIncrement;
    private float mSpeedMulitplier;

    public override string actionName
    {
        get
        {
            return "intro_animate_text_in";
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

        mDelayIncrement = dataProvider.GetFloatData(mParameters, "delay_inc", 0.25f);
        mSpeedMulitplier = dataProvider.GetFloatData(mParameters, "speed_mul", 1f);
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        TextMeshProUGUI text = player.objectMap.GetObjectByName(mTarget).GetComponent<TextMeshProUGUI>();

        string originalText = text.text;
        float originalSize = (float)text.fontSize;

        float[] alphas = new float[originalText.Length];
        float[] sizes = new float[originalText.Length];
        float[] delays = new float[originalText.Length];

        for (int i = 0; i < originalText.Length; ++i)
        {
            alphas[i] = 0f;
            sizes[i] = originalSize + 30f;
            delays[i] = i * mDelayIncrement;
        }

        float time = 0f;

        // Manipulate the string from right to left so we can replace at position
        // without blowing up, but still perform the core animation from left-to-right
        while (time < originalText.Length * 3f)
        {
            for (int i = 0; i < originalText.Length; ++i)
            {
                if (time > delays[i])
                {
                    sizes[i] -= Time.deltaTime * 30f * mSpeedMulitplier * player.playbackTimeScale;
                    sizes[i] = Mathf.Max(sizes[i], originalSize);

                    alphas[i] += Time.deltaTime * mSpeedMulitplier * 0.25f * player.playbackTimeScale;
                    alphas[i] = Mathf.Min(alphas[i], 1f);
                }
            }

            // Manipulate the string from right-to-left to make inserting the color codes easier
            // TODO bdsowers - we probably want to use a StringBuilder and make this a tonnnn cleaner.
            string animatedText = (string)originalText.Clone();

            for (int i = originalText.Length - 1; i >= 0; --i)
            {
                Color color = new Color(0f, 0f, 0f, alphas[i]);
                float size = sizes[i];

                string colorRichText = "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">";
                string sizeRichText = "<size=" + size + ">";

                animatedText = animatedText.Insert(i + 1, "</size></color>");
                animatedText = animatedText.Insert(i, colorRichText + sizeRichText);
            }

            text.text = animatedText;

            time += Time.deltaTime * mSpeedMulitplier * player.playbackTimeScale;

            yield return null;
        }

        yield break;
    }
}
