using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionMoveCamera : CinematicAction
{
    public override string actionName
    {
        get { return "move_camera"; }
    }

    private string mTargetTransform;
    private float mTime;

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTargetTransform = dataProvider.GetStringData(mParameters, "target");
        mTime = dataProvider.GetFloatData(mParameters, "time", 1f);
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject target = player.objectMap.GetObjectByName(mTargetTransform);
        Quaternion currentRotation = Camera.main.transform.rotation;
        Vector3 currentPosition = Camera.main.transform.position;
        FollowCamera follow = Camera.main.GetComponent<FollowCamera>();
        follow.enabled = false;

        float time = 0f;
        while (time < 1f)
        {
            Quaternion rotation = Quaternion.Slerp(currentRotation, target.transform.rotation, time);
            Vector3 position = Vector3.Lerp(currentPosition, target.transform.position, time);
            Camera.main.transform.rotation = rotation;
            Camera.main.transform.position = position;

            time += Time.deltaTime / mTime;

            yield return null;
        }

        Camera.main.transform.rotation = target.transform.rotation;
        Camera.main.transform.position = target.transform.position;

        yield return new WaitForSeconds(4f);

        // TODO bdsowers: this shouldn't move back, but we don't need a separate action/bookkeeping right now
        while (time > 0f)
        {
            Quaternion rotation = Quaternion.Slerp(currentRotation, target.transform.rotation, time);
            Vector3 position = Vector3.Lerp(currentPosition, target.transform.position, time);
            Camera.main.transform.rotation = rotation;
            Camera.main.transform.position = position;

            time -= Time.deltaTime / mTime;

            yield return null;
        }

        Camera.main.transform.rotation = currentRotation;
        Camera.main.transform.position = currentPosition;
        follow.enabled = true;

        yield break;
    }
}
