using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject heartPrefab;
    public GameObject titleContainer;

    private bool mLeaving = false;
    private float mLeaveTimer = 0f;

    private float mStartupDelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = -5; x <= 5; ++x)
        {
            for (int y = -5; y <= 5; ++y)
            {
                float offset = 0f;

                if (y <= 0)
                    offset = -1f;

                Vector3 pos = new Vector3(x * 3.75f, offset + y * 4f, 0f);
                GameObject newHeart = GameObject.Instantiate(heartPrefab);
                newHeart.transform.position = pos;
            }
        }

        // QQQ ResetDemoGame();
    }

    void ResetDemoGame()
    {
#if DEMO
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Game.instance.saveManager.LoadGame();
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

        if (Input.anyKeyDown && !Game.instance.transitionManager.isTransitioning)
            Leave();

        if (mLeaving)
        {
            mLeaveTimer += Time.deltaTime * 6f;
            mLeaveTimer = Mathf.Min(mLeaveTimer, 1f);
            titleContainer.transform.localScale = new Vector3(1f - mLeaveTimer, 1f, 1f);
        }
    }

    void Leave()
    {
        if (mLeaving)
            return;

        mLeaving = true;

        TitleHeart[] hearts = GameObject.FindObjectsOfType<TitleHeart>();
        for (int i = 0; i < hearts.Length; ++i)
        {
            hearts[i].MakeDisappear();
        }

        Invoke("MoveToNextScene", 0.6f);
    }

    void MoveToNextScene()
    {
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
