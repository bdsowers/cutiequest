﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartDisplay : MonoBehaviour
{
    public Text amountLabel;
    
    // Start is called before the first frame update
    void Start()
    {
        Game.instance.playerData.onPlayerDataChanged += OnPlayerDataChanged;
        UpdateLabel();
    }

    private void OnDestroy()
    {
        Game.instance.playerData.onPlayerDataChanged -= OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged(PlayerData newData)
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        amountLabel.text = Game.instance.playerData.numHearts.ToString();
    }
}
