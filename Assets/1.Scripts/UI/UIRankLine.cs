using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRankLine : MonoBehaviour
{
    public Sprite FullLoopCompleteImg;
    public Sprite StageFailedImg;

    public Image rankThumbnail;
    public Image lightThumbnail;
    public TextMeshProUGUI shadowRankLevelText;
    public TextMeshProUGUI rankLevelText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI savedTimeText;
    public TextMeshProUGUI leftSideText;
    public Image leftSideImage;

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
        if (iCombo > 1)
            comboText.text = "x" + iCombo;
        else
            comboText.text = "";
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

    public void SetDepthText(int iDepth)
    {
        leftSideText.text = iDepth.ToString();
        
        leftSideText.gameObject.SetActive(true);
        leftSideImage.gameObject.SetActive(false);
    }


    public void Setup(MiniGameLoopSnapshot iSnap)
    {
        SetRankText(iSnap.completionRank.ToString());
        SetRankImg(GameData.GetSettings.RankSettings.GetImageFromRank(iSnap.completionRank));
        SetCombo(iSnap.comboMultiplier);
        SetSavedTime(iSnap.SavedTime);

        SetDepthText(iSnap.depth);

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

    public void SetupTail(LoopRank iFinalRank)
    {
        SetRankText(iFinalRank.ToString());
        SetRankImg(GameData.GetSettings.RankSettings.GetImageFromRank(iFinalRank));
        comboText.gameObject.SetActive(false);
        savedTimeText.gameObject.SetActive(false);

        leftSideImage.sprite = FullLoopCompleteImg;
        leftSideText.gameObject.SetActive(false);
        leftSideImage.gameObject.SetActive(true);
    }

    public void SetAsFailedStage()
    {
        leftSideImage.sprite = StageFailedImg;
        leftSideText.gameObject.SetActive(false);
        leftSideImage.gameObject.SetActive(true);
    }
}
