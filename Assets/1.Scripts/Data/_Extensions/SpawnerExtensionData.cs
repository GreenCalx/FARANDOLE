using UnityEngine;
using System;

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
}
