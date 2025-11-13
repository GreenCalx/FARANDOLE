using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;

public class UIRunOverview : MonoBehaviour
{
    public GameObject prefab_UIRankLine;
    public GameObject prefab_UIStartThumbnail;
    public RectTransform h_RunDisplay;
    RectTransform inst_startThumbnail;
    List<UIRankLine> inst_rankLines;
    public UILineRenderer LR;

    public void Setup(PlayerData iPlayerData)
    {

        MiniGameLoopHistory history = iPlayerData.loopHistory;
        inst_rankLines = new List<UIRankLine>(history.Count + 1);

        // Add start thumbnail
        inst_startThumbnail = GOBuilder.Create(prefab_UIStartThumbnail)
                                .WithParent(h_RunDisplay)
                                .WithLocalPosition(Vector3.zero)
                                .BuildAs<RectTransform>();
        inst_startThumbnail.gameObject.SetActive(true);

        LR.Points = new Vector2[2];
        LR.Points[0] = inst_startThumbnail.anchoredPosition;
        // TODO LR START POS

        // Add snapshots
        int index = 1;
        Vector3 lastPos = Vector3.zero;
        foreach (MiniGameLoopSnapshot snap in history)
        {
            Vector3 anchoredPosition = new Vector3(0f, index * -64f, 0f);
            UIRankLine newLine = GOBuilder.Create(prefab_UIRankLine)
                                    .WithParent(h_RunDisplay)
                                    .WithLocalPosition(anchoredPosition)
                                    .BuildAs<UIRankLine>();
            inst_rankLines.Add(newLine);

            newLine.SetRankText(snap.completionRank.ToString());
            newLine.SetRankImg(GameData.GetSettings.RankSettings.GetImageFromRank(snap.completionRank));
            newLine.SetCombo(snap.comboMultiplier);

            newLine.gameObject.SetActive(true);
            index++;

            lastPos = newLine.transform.localPosition;
        }
        // TODO LR LAST POS
        LR.Points[1] = lastPos;
        LR.gameObject.SetActive(true);
    }
}
