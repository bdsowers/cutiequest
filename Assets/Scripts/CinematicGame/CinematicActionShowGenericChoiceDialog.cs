using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionShowGenericChoiceDialog : CinematicAction
{
    public override string actionName => "generic_choice";

    private string mText;
    private List<DialogButton> mButtons = new List<DialogButton>();

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mButtons.Clear();

        mText = dataProvider.GetStringData(mParameters, "text");

        foreach (string key in mParameters.Keys)
            Debug.Log(key);

        int buttonIndex = 1;
        bool keepGoing = true;
        while (keepGoing)
        {
            string buttonName = dataProvider.GetStringData(mParameters, "button" + buttonIndex);
            if (string.IsNullOrEmpty(buttonName))
            {
                keepGoing = false;
                continue;
            }

            string buttonText = dataProvider.GetStringData(mParameters, "button" + buttonIndex + "_text");
            string buttonIcon = dataProvider.GetStringData(mParameters, "button" + buttonIndex + "_icon");

            DialogButton button = new DialogButton()
            {
                name = buttonName,
                text = buttonText,
                icon = null // todo bdsowers
            };

            mButtons.Add(button);

            buttonIndex++;
        }
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject dialog = player.objectMap.GetObjectByName("choice_dialog");
        dialog.GetComponent<GenericChoiceDialog>().Show(mText, mButtons);

        while (dialog.activeInHierarchy)
            yield return null;

        yield break;
    }
}
