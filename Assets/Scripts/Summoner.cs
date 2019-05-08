using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class Summoner : MonoBehaviour
{
    public GameObject[] summonedEntities;
    public int numEnemiesToSummon;
    public int maxRadius;
    public int minRadius;

    public bool isSummoning { get; private set; }

    private CollisionMap mCollisionMap;
    public float castSpeed = 4f;

    private void Start()
    {
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
    }

    public void CastSummon()
    {
        StartCoroutine(CastSummonCoroutine());
    }

    public IEnumerator CastSummonCoroutine()
    {
        // todo bdsowers - UGH
        SimpleMovement root = GetComponentInParent<SimpleMovement>();
        if (root != null && !DancePartyQuirk.quirkEnabled)
        {
            root.GetComponentInChildren<Animator>().Play("Spell");
        }

        isSummoning = true;

        yield return new WaitForSeconds(1.75f);

        Summon();

        yield return new WaitForSeconds(1f);

        isSummoning = false;

        yield break;
    }

    private void Summon()
    {
        
    }
}
