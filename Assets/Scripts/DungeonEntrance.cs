﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DungeonEntrance : MonoBehaviour
{
    public DungeonData dungeonData;
    public GameObject visualDungeonEntrance;

    private bool mActive = false;

    public string entranceId;

    private Vector3 mBlockerContainerOffset = new Vector3(0f, 0f, 15f);
    private bool mIsDoorOpen = true;
    private bool mIsAnimating = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            if (Game.instance.followerData != null)
            {
                EnterDungeon();
            }
        }
    }

    private void Start()
    {
        visualDungeonEntrance.transform.localPosition = mBlockerContainerOffset;
    }

    private void EnterDungeon()
    {
        // todo bdsowers - if the player has hearts left, we need to ask them if they really
        // want to dungeon dive before entering and resetting their hearts.
        Game.instance.EnterDungeon(dungeonData, entranceId);

        Game.instance.transitionManager.TransitionToScreen("Dungeon");
    }

    private void Update()
    {
        if (mActive && Game.instance.followerData == null)
        {
            mActive = false;
        }

        if (!mActive && Game.instance.followerData != null)
        {
            mActive = true;
        }

        if (mActive && !mIsDoorOpen && !mIsAnimating)
        {
            StartCoroutine(OpenDoor());
        }

        if (!mActive && mIsDoorOpen && !mIsAnimating)
        {
            StartCoroutine(CloseDoor());
        }
    }

    private IEnumerator CloseDoor()
    {
        mIsAnimating = true;
        mIsDoorOpen = false;

        visualDungeonEntrance.transform.DOLocalMove(Vector3.zero, 1f);
        yield return new WaitForSeconds(1f);

        mIsAnimating = false;
        yield break;
    }

    private IEnumerator OpenDoor()
    {
        mIsAnimating = true;
        mIsDoorOpen = true;

        visualDungeonEntrance.transform.DOLocalMove(mBlockerContainerOffset, 1f);
        yield return new WaitForSeconds(1f);

        mIsAnimating = false;

        yield break;
    }
}
