using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
public class UIHighScores : UIPanel
{
    public GameObject prefab_scoreBlocks;
    public RectTransform handle_blockLayout;
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

    public void InitScores()
    {
        UserHighScores uhs = UserData.userHighScores;
        List<LoopHighScore> lhs_list = uhs.highScores;

        foreach (LoopHighScore lhs in lhs_list)
        {
            UIHighScoreBlock uihsb = GOBuilder.Create(prefab_scoreBlocks)
                                .WithName("HighScoreBlock")
                                .WithParent(handle_blockLayout)
                                .BuildAs<UIHighScoreBlock>();
            inst_scoreBlocks.Add(uihsb);
            uihsb.associatedLHS = lhs;
        }
        
    }
}
