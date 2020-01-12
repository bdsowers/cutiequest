using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class FireStaffTrap : PlacedTrap
{
    public ParticleSystem castingFX;
    private Vector3 castingFXScale;

    public SpellCaster spellCaster;

    private float mCooldown;

    // Start is called before the first frame update
    void Start()
    {
        castingFXScale = castingFX.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (mCooldown > 0f)
        {
            mCooldown -= Time.deltaTime;
            return;
        }

        // See if the player is in range and in a straight line, and if so, cast!
        if (Vector3.Distance(transform.position, Game.instance.avatar.transform.position) < 3f)
        {
            // Cast toward the player ...
            Vector3 direction = Game.instance.avatar.transform.position - spellCaster.transform.position;
            direction.y = 0f;
            direction.Normalize();
            SimpleMovement.OrientToDirection(spellCaster.gameObject, direction);
            spellCaster.gameObject.transform.Rotate(0f, -90f, 0f);

            castingFX.gameObject.SetActive(true);
            castingFX.enableEmission = true;
            castingFX.transform.localScale = castingFXScale;

            spellCaster.CastSpell(12);
            mCooldown = 6f;
            StartCoroutine(DisableVFX());
        }
    }

    private IEnumerator DisableVFX()
    {
        yield return new WaitForSeconds(3.5f);
        castingFX.enableEmission = false;

        float time = 1f;
        while (time > 0f)
        {
            time -= Time.deltaTime * 2f;
            castingFX.transform.localScale = castingFXScale * time;
            yield return null;
        }

        castingFX.gameObject.SetActive(false);

        yield break;
    }

    public override bool CanSpawn(List<Vector2Int> region)
    {
        return true;
    }

    public override void Spawn(List<Vector2Int> region, LevelGenerator levelGenerator)
    {
        int maxTries = 50;
        while (maxTries > 0)
        {
            Vector2Int pos = region.Sample();
            maxTries--;

            if (levelGenerator.collisionMap.SpaceMarking(pos.x, pos.y) == 0)
            {
                // Placed successfully
                maxTries = 0;

                GameObject trapObj = levelGenerator.PlaceMapPrefab("FireStaffTrap", pos.x, pos.y);
                Game.instance.levelGenerator.collisionMap.MarkSpace(pos.x, pos.y, 6);
            }
        }
    }
}
