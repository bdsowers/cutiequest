using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorDisplay : MonoBehaviour
{
    private int mPreviousDisplayValue = -1;
    public Text label;
    public Text labelShadow;

    // Update is called once per frame
    void Update()
    {
        if (mPreviousDisplayValue != Game.instance.currentDungeonFloor)
        {
            mPreviousDisplayValue = Game.instance.currentDungeonFloor;
            label.text = BadAtMathQuirk.ApplyQuirkIfPresent(mPreviousDisplayValue).ToString();
            labelShadow.text = mPreviousDisplayValue.ToString();
        }
    }
}
