using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using System.Globalization;

public class LocalizedText
{
#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern string EMSystemLocale();
#else
    private static string EMSystemLocale()
    {
        return "en-US";
    }
#endif

    static LocalizedText mInstance = null;

    Dictionary<string, string> mStringTable;
    Dictionary<string, List<string>> mLists;

    static CultureInfo mActiveCulture;

    LocalizedText()
    {
        // Load string table for active language or fallback to English
        TextAsset textAsset = null;

        SystemLanguage language = Application.systemLanguage;

        switch (language)
        {
            /*case SystemLanguage.French:
                textAsset = Resources.Load("French", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.Italian:
                textAsset = Resources.Load("Italian", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.German:
                textAsset = Resources.Load("German", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.Spanish:
                textAsset = Resources.Load("Spanish", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                textAsset = Resources.Load("Chinese", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.Japanese:
                textAsset = Resources.Load("Japanese", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.Swedish:
                textAsset = Resources.Load("Swedish", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.Dutch:
                textAsset = Resources.Load("Dutch", typeof(TextAsset)) as TextAsset;
                break;
            case SystemLanguage.Russian:
                textAsset = Resources.Load("Russian", typeof(TextAsset)) as TextAsset;
                break;*/
            default:
                textAsset = Resources.Load("English", typeof(TextAsset)) as TextAsset;
                break;
        }

        mStringTable = new Dictionary<string, string>();
        mLists = new Dictionary<string, List<string>>();

        string[] csvLines = textAsset.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < csvLines.Length; ++i)
        {
            string line = csvLines[i];

            // Replace any double quotes with a special character that we'll change back later .. the Csv parser doesn't like them
            line = line.Replace("\"\"", "<<");

            string[] elements = SplitCsvLine(line);
            if (elements.Length > 1)
            {
                string key = elements[0];
                key = key.Trim();
                if (key.Length == 0)
                    continue;

                string value = elements[1];
                value = value.Replace("{NEWLINE}", "\n");
                value = value.Replace("<<", "\"");

                mStringTable.Add(key, value);
                
                string listName;
                int listNumber;
                if (BelongsToList(key, out listName, out listNumber))
                {
                    AddToList(listName, key);
                }
            }
        }
    }

    private void AddToList(string listName, string key)
    {
        List<string> keys = null;
        mLists.TryGetValue(listName, out keys);
        if (keys == null)
        {
            keys = new List<string>();
            mLists.Add(listName, keys);
        }

        keys.Add(key);
    }

    private bool BelongsToList(string str, out string listName, out int listNumber)
    {
        listName = null;
        listNumber = -1;

        int lastUnderscore = str.LastIndexOf("_");
        if (lastUnderscore == -1)
            return false;

        string substr = str.Substring(lastUnderscore + 1);
        
        bool endsWithBracket = substr.EndsWith("]");
        if (endsWithBracket)
            substr = substr.Substring(0, substr.Length - 1);
       
        if (int.TryParse(substr, out listNumber))
        {
            listName = str.Substring(0, lastUnderscore) + (endsWithBracket ? "]" : "");
            return true;
        }
        else
        {
            return false;
        }
    }

    private string[] SplitCsvLine(string line)
    {
        string pattern = @"
     # Match one value in valid CSV string.
     (?!\s*$)                                      # Don't match empty last value.
     \s*                                           # Strip whitespace before value.
     (?:                                           # Group for value alternatives.
     | ""(?<val>[^""\\]*(?:\\[\S\s][^""\\]*)*)""   # or $2: Double quoted string,
     | (?<val>[^,""\s\\]*(?:\s+[^,""\s\\]+)*)    # or $3: Non-comma, non-quote stuff.
     )                                             # End group of value alternatives.
     \s*                                           # Strip whitespace after value.
     (?:,|$)                                       # Field ends on comma or EOS.
     ";
        string[] values = (from Match m in Regex.Matches(line, pattern,
            RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
                           select m.Groups[1].Value).ToArray();
        return values;
    }

    string InternalGet(string key)
    {
        string value;
        if (mStringTable.TryGetValue(key, out value))
            return value;
        else
            return key;
    }

    public static string Get(string key)
    {
        if (mInstance == null)
            mInstance = new LocalizedText();

        return mInstance.InternalGet(key);
    }

    public static List<string> GetKeysInList(string listName)
    {
        if (mInstance == null)
            mInstance = new LocalizedText();

        if (mInstance.mLists.ContainsKey(listName))
        {
            return mInstance.mLists[listName];
        }
        else
        {
            return new List<string>();
        }
    }

    public static string Get(string key, params object[] args)
    {
        return string.Format(Get(key), args);
    }

    public static string FormatNumber(int number)
    {
        if (mActiveCulture == null)
        {
            string locale = EMSystemLocale();
            locale = locale.Replace("_", "-"); // Some reports that iOS sometimes uses _ instead of - in its locale codes
            Debug.Log("Trying locale: " + locale);

            // Try a simple formatting operation - if it fails, then we'll use a fallback
            try
            {
                mActiveCulture = new CultureInfo(locale);
                string.Format(mActiveCulture, "{0:n0}", number);
            }
            catch (System.Exception)
            {
                mActiveCulture = System.Globalization.CultureInfo.InvariantCulture;
            }
        }

        string s = string.Format(mActiveCulture, "{0:n0}", number);
        return s;
    }
}