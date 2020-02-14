using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Leave", 4f);
    }

    void Leave()
    {
        Game.instance.transitionManager.TransitionToScreen("Title");
    }
}
