using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterSelect : MonoBehaviour
{
    public GameObject title;
    public GameObject[] selectableCharacters;

    private int mCurrentSelection = -1;
    private float mControlTimer = 0.55f;

    // Start is called before the first frame update
    void Start()
    {
        title.transform.localScale = new Vector3(0f, 1f, 1f);
        title.transform.DOScaleX(1f, 0.5f);

        for (int i = 0; i < selectableCharacters.Length; ++i)
        {
            selectableCharacters[i].GetComponentInChildren<Animator>().speed = 0f;

            selectableCharacters[i].transform.localScale = Vector3.zero;
            selectableCharacters[i].transform.DOScale(Vector3.one, 0.5f);
        }

        ChangeCharacterSelection(2);
    }

    // Update is called once per frame
    void ChangeCharacterSelection(int newSelection)
    {
        if (mCurrentSelection != -1)
        {
            selectableCharacters[mCurrentSelection].GetComponentInChildren<Animator>().speed = 0f;
            selectableCharacters[mCurrentSelection].transform.DOScale(1f, 0.5f);
        }

        mCurrentSelection = newSelection;
        selectableCharacters[mCurrentSelection].transform.DOScale(1.4f, 0.5f);

        //selectableCharacters[mCurrentSelection].GetComponentInChildren<Animator>().speed = 1f;
    }

    void Update()
    {
        if (mControlTimer > 0f)
        {
            mControlTimer -= Time.deltaTime;
            return;
        }
        
        if (Game.instance.actionSet.MoveLeft.WasPressed && mCurrentSelection > 0)
        {
            transform.DOMoveX(transform.position.x + 4f, 0.5f);
            ChangeCharacterSelection(mCurrentSelection - 1);
            mControlTimer = 0.55f;
        }
        else if (Game.instance.actionSet.MoveRight.WasPressed && mCurrentSelection < 4)
        {
            transform.DOMoveX(transform.position.x - 4f, 0.5f);
            ChangeCharacterSelection(mCurrentSelection + 1);
            mControlTimer = 0.55f;
        }

        
        if (Game.instance.actionSet.Spell.WasPressed)
        {
            Game.instance.playerData.model = selectableCharacters[mCurrentSelection].name;
            string materialName = selectableCharacters[mCurrentSelection].GetComponentInChildren<Renderer>().material.name;
            materialName = materialName.Replace(" (Instance)", "");

            Game.instance.playerData.material = materialName;
            Game.instance.transitionManager.TransitionToScreen("Dungeon");
        }
    }
}
