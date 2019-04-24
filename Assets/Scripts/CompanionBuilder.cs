using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class CompanionBuilder : MonoBehaviour
{
    private const int REUSE_WINDOW_SIZE = 5;

    private List<GameObject> previouslyUsedQuirks = new List<GameObject>();
    private List<GameObject> previouslyUsedSpells = new List<GameObject>();
    private List<CharacterModelData> previouslyUsedModels = new List<CharacterModelData>();

    public CharacterData BuildRandomCharacter()
    {
        CharacterData randomCharacter = ScriptableObject.CreateInstance<CharacterData>();

        int genderNum = Random.Range(0, 2);
        string gender = genderNum == 0 ? "MALE" : "FEMALE";

        // todo bdsowers - younger characters should be more common; this shouldn't be evenly distributed
        randomCharacter.age = Random.Range(18, 100);
        randomCharacter.bio = LocalizedText.GetKeysInList("[" + gender + "_BIO]").Sample();
        randomCharacter.characterName = LocalizedText.GetKeysInList("[" + gender + "_NAME]").Sample();
        randomCharacter.levelRequirement = 1;
        CharacterModelData modelData = Game.instance.characterDataList.CharacterModelsWithGender(genderNum).Sample(previouslyUsedModels);
        randomCharacter.model = modelData.model.name;
        randomCharacter.tagline = LocalizedText.GetKeysInList("[" + gender + "_TAGLINE]").Sample();
        randomCharacter.characterUniqueId = randomCharacter.characterName + ":::" + randomCharacter.bio + ":::" + randomCharacter.tagline + ":::" + randomCharacter.model;
        randomCharacter.quirk = PrefabManager.instance.quirkPrefabs.Sample(previouslyUsedQuirks).GetComponent<Quirk>();
        randomCharacter.spell = PrefabManager.instance.spellPrefabs.Sample(previouslyUsedSpells).GetComponent<Spell>();
        randomCharacter.statBoost = (CharacterStatType)Random.Range(1, 6);
        randomCharacter.statBoostAmount = 1 + Random.Range(0, Game.instance.playerData.attractiveness);

        previouslyUsedQuirks.AddWindowed(randomCharacter.quirk.gameObject, REUSE_WINDOW_SIZE);
        previouslyUsedSpells.AddWindowed(randomCharacter.spell.gameObject, REUSE_WINDOW_SIZE);
        previouslyUsedModels.AddWindowed(modelData, REUSE_WINDOW_SIZE);

        return randomCharacter;
    }
}
