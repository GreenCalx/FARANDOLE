using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Cysharp.Threading.Tasks;

public static class UGSCloudSaveManager
{
    static readonly string kPremium = "player_premium";

    public static string GetTodayKey()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd");
    }

    public static string GetKeyForDate(DateTime iDate)
    {
        return iDate.ToString("yyyy-MM-dd");
    }

    public static string GetDailySeedLeaderboardKey(string iDateKey)
    {
        return "LEADERBOARD_DAILY_SEED";
    }

    /// PREMIUM
    public static async UniTaskVoid SavePremium()
    {
        await CloudSaveService.Instance.Data.Player.SaveAsync(
            new Dictionary<string, object> { { kPremium, SessionData.IsPremium } }
        );
    }

    public static async UniTask<bool> LoadPremium()
    {
        var result = await CloudSaveService.Instance.Data.Player.LoadAsync(
            new HashSet<string> { kPremium }
        );

        if (result.TryGetValue(kPremium, out var item))
            return item.Value.GetAs<bool>();

        return false;
    }

    /// HIGH SCORES
    public static async UniTaskVoid SubmitDailySeedScore(double iScore)
    {
        string dateKey = GetTodayKey();
        string leaderboardID = GetDailySeedLeaderboardKey(dateKey);

        double bestToday = await LoadTodayBestScore(leaderboardID);
        if (iScore <= bestToday)
            return;

        await SaveDailySeedHighScore(leaderboardID, iScore);

        await LeaderboardsService.Instance.AddPlayerScoreAsync(
            leaderboardID,
            iScore
        );
    }

    static async UniTask SaveDailySeedHighScore(string iKey, double iScore)
    {
        await CloudSaveService.Instance.Data.Player.SaveAsync(
            new Dictionary<string, object>
            {
                { iKey, iScore }
            }
        );
    }

    static async UniTask<double> LoadTodayBestScore(string iKey)
    {
        try
        {
            LeaderboardEntry entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(iKey);
            return entry.Score;
        }
        catch (Exception)
        {
            // Expected when player has no score yet for this leaderboard
        }
        return 0;
    }

    static async UniTask<int> LoadAllTimeBestDailySeedScore(string iKey)
    {
        var result = await CloudSaveService.Instance.Data.Player.LoadAsync(
            new HashSet<string> { iKey }
        );

        if (result.TryGetValue(iKey, out var item))
            return item.Value.GetAs<int>();

        return 0;
    }

    public static async UniTask<List<LeaderboardEntry>> FetchTodayDailySeedLeaderboard(int iLimit = 100)
    {
        LeaderboardScoresPage scores = await LeaderboardsService.Instance.GetScoresAsync(
            GetDailySeedLeaderboardKey(GetTodayKey()),
            new GetScoresOptions
            {
                Limit = iLimit
            }
        );

        return scores.Results;
    }

    public static async UniTask<List<LeaderboardEntry>> FetchDailySeedLeaderboardForDate(
        DateTime iDate,
        int iLimit = 100
    )
    {
        LeaderboardScoresPage scores = await LeaderboardsService.Instance.GetScoresAsync(
            GetDailySeedLeaderboardKey(GetKeyForDate(iDate)),
            new GetScoresOptions
            {
                Limit = iLimit
            }
        );

        return scores.Results;
    }

    /// ARCHIVED LEADERBOARDS
    // Returns the list of past reset versions, most recent first.
    public static async UniTask<List<LeaderboardVersion>> FetchArchivedVersions(int iLimit = 10)
    {
        LeaderboardVersions versions = await LeaderboardsService.Instance.GetVersionsAsync(
            GetDailySeedLeaderboardKey(GetTodayKey()),
            new GetVersionsOptions { Limit = iLimit }
        );

        return versions.Results;
    }

    // Returns the top scores for a specific archived version (use version.Id from FetchArchivedVersions).
    public static async UniTask<List<LeaderboardEntry>> FetchArchivedLeaderboard(
        string iVersionId,
        int iLimit = 100
    )
    {
        LeaderboardVersionScoresPage scores = await LeaderboardsService.Instance.GetVersionScoresAsync(
            GetDailySeedLeaderboardKey(GetTodayKey()),
            iVersionId,
            new GetVersionScoresOptions { Limit = iLimit }
        );

        return scores.Results;
    }

    // Returns the local player's entry in a specific archived version, or null if they have no score.
    public static async UniTask<LeaderboardVersionEntry> GetPlayerArchivedScore(string iVersionId)
    {
        try
        {
            return await LeaderboardsService.Instance.GetVersionPlayerScoreAsync(
                GetDailySeedLeaderboardKey(GetTodayKey()),
                iVersionId
            );
        }
        catch (Exception)
        {
            // Player has no score in this version
            return null;
        }
    }
}
