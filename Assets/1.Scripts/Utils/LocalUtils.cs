using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Farandole util methods
/// </summary>
public static class LocalUtils
{
    public static T GetValueFromRank<T>(T[] iRankedValues, MiniGameManager iMGM)
    {
        return iRankedValues[Mathf.Min( iRankedValues.Length-1, iMGM.miniGamesDifficulty - 1)];
    }
}