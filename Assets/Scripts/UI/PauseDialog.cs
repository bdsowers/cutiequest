using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

// todo bdsowers - this whole damn file is a hack in preparation for a con
// the Time.timeScale stuff is wonky, as well as the way DisableScreen is called.
// It was all rooted in trying to get inputs that manipulate the dialog to not also
// trigger spells.
public class PauseDialog : Dialog
{
    public Button returnToGameButton;
    public Button leaveDungeonButton;
    public Button settingsButton;
    public GenericChoiceDialog genericChoiceDialog;

    private void Start()
    {
        leaveDungeonButton.interactable = Game.instance.InDungeon() && Game.instance.finishedTutorial;

        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnDown = (leaveDungeonButton.interactable ? leaveDungeonButton : settingsButton);
        returnToGameButton.navigation = nav;
    }

    public void OnReturnToGamePressed()
    {
        Time.timeScale = 1f;
        Invoke("DisableScreen", 0.01f);
    }

    public void OnLeaveDungeonPressed()
    {
        Time.timeScale = 1f;
        Invoke("DisableScreen", 0.01f);
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
        genericChoiceDialog.onDialogButtonPressed -= OnQuitDialogButtonPressed;

        if (buttonName == "button_yes")
        {
#if DEMO || RELEASE
            Game.instance.CloseGame();
#else
            Game.instance.CheckRatingDialog(true);
#endif
        }
        else
        {
            returnToGameButton.Select();
        }
    }

    private void DisableScreen()
    {
        gameObject.SetActive(false);
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
