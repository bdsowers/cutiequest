using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    public Text[] labels;

    private float mAlpha = 1f;
    private float mDirection = -1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mAlpha += mDirection * Time.deltaTime * 1.5f;
        float maxAlpha = 1.25f; // Allow a little overage to simulate a 'pause'
        if (mAlpha < 0f || mAlpha > maxAlpha)
        {
            mAlpha = Mathf.Clamp(mAlpha, 0f, maxAlpha);
            mDirection = -mDirection;
        }

        for (int i = 0; i < labels.Length; ++i)
        {
            labels[i].color = new Color(labels[i].color.r, labels[i].color.g, labels[i].color.b, mAlpha);
        }
    }
}
