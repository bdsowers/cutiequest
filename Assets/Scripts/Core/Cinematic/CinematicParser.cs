using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicParser
{
    CinematicActionPluggableFactory mActionFactory = new CinematicActionPluggableFactory();

    // Splits based on , but ignores any delimiters contained within double quotes
    // Not using regex because c'mon.
    private List<string> SplitParameterList(string parameterList)
    {
        List<string> tokens = new List<string>();
        System.Text.StringBuilder currentToken = new System.Text.StringBuilder();
        bool inQuote = false;
        
        for (int i = 0; i < parameterList.Length; ++i)
        {
            char character = parameterList[i];
            if (character == ',' &&  !inQuote)
            {
                // Split!
                string token = currentToken.ToString().Trim();
                tokens.Add(token);

                currentToken.Length = 0;
            }
            else if (character == '\"')
            {
                inQuote = !inQuote;
            }
            else if (character == '\\')
            {
                // Do a sneaky look-ahead to see if this is a \"
                // If so, add it and skip over the next character; otherwise just treat it as a slash.
                // A little inelegant, but gets the job done.
                char nextCharacter = (i + 1 >= parameterList.Length ? ' ' : parameterList[i + 1]);
                if (nextCharacter == '\"')
                {
                    currentToken.Append('\"');
                    i++;
                }
                else
                {
                    currentToken.Append(character);
                }
            }
            else
            {
                currentToken.Append(character);
            }
        }

        string finalToken = currentToken.ToString().Trim();
        if (finalToken.Length > 0)
        {
            tokens.Add(finalToken);
        }

        return tokens;
    }

    public List<CinematicAction> ParseCinematic(string blob)
    {
        List<CinematicAction> actions = new List<CinematicAction>();
        List<CinematicAction> subActionStack = new List<CinematicAction>();
        CinematicAction previousAction = null;
        bool inMultiLineComment = false;
        
        string[] lines = blob.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i].Trim();
            if (line.Length == 0 || line.StartsWith("//"))
            {
                // do nothing - ignore the line
            }
            else if (line.StartsWith("/*"))
            {
                inMultiLineComment = true;
            }
            else if (line.EndsWith("*/"))
            {
                inMultiLineComment = false;
            }
            else if (inMultiLineComment)
            {
                // do nothing - we're in a comment
            }
            else if (line == "{")
            {
                Debug.Assert(previousAction != null);
                subActionStack.Add(previousAction);
            }
            else if (line == "}")
            {
                subActionStack.RemoveAt(subActionStack.Count - 1);
            }
            else
            {
                string[] symbols = line.Split(new char[] { ' ' }, 2, System.StringSplitOptions.RemoveEmptyEntries);

                string actionName = symbols[0].Trim();

                if (!mActionFactory.ActionExists(actionName))
                {
                    Debug.LogError("Action does not exist: " + actionName);
                }

                CinematicAction newAction = mActionFactory.ConstructNewActionByName(actionName);
                Debug.Assert(newAction != null, actionName);

                if (symbols.Length > 1)
                {
                    List<string> parameterList = SplitParameterList(symbols[1].Trim()); 

                    bool simpleParameter = (parameterList.Count == 1 && !parameterList[0].Contains(":"));

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    newAction.alias = actionName;

                    if (simpleParameter)
                    {
                        parameters.Add(newAction.simpleParameterName, parameterList[0]);
                    }
                    else
                    {
                        for (int parameterIdx = 0; parameterIdx < parameterList.Count; ++parameterIdx)
                        {
                            string[] parameterKeyValue = parameterList[parameterIdx].Split(new char[] { ':' });
                            string key = parameterKeyValue[0].Trim();
                            string value = parameterKeyValue[1].Trim();
                            parameters.Add(key, value);
                        }
                    }

                    newAction.SetParameters(parameters);
                }

                previousAction = newAction;

                if (subActionStack.Count > 0)
                {
                    CinematicAction parent = subActionStack[subActionStack.Count - 1];
                    newAction.parentAction = parent;

                    parent.AddChildAction(newAction);
                }
                else
                {
                    actions.Add(newAction);
                }
            }
        }

        return actions;
    }
}
