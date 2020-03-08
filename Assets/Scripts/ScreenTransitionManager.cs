using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ColorExtensions;
using ArrayExtensions;
using TMPro;

public class ScreenTransitionManager : MonoBehaviour
{
    public Image fullScreenQuad;
    public Typewriter deathMessage;
    public RawImage deathSpeakerImage;
    public Text intermission;

    public bool isTransitioning { get; private set; }

    private GameObject mCharacterImageCapture;

    public void TransitionToScreen(string name, string extraData = null)
    {
        if (Game.instance.activeScene == "Dungeon" &&
            name == "Dungeon")
        {
            StartCoroutine(TransitionToNextDungeonLevel());
        }
        else if (Game.instance.activeScene == "Dungeon" &&
            name == "HUB")
        {
            StartCoroutine(DeathTransition(extraData));
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
        Follower follower = GameObject.FindObjectOfType<Follower>();
        if (follower != null)
        {
            follower.gameObject.GetComponentInChildren<Animator>().Play("Falling");
        }

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

    private IEnumerator ShowIntermission()
    {
        intermission.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            intermission.transform.localPosition = Vector3.zero + Vector3.right * Random.Range(-2f, 2f) + Vector3.up * Random.Range(-2f, 2f);

            yield return null;
        }

        intermission.gameObject.SetActive(false);

        yield break;
    }

    private IEnumerator StandardTransition(string targetScene)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1)));

        yield return StartCoroutine(ChangeScene(targetScene));

        if (Game.instance.quirkRegistry.IsQuirkActive<OldTimeyQuirk>())
        {
            yield return StartCoroutine(ShowIntermission());
        }

        yield return StartCoroutine(Fade(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0)));

        fullScreenQuad.gameObject.SetActive(false);

        if (!Game.instance.finishedTutorial && Game.instance.InDungeon())
        {
            yield return new WaitForSeconds(0.2f);
            Game.instance.cinematicDirector.PostCinematicEvent("intro_ready");
        }

        isTransitioning = false;

        yield break;
    }

    private IEnumerator DeathTransition(string extraData)
    {
        isTransitioning = true;

        deathMessage.GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(1f, 0f, false);

        yield return StartCoroutine(Fade(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1)));

        mCharacterImageCapture = GameObject.Find("CharacterImageCapture");
        CharacterModel model = mCharacterImageCapture.GetComponentInChildren<CharacterModel>();

        model.transform.localPosition = Vector3.zero;
        model.ChangeModel(Game.instance.followerData);

        deathSpeakerImage.gameObject.SetActive(true);
        StartCoroutine(FadeDeathSpeaker(new Color(0, 0, 0, 0), new Color(1, 1, 1, 1)));

        string msg = null;
        if (extraData == "success")
            msg = LocalizedText.Get(LocalizedText.GetKeysInList("[DUNGEON_SUCCESS]").Sample());
        else
            msg = LocalizedText.Get(LocalizedText.GetKeysInList("[BREAKUP]").Sample());

        if (!Game.instance.finishedTutorial)
        {
            msg = LocalizedText.Get("[BREAKUP_1]");
        }
        yield return deathMessage.ShowTextCoroutine(msg, 1f, false);
        yield return new WaitForSeconds(1f);

        deathMessage.GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0f, 0.5f, false);
        yield return StartCoroutine(FadeDeathSpeaker(new Color(1, 1, 1, 1), new Color(0, 0, 0, 0)));

        deathSpeakerImage.gameObject.SetActive(false);

        yield return StartCoroutine(ShowUnlocks());

        yield return StartCoroutine(ChangeScene("HUB"));
        yield return new WaitForSeconds(0.1f);

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

    private IEnumerator ChangeScene(string newScene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(newScene);
        yield return null;
        Game.instance.centralEvents.FireSceneChanged(newScene);
        yield break;
    }

    private IEnumerator ShowUnlocks()
    {
        if (!Game.instance.finishedTutorial)
            yield break;

        List<Quirk> unlockedQuirks = Game.instance.companionBuilder.QuirksInLevel(Game.instance.playerData.attractiveness, Game.instance.attractivenessWhenDungeonEntered + 1, Game.instance.playerData.scoutLevel, Game.instance.playerData.scoutLevel);
        List<Spell> unlockedSpells = Game.instance.companionBuilder.SpellsInLevel(Game.instance.playerData.attractiveness, Game.instance.attractivenessWhenDungeonEntered + 1, Game.instance.playerData.scoutLevel, Game.instance.playerData.scoutLevel);
        List<Item> unlockedItems = Game.instance.companionBuilder.ItemsInLevel(Game.instance.playerData.attractiveness, Game.instance.attractivenessWhenDungeonEntered + 1, Game.instance.playerData.scoutLevel, Game.instance.playerData.scoutLevel);

        for (int i = 0; i < unlockedQuirks.Count; ++i)
        {
            Game.instance.hud.unlockDialog.ShowWithQuirk(unlockedQuirks[i]);

            while (Game.instance.hud.unlockDialog.gameObject.activeSelf)
                yield return null;

            // Add a small time delay to prevent early closing
            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < unlockedSpells.Count; ++i)
        {
            Game.instance.hud.unlockDialog.ShowWithSpell(unlockedSpells[i]);

            while (Game.instance.hud.unlockDialog.gameObject.activeSelf)
                yield return null;

            // Add a small time delay to prevent early closing
            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < unlockedItems.Count; ++i)
        {
            Game.instance.hud.unlockDialog.ShowWithItem(unlockedItems[i]);

            while (Game.instance.hud.unlockDialog.gameObject.activeSelf)
                yield return null;

            // Add a small time delay to prevent early closing
            yield return new WaitForSeconds(0.2f);
        }

        // TODO bdsowers : HACK!
        // Bumping scout level up so that those unlocks don't show over & over
        if (Game.instance.playerData.scoutLevel % 2 == 1)
            Game.instance.playerData.scoutLevel++;

        yield break;
    }
}
