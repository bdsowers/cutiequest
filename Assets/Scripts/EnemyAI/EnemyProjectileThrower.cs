using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class EnemyProjectileThrower : EnemyAI
{
    ProjectileThrower mProjectileThrower;
    SimpleMovement mSimpleMovement;
    CollisionMap mCollisionMap;
    EnemyTeleport mTeleport;

    int mThrowCounter = 0;

    private GameObject target
    {
        get
        {
            return Decoy.instance != null ? Decoy.instance.gameObject : Game.instance.avatar.gameObject;
        }
    }

    private void Start()
    {
        mProjectileThrower = GetComponentInChildren<ProjectileThrower>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
        mTeleport = GetComponent<EnemyTeleport>();
    }

    public override void AIStructureChanged()
    {
        mProjectileThrower = GetComponentInChildren<ProjectileThrower>();
    }

    private void ThrowProjectile()
    {
        SimpleMovement.OrientToDirection(GetComponent<SimpleMovement>().subMesh, TargetDirection());

        int magic = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, gameObject);

        float speedModifier = 1f;
        if (Game.instance.playerStats.IsItemEquipped<BlackCoffee>())
            speedModifier = 0.65f;

        mProjectileThrower.ThrowProjectile(magic, TargetDirection(), null, speedModifier);

        mThrowCounter = 2;
    }

    // The projectile thrower AI is intentionally a little clunky, but they shouldn't try throwing
    // stuff at walls right next to them. This detects if that might be the case.
    private bool WouldThrowProjectileAtWall()
    {
        Vector3 direction = TargetDirection();
        Vector2Int currentMapCoords = MapCoordinateHelper.WorldToMapCoords(transform.position);
        currentMapCoords.x += Mathf.FloorToInt(direction.x);
        currentMapCoords.y += -Mathf.FloorToInt(direction.z);
        if (mCollisionMap.SpaceMarking(currentMapCoords.x, currentMapCoords.y) == 1)
            return true;

        return false;
    }

    private Vector3 TargetDirection()
    {
        Vector3 targetDirection = (target.transform.position - transform.position);
        targetDirection.y = 0f;
        targetDirection.x = Mathf.Round(targetDirection.x);
        targetDirection.z = Mathf.Round(targetDirection.z);

        if (Mathf.Abs(targetDirection.x) > Mathf.Abs(targetDirection.z))
            targetDirection.z = 0f;
        else
            targetDirection.x = 0f;

        targetDirection.Normalize();

        return targetDirection;
    }

    public override void UpdateAI()
    {
        Vector3 largeDistanceDirection = OrthogonalDirection(transform, target.transform, true);
        Vector3 smallDistanceDirection = OrthogonalDirection(transform, target.transform, false);

        Vector3 diff = transform.position - target.transform.position;
        bool alignedOnXAxis = (Mathf.Abs(diff.x) < 0.1f);
        bool alignedOnZAxis = (Mathf.Abs(diff.z) < 0.1f);
        bool alignedOnEitherAxis = (alignedOnXAxis || alignedOnZAxis);

        --mThrowCounter;

        if (mTeleport != null && mTeleport.ShouldTeleport())
        {
            mTeleport.Teleport();
        }
        else if (mProjectileThrower.IsInRange())
        {
            if (alignedOnEitherAxis)
            {
                // Large chance we're going to throw but a small chance we'll move in a random direction instead
                if (Random.Range(0, 100) > 15 && mThrowCounter <= 0 && !WouldThrowProjectileAtWall())
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
                    else if (!WouldThrowProjectileAtWall())
                    {
                        ThrowProjectile();
                    }
                }
            }
            else
            {
                // If we're not aligned on one of the axis, there's a decent chance we'll try to become aligned;
                // otherwise, we'll just do a throw.
                if (Random.Range(0, 100) < 20 && mThrowCounter <= 0 && !WouldThrowProjectileAtWall())
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
