using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedMovement : MonoBehaviour
{
    public delegate void TurnGranted();
    public event TurnGranted onTurnGranted;

    public int speed;
    public Sprite turnOrderRepresentation;

    private TurnManager mTurnManager;
    private bool mCanTakeTurns;

    public int effectiveSpeed { get; set; }
    public bool isMyTurn { get; set; }
    public bool canTakeTurns { get { return mCanTakeTurns; } }

    private void Awake()
    {
        mTurnManager = GameObject.FindObjectOfType<TurnManager>();
    }

    public void ActivateTurnMovement()
    {
        if (mCanTakeTurns)
            return;

        mCanTakeTurns = true;
        mTurnManager.AddToActiveList(this);
    }

    public void DeactivateTurnMovement()
    {
        if (!mCanTakeTurns)
            return;

        mCanTakeTurns = false;
        mTurnManager.RemoveFromActiveList(this);
    }

    public void GrantTurn()
    {
        isMyTurn = true;
        
        if (onTurnGranted != null)
        {
            onTurnGranted();
        }
    }

    public void TurnFinished()
    {
        isMyTurn = false;

        mTurnManager.CurrentTurnFinished();
    }

    private void OnDestroy()
    {
        mTurnManager.RemoveFromActiveList(this);
    }
}
