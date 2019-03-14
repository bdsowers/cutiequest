using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontloader : MonoBehaviour
{
    public void Start()
    {
        Game.instance.transitionManager.TransitionToScreen("HUB");
    }
}
