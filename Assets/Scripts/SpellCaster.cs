﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public int targetDeviation = 0;
    public bool targetCaster;
    public bool orientToDirection;

    [TextArea(8, 10)]
    public string patternString;

    public bool isCasting { get; private set; }

    private int[,] mPattern = null;

    private CollisionMap mCollisionMap;
    private float timeScale = 4f;

    private void Start()
    {
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
        mPattern = ParsePatternString(patternString);
    }

    private int[,] ParsePatternString(string str)
    {
        string[] lines = str.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        int[,] result = new int[lines[0].Length, lines.Length];
        
        for (int x = 0; x < lines[0].Length; ++x)
        {
            for (int y = 0; y < lines.Length; ++y)
            {
                result[x, y] = int.Parse(lines[y][x].ToString());
            }
        }

        return result;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CastSpell();
        }
    }

    public bool CanCast()
    {
        return Vector3.Distance(transform.position, Game.instance.avatar.transform.position) < 4f;
    }

    public void CastSpell()
    {
        StartCoroutine(CastSpellCoroutine());
    }

    public IEnumerator CastSpellCoroutine()
    {
        GetComponentInChildren<Animator>().Play("Spell");

        isCasting = true;

        Vector3 avatarPosition = Game.instance.avatar.transform.position;

        int spellX = Mathf.RoundToInt(avatarPosition.x);
        int spellZ = Mathf.RoundToInt(avatarPosition.z);

        if (targetDeviation != 0)
        {
            spellX += Random.Range(-targetDeviation, targetDeviation + 1);
            spellZ += Random.Range(-targetDeviation, targetDeviation + 1);
        }

        if (targetCaster)
        {
            spellX = Mathf.RoundToInt(transform.position.x);
            spellZ = Mathf.RoundToInt(transform.position.z);
        }

        spellZ = -spellZ;

        int currentPart = 1;
        bool keepCasting = true;
        while (keepCasting)
        {
            keepCasting = CastSpellPart(currentPart, spellX, spellZ);
            ++currentPart;
            yield return new WaitForSeconds(1.75f / timeScale);
        }

        isCasting = false;

        yield break;
    }

    private bool CastSpellPart(int currentPart, int spellX, int spellZ)
    {
        bool partFound = false;

        int patternWidth = mPattern.GetLength(0);
        int patternHeight = mPattern.GetLength(1);

        GameObject spellContainer = new GameObject();
        spellContainer.transform.position = transform.position;

        for (int x = 0; x < patternWidth; ++x)
        {
            for (int z = 0; z < patternHeight; ++z)
            {
                int offsetX = -patternWidth / 2 + x;
                int offsetZ = -patternHeight / 2 + z;

                int targetX = spellX + offsetX;
                int targetZ = spellZ + offsetZ;

                if (mPattern[x, z] == currentPart)
                {
                    GameObject target = GameObject.Instantiate(PrefabManager.instance.PrefabByName("SpellTarget"), spellContainer.transform);
                    target.transform.position = new Vector3(targetX, 1f, -targetZ);
                    target.GetComponent<SpellTarget>().castTime = 1f * (1f / timeScale);
                    target.GetComponent<SpellTarget>().effect = "Explosion";

                    partFound = true;
                }
            }
        }

        if (orientToDirection)
        {
            spellContainer.transform.localRotation = GetComponentInChildren<Animator>().transform.localRotation;

            // Round the Y rotation to the nearest 90 degree interval; root motion makes the rotation a little imprecise.
            float y = spellContainer.transform.localRotation.eulerAngles.y;
            y = Mathf.Round(y / 90) * 90.0f;
            spellContainer.transform.localRotation = Quaternion.Euler(spellContainer.transform.localRotation.eulerAngles.x, y, spellContainer.transform.localRotation.eulerAngles.z);
        }

        return partFound;
    }
}