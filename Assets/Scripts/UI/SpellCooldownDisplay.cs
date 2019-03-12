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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Spell spell = Game.instance.avatar.GetComponentInChildren<Spell>();
        if (spell == null)
            return;

        icon.sprite = spell.icon;

        inactiveBackground.SetActive(!spell.canActivate);
        activeBackground.SetActive(spell.canActivate);
        cooldownOverlay.gameObject.SetActive(!spell.canActivate);
        cooldownSecondsLabel.gameObject.SetActive(!spell.canActivate);

        int secondsRemaining = Mathf.CeilToInt(spell.cooldownTimer);
        cooldownSecondsLabel.text = secondsRemaining.ToString();

        cooldownOverlay.fillAmount = (spell.cooldownTimer / spell.cooldown);
    }
}
