using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class CompanionBuilder : MonoBehaviour
{
    private const int GAMEPLAY_REUSE_WINDOW_SIZE = 5;

    // todo - once we have more content, actually bump this number up
    private const int TEXT_REUSE_WINDOW_SIZE = 0;

    private List<Quirk> previouslyUsedQuirks = new List<Quirk>();
    private List<Spell> previouslyUsedSpells = new List<Spell>();
    private List<CharacterModelData> previouslyUsedModels = new List<CharacterModelData>();

    private List<int> previouslyUsedBios = new List<int>();
    private List<string> previouslyUsedMaleNames = new List<string>();
    private List<string> previouslyUsedFemaleNames = new List<string>();

    public Material[] materials;

    public CharacterData BuildRandomCharacter(Quirk quirkOverride = null, Spell spellOverride = null)
    {
        CharacterData randomCharacter = ScriptableObject.CreateInstance<CharacterData>();

        int genderNum = Random.Range(0, 2);
        string gender = genderNum == 0 ? "MALE" : "FEMALE";

        // todo bdsowers - add large window for taglines + bios, names
        // also pull from the NEUTRAL list for taglines + bios

        randomCharacter.age = GenerateCharacterAge();

        List<string> taglines = LocalizedText.GetKeysInList("[" + gender + "_TAGLINE]");
        taglines.AddRange(LocalizedText.GetKeysInList("[NEUTRAL_TAGLINE]"));

        List<string> bios = LocalizedText.GetKeysInList("[" + gender + "_BIO]");
        bios.AddRange(LocalizedText.GetKeysInList("[NEUTRAL_BIO]"));

        // These are parallel arrays
        int bioAndTaglinePosition = bios.SamplePosition(previouslyUsedBios);
        
        randomCharacter.bio = bios[bioAndTaglinePosition];
        randomCharacter.characterName = LocalizedText.GetKeysInList("[" + gender + "_NAME]").Sample((genderNum == 0 ? previouslyUsedMaleNames : previouslyUsedFemaleNames));
        randomCharacter.levelRequirement = 1;

        List<CharacterModelData> unusableModels = new List<CharacterModelData>(previouslyUsedModels);
        unusableModels.Add(PlayerModel());

        CharacterModelData modelData = Game.instance.characterDataList.CharacterModelsWithGender(genderNum).Sample(unusableModels);
        randomCharacter.model = modelData.model.name;
        randomCharacter.tagline = taglines[bioAndTaglinePosition];
        randomCharacter.characterUniqueId = randomCharacter.characterName + ":::" + randomCharacter.bio + ":::" + randomCharacter.tagline + ":::" + randomCharacter.model;
        randomCharacter.quirk = QuirksInLevel(Game.instance.playerData.attractiveness).Sample(previouslyUsedQuirks).GetComponent<Quirk>();
        randomCharacter.spell =SpellsInLevel(Game.instance.playerData.attractiveness).Sample(previouslyUsedSpells).GetComponent<Spell>();
        randomCharacter.statBoost = (CharacterStatType)Random.Range(1, 6);
        randomCharacter.statBoostAmount = 1 + Random.Range(0, MaximumPassiveStatBoost(randomCharacter.statBoost) + 1);
        randomCharacter.material = materials.Sample();

        if (quirkOverride != null)
        {
            randomCharacter.quirk = quirkOverride;
        }

        if (spellOverride != null)
        {
            randomCharacter.spell = spellOverride;
        }

        previouslyUsedQuirks.AddWindowed(randomCharacter.quirk, GAMEPLAY_REUSE_WINDOW_SIZE);
        previouslyUsedSpells.AddWindowed(randomCharacter.spell, GAMEPLAY_REUSE_WINDOW_SIZE);
        previouslyUsedModels.AddWindowed(modelData, GAMEPLAY_REUSE_WINDOW_SIZE);
        
        previouslyUsedBios.AddWindowed(bioAndTaglinePosition, TEXT_REUSE_WINDOW_SIZE);
        if (genderNum == 0)
            previouslyUsedMaleNames.AddWindowed(randomCharacter.characterName, TEXT_REUSE_WINDOW_SIZE);
        else
            previouslyUsedFemaleNames.AddWindowed(randomCharacter.characterName, TEXT_REUSE_WINDOW_SIZE);

        return randomCharacter;
    }

    private int MaximumPassiveStatBoost(CharacterStatType statType)
    {
        // Look at the player's current level in that area
        int baseLevel = Game.instance.avatar.GetComponent<CharacterStatistics>().BaseStatValue(statType);
        
        return baseLevel / 2;
    }

    private CharacterModelData PlayerModel()
    {
        return Game.instance.characterDataList.CharacterModelWithName(Game.instance.playerData.model);
    }

    public void BuildCompanionSet()
    {
        Game.instance.playerData.followerUid = null;

        int numCharacters = Random.Range(3, 6);
        
        CharacterData[] characters = new CharacterData[numCharacters];
        for (int i = 0; i < numCharacters; ++i)
        {
            CharacterData character = Game.instance.companionBuilder.BuildRandomCharacter();
            characters[i] = character;
        }

        Game.instance.characterDataList.characterData = characters;

        Follower currentFollower = GameObject.FindObjectOfType<Follower>();
        if (currentFollower != null)
        {
            currentFollower.GetComponentInChildren<CharacterModel>().RemoveModel();
        }
    }

    public void BuildCheatCompanionSet()
    {
        Game.instance.playerData.followerUid = null;

        List<Quirk> quirks = QuirksInLevel(90);
        List<Spell> spells = SpellsInLevel(90);

        int numCharacters = Mathf.Max(quirks.Count, spells.Count);

        CharacterData[] characters = new CharacterData[numCharacters];
        for (int i = 0; i < numCharacters; ++i)
        {
            Spell spell = spells[i % spells.Count];
            Quirk quirk = quirks[i % quirks.Count];

            CharacterData character = Game.instance.companionBuilder.BuildRandomCharacter(quirk, spell);
            characters[i] = character;
        }

        Game.instance.characterDataList.characterData = characters;

        Follower currentFollower = GameObject.FindObjectOfType<Follower>();
        if (currentFollower != null)
        {
            currentFollower.GetComponentInChildren<CharacterModel>().RemoveModel();
        }
    }

    private int GenerateCharacterAge()
    {
        // Younger characters, 18 - 35 range, are more common.
        bool useYoungAgeRange = (Random.Range(0, 100) < 65);

        if (useYoungAgeRange)
        {
            return Random.Range(18, 36);
        }
        else
        {
            return Random.Range(18, 99);
        }
    }

    public List<Quirk> QuirksInLevel(int maxLevel, int minLevel = -1)
    {
        List<Quirk> quirks = new List<Quirk>();
        for (int i = 0; i < PrefabManager.instance.quirkPrefabs.Length; ++i)
        {
            Quirk quirk = PrefabManager.instance.quirkPrefabs[i].GetComponent<Quirk>();
            if (quirk.requiredLevel <= maxLevel && quirk.requiredLevel >= minLevel)
            {
                quirks.Add(quirk);
            }
        }
        return quirks;
    }

    public List<Spell> SpellsInLevel(int maxLevel, int minLevel = -1)
    {
        List<Spell> spells = new List<Spell>();
        for (int i = 0; i < PrefabManager.instance.spellPrefabs.Length; ++i)
        {
            Spell spell = PrefabManager.instance.spellPrefabs[i].GetComponent<Spell>();
            if (spell.requiredLevel <= maxLevel && spell.requiredLevel >= minLevel)
            {
                spells.Add(spell);
            }
        }
        return spells;
    }

    public Material MaterialByName(string name)
    {
        for (int i = 0; i < materials.Length; ++i)
        {
            if (materials[i].name == name)
                return materials[i];
        }

        return null;
    }
}
