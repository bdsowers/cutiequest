using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewMode : MonoBehaviour
{
    private bool mIsLeaving = false;

    // Start is called before the first frame update
    void Start()
    {
        Game.instance.soundManager.StopMusic();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !mIsLeaving)
        {
            Leave();
        }
    }

    private void Leave()
    {
        mIsLeaving = true;
        Game.instance.transitionManager.TransitionToScreen("Title");
    }
}
