using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRankLine : MonoBehaviour
{
    public Image rankThumbnail;
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
}
