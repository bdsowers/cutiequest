using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDialog : Dialog
{
    public GameObject currentSelectionHighlight;
    public GameObject[] selections;

    public GameObject buttonSet;

    public Text resolutionText;
    public Image fullScreenCheckbox;
    public ProgressBar musicVolume;
    public ProgressBar sfxVolume;

    public GameObject applyButton;

    const int RESOLUTION = 0;
    const int FULL_SCREEN = 1;
    const int MUSIC_VOLUME = 2;
    const int SFX_VOLUME = 3;

    private int mCurrentSelection = 0;

    private bool mFullScreen;
    private int mSelectedResolution = -1;
    private Resolution[] mAvailableResolutions;

    private bool mScreenSettingsChanged = false;

    private void OnEnable()
    {
        buttonSet.GetComponent<ButtonSet>().ForceUnfocus();

        mScreenSettingsChanged = false;
        mCurrentSelection = 0;
        mFullScreen = Screen.fullScreen;
        mAvailableResolutions = Screen.resolutions;

        mSelectedResolution = System.Array.IndexOf(mAvailableResolutions, Screen.currentResolution);
        if (mSelectedResolution < 0)
            mSelectedResolution = 0;

        UpdateVisuals();
    }

    public override void Update()
    {
        base.Update();

        if (Game.instance.actionSet.MoveDown.WasPressed)
        {
            mCurrentSelection = Mathf.Min(mCurrentSelection + 1, selections.Length - 1);

            if (mCurrentSelection == selections.Length - 1)
            {
                buttonSet.GetComponent<ButtonSet>().TakeFocus();
            }

            UpdateVisuals();
        }
        if (Game.instance.actionSet.MoveUp.WasPressed)
        {
            mCurrentSelection = Mathf.Max(mCurrentSelection - 1, 0);

            buttonSet.GetComponent<ButtonSet>().ForceUnfocus();

            UpdateVisuals();
        }

        // Don't account for settings changes while there's a large value in the Y direction
        if (Mathf.Abs(Game.instance.actionSet.Move.Y) > 0.3f)
            return;

        if (Game.instance.actionSet.MoveLeft.WasPressed)
        {
            if (mCurrentSelection == MUSIC_VOLUME)
            {
                Game.instance.soundManager.MusicVolume -= 0.2f;
            }
            if (mCurrentSelection == SFX_VOLUME)
            {
                Game.instance.soundManager.SFXVolume -= 0.2f;
            }
            if (mCurrentSelection == RESOLUTION)
            {
                mSelectedResolution = (mSelectedResolution + 1) % mAvailableResolutions.Length;

                mScreenSettingsChanged = true;
                UpdateVisuals();
            }

            UpdateVisuals();
        }
        else if (Game.instance.actionSet.MoveRight.WasPressed)
        {
            if (mCurrentSelection == MUSIC_VOLUME)
            {
                Game.instance.soundManager.MusicVolume += 0.2f;
            }
            if (mCurrentSelection == SFX_VOLUME)
            {
                Game.instance.soundManager.SFXVolume += 0.2f;
            }
            if (mCurrentSelection == RESOLUTION)
            {
                mSelectedResolution = mSelectedResolution - 1;
                if (mSelectedResolution < 0)
                    mSelectedResolution = mAvailableResolutions.Length - 1;

                mScreenSettingsChanged = true;
                UpdateVisuals();
            }

            UpdateVisuals();
        }

        if (Game.instance.actionSet.Activate.WasPressed ||
            Game.instance.actionSet.Spell.WasPressed)
        {
            if (mCurrentSelection == FULL_SCREEN)
            {
                mFullScreen = !mFullScreen;
                mScreenSettingsChanged = true;
                UpdateVisuals();
            }
        }
    }

    private void UpdateVisuals()
    {
        currentSelectionHighlight.transform.position = selections[mCurrentSelection].transform.position;
        currentSelectionHighlight.gameObject.SetActive(mCurrentSelection != selections.Length - 1);

        musicVolume.SetWithPercent(Game.instance.soundManager.MusicVolume);
        sfxVolume.SetWithPercent(Game.instance.soundManager.SFXVolume);
        fullScreenCheckbox.gameObject.SetActive(mFullScreen);

        Resolution res = mAvailableResolutions[mSelectedResolution];
        resolutionText.text = res.width + " x " + res.height;

        applyButton.SetActive(mScreenSettingsChanged);
    }

    public void OnApply()
    {
        Resolution finalSelection = mAvailableResolutions[mSelectedResolution];
        ResolutionManager.ChangeResolution(finalSelection.width, finalSelection.height, mFullScreen);

        Close();
    }

    public void OnClose()
    {
        Close();
    }
}
