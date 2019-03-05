using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private List<TurnBasedMovement> mOrderList = new List<TurnBasedMovement>();

    public void AddToActiveList(TurnBasedMovement entity)
    {
        mOrderList.Add(entity);

        // New enemies always appear at the end of the list
        entity.effectiveSpeed = LargestEffectiveSpeed() + entity.speed;

        if (mOrderList.Count == 1)
        {
            MoveToNextTurn();
        }
    }

    public void RemoveFromActiveList(TurnBasedMovement entity)
    {
        mOrderList.Remove(entity);

        if (entity.isMyTurn)
        {
            MoveToNextTurn();
        }
    }

    public void CurrentTurnFinished()
    {
        MoveToNextTurn();
    }

    private void MoveToNextTurn()
    {
        if (Game.instance.realTime)
            return;

        // todo bdsowers - move this out of here, this shouldn't be changing view code
        /*
        TurnOrderDisplay display = GameObject.FindObjectOfType<TurnOrderDisplay>();
        List<TurnBasedMovement> nextList = WhoIsNext(5);
        display.UpdateList(nextList);*/

        TurnBasedMovement next = WhoIsNext();
        next.effectiveSpeed += next.speed;
        next.GrantTurn();
    }

    public TurnBasedMovement WhoIsNext()
    {
        if (mOrderList.Count == 0)
            return null;

        int minPosition = 0;
        for (int i = 0; i < mOrderList.Count; ++i)
        {
            if (mOrderList[i].effectiveSpeed < mOrderList[minPosition].effectiveSpeed)
            {
                minPosition = i;
            }
        }

        return mOrderList[minPosition];
    }

    public List<TurnBasedMovement> WhoIsNext(int future)
    {
        List<TurnBasedMovement> result = new List<TurnBasedMovement>();
        Dictionary<TurnBasedMovement, int> originalEffectiveSpeeds = new Dictionary<TurnBasedMovement, int>();

        while (future > 0)
        {
            TurnBasedMovement next = WhoIsNext();
            result.Add(next);

            if (!originalEffectiveSpeeds.ContainsKey(next))
            {
                originalEffectiveSpeeds.Add(next, next.effectiveSpeed);
            }

            next.effectiveSpeed += next.speed;
            future--;
        }

        // Return all effective speeds to what they once were
        foreach(KeyValuePair<TurnBasedMovement, int> pair in originalEffectiveSpeeds)
        {
            pair.Key.effectiveSpeed = pair.Value;
        }

        return result;
    }

    public int LargestEffectiveSpeed()
    {
        if (mOrderList.Count == 0)
            return 0;

        int maxPosition = 0;
        for (int i = 0; i < mOrderList.Count; ++i)
        {
            if (mOrderList[i].effectiveSpeed > mOrderList[maxPosition].effectiveSpeed)
            {
                maxPosition = i;
            }
        }
        return mOrderList[maxPosition].effectiveSpeed;
    }
}
