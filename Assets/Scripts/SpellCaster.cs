using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class SpellCaster : MonoBehaviour
{
    public int targetDeviation = 0;
    public bool targetCaster;
    public bool orientToDirection;
    public bool randomOrientation;
    public bool showTargetArea = true;

    [TextArea(8, 10)]
    public string patternString;

    public bool isCasting { get; private set; }

    private int[,] mPattern = null;

    private CollisionMap mCollisionMap;
    public float castSpeed = 4f;

    public string effectName;
    public Vector3 effectOffset;

    public bool hideEffectIfNoHit;

    public string tag;

    private GameObject target
    {
        get
        {
            return Decoy.instance != null ? Decoy.instance.gameObject : Game.instance.avatar.gameObject;
        }
    }

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
            CastSpell(2);
        }
    }

    public bool IsInRange()
    {
        float range = 0f;
        if (targetCaster)
        {
            range = mPattern.GetLength(0) + 1;
        }
        else
        {
            range = 5f;
        }

        if (ClingyQuirk.quirkEnabled)
            range = 2f;

        return Vector3.Distance(transform.position, target.transform.position) < range;
    }

    public void CastSpell(int strength)
    {
        StartCoroutine(CastSpellCoroutine(strength));
    }

    public IEnumerator CastSpellCoroutine(int strength)
    {
        // todo bdsowers - UGH
        SimpleMovement root = GetComponentInParent<SimpleMovement>();
        if (root != null && !DancePartyQuirk.quirkEnabled)
        {
            root.GetComponentInChildren<Animator>().Play("Spell");
        }

        isCasting = true;

        Vector3 targetPosition = target.transform.position;

        int spellX = Mathf.RoundToInt(targetPosition.x);
        int spellZ = Mathf.RoundToInt(targetPosition.z);

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
            keepCasting = CastSpellPart(currentPart, spellX, spellZ, strength);
            ++currentPart;
            yield return new WaitForSeconds(1.75f / castSpeed);
        }

        isCasting = false;

        yield break;
    }

    private bool CastSpellPart(int currentPart, int spellX, int spellZ, int strength)
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
                    GameObject targetObj = GameObject.Instantiate(PrefabManager.instance.PrefabByName("SpellTarget"), spellContainer.transform);
                    SpellTarget target = targetObj.GetComponent<SpellTarget>();
                    targetObj.transform.position = new Vector3(targetX, 1f, -targetZ);
                    target.castTime = 1f * (1f / castSpeed);
                    target.strength = strength;
                    targetObj.SetLayerRecursive(gameObject.layer);
                    targetObj.GetComponentInChildren<Renderer>().enabled = showTargetArea;
                    target.effect = effectName;
                    target.hideEffectIfNoHit = hideEffectIfNoHit;
                    target.effectOffset = effectOffset;

                    partFound = true;
                }
            }
        }

        if (orientToDirection)
        {
            spellContainer.transform.localRotation = GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().transform.localRotation;

            // Round the Y rotation to the nearest 90 degree interval; root motion makes the rotation a little imprecise.
            float y = spellContainer.transform.localRotation.eulerAngles.y;
            y = Mathf.Round(y / 90) * 90.0f;
            spellContainer.transform.localRotation = Quaternion.Euler(spellContainer.transform.localRotation.eulerAngles.x, y, spellContainer.transform.localRotation.eulerAngles.z);
        }

        if (randomOrientation)
        {
            spellContainer.transform.localRotation = GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().transform.localRotation;

            // Round the Y rotation to the nearest 90 degree interval; root motion makes the rotation a little imprecise.
            float y = Random.Range(0, 4) * 90;
            spellContainer.transform.localRotation = Quaternion.Euler(spellContainer.transform.localRotation.eulerAngles.x, y, spellContainer.transform.localRotation.eulerAngles.z);
        }

        return partFound;
    }

    // Helper functions for manipulating lists of spell casters
    public static SpellCaster SpellCasterWithTag(SpellCaster[] casters, string tag)
    {
        for (int i = 0; i < casters.Length; ++i)
        {
            if (casters[i].tag == tag)
                return casters[i];
        }

        return null;
    }

    public static bool AnySpellCastersCasting(SpellCaster[] casters)
    {
        for (int i = 0; i < casters.Length; ++i)
        {
            if (casters[i].isCasting)
                return true;
        }

        return false;
    }
}
