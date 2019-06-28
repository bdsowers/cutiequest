﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class MinimapCamera : MonoBehaviour
{
    public GameObject target;

    public GameObject miniMap;
    public GameObject fullMap;

    private float mOriginalOrthoSize;
    private Camera mCamera;

    private bool mShowingWholeMap = false;

    private int mSelectedDisplay = -1;

    public bool showingWholeMap {  get { return mShowingWholeMap; } }

    private List<MapDisplay> mInterestingDisplays;

    public GameObject teleportWarning;

    // Start is called before the first frame update
    void Start()
    {
        mCamera = GetComponent<Camera>();
        mOriginalOrthoSize = mCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleFullMap(!mShowingWholeMap);
        }

        if (mShowingWholeMap)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectMapDisplay(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectMapDisplay(Vector3.right);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SelectMapDisplay(Vector3.forward);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SelectMapDisplay(Vector3.back);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && CanTeleport())
            {
                Teleport();
                ToggleFullMap(false);
            }
        }
    }

    private void ToggleFullMap(bool newShowingWholeMap)
    {
        mShowingWholeMap = newShowingWholeMap;
        teleportWarning.SetActive(false);

        if (mShowingWholeMap)
        {
            LevelGenerator generator = GameObject.FindObjectOfType<LevelGenerator>();
            float midX = generator.dungeon.width / 2;
            float midY = generator.dungeon.height / 2;
            mCamera.transform.position = new Vector3(midX, mCamera.transform.position.y, -midY);

            mCamera.orthographicSize = Mathf.Max(generator.dungeon.width, generator.dungeon.height) / 2f + 15f;

            mInterestingDisplays = InterestingMapDisplays();
            SelectClosestMapDisplay();

            if (!CanTeleport())
            {
                teleportWarning.SetActive(true);
            }
        }
        else
        {
            mCamera.orthographicSize = mOriginalOrthoSize;

            mInterestingDisplays.Clear();
            DeselectAllMapDisplays();
        }

        fullMap.SetActive(mShowingWholeMap);
        miniMap.SetActive(!mShowingWholeMap);
    }
    
    List<MapDisplay> InterestingMapDisplays()
    {
        MapDisplay[] displays = GameObject.FindObjectsOfType<MapDisplay>();
        List<MapDisplay> interesting = new List<MapDisplay>();
        for (int i = 0; i < displays.Length; ++i)
        {
            if (displays[i].displayType == MapDisplayType.Chest ||
                displays[i].displayType == MapDisplayType.Exit ||
                displays[i].displayType == MapDisplayType.Secret ||
                displays[i].displayType == MapDisplayType.Shop)
            {
                if (displays[i].GetComponentInParent<RevealWhenAvatarIsClose>().fullyRevealed)
                {
                    interesting.Add(displays[i]);
                }
            }
        }
        return interesting;
    }

    void SelectClosestMapDisplay()
    {
        DeselectAllMapDisplays();

        if (mInterestingDisplays.Count == 0)
            return;

        float minDistance = float.MaxValue;
        for (int i = 0; i < mInterestingDisplays.Count; ++i)
        {
            MapDisplay display = mInterestingDisplays[i];
            float distance = Vector3.Distance(display.transform.position, Game.instance.avatar.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                mSelectedDisplay = i;
            }
        }

        mInterestingDisplays[mSelectedDisplay].isSelected = true;
    }

    void SelectMapDisplay(Vector3 direction)
    {
        if (mInterestingDisplays.Count <= 1)
            return;

        int previousSelection = mSelectedDisplay;
        
        MapDisplay currentDisplay = mInterestingDisplays[mSelectedDisplay];
        float minDistance = float.MaxValue;
        Debug.Log("-----");
        for (int i = 0; i < mInterestingDisplays.Count; ++i)
        {
            if (i == mSelectedDisplay)
                continue;

            MapDisplay testDisplay = mInterestingDisplays[i];
            
            // Find the one with the minimum distance that isn't in the other 
            Vector3 testDirection = testDisplay.transform.position.WithZeroY() - currentDisplay.transform.position.WithZeroY();
            float distance = VectorHelper.OrthogonalDistance(testDisplay.transform.position.WithZeroY(), currentDisplay.transform.position.WithZeroY());
            testDirection.Normalize();
            float dot = Vector3.Dot(testDirection, direction);

            Debug.Log(testDisplay.gameObject.name + "   " + distance);
            if (dot > 0.01f && distance < minDistance)
            {
                minDistance = distance;
                mSelectedDisplay = i;
            }
        }

        
        if (previousSelection != mSelectedDisplay)
        {
            mInterestingDisplays[previousSelection].isSelected = false;
            mInterestingDisplays[mSelectedDisplay].isSelected = true;
        }
    }

    void DeselectAllMapDisplays()
    {
        MapDisplay[] displays = GameObject.FindObjectsOfType<MapDisplay>();
        for (int i = 0; i < displays.Length; ++i)
        {
            displays[i].isSelected = false;
        }

        mSelectedDisplay = -1;
    }

    private void LateUpdate()
    {
        if (mShowingWholeMap)
            return;

        Vector3 pos = target.transform.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }

    private bool CanTeleport()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; ++i)
        {
            float distance = Vector3.Distance(Game.instance.avatar.transform.position, enemies[i].transform.position);

            if (distance < 7f)
                return false;
        }
        return true;
    }

    private void Teleport()
    {
        if (mInterestingDisplays.Count <= 1 || mSelectedDisplay == -1)
            return;

        MapDisplay targetDisplay = mInterestingDisplays[mSelectedDisplay];
        Vector2Int target = MapCoordinateHelper.WorldToMapCoords(targetDisplay.transform.position);
        Game.instance.avatar.QueueTeleportation(target);
    }
}
