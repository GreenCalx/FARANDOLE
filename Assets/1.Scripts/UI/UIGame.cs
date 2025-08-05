using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGame : MonoBehaviour, IDynamicUI
{
    public TextMeshProUGUI miniGameClock;
    public TextMeshProUGUI hpClock;

    public RectTransform infoArea;
    [Header("Score")]
    public TextMeshProUGUI score;
    public RectTransform scoreUIVisuals;
    public RectTransform scoreUIText;

    [Header("Success")]
    public RectTransform successArea;

    public void Refresh()
    {
        infoArea.anchoredPosition = new Vector2(0f, -GameData.GetSettings.GameUIScreenProportion * Screen.height);
        infoArea.sizeDelta = new Vector2(0f, Screen.height * GameData.GetSettings.GameUIScreenProportion);

        // TODO : compute sizes according to screen.
        scoreUIVisuals.sizeDelta = new Vector2(256f, 256f);

        scoreUIText.sizeDelta = new Vector2(158.78f, 200f);
        scoreUIText.anchoredPosition += new Vector2(-12.9f, 128f);
    }

    public void Init()
    {
        miniGameClock.text = "";
        hpClock.text = "";
        score.text = "";

        ShowMiniGameMode(false);
        ShowSuccessArea(false);
        Refresh();
    }

    public void ShowMiniGameMode(bool iState)
    {
        miniGameClock.enabled = iState;
        hpClock.enabled = iState;
        score.enabled = iState;
    }

    public void ShowSuccessArea(bool iState)
    {
        successArea.gameObject.SetActive(iState) ;
    }
}
