using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class CompanionBuilder : MonoBehaviour
{
    private const int GAMEPLAY_REUSE_WINDOW_SIZE = 5;

    private const int TEXT_REUSE_WINDOW_SIZE = 15;

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

        List<string> taglines = new List<string>();
        LocalizedText.GetKeysInListCopy("[NEUTRAL_TAGLINE]", taglines);
        LocalizedText.GetKeysInListCopy("[MALE_TAGLINE]", taglines);
        LocalizedText.GetKeysInListCopy("[FEMALE_TAGLINE]", taglines);

        List<string> bios = new List<string>();
        LocalizedText.GetKeysInListCopy("[NEUTRAL_BIO]", bios);
        LocalizedText.GetKeysInListCopy("[MALE_BIO]", bios);
        LocalizedText.GetKeysInListCopy("[FEMALE_BIO]", bios);

        // To get our repetition guards to work properly, things need to maintain positions.
        // But we still want to support gendered text, so create a temporary ignore list
        // that contains the previously used positions plus any gender-invalid positions
        List<int> ignoreList = new List<int>(previouslyUsedBios);
        string invalid = "[MALE";
        if (genderNum == 0)
            invalid = "[FEMALE";

        for (int i = 0; i < bios.Count; ++i)
        {
            if (bios[i].StartsWith(invalid))
            {
                ignoreList.Add(i);
            }
        }

        int bioAndTaglinePosition = bios.SamplePosition(ignoreList);

        randomCharacter.bio = bios[bioAndTaglinePosition];
        randomCharacter.characterName = LocalizedText.GetKeysInList("[" + gender + "_NAME]").Sample((genderNum == 0 ? previouslyUsedMaleNames : previouslyUsedFemaleNames));
        randomCharacter.levelRequirement = 1;

        List<CharacterModelData> unusableModels = new List<CharacterModelData>(previouslyUsedModels);
        unusableModels.Add(PlayerModel());

        CharacterModelData modelData = Game.instance.characterDataList.CharacterModelsWithGender(genderNum).Sample(unusableModels);
        randomCharacter.model = modelData.model.name;
        randomCharacter.tagline = taglines[bioAndTaglinePosition];
        randomCharacter.characterUniqueId = randomCharacter.characterName + ":::" + randomCharacter.bio + ":::" + randomCharacter.tagline + ":::" + randomCharacter.model;
        randomCharacter.quirk = QuirksInLevel(Game.instance.playerData.attractiveness, -1, Game.instance.playerData.scoutLevel, -1).Sample(previouslyUsedQuirks).GetComponent<Quirk>();
        randomCharacter.spell =SpellsInLevel(Game.instance.playerData.attractiveness, -1, Game.instance.playerData.scoutLevel, -1).Sample(previouslyUsedSpells).GetComponent<Spell>();
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

        return baseLevel / 5;
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

        List<Quirk> quirks = QuirksInLevel(90, -1, 90, -1);
        List<Spell> spells = SpellsInLevel(90, -1, 90, -1);

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

    public List<Quirk> QuirksInLevel(int maxLevel, int minLevel, int maxScoutLevel, int minScoutLevel)
    {
        List<Quirk> quirks = new List<Quirk>();
        for (int i = 0; i < PrefabManager.instance.quirkPrefabs.Length; ++i)
        {
            Quirk quirk = PrefabManager.instance.quirkPrefabs[i].GetComponent<Quirk>();
            if (quirk.requiredScoutLevel == 0 && quirk.requiredLevel <= maxLevel && quirk.requiredLevel >= minLevel)
            {
                quirks.Add(quirk);
            }

            if (quirk.requiredScoutLevel != 0 && quirk.requiredScoutLevel <= maxScoutLevel && quirk.requiredScoutLevel >= minScoutLevel)
            {
                quirks.Add(quirk);
            }
        }
        return quirks;
    }

    public List<Spell> SpellsInLevel(int maxLevel, int minLevel, int maxScoutLevel, int minScoutLevel)
    {
        List<Spell> spells = new List<Spell>();
        for (int i = 0; i < PrefabManager.instance.spellPrefabs.Length; ++i)
        {
            Spell spell = PrefabManager.instance.spellPrefabs[i].GetComponent<Spell>();
            if (spell.scoutLevel == 0 && spell.requiredLevel <= maxLevel && spell.requiredLevel >= minLevel)
            {
                spells.Add(spell);
            }

            if (spell.scoutLevel != 0 && spell.scoutLevel <= maxScoutLevel && spell.scoutLevel >= minScoutLevel)
            {
                spells.Add(spell);
            }
        }
        return spells;
    }

    public List<Item> ItemsInLevel(int maxLevel, int minLevel, int maxScoutLevel, int minScoutLevel)
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < PrefabManager.instance.itemPrefabs.Length; ++i)
        {
            Item item = PrefabManager.instance.itemPrefabs[i].GetComponent<Item>();
            if (item.scoutLevel == 0 && item.requiedLevel <= maxLevel && item.requiedLevel >= minLevel)
            {
                items.Add(item);
            }

            if (item.scoutLevel != 0 && item.scoutLevel <= maxScoutLevel && item.scoutLevel >= minScoutLevel)
            {
                items.Add(item);
            }
        }
        return items;
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
