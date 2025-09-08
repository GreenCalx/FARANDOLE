using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using TMPro;

public class UIGame : MonoBehaviour, IManager, IDynamicUI
{
    public TextMeshProUGUI miniGameClock;
    public TextMeshProUGUI hpClock;
    public Image timeIndicatorImg;
    public RectTransform infoArea;
    public UILoopInfo handle_UILoopInfo;
    public UIDoorAnim handle_UIDoorAnim;
    public UILoopCompleteAnimation handle_animLoopSuccess; 
    [Header("Score")]
    public TextMeshProUGUI score;
    public RectTransform scoreUIVisuals;
    public RectTransform scoreUIText;
    public Sprite loopLevel0Sprite;
    public Sprite loopLevel1Sprite;
    public Sprite loopLevel2Sprite;
    public Sprite loopLevel3Sprite;

    [Header("Success")]
    public UIStageClearAnimation handle_animStageClear;
    public TextMeshProUGUI successTimeTxt;
    public Color successTimePositiveColor;
    public Color successTimeNegativeColor;
    public Color frozenTimeColor;
    [Header("Loop Complete Animation Handles")]
    public TextMeshProUGUI handle_CurrentRank;
    public TextMeshProUGUI handle_NewRank;
    [Header("Launch Game")]
    public UIButton launchGameBtn;
    public UILoopPresentationAnim loopPresentationAnim;
    [Header("Callbacks")]
    public UnityEvent OnBeforeLoopDepth;
    public UIButton skipAnimBtn;
    // Internals
    bool InitDone = false;
    AnimationManager ANIM;

    public void Init(GameManager iGameManager)
    {
        ANIM = iGameManager.ANIM;
        miniGameClock.text = GameData.GetSettings.MiniGameTime.ToString("#0");
        hpClock.text = GameData.GetSettings.PlayerHP.ToString("#0.0");
        score.text = "";
        timeIndicatorImg.color = frozenTimeColor;

        ShowMiniGameMode(false);
        //ShowSuccessArea(false);
        handle_UILoopInfo.Init();
        Refresh();
        InitDone = true;
    }

    public bool IsReady()
    {
        return InitDone;
    }

    public void Refresh()
    {
        // infoArea.anchoredPosition = new Vector2(0f, -GameData.GetSettings.GameUIScreenProportion * Screen.height);
        // infoArea.sizeDelta = new Vector2(0f, Screen.height * GameData.GetSettings.GameUIScreenProportion);

        // TODO : compute sizes according to screen.
        // scoreUIVisuals.sizeDelta = new Vector2(256f, 256f);

        // scoreUIText.sizeDelta = new Vector2(158.78f, 200f);
        // scoreUIText.anchoredPosition += new Vector2(-12.9f, 128f);

        handle_UIDoorAnim.Init();

        handle_animLoopSuccess.passedTextColor = successTimePositiveColor;
        handle_animLoopSuccess.failedTextColor = successTimeNegativeColor;
    }

    public void RefreshLoopLevelText(string iRankText)
    {
        handle_UILoopInfo.UpdateLoopLevelText(iRankText);
    }

    public void RefreshLoopStage(int iIndex, MiniGameSuccessState iState)
    {
        handle_UILoopInfo.TurnOnLight(iIndex, iState);
    }

    public void RefreshTimeIndicator(GameClock iGameClock)
    {
        if (iGameClock.IsFrozen)
        {
            timeIndicatorImg.color = frozenTimeColor;
        }
        else if (iGameClock.MiniGameTimeExpired())
        {
            timeIndicatorImg.color = successTimeNegativeColor;
        }
        else
        {
            timeIndicatorImg.color = successTimePositiveColor;
        }
    }

    public void ResetLoopStage()
    {
        handle_UILoopInfo.TurnOffLights();
    }

    public void ShowMiniGameMode(bool iState)
    {
        miniGameClock.enabled = iState;
        hpClock.enabled = iState;
        score.enabled = iState;
        handle_UIDoorAnim.OpenAnim();
    }

    public void ShowSuccessArea(float iTime = 0f)
    {
        //successArea.gameObject.SetActive(iState);
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
        
        handle_animStageClear.Animate();
    }

    public void InterStageAnimation()
    {
        handle_UIDoorAnim.ClapAnim();
    }

    public async Task LoopCompleteAnim(MiniGameSuccessState[] iLoopSuccesses, bool iLoopPassed, bool iRankUp, int iLoopDepth, CancellationToken iCT)
    {
        Color[] colors = new Color[iLoopSuccesses.Length];
        for (int i = 0; i < iLoopSuccesses.Length; i++)
        {
            colors[i] = (iLoopSuccesses[i] == MiniGameSuccessState.PASSED) ? successTimePositiveColor : successTimeNegativeColor;
        }
        handle_animLoopSuccess.Init(colors, iLoopPassed, iRankUp, iLoopDepth);
        handle_animLoopSuccess.OnBeforeLoopDepth = new UnityEvent();
        handle_animLoopSuccess.OnBeforeLoopDepth.AddListener(()=>OnBeforeLoopDepth?.Invoke());

        ANIM.TrackAnimator(handle_animLoopSuccess.animator, iCT);
        ANIM.QueueAnimRange(handle_animLoopSuccess.animator, handle_animLoopSuccess.GetAnimQueue(iCT));
        await ANIM.PlayAnim(handle_animLoopSuccess.animator);
        ANIM.StopTrackAnimator(handle_animLoopSuccess.animator);
        //await handle_animLoopSuccess.Animate(colors, iLoopPassed, iRankUp, iLoopDepth, iCT);

        handle_animLoopSuccess.OnBeforeLoopDepth.RemoveListener(()=>OnBeforeLoopDepth?.Invoke());
    }
}
