using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject heartPrefab;
    public GameObject titleContainer;
    public Text buildNumber;

    private bool mLeaving = false;
    private float mLeaveTimer = 0f;

    private float mStartupDelay = 0.5f;

    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        ResetDemoGame();

        buildNumber.text = "Build " + Game.instance.BUILD_NUMBER;
    }

    void ResetDemoGame()
    {
#if DEMO
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Game.instance.saveManager.LoadGame();
        quitButton.interactable = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (mStartupDelay > 0f)
        {
            mStartupDelay -= Time.deltaTime;
            return;
        }
    }

    public void OnPlay()
    {
        MoveToNextScene();
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void OnCredits()
    {

    }

    public void OnSettings()
    {
        Game.instance.hud.settingsDialog.gameObject.SetActive(true);
    }

    void MoveToNextScene()
    {
        if (mLeaving)
            return;

        mLeaving = true;

        if (Game.instance.finishedTutorial && Game.instance.playerData.model != null)
        {
            Game.instance.transitionManager.TransitionToScreen("HUB");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelect");
        }
    }
}
