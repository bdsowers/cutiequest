using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class SimpleMovement : MonoBehaviour
{
    public delegate void MoveFinished();
    public event MoveFinished onMoveFinished;

    public GameObject mesh;
    public GameObject subMesh;
    public List<int> collisionIgnoreList;
    public int collisionIdentity;

    private bool mIsMoving = false;

    private Vector3 mMeshLocalPosition;
    private CollisionMap mCollisionMap;
    private CharacterStatistics mCharacterStatistics;

    public bool isMoving
    {
        get { return mIsMoving; }
    }

    public bool useCollisionMap
    {
        get; set;
    }

    private void Start()
    {
        mCharacterStatistics = GetComponent<CharacterStatistics>();
        mMeshLocalPosition = mesh.transform.localPosition;

        // todo bdsowers - there's a better way to do this...
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
        useCollisionMap = (mCollisionMap != null);
    }

    public void Move(Vector3 direction, Vector3? absolutePosition = null)
    {
        StartCoroutine(MoveCoroutine(direction, absolutePosition));
    }

    public bool CanMove(Vector3 direction)
    {
        if (!useCollisionMap)
        {
            Ray ray = new Ray(transform.position + new Vector3(0f, 0.2f, 0f),direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, 1f);
            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].collider.gameObject != gameObject && !hits[i].collider.isTrigger)
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            Vector3 targetPos = transform.position + direction;
            Vector2Int coords = targetPos.AsVector2IntUsingXZ();
            int marking = mCollisionMap.SpaceMarking(coords.x, -coords.y);

            return marking == 0 || collisionIgnoreList.Contains(marking);
        }
    }

    private void OnDestroy()
    {
        Vector2Int oldCoords = transform.position.AsVector2IntUsingXZ();
        if (mCollisionMap != null && mCollisionMap.SpaceMarking(oldCoords.x, -oldCoords.y) == collisionIdentity)
        {
            mCollisionMap.MarkSpace(oldCoords.x, -oldCoords.y, 0);
        }
    }

    private void UpdateCollisionMapForMove(Vector3 currentPosition, Vector3 targetPosition)
    {
        if (collisionIdentity < 0)
            return;

        Vector2Int oldCoords = currentPosition.AsVector2IntUsingXZ();
        Vector2Int newCoords = targetPosition.AsVector2IntUsingXZ();

        // Clear us from the old space, but only do it if we are currently registered
        // as being there. Certain overlap scenarios may make this not necessarily true.
        if (mCollisionMap.SpaceMarking(oldCoords.x, -oldCoords.y) == collisionIdentity)
        {
            mCollisionMap.MarkSpace(oldCoords.x, -oldCoords.y, 0);
        }

        mCollisionMap.MarkSpace(newCoords.x, -newCoords.y, collisionIdentity);
    }

    public static void OrientToDirection(GameObject subMesh, Vector3 direction)
    {
        if (subMesh != null)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                if (direction.x < -0.1f)
                {
                    subMesh.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                }
                else if (direction.x > 0.1f)
                {
                    subMesh.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                }
            }
            else
            {
                if (direction.z < -0.1f)
                {
                    subMesh.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (direction.z > 0.1f)
                {
                    subMesh.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
        }
    }

    private IEnumerator MoveCoroutine(Vector3 direction, Vector3? absoluteTargetPosition = null)
    {
        mIsMoving = true;

        Vector3 position = transform.position;
        Vector3 targetPosition = transform.position + direction;

        if (absoluteTargetPosition.HasValue)
            targetPosition = absoluteTargetPosition.Value;

        bool adjustScale = transform.localScale.x > 0.9f;

        RevealWhenAvatarIsClose revealComp = GetComponent<RevealWhenAvatarIsClose>();
        if (revealComp != null)
            adjustScale = revealComp.fullyRevealed;

        if (useCollisionMap)
        {
            UpdateCollisionMapForMove(position, targetPosition);
        }

        OrientToDirection(subMesh, direction);

        float speedMultiplier = 1f;
        if (mCharacterStatistics != null)
            speedMultiplier = 1f + mCharacterStatistics.ModifiedStatValue(CharacterStatType.Speed, gameObject) / 10f;

        float time = 0f;
        while (time < 1f)
        {
            transform.position = Vector3.Lerp(position, targetPosition, time);
            time += Time.deltaTime * 3f * speedMultiplier;

            float bounceY = (time > 0.5 ? 1f - time : time);
            bounceY *= bounceY;
            mesh.transform.localPosition = mMeshLocalPosition + Vector3.up * bounceY;

            float rotation = (time > 0.5f ? 1 - time : time);
            rotation *= rotation;
            float rotationAngle = 60f;
            mesh.transform.localRotation = Quaternion.Euler(direction.z * rotation * rotationAngle, 0f, -direction.x * rotation * rotationAngle);

            float squashTime = 0.3f;
            float stretchTime = 0.75f;

            float scale = 1f;
            if (time < squashTime)
            {
                scale = 1f - time;
            }
            else if (time < stretchTime)
            {
                scale = (1f - squashTime) + time * 0.65f;
            }
            else
            {
                float maxStretch = (1f - squashTime) + stretchTime * 0.75f;

                scale = maxStretch - (time - stretchTime) * (1 - maxStretch);
            }

            if (adjustScale)
            {
                transform.localScale = new Vector3(1f, 1f * scale, 1f);
            }

            yield return null;
        }

        transform.position = targetPosition;
        if (adjustScale) transform.localScale = Vector3.one;
        mesh.transform.localRotation = Quaternion.identity;
        mesh.transform.localPosition = mMeshLocalPosition;

        mIsMoving = false;

        if (onMoveFinished != null)
        {
            onMoveFinished();
        }

        yield break;
    }
}
