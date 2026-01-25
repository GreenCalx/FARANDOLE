using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SpawnerExtensionData : MiniGameExtensionData
{
    public override EMiniGameMods Tag => EMiniGameMods.SPAWNER;
    
    public SerializableDictionary<LoopRank, int>  DataOverRanks;

    public SpawnerExtensionData()
    {
        DataOverRanks = 
        new SerializableDictionary<LoopRank, int>();
    }

    public SpawnerExtensionData Clone()
    {
        var clone = new SpawnerExtensionData();
        foreach (var kvp in DataOverRanks.Dictionary)
        {
            clone.DataOverRanks.Dictionary[kvp.Key] = kvp.Value;
        }
        return clone;
    }
}
