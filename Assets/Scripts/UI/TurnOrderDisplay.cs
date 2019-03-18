using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderDisplay : MonoBehaviour
{
    public GameObject trackerTemplate;

    private List<TurnOrderTracker> mTrackers = new List<TurnOrderTracker>();
    private List<TurnBasedMovement> mTrackedEntities;

    public void UpdateList(List<TurnBasedMovement> entities)
    {
        mTrackedEntities = entities;

        int unique = NumUnique(entities);
        int trackersToShow = 0;
        if (unique > 1)
        {
            trackersToShow = entities.Count;
        }

        for (int i = 0; i < trackersToShow; ++i)
        {
            CreateTrackerForEntity(entities[i], i);
        }
    }

    private bool EntityBeingTracked(TurnBasedMovement entity)
    {
        for (int i = 0; i < mTrackers.Count; ++i)
        {
            if (mTrackers[i].trackedEntity == entity)
            {
                return true;
            }
        }

        return false;
    }

    private int NumUnique(List<TurnBasedMovement> entities)
    {
        List<TurnBasedMovement> seen = new List<TurnBasedMovement>();
        for (int i = 0; i < entities.Count; ++i)
        {
            if (!seen.Contains(entities[i]))
            {
                seen.Add(entities[i]);
            }
        }

        return seen.Count;
    }

    private void ClearTrackers()
    {
        for (int i = 0; i < mTrackers.Count; ++i)
        {
            Destroy(mTrackers[i].gameObject);
        }
        mTrackers.Clear();
    }

    private void CreateTrackerForEntity(TurnBasedMovement entity, int position)
    {
        float separation = 125f;

        GameObject trackerObj = null;
        if (position < mTrackers.Count)
        {
            if (position == 0)
            {
                // Slide the left-most one off the screen and destroy it afterward
                StartCoroutine(SlideTracker(mTrackers[0], mTrackers[0].transform.position + Vector3.left * 200f, true));
            }
            else
            {
                // Slide everything else over 1
                StartCoroutine(SlideTracker(mTrackers[position], mTrackers[position].transform.position + Vector3.left * separation, false));
            }
        }
        else
        {
            trackerObj = GameObject.Instantiate(trackerTemplate, transform);
            trackerObj.gameObject.SetActive(true);
            trackerObj.GetComponentInChildren<Image>().enabled = true;
            trackerObj.transform.position = trackerTemplate.transform.position + Vector3.right * separation * position;

            trackerObj.GetComponentInChildren<Image>().sprite = entity.turnOrderRepresentation;
            TurnOrderTracker tracker = trackerObj.GetComponent<TurnOrderTracker>();
            tracker.trackedEntity = entity;
            mTrackers.Add(tracker);
            StartCoroutine(ScaleTrackerIn(tracker, position * 0.01f));
        }
    }

    private IEnumerator SlideTracker(TurnOrderTracker tracker, Vector3 targetPosition, bool destroyAfter)
    {
        float time = 0f;
        Vector3 currentPosition = tracker.transform.position;
        
        while (time < 1f)
        {
            time += Time.deltaTime * 5f;
            tracker.transform.position = Vector3.Lerp(currentPosition, targetPosition, time);
            yield return null;
        }

        tracker.transform.position = targetPosition;

        if (destroyAfter)
        {
            mTrackers.Remove(tracker);
            Destroy(tracker.gameObject);

            if (mTrackers.Count < mTrackedEntities.Count)
            {
                CreateTrackerForEntity(mTrackedEntities[mTrackedEntities.Count - 1], mTrackedEntities.Count - 1);
            }
        }

        yield break;
    }

    private IEnumerator ScaleTrackerIn(TurnOrderTracker tracker, float delay)
    {
        tracker.transform.localScale = Vector3.zero;
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 5f;
            tracker.transform.localScale = Vector3.one * time;
            yield return null;
        }

        tracker.transform.localScale = Vector3.one;
        yield break;
    }
}
