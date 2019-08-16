using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigLatinQuirk : Quirk
{
    private static bool mEnabled;

    private void Start()
    {
        mEnabled = true;
    }

    private void OnEnable()
    {
        mEnabled = true;
    }

    private void OnDestroy()
    {
        mEnabled = false;
    }

    private void OnDisable()
    {
        mEnabled = false;
    }

    public static string ApplyQuirkIfPresent(string str)
    {
        if (mEnabled)
        {
            string[] words = str.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; ++i)
            {
                string word = words[i];
                char firstLetter = word[0];
                char lastLetter = word[word.Length - 1];

                // Skip over anything that doesn't start with a letter - it's not a word we care about
                if (!IsLetter(firstLetter))
                    continue;

                // Skip over anything only a single letter long
                if (word.Length == 1)
                    continue;

                bool isFirstLetterCapital = IsCapitalLetter(firstLetter);
                bool isLastLetterPunctuation = IsPunctuation(lastLetter);

                if (isLastLetterPunctuation)
                {
                    word = word.Substring(0, word.Length - 1);
                }

                if (IsVowel(firstLetter))
                {
                    string ending = IsWholeWordCapitalized(word) ? "WAY" : "way";
                    word = word + ending;
                }
                else
                {
                    string ending = IsWholeWordCapitalized(word) ? "AY" : "ay";
                    string cluster = ConsonantCluster(word);
                    cluster = IsWholeWordCapitalized(word) ? cluster.ToUpper() : cluster.ToLower();

                    word = word.Substring(cluster.Length) + cluster + ending;

                    // Maintain first letter capitalization so sentences still read properly.
                    if (isFirstLetterCapital)
                    {
                        word = word.Substring(0, 1).ToUpper() + word.Substring(1);
                    }
                }

                if (isLastLetterPunctuation)
                {
                    word = word + lastLetter;
                }

                words[i] = word;
            }

            return string.Join(" ", words);
        }

        return str;
    }

    private static bool IsPunctuation(char letter)
    {
        return (letter == '.' || letter == '?' || letter == '!' || 
            letter == ',' || letter == ':' || letter == ';' || letter == '\'' ||
            letter == '(' || letter == ')' || letter == '[' || letter == ']');
    }

    private static bool IsWholeWordCapitalized(string word)
    {
        return word == word.ToUpper();
    }

    private static bool IsCapitalLetter(char c)
    {
        return (c >= 'A' && c <= 'Z');
    }

    private static bool IsLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private static bool IsVowel(char c)
    {
        return (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' ||
                c == 'A' || c == 'E' || c == 'I' || c == 'O' || c == 'U');
    }

    private static string ConsonantCluster(string word)
    {
        int position = 0;
        while (position < word.Length && !IsVowel(word[position]))
        {
            ++position;
        }

        return word.Substring(0, position);
    }
}
