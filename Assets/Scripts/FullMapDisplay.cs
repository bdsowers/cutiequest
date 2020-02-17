using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;
using TMPro;

public class FullMapDisplay : MonoBehaviour
{
    private List<MapDisplay> mInterestingDisplays;

    public GameObject teleportWarning;
    public GameObject normalControls;

    private int mSelectedDisplay = -1;

    private float mReselectDelay = 0f;
    private string mNormalControlMsg;

    private void OnEnable()
    {
        if (mNormalControlMsg == null)
            mNormalControlMsg = normalControls.GetComponent<TextMeshProUGUI>().text;

        normalControls.GetComponent<TextMeshProUGUI>().SetText(ActionGlyphMapper.ReplaceActionCodesWithGlyphs(mNormalControlMsg));

        mInterestingDisplays = InterestingMapDisplays();
        SelectClosestMapDisplay();
    }

    private void OnDisable()
    {
        mInterestingDisplays.Clear();
        DeselectAllMapDisplays();
    }

    private bool CanTeleport()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; ++i)
        {
            if (!enemies[i].enabled)
                continue;

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

        GetComponentInParent<QuestR>().gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (teleportWarning != null)
        {
            teleportWarning.SetActive(!CanTeleport());
        }

        if (normalControls != null)
        {
            // Keep normal controls visible always normalControls.SetActive(CanTeleport());
        }
    }

    private void Update()
    {
        if (mReselectDelay > 0f)
            mReselectDelay -= Time.deltaTime;

        if (Game.instance.actionSet.MoveLeft.WasPressed && mReselectDelay <= 0f)
        {
            SelectMapDisplay(-1);
        }
        else if (Game.instance.actionSet.MoveRight.WasPressed && mReselectDelay <= 0f)
        {
            SelectMapDisplay(1);
        }
        else if ((Game.instance.actionSet.Activate.WasPressed || Game.instance.actionSet.Spell.WasPressed) && CanTeleport())
        {
            Teleport();
        }
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
        interesting.Sort((i1, i2) => {
            return i1.transform.position.x.CompareTo(i2.transform.position.x);
        });

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

    void SelectMapDisplay(int direction)
    {
        if (mInterestingDisplays.Count <= 1)
            return;

        int previousSelection = mSelectedDisplay;
        mSelectedDisplay = mSelectedDisplay + direction;
        if (mSelectedDisplay < 0)
            mSelectedDisplay = mInterestingDisplays.Count - 1;
        else if (mSelectedDisplay >= mInterestingDisplays.Count)
            mSelectedDisplay = 0;

        if (previousSelection != mSelectedDisplay)
        {
            mInterestingDisplays[previousSelection].isSelected = false;
            mInterestingDisplays[mSelectedDisplay].isSelected = true;
        }

        mReselectDelay = 0.25f;
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
}
