using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    public CharacterStatType statType;
    private Text mLabel;

    private Text label
    {
        get
        {
            if (mLabel == null)
            {
                mLabel = GetComponent<Text>();
            }

            return mLabel;
        }
    }
    private void Start()
    {
        Game.instance.playerData.onPlayerDataChanged += OnPlayerDataChanged;
    }

    private void OnEnable()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        int value = Game.instance.playerStats.BaseStatValue(statType);
        label.text = value.ToString();
    }

    private void OnPlayerDataChanged(PlayerData newData)
    {
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        Game.instance.playerData.onPlayerDataChanged -= OnPlayerDataChanged;
    }
}
