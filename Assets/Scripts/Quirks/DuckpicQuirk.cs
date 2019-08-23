using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckpicQuirk : Quirk
{
    private float mTimer;

    public override void Start()
    {
        base.Start();

        mTimer = Random.Range(25, 75);
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
        CharacterModel model = GameObject.Find("CharacterImageCapture").GetComponentInChildren<CharacterModel>();
        model.transform.localPosition = Vector3.zero;
        model.ChangeModel(Game.instance.followerData);
        canvas.duckpicUI.gameObject.SetActive(true);
    }
}
