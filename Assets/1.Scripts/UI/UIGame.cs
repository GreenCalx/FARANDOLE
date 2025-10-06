using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using TMPro;

public class UIGame : MonoBehaviour, IManager, IDynamicUI
{
    [Header("UI Spaces")]
    public RectTransform OverlaySpace;
    public RectTransform CameraSpace;
    [Header("Player UI")]
    public TextMeshProUGUI miniGameClock;
    public Image m_HPImage;
    public Image timeIndicatorImg;
    public RotateAlongTimeAnim timeNeedleAnim;
    public RectTransform infoArea;
    public UIDoorAnim handle_UIDoorAnim;
    public UILoopCompleteAnimation handle_animLoopSuccess; 

    [Header("Success")]
    public UIStageClearAnimation handle_animStageClear;
    public TextMeshProUGUI successTimeTxt;
    [Header("Loop Presentation")]
    public GameObject prefab_loopPresentationAnim;
    public UILoopPresentationAnim inst_loopPresentationAnim;

    [Header("Launch Game")]
    public UIButton launchGameBtn;
    [Header("Pause Menu")]
    public UIPauseMenu h_PauseMenu;
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
        h_PauseMenu.PC = iGameManager.PC;
        miniGameClock.text = GameData.GetSettings.MiniGameTime.ToString("#0");

        m_HPImage.color = GameData.GetSettings.LoopPassedColor;
        m_HPImage.fillAmount = 1f;

        timeIndicatorImg.color = GameData.GetUITheme.FrozenTimeColor;
        GameData.GetUITheme.PositiveTimeColor = GameData.GetSettings.LoopPassedColor;
        GameData.GetUITheme.NegativeTimeColor = GameData.GetSettings.LoopFailedColor;
        timeNeedleAnim.Init(MGM.gameClock);

        inst_loopPresentationAnim = GOBuilder.Create(prefab_loopPresentationAnim)
                                    .WithParent(OverlaySpace)
                                    .WithAnchoredPosition(Vector3.zero)
                                    .BuildAs<UILoopPresentationAnim>();
        inst_loopPresentationAnim.Init(MGM.MGLoop);

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

        //handle_animLoopSuccess.passedTextColor = GameData.GetUITheme.PositiveTimeColor;
        //handle_animLoopSuccess.failedTextColor = GameData.GetUITheme.NegativeTimeColor;
    }

    public void RefreshHPImage(PlayerData iData)
    {
        float hp_frac = iData.HP / GameData.GetSettings.PlayerHP;
        m_HPImage.fillAmount = hp_frac;
        m_HPImage.color = Color.Lerp(GameData.GetSettings.LoopPassedColor, GameData.GetSettings.LoopFailedColor, 1f - hp_frac);
    }

    public void RefreshTimeIndicator(GameClock iGameClock)
    {
        if (iGameClock.IsFrozen)
        {
            timeIndicatorImg.color = GameData.GetUITheme.FrozenTimeColor;
        }
        else if ((iGameClock.GetRemainingTime() <= 1f) && (iGameClock.GetRemainingTime() > 0f))
        {
            timeIndicatorImg.color = GameData.GetUITheme.LastSecondTimeColor;
        }
        else if (iGameClock.MiniGameTimeExpired())
        {
            timeIndicatorImg.color = GameData.GetUITheme.NegativeTimeColor;
        }
        else
        {
            timeIndicatorImg.color = GameData.GetUITheme.PositiveTimeColor;
        }
    }


    public async UniTask PresentLoop()
    {
        launchGameBtn.gameObject.SetActive(false);
        await inst_loopPresentationAnim.Show(MGM.MGLoop);
        launchGameBtn.gameObject.SetActive(true);
    }

    public async UniTask HideLoopPresentation()
    {
        await inst_loopPresentationAnim.Hide();
    }

    public void ShowMiniGameMode(bool iState)
    {
        miniGameClock.enabled = iState;
        m_HPImage.enabled = iState;
        handle_UIDoorAnim.OpenAnim();
    }

    public void ShowSuccessArea(float iTime = 0f)
    {
        //successArea.gameObject.SetActive(iState);
        string successTimeStr = "";
        if (iTime >= 0f)
        {
            successTimeTxt.color = GameData.GetUITheme.PositiveTimeColor;
            successTimeStr += "+";
        }
        else
        {
            successTimeTxt.color = GameData.GetUITheme.NegativeTimeColor;
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
        handle_animLoopSuccess.Init(iMGLoop, inst_loopPresentationAnim);
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
