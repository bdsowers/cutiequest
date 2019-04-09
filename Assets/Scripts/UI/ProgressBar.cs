using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image emptyImage;
    public Image fullImage;
    public Text label;

    public void SetWithValues(int min, int max, int value)
    {
        float percent = (float)value / (float)(max - min);
        fullImage.fillAmount = percent;

        if (label != null)
        {
            label.text = "" + BadAtMathQuirk.ApplyQuirkIfPresent(value) + " / " + BadAtMathQuirk.ApplyQuirkIfPresent(max);
        }
    }

    public void SetWithPercent(float percent)
    {
        fullImage.fillAmount = percent;

        if (label != null)
        {
            label.text = "" + Mathf.Round(percent * 100) + "%";
        }
    }
}
