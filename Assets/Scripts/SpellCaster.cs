using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public int targetDeviation = 0;

    private int[,] mPattern = new int[,]
    {
        { 2, 2, 2, 2, 2 },
        { 2, 1, 1, 1, 2 },
        { 2, 1, 0, 1, 2 },
        { 2, 1, 1, 1, 2 },
        { 2, 2, 2, 2, 2 }
    };

    private CollisionMap mCollisionMap;

    private void Start()
    {
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CastSpell();
        }
    }

    public void CastSpell()
    {
        StartCoroutine(CastSpellCoroutine());
    }

    public IEnumerator CastSpellCoroutine()
    {
        Vector3 avatarPosition = Game.instance.avatar.transform.position;

        int spellX = Mathf.RoundToInt(avatarPosition.x);
        int spellZ = Mathf.RoundToInt(avatarPosition.z);

        if (targetDeviation != 0)
        {
            spellX += Random.Range(-targetDeviation, targetDeviation + 1);
            spellZ += Random.Range(-targetDeviation, targetDeviation + 1);
        }

        spellZ = -spellZ;

        

        int currentPart = 1;
        bool keepCasting = true;
        while (keepCasting)
        {
            keepCasting = CastSpellPart(currentPart, spellX, spellZ);
            ++currentPart;
            yield return new WaitForSeconds(1.2f);
        }

        yield break;
    }

    private bool CastSpellPart(int currentPart, int spellX, int spellZ)
    {
        bool partFound = false;

        int patternWidth = mPattern.GetLength(0);
        int patternHeight = mPattern.GetLength(1);

        for (int x = 0; x < patternWidth; ++x)
        {
            for (int z = 0; z < patternHeight; ++z)
            {
                int offsetX = -patternWidth / 2 + x;
                int offsetZ = -patternHeight / 2 + z;

                int targetX = spellX + offsetX;
                int targetZ = spellZ + offsetZ;

                if (mPattern[x, z] == currentPart && mCollisionMap.SpaceMarking(targetX, targetZ) != 1)
                {
                    GameObject target = GameObject.Instantiate(PrefabManager.instance.PrefabByName("SpellTarget"));
                    target.transform.position = new Vector3(targetX, 1f, -targetZ);
                    target.GetComponent<SpellTarget>().castTime = 1f;
                    target.GetComponent<SpellTarget>().effect = "Explosion";

                    partFound = true;
                }
            }
        }

        return partFound;
    }
}
