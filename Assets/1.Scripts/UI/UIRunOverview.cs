using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;
using System.Linq;

public class UIRunOverview : MonoBehaviour
{
    public float Spacing = -80f;
    public GameObject prefab_UIRankLine;
    public GameObject prefab_UIStartThumbnail;
    public GameObject prefab_UIFullLoopCompletedThumbnail;
    public GameObject prefab_UIGameOverThumbnail;
    public RectTransform h_RunDisplay;
    RectTransform inst_startThumbnail;
    List<UIRankLine> inst_rankLines;
    RectTransform inst_RunTailThumbnail;
    public UILineRenderer LR;

    public UIDragScroll dragScroll;

    public void Setup(PlayerData iPlayerData)
    {
        MiniGameLoopHistory history = iPlayerData.loopHistory;
        if (history.Count <= 0)
            return;
        inst_rankLines = new List<UIRankLine>(history.Count);
        int index = 1;

        // Add start thumbnail
        // inst_startThumbnail = GOBuilder.Create(prefab_UIStartThumbnail)
        //                         .WithParent(h_RunDisplay)
        //                         .WithLocalPosition(new Vector3(0f, index * Spacing, 0f))
        //                         .BuildAs<RectTransform>();
        // inst_startThumbnail.gameObject.SetActive(true);

        // index++;

        // Add snapshots

        foreach (MiniGameLoopSnapshot snap in history)
        {
            Vector3 anchoredPosition = new Vector3(0f, index * Spacing, 0f);
            UIRankLine newLine = GOBuilder.Create(prefab_UIRankLine)
                                    .WithParent(h_RunDisplay)
                                    .WithLocalPosition(anchoredPosition)
                                    .BuildAs<UIRankLine>();
            inst_rankLines.Add(newLine);

            newLine.Setup(snap);

            newLine.gameObject.SetActive(true);
            index++;
        }


        // Game Over or Success Image
        if (iPlayerData.FullLoopCompleted)
        {
            Vector3 anchoredPosition = new Vector3(0f, index * Spacing, 0f);
            UIRankLine tailLine = GOBuilder.Create(prefab_UIRankLine)
                                    .WithParent(h_RunDisplay)
                                    .WithLocalPosition(anchoredPosition)
                                    .BuildAs<UIRankLine>();
            inst_rankLines.Add(tailLine);

            tailLine.SetupTail(history.RankOnFullCompletion);

            tailLine.gameObject.SetActive(true);
        }
        else
        {
            // mark in left uirankline image with failure sprite along the same line
            // than the last depth played
            inst_rankLines.Last().SetAsFailedStage();
            // inst_RunTailThumbnail = GOBuilder.Create(prefab_UIGameOverThumbnail)
            //     .WithParent(h_RunDisplay)
            //     .WithLocalPosition(new Vector3(0f, index * Spacing, 0f))
            //     .BuildAs<RectTransform>();
        }

        // LR LAST POS
        LR.Points = new Vector2[2];
        LR.Points[0] = inst_rankLines[0].GetComponent<RectTransform>().anchoredPosition;

        LR.Points[1] = inst_rankLines.Last().GetComponent<RectTransform>().anchoredPosition;
        LR.gameObject.SetActive(true);

        dragScroll.CalculateBounds();
    }
}
