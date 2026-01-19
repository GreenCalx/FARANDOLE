using UnityEngine;
using UnityEngine.Splines;
using System;

[Serializable]
public class PathExtension : MiniGameExtension, IPathExtension
{
    public override EMiniGameMods Tag => EMiniGameMods.PATH;

    [SerializeField]
    public SerializableDictionary<PathDef, PathExtensionData>
        DataOverPathes = new();

    public SplineContainer Get(PathDef def, LoopRank rank)
    {
        if (def == null)
            return null;

        if (!DataOverPathes.Dictionary.TryGetValue(def, out var data))
            return null;

        return data.DataOverRanks.Dictionary.TryGetValue(rank, out var v)
            ? v
            : null;
    }

    public void Set(PathDef def, LoopRank rank, SplineContainer value)
    {
        if (def == null)
            return;

        if (!DataOverPathes.Dictionary.TryGetValue(def, out var data))
        {
            data = new PathExtensionData();
                DataOverPathes.Add(def, data);
        }

        data.DataOverRanks.Dictionary[rank] = value;
    }

    public void Mutate(PathDef def, LoopRank rank, SplineContainer delta)
    {
        // TODO
        //Set(def, rank, Get(def, rank) + delta);
    }

    // INTERFACE BRIDGE
    SplineContainer IMiniGameExtension<SplineContainer>.Get(object key, LoopRank rank)
        => key is PathDef def ? Get(def, rank) : null;

    void IMiniGameExtension<SplineContainer>.Set(object key, LoopRank rank, SplineContainer value)
    {
        if (key is PathDef def)
            Set(def, rank, value);
    }

    void IMiniGameExtension<SplineContainer>.Mutate(object key, LoopRank rank, SplineContainer delta)
    {
        if (key is PathDef def)
            Mutate(def, rank, delta);
    }
}
