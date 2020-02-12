using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public ProgressBar healthBar;
    public PauseDialog pauseDialog;
    public ProgressBar bossHealth;
    public UnlockDialog unlockDialog;
    public PurchasePrompt purchasePrompt;
    public CheatsDialog cheatsDialog;
    public SettingsDialog settingsDialog;

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
        // null reference guard when order of operations isn't clear
        if (Game.instance.avatar == null)
            return;

        if (Game.instance.avatar.GetComponent<ExternalCharacterStatistics>().externalReference == null)
            return;

        int maxHealth = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        healthBar.SetWithValues(0, maxHealth, newData.health);
    }

    private void Update()
    {

    }
}
