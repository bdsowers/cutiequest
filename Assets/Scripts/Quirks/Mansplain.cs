using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Mansplain : MonoBehaviour
{
    public Typewriter typewriter;

    private float mChangeTextTimer = 0f;

    string[] mMessages = new string[]
    {
        "#notallmen",
        "Well, actually, it's about ethics in journalism.",
        "Well, actually, the first appearance of Spider-Man was Amazing Fantasy #15.",
        "Well, actually, the gender pay gap isn't nearly as big if you account for various factors."
    };

    // Update is called once per frame
    void Update()
    {
        // todo bdsowers - only do this if this enemy has been revealed

        mChangeTextTimer -= Time.deltaTime;
        if (mChangeTextTimer <= 0f)
        {
            string message = mMessages.Sample();
            typewriter.ShowText(message, 1f);

            mChangeTextTimer = Random.Range(3f, 10f);
        }
    }
}
