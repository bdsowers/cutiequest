using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ColorExtensions;
using ArrayExtensions;

public class ScreenTransitionManager : MonoBehaviour
{
    public Image fullScreenQuad;
    public Typewriter deathMessage;
    public RawImage deathSpeakerImage;

    public bool isTransitioning { get; private set; }

    private GameObject mCharacterImageCapture;

    private string[] mDeathMessages = new string[]
    {
        "Maybe we should see other adventurers.",
        "I'm sorry, I just don't think you're the hero for me.",
        "Call me when you've got your life sorted out.",
        "It's not you. Well, I mean, it kinda is.",
        "I just think we're in different places in our lives, y'know?"
    };

    public void TransitionToScreen(string name)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dungeon" &&
            name == "Dungeon")
        {
            StartCoroutine(TransitionToNextDungeonLevel());
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dungeon" &&
            name == "HUB")
        {
            StartCoroutine(DeathTransition());
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

        yield return StartCoroutine(Fade(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1)));
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);

        yield return StartCoroutine(Fade(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0)));

        fullScreenQuad.gameObject.SetActive(false);

        isTransitioning = false;

        yield break;
    }

    private IEnumerator DeathTransition()
    {
        isTransitioning = true;

        deathMessage.GetComponentInChildren<Text>().CrossFadeAlpha(1f, 0f, false);

        yield return StartCoroutine(Fade(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1)));

        mCharacterImageCapture = GameObject.Find("CharacterImageCapture");
        mCharacterImageCapture.GetComponentInChildren<CharacterModel>().ChangeModel(Game.instance.followerData.model);

        deathSpeakerImage.gameObject.SetActive(true);
        StartCoroutine(FadeDeathSpeaker(new Color(0, 0, 0, 0), new Color(1, 1, 1, 1)));

        yield return deathMessage.ShowTextCoroutine(mDeathMessages.Sample(), 1f);
        yield return new WaitForSeconds(1f);

        deathMessage.GetComponentInChildren<Text>().CrossFadeAlpha(0f, 0.5f, false);
        yield return StartCoroutine(FadeDeathSpeaker(new Color(1, 1, 1, 1), new Color(0, 0, 0, 0)));

        deathSpeakerImage.gameObject.SetActive(false);

        UnityEngine.SceneManagement.SceneManager.LoadScene("HUB");

        yield return StartCoroutine(Fade(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0)));

        fullScreenQuad.gameObject.SetActive(false);
        
        deathMessage.gameObject.SetActive(false);

        isTransitioning = false;

        yield break;
    }

    private IEnumerator Fade(Color startColor, Color targetColor)
    {
        fullScreenQuad.gameObject.SetActive(true);

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            fullScreenQuad.color = ColorHelper.Lerp(startColor, targetColor, time);
            yield return null;
        }

        fullScreenQuad.color = targetColor;

        yield break;
    }

    private IEnumerator FadeDeathSpeaker(Color startColor, Color targetColor)
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            Color color = ColorHelper.Lerp(startColor, targetColor, time);
            deathSpeakerImage.color = color;
            yield return null;
        }

        deathSpeakerImage.color = targetColor;
        
        yield break;
    }
}
