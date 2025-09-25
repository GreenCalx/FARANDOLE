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
    public UITextFaderAnimation loopPassedTextAnim;
    public TextMeshProUGUI loopPassedTxt;
    [Header("Loop Depth")]
    public TextMeshProUGUI loopDepthValueTxt;
    [Header("Callbacks")]
    public UnityEvent OnBeforeLoopDepth;
    public UnityEvent SkipAnimCB;
    public UnityEvent OnNewRankDisplayedCB;

    // internals
    MiniGameLoop MGLoop;
    UILoopPresentationAnim loopPresentationAnim;

    void Start()
    {
        animator = GetComponent<Animator>();

        loopPassedTxt.color = new Color
        (
            loopPassedTxt.color.r,
            loopPassedTxt.color.g,
            loopPassedTxt.color.b,
            0
        );
        loopPassedTxt.ForceMeshUpdate();
    }

    public void Skip()
    {
        animator.SetTrigger(LoopAnimSkipTrigger);
    }

    public void Init(MiniGameLoop iMGLoop, UILoopPresentationAnim iLoopPresentationAnim)
    {
        loopDepthValueTxt.text = iMGLoop.depth.ToString();
        MGLoop = iMGLoop;
        loopPresentationAnim = iLoopPresentationAnim;

        // Init ani text
        if (iMGLoop.IsLoopPerfect())
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
        else if (!iMGLoop.IsLoopPassed())
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

        // Animation loop
        Func<UniTask> step1 = async () =>
        {
            await UniTask.SwitchToMainThread();
            animator.SetBool(LoopRankUpBoolParm, MGLoop.IsRankUpdateRequested);
            animator.SetTrigger(LoopPassedTrigger);
            await WaitMainAnimTask(1f, iCT); // full anim
            if (iCT.IsCancellationRequested)
            {
                OnBeforeLoopDepth?.Invoke();
                if (MGLoop.IsRankUpdateRequested)
                    OnNewRankDisplayedCB?.Invoke();
                return;
            }
            animator.SetTrigger(LoopShowRankTrigger);
        };
        q.Enqueue(step1);

        Func<UniTask> step2 = async () =>
        {
            await UniTask.SwitchToMainThread();
            await loopPresentationAnim.Show(MGLoop);
            await loopPresentationAnim.ShowLights(MGLoop, true);
            await WaitShowLoopAnimTask(1f, iCT);
            if (iCT.IsCancellationRequested) 
            {
                OnBeforeLoopDepth?.Invoke();
                if (MGLoop.IsRankUpdateRequested)
                    OnNewRankDisplayedCB?.Invoke();
                return;
            }
            animator.SetTrigger(LoopShowSuccessTrigger);
            
        };
        q.Enqueue(step2);

        Func<UniTask> step3 = async () =>
        {
            await UniTask.SwitchToMainThread();

            CancellationTokenSource ctsTextAnim = new CancellationTokenSource();
            loopPassedTextAnim.Animate(ctsTextAnim.Token, iCT);
            await WaitShowSuccessAnimTask(1f, iCT);
            ctsTextAnim.Cancel();

            if (iCT.IsCancellationRequested)
            {
                OnBeforeLoopDepth?.Invoke();
                if (MGLoop.IsRankUpdateRequested)
                    OnNewRankDisplayedCB?.Invoke();
                return;
            }
        };
        q.Enqueue(step3);

        if (MGLoop.IsRankUpdateRequested)
        {
            Func<UniTask> step4 = async () =>
            {
                await UniTask.SwitchToMainThread();
                //await WaitRankUpAnimTask(1f, iCT);
                await loopPresentationAnim.rankMedalAnimation.RankUp(iCT);
                OnNewRankDisplayedCB?.Invoke();
                if (iCT.IsCancellationRequested)
                {
                    OnBeforeLoopDepth?.Invoke();
                    return;
                }
            };
            q.Enqueue(step4);
        }

        Func<UniTask> step5 = async () =>
        {
            await Task.Delay(GameData.GetSettings.LoopCompleteAfterAnimDisplayTimeMs);
            await UniTask.SwitchToMainThread();
            animator.SetTrigger(LoopHideTrigger);
            if (iCT.IsCancellationRequested)
            { OnBeforeLoopDepth?.Invoke(); return; }
        };
        q.Enqueue(step5);

        Func<UniTask> step6 = async () =>
        {
            await UniTask.SwitchToMainThread();
            await loopPresentationAnim.Hide();
            await UniTask.WhenAny(
                WaitHideFromRankUpAnimTask(1f, iCT),
                WaitHideAnimTask(1f, iCT)
            );
            animator.SetTrigger(LoopShowDepthTrigger);
            if (iCT.IsCancellationRequested)
            { OnBeforeLoopDepth?.Invoke(); return; }
        };
        q.Enqueue(step6);

        Func<UniTask> step7 = async () =>
        {
            await UniTask.SwitchToMainThread();
            OnBeforeLoopDepth?.Invoke();
            await WaitShowLoopDepth(1f, iCT);
            if (iCT.IsCancellationRequested)
            { return; }
            animator.SetTrigger(LoopHideDepthTrigger);
        };
        q.Enqueue(step7);

        Func<UniTask> step8 = async () =>
        {
            await UniTask.SwitchToMainThread();
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
