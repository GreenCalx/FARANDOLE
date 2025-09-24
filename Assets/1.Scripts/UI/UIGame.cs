using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using TMPro;

public class UIGame : MonoBehaviour, IManager, IDynamicUI
{
    [Header("Player UI")]
    public TextMeshProUGUI miniGameClock;
    public TextMeshProUGUI hpClock;
    public Image timeIndicatorImg;
    public RotateAlongTimeAnim timeNeedleAnim;
    public RectTransform infoArea;
    public UIDoorAnim handle_UIDoorAnim;
    public UILoopCompleteAnimation handle_animLoopSuccess; 
    [Header("Score")]
    public RectTransform scoreUIVisuals;
    public RectTransform scoreUIText;

    [Header("Success")]
    public UIStageClearAnimation handle_animStageClear;
    public TextMeshProUGUI successTimeTxt;
    public Color successTimePositiveColor;
    public Color successTimeNegativeColor;
    public Color frozenTimeColor;
    [Header("Loop Complete Animation Handles")]
    public TextMeshProUGUI handle_CurrentRank;
    public TextMeshProUGUI handle_NewRank;
    public Image handle_CurrentRankImg;
    public Image handle_NewRankImg;
    [Header("Launch Game")]
    public UIButton launchGameBtn;
    public UILoopPresentationAnim loopPresentationAnim;
    [Header("Callbacks")]
    public UnityEvent OnBeforeLoopDepth;
    public UIButton skipAnimBtn;
    // Internals
    bool InitDone = false;
    AnimationManager ANIM;
    AudioManager AUDIO;
    MiniGameManager MGM;

    public void Init(GameManager iGameManager)
    {
        ANIM = iGameManager.ANIM;
        AUDIO = iGameManager.AUDIO;
        MGM = iGameManager.MGM;
        miniGameClock.text = GameData.GetSettings.MiniGameTime.ToString("#0");
        hpClock.text = GameData.GetSettings.PlayerHP.ToString("#0.0");
        timeIndicatorImg.color = frozenTimeColor;
        successTimePositiveColor = GameData.GetSettings.LoopPassedColor;
        successTimeNegativeColor = GameData.GetSettings.LoopFailedColor;
        timeNeedleAnim.Init(MGM.gameClock);
        
        ShowMiniGameMode(false);
        //ShowSuccessArea(false);

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

        //handle_animLoopSuccess.passedTextColor = successTimePositiveColor;
        //handle_animLoopSuccess.failedTextColor = successTimeNegativeColor;
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



    public void ShowMiniGameMode(bool iState)
    {
        miniGameClock.enabled = iState;
        hpClock.enabled = iState;
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

    public async Task LoopCompleteAnim(MiniGameLoop iMGLoop, CancellationToken iCT)
    {
        MiniGameSuccessState[] arr_states = iMGLoop.GetSuccessStates();
        Color[] colors = new Color[arr_states.Length];
        for (int i = 0; i < arr_states.Length; i++)
        {
            colors[i] = (arr_states[i] == MiniGameSuccessState.PASSED) ? successTimePositiveColor : successTimeNegativeColor;
        }

        handle_animLoopSuccess.Init(colors, iMGLoop);
        handle_animLoopSuccess.OnBeforeLoopDepth = new UnityEvent();
        handle_animLoopSuccess.OnBeforeLoopDepth.AddListener(()=>OnBeforeLoopDepth?.Invoke());

        float newRank = (float)iMGLoop.rank;
        float prevRank = iMGLoop.rank > 0 ? newRank - 1f : 0f;
        handle_animLoopSuccess.OnNewRankDisplayedCB = new UnityEvent();
        handle_animLoopSuccess.OnNewRankDisplayedCB.AddListener(
            () => { AUDIO.LerpRank(prevRank, newRank); }
            );

        ANIM.TrackAnimator(handle_animLoopSuccess.animator, iCT);
        ANIM.QueueAnimRange(handle_animLoopSuccess.animator, handle_animLoopSuccess.GetAnimQueue(iCT));
        await ANIM.PlayAnim(handle_animLoopSuccess.animator);
        ANIM.StopTrackAnimator(handle_animLoopSuccess.animator);
        //await handle_animLoopSuccess.Animate(colors, iLoopPassed, iRankUp, iLoopDepth, iCT);

        handle_animLoopSuccess.OnBeforeLoopDepth.RemoveListener(()=>OnBeforeLoopDepth?.Invoke());
        handle_animLoopSuccess.OnNewRankDisplayedCB.RemoveListener(()=> { AUDIO.LerpRank(prevRank, newRank); });
    }
}
