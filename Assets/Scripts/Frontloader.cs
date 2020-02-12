using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontloader : MonoBehaviour
{
    public IEnumerator Start()
    {
        SetupResolution();

        yield return new WaitForSeconds(1f);

        Game.instance.transitionManager.TransitionToScreen("Title");
    }

    public void SetupResolution()
    {
        int targetResX = 1920;
        int targetResY = 1080;
        bool fullScreen = true;

        string res = PlayerPrefs.GetString("resolution");
        if (string.IsNullOrEmpty(res))
        {
            Vector2Int chosenResolution = CloseResolution(targetResX, targetResY);

            Debug.Log("Changing resolution to " + chosenResolution);
            Screen.SetResolution(chosenResolution.x, chosenResolution.y, true);
        }
        else
        {
            string[] tokens = res.Split(new char[] { ':' });
            targetResX = int.Parse(tokens[0]);
            targetResY = int.Parse(tokens[1]);
            fullScreen = (int.Parse(tokens[2]) == 0);

            Debug.Log("Changing resolution to " + targetResX + ", " + targetResY);
            Screen.SetResolution(targetResX, targetResY, fullScreen);
        }
    }

    Vector2Int CloseResolution(int targetX, int targetY)
    {
        Resolution[] resolutions = Screen.resolutions;

        float targetAspect = ((float)targetY / (float)targetX);

        // See if resolution already exists
        foreach(Resolution resolution in resolutions)
        {
            if (resolution.width == targetX && resolution.height == targetY)
            {
                return new Vector2Int(targetX, targetY);
            }
        }

        // Prefer the highest resolution that's as close to our aspect ratio as possible.
        List<Resolution> sortedResolutions = new List<Resolution>(resolutions);
        sortedResolutions.Sort((i1, i2) => {
            float i1Aspect = ((float)i1.height / (float)i1.width);
            float i2Aspect = ((float)i2.height / (float)i2.width);

            float i1Diff = Mathf.Abs(targetAspect - i1Aspect);
            float i2Diff = Mathf.Abs(targetAspect - i2Aspect);

            return i1Diff.CompareTo(i2Diff);
        });

        int bestMatch = 0;
        int totalResolution = sortedResolutions[0].width * sortedResolutions[0].height;

        for (int i = 1; i < sortedResolutions.Count; ++i)
        {
            // Sorted based on float values; may be off somewhat ...
            float aspect = (float)sortedResolutions[i].height / (float)sortedResolutions[i].width;
            float prevAspect = (float)sortedResolutions[i - 1].height / (float)sortedResolutions[i].width;
            if (Mathf.Abs(aspect - prevAspect) > 0.001f)
                break;

            Resolution possibleRes = sortedResolutions[i];
            int possibleTotalRes = possibleRes.width * possibleRes.height;

            if (possibleTotalRes > totalResolution)
            {
                bestMatch = i;
                totalResolution = possibleTotalRes;
            }
        }

        return new Vector2Int(sortedResolutions[bestMatch].width, sortedResolutions[bestMatch].height);
    }
}
