using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Mansplain : MonoBehaviour
{
    public Typewriter typewriter;

    private float mChangeTextTimer = 0f;
    private Enemy mParentEnemy;

    private Enemy parentEnemy
    {
        get
        {
            if (mParentEnemy == null)
                mParentEnemy = GetComponentInParent<Enemy>();

            return mParentEnemy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only update this if the parent enemy is revealed
        if (parentEnemy != null && parentEnemy.commonComponents.revealWhenAvatarIsClose.fullyRevealed);

        mChangeTextTimer -= Time.deltaTime;
        if (mChangeTextTimer <= 0f)
        {
            string message = LocalizedText.Get(LocalizedText.GetKeysInList("[MANSPLAIN]").Sample());
            typewriter.ShowText(message, 1f);

            mChangeTextTimer = Random.Range(3f, 10f);
        }
    }
}
