using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class SimpleMovement : MonoBehaviour
{
    public delegate void MoveFinished();
    public event MoveFinished onMoveFinished;

    public GameObject mesh;
    public List<int> collisionIgnoreList;
    public int collisionIdentity;

    private bool mIsMoving = false;

    private Vector3 mMeshLocalPosition;
    private CollisionMap mCollisionMap;

    public bool isMoving
    {
        get { return mIsMoving; }
    }


    private void Start()
    {
        mMeshLocalPosition = mesh.transform.localPosition;

        // todo bdsowers - there's a better way to do this...
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();
    }

    public void Move(Vector3 direction)
    {
        StartCoroutine(MoveCoroutine(direction));
    }

    public bool CanMove(Vector3 direction)
    {
        Vector3 targetPos = transform.position + direction;
        Vector2Int coords = targetPos.AsVector2IntUsingXZ();
        int marking = mCollisionMap.SpaceMarking(coords.x, -coords.y);

        return marking == 0 || collisionIgnoreList.Contains(marking);
    }

    private void OnDestroy()
    {
        Vector2Int oldCoords = transform.position.AsVector2IntUsingXZ();
        if (mCollisionMap.SpaceMarking(oldCoords.x, -oldCoords.y) == collisionIdentity)
        {
            mCollisionMap.MarkSpace(oldCoords.x, -oldCoords.y, 0);
        }
    }

    private IEnumerator MoveCoroutine(Vector3 direction)
    {
        mIsMoving = true;

        Vector3 position = transform.position;
        Vector3 targetPosition = transform.position + direction;

        Vector2Int oldCoords = position.AsVector2IntUsingXZ();
        Vector2Int newCoords = targetPosition.AsVector2IntUsingXZ();

        bool adjustScale = transform.localScale.x > 0.1f;

        // Clear us from the old space, but only do it if we are currently registered
        // as being there. Certain overlap scenarios may make this not necessarily true.
        if (mCollisionMap.SpaceMarking(oldCoords.x, -oldCoords.y) == collisionIdentity)
        {
            mCollisionMap.MarkSpace(oldCoords.x, -oldCoords.y, 0);
        }

        mCollisionMap.MarkSpace(newCoords.x, -newCoords.y, collisionIdentity);
        
        float time = 0f;
        while (time < 1f)
        {
            transform.position = Vector3.Lerp(position, targetPosition, time);
            time += Time.deltaTime * 3f;

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
        transform.localScale = Vector3.one;
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
