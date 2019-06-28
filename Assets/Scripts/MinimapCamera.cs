using System.Collections;
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

            SelectClosestMapDisplay();
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

    void SelectClosestMapDisplay()
    {
        DeselectAllMapDisplays();

        List<MapDisplay> interestingDisplays = InterestingMapDisplays();
        if (interestingDisplays.Count == 0)
            return;

        float minDistance = float.MaxValue;
        for (int i = 0; i < interestingDisplays.Count; ++i)
        {
            MapDisplay display = interestingDisplays[i];
            float distance = Vector3.Distance(display.transform.position, Game.instance.avatar.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                mSelectedDisplay = i;
            }
        }

        interestingDisplays[mSelectedDisplay].isSelected = true;
    }

    void SelectMapDisplay(Vector3 direction)
    {
        List<MapDisplay> interestingDispalys = InterestingMapDisplays();
        if (interestingDispalys.Count <= 1)
            return;

        int previousSelection = mSelectedDisplay;
        
        MapDisplay currentDisplay = interestingDispalys[mSelectedDisplay];
        float minDistance = float.MaxValue;
        Debug.Log("-----");
        for (int i = 0; i < interestingDispalys.Count; ++i)
        {
            if (i == mSelectedDisplay)
                continue;

            MapDisplay testDisplay = interestingDispalys[i];
            
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
            interestingDispalys[previousSelection].isSelected = false;
            interestingDispalys[mSelectedDisplay].isSelected = true;
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
}
