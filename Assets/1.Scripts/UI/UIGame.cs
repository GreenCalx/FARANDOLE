using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGame : MonoBehaviour, IDynamicUI
{
    public TextMeshProUGUI miniGameClock;
    public TextMeshProUGUI hpClock;
    public TextMeshProUGUI score;

    public RectTransform infoArea;
    
    public void Refresh()
    {
        infoArea.anchoredPosition = new Vector2(0f, -GameData.GetSettings.GameUIScreenProportion * Screen.height);
        infoArea.sizeDelta = new Vector2(0f, Screen.height * GameData.GetSettings.GameUIScreenProportion);
    }

    public void Init()
    {
        miniGameClock.text = "";
        hpClock.text = "";
        score.text = "";

        ShowMiniGameMode(false);
        Refresh();
    }

    public void ShowMiniGameMode(bool iState)
    {
        miniGameClock.enabled = iState;
        hpClock.enabled = iState;
        score.enabled = iState;
    }
}
