using UnityEngine;
using System;

[Serializable]
public class TargetObjectiveExtensionData : MiniGameExtensionData
{
    public override EMiniGameMods Tag => EMiniGameMods.TARGET_OBJECTIVE;
    
    public SerializableDictionary<LoopRank, int>  DataOverRanks;

    public TargetObjectiveExtensionData()
    {
        DataOverRanks = 
        new SerializableDictionary<LoopRank, int>();
    }
}
