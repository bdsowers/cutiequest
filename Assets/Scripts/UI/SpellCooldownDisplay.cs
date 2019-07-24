using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCooldownDisplay : MonoBehaviour
{
    public GameObject inactiveBackground;
    public GameObject activeBackground;
    public Image icon;
    public Image cooldownOverlay;
    public Text cooldownSecondsLabel;

    private int mPreviousDisplaySeconds = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Spell spell = Game.instance.avatar.GetComponentInChildren<Spell>();

        bool canActivate = (spell != null && spell.canActivate && Game.instance.InDungeon());

        if (spell != null)
            icon.sprite = spell.icon;

        inactiveBackground.SetActive(!canActivate);
        activeBackground.SetActive(canActivate);
        cooldownOverlay.gameObject.SetActive(!canActivate);
        cooldownSecondsLabel.gameObject.SetActive(!canActivate);

        int secondsRemaining = (spell == null ? 0 : Mathf.CeilToInt(spell.cooldownTimer));
        if (mPreviousDisplaySeconds != secondsRemaining)
        {
            mPreviousDisplaySeconds = secondsRemaining;
            cooldownSecondsLabel.text = BadAtMathQuirk.ApplyQuirkIfPresent(secondsRemaining).ToString();
        }

        if (spell == null || !Game.instance.InDungeon())
            cooldownOverlay.fillAmount = 1f;
        else
            cooldownOverlay.fillAmount = (spell.cooldownTimer / spell.cooldown);
    }
}
