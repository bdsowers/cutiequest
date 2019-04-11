using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckpicQuirk : Quirk
{
    private float mTimer;

    private void Start()
    {
        mTimer = Random.Range(30, 120);
    }

    private void Update()
    {
        mTimer -= Time.deltaTime;

        if (mTimer < 0f)
        {
            mTimer = Random.Range(30, 120);

            DungeonCanvas dc = GameObject.FindObjectOfType<DungeonCanvas>();
            if (dc != null)
            {
                ShowDuckpic(dc);
            }
        }
    }

    void ShowDuckpic(DungeonCanvas canvas)
    {
        GameObject.Find("CharacterImageCapture").GetComponentInChildren<CharacterModel>().ChangeModel(Game.instance.followerData.model);
        canvas.duckpicUI.gameObject.SetActive(true);
    }
}
