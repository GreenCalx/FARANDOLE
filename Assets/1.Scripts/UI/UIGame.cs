using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGame : MonoBehaviour, IDynamicUI
{
    public TextMeshProUGUI miniGameClock;
    public TextMeshProUGUI hpClock;
    public TextMeshProUGUI miniGameDesc;
    public RectTransform infoArea;
    public UILoopInfo handle_UILoopInfo;
    [Header("Score")]
    public TextMeshProUGUI score;
    public RectTransform scoreUIVisuals;
    public RectTransform scoreUIText;
    public Sprite loopLevel0Sprite;
    public Sprite loopLevel1Sprite;
    public Sprite loopLevel2Sprite;
    public Sprite loopLevel3Sprite;


    [Header("Success")]
    public RectTransform successArea;
    public TextMeshProUGUI successTimeTxt;
    public Color successTimePositiveColor;
    public Color successTimeNegativeColor;

    public void Refresh()
    {
        infoArea.anchoredPosition = new Vector2(0f, -GameData.GetSettings.GameUIScreenProportion * Screen.height);
        infoArea.sizeDelta = new Vector2(0f, Screen.height * GameData.GetSettings.GameUIScreenProportion);

        // TODO : compute sizes according to screen.
        scoreUIVisuals.sizeDelta = new Vector2(256f, 256f);

        scoreUIText.sizeDelta = new Vector2(158.78f, 200f);
        scoreUIText.anchoredPosition += new Vector2(-12.9f, 128f);
    }

    public void RefreshLoopLevelText(int iLoopLevel)
    {
        switch (iLoopLevel)
        {
            case 1:
                handle_UILoopInfo.UpdateLoopLevelText("I");
                break;
            case 2:
                handle_UILoopInfo.UpdateLoopLevelText("II");
                break;
            case 3:
                handle_UILoopInfo.UpdateLoopLevelText("III");
                break;
            default:
                handle_UILoopInfo.UpdateLoopLevelText("D");
                break;
        }
    }

    public void RefreshLoopStage(int iIndex, MiniGameSuccessState iState)
    {
        handle_UILoopInfo.TurnOnLight(iIndex, iState);
    }

    public void ResetLoopStage()
    {
        handle_UILoopInfo.TurnOffLights();
    }

    public void Init()
    {
        miniGameClock.text = "";
        hpClock.text = "";
        score.text = "";

        ShowMiniGameMode(false);
        ShowSuccessArea(false);
        handle_UILoopInfo.Init();
        Refresh();
    }

    public void ShowMiniGameMode(bool iState)
    {
        miniGameClock.enabled = iState;
        hpClock.enabled = iState;
        score.enabled = iState;
    }

    public void ShowSuccessArea(bool iState, float iTime = 0f)
    {
        successArea.gameObject.SetActive(iState);

        string successTimeStr = "";
        if (iTime >= 0f)
        {
            successTimeTxt.color = successTimePositiveColor;
            successTimeStr += "+";
        }
        else
        {
            successTimeTxt.color = successTimeNegativeColor;
            //successTimeStr += "-";
        }

        successTimeStr += iTime.ToString("#0.0");
        successTimeTxt.text = successTimeStr;
    }
}
