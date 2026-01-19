using UnityEngine;
using System;

[Serializable]
public class PathExtension : MiniGameExtension, IPathExtension
{
    public override EMiniGameMods Tag => EMiniGameMods.PATH;

    [SerializeField]
    public SerializableDictionary<PathDef, PathExtensionData>
        DataOverPathes = new();

    public int Get(PathDef def, LoopRank rank)
    {
        if (def == null)
            return 0;

        if (!DataOverPathes.Dictionary.TryGetValue(def, out var data))
            return 0;

        return data.DataOverRanks.Dictionary.TryGetValue(rank, out var v)
            ? v
            : 0;
    }

    public void Set(PathDef def, LoopRank rank, int value)
    {
        if (def == null)
            return;

        if (!DataOverPathes.Dictionary.TryGetValue(def, out var data))
        {
            data = new SpawnerExtensionData();
            DataOverPathes.Add(def, data);
        }

        data.DataOverRanks.Dictionary[rank] = value;
    }

    public void Mutate(PathDef def, LoopRank rank, int delta)
    {
        Set(def, rank, Get(def, rank) + delta);
    }

    // INTERFACE BRIDGE
    int IMiniGameExtension<int>.Get(object key, LoopRank rank)
        => key is PathDef def ? Get(def, rank) : 0;

    void IMiniGameExtension<int>.Set(object key, LoopRank rank, int value)
    {
        if (key is PathDef def)
            Set(def, rank, value);
    }

    void IMiniGameExtension<int>.Mutate(object key, LoopRank rank, int delta)
    {
        if (key is PathDef def)
            Mutate(def, rank, delta);
    }
}
