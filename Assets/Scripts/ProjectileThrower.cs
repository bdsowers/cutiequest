using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    public GameObject projectilePrefab;

    public bool isThrowing { get; private set; }

    public bool ShouldThrow()
    {
        return Vector3.Distance(Game.instance.avatar.transform.position, transform.position) < 4f;
    }

    public void ThrowProjectile()
    {
        StartCoroutine(ThrowProjectileCoroutine());
    }

    public IEnumerator ThrowProjectileCoroutine()
    {
        isThrowing = true;
        Vector3 avatarDirection = (Game.instance.avatar.transform.position - transform.position);
        avatarDirection.y = 0f;
        avatarDirection.x = Mathf.Round(avatarDirection.x);
        avatarDirection.z = Mathf.Round(avatarDirection.z);
        if (Mathf.Abs(avatarDirection.x) > Mathf.Abs(avatarDirection.z))
            avatarDirection.z = 0f;
        else
            avatarDirection.x = 0f;

        avatarDirection.Normalize();

        SimpleMovement.OrientToDirection(GetComponent<SimpleMovement>().subMesh, avatarDirection);
        GetComponentInChildren<Animator>().Play("Throw", 0, 0f);
        yield return new WaitForSeconds(0.75f);

        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.GetComponent<ConstantTranslation>().direction = avatarDirection;
        Transform handTransform = GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        projectile.transform.position = handTransform.position;

        yield return new WaitForSeconds(2f);

        isThrowing = false;

        yield break;
    }
}
