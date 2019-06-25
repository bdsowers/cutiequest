using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnlockDialog : MonoBehaviour
{
    public Image unlockImage;
    public Text unlockTitleText;
    public Text unlockNameText;
    public Text unlockDescText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowWithSpell(Spell spell)
    {
        unlockTitleText.text = "You unlocked a new spell!";
        unlockNameText.text = LocalizedText.Get(spell.friendlyName);
        unlockDescText.text = LocalizedText.Get(spell.description);
        unlockImage.sprite = spell.icon;
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.5f);
    }

    public void ShowWithQuirk(Quirk quirk)
    {
        unlockTitleText.text = "You unlocked a new quirk!";
        unlockNameText.text = LocalizedText.Get(quirk.friendlyName);
        unlockDescText.text = LocalizedText.Get(quirk.description);
        unlockImage.sprite = quirk.icon;
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.5f);
    }

    public void OnConfirmButtonPressed()
    {
        gameObject.SetActive(false);
    }
}
