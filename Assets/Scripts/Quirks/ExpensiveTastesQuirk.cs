using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpensiveTastesQuirk : Quirk
{
    public static int CostModification()
    {
        return Random.Range(10, 100);
    }
}
