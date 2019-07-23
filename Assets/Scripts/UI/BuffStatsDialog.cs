using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffStatsDialog : Dialog
{
    public Text maxHealthCostLabel;
    public Text strengthCostLabel;
    public Text defenseCostLabel;
    public Text magicCostLabel;
    public Text speedCostLabel;
    public Text luckCostLabel;

    public void ShowDialog()
    {
        maxHealthCostLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(UpgradeCost(CharacterStatType.MaxHealth)).ToString();
        strengthCostLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(UpgradeCost(CharacterStatType.Strength)).ToString();
        defenseCostLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(UpgradeCost(CharacterStatType.Defense)).ToString();
        magicCostLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(UpgradeCost(CharacterStatType.Magic)).ToString();
        speedCostLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(UpgradeCost(CharacterStatType.Speed)).ToString();
        luckCostLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(UpgradeCost(CharacterStatType.Luck)).ToString();

        gameObject.SetActive(true);
    }

    private int UpgradeCost(CharacterStatType statType)
    {
        int level = Game.instance.playerStats.BaseStatValue(statType);
        if (statType == CharacterStatType.MaxHealth)
            level = (level - 90) / 10;

        int cost = Mathf.RoundToInt(Mathf.Pow(2, level));
        return cost;
    }

    public void OnStatButtonPressed(string stat)
    {
        CharacterStatType statSelected = CharacterStatistics.StatTypeFromString(stat);

        int currentLevel = Game.instance.playerStats.BaseStatValue(statSelected);
        int cost = UpgradeCost(statSelected);

        if (Game.instance.playerData.numHearts >= cost)
        {
            Game.instance.playerData.numHearts -= cost;
            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 0.7f, cost, NumberPopupReason.RemoveHearts);

            if (statSelected == CharacterStatType.MaxHealth)
            {
                Game.instance.playerStats.ChangeBaseStat(statSelected, currentLevel + 10);
                Game.instance.playerData.health = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
            }
            else
            {
                Game.instance.playerStats.ChangeBaseStat(statSelected, currentLevel + 1);
            }

            Game.instance.cinematicDirector.PostCinematicEvent("trainer_success");

            Game.instance.soundManager.PlaySound("confirm_special");
        }
        else
        {
            Game.instance.cinematicDirector.PostCinematicEvent("trainer_fail");
        }

        Close();
    }

    public void OnCancelPressed()
    {
        Close();
    }
}
