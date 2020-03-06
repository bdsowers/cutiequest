using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsDialog : Dialog
{
    public Button cheatButtonTemplate;
    private Button mPrevButton;

    public bool needsBuilding { get; set; }

    public void AddButton(string text, System.Action listener, string keyboardKey)
    {
        GameObject newButtonObj = GameObject.Instantiate(cheatButtonTemplate.gameObject, cheatButtonTemplate.transform.parent);
        newButtonObj.SetActive(true);
        Button newButton = newButtonObj.GetComponent<Button>();
        newButtonObj.transform.Find("MainText").GetComponent<Text>().text = text;
        newButtonObj.transform.Find("KeyboardKey").GetComponent<Text>().text = keyboardKey;
        newButton.onClick.AddListener(() => listener());

        GetComponentInChildren<ButtonSet>().AddButton(newButton);

        if (mPrevButton != null)
        {
            Navigation prevNav = mPrevButton.navigation;
            Navigation newNav = newButton.navigation;

            prevNav.selectOnRight = newButton;
            newNav.selectOnLeft = mPrevButton;

            mPrevButton.navigation = prevNav;
            newButton.navigation = newNav;
        }

        mPrevButton = newButton;
    }
}
