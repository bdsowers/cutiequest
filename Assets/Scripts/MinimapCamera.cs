﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public GameObject target;

    public GameObject miniMap;
    public GameObject fullMap;

    private float mOriginalOrthoSize;
    private Camera mCamera;

    private bool mShowingWholeMap = false;

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
            mShowingWholeMap = !mShowingWholeMap;

            if (mShowingWholeMap)
            {
                LevelGenerator generator = GameObject.FindObjectOfType<LevelGenerator>();
                float midX = generator.dungeon.width / 2;
                float midY = generator.dungeon.height / 2;
                mCamera.transform.position = new Vector3(midX, mCamera.transform.position.y, -midY);

                mCamera.orthographicSize = Mathf.Max(generator.dungeon.width, generator.dungeon.height) / 2f + 15f;
            }
            else
            {
                mCamera.orthographicSize = mOriginalOrthoSize;
            }

            fullMap.SetActive(mShowingWholeMap);
            miniMap.SetActive(!mShowingWholeMap);
        }
    }

    private void LateUpdate()
    {
        if (mShowingWholeMap)
            return;

        Vector3 pos = target.transform.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }
}
