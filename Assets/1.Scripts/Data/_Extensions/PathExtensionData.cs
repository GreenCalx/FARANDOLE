using UnityEngine;
using UnityEngine.Splines;
using System;

[Serializable]
public class PathExtensionData : MiniGameExtensionData
{
    public override EMiniGameMods Tag => EMiniGameMods.PATH;
    
    public SerializableDictionary<LoopRank, SplineContainer>  DataOverRanks;

    public PathExtensionData()
    {
        DataOverRanks = 
        new SerializableDictionary<LoopRank, SplineContainer>();
    }
}
