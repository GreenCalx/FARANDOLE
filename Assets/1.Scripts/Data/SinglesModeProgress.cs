using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct SinglesData
{
    public int MGID;
    public bool completed;
    public double score;
    public LoopRank maxRank;
    public string dateUtc; // ISO string for safety
}


public static class SinglesModeProgress
{
    private const string KEY_PREFIX = "kSinglesMG_";

    private static string GetKey(int mgId)
    {
        return KEY_PREFIX + mgId;
    }

    public static void SaveLocalProgress(
        int mgId,
        bool completed,
        double score,
        LoopRank maxRank
    )
    {
        var data = new SinglesData
        {
            MGID = mgId,
            completed = completed,
            score = score,
            maxRank = maxRank,
            dateUtc = DateTime.UtcNow.ToString("O")
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(GetKey(mgId), json);
        PlayerPrefs.Save();
    }

    public static bool TryLoadLocalProgress(int mgId, out SinglesData data)
    {
        string key = GetKey(mgId);

        if (!PlayerPrefs.HasKey(key))
        {
            data = default;
            return false;
        }

        string json = PlayerPrefs.GetString(key);
        data = JsonUtility.FromJson<SinglesData>(json);
        return true;
    }

    public static Dictionary<int, SinglesData> LoadAllLocalProgress(
        List<int> knownMiniGameIds
    )
    {
        var result = new Dictionary<int, SinglesData>();

        foreach (int mgId in knownMiniGameIds)
        {
            if (TryLoadLocalProgress(mgId, out var data))
            {
                result[mgId] = data;
            }
        }

        return result;
    }

    public static void ResetProgress(List<int> knownMiniGameIds)
    {
        foreach (int mgId in knownMiniGameIds)
        {
            PlayerPrefs.DeleteKey(GetKey(mgId));
        }

        PlayerPrefs.Save();
    }
}
