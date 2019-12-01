using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionCameraShake : CinematicAction
{
    float mIntensity;
    float mSpeed;
    float mSeconds;

    public override string actionName
    {
        get
        {
            return "camera_shake";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "intensity";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);
        
        mIntensity = dataProvider.GetFloatData(mParameters, "intensity", 1f);
        mSpeed = dataProvider.GetFloatData(mParameters, "speed", 1f);
        mSeconds = dataProvider.GetFloatData(mParameters, "seconds", 1f);
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        float seconds = 0f;
        FollowCamera cameraController = Camera.main.GetComponent<FollowCamera>();
        while (seconds < mSeconds)
        {
            Vector3 offset = cameraController.animationOffset;
            offset.x = Mathf.Sin(seconds * mSpeed * 2f * 3.1415f) * mIntensity;
            offset.y = Mathf.Sin(seconds * mSpeed * 2f * 3.1415f * 0.25f) * mIntensity;

            cameraController.animationOffset = offset;
            seconds += Time.deltaTime;
            yield return null;
        }

        while (cameraController.animationOffset.magnitude > 0.1f)
        {
            cameraController.animationOffset *= 0.5f;
            yield return null;
        }

        cameraController.animationOffset = Vector3.zero;

        yield break;
    }
}