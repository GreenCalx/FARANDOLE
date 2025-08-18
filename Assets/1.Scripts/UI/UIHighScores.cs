using System.Collections.Generic;
using UnityEngine;

public class UIHighScores : MonoBehaviour
{
    public GameObject prefab_scoreBlocks;
    public RectTransform handle_blockLayout;
    List<UIHighScoreBlock> inst_scoreBlocks;

    void OnEnable()
    {
        // OnDisable not called ?  TODO fixme
        if (inst_scoreBlocks != null)
        {
            inst_scoreBlocks.ForEach(e => Destroy(e.gameObject));
            inst_scoreBlocks.Clear();
        }
        inst_scoreBlocks = new List<UIHighScoreBlock>();
        InitScores();
    }

    void OnDisable()
    {
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
