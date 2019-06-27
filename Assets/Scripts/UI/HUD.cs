using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public ProgressBar healthBar;
    public PauseDialog pauseDialog;
    public ProgressBar bossHealth;
    public UnlockDialog unlockDialog;

    private void Start()
    {
        Game.instance.playerData.onPlayerDataChanged += OnPlayerDataChanged;

        OnPlayerDataChanged(Game.instance.playerData);
    }

    private void OnDestroy()
    {
        Game.instance.playerData.onPlayerDataChanged -= OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged(PlayerData newData)
    {
        int maxHealth = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        healthBar.SetWithValues(0, maxHealth, newData.health);
    }

    private void Update()
    {
        
    }
}
