using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;

using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

public class UIHighScores : UIPanel
{
    public GameObject prefab_scoreBlocks;
    public RectTransform handle_blockLayout;
    public int MAX_SCORE_PER_PAGE = 14;
    List<UIHighScoreBlock> inst_scoreBlocks;

    public override void OnNavEnter(CancellationToken iCT)
    {
        if (inst_scoreBlocks != null)
        {
            inst_scoreBlocks.ForEach(e => Destroy(e.gameObject));
            inst_scoreBlocks.Clear();
        }
        inst_scoreBlocks = new List<UIHighScoreBlock>();
        InitScores();
        base.OnNavEnter(iCT);
    }

    public override void OnNavExit(CancellationToken iCT)
    {
        base.OnNavExit(iCT);
        inst_scoreBlocks.ForEach(e => Destroy(e.gameObject));
        inst_scoreBlocks.Clear();
    }

    public async UniTask InitScores()
    {
        List<LeaderboardEntry> leaderboardEntries = await UGSCloudSaveManager.FetchTodayDailySeedLeaderboard(MAX_SCORE_PER_PAGE);
        foreach(LeaderboardEntry entry in leaderboardEntries)
        {
            UIHighScoreBlock uihsb = GOBuilder.Create(prefab_scoreBlocks)
                    .WithName("HighScoreBlock")
                    .WithParent(handle_blockLayout)
                    .BuildAs<UIHighScoreBlock>();
            uihsb.Setup(entry);
            inst_scoreBlocks.Add(uihsb);

            if (inst_scoreBlocks.Count >= MAX_SCORE_PER_PAGE)
            {
                Debug.LogWarning("PAGE IS FULL");
                // TODO Pages
                return;
            }
        }
    }
}
