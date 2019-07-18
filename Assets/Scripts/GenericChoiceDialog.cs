using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct DialogButton
{
    public string name;
    public string text;
    public Sprite icon;
}

public class GenericChoiceDialog : Dialog
{
    public Text label;
    public GameObject buttonTemplate;

    public delegate void DialogButtonPressed(string buttonName);
    public event DialogButtonPressed onDialogButtonPressed;

    private List<GameObject> mButtons = new List<GameObject>();

    public void Show(string text, List<DialogButton> buttons)
    {
        Clear();

        label.text = text;

        for (int i = 0; i < buttons.Count; ++i)
        {
            GameObject newButton = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            newButton.transform.GetChild(0).GetComponentInChildren<Text>().text = buttons[i].text;
            newButton.name = buttons[i].name;
            newButton.transform.GetChild(0).GetComponentInChildren<Image>().sprite = buttons[i].icon;
            newButton.transform.GetChild(0).GetComponentInChildren<Image>().gameObject.SetActive(buttons[i].icon != null);
            newButton.SetActive(true);
            string buttonName = buttons[i].name;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnButtonPressed(buttonName));
            mButtons.Add(newButton);
        }

        // Hook up controller navigation
        for (int i = 0; i < buttons.Count; ++i)
        {
            Navigation buttonNav = new Navigation();
            buttonNav.mode = Navigation.Mode.Explicit;
            buttonNav.selectOnLeft = (i == 0 ? null : mButtons[i - 1].GetComponent<Button>());
            buttonNav.selectOnRight = (i == buttons.Count - 1 ? null : mButtons[i + 1].GetComponent<Button>());
            mButtons[i].GetComponent<Button>().navigation = buttonNav;
        }

        gameObject.SetActive(true);

        StartCoroutine(SelectButton());
    }

    private IEnumerator SelectButton()
    {
        yield return null;
        mButtons[0].GetComponent<Button>().Select();
        yield break;
    }

    private void Clear()
    {
        for (int i = 0; i < mButtons.Count; ++i)
        {
            Destroy(mButtons[i]);
        }
        mButtons.Clear();
    }

    public void OnButtonPressed(string buttonName)
    {
        Game.instance.cinematicDataProvider.SetData("dialog_choice", buttonName);

        gameObject.SetActive(false);

        if (onDialogButtonPressed != null)
        {
            onDialogButtonPressed(buttonName);
        }
    }
}
