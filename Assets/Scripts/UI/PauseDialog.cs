using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseDialog : MonoBehaviour
{
    public GameObject leaveDungeonButton;
    public GenericChoiceDialog genericChoiceDialog;

    private void Start()
    {
        leaveDungeonButton.GetComponent<Button>().interactable = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dungeon");
    }

    public void OnReturnToGamePressed()
    {
        gameObject.SetActive(false);
    }

    public void OnLeaveDungeonPressed()
    {
        gameObject.SetActive(false);
        Game.instance.avatar.GetComponent<Killable>().TakeDamage(Game.instance.playerData.health);
    }

    public void OnSettingsPressed()
    {
        // todo bdsowers
        gameObject.SetActive(false);
    }

    public void OnQuitPressed()
    {
        DialogButton yes = new DialogButton();
        yes.name = "button_yes";
        yes.text = "Yes";

        DialogButton no = new DialogButton();
        no.name = "button_no";
        no.text = "No";

        genericChoiceDialog.Show("Are you sure you want to quit?", new List<DialogButton>() { yes, no });
        genericChoiceDialog.onDialogButtonPressed += OnQuitDialogButtonPressed;
    }

    private void OnQuitDialogButtonPressed(string buttonName)
    {
        if (buttonName == "button_yes")
        {
            Application.Quit();
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
