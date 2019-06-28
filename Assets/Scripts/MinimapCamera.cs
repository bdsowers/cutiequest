using System.Collections;
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

    private int mSelectedDisplay = -1;

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
                SelectMapDisplay(mSelectedDisplay + 1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectMapDisplay(mSelectedDisplay - 1);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {

            }
        }
    }

    private void ToggleFullMap(bool newShowingWholeMap)
    {
        mShowingWholeMap = newShowingWholeMap;

        if (mShowingWholeMap)
        {
            LevelGenerator generator = GameObject.FindObjectOfType<LevelGenerator>();
            float midX = generator.dungeon.width / 2;
            float midY = generator.dungeon.height / 2;
            mCamera.transform.position = new Vector3(midX, mCamera.transform.position.y, -midY);

            mCamera.orthographicSize = Mathf.Max(generator.dungeon.width, generator.dungeon.height) / 2f + 15f;

            SelectMapDisplay(0);
        }
        else
        {
            mCamera.orthographicSize = mOriginalOrthoSize;

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

    void SelectMapDisplay(int displayIndex)
    {
        DeselectAllMapDisplays();

        List<MapDisplay> potentials = InterestingMapDisplays();
        if (potentials.Count == 0)
            return;

        mSelectedDisplay = displayIndex;
        if (mSelectedDisplay < 0)
            mSelectedDisplay = potentials.Count - 1;
        if (mSelectedDisplay >= potentials.Count)
            mSelectedDisplay = 0;

        MapDisplay display = potentials[mSelectedDisplay];
        display.isSelected = true;
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
}
