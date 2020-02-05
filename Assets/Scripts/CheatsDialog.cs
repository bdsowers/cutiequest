using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatsDialog : MonoBehaviour
{
    public Button cheatButtonTemplate;

    public void AddButton(string text, System.Action listener, string keyboardKey)
    {
        GameObject newButtonObj = GameObject.Instantiate(cheatButtonTemplate.gameObject, cheatButtonTemplate.transform.parent);
        newButtonObj.SetActive(true);
        Button newButton = newButtonObj.GetComponent<Button>();
        newButtonObj.transform.Find("MainText").GetComponent<Text>().text = text;
        newButtonObj.transform.Find("KeyboardKey").GetComponent<Text>().text = keyboardKey;
        newButton.onClick.AddListener(() => listener());
    }
}
