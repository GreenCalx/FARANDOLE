using UnityEngine;
using System;

[Serializable]
public class TargetObjectiveExtension : MiniGameExtension, ITargetObjectiveExtension
{
    public override EMiniGameMods Tag => EMiniGameMods.TARGET_OBJECTIVE;

    [SerializeField]
    public SerializableDictionary<string, TargetObjectiveExtensionData>
        DataOverIDs = new();

    public int Get(string id, LoopRank rank)
    {

        if (!DataOverIDs.Dictionary.TryGetValue(id, out var data))
            return 0;

        return data.DataOverRanks.Dictionary.TryGetValue(rank, out var v)
            ? v
            : 0;
    }

    public void Set(string id, LoopRank rank, int value)
    {
        if (!DataOverIDs.Dictionary.TryGetValue(id, out var data))
        {
            data = new TargetObjectiveExtensionData();
            DataOverIDs.Add(id, data);
        }

        data.DataOverRanks.Dictionary[rank] = value;
    }

    public void Mutate(string id, LoopRank rank, int delta)
    {
        Set(id, rank, Get(id, rank) + delta);
    }

    // INTERFACE BRIDGE
    int IMiniGameExtension<int>.Get(object key, LoopRank rank)
        => key is string id ? Get(id, rank) : 0;

    void IMiniGameExtension<int>.Set(object key, LoopRank rank, int value)
    {
        if (key is string id)
            Set(id, rank, value);
    }

    void IMiniGameExtension<int>.Mutate(object key, LoopRank rank, int delta)
    {
        if (key is string id)
            Mutate(id, rank, delta);
    }
}
