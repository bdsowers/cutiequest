using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class EnemyProjectileThrower : EnemyAI
{
    ProjectileThrower mProjectileThrower;
    SimpleMovement mSimpleMovement;

    private void Start()
    {
        mProjectileThrower = GetComponent<ProjectileThrower>();
        mSimpleMovement = GetComponent<SimpleMovement>();
    }

    private void ThrowProjectile()
    {
        SimpleMovement.OrientToDirection(GetComponent<SimpleMovement>().subMesh, AvatarDirection());

        int magic = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, gameObject);
        mProjectileThrower.ThrowProjectile(magic, AvatarDirection());
    }

    private Vector3 AvatarDirection()
    {
        Vector3 avatarDirection = (Game.instance.avatar.transform.position - transform.position);
        avatarDirection.y = 0f;
        avatarDirection.x = Mathf.Round(avatarDirection.x);
        avatarDirection.z = Mathf.Round(avatarDirection.z);

        if (Mathf.Abs(avatarDirection.x) > Mathf.Abs(avatarDirection.z))
            avatarDirection.z = 0f;
        else
            avatarDirection.x = 0f;

        avatarDirection.Normalize();

        return avatarDirection;
    }

    public override void UpdateAI()
    {
        Vector3 largeDistanceDirection = OrthogonalDirection(transform, Game.instance.avatar.transform, true);
        Vector3 smallDistanceDirection = OrthogonalDirection(transform, Game.instance.avatar.transform, false);

        Vector3 diff = transform.position - Game.instance.avatar.transform.position;
        bool alignedOnXAxis = (Mathf.Abs(diff.x) < 0.1f);
        bool alignedOnZAxis = (Mathf.Abs(diff.z) < 0.1f);
        bool alignedOnEitherAxis = (alignedOnXAxis || alignedOnZAxis);

        if (mProjectileThrower.IsInRange())
        {
            if (alignedOnEitherAxis)
            {
                // Large chance we're going to throw but a small chance we'll move in a random direction instead
                if (Random.Range(0, 100) > 15)
                {
                    ThrowProjectile();
                }
                else
                {
                    Vector3 randomDirection = VectorHelper.RandomOrthogonalVectorXZ();
                    if (mSimpleMovement.CanMove(randomDirection))
                    {
                        mSimpleMovement.Move(randomDirection);
                    }
                    else
                    {
                        ThrowProjectile();
                    }
                }
            }
            else
            {
                // If we're not aligned on one of the axis, there's a decent chance we'll try to become aligned;
                // otherwise, we'll just do a throw.
                if (Random.Range(0, 100) < 20)
                {
                    ThrowProjectile();
                }
                else
                {
                    if (mSimpleMovement.CanMove(smallDistanceDirection))
                    {
                        mSimpleMovement.Move(smallDistanceDirection);
                    }
                }
            }
        }
        else
        {
            // If we're not in range, try ot get in range...
            if (mSimpleMovement.CanMove(largeDistanceDirection))
            {
                mSimpleMovement.Move(largeDistanceDirection);
            }
            else if (mSimpleMovement.CanMove(smallDistanceDirection))
            {
                mSimpleMovement.Move(smallDistanceDirection);
            }
        }
    }

    public override bool CanUpdateAI()
    {
        if (mProjectileThrower != null && mProjectileThrower.isThrowing)
            return false;

        return true;
    }
}
