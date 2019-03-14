using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransitionManager : MonoBehaviour
{
    public Image fullScreenQuad;

    public void TransitionToScreen(string name)
    {
        StartCoroutine(StandardTransition(name));
    }

    private IEnumerator TransitionToNextDungeonLevel()
    {
        yield break;
    }

    private IEnumerator TransitionToFirstDungeonLevel()
    {
        yield break;
    }

    private IEnumerator StandardTransition(string targetScene)
    {
        fullScreenQuad.gameObject.SetActive(true);

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            fullScreenQuad.color = new Color(0f, 0f, 0f, time);
            yield return null;
        }

        fullScreenQuad.color = new Color(0f, 0f, 0f, 1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);

        while (time > 0f)
        {
            time -= Time.deltaTime;
            fullScreenQuad.color = new Color(0f, 0f, 0f, time);
            yield return null;
        }

        fullScreenQuad.gameObject.SetActive(false);

        yield break;
    }
}
