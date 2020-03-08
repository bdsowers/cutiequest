using System.Collections;
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

    private bool mEntering = false;

    public bool freezeFollowerWhenEntering;

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
        if (mEntering) return;

        mEntering = true;

        Game.instance.soundManager.PlaySound("warp");

        Game.instance.EnterDungeon(dungeonData, entranceId);

        Game.instance.transitionManager.TransitionToScreen("Dungeon");

        if (Game.instance.avatar != null && Game.instance.avatar.follower != null && freezeFollowerWhenEntering)
            Game.instance.avatar.follower.Freeze();
    }

    private void Update()
    {
        if (mEntering && Game.instance.avatar != null)
        {
            Game.instance.avatar.transform.position += new Vector3(0, 0, 1) * Time.deltaTime * 2f;

            if (!freezeFollowerWhenEntering && Game.instance.avatar.follower != null)
            {
                Game.instance.avatar.follower.transform.position += new Vector3(0, 0, 1) * Time.deltaTime * 2f;
            }
        }

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
