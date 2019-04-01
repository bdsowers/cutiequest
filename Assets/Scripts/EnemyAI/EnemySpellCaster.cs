using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellCaster : EnemyAI
{
    SpellCaster mSpellCaster;
    SimpleMovement mSimpleMovement;

    private void Start()
    {
        mSpellCaster = GetComponent<SpellCaster>();
        mSimpleMovement = GetComponent<SimpleMovement>();
    }

    public override void UpdateAI()
    {
        Vector3 direction = OrthogonalDirection(transform, Game.instance.avatar.transform, true);

        if (mSpellCaster != null && mSpellCaster.CanCast())
        {
            int magic = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, gameObject);
            mSpellCaster.CastSpell(magic);
        }
        else if (mSimpleMovement.CanMove(direction))
        {
            mSimpleMovement.Move(direction);
        }
        else
        {
            direction = OrthogonalDirection(transform, GameObject.Find("Avatar").transform, false);
            if (mSimpleMovement.CanMove(direction))
            {
                mSimpleMovement.Move(direction);
            }
        }
    }

    public override bool CanUpdateAI()
    {
        if (mSpellCaster != null && mSpellCaster.isCasting)
            return false;

        return true;
    }
}
