using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SpawnerExtension : MiniGameExtension, ISpawnerExtension
{
    public override EMiniGameMods Tag => EMiniGameMods.SPAWNER;

    [SerializeField]
    public SerializableDictionary<SpawnableDef, SpawnerExtensionData>
        DataOverSpawnables = new();

    [System.NonSerialized]
    Dictionary<SpawnableDef, SpawnerExtensionData> RTData;

    public override void ResetMutation()
    {
        RTData = new Dictionary<SpawnableDef, SpawnerExtensionData>();

        foreach (var kvp in DataOverSpawnables.Dictionary)
        {
            RTData[kvp.Key] = kvp.Value.Clone();
        }
    }

    public override void ApplyMutation(float iChannel)
    {
        if (RTData == null)
            return;

        foreach (var spawnable in RTData.Values)
        {
            var dict = spawnable.DataOverRanks.Dictionary;
            var keys = dict.Keys.ToArray(); // avoid collection modification

            foreach (var rank in keys)
            {
                int baseValue = dict[rank];
                // v Actual mutation v
                dict[rank] = Mathf.RoundToInt(baseValue + (baseValue*iChannel));
            }
        }
    }


    public int Get(SpawnableDef def, LoopRank rank)
    {
        if (def == null)
            return 0;

        if (!RTData.TryGetValue(def, out var data))
            return 0;

        return data.DataOverRanks.Dictionary.TryGetValue(rank, out var v)
            ? v
            : 0;
    }

    public void Set(SpawnableDef def, LoopRank rank, int value)
    {
        if (def == null)
            return;

        if (!DataOverSpawnables.Dictionary.TryGetValue(def, out var data))
        {
            data = new SpawnerExtensionData();
            DataOverSpawnables.Add(def, data);
        }

        data.DataOverRanks.Dictionary[rank] = value;
    }

    public void Mutate(SpawnableDef def, LoopRank rank, int delta)
    {
        Set(def, rank, Get(def, rank) + delta);
    }

    // INTERFACE BRIDGE
    int IMiniGameExtension<int>.Get(object key, LoopRank rank)
        => key is SpawnableDef def ? Get(def, rank) : 0;

    void IMiniGameExtension<int>.Set(object key, LoopRank rank, int value)
    {
        if (key is SpawnableDef def)
            Set(def, rank, value);
    }

    void IMiniGameExtension<int>.Mutate(object key, LoopRank rank, int delta)
    {
        if (key is SpawnableDef def)
            Mutate(def, rank, delta);
    }
}
