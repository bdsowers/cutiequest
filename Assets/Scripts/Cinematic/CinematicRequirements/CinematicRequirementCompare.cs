using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicRequirementCompare : CinematicRequirement
{
    private bool mResult = false;

    public override string actionName
    {
        get
        {
            return "compare";
        }
    }

    public override string[] aliases
    {
        get
        {
            return new string[] { "if" };
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "expression";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        string exp = dataProvider.GetStringData(mParameters, "expression");
        string[] tokens = exp.Split(new string[] { ">", "<", ">=", "<=", "=", "==", "!=" }, System.StringSplitOptions.RemoveEmptyEntries);
        string left = tokens[0];
        string op = tokens[1];
        string right = tokens[2];

        float leftFloat = 0f;
        int leftInt = 0;
        bool isLeftFloat = float.TryParse(left, out leftFloat);
        bool isLeftInt = int.TryParse(left, out leftInt);

        float rightFloat = 0f;
        int rightInt = 0;
        bool isRightFloat = float.TryParse(right, out rightFloat);
        bool isRightInt = int.TryParse(right, out rightInt);

        int mathType = 0; // 0=>int, 1=>float, 2=>string
        if (isLeftInt && isRightInt)
            mathType = 0;
        else if (isLeftFloat || isRightFloat)
            mathType = 1;
        else
            mathType = 2;

        // todo bdsowers - there's gotta be a better way to do this
        if (mathType == 2)
        {
            if (op == "==") mResult = (left == right);
            else if (op == "=") mResult = (left == right);
            else if (op == "!=") mResult = (left != right);
        }
        else if (mathType == 1)
        {
            if (op == "==") mResult = Mathf.Approximately(leftFloat, rightFloat);
            else if (op == "=") mResult = Mathf.Approximately(leftFloat, rightFloat);
            else if (op == "!=") mResult = !Mathf.Approximately(leftFloat, rightFloat);

            // All of these are dangerous when dealing with floats
            // The distinction between > and >= may be lost by precision issues.
            else if (op == ">") mResult = leftFloat > rightFloat;
            else if (op == "<") mResult = leftFloat < rightFloat;
            else if (op == ">=") mResult = leftFloat >= rightFloat;
            else if (op == "<=") mResult = leftFloat <= rightFloat;
        }
        else if (mathType == 0)
        {
            if (op == "==") mResult = (leftInt == rightInt);
            else if (op == "=") mResult = (leftInt == rightInt);
            else if (op == "!=") mResult = (leftInt != rightInt);
            else if (op == ">") mResult = (leftInt > rightInt);
            else if (op == "<") mResult = (leftInt < rightInt);
            else if (op == ">=") mResult = (leftInt >= rightInt);
            else if (op == "<=") mResult = (leftInt <= rightInt);
        }
    }

    public override bool Evaluate(CinematicDirector player)
    {
        InterpretParameters(player.dataProvider);

        return mResult;
    }
}
