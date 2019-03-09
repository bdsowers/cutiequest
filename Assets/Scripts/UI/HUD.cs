﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public ProgressBar healthBar;

    private void Start()
    {
        Game.instance.playerData.onPlayerDataChanged += OnPlayerDataChanged;
    }

    private void OnDestroy()
    {
        Game.instance.playerData.onPlayerDataChanged -= OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged(PlayerData newData)
    {
        healthBar.SetWithValues(0, newData.maxHealth, newData.health);
    }

    private void Update()
    {
        
    }
}