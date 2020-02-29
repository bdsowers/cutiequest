using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnlockDialog : Dialog
{
    public Image unlockImage;
    public Image unlockImageBackground;
    public Text unlockTitleText;
    public Text unlockNameText;
    public Text unlockDescText;
    public Image scoutImage;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Update()
    {
        if (Game.instance.actionSet.CloseMenu.WasPressed || Game.instance.actionSet.Activate.WasPressed)
        {
            Close();
        }
    }

    public void ShowWithSpell(Spell spell)
    {
        unlockTitleText.text = "You unlocked a new spell!";
        unlockNameText.text = LocalizedText.Get(spell.friendlyName);
        unlockDescText.text = LocalizedText.Get(spell.description);
        unlockImage.sprite = spell.icon;
        unlockImageBackground.sprite = spell.icon;
        gameObject.SetActive(true);
        scoutImage.gameObject.SetActive(spell.scoutLevel > 0);

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.25f);
    }

    public void ShowWithQuirk(Quirk quirk)
    {
        unlockTitleText.text = "You unlocked a new quirk!";
        unlockNameText.text = LocalizedText.Get(quirk.friendlyName);
        unlockDescText.text = LocalizedText.Get(quirk.description);
        unlockImage.sprite = quirk.icon;
        unlockImageBackground.sprite = quirk.icon;
        gameObject.SetActive(true);
        scoutImage.gameObject.SetActive(quirk.requiredScoutLevel > 0);

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.25f);
    }

    public void ShowWithItem(Item item)
    {
        unlockTitleText.text = "You unlocked a new item!";
        unlockNameText.text = LocalizedText.Get(item.friendlyName);
        unlockDescText.text = LocalizedText.Get(item.description);
        unlockImage.sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
        unlockImageBackground.sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
        gameObject.SetActive(true);
        scoutImage.gameObject.SetActive(item.scoutLevel > 0);

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.25f);
    }

    public void OnConfirmButtonPressed()
    {
        if (transform.localScale.x > 0.8f)
        {
            gameObject.SetActive(false);
        }
    }
}
