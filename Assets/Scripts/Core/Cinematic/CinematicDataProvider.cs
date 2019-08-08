using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides simple variable support to scripts.
/// Client code can call SetData to set variables, and also cinematics can use the CinematicActionSetVariable action to set them via scripts.
/// Whenever some bit of data is read from a script, it will first be checked against the variable list and that variable's value will
/// be returned in its place; if no varible is found, the value will be treated as raw inline data (not a variable) and converted.
/// </summary>
public class CinematicDataProvider : MonoBehaviour
{
    // TODO bdsowers - this is a horrible hack; the whole cinematic system should be auto-preserved between scenes
    // but that isn't setup right now.
    static Dictionary<string, string> mVariables = new Dictionary<string, string>();

    public void Reset()
    {
        mVariables.Clear();
    }

    public void SetData(string key, object value)
    {
        if (mVariables.ContainsKey(key))
        {
            mVariables[key] = value.ToString();
        }
        else
        {
            mVariables.Add(key, value.ToString());
        }
    }

    public void SetData(string key, Vector3 value)
    {
        string strRepresentation = "" + value.x + ":" + value.y + ":" + value.z;
        SetData(key, strRepresentation);
    }

    public void SetData(string key, Color value)
    {
        string strRepresentation = "" + value.r + ":" + value.g + ":" + value.b + ":" + value.a;
        SetData(key, strRepresentation);
    }

    public string GetStringData(Dictionary<string, string> parameterList, string key, string defaultValue = null)
    {
        string value = null;
        if (parameterList != null && !parameterList.TryGetValue(key, out value))
        {
            return defaultValue;
        }

        if (parameterList == null)
        {
            value = key;
        }

        // If the value provided was a variable that we have stored, return the current
        // value of that stored variable.
        string variableValue = null;
        if (mVariables.TryGetValue(value, out variableValue))
        {
            return variableValue;
        }
        
        return value;
    }

    public float GetFloatData(Dictionary<string, string> parameterList, string key, float defaultValue = 0f)
    {
        string strData = GetStringData(parameterList, key);
        if (strData == null)
            return defaultValue;
        else
            return float.Parse(strData);
    }

    public int GetIntData(Dictionary<string, string> parameterList, string key, int defaultValue = 0)
    {
        string strData = GetStringData(parameterList, key);
        if (strData == null)
            return defaultValue;
        else
            return int.Parse(strData);
    }

    public bool GetBoolData(Dictionary<string, string> parameterList, string key, bool defaultValue = false)
    {
        string strData = GetStringData(parameterList, key);
        if (strData == null)
        {
            return defaultValue;
        }
        else
        {
            strData = strData.ToLower();
            return strData == "yes" || strData == "true" || strData == "1";
        }
    }

    public Vector3 GetVectorData(Dictionary<string, string> parameterList, string key)
    {
        string strData = GetStringData(parameterList, key);
        if (strData == null)
        {
            return Vector3.zero;
        }
        else
        {
            // TODO bdsowers - we can do this without garbage generation.
            string[] tokens = strData.Split(new char[] { ':' });
            Vector3 newVec;
            newVec.x = float.Parse(tokens[0].Trim());
            newVec.y = float.Parse(tokens[1].Trim());
            newVec.z = float.Parse(tokens[2].Trim());
            return newVec;
        }
        
    }

    public Color GetColorData(Dictionary<string, string> parameterList, string key)
    {
        string strData = GetStringData(parameterList, key);
        if (strData == null)
        {
            return new Color(0, 0, 0, 1);
        }
        else
        {
            // TODO bdsowers - we can do this without garbage generation.
            string[] tokens = strData.Split(new char[] { ':' });
            Color newColor;
            newColor.r = float.Parse(tokens[0].Trim());
            newColor.g = float.Parse(tokens[1].Trim());
            newColor.b = float.Parse(tokens[2].Trim());
            newColor.a = tokens.Length == 4 ? float.Parse(tokens[3].Trim()) : 1;
            return newColor;
        }
    }
}
