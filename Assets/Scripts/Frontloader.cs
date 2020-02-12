using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontloader : MonoBehaviour
{
    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        Game.instance.transitionManager.TransitionToScreen("Title");

        SetupResolution();
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
            Screen.SetResolution(chosenResolution.x, chosenResolution.y, true);
        }
        else
        {
            string[] tokens = res.Split(new char[] { ':' });
            targetResX = int.Parse(tokens[0]);
            targetResY = int.Parse(tokens[1]);
            fullScreen = (int.Parse(tokens[2]) == 0);

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

        // Find a close resolution
        // Try to stay close with the aspect ratio
        // Barring that, don't go below 800 x 600
        // Barring THAT, just pick one.
        float bestMatchWeight = 0;
        int bestMatch = 0;
        foreach(Resolution resolution in resolutions)
        {
            float aspect = (resolution.height / resolution.width);

            float aspectDiff = Mathf.Abs(targetAspect - aspect);
        }

        return new Vector2Int(resolutions[bestMatch].width, resolutions[bestMatch].height);
    }
}
