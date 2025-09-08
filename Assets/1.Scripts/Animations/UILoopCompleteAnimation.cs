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

public class UILoopCompleteAnimation : ManagedAnimation, IAnimationQueue
{
    public const string LoopAnimSkipTrigger = "LoopSkip";
    public const string LoopPassedTrigger = "LoopPassed";
    public const string LoopHideTrigger = "Hide";
    public const string LoopShowRankTrigger = "ShowRank";
    public const string LoopHideDepthTrigger = "HideLoopDepth";
    public const string LoopRankUpBoolParm = "RankingUp";
    public const string LoopPassedAnimStateName = "OnLoopPassed";
    public const string LoopHideAnimStateName = "OnLoopHide";
    public const string LoopHideFromRankUpAnimStateName = "OnLoopHideFromRankUp";
    public const string LoopShowRankStateName = "OnLoopShowRank";
    public const string LoopRankUpStateName = "OnLoopRankUp";
    public const string LoopDepthStateName = "OnLoopDepth";
    public const string LoopIdleStateName = "IDLE";
    public List<Image> lightImages;

    [Header("LoopPassed Text")]
    public const string OnPassedTextValue = "PASSED";
    public const string OnFailedTextValue = "FAILED";
    public Color passedTextColor;
    public Color failedTextColor;
    public TextMeshProUGUI loopPassedTxt;
    public TextMeshProUGUI loopDepthValueTxt;
    public int RolloverCharacterSpread = 10;
    public int FadeSpeed = 20;
    [Header("Callbacks")]
    public UnityEvent OnBeforeLoopDepth;
    public UnityEvent SkipAnimCB;

    // internals
    bool LoopPassed = false;
    bool RankUp = false;

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

    public void Init(Color[] iLightColors, bool iLoopPassed, bool iRankUp, int iLoopDepth)
    {
        for (int i = 0; i < iLightColors.Length; i++)
        {
            if (i >= lightImages.Count)
                break;
            lightImages[i].color = iLightColors[i];
        }
        loopDepthValueTxt.text = iLoopDepth.ToString();
        LoopPassed = iLoopPassed;
        RankUp = iRankUp;
    }
    public Queue<Func<UniTask>> GetAnimQueue(CancellationToken iCT)
    {
        Queue<Func<UniTask>> q = new Queue<Func<UniTask>>();

        CancellationTokenRegistration ctr = iCT.Register(() => Skip());

        // Animation loop
        Func<UniTask> step1 = async () =>
        {
            await UniTask.SwitchToMainThread();
            animator.SetTrigger(LoopPassedTrigger);
            animator.SetBool(LoopRankUpBoolParm, RankUp);
            await WaitMainAnimTask(1f, iCT); // full anim
            if (iCT.IsCancellationRequested)
            { OnBeforeLoopDepth?.Invoke(); return; }
        };
        q.Enqueue(step1);

        Func<UniTask> step2 = async () =>
        {
            await UniTask.SwitchToMainThread();
            await AnimateTextTask(LoopPassed, iCT);
            if (iCT.IsCancellationRequested)
            { OnBeforeLoopDepth?.Invoke(); return; }
        };
        q.Enqueue(step2);
        
        Func<UniTask> step3 = async () =>
        {
            await UniTask.SwitchToMainThread();
            animator.SetTrigger(LoopShowRankTrigger);
            await WaitShowRankAnimTask(1f, iCT);
            if (iCT.IsCancellationRequested)
            { OnBeforeLoopDepth?.Invoke(); return; }
        };
        q.Enqueue(step3);

        if (RankUp)
        {
            Func<UniTask> step4 = async () =>
            {
                await UniTask.SwitchToMainThread();
                await WaitRankUpAnimTask(1f, iCT);
                if (iCT.IsCancellationRequested)
                { OnBeforeLoopDepth?.Invoke(); return; }
            };
            q.Enqueue(step4);
        }

        Func<UniTask> step5 = async () =>
        {
            await UniTask.SwitchToMainThread();
            await Task.Delay(GameData.GetSettings.LoopCompleteAfterAnimDisplayTimeMs);
            if (iCT.IsCancellationRequested)
            { OnBeforeLoopDepth?.Invoke(); return; }
        };
        q.Enqueue(step5);

        Func<UniTask> step6 = async () =>
        {
            await UniTask.SwitchToMainThread();
            animator.SetTrigger(LoopHideTrigger);
            if (RankUp)
                await WaitHideFromRankUpAnimTask(0.5f, iCT); // half anim
            else
                await WaitHideAnimTask(0.5f, iCT); // half anim
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
        };
        q.Enqueue(step7);

        Func<UniTask> step8 = async () =>
        {
            await UniTask.SwitchToMainThread();
            animator.SetTrigger(LoopHideDepthTrigger);
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

    async UniTask WaitShowRankAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopShowRankStateName, iCompletionFrac, iCT);
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
    async UniTask AnimateTextTask(bool iLoopPassed, CancellationToken iCT)
    {
        // make transparent
        loopPassedTxt.text = iLoopPassed ? OnPassedTextValue : OnFailedTextValue;
        loopPassedTxt.color = new Color
        (
            iLoopPassed ? passedTextColor.r : failedTextColor.r,
            iLoopPassed ? passedTextColor.g : failedTextColor.g,
            iLoopPassed ? passedTextColor.b : failedTextColor.b,
            0
        );

        loopPassedTxt.ForceMeshUpdate();

        // fade in text
        TMP_TextInfo textInfo = loopPassedTxt.textInfo;
        Color32[] newVertexColors;
        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool endReached = false;
        int characterCount = textInfo.characterCount;

        byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);
        Debug.Log(fadeSteps);
        while (!endReached)
        {
            if (iCT.IsCancellationRequested)
                return;
            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps, 0, 255);
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;
                Debug.Log(alpha);
                if (alpha == 255)
                {
                    startingCharacterRange++;
                    if (startingCharacterRange == characterCount)
                    {
                        currentCharacter = 0;
                        startingCharacterRange = 0;

                        endReached = true;
                    }
                }
            }
            loopPassedTxt.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            if (currentCharacter + 1 < characterCount)
                currentCharacter++;
            Debug.Log(currentCharacter);
            await UniTask.Delay(25 - FadeSpeed);
        }
    }

}
