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

    public static bool IsMiniGameExtensionInterface(Type iface)
    {
        if (!iface.IsInterface)
            return false;

        // Direct IMiniGameExtension<T>
        if (iface.IsGenericType &&
            iface.GetGenericTypeDefinition() == typeof(IMiniGameExtension<>))
            return true;

        // Derived interfaces (ISpawnerExtension, etc.)
        foreach (var parent in iface.GetInterfaces())
        {
            if (parent.IsGenericType &&
                parent.GetGenericTypeDefinition() == typeof(IMiniGameExtension<>))
                return true;
        }

        return false;
    }

}