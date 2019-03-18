using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransitionManager : MonoBehaviour
{
    public Image fullScreenQuad;

    public bool isTransitioning { get; private set; }

    public void TransitionToScreen(string name)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dungeon" &&
            name == "Dungeon")
        {
            StartCoroutine(TransitionToNextDungeonLevel());
        }
        else
        {
            StartCoroutine(StandardTransition(name));
        }
    }

    private IEnumerator TransitionToNextDungeonLevel()
    {
        isTransitioning = true;

        SnapToGround[] snappers = GameObject.FindObjectsOfType<SnapToGround>();
        for (int i = 0; i < snappers.Length; ++i)
        {
            snappers[i].enabled = false;
        }

        RevealWhenAvatarIsClose[] entities = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
        for (int i = 0; i < entities.Length; ++i)
        {
            StartCoroutine(Drop(entities[i].gameObject));
        }

        yield return new WaitForSeconds(0.5f);

        Game.instance.avatar.GetComponentInChildren<Animator>().Play("Falling");
        GameObject.FindObjectOfType<Follower>().gameObject.GetComponentInChildren<Animator>().Play("Falling");

        yield return new WaitForSeconds(0.75f);
        StartCoroutine(StandardTransition("Dungeon"));

        // Purposefully not setting isTransitioning to false here - the StandardTransition will do that at the right time.
        yield break;
    }

    private IEnumerator TransitionToFirstDungeonLevel()
    {
        yield break;
    }

    private IEnumerator Drop(GameObject obj)
    {
        Vector3 current = obj.transform.position;
        Vector3 target = current + Vector3.down * 45f;
        Vector3 slightUpward = current + Vector3.up * Random.Range(1f, 2f);

        Vector3 currentScale = obj.transform.localScale;

        float time = -Random.Range(0f, 1f);
        while (time < 1f)
        {
            time += Time.deltaTime * 4f;
            if (time >= 0f)
            {
                obj.transform.position = Vector3.Lerp(current, slightUpward, time);
            }
            yield return null;
        }

        current = obj.transform.position;

        time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 0.75f;
            obj.transform.position = Vector3.Lerp(current, target, time);
            obj.transform.localScale = Vector3.Lerp(currentScale, Vector3.zero, time);
            yield return null;
        }

        obj.transform.localScale = Vector3.zero;

        yield break;
    }

    private IEnumerator StandardTransition(string targetScene)
    {
        isTransitioning = true;

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

        isTransitioning = false;

        yield break;
    }
}
