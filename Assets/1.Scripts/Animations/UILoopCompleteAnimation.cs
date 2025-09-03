using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TMPro;

public class UILoopCompleteAnimation : MonoBehaviour
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
    public const string LoopDepthHideStateName = "OnLoopDepthHide";
    public List<Image> lightImages;
    public Animator animator;
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
    public CancellationTokenSource cancellationTokenSource;
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
    public async Task Animate(Color[] iLightColors, bool iLoopPassed, bool iRankUp, int iLoopDepth, CancellationToken iCT)
    {
        CancellationTokenRegistration ctr = iCT.Register(() => Skip());

        for (int i = 0; i < iLightColors.Length; i++)
        {
            if (i >= lightImages.Count)
                break;
            lightImages[i].color = iLightColors[i];
        }
        loopDepthValueTxt.text = iLoopDepth.ToString();
        animator.SetTrigger(LoopPassedTrigger);
        animator.SetBool(LoopRankUpBoolParm, iRankUp);

        await WaitMainAnimTask(1f, iCT); // full anim
        if (iCT.IsCancellationRequested)
        { OnBeforeLoopDepth?.Invoke(); return; }

        await AnimateTextTask(iLoopPassed, iCT);
        if (iCT.IsCancellationRequested)
        { OnBeforeLoopDepth?.Invoke(); return; }

        ShowRank();
        await WaitShowRankAnimTask(1f, iCT);
        if (iCT.IsCancellationRequested)
        { OnBeforeLoopDepth?.Invoke(); return; }

        if (iRankUp)
        {
            await WaitRankUpAnimTask(1f, iCT);
            if (iCT.IsCancellationRequested)
            { OnBeforeLoopDepth?.Invoke(); return; }
        }

        await Task.Delay(GameData.GetSettings.LoopCompleteAfterAnimDisplayTimeMs);
        if (iCT.IsCancellationRequested)
        { Skip(); return; }
        Hide();

        if (iRankUp)
            await WaitHideFromRankUpAnimTask(0.5f, iCT); // half anim
        else
            await WaitHideAnimTask(0.5f, iCT); // half anim
        if (iCT.IsCancellationRequested)
        { OnBeforeLoopDepth?.Invoke(); return; }

        OnBeforeLoopDepth?.Invoke();

        await WaitShowLoopDepth(1f, iCT);
        if (iCT.IsCancellationRequested)
        { return; }

        animator.SetTrigger(LoopHideDepthTrigger);
        await WaitHideLoopDepth(1f, iCT);
    }
    public void ShowRank()
    {
        animator.SetTrigger(LoopShowRankTrigger);
    }
    public void Hide()
    {
        animator.SetTrigger(LoopHideTrigger);
    }

    async Task WaitAnimState(string iStateName, float iCompletionFrac, CancellationToken iCT)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(iStateName))
        {
            if (iCT.IsCancellationRequested)
                return;
            await Task.Yield();
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac)
        {
            if (iCT.IsCancellationRequested)
                return;
            await Task.Yield();
        }
    }

    async Task WaitShowLoopDepth(float iCompletionFrac, CancellationToken iCT)
    {
        // while (!animator.GetCurrentAnimatorStateInfo(0).IsName(LoopDepthStateName))
        // { await Task.Yield(); }
        // while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac)
        // { await Task.Yield(); }

        await WaitAnimState(LoopDepthStateName, iCompletionFrac, iCT);
    }

    async Task WaitHideLoopDepth(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopDepthHideStateName, iCompletionFrac, iCT);
    }

    async Task WaitRankUpAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopRankUpStateName, iCompletionFrac, iCT);
    }

    async Task WaitShowRankAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopShowRankStateName, iCompletionFrac, iCT);
    }

    async Task WaitHideAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopHideAnimStateName, iCompletionFrac, iCT);
    }
    async Task WaitHideFromRankUpAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopHideFromRankUpAnimStateName, iCompletionFrac, iCT);
    }

    async Task WaitMainAnimTask(float iCompletionFrac, CancellationToken iCT)
    {
        await WaitAnimState(LoopPassedAnimStateName, iCompletionFrac, iCT);
    }
    async Task AnimateTextTask(bool iLoopPassed, CancellationToken iCT)
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
            await Task.Delay(25 - FadeSpeed);
        }
        Debug.Log("Text displayed");
    }

}
