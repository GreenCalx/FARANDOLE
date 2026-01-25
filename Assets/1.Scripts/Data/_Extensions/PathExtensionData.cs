using UnityEngine;
using UnityEngine.Splines;
using System;
using System.Collections.Generic;

[Serializable]
public class PathExtensionData : MiniGameExtensionData
{
    public override EMiniGameMods Tag => EMiniGameMods.PATH;
    
    public SerializableDictionary<LoopRank, SplineContainer>  DataOverRanks;
    public Dictionary<LoopRank, SplineContainer>  Cache_DataOverRanks;

    public PathExtensionData()
    {
        DataOverRanks = 
        new SerializableDictionary<LoopRank, SplineContainer>();
    }
}
