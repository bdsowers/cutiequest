using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;
using DG.Tweening;

public class HUB : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        Game.instance.companionBuilder.BuildCompanionSet();
        QuestR.seenMatches = false;

        ResetCharacter();

        yield return null;
        Game.instance.playerData.health = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        yield return null;
        Game.instance.RefreshInventory();

        // note bdsowers - used exclusively for rating dialog
        Game.instance.HubEntered();

        if (!Game.instance.finishedTutorial)
        {
            Game.instance.cinematicDirector.PostCinematicEvent("hub_tutorial");
            Game.instance.finishedTutorial = true;
        }

        Game.instance.saveManager.TriggerSave();

        Game.instance.cinematicDirector.PostCinematicEvent("Hub_Reload");

        Game.instance.soundManager.PlayRandomMusicInCategory("TownMusic");

        if (ShouldPlayExitDungeonSequence())
        {
            StartCoroutine(PlayExitDungeonSequence());
        }
    }

    private bool ShouldPlayExitDungeonSequence()
    {
        return !string.IsNullOrEmpty(Game.instance.dungeonEntranceId) &&
            DungeonEntranceWithId(Game.instance.dungeonEntranceId) != null;
    }

    private IEnumerator PlayExitDungeonSequence()
    {
        // Find the player and place them at the correct location
        PlayerController avatar = Game.instance.avatar;
        avatar.enabled = false;

        SimpleMovement.OrientToDirection(avatar.commonComponents.simpleMovement.subMesh, new Vector3(0f, 0f, -1f));
        // Find the dungeon entrance they just escaped from
        GameObject entrance = DungeonEntranceWithId(Game.instance.dungeonEntranceId);

        avatar.transform.position = entrance.transform.position + new Vector3(0f, 0f, -4f);
        Vector3 roundedPos = avatar.transform.position;
        roundedPos.x = Mathf.FloorToInt(roundedPos.x);
        roundedPos.z = Mathf.RoundToInt(roundedPos.z);
        avatar.transform.position = roundedPos;

        Vector3 originPos = avatar.modelContainer.transform.localPosition;
        Vector3 backPos = avatar.modelContainer.transform.localPosition + new Vector3(0f, 0f, 6f);
        Vector3 fallTarget = originPos + new Vector3(0f, -0.25f, 0f);

        avatar.commonComponents.animator.Play("FallingSpecial");

        avatar.modelContainer.transform.localPosition = backPos;
        avatar.modelContainer.transform.DOLocalJump(fallTarget, 1f, 1, 1f);

        yield return new WaitForSeconds(0.9f);
        avatar.commonComponents.animator.speed = 0.0f;
        yield return new WaitForSeconds(0.4f);
        avatar.commonComponents.animator.speed = 1f;

        avatar.modelContainer.transform.DOLocalMove(originPos, 0.25f);

        avatar.enabled = true;

        yield break;
    }

    private GameObject DungeonEntranceWithId(string entranceId)
    {
        DungeonEntrance[] allEntrances = GameObject.FindObjectsOfType<DungeonEntrance>();
        foreach(DungeonEntrance entrance in allEntrances)
        {
            if (entrance.entranceId == entranceId)
            {
                return entrance.gameObject;
            }
        }

        return null;
    }

    private void ResetCharacter()
    {
        if (!Game.instance.playerStats.IsItemEquipped<LuckyPenny>())
        {
            Game.instance.playerData.numCoins = 0;
        }
        else
        {
            Game.instance.luckyPennyUsed = true;
        }

        Game.instance.playerStats.gameObject.RemoveAllChildren();

        CharacterStatModifier[] modifiers = Game.instance.playerStats.GetComponentsInChildren<CharacterStatModifier>();
        for (int i = 0; i < modifiers.Length; ++i)
        {
            Destroy(modifiers[i]);
        }
    }
}
