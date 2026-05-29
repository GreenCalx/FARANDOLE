using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using static AsyncUtils;
public class UILoopCompleteAnimation : ManagedAnimation, IAnimationQueue
{
    public const string LoopAnimSkipTrigger = "LoopSkip";
    public const string LoopPassedTrigger = "LoopPassed";
    public const string LoopHideTrigger = "Hide";
    public const string LoopShowRankTrigger = "ShowRank";
    public const string LoopShowDepthTrigger = "ShowLoopDepth";
    public const string LoopHideDepthTrigger = "HideLoopDepth";
    public const string LoopShowSuccessTrigger = "ShowSuccess";
    public const string LoopRankUpBoolParm = "RankingUp";
    public const string LoopPassedAnimStateName = "OnLoopPassed";
    public const string LoopHideAnimStateName = "OnLoopHide";
    public const string LoopHideFromRankUpAnimStateName = "OnLoopHideFromRankUp";
    public const string LoopShowkStateName = "OnLoopShow";
    public const string LoopRankUpStateName = "OnLoopRankUp";
    public const string LoopDepthStateName = "OnLoopDepth";
    public const string LoopShowSuccessStateName = "OnLoopShowSuccess";
    public const string LoopIdleStateName = "IDLE";

    [Header("LoopPassed Text")]
    public const string OnPerfectTextValue = "PERFECT";
    public const string OnPassedTextValue = "PASSED";
    public const string OnFailedTextValue = "FAILED";
    [Header("References")]
    public UITextFaderAnimation loopPassedTextAnim;
    public TextMeshProUGUI loopPassedTxt;
    [Header("Combo References")]
    public RectTransform handle_ComboBarLayout;
    public GameObject prefab_UIComboPointContainer;
    public List<UIComboPoint> m_ComboPoints;
    [Header("Loop Depth")]
    public TextMeshProUGUI loopDepthValueTxt;
    [Header("Callbacks")]
    public UnityEvent OnBeforeLoopDepth;
    public UnityEvent OnLoopResultProcessedCB;
    public UnityEvent OnNewRankDisplayedCB;
    public UnityEvent OnFinalLoopDisplayedCB;
    // internals
    [Header("Internals")]
    MiniGameLoop MGLoop;
    MiniGameLoopSnapshot previousMGLoop;
    public UILoopPresentationAnim loopPresentationAnim;
    public bool PresentationShown => loopPresentationAnim.PresentationShown;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        cancellationTokenSource = new CancellationTokenSource();
        loopPassedTxt.color = new Color
        (
            loopPassedTxt.color.r,
            loopPassedTxt.color.g,
            loopPassedTxt.color.b,
            0
        );
        loopPassedTxt.ForceMeshUpdate();
        InitComboPoints();
    }

    public void Skip()
    {
        m_Animator.SetTrigger(LoopAnimSkipTrigger);
        if (inst_SkipHandler != null)
            GameObject.Destroy(inst_SkipHandler.gameObject);
    }

    void InitComboPoints()
    {
        if (m_ComboPoints!=null && m_ComboPoints.Count >= GameData.GetSettings.MaxComboPoints)
        {
            PreviousComboPoints(cancellationTokenSource.Token);
            return;
        }

        foreach( Transform child in handle_ComboBarLayout.transform)
            GameObject.Destroy(child.gameObject);

        m_ComboPoints = new List<UIComboPoint>(GameData.GetSettings.MaxComboPoints);
        for (int i=0; i < GameData.GetSettings.MaxComboPoints; i++)
        {
            m_ComboPoints.Add(GOBuilder.Create(prefab_UIComboPointContainer)
                                    .WithName("ComboPoint " + i)
                                    .WithParent(handle_ComboBarLayout)
                                    .WithLocalPosition(Vector3.zero)
                                    .WithLocalScale(Vector3.one)
                                    .BuildAs<UIComboPoint>());
            RectTransform as_rt = m_ComboPoints[i].GetComponent<RectTransform>();
            handle_ComboBarLayout.sizeDelta += new Vector2(as_rt.rect.width, 0f);
        }
        PreviousComboPoints(cancellationTokenSource.Token);
    }

    void PreviousComboPoints(CancellationToken iCT)
    {
        if (previousMGLoop==null)
            return;

        for (int i=0; i < m_ComboPoints.Count; i++)
        {
            if (previousMGLoop.comboMultiplier > i)
            {
                m_ComboPoints[i].DefaultShow(iCT);
                continue;
            }
            m_ComboPoints[i].DefaultHide(iCT);
        }
    }
    void RefreshComboPoints(CancellationToken iCT)
    {
        for (int i=0; i < m_ComboPoints.Count; i++)
        {
            if (MGLoop.combo > i)
            {
                m_ComboPoints[i].DefaultShow(iCT);
                continue;
            }
            m_ComboPoints[i].DefaultHide(iCT);
        }
    }

    void HideComboPoints(CancellationToken iCT)
    {
        for (int i=0; i < m_ComboPoints.Count; i++)
        {
            m_ComboPoints[i].DefaultHide(iCT);
        }
    }

    public void Init(MiniGameLoopSnapshot iFinishedMGLoop, MiniGameLoop iNextMGLoop, UILoopPresentationAnim iLoopPresentationAnim)
    {
        MGLoop = iNextMGLoop;
        previousMGLoop = iFinishedMGLoop;

        // Depth
        loopDepthValueTxt.text = MGLoop.depth.ToString();

        // Combo
        //loopComboTxt.text =  "X" + MGLoop.previousCombo.ToString();
        InitComboPoints();
        
        loopPresentationAnim = iLoopPresentationAnim;
        //if (GameData.Get.currentGameMode == GAME_MODE.DAILY_SEED)
            loopPresentationAnim.DisableThumbnailButtons();
        loopPresentationAnim.ResetLights();

        // Init ani text
        if (iFinishedMGLoop.IsPerfect)
        {
            loopPassedTxt.text = OnPerfectTextValue;
            loopPassedTxt.color = new Color
            (
                GameData.GetSettings.LoopPefectColor.r,
                GameData.GetSettings.LoopPefectColor.g,
                GameData.GetSettings.LoopPefectColor.b,
                0
            );
        }
        else if (iFinishedMGLoop.IsFailed)
        {
            loopPassedTxt.text = OnFailedTextValue;
            loopPassedTxt.color = new Color
            (
                GameData.GetSettings.LoopFailedColor.r,
                GameData.GetSettings.LoopFailedColor.g,
                GameData.GetSettings.LoopFailedColor.b,
                0
            );
        }
        else
        {
            loopPassedTxt.text = OnPassedTextValue;
            loopPassedTxt.color = new Color
            (
                GameData.GetSettings.LoopPassedColor.r,
                GameData.GetSettings.LoopPassedColor.g,
                GameData.GetSettings.LoopPassedColor.b,
                0
            );
        }
        loopPassedTxt.ForceMeshUpdate(true);
        loopPassedTextAnim.Bake();
    }
    public Queue<Func<UniTask>> GetAnimQueue(CancellationToken iCT)
    {
        Queue<Func<UniTask>> q = new Queue<Func<UniTask>>();

        CancellationTokenRegistration ctr = iCT.Register(() => Skip());

        UnityEvent cancelCB = new UnityEvent();
        cancelCB.AddListener(() => OnBeforeLoopDepth?.Invoke());
        if (MGLoop.IsRankUpdateRequested)
            cancelCB.AddListener(() => OnNewRankDisplayedCB?.Invoke());
        cancelCB.AddListener(()=>OnLoopResultProcessedCB?.Invoke());
        
        // Animation loop
        Func<UniTask> step1 = async () =>
        {
            await UniTask.SwitchToMainThread();
            inst_SkipHandler = GOBuilder.Create(prefab_LongTapListener)
                                        .WithParent(transform)
                                        .BuildAs<LongTapListener>();
            inst_SkipHandler.m_OnTapEvent.AddListener( () => Skip() );

            m_Animator.SetBool(LoopRankUpBoolParm, MGLoop.IsRankUpdateRequested);
            m_Animator.SetTrigger(LoopPassedTrigger);
            await WaitMainAnimTask(1f, iCT); // full anim
            if (iCT.IsCancellationRequested)
            {
                cancelCB.Invoke();
                return;
            }
            m_Animator.SetTrigger(LoopShowRankTrigger);
        };
        q.Enqueue(step1);

        Func<UniTask> step2 = async () =>
        {
            await UniTask.SwitchToMainThread();
            cancelCB.AddListener(() => loopPresentationAnim.Cancel());
            InitComboPoints();
            // Show Presentation Loop
            await
            UniTask.WhenAll(
                loopPresentationAnim.Show(MGLoop, iCT),
                WaitShowLoopAnimTask(1f, iCT)
                );
            if (iCT.IsCancellationRequested)
            {
                cancelCB.Invoke();
                return;
            }

            // Show Thumbnail Lights and fill power bar
            
            await loopPresentationAnim.ShowLights(MGLoop, true, iCT);
            if (iCT.IsCancellationRequested)
            {
                cancelCB.Invoke();
                return;
            }
            
            m_Animator.SetTrigger(LoopShowSuccessTrigger);
            
        };
        q.Enqueue(step2);

        // Show loop result canvas
        Func<UniTask> step3 = async () =>
        {
            await UniTask.SwitchToMainThread();

            CancellationTokenSource ctsTextAnim = new CancellationTokenSource();

            loopPresentationAnim.rankMedalAnimation.AnimateShake();
            loopPassedTextAnim.Animate(ctsTextAnim.Token, iCT);
            
            await WaitShowSuccessAnimTask(0.5f, iCT);

            RefreshComboPoints(iCT);

            await WaitShowSuccessAnimTask(1f, iCT);
            
            //ctsTextAnim.Cancel();

            if (iCT.IsCancellationRequested)
            {
                cancelCB.Invoke();
                return;
            }
            OnLoopResultProcessedCB?.Invoke();
            await UniTask.Delay(GameData.GetSettings.PostShowLoopResultDelayInMs);
        };
        q.Enqueue(step3);

        // Rank Up/Down
        if (MGLoop.IsRankUpdateRequested)
        {
            Func<UniTask> step4 = async () =>
            {
                await UniTask.SwitchToMainThread();
                await loopPresentationAnim.rankMedalAnimation.RankUp(iCT);

                cancelCB.RemoveListener(() => OnNewRankDisplayedCB?.Invoke());
                OnNewRankDisplayedCB?.Invoke();
                if (iCT.IsCancellationRequested)
                {
                    cancelCB.Invoke();
                    return;
                }

                loopPresentationAnim.rankMedalAnimation.AnimateNewRankShine();
                await UniTask.Delay(GameData.GetSettings.PostShowNewRankDelayInMs);

            };
            q.Enqueue(step4);
        }

        // TODO Mutation & mods here

        // Hide Loop Complete UI
        Func<UniTask> step6 = async () =>
        {
            await UniTask.SwitchToMainThread();
            m_Animator.SetTrigger(LoopHideTrigger);
            HideComboPoints(iCT);
            await UniTask.WhenAll(
                loopPresentationAnim.Hide(iCT),
                UniTask.WhenAny(
                    WaitHideFromRankUpAnimTask(0.75f, iCT),
                    WaitHideAnimTask(0.75f, iCT)
                )
            );
            
            cancelCB.RemoveListener(() => loopPresentationAnim.Cancel());

            if (MGLoop.IsFinalLoop)
            {
                OnFinalLoopDisplayedCB?.Invoke();
                return;
            }
            
            if (iCT.IsCancellationRequested)
            {
                cancelCB.Invoke(); return;
            }
            m_Animator.SetTrigger(LoopShowDepthTrigger);
        };
        q.Enqueue(step6);

        // Show new Loop depth introduciton
        Func<UniTask> step7 = async () =>
        {
            await UniTask.SwitchToMainThread();
            OnBeforeLoopDepth?.Invoke();
            await WaitShowLoopDepth(1f, iCT);
            if (iCT.IsCancellationRequested)
            { return; }
            m_Animator.SetTrigger(LoopHideDepthTrigger);
        };
        q.Enqueue(step7);

        // Hide Loop Depth Introduction and finish animation
        Func<UniTask> step8 = async () =>
        {
            await UniTask.SwitchToMainThread();
            if (inst_SkipHandler != null)
                GameObject.Destroy(inst_SkipHandler.gameObject);
            await WaitBackToIdle(1f, iCT);
        };
        q.Enqueue(step8);

        return q;
    }

    async UniTask WaitShowLoopDepth(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopDepthStateName, iCompletionFrac, iCT);
    }

    async UniTask WaitBackToIdle(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopIdleStateName, iCompletionFrac, iCT);
    }

    async UniTask WaitRankUpAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopRankUpStateName, iCompletionFrac, iCT);
    }

    async UniTask WaitShowLoopAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopShowkStateName, iCompletionFrac, iCT);
    }

    async UniTask WaitHideAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopHideAnimStateName, iCompletionFrac, iCT);
    }
    async UniTask WaitHideFromRankUpAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopHideFromRankUpAnimStateName, iCompletionFrac, iCT);
    }

    async UniTask WaitMainAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopPassedAnimStateName, iCompletionFrac, iCT);
    }
    async UniTask WaitShowSuccessAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopShowSuccessStateName, iCompletionFrac, iCT);
    }
    
}
