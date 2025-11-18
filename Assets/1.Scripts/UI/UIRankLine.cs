using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRankLine : MonoBehaviour
{
    public Image rankThumbnail;
    public Image lightThumbnail;
    public TextMeshProUGUI shadowRankLevelText;
    public TextMeshProUGUI rankLevelText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI savedTimeText;

    public void SetRankText(string iTxt)
    {
        rankLevelText.text = iTxt;
        shadowRankLevelText.text = iTxt;
    }

    public void SetRankImg(Sprite iSprite)
    {
        rankThumbnail.sprite = iSprite;
    }

    public void SetCombo(int iCombo)
    {
        comboText.text = "x" + iCombo;
    }

    public void SetSavedTime(float iTotalSavedTime)
    {
        if (savedTimeText == null)
            return;

        Color c =  (iTotalSavedTime < 0f) ? GameData.GetSettings.LoopFailedColor : GameData.GetSettings.LoopPassedColor;
        string prefix  =(iTotalSavedTime < 0f )? "" : "+";
        savedTimeText.color = c;
        savedTimeText.text = "("+prefix +iTotalSavedTime.ToString("#0.0")+"s)";
    }

    public void Setup(MiniGameLoopSnapshot iSnap)
    {
        SetRankText(iSnap.completionRank.ToString());
        SetRankImg(GameData.GetSettings.RankSettings.GetImageFromRank(iSnap.completionRank));
        SetCombo(iSnap.comboMultiplier);
        SetSavedTime(iSnap.SavedTime);

        // Light update
        if (iSnap.IsPerfect)
        {
            lightThumbnail.color = GameData.GetSettings.LoopPefectColor;
        }
        else if (iSnap.IsFailed)
        {
            lightThumbnail.color = GameData.GetSettings.LoopFailedColor;
        }
        else
        {
            lightThumbnail.color = GameData.GetSettings.LoopPassedColor;
        }
    }
}
