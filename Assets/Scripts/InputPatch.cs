using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Combats a rare but nasty bug where InControl freezes input
/// </summary>
public class InputPatch : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Game.instance != null && Game.instance.actionSet != null)
            Game.instance.actionSet.ClearAxisState();
    }
}
