using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Mansplain : MonoBehaviour
{
    public Typewriter typewriter;

    private float mChangeTextTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        // todo bdsowers - only do this if this enemy has been revealed
        
        mChangeTextTimer -= Time.deltaTime;
        if (mChangeTextTimer <= 0f)
        {
            string message = LocalizedText.Get(LocalizedText.GetKeysInList("[MANSPLAIN]").Sample());
            typewriter.ShowText(message, 1f);

            mChangeTextTimer = Random.Range(3f, 10f);
        }
    }
}
