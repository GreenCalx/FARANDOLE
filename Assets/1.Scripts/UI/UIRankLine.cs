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

    public void Setup(MiniGameLoopSnapshot iSnap)
    {
        SetRankText(iSnap.completionRank.ToString());
        SetRankImg(GameData.GetSettings.RankSettings.GetImageFromRank(iSnap.completionRank));
        SetCombo(iSnap.comboMultiplier);

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
